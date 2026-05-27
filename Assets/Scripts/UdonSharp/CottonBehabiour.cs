
using System;
using UnityEngine;

public class CottonBehabiour : MonoBehaviour
{

    [SerializeField] CottonTimer cottonTimer;
    [SerializeField] Material cottonMat;
    [SerializeField] private bool hasAlcohol;

    private const float AP_NA = 0.1f;
    private const float HA_NA = 0f;
    private const float AP_A = 3.5f;
    private const float HA_A = 1.0f;

    private void Start()
    {
        cottonMat = GetComponentInParent<MeshRenderer>().material;
        cottonTimer = GetComponent<CottonTimer>();
    }

    public void AddAlcohol()
    {
        if(hasAlcohol)
            return;
        hasAlcohol = true;
        cottonTimer.enabled = true;
        //cottonMat.SetFloat("_AditionalPower", AP_A);
        //cottonMat.SetFloat("_HasAlcohol", 1); // https://www.reddit.com/r/Unity3D/comments/wgmias/property_rect_already_exists_in_the_property/?rdt=53009
    }

    public void ChangeAlcohol(float value)
    {
        if (cottonMat == null)
        {
            Debug.Log("Researching material cottonMat");
            cottonMat = GetComponentInParent<MeshRenderer>().material;
        }
        cottonMat.SetFloat("_AditionalPower", value * AP_A);
        cottonMat.SetFloat("_HasAlcoholFloat", value * HA_A); // https://www.reddit.com/r/Unity3D/comments/wgmias/property_rect_already_exists_in_the_property/?rdt=53009
    }

}
