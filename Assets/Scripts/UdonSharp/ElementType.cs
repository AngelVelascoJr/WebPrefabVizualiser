using UnityEngine;
using VRChatMigration;

public class ElementType : MonoBehaviour
{
    public string type;

    [Header("Legacy URLs (VRChat VRCUrl)")]
    [VRChatLegacy("VRCUrl", "Remote texture URLs per magnification")]
    [SerializeField] string[] _legacyLinkX100;
    [VRChatLegacy("VRCUrl", "Remote texture URLs per magnification")]
    [SerializeField] string[] _legacyLinkX200;
    [VRChatLegacy("VRCUrl", "Remote texture URLs per magnification")]
    [SerializeField] string[] _legacyLinkX500;
    [VRChatLegacy("VRCUrl", "Remote texture URLs per magnification")]
    [SerializeField] string[] _legacyLinkX1000;

    [Header("Textures (assign in inspector for viewer)")]
    public Texture2D[] TextureX100;
    public Texture2D[] TextureX200;
    public Texture2D[] TextureX500;
    public Texture2D[] TextureX1000;

    // VRChat: VRCImageDownloader.Setup — no remote download in visualizer
    public void Setup()
    {
    }

    public Texture2D[] GetAumentTextures(int aumento)
    {
        switch (aumento)
        {
            case 100: return TextureX100;
            case 200: return TextureX200;
            case 500: return TextureX500;
            case 1000: return TextureX1000;
            default:
                Debug.LogWarning($"Aumento x{aumento} no encontrado");
                return null;
        }
    }

    // VRChat: OnImageLoadSuccess
    public void OnImageLoadSuccess_VRChat()
    {
    }

    // VRChat: OnImageLoadError
    public void OnImageLoadError_VRChat()
    {
    }
}
