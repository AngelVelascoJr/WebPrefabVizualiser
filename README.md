# Web Prefab Visualiser


Runtime prefab viewer for **Unity WebGL**: pick a prefab from a catalog, browse its **Hierarchy**, and read a **read-only Inspector** in the browser. Deployed via **GitHub Pages** (`docs/` folder).

## Requirements

- Unity **2022.3.62f3** (or compatible 2022.3 LTS)
- **WebGL Build Support** module (Unity Hub → Add modules)
- Git + GitHub account (for Pages)

## Quick start (Editor)

1. Open this project in Unity (wait for script compile).
2. If sample prefabs are missing: **Prefab Viewer → Setup Project** (optional; assets may already exist in repo).
3. Open `Assets/Scenes/Main.unity` and press **Play**.

> **WebGL build while the Editor is open:** use **Prefab Viewer → Build WebGL to docs** (batch CLI requires closing Unity first).

## Add prefabs to the catalog

1. Place prefabs under `Assets/Prefabs/`.
2. Run **Prefab Viewer → Setup Project** again (refreshes `Assets/Resources/PrefabCatalog.asset`),  
   **or** open the catalog asset and add entries manually (`displayName`, `category`, `prefab` reference).
3. Rebuild WebGL so new prefabs are included in the player build.

## WebGL build (local)

1. **Prefab Viewer → Build WebGL to docs**  
   Output: project folder `docs/` (`index.html`, `Build/`, `TemplateData/`).

2. Test with a local static server (do not open `index.html` via `file://`):

```bash
npx serve docs -p 8080
```

Open http://localhost:8080

### Batch build (CLI)

```powershell
& "C:\Program Files\Unity\Hub\Editor\2022.3.62f3\Editor\Unity.exe" `
  -batchmode -quit -nographics `
  -projectPath "d:\Documentos\Unity\WebPrefabVizualiser" `
  -executeMethod PrefabViewer.Editor.PrefabViewerSetup.SetupAndBuildBatch `
  -logFile "build.log"
```

## GitHub Pages

1. Push the repo to GitHub.
2. **Settings → Pages → Build and deployment**
   - Source: **Deploy from a branch**
   - Branch: `main` (or your default)
   - Folder: **`/docs`**
3. After the workflow or manual build, the site is available at:

   `https://<your-username>.github.io/WebPrefabVizualiser/`

   (replace with your GitHub username and repository name)

The custom WebGL template adjusts `buildUrl` for project-site subpaths (e.g. `/WebPrefabVizualiser/`).

## Project layout

| Path | Purpose |
|------|---------|
| `Assets/Scripts/PrefabViewer/` | Runtime viewer + Editor setup |
| `Assets/Prefabs/` | Catalog prefabs |
| `Assets/Resources/PrefabCatalog.asset` | ScriptableObject catalog |
| `Assets/Scenes/Main.unity` | Build scene |
| `Assets/WebGLTemplates/Custom/` | GitHub Pages–friendly WebGL template |
| `docs/` | WebGL build output for GitHub Pages |

## Limitations (WebGL)

- Only prefabs **included in the build** (via catalog) can be viewed.
- Inspector is **read-only** and uses reflection (not the Unity Editor inspector).
- Some field types show `[Unsupported type]`.

## CI

`.github/workflows/verify-docs.yml` checks that `docs/index.html` exists on push (build is still produced locally or via Unity menu until GameCI is configured).
