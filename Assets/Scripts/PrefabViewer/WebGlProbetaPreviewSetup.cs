using UnityEngine;

namespace PrefabViewer
{
    /// <summary>
    /// VRChat probeta prefabs drive face visibility via UpdatePreview (Shader1 + borders + canvas).
    /// The WebGL viewer shows both face meshes (Shader1/Shader2) with a static unlit albedo; borders stay hidden.
    /// </summary>
    static class WebGlProbetaPreviewSetup
    {
        const string FacePreviewMaterialResource = "PrefabViewer/FacePreviewUnlit";

        static Material s_facePreviewTemplate;
        static Shader s_cachedFaceUnlitShader;

        public static int Apply(GameObject root)
        {
            if (root == null || Application.platform != RuntimePlatform.WebGLPlayer)
                return 0;

            var changes = 0;
            changes += MuteFaceBehaviours(root);
            changes += SetupInitialFaceVisibility(root);
            changes += ApplyFaceMeshPreviewMaterials(root);
            changes += SimplifyBodyRenderers(root);
            return changes;
        }

        static int MuteFaceBehaviours(GameObject root)
        {
            var changes = 0;
            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null)
                    continue;

                var typeName = behaviour.GetType().Name;
                if (typeName != "InteractProbe" && typeName != "UpdatePreview")
                    continue;

                if (behaviour.enabled)
                {
                    behaviour.enabled = false;
                    changes++;
                }
            }

