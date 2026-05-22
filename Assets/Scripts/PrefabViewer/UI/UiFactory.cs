using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public static class UiFactory
    {
        public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static GameObject CreatePanel(Transform parent, string name, Color bg)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = bg;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return go;
        }

        public static GameObject CreateHeader(Transform parent, string title)
        {
            var go = new GameObject("Header", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = UiTheme.PanelHeader;
            var le = go.GetComponent<LayoutElement>();
            le.flexibleHeight = 0;
            le.flexibleWidth = 1;
            le.preferredHeight = UiTheme.QuadrantHeaderHeight;
            le.minHeight = UiTheme.QuadrantHeaderHeight;

            var textGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(go.transform, false);
            var tmp = ApplyQuadrantHeaderStyle(textGo.GetComponent<TextMeshProUGUI>(), title);
            var rt = textGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return go;
        }

        public static TextMeshProUGUI ApplyQuadrantHeaderStyle(TextMeshProUGUI tmp, string title)
        {
            tmp.text = title;
            tmp.fontSize = UiTheme.QuadrantHeaderFontSize;
            tmp.enableAutoSizing = false;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = UiTheme.TextPrimary;
            tmp.margin = new Vector4(8, 2, 8, 2);
            tmp.rectTransform.sizeDelta = new Vector2(0, UiTheme.QuadrantHeaderHeight);
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.font = GetDefaultTmpFont();
            return tmp;
        }

        public static TMP_FontAsset GetDefaultTmpFont()
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;
            return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }

        public static ScrollRect CreateScrollArea(Transform parent, string name, out RectTransform content, bool verticalScrollbar = false)
        {
            const float scrollbarWidth = 14f;

            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(ScrollRect), typeof(LayoutElement));
            root.transform.SetParent(parent, false);
            root.GetComponent<Image>().color = UiTheme.InspectorPanelBg;
            var rootLe = root.GetComponent<LayoutElement>();
            rootLe.flexibleWidth = 1;
            rootLe.flexibleHeight = 1;
            rootLe.minHeight = 60;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(root.transform, false);
            var vpRt = viewport.GetComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = Vector2.zero;
            vpRt.offsetMax = verticalScrollbar ? new Vector2(-scrollbarWidth, 0f) : Vector2.zero;
            viewport.GetComponent<Image>().color = UiTheme.InspectorPanelBg;
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentGo.transform.SetParent(viewport.transform, false);
            content = contentGo.GetComponent<RectTransform>();
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;

            var vlg = contentGo.GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 2;
            vlg.padding = new RectOffset(4, 4, 4, 4);

            var fitter = contentGo.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var scroll = root.GetComponent<ScrollRect>();
            scroll.viewport = vpRt;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;

            if (verticalScrollbar)
            {
                scroll.verticalScrollbar = CreateVerticalScrollbar(root.transform, scrollbarWidth);
                scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            }

            var rootRt = root.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            return scroll;
        }

        static Scrollbar CreateVerticalScrollbar(Transform parent, float width)
        {
            var trackColor = new Color(0.14f, 0.14f, 0.14f, 0.95f);
            var handleColor = new Color(0.42f, 0.42f, 0.42f, 1f);

            var scrollbarGo = new GameObject("Scrollbar Vertical", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
            scrollbarGo.transform.SetParent(parent, false);
            var sbRt = scrollbarGo.GetComponent<RectTransform>();
            sbRt.anchorMin = new Vector2(1f, 0f);
            sbRt.anchorMax = new Vector2(1f, 1f);
            sbRt.pivot = new Vector2(1f, 0.5f);
            sbRt.anchoredPosition = Vector2.zero;
            sbRt.sizeDelta = new Vector2(width, 0f);
            scrollbarGo.GetComponent<Image>().color = trackColor;

            var slidingArea = new GameObject("Sliding Area", typeof(RectTransform));
            slidingArea.transform.SetParent(scrollbarGo.transform, false);
            var slideRt = slidingArea.GetComponent<RectTransform>();
            slideRt.anchorMin = Vector2.zero;
            slideRt.anchorMax = Vector2.one;
            slideRt.offsetMin = new Vector2(2f, 4f);
            slideRt.offsetMax = new Vector2(-2f, -4f);

            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(slidingArea.transform, false);
            var handleImg = handle.GetComponent<Image>();
            handleImg.color = handleColor;
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(width - 4f, 48f);

            var sb = scrollbarGo.GetComponent<Scrollbar>();
            sb.handleRect = handleRt;
            sb.targetGraphic = handleImg;
            sb.direction = Scrollbar.Direction.BottomToTop;
            return sb;
        }

        public static Button CreateListButton(Transform parent, string label, out Image bg)
        {
            var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            bg = go.GetComponent<Image>();
            bg.color = UiTheme.RowNormal;
            var le = go.GetComponent<LayoutElement>();
            le.preferredHeight = 26;
            le.minHeight = 26;

            var textGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(go.transform, false);
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 13;
            tmp.color = UiTheme.TextPrimary;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.margin = new Vector4(8, 0, 8, 0);
            var rt = textGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            return go.GetComponent<Button>();
        }
    }
}
