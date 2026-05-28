
using UnityEngine;
public class UpdatePreview : MonoBehaviour
{
    [SerializeField] private OrientationChecker orientationChecker;
    [SerializeField] private Vector3 positionCanvaUp = new Vector3(-0.541f, 0.291f, 0.034f);
    [SerializeField] private Vector3 positionCanvaDown = new Vector3(-0.541f, -0.291f, 0.034f);

    [Header("Faces Probeta")]
    public GameObject probetaShaderParent_inf;
    public GameObject probetaMirrorParent_inf;
    public GameObject probetaBorderParent_inf;

    public GameObject probetaShaderParent_sup;
    public GameObject probetaMirrorParent_sup;
    public GameObject probetaBorderParent_sup;

    [Header("Faces Canva")]
    public GameObject probetaShaderChildren;
    public GameObject probetaMirrorChildren;
    [SerializeField] private GameObject probetaShaderActual;

    [SerializeField] public BorderColor borderColor;
    [SerializeField] private bool supIsActive = false;


    [Header("RenderInfo")]
    [SerializeField] private float reflexion = 0;
    [SerializeField] private float granoLija = 0;
    [SerializeField] private int isFirstSanding = 0;
    [SerializeField] private float angleRotation = 0;

    [SerializeField] private float timer = 0f;
    [SerializeField] private bool switchFlag = false;

    private void Start()
    {
        activeSelf(probetaShaderParent_inf, probetaMirrorParent_inf);
        probetaShaderActual = probetaShaderParent_inf;
    }
    private void Update()
    {    
        moveCanva();
        rotateCanva();

        updateShader(probetaShaderChildren);

        if (supIsActive)
        {
            activeSelf(probetaShaderParent_sup, probetaMirrorParent_sup);
        }
        if (!supIsActive)
        {
            activeSelf(probetaShaderParent_inf, probetaMirrorParent_inf);
        }


        if (switchFlag && timer<1.5f)
        {
            borderColor.SendMessage("colorYellow", SendMessageOptions.DontRequireReceiver); // VRChat: SendCustomEvent
            timer += Time.deltaTime;
        }

        if(timer > 1f)
        {
            switchFlag = false;
            timer = 0f;
        }

        if(!switchFlag)
        {
            scaleAllMaterial();

            if (probetaMirrorChildren.activeSelf)
            {
                if (supIsActive) { probetaShaderParent_sup.SetActive(false); }
                if (!supIsActive) { probetaShaderParent_inf.SetActive(false); }
            }
        }
    }

    public void switchFaces_Off()
    {
        forceActiveShader(probetaShaderParent_inf, probetaMirrorParent_inf);
        scaleShader(probetaBorderParent_sup, 0f, 0);
        borderColor.bodyMaterial = probetaBorderParent_inf;
        probetaShaderActual = probetaShaderParent_inf;
        supIsActive = false;
        switchFlag = true;
        timer = 0f;
        Debug.LogWarning("[<color=blue>Change to face inferior</color>]");
    }

    public void switchFaces_On()
    {
        forceActiveShader(probetaShaderParent_sup, probetaMirrorParent_sup);
        scaleShader(probetaBorderParent_inf, 0f, 0);
        borderColor.bodyMaterial = probetaBorderParent_sup;
        probetaShaderActual = probetaShaderParent_sup;
        supIsActive = true;
        switchFlag = true;
        timer = 0f;
        Debug.LogWarning("[<color=blue>Change to face superior</color>]");
    }

    private void getDataFaces(GameObject thisFace)
    {
        var material = thisFace.GetComponent<Renderer>().material;
        reflexion = ShaderMaterialAccess.GetFloat(material, "_Reflexion");
        granoLija = ShaderMaterialAccess.GetFloat(material, "_GranoLija");
        isFirstSanding = ShaderMaterialAccess.GetInt(material, "_IsFirstSanding");
        angleRotation = ShaderMaterialAccess.GetFloat(material, "_AngleRotation");
    }

    private void activeSelf(GameObject shader, GameObject mirror)
    {
        bool thisBool = false;
        thisBool = shader.activeSelf;
        probetaMirrorChildren.SetActive(mirror.activeSelf);
        if (probetaMirrorChildren.activeSelf)
            thisBool = false;
        probetaShaderChildren.SetActive(thisBool);
    }

    private void updateShader(GameObject shader)
    {
        getDataFaces(probetaShaderActual);
        var material = shader.GetComponent<Renderer>().material;
        ShaderMaterialAccess.SetFloat(material, "_Reflexion", reflexion);
        ShaderMaterialAccess.SetFloat(material, "_GranoLija", granoLija);
        ShaderMaterialAccess.SetInt(material, "_IsFirstSanding", isFirstSanding);
        ShaderMaterialAccess.SetFloat(material, "_AngleRotation", angleRotation);
    }

    private void scaleShader(GameObject shadertoScale, float scale, int posArray)
    {
        ShaderMaterialAccess.SetFloat(shadertoScale.GetComponent<Renderer>().materials[posArray], "_Scale", scale);
    }

    private void forceActiveShader(GameObject shaderToActivate, GameObject mirrorToCheck)
    {
        if(!mirrorToCheck.activeSelf)
            return;
        else
            shaderToActivate.SetActive(true);
    }

    public void scaleAllMaterial()
    {
        scaleShader(probetaBorderParent_inf, 0f, 0);
        scaleShader(probetaBorderParent_sup, 0f, 0);
    }

    /// <summary>
    /// Rota el Canvas para que mire hacia el jugador local, manteniendo la rotación únicamente en Y.
    /// Afecta solo al jugador local.
    /// </summary>
    private void rotateCanva()
    {
        var cam = Camera.main;
        if (cam == null) return;
        transform.LookAt(cam.transform.position); // VRChat: LocalPlayer position
        Vector3 euler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(euler.x - 80, euler.y, 0); // Solo rotación en X, Y
    }

    /// <summary>
    /// Mueve el Canvas a una posición local predefinida (arriba o abajo) según la orientación detectada.
    /// </summary>
    private void moveCanva()
    {
        string state = orientationChecker.checkOrientation();
        if(state == "Nothing") { return; }
        //Debug.Log("[<color=orange>Postition actual: </color>]" + transform.localPosition);
        if(state == "Up") // Is Up
        {
            gameObject.transform.localPosition = positionCanvaUp;
        }
        else // Is Down
        {
            gameObject.transform.localPosition = positionCanvaDown;
        }
        //Debug.Log("[<color=green>Postition new: </color>]" + transform.localPosition + ", State: " + state);
    }

}
