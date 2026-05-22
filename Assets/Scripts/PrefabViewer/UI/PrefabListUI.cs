using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrefabViewer.UI
{
    public class PrefabListUI : MonoBehaviour
    {
        RectTransform content;
        readonly List<Button> buttons = new List<Button>();
        readonly List<Image> buttonBackgrounds = new List<Image>();
        int selectedIndex = -1;
        Action<int> onSelected;

        public void Initialize(RectTransform listContent, Action<int> onEntrySelected)
        {
            content = listContent;
            onSelected = onEntrySelected;
        }

        public void Bind(IReadOnlyList<PrefabCatalog.Entry> entries)
        {
            Clear();
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry?.prefab == null)
                    continue;

                var label = string.IsNullOrEmpty(entry.displayName) ? entry.prefab.name : entry.displayName;
                var btn = UiFactory.CreateListButton(content, label, out var bg);
                var index = i;
                btn.onClick.AddListener(() => Select(index));
                buttons.Add(btn);
                buttonBackgrounds.Add(bg);
            }

        }

        public void Select(int index)
        {
            if (index < 0 || index >= buttons.Count)
                return;

            selectedIndex = index;
            for (var i = 0; i < buttonBackgrounds.Count; i++)
                buttonBackgrounds[i].color = i == selectedIndex ? UiTheme.RowSelected : UiTheme.RowNormal;

            onSelected?.Invoke(index);
        }

        void Clear()
        {
            foreach (var btn in buttons)
            {
                if (btn != null)
                    Destroy(btn.gameObject);
            }
            buttons.Clear();
            buttonBackgrounds.Clear();
            selectedIndex = -1;
        }
    }
}
