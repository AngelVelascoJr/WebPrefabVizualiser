using System.Collections.Generic;
using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback para el tipo \"VRCObjectPool\" del SDK.
    /// En este proyecto aparece con campos Pool/StartPositions/StartRotations en YAML.
    /// Comportamiento: stub-only (no spawnea), pero conserva datos.
    /// </summary>
    public class VRCObjectPoolRuntimeFallback : MonoBehaviour
    {
        public List<GameObject> Pool = new List<GameObject>();
        public List<Vector3> StartPositions = new List<Vector3>();
        public List<Quaternion> StartRotations = new List<Quaternion>();

        public GameObject TryToSpawn() => null;
        public void Return(GameObject obj)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}

