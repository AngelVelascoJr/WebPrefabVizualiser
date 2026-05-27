using UnityEngine;

/// <summary>
/// VRChat: sustituto local de SendCustomNetworkEvent / UdonBehaviour.SendCustomNetworkEvent.
/// Solo para compilación; no replica red VRChat.
/// </summary>
public static class LocalUdonEventBridge
{
    public static void SendLocalEvent(MonoBehaviour target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            return;
        target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
    }

    public static void SendLocalEvent(GameObject target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            return;
        target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
    }
}
