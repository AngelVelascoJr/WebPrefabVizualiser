using System.Collections.Generic;
using UnityEngine;
using VRChatMigration;

public class LijaDataholder : MonoBehaviour
{
    [VRChatLegacy("VRCObjectPool", "VRChat object pool")]
    [SerializeField] string _legacyLijaPool;

    public Material[] MaterialesSegunTamañosDeLija;

    // VRChat: DataDictionary — grain size to material index
    public static readonly Dictionary<int, int> LijaDict = new Dictionary<int, int>
    {
        { 120, 0 }, { 180, 1 }, { 240, 2 }, { 360, 3 },
        { 400, 4 }, { 500, 5 }, { 600, 6 }, { 800, 7 },
    };

    private void Start()
    {
        Debug.Log(MaterialesSegunTamañosDeLija.Length);
    }
}
