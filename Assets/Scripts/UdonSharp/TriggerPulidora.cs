
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TriggerPulidora : MonoBehaviour
{

    const float TIMER_MAX = 1f;
    float ResetTimer = TIMER_MAX;
    bool startTimer = false;

    [SerializeField] GameObject coliderToShrink;
    Vector3 originalSize;

    private void Start()
    {
        originalSize = coliderToShrink.transform.localScale;
    }

    private void Update()
    {
        if(startTimer)
        {
            ResetTimer -= Time.deltaTime;
            if(ResetTimer < 0 )
            {
                ResetTimer = TIMER_MAX;
                startTimer = false;
                coliderToShrink.transform.localScale = originalSize;
                Debug.LogWarning("Originalited");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ActivateMirror>() == null)
            return;
        // VRChat: Networking.IsOwner check skipped in visualizer

        //LocalUdonEventBridge.SendLocalEvent(this, "Shrink"); // VRChat: SendCustomNetworkEvent
        //Shrink();
    }

    public void Shrink()
    {
        startTimer = true;
        coliderToShrink.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        Debug.LogWarning("shrinked");
    }
}
