#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrefabViewer.Editor
{
    public static class PrefabViewerSetup
    {
        const string PrefabsPath = "Assets/Prefabs";
        const string ResourcesPath = "Assets/Resources";
        const string CatalogPath = ResourcesPath + "/PrefabCatalog.asset";
        const string MainScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("Prefab Viewer/Setup Project")]
        public static void SetupProject()
        {
            if (!RunSetup())
            {
                EditorUtility.DisplayDialog("Prefab Viewer", "Setup failed. Check the Console.", "OK");
                return;
            }
            EditorUtility.DisplayDialog("Prefab Viewer", "Project setup complete.\n\nOpen Assets/Scenes/Main.unity and press Play.", "OK");
        }

        [MenuItem("Prefab Viewer/Build WebGL to docs")]
        public static void BuildWebGL()
        {
            var ok = RunWebGLBuild();
            if (ok)
                EditorUtility.DisplayDialog("WebGL Build", "Build succeeded:\n" + GetDocsOutputPath(), "OK");
            else
                EditorUtility.DisplayDialog("WebGL Build", "Build failed. Check the Console.", "OK");
        }

        /// <summary>Batchmode: -executeMethod PrefabViewer.Editor.PrefabViewerSetup.SetupAndBuildBatch</summary>
        public static void SetupAndBuildBatch()
        {
            var setupOk = RunSetup();
            if (!setupOk)
            {
                EditorApplication.Exit(1);
                return;
            }
            EditorApplication.Exit(RunWebGLBuild() ? 0 : 1);
        }

        public static bool RunSetup()
        {
            EnsureFolders();
            CreateSamplePrefabs();
            var catalog = CreateOrUpdateCatalog(null);
            CreateMainScene(catalog);
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return File.Exists(CatalogPath) && File.Exists(MainScenePath);
        }

        public static bool RunWebGLBuild()
        {
            if (!File.Exists(MainScenePath) && !RunSetup())
                return false;

            ConfigureBuildSettings();
            var output = GetDocsOutputPath();
            if (Directory.Exists(output))
            {
                foreach (var file in Directory.GetFiles(output))
                    File.Delete(file);
                foreach (var dir in Directory.GetDirectories(output))
                    Directory.Delete(dir, true);
            }
            else
            {
                Directory.CreateDirectory(output);
            }

            var options = new BuildPlayerOptions
            {
                scenes = new[] { MainScenePath },
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var ok = report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded;
            if (ok)
                Debug.Log("WebGL build output: " + output);
            else
                Debug.LogError("WebGL build failed: " + report.summary.result);
            return ok;
        }

        static string GetDocsOutputPath() => Path.Combine(Directory.GetCurrentDirectory(), "docs");

        static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Scripts"))
                AssetDatabase.CreateFolder("Assets", "Scripts");
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/PrefabViewer"))
                AssetDatabase.CreateFolder("Assets/Scripts", "PrefabViewer");
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/PrefabViewer/Editor"))
                AssetDatabase.CreateFolder("Assets/Scripts/PrefabViewer", "Editor");
        }

        static GameObject CreateSamplePrefabs()
        {
            var empty = CreateEmptyPrefab("EmptyPrefab");
            var nested = CreateNestedPrefab();
            var complex = CreateComplexPrefab();
            AssetDatabase.SaveAssets();
            return complex;
        }

        static GameObject CreateEmptyPrefab(string name)
        {
            var go = new GameObject(name);
            var path = $"{PrefabsPath}/{name}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject CreateNestedPrefab()
        {
            var root = new GameObject("NestedPrefab");
            var childA = new GameObject("Armature");
            childA.transform.SetParent(root.transform);
            var childB = new GameObject("Mesh");
            childB.transform.SetParent(childA.transform);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
            cube.transform.SetParent(childB.transform);
            cube.transform.localScale = Vector3.one * 0.5f;

            var path = $"{PrefabsPath}/NestedPrefab.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static GameObject CreateComplexPrefab()
        {
            var root = new GameObject("ComplexPrefab");
            root.AddComponent<SamplePrefabComponent>();
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform);
            var collider = root.AddComponent<SphereCollider>();
            collider.radius = 1f;

            var path = $"{PrefabsPath}/ComplexPrefab.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        static PrefabCatalog CreateOrUpdateCatalog(GameObject _)
        {
            var catalog = AssetDatabase.LoadAssetAtPath<PrefabCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<PrefabCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsPath });
            var entries = new List<PrefabCatalog.Entry>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;
                entries.Add(new PrefabCatalog.Entry
                {
                    displayName = prefab.name,
                    category = "Samples",
                    prefab = prefab
                });
            }

            var so = new SerializedObject(catalog);
            var prop = so.FindProperty("entries");
            prop.ClearArray();
            for (var i = 0; i < entries.Count; i++)
            {
                prop.InsertArrayElementAtIndex(i);
                var el = prop.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("displayName").stringValue = entries[i].displayName;
                el.FindPropertyRelative("category").stringValue = entries[i].category;
                el.FindPropertyRelative("prefab").objectReferenceValue = entries[i].prefab;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        static void CreateMainScene(PrefabCatalog catalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var appGo = new GameObject("PrefabViewerApp");
            var app = appGo.AddComponent<PrefabViewerApp>();
            var so = new SerializedObject(app);
            so.FindProperty("catalog").objectReferenceValue = catalog;
            so.ApplyModifiedPropertiesWithoutUndo();

            var scenePath = MainScenePath;
            if (!File.Exists(scenePath))
                EditorSceneManager.SaveScene(scene, scenePath);
            else
                EditorSceneManager.SaveScene(scene, scenePath);
        }

        static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MainScenePath, true) };

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.initialMemorySize = 64;
            PlayerSettings.defaultWebScreenWidth = 1280;
            PlayerSettings.defaultWebScreenHeight = 720;

            var template = "PROJECT:Custom";
            if (AssetDatabase.IsValidFolder("Assets/WebGLTemplates/Custom"))
                PlayerSettings.WebGL.template = template;
        }
    }
}
#endif
