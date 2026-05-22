using UnityEngine;

namespace PrefabViewer.UI
{
    public static class UiTheme
    {
        public static readonly Color PanelBg = new Color(0.22f, 0.22f, 0.22f, 1f);
        public static readonly Color InspectorPanelBg = new Color(0.22f, 0.22f, 0.22f, 1f);
        public static readonly Color PanelHeader = new Color(0.18f, 0.18f, 0.18f, 1f);
        public static readonly Color RowNormal = new Color(0.28f, 0.28f, 0.28f, 1f);
        public static readonly Color RowSelected = new Color(0.24f, 0.47f, 0.74f, 1f);
        public static readonly Color RowHover = new Color(0.35f, 0.35f, 0.35f, 1f);
        public static readonly Color TextPrimary = new Color(0.9f, 0.9f, 0.9f, 1f);
        public static readonly Color TextMuted = new Color(0.65f, 0.65f, 0.65f, 1f);
        public static readonly Color Border = new Color(0.1f, 0.1f, 0.1f, 1f);
        public static readonly Color SceneViewBg = new Color(0.2f, 0.2f, 0.2f, 1f);

        public const float QuadrantHeaderHeight = 32f;
        public const float QuadrantHeaderFontSize = 20f;

        public static readonly Color InspectorFieldBg = new Color(0.165f, 0.165f, 0.165f, 1f);
        public static readonly Color InspectorFieldBorder = new Color(0.29f, 0.29f, 0.29f, 1f);
        public static readonly Color InspectorLabel = new Color(0.7f, 0.7f, 0.7f, 1f);
        public static readonly Color InspectorCheckboxBg = new Color(0.22f, 0.22f, 0.22f, 1f);
        public static readonly Color InspectorCheckboxOn = new Color(0.55f, 0.65f, 0.78f, 1f);

        public const float InspectorLabelWidth = 130f;
        public const float InspectorRowHeight = 20f;
        public const float InspectorFieldHeight = 18f;
    }
}
