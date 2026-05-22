using System;
using System.Collections.Generic;
using PrefabViewer.Hierarchy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public class HierarchyUI : MonoBehaviour
    {
        RectTransform content;
        HierarchyNode root;
        string selectedId;
        Action<HierarchyNode> onNodeSelected;
        readonly List<GameObject> rowObjects = new List<GameObject>();

        public void Initialize(RectTransform hierarchyContent, Action<HierarchyNode> onSelected)
        {
            content = hierarchyContent;
            onNodeSelected = onSelected;
        }

        public void Clear()
        {
            root = null;
            selectedId = null;
            ClearRows();
        }

        public void Bind(HierarchyNode treeRoot)
        {
            root = treeRoot;
            selectedId = null;
            if (root != null)
                selectedId = root.Id;
            Rebuild();
        }

        public void Rebuild()
        {
            ClearRows();
            if (root == null)
                return;

            foreach (var node in root.FlattenVisible())
                rowObjects.Add(CreateRow(node));
        }

        GameObject CreateRow(HierarchyNode node)
        {
            var hasChildren = node.Children.Count > 0;
            var indent = 12 + node.Depth * 16;

            var row = new GameObject("HierarchyRow", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            row.transform.SetParent(content, false);
            var bg = row.GetComponent<Image>();
            bg.color = node.Id == selectedId ? UiTheme.RowSelected : UiTheme.RowNormal;
            var le = row.GetComponent<LayoutElement>();
            le.preferredHeight = 24;
            le.minHeight = 24;

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(indent, 4, 0, 0);
            hlg.spacing = 4;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;

            if (hasChildren)
            {
                var toggleGo = new GameObject("Expand", typeof(RectTransform), typeof(Button), typeof(Image), typeof(LayoutElement));
                toggleGo.transform.SetParent(row.transform, false);
                toggleGo.GetComponent<LayoutElement>().preferredWidth = 18;
                var toggleImg = toggleGo.GetComponent<Image>();
                toggleImg.color = UiTheme.RowNormal;
                var toggleBtn = toggleGo.GetComponent<Button>();
                var toggleLabel = new GameObject("Icon", typeof(RectTransform), typeof(TextMeshProUGUI));
                toggleLabel.transform.SetParent(toggleGo.transform, false);
                var tmp = toggleLabel.GetComponent<TextMeshProUGUI>();
                tmp.text = node.IsExpanded ? "▼" : "▶";
                tmp.fontSize = 12;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = UiTheme.TextMuted;
                Stretch(toggleLabel.GetComponent<RectTransform>());
                var captured = node;
                toggleBtn.onClick.AddListener(() =>
                {
                    captured.IsExpanded = !captured.IsExpanded;
                    Rebuild();
                });
            }
            else
            {
                var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
                spacer.transform.SetParent(row.transform, false);
                spacer.GetComponent<LayoutElement>().preferredWidth = 18;
            }

            var labelGo = new GameObject("Name", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            labelGo.transform.SetParent(row.transform, false);
            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = node.Name;
            label.fontSize = 13;
            label.color = UiTheme.TextPrimary;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            labelGo.GetComponent<LayoutElement>().flexibleWidth = 1;

            var rowBtn = row.GetComponent<Button>();
            var capturedNode = node;
            rowBtn.onClick.AddListener(() =>
            {
                selectedId = capturedNode.Id;
                onNodeSelected?.Invoke(capturedNode);
                Rebuild();
            });

            return row;
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        void ClearRows()
        {
            foreach (var row in rowObjects)
            {
                if (row != null)
                    Destroy(row);
            }
            rowObjects.Clear();
        }
    }
}
