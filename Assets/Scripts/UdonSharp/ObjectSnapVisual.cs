
using UnityEngine;

public class ObjectSnapVisual : MonoBehaviour
{
    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    [SerializeField] Mesh MeshToShow;
    [SerializeField] Material MeshToShowMaterial;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        if(MeshToShow == null || MeshToShowMaterial == null)
        {
            enabled = false;
            return;
        }
        _meshFilter.mesh = MeshToShow;
        _meshRenderer.material = MeshToShowMaterial;
        _meshRenderer.enabled = false;
    }

    public void Show()
    {
        _meshRenderer.enabled = true;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }

}
