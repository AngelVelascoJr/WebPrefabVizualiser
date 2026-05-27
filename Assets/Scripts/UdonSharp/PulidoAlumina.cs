using UnityEngine;
using VRChatMigration;

public class PulidoAlumina : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup_pickup;

    [Header("Pulidora")]
    [SerializeField] private GameObject rotorPulidora;
    [SerializeField] private PulidoraScript pulidoraScript;

    [Header("Pulido variables")]
    const float timePulido = 10f;

    public bool haveAluminaGris = false;   //SCNE
    public bool haveAluminaBlanca = false; //SCNE

    public bool isInPulidoraGris = false;  //SCNE
    public bool isInPulidoraBlanca = false;//SCNE

    public bool finishedPulido1 = false;   //SCNE
    public bool finishedPulido2 = false;   //SCNE

    [Header("Lavado Alumina")]
    public bool finishedWater = false;     //SCNE
    public bool finishedEnjuagado = false; //SCNE

    public ParticleSystem waterPS;

    private float timer = 0f;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
        // VRChat: pickup = GetComponentInParent<VRC_Pickup>();
    }


    public void ProcessPulido()
    {
        if (isInPulidoraGris || isInPulidoraBlanca)
        {
            Retroalimentacion();
            Pulido();
        }
    }

    public void ProcessLavado()
    {
        if (face.residuosAlumina == null)
            return;

        if (finishedWater)
        {
            face.residuosAlumina.Stop();

            if (face.probetaWaterPS != null && !face.probetaWaterPS.isEmitting && !finishedEnjuagado)
            {
                face.probetaWaterPS.Play();
                LocalUdonEventBridge.SendLocalEvent(this, "ResetTimer"); // VRChat: SendCustomNetworkEvent
            }

            if (face.newcalor)
            {
                face.probetaWaterPS.Stop();

                LocalUdonEventBridge.SendLocalEvent(this, "enjuagadoFinished"); // VRChat: SendCustomNetworkEvent
            }

            return;

        }


        if (waterPS != null)
        {
            if (!waterPS.isEmitting)
            {
                waterPS = null;
                face.residuosAlumina.Stop();
            }

            else
            {
                if (!face.residuosAlumina.isEmitting)
                {
                    face.residuosAlumina.Play();
                }

                if (generalTimer(1.5f))
                {
                    LocalUdonEventBridge.SendLocalEvent(this, "waterFinished"); // VRChat: SendCustomNetworkEvent

                }
            }
        }
    }


    private void Retroalimentacion()
    {
        if (pulidoraScript != null && false /* VRChat: pickup.currentPlayer */) // VRChat: IsOwner
        {
            gameObject.GetComponentInParent<HapticFeedback>().SendMessage("hapticFeedbackPulido", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
        }
    }

    private void Pulido()
    {

        if (!probeBehaviour.IsLijadoMax())
        {
            gameObject.GetComponentInParent<BorderColor>().SendMessage("colorRed", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
            return;
        }

        if ((finishedPulido1 && isInPulidoraGris) || (finishedPulido2 && isInPulidoraBlanca))
        {
            gameObject.GetComponentInParent<BorderColor>().SendMessage("colorGreen", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
            return;
        }

        if(true /* VRChat: IsOwner */)
        {
            checkAluminas();
        }
        bool probeNoHaveAlumina = !haveAluminaGris && !haveAluminaBlanca;
        bool pulidoCorrecto = (isInPulidoraGris && haveAluminaGris) || (isInPulidoraBlanca && haveAluminaBlanca);

        if (probeNoHaveAlumina || !pulidoCorrecto)
        {
            gameObject.GetComponentInParent<BorderColor>().SendMessage("colorRed", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
            return;
        }

        if (pulidoCorrecto && !finishedPulido1)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "CallScaleBorder"); // VRChat: SendCustomNetworkEvent
            if (generalTimer(timePulido))
            {
                face.probetaShader.GetComponent<Renderer>().material.SetFloat("_Reflexion", 1);
                finishedPulido1 = true;
            }
        }

        else if (pulidoCorrecto && finishedPulido1)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "CallScaleBorder"); // VRChat: SendCustomNetworkEvent
            if (generalTimer(timePulido))
            {
                face.probetaShader.SetActive(false);
                face.probetaMirror.SetActive(true);
                finishedPulido2 = true;
            }
        }
    }

    public bool generalTimer(float maxTime)
    {
        timer += Time.deltaTime;

        if (timer > maxTime)
        { return true; }

        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!true /* VRChat: IsOwner */)
            return;

        if (other.gameObject.name == "RotorPulidora")
        {
            rotorPulidora = other.GetComponentInParent<PulidoraScript>().gameObject;
            pulidoraScript = rotorPulidora.GetComponent<PulidoraScript>();

            //checkAluminas();

            if (pulidoraScript.Rotating)
            {
                if (pulidoraScript.IsForGris)
                {
                    LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraGrisTrue"); // VRChat: SendCustomNetworkEvent
                }

                if (pulidoraScript.IsForBlanca)
                {
                    LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraBlancaTrue"); // VRChat: SendCustomNetworkEvent

                }

            }

        }

    }

    private void checkAluminas()
    {
        if (pulidoraScript.GrisLoaded)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "HaveAlumGrisTrue"); // VRChat: SendCustomNetworkEvent
        }

        if (finishedPulido1)
        {
            if (pulidoraScript.BlancaLoaded)
            {
                LocalUdonEventBridge.SendLocalEvent(this, "HaveAlumBlanTrue"); // VRChat: SendCustomNetworkEvent
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (true /* VRChat: pickup.currentPlayer */)
        {
            if (isInPulidoraGris || isInPulidoraBlanca)
            {
                LocalUdonEventBridge.SendLocalEvent(this, "ResetTimer"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "ResetVars"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "CallScaleBorder"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraGrisFalse"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraBlancaFalse"); // VRChat: SendCustomNetworkEvent
            }

        }

        if (true /* VRChat: IsOwner */)
        {
            Debug.LogWarning("This player is owner, objeto: " + other.gameObject.name);
            if (other.gameObject.name == "RotorPulidora")
            {
                LocalUdonEventBridge.SendLocalEvent(this, "ResetTimer"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "ResetVars"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "CallScaleBorder"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraGrisFalse"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "IsInPulidoraBlancaFalse"); // VRChat: SendCustomNetworkEvent
            }
        }

    }


    // SCNE

    public void IsInPulidoraGrisTrue()
    {
        isInPulidoraGris = true;
    }
    public void IsInPulidoraGrisFalse()
    {
        isInPulidoraGris = false;
    }
    public void IsInPulidoraBlancaTrue()
    {
        isInPulidoraBlanca = true;
    }
    public void IsInPulidoraBlancaFalse()
    {
        isInPulidoraBlanca = false;
    }

    public void ResetVars()
    {
        rotorPulidora = null;
        pulidoraScript = null;
    }


    public void HaveAlumBlanTrue()
    {
        haveAluminaBlanca = true;
    }
    public void HaveAlumGrisTrue()
    {
        haveAluminaGris = true;
    }


    public void ResetTimer()
    {
        timer = 0f;
    }

    public void waterFinished()
    {
        finishedWater = true;
    }
    public void enjuagadoFinished()
    {
        finishedEnjuagado = true;
    }

    
    public void CallScaleBorder()
    {
        face.ScaleBorder();
    }

    //////////////////////////////////

}
