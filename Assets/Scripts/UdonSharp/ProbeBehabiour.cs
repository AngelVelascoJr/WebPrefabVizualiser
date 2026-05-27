using System;
using UnityEngine;
using VRChatMigration;


public class ProbeBehabiour : MonoBehaviour
{
    const float DesgasteMin = 0f;
    const float DesgasteMax = 800f;

    const int ParticleRateMin = 10;
    const int ParticleRateMax = 50;

    [SerializeField][Range(DesgasteMin, DesgasteMax)] public float Desgaste; //0 a Lija
    [SerializeField] private float TamañoDeGranoEnLija;
    [SerializeField] private float desgasteEnShader;

    [SerializeField] public Vector3 LijaToObjSize;
    [SerializeField] public Vector3 Up;
    [SerializeField] public Vector3 VectorDeDireccionDeDesgasteActual;

    Material EsteMaterial;
    [SerializeField] public ParticleSystem EsteParticleSystem;
    [SerializeField] GameObject LijaRotationActivaGO;
    [SerializeField] LijaRotation LijaRotationActiva;

    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup_pickup;
    [SerializeField] private InteractProbe interactProbe;
    private static readonly int[] TamañosDeLija = { 120, 180, 240, 360, 400, 500, 600, 800, 9000, 9001 };


    public GameObject probetaShader;
    public GameObject bodyMaterial;
    // VRChat: UdonSynced
    public float _insideColliderTimer = 0f;
    private const float timeLijado = 10f;
    public bool _isInsideCollider = false;
    // VRChat: UdonSynced
    public bool canLijar = false;
    public bool isHumedo = true;
    private LijaCircularBehabiour lijaCircularScript = null;

    public string ProbeType = "";

