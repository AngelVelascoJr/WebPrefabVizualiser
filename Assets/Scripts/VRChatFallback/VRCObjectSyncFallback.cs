using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback para VRCObjectSync. No implementa red; solo permite compilar y
    /// exponer la API mínima usada por scripts migrados.
    /// </summary>
    public class VRCObjectSyncFallback : MonoBehaviour
    {
        public void SetKinematic(bool isKinematic)
        {
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = isKinematic;
        }

        public void Respawn()
        {
            // Stub: sin comportamiento por defecto.
        }
    }
}

