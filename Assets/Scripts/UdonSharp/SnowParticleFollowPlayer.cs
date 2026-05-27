using UnityEngine;
using VRChatMigration;

public class SnowParticleFollowPlayer : MonoBehaviour
{
    [VRChatLegacy("VRCObjectPool", "VRChat object pool")]
    [SerializeField] string _legacyPool;

    // VRChat: VRCPlayerApi target
    public Transform target;

    private void Update()
    {
        if (target != null)
            transform.position = target.position;
    }

    public void OnPlayerLeft_VRChat()
    {
        gameObject.SetActive(false);
    }
}
