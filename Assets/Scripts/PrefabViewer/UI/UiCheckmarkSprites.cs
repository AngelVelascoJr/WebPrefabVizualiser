using UnityEngine;

namespace PrefabViewer.UI
{
    static class UiCheckmarkSprites
    {
        const string CheckedPath = "PrefabViewer/UI/Checkmark_Checked";
        const string UncheckedPath = "PrefabViewer/UI/Checkmark_UnChecked";

        static Sprite _checked;
        static Sprite _unchecked;

        public static Sprite Checked => _checked ??= Load(CheckedPath);
        public static Sprite Unchecked => _unchecked ??= Load(UncheckedPath);

        static Sprite Load(string path)
        {
            var sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
                return sprite;

            var all = Resources.LoadAll<Sprite>(path);
            return all != null && all.Length > 0 ? all[0] : null;
        }
    }
}
