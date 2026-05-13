using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setup")]
    public Transform target; // The player

    [Header("Camera Positioning")]
    public Vector3 offset = new Vector3(1.5f, 1.5f, -4f); 

    [Header("Mouse Aiming (Vertical)")]
    public float mouseSensitivity = 200f;
    public float minViewAngle = -60f; // How far down you can look
    public float maxViewAngle = 60f;  // How far up you can look
    
    private float verticalRotation = 0f;

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
            return;
        }

        // 1. VERTICAL AIMING: Tilt the camera up and down using the Mouse
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY; // Subtract so controls aren't inverted
        verticalRotation = Mathf.Clamp(verticalRotation, minViewAngle, maxViewAngle);

        // 2. POSITION: Snap exactly to the over-the-shoulder spot
        Vector3 targetPosition = target.position 
                               + (target.right * offset.x) 
                               + (target.up * offset.y) 
                               + (target.forward * offset.z);
        transform.position = targetPosition;

        // 3. ROTATION: Combine the Camera's Up/Down tilt with the Player's Left/Right rotation
        transform.rotation = Quaternion.Euler(verticalRotation, target.eulerAngles.y, 0f);
    }
}