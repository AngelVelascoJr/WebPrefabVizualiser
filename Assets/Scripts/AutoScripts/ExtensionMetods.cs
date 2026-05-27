using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ExtensionMetods
{
    public static Vector3 YTo0(this Vector3 value)
    {
        return new Vector3(value.x, 0, value.z);
    }

    public static Vector4 ToVector4(this Quaternion value)
    {
        return new Vector4(value.x, value.y, value.z, value.w);
    } 
}
