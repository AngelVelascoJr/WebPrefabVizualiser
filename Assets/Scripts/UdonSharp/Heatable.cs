using UnityEngine;

public class Heatable : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] UdonBehabiourListenersRef;
    const string ActivateMethod = "OnHeatActivated";
    const string DeactivateMethod = "OnHeatDeactivated";

    private void Start()
    {
        UdonBehabiourListenersRef = GetComponents<MonoBehaviour>();
    }

    public void ActivateCalor()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            if (UdonBehabiourListenersRef[i] != null && UdonBehabiourListenersRef[i] != this)
                LocalUdonEventBridge.SendLocalEvent(UdonBehabiourListenersRef[i], ActivateMethod);
        }
    }

    public void DeactivateCalor()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            if (UdonBehabiourListenersRef[i] != null && UdonBehabiourListenersRef[i] != this)
                LocalUdonEventBridge.SendLocalEvent(UdonBehabiourListenersRef[i], DeactivateMethod);
        }
    }
}
