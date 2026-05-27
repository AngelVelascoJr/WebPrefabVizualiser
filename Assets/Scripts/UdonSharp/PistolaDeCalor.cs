using UnityEngine;
using VRChatMigration;

public class PistolaDeCalor : MonoBehaviour
{
    public CapsuleCollider collisionPistol;
    [SerializeField] private bool Used;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _particleSystem;
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup;

    [Header("Audio Config")]
    [SerializeField] private AudioSource _StartStopAudioSource;
    [SerializeField] private AudioSource _LoopAudioSource;
    [SerializeField] private AudioClip _StartAudioClip;
    [SerializeField] private AudioClip _LoopAudioClip;
    [SerializeField] private AudioClip _StopAudioClip;
    [SerializeField] private bool AudioStart;
    [SerializeField] private float StartTimer;
    [SerializeField] private bool AudioLoop;

    private static readonly float[][] ColliderByUsed = {
        new[] { -0.2f, 0.01f, 0.15f },
        new[] { 0.1f, 0.05f, 0.25f }
    };

    private void Start()
    {
        ApplyColliderShape(Used);
    }

    private void Update()
    {
        AudioSystem();
    }

    void ApplyColliderShape(bool used)
    {
        if (collisionPistol == null)
            return;
        var shape = ColliderByUsed[used ? 1 : 0];
        collisionPistol.center = new Vector3(0, 0, shape[0]);
        collisionPistol.radius = shape[1];
        collisionPistol.height = shape[2];
    }

    private void AudioSystem()
    {
        if (Used & !AudioStart && !AudioLoop)
        {
            AudioStart = true;
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StartAudioClip;
            _StartStopAudioSource.Play();
            StartTimer = 0f;
        }

        if (StartTimer > 0.9f * _StartAudioClip.length && !AudioLoop)
        {
            AudioStart = false;
            AudioLoop = true;
            _StartStopAudioSource.Stop();
            _LoopAudioSource.clip = _LoopAudioClip;
            _LoopAudioSource.Play();
        }

        if (!Used && (AudioStart || AudioLoop))
        {
            AudioStart = false;
            AudioLoop = false;
            StartTimer = 0f;
            _LoopAudioSource.Stop();
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StopAudioClip;
            _StartStopAudioSource.Play();
        }

        if (AudioStart)
            StartTimer += Time.deltaTime;
    }

    public void OnPickupUseDown_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "usePistol"); // VRChat: SendCustomNetworkEvent
    }

    public void usePistol()
    {
        Used = !Used;
        ApplyColliderShape(Used);
        if (Used)
            _particleSystem.Play();
        else
            _particleSystem.Stop();
        _animator.SetBool("Used", Used);
    }

    private void OnTriggerEnter(Collider other)
    {
        Heatable[] heatables = other.GetComponentsInChildren<Heatable>();
        Heatable heatableTarget = null;
        string direccion = null;

        if (other.GetComponent<OrientationChecker>() != null)
            direccion = other.GetComponent<OrientationChecker>().checkOrientation();

        if (direccion == "Up" && heatables.Length > 1) heatableTarget = heatables[1];
        else if (direccion == "Down" && heatables.Length > 0) heatableTarget = heatables[0];

        if (heatableTarget != null)
            LocalUdonEventBridge.SendLocalEvent(heatableTarget, "ActivateCalor"); // VRChat: SendCustomNetworkEvent
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Heatable heatable in other.GetComponentsInChildren<Heatable>())
            LocalUdonEventBridge.SendLocalEvent(heatable, "DeactivateCalor"); // VRChat: SendCustomNetworkEvent
    }
}
