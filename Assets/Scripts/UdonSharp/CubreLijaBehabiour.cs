using UnityEngine;

public class CubreLijaBehabiour : MonoBehaviour
{
    public GameObject cubrelijaReference;
    public LijaRotation lijaRotation;
  
    private void Update()
    {
        if(lijaRotation != null)
        {
            if (lijaRotation.Rotating)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                gameObject.GetComponent<MeshCollider>().enabled = false;
            }
            if (!lijaRotation.Rotating)
            {
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                gameObject.GetComponent<MeshCollider>().enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<LijaRotation>() != null)
        {
            gameObject.GetComponent<MeshCollider>().excludeLayers = LayerMask.GetMask("Nothing");

        }
    }
}
