using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback stub-only para VRCObjectPool (sin red, sin instanciación).
    /// </summary>
    public class VRCObjectPoolFallback : MonoBehaviour
    {
        public GameObject TryToSpawn()
        {
            return null;
        }

        public void Return(GameObject obj)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}

