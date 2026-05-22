using UnityEngine;

namespace PrefabViewer
{
    public class SamplePrefabComponent : MonoBehaviour
    {
        [SerializeField] string label = "Sample";
        [SerializeField] int count = 3;
        [SerializeField] float speed = 1.5f;
        [SerializeField] bool active = true;
        [SerializeField] Vector3 offset = Vector3.up;
        [SerializeField] Color tint = Color.cyan;
    }
}
