using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PrefabViewer.Editor
{
    /// <summary>
    /// Patches io.z3y.github.shadergraph HLSL for GLES3/WebGL (ShadowCaster Metallic, SSR BlueNoise).
    /// </summary>
    static class Z3yShaderGraphWebGlFix
    {
        const string PackageId = "io.z3y.github.shadergraph";
        const string EmbeddedShadowCaster = "Assets/Editor/Z3yPatches/FragmentShadowCaster.hlsl";

        const string MetallicLine =
            "surfaceDescription.Alpha = lerp(surfaceDescription.Alpha, 1.0, surfaceDescription.Metallic);";

        const string MetallicLineFixed =
            "surfaceDescription.Alpha = lerp(surfaceDescription.Alpha, 1.0, 0.0);";

        const string SsrGuardBlock =
            "#if (defined(SHADER_API_GLES3) || defined(SHADER_API_WEBGL)) && defined(_SSR)\n#undef _SSR\n#endif";

        [InitializeOnLoadMethod]
        static void OnLoad()
        {
            EditorApplication.delayCall += () => ApplyPatch(logIfChanged: false);
        }

        [MenuItem("Prefab Viewer/Apply z3y WebGL Shader Fix")]
        static void ApplyFromMenu()
        {
            var ok = ApplyPatch();
            ReimportShaderGraphs();
            EditorUtility.DisplayDialog("z3y WebGL fix",
                ok
                    ? "Patch applied. ShaderGraph assets reimported."
                    : "Patch was already applied or package not found. ShaderGraph assets reimported.",
                "OK");
        }

        public static bool ApplyPatch(bool logIfChanged = true)
        {
            var packageRoot = FindPackageRoot();
            if (packageRoot == null)
            {
                if (logIfChanged)
                    Debug.LogWarning("[Z3yShaderGraphWebGlFix] Package not found in Library/PackageCache.");
                return false;
            }

            var changed = false;
            changed |= PatchShadowCaster(Path.Combine(packageRoot, "ShaderLibrary/FragmentShadowCaster.hlsl"));
            changed |= PatchForwardLighting(Path.Combine(packageRoot, "ShaderLibrary/ForwardLighting.hlsl"));

            if (changed && logIfChanged)
                Debug.Log("[Z3yShaderGraphWebGlFix] Applied WebGL/GLES3 patches to z3y ShaderGraph package.");
            else if (logIfChanged && IsPackagePatched(packageRoot))
                Debug.Log("[Z3yShaderGraphWebGlFix] Package already patched.");

            return changed || IsPackagePatched(packageRoot);
        }

        static bool IsPackagePatched(string packageRoot)
        {
            var shadow = File.ReadAllText(Path.Combine(packageRoot, "ShaderLibrary/FragmentShadowCaster.hlsl"));
            var forward = File.ReadAllText(Path.Combine(packageRoot, "ShaderLibrary/ForwardLighting.hlsl"));
            return shadow.Contains(MetallicLineFixed) && forward.Contains(SsrGuardBlock);
        }

        static bool PatchShadowCaster(string targetPath)
        {
            if (!File.Exists(EmbeddedShadowCaster))
            {
                Debug.LogWarning("[Z3yShaderGraphWebGlFix] Missing embedded FragmentShadowCaster.hlsl");
                return PatchShadowCasterInline(targetPath);
            }

            var embedded = File.ReadAllText(EmbeddedShadowCaster);
            if (File.Exists(targetPath) && File.ReadAllText(targetPath) == embedded)
                return false;

            File.WriteAllText(targetPath, embedded);
            return true;
        }

        static bool PatchShadowCasterInline(string path)
        {
            if (!File.Exists(path))
                return false;

            var text = File.ReadAllText(path);
            if (text.Contains(MetallicLineFixed) || !text.Contains(MetallicLine))
                return false;

            File.WriteAllText(path, text.Replace(MetallicLine, MetallicLineFixed));
            return true;
        }

        static bool PatchForwardLighting(string path)
        {
            if (!File.Exists(path))
                return false;

            var text = File.ReadAllText(path);
            var normalized = NormalizeForwardLightingSsrGuard(text);
            if (normalized == text)
                return false;

            File.WriteAllText(path, normalized);
            return true;
        }

        static string NormalizeForwardLightingSsrGuard(string text)
        {
            var guardWithNewlines = "\n" + SsrGuardBlock + "\n";
            while (text.Contains(guardWithNewlines))
                text = text.Replace(guardWithNewlines, "\n");

            var pattern = @"(#include\s+""Alpha\.hlsl""\s*\r?\n)";
            if (!Regex.IsMatch(text, pattern))
                return text;

            if (!text.Contains(SsrGuardBlock))
                text = Regex.Replace(text, pattern, "$1\n" + SsrGuardBlock + "\n\n");

            return text;
        }

        public static void ReimportShaderGraphs()
        {
            foreach (var guid in AssetDatabase.FindAssets("a:assets", new[] { "Assets/ShaderGraph" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".shadergraph"))
                    continue;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
        }

        static string FindPackageRoot()
        {
            var cache = Path.Combine(Directory.GetCurrentDirectory(), "Library/PackageCache");
            if (!Directory.Exists(cache))
                return null;

            return Directory.GetDirectories(cache, PackageId + "@*").FirstOrDefault();
        }
    }

    sealed class Z3yShaderGraphWebGlFixBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => -100;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            Z3yShaderGraphWebGlFix.ApplyPatch();
            Z3yShaderGraphWebGlFix.ReimportShaderGraphs();
            WebGlShaderWarmupBuilder.BuildWarmupAsset();
            WebGlShaderWarmupBuilder.EnsureAlwaysIncludedShaderGraphs();
        }
    }
}
