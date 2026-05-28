using UnityEngine;

/// <summary>
/// Safe material property access when shaders may be swapped (e.g. WebGL preview fallbacks).
/// </summary>
public static class ShaderMaterialAccess
{
    public static float GetFloat(Material material, string propertyName, float defaultValue = 0f)
    {
        if (material == null || !material.HasProperty(propertyName))
            return defaultValue;
        return material.GetFloat(propertyName);
    }

    public static int GetInt(Material material, string propertyName, int defaultValue = 0)
    {
        if (material == null || !material.HasProperty(propertyName))
            return defaultValue;
        return material.GetInt(propertyName);
    }

    public static void SetFloat(Material material, string propertyName, float value)
    {
        if (material == null || !material.HasProperty(propertyName))
            return;
        material.SetFloat(propertyName, value);
    }

    public static void SetInt(Material material, string propertyName, int value)
    {
        if (material == null || !material.HasProperty(propertyName))
            return;
        material.SetInt(propertyName, value);
    }
}
