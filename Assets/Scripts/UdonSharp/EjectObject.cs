
using UnityEngine;
using VRChatMigration;

public class EjectObject : MonoBehaviour
{

    [SerializeField] private GameObject objectEjectable;
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup_objectPickup;


    [SerializeField] private Vector3 RotorToObjSize;
    [SerializeField] private Vector3 Up;
    [SerializeField] private Vector3 VectorDeDireccion;

    private void Update()
    {
        if (gameObject.GetComponent<LijaRotation>() != null)
        {
            if (gameObject.GetComponent<LijaRotation>().Rotating)
            {
                ejectObject();
            }
        }

        if (gameObject.GetComponent<PulidoraScript>() != null)
        {
            if (gameObject.GetComponent<PulidoraScript>().Rotating)
            {
                ejectObject();
            }
        }
    }

    private void ejectObject()
    {
        if (objectEjectable != null)
        {
            RotorToObjSize = new Vector3(objectEjectable.transform.position.x - gameObject.transform.position.x, 0f, objectEjectable.transform.position.z - gameObject.transform.position.z);
            Up = objectEjectable.transform.up;
            VectorDeDireccion = Vector3.Cross(RotorToObjSize, Up);
            //Debug.Log("Vector asigned");

            var rb = objectEjectable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                VectorDeDireccion.y = Mathf.Abs(VectorDeDireccion.y);
                rb.AddForce(VectorDeDireccion.normalized * 200f);
                //Debug.Log("Object Ejected");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Ejectable>() != null)
        {
            objectEjectable = other.gameObject;
            // VRChat: objectPickup = other.GetComponent<VRC_Pickup>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ejectable>() != null)
        {
            objectEjectable = null;
        }
    }
}
