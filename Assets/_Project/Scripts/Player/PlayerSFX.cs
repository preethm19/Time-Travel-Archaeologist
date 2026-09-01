using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Footsteps")]
    public AudioClip[] footstepSounds;

    [Header("Player Actions")]
    public AudioClip jumpSound;

    [Header("Footstep Timing")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.32f;

    private float stepTimer = 0f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        HandleFootsteps();
        HandleJumpSound();
    }

    private void HandleFootsteps()
    {
        // Player must be on the ground
        if (!controller.isGrounded)
        {
            stepTimer = 0f;
            return;
        }

        // WASD movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // Not moving
        if (movement.magnitude < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        // Detect running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentInterval =
            isRunning ? runStepInterval : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, footstepSounds.Length);

        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    private void HandleJumpSound()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

}