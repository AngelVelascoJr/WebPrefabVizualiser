using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public class QuadrantPanel
    {
        public GameObject Root { get; }
        public RectTransform Body { get; }

        QuadrantPanel(GameObject root, RectTransform body)
        {
            Root = root;
            Body = body;
        }

        public bool IsVisible
        {
            get => Root.activeSelf;
            set => Root.SetActive(value);
        }

        public static QuadrantPanel Create(Transform parent, string name, string title)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(VerticalLayoutGroup));
            root.transform.SetParent(parent, false);
            root.GetComponent<Image>().color = UiTheme.Border;

            var le = root.GetComponent<LayoutElement>();
            le.flexibleWidth = 1;
            le.flexibleHeight = 1;
            le.minWidth = 120;
            le.minHeight = 120;

            var vlg = root.GetComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 0;
            vlg.padding = new RectOffset(0, 0, 0, 0);

            UiFactory.CreateHeader(root.transform, title);

            var bodyGo = new GameObject("Body", typeof(RectTransform), typeof(LayoutElement));
            bodyGo.transform.SetParent(root.transform, false);
            var bodyLe = bodyGo.GetComponent<LayoutElement>();
            bodyLe.flexibleHeight = 1;
            bodyLe.minHeight = 80;

            var bodyRt = bodyGo.GetComponent<RectTransform>();
            bodyRt.anchorMin = Vector2.zero;
            bodyRt.anchorMax = Vector2.one;
            bodyRt.offsetMin = Vector2.zero;
            bodyRt.offsetMax = Vector2.zero;

            return new QuadrantPanel(root, bodyRt);
        }
    }
}
