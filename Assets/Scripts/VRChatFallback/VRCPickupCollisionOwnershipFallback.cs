using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback para el pequeño componente de VRChat que solo expone
    /// AllowCollisionOwnershipTransfer en prefabs.
    /// </summary>
    public class VRCPickupCollisionOwnershipFallback : MonoBehaviour
    {
        public int AllowCollisionOwnershipTransfer = 1;
    }
}

