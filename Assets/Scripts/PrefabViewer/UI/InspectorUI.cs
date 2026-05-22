using System.Collections.Generic;
using PrefabViewer.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public class InspectorUI : MonoBehaviour
    {
        RectTransform content;
        ScrollRect scrollRect;
        Slider scrollSpeedSlider;
        readonly List<GameObject> blocks = new List<GameObject>();

        public void Initialize(RectTransform inspectorContent, ScrollRect scroll)
        {
            content = inspectorContent;
            scrollRect = scroll;
            var body = scroll.transform.parent;
            EnsureBodyLayout(body);
            CreateScrollSpeedSlider(body);
        }

        static void EnsureBodyLayout(Transform body)
        {
            if (body.GetComponent<VerticalLayoutGroup>() != null)
                return;
            var vlg = body.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 0;
        }

        void CreateScrollSpeedSlider(Transform body)
        {
            var bar = new GameObject("ScrollSpeedBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            bar.transform.SetParent(body, false);
            bar.transform.SetAsFirstSibling();
            bar.GetComponent<Image>().color = UiTheme.PanelHeader;
            var barLe = bar.GetComponent<LayoutElement>();
            barLe.flexibleHeight = 0;
            barLe.preferredHeight = 22;
            barLe.minHeight = 22;

            var hlg = bar.GetComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(6, 6, 2, 2);
            hlg.spacing = 6;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childForceExpandWidth = false;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            labelGo.transform.SetParent(bar.transform, false);
            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = "Scroll";
            label.fontSize = 10;
            label.color = UiTheme.TextMuted;
            label.font = UiFactory.GetDefaultTmpFont();
            labelGo.GetComponent<LayoutElement>().preferredWidth = 42;

            var sliderGo = new GameObject("Slider", typeof(RectTransform), typeof(Slider), typeof(LayoutElement));
            sliderGo.transform.SetParent(bar.transform, false);
            var sliderLe = sliderGo.GetComponent<LayoutElement>();
            sliderLe.flexibleWidth = 1;
            sliderLe.preferredHeight = 14;
            sliderLe.minWidth = 80;

            var sliderRt = sliderGo.GetComponent<RectTransform>();
            sliderRt.sizeDelta = new Vector2(120, 14);

            var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderGo.transform, false);
            Stretch(bg.GetComponent<RectTransform>());
            bg.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f, 1f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGo.transform, false);
            var fillAreaRt = fillArea.GetComponent<RectTransform>();
            fillAreaRt.anchorMin = new Vector2(0, 0.25f);
            fillAreaRt.anchorMax = new Vector2(1, 0.75f);
            fillAreaRt.offsetMin = new Vector2(4, 0);
            fillAreaRt.offsetMax = new Vector2(-4, 0);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.GetComponent<Image>();
            fillImg.color = UiTheme.RowSelected;
            Stretch(fill.GetComponent<RectTransform>());

            var handleSlide = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlide.transform.SetParent(sliderGo.transform, false);
            Stretch(handleSlide.GetComponent<RectTransform>());

            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleSlide.transform, false);
            var handleImg = handle.GetComponent<Image>();
            handleImg.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            var handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(12, 0);

            scrollSpeedSlider = sliderGo.GetComponent<Slider>();
            scrollSpeedSlider.fillRect = fill.GetComponent<RectTransform>();
            scrollSpeedSlider.handleRect = handleRt;
            scrollSpeedSlider.targetGraphic = handleImg;
            scrollSpeedSlider.minValue = 8f;
            scrollSpeedSlider.maxValue = 80f;
            scrollSpeedSlider.value = scrollRect != null ? scrollRect.scrollSensitivity : 30f;
            scrollSpeedSlider.onValueChanged.AddListener(v =>
            {
                if (scrollRect != null)
                    scrollRect.scrollSensitivity = v;
            });
        }

        public void Show(GameObject target, IReadOnlyList<ComponentInfo> components)
        {
            Clear();
            if (target == null)
            {
                blocks.Add(CreateHintBlock("Select an object in the Hierarchy."));
                ResetScrollTop();
                return;
            }

            blocks.Add(InspectorFieldFactory.CreateObjectHeader(content, target.name));
            foreach (var component in components)
                blocks.Add(CreateComponentBlock(component));
            ResetScrollTop();
        }

        void ResetScrollTop()
        {
            if (scrollRect == null || content == null)
                return;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }

        GameObject CreateComponentBlock(ComponentInfo component)
        {
            var block = new GameObject("Component_" + component.typeName, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            block.transform.SetParent(content, false);
            var vlg = block.GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(0, 0, 0, 0);
            vlg.spacing = 0;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            block.GetComponent<LayoutElement>().minHeight = 24;

            InspectorFieldFactory.CreateComponentHeader(block.transform, component.typeName, component.enabled);

            foreach (var prop in component.properties)
                InspectorFieldFactory.CreatePropertyRow(block.transform, prop);

            return block;
        }

        GameObject CreateHintBlock(string message)
        {
            var block = new GameObject("Hint", typeof(RectTransform), typeof(LayoutElement));
            block.transform.SetParent(content, false);
            block.GetComponent<LayoutElement>().preferredHeight = 40;

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(block.transform, false);
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = message;
            tmp.fontSize = 12;
            tmp.color = UiTheme.TextMuted;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.margin = new Vector4(8, 8, 8, 8);
            Stretch(textGo.GetComponent<RectTransform>());
            return block;
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        void Clear()
        {
            foreach (var block in blocks)
            {
                if (block != null)
                    Destroy(block);
            }
            blocks.Clear();
        }
    }
}
