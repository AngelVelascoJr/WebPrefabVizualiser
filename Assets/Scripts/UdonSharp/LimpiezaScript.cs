using UnityEngine;

public class LimpiezaScript : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private FaceBehaviour face;
    [SerializeField] private ProbeBehabiour probeBehaviour;

    [Header("Limpieza variables")]

    public bool isCotton = false;
    public bool haveAlcohol = false;

    public bool finishCotton = false;
    public bool finishedLimpieza = false;

    private void Start()
    {
        face = GetComponent<FaceBehaviour>();
        probeBehaviour = GetComponentInParent<ProbeBehabiour>();
    }
    public void ProcessLimpieza()
    {
        if (face.alcoholPS == null)
            return;

        var mainAlcoholPS = face.alcoholPS.main;

        if (!face.alcoholPS.isEmitting && haveAlcohol && !finishedLimpieza)
        {
            face.alcoholPS.Play();
            Debug.LogWarning("[<color=blue>ALCOHOL PS play</color>]");
        }

        if (isCotton && face.alcoholPS.isEmitting && !finishedLimpieza)
        {
            if (haveAlcohol)
            {
                LocalUdonEventBridge.SendLocalEvent(this, "resetCotton"); // VRChat: SendCustomNetworkEvent
                LocalUdonEventBridge.SendLocalEvent(this, "finishCottonTrue"); // VRChat: SendCustomNetworkEvent

                mainAlcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.005f);
                Debug.LogWarning("[<color=blue>Alcohol absorbed</color>]");
            }
        }


        if (face.newcalor && finishCotton)
        {
            LocalUdonEventBridge.SendLocalEvent(this, "resetHaveAlcohol"); // VRChat: SendCustomNetworkEvent
            LocalUdonEventBridge.SendLocalEvent(this, "finishedLimpiezaTrue"); // VRChat: SendCustomNetworkEvent
            face.alcoholPS.Stop();
            mainAlcoholPS.startSize = new ParticleSystem.MinMaxCurve(0f, 0.02f);
            Debug.LogWarning("[<color=blue>Finished Limpieza</color>]");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (face.IsReady())
            return;

        var cotton = other.GetComponent<CottonBehabiour>();

        if (cotton != null && !isCotton && haveAlcohol)
        {
            LocalUdonEventBridge.SendLocalEvent(cotton, "AddAlcohol"); // VRChat: SendCustomNetworkEvent
            LocalUdonEventBridge.SendLocalEvent(this, "cottonColission"); // VRChat: SendCustomNetworkEvent
            Debug.LogWarning("Cotton enter");
        }
    }

    // SCNE

    public void resetCotton()
    {
        isCotton = false;
    }
    public void finishCottonTrue()
    {
        finishCotton = true;
    }

    public void resetHaveAlcohol()
    {
        haveAlcohol = false;
    }
    public void finishedLimpiezaTrue()
    {
        finishedLimpieza = true;
    }



    public void addedAlcohol()
    {
        haveAlcohol = true;
    }
    public void cottonColission()
    {
        isCotton = true;
    }



    /////////////////

}
