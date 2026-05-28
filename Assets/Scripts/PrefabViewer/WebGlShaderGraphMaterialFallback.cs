using UnityEngine;
using UnityEngine.Rendering;

namespace PrefabViewer
{
    /// <summary>
    /// z3y ShaderGraph shaders on WebGL expose a single pass without LightMode=ForwardBase,
    /// which renders magenta in the Built-in forward renderer. Swap to Standard for preview only.
    /// </summary>
    static class WebGlShaderGraphMaterialFallback
    {
        static Shader s_standardShader;

        public static int Apply(GameObject root)
        {
            if (root == null || Application.platform != RuntimePlatform.WebGLPlayer)
                return 0;

            if (s_standardShader == null)
            {
                s_standardShader = Shader.Find("Standard");
                if (s_standardShader == null)
                    return 0;
            }

            var replaced = 0;
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                var materials = renderer.materials;
                var changed = false;
                for (var i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (mat == null || mat.shader == null || !NeedsFallback(mat.shader))
                        continue;

                    materials[i] = CreateStandardFallback(mat);
                    replaced++;
                    changed = true;
                }

                if (changed)
                    renderer.materials = materials;
            }

            return replaced;
        }

        static bool NeedsFallback(Shader shader)
        {
            if (shader == null)
                return false;

            var shaderName = shader.name;
            if (shaderName.Contains("InternalErrorShader") || !shader.isSupported)
                return true;

            if (!shaderName.StartsWith("Shader Graphs/"))
                return false;

            for (var p = 0; p < shader.passCount; p++)
            {
                var lightMode = shader.FindPassTagValue(p, new ShaderTagId("LightMode")).name;
                if (lightMode == "ForwardBase")
                    return false;
            }

            return true;
        }

        static Material CreateStandardFallback(Material source)
        {
            var fallback = new Material(s_standardShader) { name = source.name + " (WebGL preview)" };

            fallback.color = ResolveAlbedoColor(source);
            CopyAlbedoTexture(source, fallback);
            CopyPbrMaps(source, fallback);

            if (source.HasProperty("_Cutoff") && fallback.HasProperty("_Cutoff"))
                fallback.SetFloat("_Cutoff", source.GetFloat("_Cutoff"));

            if (source.HasProperty("_Glossiness") && fallback.HasProperty("_Glossiness"))
                fallback.SetFloat("_Glossiness", source.GetFloat("_Glossiness"));
            else if (source.HasProperty("_Smoothness") && fallback.HasProperty("_Glossiness"))
                fallback.SetFloat("_Glossiness", source.GetFloat("_Smoothness"));
            else if (source.HasProperty("_ColorDeLija"))
                fallback.SetFloat("_Glossiness", 0.12f);
            else if (source.shader != null && source.shader.name.IndexOf("Probeta", System.StringComparison.OrdinalIgnoreCase) >= 0)
                fallback.SetFloat("_Glossiness", Mathf.Clamp01(source.HasProperty("_Glossiness") ? source.GetFloat("_Glossiness") : 0.25f));

            if (source.HasProperty("_Metallic") && fallback.HasProperty("_Metallic"))
                fallback.SetFloat("_Metallic", source.GetFloat("_Metallic"));
            else if (source.shader != null && source.shader.name.Contains("InternalErrorShader"))
                ApplyMirrorFallbackSurface(fallback);
            else if (source.name.IndexOf("espejo", System.StringComparison.OrdinalIgnoreCase) >= 0)
                ApplyMirrorFallbackSurface(fallback);

            ApplyStandardRenderingMode(fallback, source);
            FinalizeStandardMaterial(fallback);
            return fallback;
        }

        public static void FinalizeStandardMaterial(Material material)
        {
            if (material == null || material.shader == null || material.shader.name != "Standard")
                return;

            if (material.GetTexture("_MetallicGlossMap") != null)
                material.EnableKeyword("_METALLICGLOSSMAP");

            if (material.GetTexture("_BumpMap") != null)
                material.EnableKeyword("_NORMALMAP");
        }

        static void ApplyMirrorFallbackSurface(Material fallback)
        {
            fallback.SetFloat("_Metallic", 0.9f);
            fallback.SetFloat("_Glossiness", 0.92f);
            fallback.color = new Color(0.82f, 0.84f, 0.86f, 1f);
        }

        static Color ResolveAlbedoColor(Material source)
        {
            if (source.HasProperty("_ColorDeLija"))
            {
                var lijaColor = source.GetColor("_ColorDeLija");
                lijaColor.a = 1f;
                return lijaColor;
            }

            if (source.HasProperty("_ColorAgua"))
                return source.GetColor("_ColorAgua");

            if (source.HasProperty("_BaseColor"))
                return source.GetColor("_BaseColor");

            if (source.HasProperty("_Color"))
            {
                var color = source.GetColor("_Color");
                if (color.a <= 0.01f && color.r + color.g + color.b < 0.05f)
                    color.a = 1f;
                return color;
            }

            return Color.white;
        }

