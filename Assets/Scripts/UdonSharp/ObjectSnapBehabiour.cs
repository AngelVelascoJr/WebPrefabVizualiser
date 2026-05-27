using System.Collections.Generic;
using UnityEngine;

public class ObjectSnapBehabiour : MonoBehaviour
{
    [Header("Snap config")]
    [SerializeField] bool SnapPosition;
    [SerializeField] bool SnapRotation;
    [SerializeField] bool freezePosition;

    [SerializeField] Vector3 PositionOffset;
    [SerializeField] Vector3 RotationOffset;

    [Header("Snap ref")]
    [SerializeField] float DetectionRadius;
    [SerializeField] float MinActivationDistance;

    [SerializeField] Vector3 DetectionOffset;

    [Header("OnSnap References")]
    [SerializeField] GameObject[] GOListeners;
    [SerializeField] MonoBehaviour[] UdonBehabiourListenersRef;

    [Header("Events configuration")]
    [SerializeField] string[] EventNames;

    [SerializeField] Collider[] GameObjectsNear;

    [SerializeField] GameObject objectToSnap;
    [SerializeField] GameObject objectToIgnoreSnap;

    [Header("Audio Config")]
    [SerializeField] private bool _AudioOnSnap;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;


    private void Start()
    {
        var list = new List<MonoBehaviour>();
        for (int i = 0; i < GOListeners.Length; i++)
        {
            if (GOListeners[i] == null) continue;
            list.AddRange(GOListeners[i].GetComponents<MonoBehaviour>());
        }
        UdonBehabiourListenersRef = list.ToArray();
    }

    public void SnapFunc()
    {
        GameObject GOToSnap = null;
        GameObjectsNear = Physics.OverlapSphere(transform.position, DetectionRadius);

        float Distance = float.PositiveInfinity;
        var GrabablesNear = "";
        foreach (Collider item in GameObjectsNear)
        {
            if(null == item) continue;
            if(item.gameObject.GetComponent<placeable>() == null) continue;
            if (objectToSnap != null)
            {
                if(item.gameObject != objectToSnap)
                {
                    Debug.Log(item + " no es");
                    continue;
                }
            }
            if (objectToIgnoreSnap != null)
            {
                if (item.gameObject == objectToIgnoreSnap)
                {
                    Debug.Log(item + " Ingnorado");
                    continue;
                }
            }
            GrabablesNear += item.name + " ";
            var dist = Vector3.Distance(transform.position + DetectionOffset, item.transform.position);
            if (Distance >= dist && item != gameObject)
            {
                Distance = dist;
                GOToSnap = item.gameObject;
                Debug.Log(this + gameObject.name + " : " + GOToSnap + " Seleccionado");
            }
        }

        if (Distance < MinActivationDistance)
        {
            Debug.Log("<color=#dd9900ff>ObjectSnapBehabiour</color> already have a element, exiting function");
            return;
        }

        //Debug.Log(gameObject.name+": = "+GrabablesNear);
        if(GOToSnap == null)
        {
            //Debug.LogWarning("No hay objetos cercanos, ¿hace falta asignar 'placeable'?");
            return;
        }
        else
        {
            Debug.Log("Object found: " + GOToSnap.name);
        }


            var GOpos = gameObject.transform.position;
        var GOrot = gameObject.transform.rotation;
        if(SnapPosition)
        {
            GOToSnap.transform.position = GOpos + PositionOffset;
            if(freezePosition)
                GOToSnap.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
        if(SnapRotation)
        {
            GOToSnap.transform.rotation = Quaternion.Euler(GOrot.eulerAngles + RotationOffset);
        }

        if(_AudioOnSnap)
        {
            _audioSource.PlayOneShot(_audioClip);
        }

        CallEvent();
    }

    private void CallEvent()
    {
        for (int i = 0; i < UdonBehabiourListenersRef.Length; i++)
        {
            for (int j = 0; j < EventNames.Length; j++)
            {
                LocalUdonEventBridge.SendLocalEvent(UdonBehabiourListenersRef[i], EventNames[j]); // VRChat: SendCustomNetworkEvent
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f,1f,0f,0.7f);
        Gizmos.DrawSphere(transform.position + DetectionOffset, DetectionRadius);
        Gizmos.color = new Color(1f, 0f, 0f, 0.7f);
        Gizmos.DrawSphere(transform.position + DetectionOffset, MinActivationDistance);
    }

}
