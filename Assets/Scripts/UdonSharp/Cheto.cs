using UnityEngine;
using VRChatMigration;

public class Cheto : MonoBehaviour
{
    [SerializeField] private bool HideOnUse;
    // VRChat: UdonSynced
    [SerializeField] private bool Used;
    // VRChat: UdonSynced
    [SerializeField] private bool Finished;
    [SerializeField] private float timer = 3f;

    [SerializeField] private Vector3 _ref;
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;
    [SerializeField] private GameObject _visual;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        _ref = transform.position;
        if ((Used || Finished) && HideOnUse)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Used && !Finished && timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Finished = true;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnDrop_VRChat()
    {
        if (_particleSystem != null) _particleSystem.Emit(3);
        if (_audioSource != null) _audioSource.Play();
        if (HideOnUse)
        {
            if (_visual != null) _visual.SetActive(false);
            Used = true;
        }
        else
            transform.position = _ref;
    }

    public void OnDeserialization_VRChat()
    {
        if ((Used || Finished) && HideOnUse)
            gameObject.SetActive(false);
    }
}
