using UnityEngine;
using VRChatMigration;

public class BoteBasura : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);
        }
        // VRChat: VRCObjectSync.Respawn — not available in visualizer
    }
}
