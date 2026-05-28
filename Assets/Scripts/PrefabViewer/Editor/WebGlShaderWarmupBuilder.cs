#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrefabViewer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PrefabViewer.Editor
{
    static class WebGlShaderWarmupBuilder
    {
        const string WarmupPath = "Assets/Resources/WebGlShaderWarmup.asset";
        const string ShaderGraphFolder = "Assets/ShaderGraph";

        static readonly (string guid, long fileId)[] ShaderGraphRefs =
        {
            ("65d6705c57f1d1b4a9411acf8d2c6278", -6465566751694194690L),
            ("28fa272e039709a4c8a92d3b57dbe52d", -6465566751694194690L),
            ("b3022a6ebd8d52343bee58d7cc7ed9b7", -6465566751694194690L),
            ("53c9e0a1ac84a3a4a83e7a04af479454", -6465566751694194690L),
            ("31c545b308b2ae74ca144e9ddc3d1b03", -6465566751694194690L),
            ("7abf0169b73bd70428c307ff86a0c328", -6465566751694194690L),
            ("44e69cff7ae5b374a84a965e8d60e764", -6465566751694194690L),
        };

        [MenuItem("Prefab Viewer/Refresh WebGL Shader Warmup")]
        public static void RefreshFromMenu()
        {
            if (BuildWarmupAsset())
                EditorUtility.DisplayDialog("WebGL Shader Warmup", "Warmup asset updated:\n" + WarmupPath, "OK");
        }

        public static bool BuildWarmupAsset()
        {
            var materials = CollectMaterials();
            var warmup = AssetDatabase.LoadAssetAtPath<WebGlShaderWarmup>(WarmupPath);
            if (warmup == null)
            {
                warmup = ScriptableObject.CreateInstance<WebGlShaderWarmup>();
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateAsset(warmup, WarmupPath);
            }

            warmup.materials = materials.ToArray();
            EditorUtility.SetDirty(warmup);
            AssetDatabase.SaveAssets();
            Debug.Log($"[WebGlShaderWarmup] {warmup.materials.Length} materials referenced for WebGL shader inclusion.");
            return true;
        }

        /// <summary>
        /// ShaderGraph guids are listed in ProjectSettings/GraphicsSettings.asset (m_AlwaysIncludedShaders).
        /// We only verify assets exist; modifying GraphicsSettings via SerializedObject is unreliable during preprocess build.
        /// </summary>
        public static void EnsureAlwaysIncludedShaderGraphs()
        {
            var graphicsYamlPath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings/GraphicsSettings.asset");
            var graphicsYaml = File.Exists(graphicsYamlPath) ? File.ReadAllText(graphicsYamlPath) : "";

            foreach (var (guid, _) in ShaderGraphRefs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning($"[WebGlShaderWarmup] ShaderGraph asset missing for guid {guid}.");
                    continue;
                }

                if (!graphicsYaml.Contains(guid))
                    Debug.LogWarning($"[WebGlShaderWarmup] Add {path} to GraphicsSettings > Always Included Shaders (guid {guid}).");
            }
        }

        const string FacePreviewMaterialPath = "Assets/Resources/PrefabViewer/FacePreviewUnlit.mat";

        static List<Material> CollectMaterials()
        {
            var set = new HashSet<Material>();

            var facePreview = AssetDatabase.LoadAssetAtPath<Material>(FacePreviewMaterialPath);
            if (facePreview != null)
                set.Add(facePreview);

            foreach (var path in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials", "Assets/ShaderGraph" })
                         .Select(AssetDatabase.GUIDToAssetPath))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null && mat.shader != null && mat.shader.name.StartsWith("Shader Graphs/"))
                    set.Add(mat);
            }

            var catalog = AssetDatabase.LoadAssetAtPath<PrefabCatalog>("Assets/Resources/PrefabCatalog.asset");
            if (catalog != null)
            {
                foreach (var entry in catalog.Entries)
                {
                    if (entry?.prefab == null)
                        continue;
                    foreach (var r in entry.prefab.GetComponentsInChildren<Renderer>(true))
                    {
                        foreach (var m in r.sharedMaterials)
                        {
                            if (m != null && m.shader != null && m.shader.name.StartsWith("Shader Graphs/"))
                                set.Add(m);
                        }
                    }
                }
            }

            return set.OrderBy(m => m.name).ToList();
        }
    }
}
#endif
