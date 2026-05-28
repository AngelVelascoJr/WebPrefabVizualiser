using UnityEngine;

namespace PrefabViewer
{
    /// <summary>
    /// Referenced from Resources so WebGL builds retain ShaderGraph shader variants used by prefabs.
    /// </summary>
    public sealed class WebGlShaderWarmup : ScriptableObject
    {
        public Material[] materials;
    }
}
