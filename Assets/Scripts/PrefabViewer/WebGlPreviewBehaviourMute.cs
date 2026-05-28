using UnityEngine;

namespace PrefabViewer
{
    /// <summary>
    /// Disables VRChat gameplay scripts that require scene references or ShaderGraph-only material properties.
    /// </summary>
    static class WebGlPreviewBehaviourMute
    {
        static readonly string[] MutedTypeNames =
        {
            "ProbeBehabiour",
            "UpdatePreview",
            "FaceBehaviour",
            "ActivateMirror",
            "LijaCuadrada",
            "IdentifyFace",
        };

        public static int Apply(GameObject root)
        {
            if (root == null || Application.platform != RuntimePlatform.WebGLPlayer)
                return 0;

            var muted = 0;
            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour == null)
                    continue;

                var typeName = behaviour.GetType().Name;
                for (var i = 0; i < MutedTypeNames.Length; i++)
                {
                    if (typeName != MutedTypeNames[i])
                        continue;

                    if (behaviour.enabled)
                    {
                        behaviour.enabled = false;
                        muted++;
                    }

                    break;
                }
            }

            return muted;
        }
    }
}
