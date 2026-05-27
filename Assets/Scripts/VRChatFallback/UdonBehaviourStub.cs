using UnityEngine;

namespace VRChatFallback
{
    /// <summary>
    /// Stub para reemplazar VRC.Udon.UdonBehaviour en prefabs.
    /// Se marca para que el Prefab Viewer lo ignore.
    /// </summary>
    [PrefabViewerIgnoreComponent]
    public class UdonBehaviourStub : MonoBehaviour
    {
        public Object interactTextPlacement;
        public string interactText;
        public Object interactTextGO;
        public float proximity = 2f;

        public int SynchronizePosition;
        public int AllowCollisionOwnershipTransfer;
        public int Reliable;
        public int _syncMethod;

        public Object serializedProgramAsset;
        public Object programSource;
        public string serializedPublicVariablesBytesString;
        public Object[] publicVariablesUnityEngineObjects = new Object[0];
        public int publicVariablesSerializationDataFormat;
    }
}

