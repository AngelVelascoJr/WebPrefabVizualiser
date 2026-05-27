using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [SerializeField] private Transform playerRespawnPositionTransform;
    [SerializeField] private ParticleSystem playerRespawnFogParticleSystem;

    // VRChat: OnPlayerTriggerEnter
    public void OnPlayerTriggerEnter_VRChat()
    {
        if (playerRespawnFogParticleSystem != null)
            playerRespawnFogParticleSystem.Play();
    }
}
