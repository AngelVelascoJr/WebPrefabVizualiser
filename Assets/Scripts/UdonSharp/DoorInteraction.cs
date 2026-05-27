using UnityEngine;
using VRChatMigration;

public class DoorInteraction : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] Rigidbody _RigidbodyDoor;
    [SerializeField] GameObject Door;
    [SerializeField] GameObject Manija;
    [SerializeField] GameObject[] NonVrPositions;
    [SerializeField] bool isOpened = false;
    bool updateDoorState;
    bool VRHolds = false;

    // VRChat: OnPickup
    public void OnPickup_VRChat()
    {
        isOpened = !isOpened;
        updateDoorState = true;
    }

    private void Update()
    {
        if (updateDoorState && Door != null && NonVrPositions != null && NonVrPositions.Length > 1)
        {
            var target = isOpened ? NonVrPositions[0] : NonVrPositions[1];
            Vector3 dir = new Vector3(target.transform.position.x, Door.transform.position.y, target.transform.position.z);
            Door.transform.LookAt(dir);
            updateDoorState = false;
        }
        if (!VRHolds && Manija != null)
            transform.position = Manija.transform.position;
    }

    // VRChat: OnDrop
    public void OnDrop_VRChat()
    {
        VRHolds = false;
    }
}
