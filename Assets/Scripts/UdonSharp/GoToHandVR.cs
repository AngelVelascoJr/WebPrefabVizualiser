using UnityEngine;

public class GoToHandVR : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform LHandHandle;
    [SerializeField] Transform RHandHandle;

    void Start()
    {
        // VRChat: hand tracking — disabled in visualizer
        if (LHandHandle != null)
            LHandHandle.gameObject.SetActive(false);
        if (RHandHandle != null)
            RHandHandle.gameObject.SetActive(false);
        enabled = false;
    }
}
