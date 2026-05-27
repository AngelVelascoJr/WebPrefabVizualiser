using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback para el componente de audio espacial del SDK VRC.
    /// Se serializa en prefabs con campos Gain/Far/Near/etc.
    /// </summary>
    public class VRCSpatialAudioSourceFallback : MonoBehaviour
    {
        public float Gain = 0f;
        public float Far = 0f;
        public float Near = 0f;
        public float VolumetricRadius = 0f;
        public int EnableSpatialization = 1;
        public int UseAudioSourceVolumeCurve = 0;
    }
}

