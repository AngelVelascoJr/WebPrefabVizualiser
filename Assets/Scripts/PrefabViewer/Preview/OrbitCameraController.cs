using UnityEngine;

namespace PrefabViewer.Preview
{
    public class OrbitCameraController
    {
        public Vector3 Target { get; private set; }
        public float Distance { get; private set; } = 5f;
        public float MinDistance { get; set; } = 0.5f;
        public float MaxDistance { get; set; } = 50f;

        float yaw = 35f;
        float pitch = 25f;

        public void SetView(Vector3 target, float distance, float yawDegrees, float pitchDegrees)
        {
            Target = target;
            Distance = Mathf.Clamp(distance, MinDistance, MaxDistance);
            yaw = yawDegrees;
            pitch = Mathf.Clamp(pitchDegrees, -85f, 85f);
        }

        public void FrameBounds(Bounds bounds, float padding = 1.25f)
        {
            Target = bounds.center;
            var extent = bounds.extents.magnitude;
            Distance = Mathf.Clamp(Mathf.Max(extent * padding * 2f, 2f), MinDistance, MaxDistance);
            pitch = 25f;
            yaw = 35f;
        }

        public void FocusPoint(Vector3 worldPoint, bool keepDistance = true)
        {
            Target = worldPoint;
            if (!keepDistance)
                Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Orbit(Vector2 deltaPixels, float sensitivity = 0.35f)
        {
            yaw += deltaPixels.x * sensitivity;
            pitch -= deltaPixels.y * sensitivity;
            pitch = Mathf.Clamp(pitch, -85f, 85f);
        }

        public void Pan(Vector2 deltaPixels, Camera camera, float sensitivity = 0.003f)
        {
            var scale = Distance * sensitivity;
            var right = camera.transform.right;
            var up = camera.transform.up;
            Target -= right * deltaPixels.x * scale;
            Target += up * deltaPixels.y * scale;
        }

        public void Zoom(float scrollDelta, float sensitivity = 0.12f)
        {
            Distance -= scrollDelta * Distance * sensitivity;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void ApplyToTransform(Transform cameraTransform)
        {
            var rot = Quaternion.Euler(pitch, yaw, 0f);
            cameraTransform.position = Target + rot * (Vector3.back * Distance);
            cameraTransform.rotation = rot;
        }
    }
}
