using System.Collections.Generic;
using UnityEngine;

public class EventCallerOnHoldOnDrop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] GOListeners;
    [SerializeField] MonoBehaviour[] UdonBehabiourListenersDebugVisualization;
    [Header("Configuration")]
    [SerializeField] string[] OnPickupEventNames;
    [SerializeField] string[] OnDropEventNames;

    private void Start()
    {
        var list = new List<MonoBehaviour>();
        for (int i = 0; i < GOListeners.Length; i++)
        {
            if (GOListeners[i] == null)
                continue;
            list.AddRange(GOListeners[i].GetComponents<MonoBehaviour>());
        }
        UdonBehabiourListenersDebugVisualization = list.ToArray();
    }

    public void OnPickup_VRChat()
    {
        for (int i = 0; i < UdonBehabiourListenersDebugVisualization.Length; i++)
        {
            for (int j = 0; j < OnPickupEventNames.Length; j++)
                LocalUdonEventBridge.SendLocalEvent(UdonBehabiourListenersDebugVisualization[i], OnPickupEventNames[j]); // VRChat: SendCustomNetworkEvent
        }
    }

    public void OnDrop_VRChat()
    {
        for (int i = 0; i < UdonBehabiourListenersDebugVisualization.Length; i++)
        {
            for (int j = 0; j < OnDropEventNames.Length; j++)
                LocalUdonEventBridge.SendLocalEvent(UdonBehabiourListenersDebugVisualization[i], OnDropEventNames[j]); // VRChat: SendCustomNetworkEvent
        }
    }

    public void SetPlacers(GameObject[] Objects)
    {
        Debug.Log("Creada lija con Placers" + Objects.Length);
        if (GOListeners.Length != 0)
            GOListeners = Objects;
    }
}
