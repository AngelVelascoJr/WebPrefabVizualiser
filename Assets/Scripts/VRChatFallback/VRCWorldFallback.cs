using System.Collections.Generic;
using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Fallback mínimo para el componente \"VRCWorld\" del SDK (prefab Assets/Prefabs/VRCWorld.prefab).
    /// Mantiene nombres de campos usados en el YAML para conservar valores.
    /// </summary>
    public class VRCWorldFallback : MonoBehaviour
    {
        public List<Transform> spawns = new List<Transform>();
        public int spawnOrder;
        public int spawnOrientation;
        public Camera ReferenceCamera;
        public float RespawnHeightY = -100f;
        public int ObjectBehaviourAtRespawnHeight;
        public int ForbidUserPortals;
        public int interactThruLayers;
        public int autoSpatializeAudioSources;
        public Vector3 gravity = Physics.gravity;
        public string layerCollisionArr;
        public int capacity;
        public int contentSex;
        public int contentViolence;
        public int contentGore;
        public int contentOther;
        public int releasePublic;
        public string unityVersion;
        public string Name;
        public int NSFW;
        public Vector3 SpawnPosition;
        public Transform SpawnLocation;
        public float DrawDistance;
        public int useAssignedLayers;
        public List<GameObject> DynamicPrefabs = new List<GameObject>();
        public List<Material> DynamicMaterials = new List<Material>();
    }
}

