using UnityEngine;
using VRChatMigration;

public class BotellaLab : MonoBehaviour
{

    private const float Max = 250f;
    public bool isInfinite = false;
    public string Tipo = "";
    [SerializeField][Range(0f, Max)] private float LiquidFill;
    [SerializeField] private float LiquidPourVel;

    [SerializeField] private GameObject InfillGO;
    [SerializeField] private Material WaterMaterial;
    [VRChatLegacy("VRC_Pickup", "Interactive pickup")]
    [SerializeField] string _legacyVrcPickup__pickupComp;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Vector3 _PSOriginalPos;
    [SerializeField] private Transform _Visual;
    bool LastUserWasVr = false;

    private void Start()
    {
        if (InfillGO == null || _particleSystem == null)
            return;

        var infillRenderer = InfillGO.GetComponent<MeshRenderer>();
        if (infillRenderer == null)
            return;

        WaterMaterial = infillRenderer.material;
        var main = _particleSystem.main;
        if (WaterMaterial != null && WaterMaterial.HasProperty("_ColorAguaSuperficie"))
            main.startColor = WaterMaterial.GetColor("_ColorAguaSuperficie");
        _PSOriginalPos = _particleSystem.transform.localPosition;
    }

    private void Update()
    {
        if (WaterMaterial == null || _particleSystem == null)
            return;

        if (WaterMaterial.HasProperty("_FillPercentage"))
            WaterMaterial.SetFloat("_FillPercentage", (Mathf.Clamp(LiquidFill, 0f, Max) / Max) * 100f);
        if(_particleSystem.isEmitting)
        {
            if(!isInfinite)
            {
                LiquidFill -= Time.deltaTime * LiquidPourVel;
            }
        }
        if(LiquidFill < 0 )
        {
            _particleSystem.Stop();
            if (!LastUserWasVr)
            {
                //_Visual.transform.localRotation = Quaternion.Euler(0, 0, 00);
            }
        }
    }

    public void OnPickup_VRChat()
    {
        //LastUserWasVr |= _pickupComp != null;
        LastUserWasVr = false; // VRChat: _pickupComp.currentPlayer.IsUserInVR()
        if (!LastUserWasVr)
        {
            //_Visual.rotation = Quaternion.Euler(0, -50f, 0);
        }
        //update Ownership of particle system
        // VRChat: Networking.SetOwner(null /* VRChat: _pickupComp.currentPlayer */, _particleSystem.gameObject);
    }

    public void OnDrop_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UnUseThisThing"); // VRChat: SendCustomNetworkEvent
        if (!LastUserWasVr)
        {
            //gameObject.transform.rotation = Quaternion.identity;
            //_Visual.rotation = Quaternion.identity; gameObject.transform.rotation = Quaternion.Euler(0, -50f, 0);
        }
    }

    public void OnPickupUseDown_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UseThisThing"); // VRChat: SendCustomNetworkEvent
    }

    public void OnPickupUseUp_VRChat()
    {
        LocalUdonEventBridge.SendLocalEvent(this, "UnUseThisThing"); // VRChat: SendCustomNetworkEvent
    }

    public void UseThisThing()
    {
        if (_particleSystem == null || WaterMaterial == null || _Visual == null)
            return;

        // VRChat: requires held pickup
        var Mainn = _particleSystem.main;
        if (WaterMaterial.HasProperty("_ColorAgua"))
            Mainn.startColor = WaterMaterial.GetColor("_ColorAgua");
        _particleSystem.transform.localPosition = _PSOriginalPos;
        if(false && false /* VRChat: _pickupComp.currentPlayer.IsUserInVR()*/)
        {
            Debug.Log("Object holder is VR user");
            if (LiquidFill > 0)
            {
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
        else
        {
            Debug.Log("Object holder is PC user");
            if(LiquidFill > 0)
            {
                _Visual.transform.localRotation = Quaternion.Euler(-15f, 90, 0);
                _particleSystem.gameObject.SetActive(true);
                _particleSystem.Play();
            }
        }
    }

    public void UnUseThisThing()
    {
        if (_particleSystem == null)
            return;

        _particleSystem.Stop();
        if (!LastUserWasVr && _Visual != null)
            _Visual.transform.localRotation = Quaternion.Euler(0, 90, 00);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponentInParent<IsLiquidSource>() != null)
        {
            fill();
        }
    }

    private void fill()
    {
        if(isInfinite)
        {
            Debug.Log($"no se puede llenar un recurso infinito: {Tipo}");
            return;
        }
        LiquidFill += 5;
        if (LiquidFill > Max)
            LiquidFill = Max;
    }


}
