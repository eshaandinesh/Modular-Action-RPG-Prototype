using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private PlayerStats stats;

    [Header("Movement Feel")]
    public float jumpHeight = 2.0f;
    public float gravity = -30f;
    private Vector3 gravityVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (controller.isGrounded && gravityVelocity.y < 0)
            gravityVelocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // move relative to the way the character's body is currently facing
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        // apply movement if there is input
        if (moveDirection.magnitude >= 0.1f)
            controller.Move(moveDirection * stats.currentSpeed * Time.deltaTime);

        gravityVelocity.y += gravity * Time.deltaTime;
        controller.Move(gravityVelocity * Time.deltaTime);

        // Find the Animator on whichever character model is currently active
        Animator currentAnimator = GetComponentInChildren<Animator>();

        if (currentAnimator != null)
        {
            // Send our movement strength (0 when still, 1 when pressing WASD) to the "Speed" parameter
            currentAnimator.SetFloat("Speed", moveDirection.magnitude);
        }
    }
}