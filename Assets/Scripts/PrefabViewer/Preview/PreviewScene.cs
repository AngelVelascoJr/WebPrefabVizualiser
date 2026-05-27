using UnityEngine;
using UnityEngine.Rendering;

namespace PrefabViewer.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        const int MinRenderSize = 128;

        public Camera PreviewCamera { get; private set; }
        public OrbitCameraController Orbit { get; } = new OrbitCameraController();
        public Transform ContentRoot { get; private set; }

        RenderTexture renderTexture;
        GameObject gridObject;
        Light keyLight;
        Light fillLight;

        public void Initialize(Transform parentRoot)
        {
            ContentRoot = parentRoot;

            var env = new GameObject("PreviewEnvironment");
            env.transform.SetParent(transform, false);
            env.transform.position = Vector3.zero;

            CreateLights(env.transform);
            CreateGrid(env.transform);
            CreateCamera(env.transform);

            Orbit.SetView(Vector3.zero, 5f, 35f, 25f);
            ApplyCameraTransform();
        }

        void CreateLights(Transform parent)
        {
            var keyGo = new GameObject("KeyLight", typeof(Light));
            keyGo.transform.SetParent(parent, false);
            keyGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            keyLight = keyGo.GetComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.1f;
            keyLight.color = new Color(1f, 0.98f, 0.95f);

            var fillGo = new GameObject("FillLight", typeof(Light));
            fillGo.transform.SetParent(parent, false);
            fillGo.transform.rotation = Quaternion.Euler(30f, 120f, 0f);
            fillLight = fillGo.GetComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.intensity = 0.35f;
            fillLight.color = new Color(0.85f, 0.9f, 1f);
        }

        void CreateGrid(Transform parent)
        {
            gridObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            gridObject.name = "Grid";
            gridObject.transform.SetParent(parent, false);
            gridObject.transform.localScale = new Vector3(4f, 1f, 4f);
            gridObject.transform.position = Vector3.zero;

            // Keep a collider for raycasts/picking in the preview.
            // Unity Plane primitive uses a MeshCollider by default; replace with a thin BoxCollider.
            var existingCollider = gridObject.GetComponent<Collider>();
            if (existingCollider != null)
                Destroy(existingCollider);
            var box = gridObject.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = new Vector3(10f, 0.02f, 10f);

            var shader = Shader.Find("Unlit/Transparent")
                ?? Shader.Find("Legacy Shaders/Transparent/Diffuse")
                ?? Shader.Find("Unlit/Texture");
            var mat = new Material(shader);
            var gridTex = CreateGridTexture(256, 32);
            if (mat.HasProperty("_MainTex"))
            {
                mat.mainTexture = gridTex;
                mat.mainTextureScale = new Vector2(8f, 8f);
            }
            mat.color = new Color(0.65f, 0.65f, 0.65f, 0.28f);
            if (mat.HasProperty("_ZWrite"))
                mat.SetInt("_ZWrite", 0);
            if (mat.HasProperty("_Cull"))
                mat.SetInt("_Cull", (int)CullMode.Off);
            var renderer = gridObject.GetComponent<Renderer>();
            renderer.sharedMaterial = mat;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        static Texture2D CreateGridTexture(int size, int cells)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;
            var major = new Color(0.45f, 0.45f, 0.45f, 0.5f);
            var minor = new Color(0.35f, 0.35f, 0.35f, 0.22f);
            var cell = size / cells;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var majorLine = x % cell == 0 || y % cell == 0;
                    tex.SetPixel(x, y, majorLine ? major : minor);
                }
            }
            tex.Apply();
            return tex;
        }

        void CreateCamera(Transform parent)
        {
            var camGo = new GameObject("SceneCamera", typeof(Camera));
            camGo.transform.SetParent(parent, false);
            PreviewCamera = camGo.GetComponent<Camera>();
            PreviewCamera.clearFlags = CameraClearFlags.SolidColor;
            PreviewCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            PreviewCamera.fieldOfView = 45f;
            PreviewCamera.nearClipPlane = 0.05f;
            PreviewCamera.farClipPlane = 200f;
            PreviewCamera.enabled = false;
        }

        public RenderTexture EnsureRenderTexture(int width, int height)
        {
            width = Mathf.Max(MinRenderSize, width);
            height = Mathf.Max(MinRenderSize, height);

            if (renderTexture != null &&
                renderTexture.width == width &&
                renderTexture.height == height)
                return renderTexture;

            if (renderTexture != null)
            {
                PreviewCamera.targetTexture = null;
                renderTexture.Release();
                Destroy(renderTexture);
            }

            renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.antiAliasing = 2;
            PreviewCamera.targetTexture = renderTexture;
            PreviewCamera.enabled = true;
            return renderTexture;
        }

        public void FrameGameObject(GameObject go)
        {
            if (go == null)
            {
                Orbit.SetView(Vector3.zero, 5f, 35f, 25f);
                ApplyCameraTransform();
                return;
            }

            var bounds = CalculateBounds(go);
            if (bounds.size.sqrMagnitude < 0.0001f)
                Orbit.SetView(go.transform.position, 3f, 35f, 25f);
            else
            {
                Orbit.FrameBounds(bounds);
            }
            ApplyCameraTransform();
        }

        public void FocusGameObject(GameObject go, bool keepDistance = true)
        {
            if (go == null)
                return;
            var bounds = CalculateBounds(go);
            Orbit.FocusPoint(bounds.size.sqrMagnitude > 0.0001f ? bounds.center : go.transform.position, keepDistance);
            ApplyCameraTransform();
        }

        public static Bounds CalculateBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(go.transform.position, Vector3.one * 0.5f);

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return bounds;
        }

        public void ApplyCameraTransform()
        {
            if (PreviewCamera != null)
                Orbit.ApplyToTransform(PreviewCamera.transform);
        }

        void OnDestroy()
        {
            if (renderTexture != null)
            {
                if (PreviewCamera != null)
                    PreviewCamera.targetTexture = null;
                renderTexture.Release();
                Destroy(renderTexture);
            }
        }
    }
}
