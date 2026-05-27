using UnityEngine;
using VRChatMigration;

public class MicroscopeElements : MonoBehaviour
{
    [SerializeField] public ElementType[] elementos;
    public Texture2D placeHolderT2D;

    [VRChatLegacy("VRCImageDownloader", "VRChat remote image download")]
    [SerializeField] string _legacyImageDownloader;

    private void Start()
    {
        foreach (var elemento in elementos)
        {
            if (elemento != null)
                elemento.Setup();
        }
    }
}
