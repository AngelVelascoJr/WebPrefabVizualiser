using UnityEngine;

public class AtaqueQuimico : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;

    [Header("Nital variables")]

    public bool haveNital = false;
    public bool nitalRemoved = false;
    public bool finishedAQ = false;

    private float timer = 0f;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
    }


    public void ProcessAtaque()
    {
        if (face.nitalInProbePS == null)
            return;

        var mainNitalPS = face.nitalInProbePS.main;

        if (!face.nitalInProbePS.isEmitting && haveNital)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "resetHaveAlcohol"); // VRChat: SendCustomNetworkEvent
            face.nitalInProbePS.Play();
            Debug.LogWarning("[<color=blue>Nital PS play</color>]");
        }


        if (face.limpieza.haveAlcohol && face.nitalInProbePS.isEmitting)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "nitalRemovedTrue"); // VRChat: SendCustomNetworkEvent
            mainNitalPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
            Debug.LogWarning("[<color=blue>Nital removed</color>]");
        }


        if (face.newcalor && nitalRemoved)
        {
            if (face.pulido.generalTimer(1.5f))
            {
                LocalUdonEventBridge.SendLocalEvent(this, "finishedAtaqueQ"); // VRChat: SendCustomNetworkEvent
                face.nitalInProbePS.Stop();

                mainNitalPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
                gameObject.GetComponentInParent<BorderColor>().SendMessage("colorGreen", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent

                Debug.LogWarning("[<color=blue>Ataque Quimico Finalizado</color>]");
            }
        }
    }



    // SCNE
    public void addedNital()
    {
        haveNital = true;
    }
    public void nitalRemovedTrue()
    {
        nitalRemoved = true;
    }
    public void finishedAtaqueQ()
    {
        finishedAQ = true;
        haveNital = false;
    }
    /////////////////
}
