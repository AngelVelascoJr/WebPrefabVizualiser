using UnityEngine;
using VRChatMigration;

public class ActivateParticlesOnPickupUse : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] private ParticleSystem _particleSystem;

    // VRChat: OnPickupUseDown
    public void OnPickupUseDown_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UseThisThing"); // VRChat: SendCustomNetworkEvent
    }

    public void UseThisThing()
    {
        if (_particleSystem != null && !_particleSystem.isPlaying)
            _particleSystem.Play();
    }

    // VRChat: OnDrop
    public void OnDrop_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UnUseThisThing"); // VRChat: SendCustomNetworkEvent
    }

    public void UnUseThisThing()
    {
        if (_particleSystem != null)
            _particleSystem.Stop();
    }
}
