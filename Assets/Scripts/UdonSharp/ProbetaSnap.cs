using UnityEngine;

public class ProbetaSnap : MonoBehaviour
{
    public bool ProbetaLoaded;
    public bool Stayed;

    [SerializeField] public GameObject ProbetaGO = null;

    public void OnProbetaSnap(Transform go)
    {
        ProbetaLoaded = true;
        go.SetParent(transform);
        ProbetaGO = go.gameObject;
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null)
            rb.excludeLayers = LayerMask.GetMask("Pickup");
        var probe = GetComponentInChildren<InteractProbe>();
        if (probe != null)
        {
            probe.DisableCanva();
            probe.gameObject.SetActive(false);
        }
    }

    public void RemoveProbeta(Transform go)
    {
        ProbetaLoaded = false;
        var RB = go.GetComponent<Rigidbody>();
        if (RB != null)
        {
            RB.excludeLayers = LayerMask.GetMask("Nothing");
            RB.constraints = RigidbodyConstraints.None;
        }
        go.parent = null;
        ProbetaGO = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (ProbetaGO != null) return;
        if (!other.GetComponent<ProbeBehabiour>()) return;
        if (!Stayed)
        {
            OnProbetaSnap(other.transform);
            Stayed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ProbeBehabiour>())
            return;
        RemoveProbeta(other.transform);
        Stayed = false;
    }
}
