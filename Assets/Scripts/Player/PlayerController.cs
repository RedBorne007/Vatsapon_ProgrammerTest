using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Settings")]
    [Tooltip("Speed for walking")]
    [SerializeField] private float walkSpeed = 5f;
    [Tooltip("Speed for running")]
    [SerializeField] private float runSpeed = 7f;
    [Tooltip("Speed for crouching")]
    [SerializeField] private float crouchSpeed = 3f;

    [Space]

    [Tooltip("Speed for turning/rotating")]
    [SerializeField] private float turningSpeed = 0.1f;

    [Header("References")]
    [Tooltip("Rigidbody of player")]
    [SerializeField] private Rigidbody rigid;
    [Tooltip("Transform that use for player's detection")]
    [SerializeField] private Transform detectTransform;
    [Tooltip("Transform of player's camera")]
    [SerializeField] private Transform cam;
    [Tooltip("Animator controller of Player's model")]
    [SerializeField] private Animator animator;
    [Tooltip("Interact handler of this player")]
    [SerializeField] private PlayerInteract interact;
    [Tooltip("Flashlight on player's body")]
    [SerializeField] private GameObject flashLight;

    private const string ANIM_WALK_HASH = "IsWalking";
    private const string ANIM_RUN_HASH = "IsRunning";
    private const string ANIM_CROUCH_HASH = "IsCrouching";
    private const string ANIM_DEATH = "Player_Death";

    private bool isDead;
    private Vector2 moveInput = Vector2.zero; // Input from player's controller.
    private Vector3 moveDirection = Vector3.zero; // Final player's movement direction.
    private bool isCrouching; // Determine if player is crouching or not.
    private bool isRunning; // Determine if player is running or not.

    private float currentTurnVelocity; // Current turn/rotation value.

    private PlayerInput input;
    private AudioManager audioM;

    public Transform Detect => detectTransform;
    public bool IsDead => isDead;
    public bool IsCrouching => isCrouching;
    public bool IsWalking => moveInput != Vector2.zero;
    public bool IsRunning => IsWalking && isRunning;
    public PlayerInput Input => input;
    public PlayerInteract Interact => interact;

    protected override void Awake()
    {
        base.Awake();

        input = new PlayerInput();
        input.Enable();

        audioM = AudioManager.Instance;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        // [F] - Toggle Flashlight.
        if (input.Player.Flashlight.WasPressedThisFrame())
        {
            audioM.PlaySFX("Player_Flashlight_Toggle");
            flashLight.SetActive(!flashLight.activeSelf);
        }

        InputHandler();
        RotationHandler();
        AnimationHandler();
    }

    private void FixedUpdate()
    {
        MovementHandler();
    }

    // Function to make player be able or unable to control character.
    public void SetControllable(bool value)
    {
        if (value)
        {
            input.Enable();
        }
        else
        {
            input.Disable();
        }
    }

    // Function to handle and read input system.
    private void InputHandler()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>().normalized;
        isRunning = input.Player.Run.IsPressed();
        isCrouching = input.Player.Crouch.IsPressed();
    }

    // Function to handle movement.
    private void MovementHandler()
    {
        float targetSpeed = walkSpeed;
        
        // Set speed value based on player's state.
        if (isCrouching)
        {
            targetSpeed = crouchSpeed;
        }
        else if (IsRunning)
        {
            targetSpeed = runSpeed;
        }

        float x = moveDirection.x * targetSpeed * Time.fixedDeltaTime;
        float z = moveDirection.z * targetSpeed * Time.fixedDeltaTime;

        // If there's no movement input, set movement velocty to 0.
        if (moveInput.magnitude < 0.1f)
        {
            x = 0f;
            z = 0f;
        }

        rigid.velocity = new Vector3(x, rigid.velocity.y, z);
    }

    // Function to handle rotation.
    private void RotationHandler()
    {
        // If there's no movement input, return.
        if (moveInput.magnitude < 0.1f)
        {
            return;
        }

        // Make player rotation related to camera's rotation.
        float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float finalAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentTurnVelocity, turningSpeed);

        transform.rotation = Quaternion.Euler(Vector3.up * finalAngle);
        moveDirection = (Quaternion.Euler(Vector3.up * targetAngle) * Vector3.forward).normalized;
    }

    // Function to handle player's animation.
    private void AnimationHandler()
    {
        animator.SetBool(ANIM_WALK_HASH, moveInput.magnitude >= 0.1f);
        animator.SetBool(ANIM_RUN_HASH, moveInput.magnitude >= 0.1f && isRunning);
        animator.SetBool(ANIM_CROUCH_HASH, isCrouching);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If player get caught by enemy while still alive, game over.
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
        {
            isDead = true;
            rigid.isKinematic = true;
            animator.Play(ANIM_DEATH);

            GameManager.Instance.GameOver();
        }
    }
}
