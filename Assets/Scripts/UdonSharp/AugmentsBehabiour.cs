using UnityEngine;

public class AugmentsBehabiour : MonoBehaviour
{
    int count = 0;
    MicroscopeBehabiour microscopeBehabiour;
    [SerializeField] GameObject Revolver;
    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;


    private void Start()
    {
        microscopeBehabiour = gameObject.GetComponentInParent<MicroscopeBehabiour>();
    }

    public void Interact_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "OnInteract"); // VRChat: SendCustomNetworkEvent
    }

    public void OnInteract()
    {
        count++;
        if (count == 4)
            count = 0;
        microscopeBehabiour.OnAugmentChange(count);
        _audioSource.PlayOneShot(_audioClip);
        Revolver.transform.rotation = Quaternion.Euler(0, count * 90, 0);
    }
}
