using TMPro;
using UnityEngine;

public class LijaCuadrada : MonoBehaviour
{
    public LijaDataholder ReferenceGOComponent;
    public GameObject[] Lijas;
    public int TamañoDeGrano;
    public TextMeshProUGUI text;
    [SerializeField] private GameObject[] Placers;

    [Header("debug tools")]
    public bool debug;
    public Rigidbody rb;

    private void Start()
    {
        if (ReferenceGOComponent == null)
            Debug.LogError($"{this}: No ReferenceGOComponent found");
        if (text != null)
            text.text = TamañoDeGrano.ToString();
        var mr = GetComponent<MeshRenderer>();
        if (mr != null)
            mr.material.SetFloat("_TamanioDeLija", TamañoDeGrano);
    }

    private void Update()
    {
        if (!debug) return;
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
            Debug.Log("velocity: " + rb.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<TijerasBehabiour>())
            LocalUdonEventBridge.SendLocalEvent(this, "OnTijeraTriggerNE"); // VRChat: SendCustomNetworkEvent
    }

    void SpawnLijaFromTemplate()
    {
        if (!LijaDataholder.LijaDict.TryGetValue(TamañoDeGrano, out int matIndex) || Lijas == null || matIndex >= Lijas.Length || Lijas[matIndex] == null)
        {
            Debug.LogWarning($"No lija de tamaño {TamañoDeGrano} en el diccionario");
            return;
        }
        var go = Instantiate(Lijas[matIndex], transform.position, transform.rotation);
        var behaviour = go.GetComponent<LijaCircularBehabiour>();
        if (behaviour != null)
            behaviour.OnPoolSpawn(ref ReferenceGOComponent, TamañoDeGrano);
        if (Placers.Length != 0)
        {
            var caller = go.GetComponent<EventCallerOnHoldOnDrop>();
            if (caller != null)
                caller.SetPlacers(Placers);
        }
        gameObject.SetActive(false);
    }

    public void OnTijeraTrigger() => SpawnLijaFromTemplate();

    public void OnTijeraTriggerNE() => SpawnLijaFromTemplate();

    public void OnDeserialization_VRChat() { }
}