        public static Texture TryGetAlbedoTexture(Material source)
        {
            if (source == null)
                return null;

            if (source.HasProperty("_MainTex"))
            {
                var main = source.GetTexture("_MainTex");
                if (main != null)
                    return main;
            }

            if (source.HasProperty("_BaseMap"))
            {
                var baseMap = source.GetTexture("_BaseMap");
                if (baseMap != null)
                    return baseMap;
            }

            string[] preferred =
            {
                "_Texture2DAsset_b1503e52ff374cbba2c19c4e6699e849_Out_0_Texture2D",
                "_SampleTexture2D_9a0c69a4f39f481b94ad4bbab769b6d5_Texture_1_Texture2D",
                "_SampleTexture2D_6b4462efd3c84517bf75e5c8a31e1c89_Texture_1_Texture2D",
            };

            for (var i = 0; i < preferred.Length; i++)
            {
                if (!source.HasProperty(preferred[i]))
                    continue;

                var tex = source.GetTexture(preferred[i]);
                if (tex != null)
                    return tex;
            }

            return TryGetBestScoredAlbedoTexture(source);
        }

        static void CopyAlbedoTexture(Material source, Material fallback)
        {
            if (!fallback.HasProperty("_MainTex"))
                return;

            var tex = TryGetAlbedoTexture(source);
            if (tex != null)
                fallback.SetTexture("_MainTex", tex);
        }

        static Texture TryGetBestScoredAlbedoTexture(Material source)
        {
            var shader = source.shader;
            if (shader == null)
                return null;

            Texture best = null;
            var bestScore = int.MinValue;

            for (var i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyType(i) != UnityEngine.Rendering.ShaderPropertyType.Texture)
                    continue;

                var propName = shader.GetPropertyName(i);
                var tex = source.GetTexture(propName);
                if (tex == null)
                    continue;

                var score = ScoreAlbedoTexture(propName, tex.name);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = tex;
                }
            }

            return best;
        }

        static int ScoreAlbedoTexture(string propertyName, string textureName)
        {
            var prop = propertyName ?? "";
            var tex = textureName ?? "";
            var s = 0;

            if (tex.IndexOf("difus", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 100;
            if (tex.IndexOf("diffuse", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 100;
            if (tex.IndexOf("albedo", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 90;
            if (tex.IndexOf("base", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 40;
            if (tex.IndexOf("color", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 40;

            if (prop.IndexOf("base", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 30;
            if (prop.IndexOf("albedo", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 30;
            if (prop.IndexOf("color", System.StringComparison.OrdinalIgnoreCase) >= 0) s += 20;

            if (tex.IndexOf("normal", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 200;
            if (tex.IndexOf("rough", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 160;
            if (tex.IndexOf("metal", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 160;
            if (tex.IndexOf("occl", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 160;

            if (prop.IndexOf("normal", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 120;
            if (prop.IndexOf("rough", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 90;
            if (prop.IndexOf("metal", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 90;
            if (prop.IndexOf("occl", System.StringComparison.OrdinalIgnoreCase) >= 0) s -= 90;

            return s;
        }

        static void CopyPbrMaps(Material source, Material fallback)
        {
            if (source.HasProperty("_MetallicGlossMap") && fallback.HasProperty("_MetallicGlossMap"))
            {
                var map = source.GetTexture("_MetallicGlossMap");
                if (map != null)
                    fallback.SetTexture("_MetallicGlossMap", map);
            }

            if (source.HasProperty("_BumpMap") && fallback.HasProperty("_BumpMap"))
            {
                var bump = source.GetTexture("_BumpMap");
                if (bump != null)
                    fallback.SetTexture("_BumpMap", bump);
            }
            else if (source.HasProperty("_ParallaxMap") && fallback.HasProperty("_BumpMap"))
            {
                var bump = source.GetTexture("_ParallaxMap");
                if (bump != null)
                    fallback.SetTexture("_BumpMap", bump);
            }
        }

        static void ApplyStandardRenderingMode(Material fallback, Material source)
        {
            var mode = 0f;
            if (source.HasProperty("_Mode"))
                mode = source.GetFloat("_Mode");
            else if (source.HasProperty("_AlphaClip") && source.GetFloat("_AlphaClip") > 0.5f)
                mode = 1f;
            else if (source.HasProperty("_Surface") && source.GetFloat("_Surface") > 0.5f)
                mode = 2f;
            else if (source.renderQueue >= 3000)
                mode = 3f;
            else if (source.renderQueue >= 2450)
                mode = 2f;

            SetStandardMode(fallback, (int)mode);
        }

        static void SetStandardMode(Material material, int mode)
        {
            switch (mode)
            {
                case 1:
                    material.SetFloat("_Mode", 1f);
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.renderQueue = (int)RenderQueue.AlphaTest;
                    break;
                case 2:
                    material.SetFloat("_Mode", 2f);
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                case 3:
                    material.SetFloat("_Mode", 3f);
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)RenderQueue.Transparent;
                    break;
                default:
                    material.SetFloat("_Mode", 0f);
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
            }
        }
    }
}
