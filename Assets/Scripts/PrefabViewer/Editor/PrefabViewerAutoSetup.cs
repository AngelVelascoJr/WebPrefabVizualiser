#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PrefabViewer.Editor
{
    [InitializeOnLoad]
    static class PrefabViewerAutoSetup
    {
        const string CatalogPath = "Assets/Resources/PrefabCatalog.asset";
        const string MainScenePath = "Assets/Scenes/Main.unity";

        static PrefabViewerAutoSetup()
        {
            EditorApplication.delayCall += TryAutoSetup;
        }

        static void TryAutoSetup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildPipeline.isBuildingPlayer)
                return;
            if (AssetDatabase.LoadAssetAtPath<PrefabCatalog>(CatalogPath) != null &&
                System.IO.File.Exists(MainScenePath))
                return;

            if (System.IO.File.Exists(CatalogPath))
                return;

            Debug.Log("Prefab Viewer: running one-time project setup (Prefab Viewer > Setup Project).");
            PrefabViewerSetup.RunSetup();
        }
    }
}
#endif
