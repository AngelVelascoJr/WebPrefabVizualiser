using UnityEngine;

public class SkyboxSetup : MonoBehaviour
{
    [SerializeField] private Material SkyboxMaterial;
    [SerializeField] private float starsIntensityPC = 1.5f;
    [SerializeField] private float starsIntensityVR = 0.6f;

    void Start()
    {
        if (SkyboxMaterial == null)
            return;
        // VRChat: LocalPlayer.IsUserInVR — default to PC intensity in visualizer
        SkyboxMaterial.SetFloat("_StarsIntensity", starsIntensityPC);
        gameObject.SetActive(false);
    }
}
