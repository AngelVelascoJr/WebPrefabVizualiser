
using TMPro;
using UnityEngine;

public class PulidoraScript : MonoBehaviour
{
    public bool Rotating;
    public bool isEnergized = false;

    [SerializeField] float RotationVelocity = 1f;

    [SerializeField] const float MaxTemp = 100f;
    [SerializeField] float Temperature = 24f;

    [SerializeField] private TextMeshProUGUI RPMText;
    [SerializeField] private TextMeshProUGUI TypeOfElementText;

    public bool IsForGris = false;
    public bool IsForBlanca = false;

    [Header("Audio Config")]
    [SerializeField] private AudioSource _StartStopAudioSource;
    [SerializeField] private AudioSource _LoopAudioSource;
    [SerializeField] private AudioClip _StartAudioClip;
    [SerializeField] private AudioClip _LoopAudioClip;
    [SerializeField] private AudioClip _StopAudioClip;
    [SerializeField] private bool AudioStart;
    [SerializeField] private float StartTimer;
    [SerializeField] private bool AudioLoop;

    public bool GrisLoaded = false;
    public bool BlancaLoaded = false;

    private void Start()
    {
        if (IsForGris == IsForBlanca)
        {
            Debug.LogError("<color=red>PulidoraScript</color> no se definio un color para gris o blanca unicamente: " + gameObject.name);
        }
        if(IsForGris)
        {
            TypeOfElementText.text = "Alumina\nGris";
        }
        else if(IsForBlanca)
        {
            TypeOfElementText.text = "Alumina\nBlanca";
        }
        else
        {
            TypeOfElementText.text = "Alumina\n######";
        }
    }

    private void Update()
    {

        if (Rotating && isEnergized)
        {
            gameObject.transform.Rotate(Vector3.forward * RotationVelocity * Time.deltaTime);
        }

        if (Rotating & !AudioStart && !AudioLoop)
        {
            AudioStart = true;
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StartAudioClip;
            _StartStopAudioSource.Play();
            StartTimer = 0f;
        }

        if (StartTimer > 0.9 * _StartAudioClip.length && !AudioLoop)
        {
            AudioStart = false;
            AudioLoop = true;
            _StartStopAudioSource.Stop();
            _LoopAudioSource.clip = _LoopAudioClip;
            _LoopAudioSource.Play();
        }

        if (!Rotating && (AudioStart || AudioLoop))
        {
            AudioStart = false;
            AudioLoop = false;
            StartTimer = 0f;
            _LoopAudioSource.Stop();
            _StartStopAudioSource.Stop();
            _StartStopAudioSource.clip = _StopAudioClip;
            _StartStopAudioSource.Play();
        }

        if (AudioStart)
            StartTimer += Time.deltaTime;
    }

    public void StartMachine()
    {
        if (isEnergized)
        {
            Rotating = true;
        }
    }

    public void StopMachine()
    {
         Rotating = false;
    }

    public void MachineEnergy_On()
    {
        isEnergized = true;
        Rotating = false;
        RPMText.text = "0900";
        RPMText.color = Color.red;
    }

    public void MachineEnergy_Off()
    {
        isEnergized = false;
        Rotating = false;
        RPMText.text = "8888";
        RPMText.color = Color.gray;
    }

    private void OnParticleCollision(GameObject other)
    {

        var elemento = other.GetComponentInParent<BotellaLab>();

        if (elemento != null)
        {
            string tipo = elemento.Tipo;
            //Debug.Log(tipo);

            if (tipo == "AGris" && IsForGris)
            {
                GrisLoaded = true;
                Debug.Log("Alumina Gris");
            }
            else if (tipo == "ABlanca"  && IsForBlanca)
            {
                BlancaLoaded = true;
                Debug.Log("Alumina Blanca");
            }
        }
    }
}
