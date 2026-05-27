using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicroscopeBehabiour : MonoBehaviour
{
    [Obsolete][SerializeField] private Image[] CompImage;
    [SerializeField] private RawImage[] CompRawImage;
    [SerializeField] private TextMeshProUGUI CompText;
    [SerializeField] private MicroscopeElements ReferenceGOComponent;
    [SerializeField] private GameObject CanvasGO;
    private ProbeBehabiour PBcomponent;

    [SerializeField] private bool IsRight;
    [SerializeField] private int Augment = 100;
    int count = 0;

    private static readonly int[] Aumentos = { 100, 200, 500, 1000 };


    void Start()
    {
        Augment = Aumentos[count];
        if (ReferenceGOComponent == null)
            Debug.LogError($"No Reference gameobject reference found, add a GameObject with a MicroscopeElements component to your scene or assign it");
        PositionImageViewer();
        //TryChangeImage("Acero 1018", Augment, 50);
        //TryChangeImageNew("Acero 1018", Augment, 50);
        CanvasGO.SetActive(false);
    }

    private void PositionImageViewer()
    {
        if (IsRight)
        {
            CanvasGO.transform.localPosition = new Vector3(0, 0.41f, 0.575f);
        }
        else
        {
            CanvasGO.transform.localPosition = new Vector3(0, 0.41f, -0.575f);
        }
    }

    public void OnAugmentChange(int count)
    {
        Augment = Aumentos[count];
        if (PBcomponent == null)
            return;
        if (TestFacesIsReady())//Si una cara esta lista, prueba a cambiar de tamaño
        {
            TryChangeImageNew(PBcomponent.getProbeType(), Augment);
            CompText.text = $"x{Augment}";
        }
    }

    //observa todas las caras, y ve si una esta lista
    private bool TestFacesIsReady()
    {
        bool OneFaceIsReady = false;
        int i = 0;
        foreach (var compo in PBcomponent.GetComponentsInChildren<FaceBehaviour>()) // ActivateMirror si se usa el script ActivateMirror
        {
            i++;
            if (compo.IsReady())
                OneFaceIsReady = true;
        }
        Debug.Log(i);
        return OneFaceIsReady;
    }

    private void TryChangeImageNew(string type, int augment, int index = 0)
    {
        Texture2D rawImagenAUsar = ReferenceGOComponent.placeHolderT2D;
        bool ValidProveType = false;

        for (int i = 0; i < ReferenceGOComponent.elementos.Length; i++)
        {
            ElementType elementType = ReferenceGOComponent.elementos[i];
            if (type == elementType.type)
            {
                
                ValidProveType = true;
                Texture2D[] textures = ReferenceGOComponent.elementos[i].GetAumentTextures(augment);
                if (textures != null && textures.Length > 0 && textures[0] != null)
                {
                    rawImagenAUsar = textures[0];
                }
                else
                {
                    Debug.LogWarning($"{this}: no texture assigned for augment {augment}");
                }
                break;
            }
        }

        if (!ValidProveType)
        {
            Debug.LogWarning($"{this}:no image detected with type {type}");
        }
        UpdateRawImage(ref rawImagenAUsar);
    }

    private void UpdateRawImage(ref Texture2D imagenAUsar)
    {
        foreach (var comp in CompRawImage)
        {
            if (comp == null)
            {
                continue;
            }

            comp.texture = imagenAUsar;
        }
    }

    private void RemoveImage()
    {
        Texture2D sprite = null;
        UpdateRawImage(ref sprite);
    }

    public void Canvas_On()
    {
        CanvasGO.SetActive(true);
    }

    public void Canvas_Off()
    {
        CanvasGO.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ProbeBehabiour>() != null && PBcomponent == null)
        {
            PBcomponent = other.gameObject.GetComponent<ProbeBehabiour>();
            if (TestFacesIsReady())
            {
                TryChangeImageNew(PBcomponent.getProbeType(), Augment);
                CompText.text = $"x{Augment}";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ProbeBehabiour>() == PBcomponent && PBcomponent != null)
        {
            RemoveImage();
            PBcomponent = null;
        }
    }

}
