using UnityEngine;

public class TijerasBehabiour : MonoBehaviour
{
    public Animator animator;
    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    public void SetAnimTrue()
    {
        animator.SetBool("Used", true);
        _audioSource.PlayOneShot(_audioClip);
    }

    public void SetAnimFalse()
    {
        animator.SetBool("Used", false);
    }

    public void OnPickupUseDown_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "SetAnimTrue"); // VRChat: SendCustomNetworkEvent
        //animator.SetBool("Used", true);
    }

    public void OnPickupUseUp_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "SetAnimFalse"); // VRChat: SendCustomNetworkEvent
    }

    public void OnDrop_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "SetAnimFalse"); // VRChat: SendCustomNetworkEvent
    }

}
