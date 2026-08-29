using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Automatically find the camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Automatically find Animator in the Knight child
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        Move();
        HandleJump();
        ApplyGravity();
    }

    private void Move()
    {
        if (Keyboard.current == null || cameraTransform == null)
            return;

        // --------------------------------
        // 1. Get WASD input
        // --------------------------------

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal += 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical -= 1f;

        if (Keyboard.current.wKey.isPressed)
            vertical += 1f;

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical);

        // Prevent diagonal movement from being faster
        inputDirection =
            Vector3.ClampMagnitude(inputDirection, 1f);

        float inputAmount = inputDirection.magnitude;

        // --------------------------------
        // 2. Check whether player is running
        // --------------------------------

        bool isRunning =
            Keyboard.current.leftShiftKey.isPressed ||
            Keyboard.current.rightShiftKey.isPressed;

        // --------------------------------
        // 3. Update Animator
        // --------------------------------

        if (animator != null)
        {
            animator.SetFloat("Speed", inputAmount);
            animator.SetBool("IsRunning", isRunning && inputAmount > 0.01f);
        }

        // --------------------------------
        // 4. No movement input
        // --------------------------------

        if (inputAmount < 0.01f)
            return;

        // --------------------------------
        // 5. Camera directions
        // --------------------------------

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Remove vertical component
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // --------------------------------
        // 6. Camera-relative movement
        // --------------------------------

        Vector3 moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        moveDirection.Normalize();

        // --------------------------------
        // 7. Choose walk or run speed
        // --------------------------------

        float currentSpeed = isRunning
            ? runSpeed
            : walkSpeed;

        // --------------------------------
        // 8. Move player
        // --------------------------------

        controller.Move(
            moveDirection *
            currentSpeed *
            Time.deltaTime
        );

        // --------------------------------
        // 9. Rotate player toward movement
        // --------------------------------

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleJump()
    {
        if (Keyboard.current == null)
            return;

        // Only allow jumping while grounded
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            controller.isGrounded)
        {
            // Jump physics
            velocity.y =
                Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );

            // Tell Animator to play jump animation
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void ApplyGravity()
    {
        // Keep player attached to ground
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move vertically
        controller.Move(
            velocity * Time.deltaTime
        );
    }
}