    [Header("Audio Config")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    private bool LastUserWasVR = false;



    private void Start()
    {
        _audioSource.clip = _audioClip;

//#if UNITY_ANDROID
        gameObject.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
//#endif
    }

    private void Update()
    {
        if (probetaShader == null)
        {
            return;
        }
        // VRChat: pickup.IsHeld
            //Debug.Log($"<color=#0ffff0>PartSystemInfo</color> EstePartSystemName{EsteParticleSystem.name}, emiting: {EsteParticleSystem.isEmitting}---------------------");

        if (LijaRotationActiva != null) //Esta sobre una lijadora
        {
            //Debug.Log($"<color=#00ff00>EnLijadora</color> LijaRotation = {LijaRotationActiva}!!!------------------------");
            if (LijaRotationActiva.Rotating) //la lijadora esta en funcionamiento
            {
                var lijaT = LijaRotationActiva.LijaGO != null ? LijaRotationActiva.LijaGO.transform : LijaRotationActiva.transform;
                LijaToObjSize = new Vector3(transform.position.x - lijaT.position.x, 0f, transform.position.z - lijaT.position.z);
                Up = gameObject.transform.up;
                VectorDeDireccionDeDesgasteActual = Vector3.Cross(LijaToObjSize, Up);

                if (false /* VRChat: pickup.currentPlayer */) // VRChat: IsOwner
                {
                    LocalUdonEventBridge.SendLocalEvent(GetComponent<HapticFeedback>(), "hapticFeedbackDesbaste");
                }

                SetParticleRateOverTime();

                if (!EsteParticleSystem.isEmitting || !EsteParticleSystem.isPlaying)
                {
                    EsteParticleSystem.Play();
                }
                EsteParticleSystem.gameObject.transform.rotation = Quaternion.FromToRotation(EsteParticleSystem.transform.rotation.eulerAngles, VectorDeDireccionDeDesgasteActual);

                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else //la lijadora no esta en funcionamiento
            {
                //Debug.Log($"<color=#f00fff>LijadoraFuncCheck</color> StopPartSystem, Lijarotatctiva:{LijaRotationActiva} , playingAudio: {_audioSource.isPlaying} ---------------------");
                EsteParticleSystem.Stop();
                if (_audioSource.isPlaying)
                {
                    //Debug.Log($"<color=#ffffff>AudioCheckNoFunca</color> StopAUDIO, Lijarotatctiva:{LijaRotationActiva} , rotationg: {LijaRotationActiva.Rotating} ---------------------");
                    _audioSource.Stop();
                }
            }
        }

        else if(LijaRotationActivaGO != null) //check por referencia
        {
            //Debug.Log($"<color=#ffff00>RefCheck</color> RefCheck, LijarotActivaGO = {LijaRotationActivaGO}!!!------------------------");
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
        }

        else if(EsteParticleSystem.isEmitting) //si no esta dentro de la pulidora, pero esta emitiendo particulas
        {
            //Debug.Log($"<color=#fff00f>AudioCheckNoPulidora</color> StopEmitting!!!, emiting:{EsteParticleSystem.isEmitting}------------------------");
            EsteParticleSystem.Stop();
            _audioSource.Stop();
        }

        else //para debug
        {
            //if(pickup.IsHeld)
            //    Debug.Log($"<color=#ff0000>ERROR IN ALL</color> no paso por nada: LijaRotActiva= {LijaRotationActiva}, EsteParticleSystem emiting = {EsteParticleSystem.isEmitting}----!!!----");
        }

        desgasteEnShader = probetaShader.GetComponent<Renderer>().material.GetFloat("_GranoLija");
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        if (LijaRotationActiva == null)
            return;
        if (LijaRotationActiva.Lija == null)
            return;

        TryChangeDesgaste();

        if (_isInsideCollider)
        {
            checkHumidity();
            if (IsLijadoMax() || desgasteEnShader == TamañoDeGranoEnLija)
            {
                gameObject.GetComponent<BorderColor>().SendMessage("colorGreen", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
                if(true /* VRChat: IsOwner */)
                {
                    LocalUdonEventBridge.SendLocalEvent(this, "updateDesgaste"); // VRChat: SendCustomNetworkEvent
                }
                return;
            }

            if (isHumedo && canLijar)
            {
                BorderToZero();
                _insideColliderTimer += Time.deltaTime;
                Debug.Log("Time lija: " + _insideColliderTimer);
                if (_insideColliderTimer >= timeLijado)
                {
                    if (true /* VRChat: IsOwner */ && desgasteEnShader != Desgaste)
                        //updateDesgaste();
                        LocalUdonEventBridge.SendLocalEvent(this, "updateDesgaste"); // VRChat: SendCustomNetworkEvent
                }
            }

            if (!isHumedo || !canLijar)
            {
                gameObject.GetComponent<BorderColor>().SendMessage("colorRed", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
            }
        }
    }

    public void updateDesgaste()
    {
        Debug.LogWarning("Send updateDesgaste" + " Desagste is: " + Desgaste + "  desgasteEnShader: " + desgasteEnShader);
        probetaShader.GetComponent<Renderer>().material.SetFloat("_GranoLija", Desgaste);
        probetaShader.GetComponent<Renderer>().material.SetFloat("_AngleRotation", Quaternion.AngleAxis(Vector3.Angle(gameObject.transform.up, VectorDeDireccionDeDesgasteActual), gameObject.transform.forward).eulerAngles.z);
    }

    private void checkHumidity()
    {
        if (true /* VRChat: IsOwner */)
        {
            if (lijaCircularScript != null)
            {
                if (lijaCircularScript.GetHumedad() > 0)
                    //isHumedoTrue();
                    LocalUdonEventBridge.SendLocalEvent(this, "isHumedoTrue"); // VRChat: SendCustomNetworkEvent
                else
                    //isHumedoFalse();
                    LocalUdonEventBridge.SendLocalEvent(this, "isHumedoFalse"); // VRChat: SendCustomNetworkEvent
            }
        }
    }

    private void SetParticleRateOverTime()
    {
        var emisionModule = EsteParticleSystem.emission;
        var waterValue = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().GetHumedad();
        emisionModule.rateOverTimeMultiplier = Mathf.Clamp(Mathf.Exp(-(waterValue / 3.03f)+7) + 10f, ParticleRateMin, ParticleRateMax);
    }

    private void TryChangeDesgaste()// 120 a 800
    {
        TamañoDeGranoEnLija = LijaRotationActiva.Lija.GetComponent<LijaCircularBehabiour>().TamañoDeGrano;
        if (LijaRotationActiva == null)
            return;
        if (!LijaRotationActiva.Rotating)
            return;
        if (Desgaste == TamañoDeGranoEnLija)
        {
            return;
        }
        if (Desgaste > TamañoDeGranoEnLija)
        {
            Debug.Log($"Cambio por bajar: {Desgaste} a {TamañoDeGranoEnLija}");
            Desgaste = TamañoDeGranoEnLija;
            canLijar = false;
        }
        else
        {
            var IndexDesgasteEnProbeta = System.Array.BinarySearch(TamañosDeLija, (int)Desgaste);
            var IndexTamañoDeGranoEnLija = System.Array.BinarySearch(TamañosDeLija, (int)TamañoDeGranoEnLija);
            if (IndexDesgasteEnProbeta < 0) IndexDesgasteEnProbeta = ~IndexDesgasteEnProbeta - 1;
            if (IndexTamañoDeGranoEnLija < 0) IndexTamañoDeGranoEnLija = ~IndexTamañoDeGranoEnLija - 1;
            if (IndexDesgasteEnProbeta < IndexTamañoDeGranoEnLija
                && (IndexDesgasteEnProbeta + 1 == IndexTamañoDeGranoEnLija || IndexDesgasteEnProbeta + 2 == IndexTamañoDeGranoEnLija))
            {
                Debug.Log($"Cambio por seguir: {Desgaste} a {TamañoDeGranoEnLija}");
                Desgaste = TamañoDeGranoEnLija;
                canLijar = true;
                Debug.Log($"Desgaste nuevo: {Desgaste}");

            }
            else
            {
                Debug.Log($"Intento de salto de lija: {Desgaste} a {TamañoDeGranoEnLija}");
                canLijar = false;
            }
        }
        Debug.Log("Puedo lijar?: " + canLijar);
    }

    public string getProbeType()
    {
        return ProbeType;
    }

    public void OnPickup_VRChat()
    {
        LastUserWasVR = false; // VRChat: pickup.currentPlayer.IsUserInVR()
        CheckInteractProve();
    }

    private void CheckInteractProve()
    {
        if (!LastUserWasVR)
        {
            interactProbe.gameObject.SetActive(true);
        }
        else
        {
            interactProbe.gameObject.SetActive(false);
            interactProbe.DisableCanva();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractProbe>() != null)
        {
            return;
        }

        if (other.GetComponent<PulidoraScript>() != null)
        {
            interactProbe.DisableCanva();
            interactProbe.gameObject.SetActive(false);
        }

        if (other.GetComponent<LijaRotation>() != null)
        {
            Debug.Log("Lijarotation in Provebehabiour");
            LijaRotationActivaGO = other.gameObject;
            LijaRotationActiva = LijaRotationActivaGO.GetComponent<LijaRotation>();
            interactProbe.DisableCanva();
            interactProbe.gameObject.SetActive(false);

        }

        
        //if (other.GetComponent<LijaCircularBehabiour>() != null)
        //{
        //    lijaCircularScript = other.GetComponent<LijaCircularBehabiour>(); 
        //}

        if(true /* VRChat: IsOwner */)
        {
            if (other.GetComponent<LijaCircularBehabiour>() != null)
            {
                lijaCircularScript = other.GetComponent<LijaCircularBehabiour>();
                //if (lijaCircularScript.GetHumedad() > 0)
                //    //isHumedoTrue();
                //    LocalUdonEventBridge.SendLocalEvent(this, "isHumedoTrue"); // VRChat: SendCustomNetworkEvent
                //else
                //    //isHumedoFalse();
                //    LocalUdonEventBridge.SendLocalEvent(this, "isHumedoFalse"); // VRChat: SendCustomNetworkEvent

            }

            if (other.gameObject.name == "Rotor" && LijaRotationActiva.Rotating && !_isInsideCollider)
            {
                //_isInsideCollider = true;
                LocalUdonEventBridge.SendLocalEvent(this, "StartTimer"); // VRChat: SendCustomNetworkEvent
        
                Debug.Log("_isInsideCollider true");
            }
        }
        // VRChat: LocalPlayer.displayName
    }

    public void isHumedoFalse()
    {
        isHumedo = false;
    }

    public void isHumedoTrue()
    {
        isHumedo = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<LijaRotation>())
        {
            LijaRotationActivaGO = null;
            LijaRotationActiva = null;
            CheckInteractProve();
            BorderToZero();
            lijaCircularScript = null;
        }

        if(other.gameObject.name == "Rotor")
        {
            if (true /* VRChat: pickup.currentPlayer */)
            {
                Debug.Log("Stop no current player");
                LocalUdonEventBridge.SendLocalEvent(this, "StopTimer"); // VRChat: SendCustomNetworkEvent
                //_isInsideCollider = false;
            }
            else if (true /* VRChat: IsOwner */)
            {
                Debug.Log("Stop Owner");
                LocalUdonEventBridge.SendLocalEvent(this, "StopTimer"); // VRChat: SendCustomNetworkEvent
                //_isInsideCollider = false;
            }
            //_insideColliderTimer = 0f;
        }
    }

    public void BorderToZero()
    {
        //Debug.LogWarning("BorderToZero");
        bodyMaterial.GetComponent<Renderer>().material.SetFloat("_Scale", 0f);
    }

    public void StartTimer()
    {
        _isInsideCollider = true;
    }

    public void StopTimer()
    {
        _insideColliderTimer = 0f;
        _isInsideCollider = false;
        Debug.Log("ResetTimer to 0 ");
    }

    private void OnDrawGizmosSelected()
    {
        if (LijaRotationActiva != null)
        {
            if (LijaRotationActiva.Lija != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(LijaRotationActiva.Lija.transform.position, LijaRotationActiva.Lija.transform.position + LijaToObjSize);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + Up);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + VectorDeDireccionDeDesgasteActual);
            }
        }
        Gizmos.color = Color.white;
    }

    public bool IsLijadoMax()
    {
        return (int)desgasteEnShader == (int)DesgasteMax;
    }
}