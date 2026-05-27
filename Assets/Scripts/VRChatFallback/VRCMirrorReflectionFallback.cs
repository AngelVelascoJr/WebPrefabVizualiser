using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback mínimo para VRCMirrorReflection (sin SDK).
    /// Mantiene nombres de campos serializados en prefabs.
    /// </summary>
    public class VRCMirrorReflectionFallback : MonoBehaviour
    {
        public int m_DisablePixelLights = 1;
        public int TurnOffMirrorOcclusion = 1;
        public LayerMask m_ReflectLayers;
        public int mirrorResolution = 256;
        public int maximumAntialiasing = 1;
        public Shader customShader;
        public int cameraClearFlags;
        public Material customSkybox;
        public Color customClearColor = Color.black;
    }
}

