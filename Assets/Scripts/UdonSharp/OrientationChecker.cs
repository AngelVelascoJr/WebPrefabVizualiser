
using System;
using UnityEngine;

public class OrientationChecker : MonoBehaviour
{
    [SerializeField][Range(0,1)]  private float umbral = 0.55f;
    //void Update()
    //{
    //    checkOrientation();
    //}

    public string checkOrientation()
    {
        Vector3 upDirection = transform.up;
        if(Vector3.Dot(upDirection, Vector3.up) > umbral)
        {
            //Debug.LogWarning(gameObject.name + " is facing up");
            return "Up";
        }
        else if (Vector3.Dot(upDirection, Vector3.down) > umbral)
        {
            //Debug.LogWarning(gameObject.name + " is facing down");
            return "Down";
        }

        return "Nothing";
    }
}