            return changes;
        }

        static int SetupInitialFaceVisibility(GameObject root)
        {
            var changes = 0;
            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null || behaviour.GetType().Name != "UpdatePreview")
                    continue;

                var t = behaviour.GetType();
                var shaderInf = t.GetField("probetaShaderParent_inf")?.GetValue(behaviour) as GameObject;
                var shaderSup = t.GetField("probetaShaderParent_sup")?.GetValue(behaviour) as GameObject;
                var mirrorInf = t.GetField("probetaMirrorParent_inf")?.GetValue(behaviour) as GameObject;
                var mirrorSup = t.GetField("probetaMirrorParent_sup")?.GetValue(behaviour) as GameObject;
                var borderInf = t.GetField("probetaBorderParent_inf")?.GetValue(behaviour) as GameObject;
                var borderSup = t.GetField("probetaBorderParent_sup")?.GetValue(behaviour) as GameObject;
                var shaderChildren = t.GetField("probetaShaderChildren")?.GetValue(behaviour) as GameObject;
                var mirrorChildren = t.GetField("probetaMirrorChildren")?.GetValue(behaviour) as GameObject;

                changes += SetActiveIfNeeded(shaderInf, true);
                changes += SetActiveIfNeeded(shaderSup, true);
                changes += SetActiveIfNeeded(mirrorInf, false);
                changes += SetActiveIfNeeded(mirrorSup, false);
                changes += SetActiveIfNeeded(borderInf, false);
                changes += SetActiveIfNeeded(borderSup, false);
                changes += SetActiveIfNeeded(shaderChildren, false);
                changes += SetActiveIfNeeded(mirrorChildren, false);

                changes += EnsureRendererEnabled(shaderInf, true);
                changes += EnsureRendererEnabled(shaderSup, true);

                break;
            }

            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null || behaviour.GetType().Name != "InteractProbe")
                    continue;

                var canva = behaviour.GetType().GetField("canva")?.GetValue(behaviour) as GameObject;
                changes += SetActiveIfNeeded(canva, true);
                break;
            }

            foreach (var tr in root.GetComponentsInChildren<Transform>(true))
            {
                if (tr == null)
                    continue;

                var n = tr.gameObject.name;
                if (n == "Mirror1" || n == "Mirror2")
                    changes += SetActiveIfNeeded(tr.gameObject, false);
                else if (n == "Border1" || n == "Border2")
                    changes += SetActiveIfNeeded(tr.gameObject, false);
                else if (n == "Shader1" || n == "Shader2")
                {
                    changes += SetActiveIfNeeded(tr.gameObject, true);
                    changes += EnsureRendererEnabled(tr.gameObject, true);
                }
            }

            return changes;
        }

        static int ApplyFaceMeshPreviewMaterials(GameObject root)
        {
            var unlitShader = ResolveFaceUnlitShader();
            if (unlitShader == null)
                return 0;

            var changes = 0;
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || !renderer.gameObject.activeInHierarchy)
                    continue;

                var goName = renderer.gameObject.name;
                if (!goName.Equals("Shader1", System.StringComparison.OrdinalIgnoreCase)
                    && !goName.Equals("Shader2", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!renderer.enabled)
                {
                    renderer.enabled = true;
                    changes++;
                }

                var mats = renderer.materials;
                var changed = false;
                for (var i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null)
                        continue;

                    if (m.name.IndexOf("OutLineMat", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;

                    var tex = WebGlShaderGraphMaterialFallback.TryGetAlbedoTexture(m);
                    if (tex == null)
                        tex = GetFacePreviewFallbackTexture();
                    if (tex == null)
                        continue;

                    mats[i] = CreateFaceUnlitMaterial(unlitShader, m.name, tex, m);
                    changed = true;
                    changes++;
                }

                if (changed)
                    renderer.materials = mats;
            }

            return changes;
        }

        static int SimplifyBodyRenderers(GameObject root)
        {
            var changes = 0;
            foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!renderer.gameObject.name.Equals("Body", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var materials = renderer.materials;
                if (materials.Length < 2)
                    continue;

                var bodyMaterial = materials[materials.Length - 1];
                WebGlShaderGraphMaterialFallback.FinalizeStandardMaterial(bodyMaterial);
                renderer.materials = new[] { bodyMaterial };
                changes++;
            }

            return changes;
        }

        static int SetActiveIfNeeded(GameObject go, bool active)
        {
            if (go == null || go.activeSelf == active)
                return 0;
            go.SetActive(active);
            return 1;
        }

        static int EnsureRendererEnabled(GameObject go, bool enabled)
        {
            if (go == null)
                return 0;

            var renderer = go.GetComponent<Renderer>();
            if (renderer == null || renderer.enabled == enabled)
                return 0;
            renderer.enabled = enabled;
            return 1;
        }

        static Shader ResolveFaceUnlitShader()
        {
            if (s_cachedFaceUnlitShader != null)
                return s_cachedFaceUnlitShader;

            var template = GetFacePreviewTemplate();
            if (template != null && template.shader != null)
            {
                s_cachedFaceUnlitShader = template.shader;
                return s_cachedFaceUnlitShader;
            }

            s_cachedFaceUnlitShader = Shader.Find("Unlit/Texture")
                                   ?? Shader.Find("Unlit/Transparent")
                                   ?? Shader.Find("Unlit/Color");
            return s_cachedFaceUnlitShader;
        }

        static Material GetFacePreviewTemplate()
        {
            if (s_facePreviewTemplate == null)
                s_facePreviewTemplate = Resources.Load<Material>(FacePreviewMaterialResource);
            return s_facePreviewTemplate;
        }

        static Texture GetFacePreviewFallbackTexture()
        {
            var template = GetFacePreviewTemplate();
            return template != null ? template.GetTexture("_MainTex") : null;
        }

        static Material CreateFaceUnlitMaterial(Shader shader, string sourceMaterialName, Texture albedo, Material tintSource)
        {
            var template = GetFacePreviewTemplate();
            Material unlit;
            if (template != null && template.shader == shader)
                unlit = new Material(template) { name = sourceMaterialName + " (Unlit)" };
            else
                unlit = new Material(shader) { name = sourceMaterialName + " (Unlit)" };

            if (unlit.HasProperty("_MainTex"))
                unlit.SetTexture("_MainTex", albedo);
            if (unlit.HasProperty("_Color"))
            {
                var tint = tintSource != null && tintSource.HasProperty("_Color")
                    ? tintSource.GetColor("_Color")
                    : Color.white;
                if (tint.a <= 0.01f)
                    tint.a = 1f;
                unlit.SetColor("_Color", tint);
            }

            if (unlit.HasProperty("_Cull"))
                unlit.SetInt("_Cull", 0);
            unlit.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            return unlit;
        }
    }
}
