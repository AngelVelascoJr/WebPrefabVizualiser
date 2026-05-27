
using UnityEngine;

public class CubreLijaSnap : MonoBehaviour
{
    [SerializeField] public bool CubreLijaLoaded;
    [SerializeField] bool Stayed;

    [SerializeField] public GameObject CubreLijaGo = null;
    [SerializeField] public GameObject CubreLija = null;

    private void Update()
    {
        if (CubreLija == null)
        {
            if (CubreLijaGo != null)
            {
                // VRChat: CubreLija = CubreLijaGo.GetComponent<VRC_Pickup>();
            }
        }
    }

    public void OnCubreLijaSnap(Transform go)
    {
        CubreLijaLoaded = true;
        go.SetParent(gameObject.transform);
        CubreLijaGo = go.gameObject;
        // VRChat: CubreLija = CubreLijaGo.GetComponent<VRC_Pickup>();
        go.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Pickup");
    }

    public void RemoveCubreLija(Transform go)
    {
        CubreLijaLoaded = false;
        // VRChat: go.GetComponent<VRC_Pickup>().pickupable = true;
        go.parent = null;
        CubreLijaGo = null;
        CubreLija = null;
    }


    private void OnTriggerStay(Collider other)
    {
        if(CubreLijaLoaded) {return; }
        if (CubreLija != null) return;
        if (!other.GetComponent<CubreLijaBehabiour>()) { return; }
        // VRChat: if (other.gameObject.GetComponent<VRC_Pickup>().currentPlayer != null) { return; }
        if (!Stayed)
        {
            OnCubreLijaSnap(other.gameObject.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<CubreLijaBehabiour>())
            return;
        if(CubreLijaGo != null)
        {
            RemoveCubreLija(other.gameObject.transform);
            Stayed = false;
        }
    }

}
