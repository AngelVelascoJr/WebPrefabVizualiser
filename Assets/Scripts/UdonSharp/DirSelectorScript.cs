using UnityEngine;
using VRChatMigration;

public class DirSelectorScript : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] private Transform GripDirVr;
    [SerializeField] private Transform GripDirPC;

    private void Start()
    {
        // VRChat: pickup grip direction by VR/PC — visualizer uses PC grip only
        if (GripDirPC != null)
            Debug.Log("DirSelectorScript: PC grip reference assigned");
    }
}
