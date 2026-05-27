using System;

/// <summary>
/// Marker attribute to hide a Component entirely from the Prefab Viewer inspector.
/// Kept in the global namespace so any runtime assembly can reference it.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PrefabViewerIgnoreComponentAttribute : Attribute
{
}

