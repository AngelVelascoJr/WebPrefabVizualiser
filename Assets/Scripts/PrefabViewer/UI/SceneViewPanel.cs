using PrefabViewer.Preview;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public class SceneViewPanel : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        PreviewScene previewScene;
        RawImage viewportImage;
        RectTransform viewportRect;
        TextMeshProUGUI labelText;
        Button particlesButton;
        bool particlesEnabled = true;

        bool dragging;
        bool panning;
        Vector2 lastPointer;
        int activePointerId = -1;

        public static SceneViewPanel Create(RectTransform quadrantBody, PreviewScene scene)
        {
            var host = quadrantBody.gameObject;
            var vlg = host.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = host.AddComponent<VerticalLayoutGroup>();
                vlg.childControlWidth = true;
                vlg.childControlHeight = true;
                vlg.childForceExpandWidth = true;
                vlg.childForceExpandHeight = false;
                vlg.spacing = 0;
            }

            var panel = host.GetComponent<SceneViewPanel>();
            if (panel == null)
                panel = host.AddComponent<SceneViewPanel>();
            panel.previewScene = scene;
            panel.BuildUi(host.transform);
            return panel;
        }

        void BuildUi(Transform host)
        {
            var toolbar = CreateToolbar(host);
            var toolbarLe = toolbar.GetComponent<LayoutElement>();
            toolbarLe.flexibleHeight = 0;
            toolbarLe.preferredHeight = 18;
            toolbarLe.minHeight = 18;

            var viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(RawImage), typeof(LayoutElement));
            viewportGo.transform.SetParent(host, false);
            var viewportLe = viewportGo.GetComponent<LayoutElement>();
            viewportLe.flexibleHeight = 1;
            viewportLe.minHeight = 200;

            viewportRect = viewportGo.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportImage = viewportGo.GetComponent<RawImage>();
            viewportImage.color = Color.white;
            viewportImage.raycastTarget = true;

            var overlay = new GameObject("OverlayLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            overlay.transform.SetParent(viewportGo.transform, false);
            var overlayRt = overlay.GetComponent<RectTransform>();
            overlayRt.anchorMin = new Vector2(0, 0);
            overlayRt.anchorMax = new Vector2(1, 0);
            overlayRt.pivot = new Vector2(0.5f, 0);
            overlayRt.sizeDelta = new Vector2(0, 22);
            overlayRt.anchoredPosition = Vector2.zero;
            labelText = overlay.GetComponent<TextMeshProUGUI>();
            labelText.fontSize = 11;
            labelText.color = UiTheme.TextMuted;
            labelText.alignment = TextAlignmentOptions.BottomLeft;
            labelText.margin = new Vector4(6, 0, 6, 4);
            labelText.text = "LMB: Orbit  |  RMB: Pan  |  Wheel: Zoom";
            labelText.raycastTarget = false;

            var relay = viewportGo.AddComponent<SceneViewInputRelay>();
            relay.panel = this;
        }

        GameObject CreateToolbar(Transform parent)
        {
            var bar = new GameObject("SceneToolbar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            bar.transform.SetParent(parent, false);
            bar.GetComponent<Image>().color = UiTheme.PanelHeader;

            var hlg = bar.GetComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(6, 6, 2, 2);
            hlg.spacing = 8;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;

            AddToolbarLabel(bar.transform, "Persp");
            AddToolbarLabel(bar.transform, "Shaded");
            particlesButton = CreateToolbarButton(bar.transform, "Particles: On");
            return bar;
        }

        static void AddToolbarLabel(Transform parent, string text)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 11;
            tmp.color = UiTheme.TextMuted;
            go.GetComponent<LayoutElement>().preferredWidth = 48;
        }

        Button CreateToolbarButton(Transform parent, string text)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var bg = go.GetComponent<Image>();
            bg.color = UiTheme.RowNormal;
            var le = go.GetComponent<LayoutElement>();
            le.preferredWidth = 110;
            le.minWidth = 110;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGo.transform.SetParent(go.transform, false);
            var tmp = labelGo.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 11;
            tmp.color = UiTheme.TextMuted;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.margin = new Vector4(6, 0, 6, 0);
            var rt = labelGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                particlesEnabled = !particlesEnabled;
                tmp.text = particlesEnabled ? "Particles: On" : "Particles: Off";
                var app = FindObjectOfType<PrefabViewer.PrefabViewerApp>();
                if (app != null)
                    app.SetParticlesEnabled(particlesEnabled);
            });
            return button;
        }

        void LateUpdate()
        {
            if (viewportRect == null || previewScene == null)
                return;

            var size = viewportRect.rect.size;
            if (size.x < 2f || size.y < 2f)
                return;

            var w = Mathf.RoundToInt(size.x);
            var h = Mathf.RoundToInt(size.y);
            var rt = previewScene.EnsureRenderTexture(w, h);
            if (viewportImage.texture != rt)
                viewportImage.texture = rt;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (previewScene?.PreviewCamera == null)
                return;

            activePointerId = eventData.pointerId;
            lastPointer = eventData.position;
            dragging = eventData.button == PointerEventData.InputButton.Left;
            var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            panning = eventData.button == PointerEventData.InputButton.Right ||
                      (eventData.button == PointerEventData.InputButton.Left && shiftHeld);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId)
                return;
            dragging = false;
            panning = false;
            activePointerId = -1;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (previewScene?.PreviewCamera == null || eventData.pointerId != activePointerId)
                return;

            var delta = eventData.position - lastPointer;
            lastPointer = eventData.position;

            if (panning)
            {
                previewScene.Orbit.Pan(delta, previewScene.PreviewCamera);
                previewScene.ApplyCameraTransform();
            }
            else if (dragging)
            {
                previewScene.Orbit.Orbit(delta);
                previewScene.ApplyCameraTransform();
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (previewScene == null)
                return;
            previewScene.Orbit.Zoom(eventData.scrollDelta.y);
            previewScene.ApplyCameraTransform();
        }

        public void FrameObject(GameObject go)
        {
            previewScene?.FrameGameObject(go);
            UpdateLabel(go);
        }

        public void FocusObject(GameObject go)
        {
            previewScene?.FocusGameObject(go, keepDistance: true);
            UpdateLabel(go);
        }

        void UpdateLabel(GameObject go)
        {
            if (labelText == null)
                return;
            labelText.text = go != null
                ? $"Viewing: {go.name}  |  LMB: Orbit  RMB: Pan  Wheel: Zoom"
                : "LMB: Orbit  |  RMB: Pan  |  Wheel: Zoom";
        }
    }
}
