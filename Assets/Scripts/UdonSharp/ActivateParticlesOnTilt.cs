using UnityEngine;
using VRChatMigration;

public class ActivateParticlesOnTilt : MonoBehaviour
{
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _UP;
    [SerializeField] private Transform _DOWN;
    [SerializeField] private Transform _Visual;

    public void OnPickupUseDown_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UseThisThing"); // VRChat: SendCustomNetworkEvent
    }

    public void UseThisThing()
    {
        var main = _particleSystem.main;
        main.startColor = m_Renderer.material.color;
        if (_particleSystem.isEmitting)
            _particleSystem.Stop();
        else if (_UP != null && _DOWN != null && _UP.position.y - _DOWN.position.y <= 0)
        {
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play();
        }
    }

    public void UnUseThisThing()
    {
        _particleSystem.Stop();
        if (_Visual != null)
            _Visual.localRotation = Quaternion.identity;
    }

    public void OnDrop_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UnUseThisThing"); // VRChat: SendCustomNetworkEvent
    }
}
