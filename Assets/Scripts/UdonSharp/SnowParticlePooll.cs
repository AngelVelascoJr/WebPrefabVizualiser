using UnityEngine;
using VRChatMigration;

public class SnowParticlePooll : MonoBehaviour
{
    [VRChatLegacy("VRCObjectPool", "VRChat object pool")]
    [SerializeField] string _legacyPool;

    // VRChat: OnPlayerJoined — pool spawn disabled in visualizer
    public void OnPlayerJoined_VRChat()
    {
    }
}
