using PrefabViewer.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public static class InspectorFieldFactory
    {
        public static GameObject CreatePropertyRow(Transform parent, PropertyInfoDto prop)
        {
            var row = new GameObject("Prop_" + prop.name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);
            var le = row.GetComponent<LayoutElement>();
            le.preferredHeight = UiTheme.InspectorRowHeight;
            le.minHeight = UiTheme.InspectorRowHeight;

            var hlg = row.GetComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(4, 4, 2, 2);
            hlg.spacing = 4;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            CreateLabel(row.transform, prop.name);

            switch (prop.displayKind)
            {
                case PropertyDisplayKind.Bool:
                    CreateReadOnlyCheckbox(row.transform, prop.boolValue);
                    break;
                case PropertyDisplayKind.Vector2:
                    CreateVectorFields(row.transform, prop.vectorX, prop.vectorY, 0f, 2);
                    break;
                case PropertyDisplayKind.Vector3:
                    CreateVectorFields(row.transform, prop.vectorX, prop.vectorY, prop.vectorZ, 3);
                    break;
                case PropertyDisplayKind.Vector4:
                    CreateVectorFields(row.transform, prop.vectorX, prop.vectorY, prop.vectorZ, 4, prop.vectorW);
                    break;
                case PropertyDisplayKind.Color:
                    CreateColorField(row.transform, prop);
                    break;
                case PropertyDisplayKind.Enum:
                case PropertyDisplayKind.LayerMask:
                    CreateDropdownField(row.transform, prop.value ?? "");
                    break;
                case PropertyDisplayKind.ObjectReference:
                    CreateObjectReferenceField(row.transform, prop.value ?? "None");
                    break;
                default:
                    CreateTextField(row.transform, prop.value ?? "");
                    break;
            }

            return row;
        }

        public static GameObject CreateComponentHeader(Transform parent, string typeName, bool enabled)
        {
            var header = new GameObject("Header", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            header.transform.SetParent(parent, false);
            header.GetComponent<Image>().color = UiTheme.PanelHeader;
            header.GetComponent<LayoutElement>().preferredHeight = 22;
            header.GetComponent<LayoutElement>().minHeight = 22;

            var hlg = header.GetComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(4, 4, 2, 2);
            hlg.spacing = 6;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childForceExpandWidth = false;

            CreateReadOnlyCheckbox(header.transform, enabled);

            var titleGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            titleGo.transform.SetParent(header.transform, false);
            var title = titleGo.GetComponent<TextMeshProUGUI>();
            title.text = typeName;
            title.fontSize = 12;
            title.fontStyle = FontStyles.Bold;
            title.color = UiTheme.TextPrimary;
            title.font = UiFactory.GetDefaultTmpFont();
            titleGo.GetComponent<LayoutElement>().flexibleWidth = 1;

            return header;
        }

        public static GameObject CreateObjectHeader(Transform parent, string objectName)
        {
            var block = new GameObject("ObjectHeader", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            block.transform.SetParent(parent, false);
            block.GetComponent<Image>().color = UiTheme.PanelHeader;
            block.GetComponent<LayoutElement>().preferredHeight = 26;

            var field = CreateFieldShell(block.transform, 1f);
            var frt = field.GetComponent<RectTransform>();
            frt.anchorMin = Vector2.zero;
            frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(6, 4);
            frt.offsetMax = new Vector2(-6, -4);
            AddFieldText(field.transform, objectName, TextAlignmentOptions.MidlineLeft, new Vector4(6, 0, 6, 0));
            return block;
        }

        static void CreateLabel(Transform parent, string text)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 11;
            tmp.color = UiTheme.InspectorLabel;
            tmp.alignment = TextAlignmentOptions.MidlineRight;
            tmp.font = UiFactory.GetDefaultTmpFont();
            var le = go.GetComponent<LayoutElement>();
            le.preferredWidth = UiTheme.InspectorLabelWidth;
            le.minWidth = UiTheme.InspectorLabelWidth;
        }

        static GameObject CreateTextField(Transform parent, string text, bool fullWidth = false)
        {
            var field = CreateFieldShell(parent, fullWidth ? 1f : 0.55f);
            AddFieldText(field.transform, text, TextAlignmentOptions.MidlineLeft, new Vector4(6, 0, 6, 0));
            return field;
        }

        static void CreateDropdownField(Transform parent, string text)
        {
            var field = CreateFieldShell(parent, 1f);
            AddFieldText(field.transform, text, TextAlignmentOptions.MidlineLeft, new Vector4(6, 0, 22, 0));
            AddDropdownArrow(field.transform);
        }

        static void CreateObjectReferenceField(Transform parent, string text)
        {
            var field = CreateFieldShell(parent, 1f);
            AddFieldText(field.transform, text, TextAlignmentOptions.MidlineLeft, new Vector4(6, 0, 28, 0));
            AddObjectPickerIcon(field.transform);
        }

        static void CreateReadOnlyCheckbox(Transform parent, bool isOn)
        {
            var box = new GameObject("Checkbox", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            box.transform.SetParent(parent, false);
            var le = box.GetComponent<LayoutElement>();
            le.preferredWidth = 14;
            le.preferredHeight = 14;
            le.minWidth = 14;
            le.minHeight = 14;
            box.GetComponent<Image>().color = UiTheme.InspectorCheckboxBg;

            if (isOn)
            {
                var mark = new GameObject("Check", typeof(RectTransform), typeof(Image));
                mark.transform.SetParent(box.transform, false);
                var rt = mark.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.15f, 0.15f);
                rt.anchorMax = new Vector2(0.85f, 0.85f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                mark.GetComponent<Image>().color = UiTheme.InspectorCheckboxOn;
            }
        }

        static void CreateVectorFields(Transform parent, float x, float y, float z, int count, float w = 0f)
        {
            var group = new GameObject("VectorGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            group.transform.SetParent(parent, false);
            var gle = group.GetComponent<LayoutElement>();
            gle.flexibleWidth = 1;
            gle.preferredHeight = UiTheme.InspectorFieldHeight;

            var hlg = group.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 2;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = true;

            CreateAxisField(group.transform, "X", x);
            CreateAxisField(group.transform, "Y", y);
            if (count >= 3)
                CreateAxisField(group.transform, "Z", z);
            if (count >= 4)
                CreateAxisField(group.transform, "W", w);
        }

        static void CreateAxisField(Transform parent, string axis, float value)
        {
            var axisRow = new GameObject(axis, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            axisRow.transform.SetParent(parent, false);
            axisRow.GetComponent<LayoutElement>().flexibleWidth = 1;

            var hlg = axisRow.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 2;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;

            var label = new GameObject("L", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            label.transform.SetParent(axisRow.transform, false);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = axis;
            tmp.fontSize = 10;
            tmp.color = UiTheme.InspectorLabel;
            tmp.alignment = TextAlignmentOptions.MidlineRight;
            tmp.font = UiFactory.GetDefaultTmpFont();
            label.GetComponent<LayoutElement>().preferredWidth = 12;

            var field = CreateFieldShell(axisRow.transform, 1f);
            AddFieldText(field.transform, FormatNumber(value), TextAlignmentOptions.MidlineLeft, new Vector4(4, 0, 4, 0));
        }

        static void CreateColorField(Transform parent, PropertyInfoDto prop)
        {
            var row = new GameObject("ColorRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);
            row.GetComponent<LayoutElement>().flexibleWidth = 1;

            var swatch = new GameObject("Swatch", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            swatch.transform.SetParent(row.transform, false);
            swatch.GetComponent<LayoutElement>().preferredWidth = 36;
            swatch.GetComponent<LayoutElement>().preferredHeight = UiTheme.InspectorFieldHeight;
            swatch.GetComponent<Image>().color = new Color(prop.vectorX, prop.vectorY, prop.vectorZ, prop.vectorW);

            var field = CreateFieldShell(row.transform, 1f);
            AddFieldText(field.transform,
                $"R {prop.vectorX:0.##} G {prop.vectorY:0.##} B {prop.vectorZ:0.##} A {prop.vectorW:0.##}",
                TextAlignmentOptions.MidlineLeft, new Vector4(4, 0, 4, 0));
        }

        static GameObject CreateFieldShell(Transform parent, float widthFlex)
        {
            var shell = new GameObject("Field", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            shell.transform.SetParent(parent, false);
            var img = shell.GetComponent<Image>();
            img.color = UiTheme.InspectorFieldBg;
            img.raycastTarget = false;

            var outline = shell.AddComponent<Outline>();
            outline.effectColor = UiTheme.InspectorFieldBorder;
            outline.effectDistance = new Vector2(1f, -1f);

            var le = shell.GetComponent<LayoutElement>();
            le.preferredHeight = UiTheme.InspectorFieldHeight;
            le.minHeight = UiTheme.InspectorFieldHeight;
            if (widthFlex >= 1f)
                le.flexibleWidth = 1;
            else
                le.flexibleWidth = widthFlex;

            return shell;
        }

        static void AddFieldText(Transform field, string text, TextAlignmentOptions align, Vector4 margin)
        {
            var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(field, false);
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 11;
            tmp.color = UiTheme.TextPrimary;
            tmp.alignment = align;
            tmp.margin = margin;
            tmp.raycastTarget = false;
            tmp.font = UiFactory.GetDefaultTmpFont();
            Stretch(textGo.GetComponent<RectTransform>());
        }

        static void AddDropdownArrow(Transform field)
        {
            var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(TextMeshProUGUI));
            arrow.transform.SetParent(field, false);
            var tmp = arrow.GetComponent<TextMeshProUGUI>();
            tmp.text = "\u25BC";
            tmp.fontSize = 9;
            tmp.color = UiTheme.InspectorLabel;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            tmp.font = UiFactory.GetDefaultTmpFont();
            var rt = arrow.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(18, 0);
            rt.anchoredPosition = Vector2.zero;
        }

        static void AddObjectPickerIcon(Transform field)
        {
            var icon = new GameObject("Picker", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(field, false);
            var img = icon.GetComponent<Image>();
            img.color = UiTheme.InspectorLabel;
            img.raycastTarget = false;
            var rt = icon.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(1, 0.5f);
            rt.sizeDelta = new Vector2(14, 14);
            rt.anchoredPosition = new Vector2(-6, 0);

            var dot = new GameObject("Dot", typeof(RectTransform), typeof(Image));
            dot.transform.SetParent(icon.transform, false);
            dot.GetComponent<Image>().color = UiTheme.InspectorFieldBg;
            var dotRt = dot.GetComponent<RectTransform>();
            dotRt.sizeDelta = new Vector2(6, 6);
            dotRt.anchoredPosition = Vector2.zero;
        }

        static string FormatNumber(float v) => v.ToString("0.####");

        static void StretchFieldInParent(GameObject field, Vector4 margin)
        {
            var rt = field.GetComponent<RectTransform>();
            if (rt == null)
                return;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(margin.x, margin.w);
            rt.offsetMax = new Vector2(-margin.z, -margin.y);
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
