using UnityEngine;
using VRChatMigration;

public class SetKinematicFalseOnPickup : MonoBehaviour
{
    [SerializeField] private bool HasObjSync;
    [VRChatLegacy("VRCObjectSync", "Network object sync")]
    [SerializeField] string _legacyVrcObjectSync;
    [SerializeField] private Rigidbody _rigidbody;
    // VRChat: UdonSynced
    private bool WasPicked;

    void Start()
    {
        if (!HasObjSync)
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody != null)
                _rigidbody.isKinematic = true;
        }
    }

    // VRChat: OnPickup
    public void OnPickup_VRChat()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
            _rigidbody.isKinematic = false;
        WasPicked = true;
        enabled = false;
    }

    // VRChat: OnDeserialization
    public void OnDeserialization_VRChat()
    {
        if (!WasPicked)
            return;
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
            _rigidbody.isKinematic = false;
        enabled = false;
    }
}
