using UnityEngine;
using TMPro;
using System;

public class LijaCircularBehabiour : MonoBehaviour
{
    //Etiqueta, no eliminar

    [SerializeField]// VRChat: UdonSynced
    public int TamañoDeGrano;
    [SerializeField] LijaDataholder referenceDataholder;

    [SerializeField] float humedad = 0;

    public TextMeshProUGUI text;
    public MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;
    private int PhysicsFrameDelay = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //fix for problem with lija moving after droping it, works in conjuction with lijaRotation
    private void FixedUpdate()
    {
        if(rb.constraints == RigidbodyConstraints.FreezePosition)
        {
            PhysicsFrameDelay -= 1;
            if(PhysicsFrameDelay <= 0)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    public float GetHumedad()
    {
        if(humedad <= 0)
            return 0;
        humedad = Mathf.Clamp(humedad, 0, 50);
        return humedad;
    }

    private void OnParticleCollision(GameObject other)
    {

        var elemento = other.GetComponentInParent<BotellaLab>();
        Debug.LogWarning("Lija collision with: " + other.gameObject.name);
        if (elemento != null)
        {
            string tipo = elemento.Tipo;
            if (tipo == "Agua")
            {
                humedad += 2;
            }
        }
    }

    public void OnPoolSpawn(ref LijaDataholder LijaDataholderRef, int tamañoDeGrano)
    {
        TamañoDeGrano = tamañoDeGrano;
        if(LijaDataholderRef != null && LijaDataholderRef != null) 
            referenceDataholder = LijaDataholderRef;
        Debug.Log($"Tamaño de grano set to {TamañoDeGrano}");
        text.text = TamañoDeGrano.ToString();
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[LijaDataholder.LijaDict[tamañoDeGrano]];
        // VRChat: RequestSerialization
    }

    public void ReturnToPool()
    {
        // VRChat: VRCObjectPool.Return
        gameObject.SetActive(false);
    }

    public void OnDeserialization_VRChat()
    {
        if (referenceDataholder == null)
        {
            Debug.LogWarning($"No referenceDataholder assigned to {gameObject}, disabling GO.");
            gameObject.SetActive(false);
            return;
        }
        text.text = TamañoDeGrano.ToString();
        // VRChat: gameObject.GetComponent<VRC_Pickup>().InteractionText = text.text;
        //Debug.Log($"Deserialization: Tamaño de grano TEXT set to {TamañoDeGrano}");
        meshRenderer.material = referenceDataholder.MaterialesSegunTamañosDeLija[LijaDataholder.LijaDict[TamañoDeGrano]];
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<LijaRotation>() != null)
        {
            gameObject.GetComponent<BoxCollider>().excludeLayers = LayerMask.GetMask("Nothing");
        }
    }
}
