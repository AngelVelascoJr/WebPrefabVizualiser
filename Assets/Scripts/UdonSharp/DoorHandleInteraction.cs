using UnityEngine;
using VRChatMigration;

public class DoorHandleInteraction : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] Vector3[] DoorRotation;
    [SerializeField] Transform Door;
    [SerializeField] GameObject GrabbableHandle;
    [SerializeField] Transform HandleDefaultPosition;
    bool IsDoorOpened;
    bool IsPickedUp;

    private void Update()
    {
        if (IsPickedUp && Door != null && DoorRotation != null && DoorRotation.Length > 1)
        {
            Door.localRotation = Quaternion.Euler(DoorRotation[IsDoorOpened ? 1 : 0]);
            IsPickedUp = false;
        }
    }

    // VRChat: OnPickup
    public void OnPickup_VRChat()
    {
        IsDoorOpened = !IsDoorOpened;
        IsPickedUp = true;
    }

    // VRChat: OnDrop
    public void OnDrop_VRChat()
    {
        IsPickedUp = false;
        if (GrabbableHandle != null && HandleDefaultPosition != null)
            GrabbableHandle.transform.SetPositionAndRotation(HandleDefaultPosition.position, HandleDefaultPosition.rotation);
    }
}
