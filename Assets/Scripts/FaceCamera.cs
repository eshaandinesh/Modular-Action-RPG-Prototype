using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Tooltip("Leave blank to auto-find the Main Camera, or drag your CameraRoot here.")]
    public Transform cameraTarget;

    void Start()
    {
        if (cameraTarget == null && Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (cameraTarget != null)
        {
            transform.LookAt(transform.position + cameraTarget.forward);
        }
    }
}