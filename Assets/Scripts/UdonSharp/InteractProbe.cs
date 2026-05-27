
using UnityEngine;
using UnityEngine.UIElements;

public class InteractProbe : MonoBehaviour
{
    public bool activate = false;
    public GameObject canva;

    [SerializeField] private Renderer materialToOff_1;
    [SerializeField] private Renderer materialToOff_2;

    private void Start()
    {
        canva.SetActive(false);
    }

    public void Interact_VRChat()
    {
        activate = !activate;
        if (activate)
        {
            canva.SetActive(true);
            manageBorders(true);
        }
        else if (!activate) 
        {
            canva.SetActive(false);
            manageBorders(false);
        }
    }

    private void manageBorders(bool state)
    {
        materialToOff_1.gameObject.SetActive(state);
        materialToOff_2.gameObject.SetActive(state);
    }

    public void DisableCanva()
    {
        canva.SetActive(false);
    }
}
