using PrefabViewer.Hierarchy;
using PrefabViewer.Inspector;
using PrefabViewer.Preview;
using PrefabViewer.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace PrefabViewer
{
    public class PrefabViewerApp : MonoBehaviour
    {
        [SerializeField] PrefabCatalog catalog;
        [SerializeField] Transform previewRoot;

        PrefabListUI prefabListUI;
        HierarchyUI hierarchyUI;
        InspectorUI inspectorUI;
        SceneViewPanel sceneViewPanel;
        PreviewScene previewScene;

        QuadrantPanel prefabQuadrant;
        QuadrantPanel sceneQuadrant;
        QuadrantPanel hierarchyQuadrant;
        QuadrantPanel inspectorQuadrant;
        GameObject bottomRow;

        GameObject currentInstance;
        HierarchyNode currentRoot;
        bool particlesEnabled = true;

        void Awake()
        {
            if (catalog == null)
                catalog = Resources.Load<PrefabCatalog>("PrefabCatalog");

            if (catalog == null)
            {
                Debug.LogError("PrefabCatalog not found. Run Prefab Viewer > Setup Project in the Editor.");
                return;
            }

            EnsurePreviewRoot();
            EnsurePreviewScene();
            EnsureEventSystem();
            BuildUi();
            prefabListUI.Bind(catalog.Entries);
            SetQuadrantVisibility(hasPrefab: false);
        }

        void EnsurePreviewRoot()
        {
            if (previewRoot != null)
                return;
            var rootGo = new GameObject("PreviewRoot");
            previewRoot = rootGo.transform;
        }

        void EnsurePreviewScene()
        {
            previewScene = GetComponentInChildren<PreviewScene>();
            if (previewScene != null)
                return;

            var envGo = new GameObject("PreviewSceneHost");
            envGo.transform.SetParent(transform, false);
            previewScene = envGo.AddComponent<PreviewScene>();
            previewScene.Initialize(previewRoot);
        }

        static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
                return;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        void BuildUi()
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var grid = new GameObject("QuadrantGrid", typeof(RectTransform), typeof(VerticalLayoutGroup));
            grid.transform.SetParent(canvasGo.transform, false);
            Stretch(grid.GetComponent<RectTransform>());
            var gridVlg = grid.GetComponent<VerticalLayoutGroup>();
            gridVlg.spacing = 2;
            gridVlg.padding = new RectOffset(4, 4, 4, 4);
            gridVlg.childControlWidth = true;
            gridVlg.childControlHeight = true;
            gridVlg.childForceExpandWidth = true;
            gridVlg.childForceExpandHeight = true;

            var topRow = CreateRow(grid.transform, "TopRow");
            var bottomRowGo = CreateRow(grid.transform, "BottomRow");
            bottomRow = bottomRowGo;

            prefabQuadrant = QuadrantPanel.Create(topRow.transform, "PrefabQuadrant", "Prefabs");
            sceneQuadrant = QuadrantPanel.Create(topRow.transform, "SceneQuadrant", "Scene");
            hierarchyQuadrant = QuadrantPanel.Create(bottomRow.transform, "HierarchyQuadrant", "Hierarchy");
            inspectorQuadrant = QuadrantPanel.Create(bottomRow.transform, "InspectorQuadrant", "Inspector");

            prefabListUI = gameObject.AddComponent<PrefabListUI>();
            hierarchyUI = gameObject.AddComponent<HierarchyUI>();
            inspectorUI = gameObject.AddComponent<InspectorUI>();

            UiFactory.CreateScrollArea(prefabQuadrant.Body, "Scroll", out var prefabContent);
            UiFactory.CreateScrollArea(hierarchyQuadrant.Body, "Scroll", out var hierarchyContent);
            var inspectorScroll = UiFactory.CreateScrollArea(inspectorQuadrant.Body, "Scroll", out var inspectorContent, verticalScrollbar: true);

            prefabListUI.Initialize(prefabContent, OnPrefabSelected);
            hierarchyUI.Initialize(hierarchyContent, OnNodeSelected);
            inspectorUI.Initialize(inspectorContent, inspectorScroll, hierarchyUI.Rebuild);

            sceneViewPanel = SceneViewPanel.Create(sceneQuadrant.Body, previewScene);
        }

        static GameObject CreateRow(Transform parent, string name)
        {
            var row = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);
            var le = row.GetComponent<LayoutElement>();
            le.flexibleWidth = 1;
            le.flexibleHeight = 1;
            le.minHeight = 160;

            var hlg = row.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 2;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            Stretch(row.GetComponent<RectTransform>());
            return row;
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        void SetQuadrantVisibility(bool hasPrefab)
        {
            prefabQuadrant.IsVisible = true;
            sceneQuadrant.IsVisible = hasPrefab;
            hierarchyQuadrant.IsVisible = hasPrefab;
            inspectorQuadrant.IsVisible = hasPrefab;
            if (bottomRow != null)
                bottomRow.SetActive(hasPrefab);
        }

        void OnPrefabSelected(int index)
        {
            var entry = catalog.GetEntry(index);
            if (entry?.prefab == null)
                return;

            if (currentInstance != null)
                Destroy(currentInstance);

            currentInstance = Instantiate(entry.prefab, previewRoot);
            currentInstance.name = entry.prefab.name;
            currentRoot = HierarchyNode.BuildTree(currentInstance);

            // Apply current particle toggle to new selection.
            ApplyParticleToggleToInstance(currentInstance, particlesEnabled);

            var probetaSetupCount = global::PrefabViewer.WebGlProbetaPreviewSetup.Apply(currentInstance);
            var shaderGraphFallbackCount = WebGlShaderGraphMaterialFallback.Apply(currentInstance);
            var mutedBehaviourCount = 0;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var mutedTypeNames = new[]
                {
                    "ProbeBehabiour",
                    "FaceBehaviour",
                    "ActivateMirror",
                    "LijaCuadrada",
                    "IdentifyFace",
                };

                foreach (var behaviour in currentInstance.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (behaviour == null)
                        continue;
                    var typeName = behaviour.GetType().Name;
                    for (var i = 0; i < mutedTypeNames.Length; i++)
                    {
                        if (typeName != mutedTypeNames[i])
                            continue;
                        if (behaviour.enabled)
                        {
                            behaviour.enabled = false;
                            mutedBehaviourCount++;
                        }
                        break;
                    }
                }
            }

            // Reduce physics tunneling / free-fall in preview.
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                foreach (var body in currentInstance.GetComponentsInChildren<Rigidbody>(true))
                {
                    if (body == null)
                        continue;
                    body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    body.interpolation = RigidbodyInterpolation.Interpolate;
                }
            }

            SetQuadrantVisibility(hasPrefab: true);
            hierarchyUI.Bind(currentRoot);
            OnNodeSelected(currentRoot);
            sceneViewPanel?.FrameObject(currentInstance);
        }

        public void SetParticlesEnabled(bool enabled)
        {
            particlesEnabled = enabled;
            ApplyParticleToggleToInstance(currentInstance, particlesEnabled);
        }

        static void ApplyParticleToggleToInstance(GameObject instance, bool enabled)
        {
            if (instance == null)
                return;

            foreach (var ps in instance.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (ps == null)
                    continue;

                if (enabled)
                {
                    var em = ps.emission;
                    em.enabled = true;
                    if (!ps.isPlaying)
                        ps.Play(withChildren: true);
                }
                else
                {
                    var em = ps.emission;
                    em.enabled = false;
                    ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        void OnNodeSelected(HierarchyNode node)
        {
            if (node?.GameObject == null)
            {
                inspectorUI.Show(null, System.Array.Empty<ComponentInfo>());
                return;
            }

            var components = ComponentPropertyReader.Read(node.GameObject);
            inspectorUI.Show(node.GameObject, components);
            sceneViewPanel?.FocusObject(node.GameObject);
        }
    }
}
