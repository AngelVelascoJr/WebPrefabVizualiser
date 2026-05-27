using UnityEngine;

public class CallerTest : MonoBehaviour
{

    int value = 0;
    [SerializeField] Material[] materials;
    private new MeshRenderer renderer;

    private void Start()
    {
        renderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
    }

    public void TestFunction()
    {
        Debug.Log("TestFunction Called in " + gameObject.name);
        renderer.material = materials[value];
        value++;
        if (value == materials.Length)
        {
            value = 0;
        }
    }
}
