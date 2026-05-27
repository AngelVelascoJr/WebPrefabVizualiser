using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback para el componente VRChat Pickup (sin SDK).
    /// Mantiene nombres de campos para conservar datos serializados en prefabs.
    /// </summary>
    public class VRCPickupFallback : MonoBehaviour
    {
        public int MomentumTransferMethod;
        public int DisallowTheft;

        public Transform ExactGun;
        public Transform ExactGrip;

        public bool allowManipulationWhenEquipped;
        public int orientation;
        public bool AutoHold;

        public string InteractionText;
        public string UseText;

        public int useEventBroadcastType;
        public string UseDownEventName;
        public string UseUpEventName;

        public int pickupDropEventBroadcastType;
        public string PickupEventName;
        public string DropEventName;

        public float ThrowVelocityBoostMinSpeed = 1f;
        public float ThrowVelocityBoostScale = 1f;

        public Object currentlyHeldBy;
        public bool pickupable = true;
        public float proximity = 1f;

        // VRChat-style callbacks kept as no-op for compatibility.
        public void OnPickupUseDown_VRChat() { }
        public void OnPickupUseUp_VRChat() { }
        public void OnPickup_VRChat() { }
        public void OnDrop_VRChat() { }
    }
}

