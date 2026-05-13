using UnityEngine;

public class AimController : MonoBehaviour
{
    [Header("Camera Tracking")]
    public Transform cameraRoot; // Drag your CameraRoot object here in the Inspector!

    [Header("Sensitivity")]
    public float mouseSensitivity = 2f;

    [Header("Up/Down Limits")]
    public float topClamp = -40f; // How far up you can look
    public float bottomClamp = 70f; // How far down you can look

    private float cameraTargetPitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // rotate the entire Player Left/Right (Yaw)
        transform.Rotate(Vector3.up * mouseX);

        // calculate Up/Down Rotation (Pitch)
        cameraTargetPitch -= mouseY;

        // Clamp it so we don't do backflips
        cameraTargetPitch = Mathf.Clamp(cameraTargetPitch, topClamp, bottomClamp);

        // apply Up/Down Rotation ONLY to the Camera Root
        cameraRoot.localRotation = Quaternion.Euler(cameraTargetPitch, 0f, 0f);
    }
}