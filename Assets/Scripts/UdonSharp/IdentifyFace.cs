
using UnityEngine;

public class IdentifyFace : MonoBehaviour
{
    [SerializeField] private GameObject myShader;
    [SerializeField] private GameObject myMirror;
    [SerializeField] private ProbeBehabiour probeBehabiour;
    [SerializeField] private ParticleSystem residuosMaterial;

    private void Update()
    {
        if (!probeBehabiour._isInsideCollider)
        {
            //residuosMaterial.Stop();
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning("Face: " + gameObject.name + " Collision with: " + other.gameObject.name);
        if(other.gameObject.GetComponent<LijaCircularBehabiour>() || other.gameObject.GetComponent<PulidoraScript>())
        {
            probeBehabiour.probetaShader = myShader;
            probeBehabiour.Desgaste = myShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");
            if(probeBehabiour.EsteParticleSystem.isEmitting)
                probeBehabiour.EsteParticleSystem.Stop();
            probeBehabiour.EsteParticleSystem = residuosMaterial;
        }
    }
}
