using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float WalkSpeed = 8f;
    public float groundDrag = 7f;
    public float airDrag = 0f;

    private float Speed;

    [Header("Jump settings")]
    public float jumpPower = 15f;
    public float coyoteTime = 0.2f;
    public float jumpBuffer = 0.2f;
    public float jumpCooldown = 0.05f;

    float coyoteTimeCounter;
    float jumpBufferCounter;
    float jumpCooldownCounter;
    bool Jumped;
    [Header("Crouch settings")]
    public float crouchYScale;
    private float startYScale;

    [Header("Dash settings")]
    public float dashCooldown = 2f;
    public float dashTimer = 0.25f;
    public float dashPower = 150f;

    float dashCooldownCounter;
    float dashTimerCounter;
    bool Dashed;
    [Header("Slide Settings")]
    public float slideTimer = 0.5f;
    public float slidePower = 150f;
    public float slideCooldown = 0.5f;
    bool Sliding;

    float slideCooldownCounter;
    float slideTimerCounter;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;


    public Transform orientation;

    // Controls
    float horizontalInput;
    float verticalInput;
    bool jumpInput;
    bool dashInput;
    bool start_slideInput;
    bool stop_slideInput;
    bool crouchingInput;
    bool decrouchingInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Делает чтоб персонаж нестал бухим и упал
        rb.freezeRotation = true;

        // Set movespeed to be walkspeed
        Speed = WalkSpeed;

        startYScale = transform.localScale.y;
    }
    private void Update()
    {
        // Uses raycast to check if players is on ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.3f, whatIsGround);
        Debug.Log(grounded);

        // Handles keyboard inputs for moving
        HandleInput();

        // Makes sure speed doesn't go over limit
        SpeedController();

        // Handles Jumping
        HandleJump();

        // Handles stuff that happends when player is/not on ground
        if (grounded)
        {
            // Sets drag to be on ground
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, groundDrag, 0.05f);
            // Sets coyote timer
            coyoteTimeCounter = coyoteTime;

            if (jumpCooldownCounter <= 0f)
            {
                Jumped = false;
            }
        }
        else
        {
            //Sets drag to be in air
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, airDrag, 0.5f);
            // Sets coyote timer
            coyoteTimeCounter -= Time.deltaTime;
            coyoteTimeCounter = Mathf.Clamp(jumpCooldownCounter, 0f, jumpCooldown);
        }

        HandleDash();
        HandleSlide();
        HandleCrouching();
    }
    private void FixedUpdate()
    {
        MovePlayer();
        //Slide();
        Dash();
    }
    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButtonDown("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        start_slideInput = Input.GetKeyDown(KeyCode.V);
        stop_slideInput = Input.GetKeyUp(KeyCode.V);
        crouchingInput = Input.GetKeyDown(KeyCode.LeftControl);
        decrouchingInput = Input.GetKeyUp(KeyCode.LeftControl);
    }
    private void HandleJump()
    {
        // Checks if jump is possible
        if ((grounded || coyoteTimeCounter > 0f) && (jumpBufferCounter > 0f) && !Jumped)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            jumpCooldownCounter = jumpCooldown;
            Jumped = true;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }
        // Jump buffer
        if (jumpInput)
        {
            jumpBufferCounter = jumpBuffer;
        }
        jumpBufferCounter -= Time.deltaTime;
        jumpBufferCounter = Mathf.Clamp(jumpBufferCounter, 0f, jumpBuffer);
        jumpCooldownCounter -= Time.deltaTime;
        jumpCooldownCounter = Mathf.Clamp(jumpCooldownCounter, 0f, jumpCooldown);
    }
    private void MovePlayer()
    {
        // Gets move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // Moves the rigi body via forces depending on state
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * Speed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * Speed * 5f, ForceMode.Force);
        }
    }
    private void SpeedController()
    {
        // Gets current velocity
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // Checks if velocity is over movespeed
        if (flatVel.magnitude > Speed)
        {
            Vector3 limitedVel = flatVel.normalized * Speed;
            //Caps it to movespeed
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void HandleDash()
    {
        if (dashInput && !Dashed)
        {
            dashTimerCounter = dashTimer;
            dashCooldownCounter = dashCooldown;
        }
        if (dashCooldownCounter == 0f)
        {
            Dashed = false;
        }
        dashCooldownCounter -= Time.deltaTime;
        dashCooldownCounter = Mathf.Clamp(dashCooldownCounter, 0f, dashCooldown);
        dashTimerCounter -= Time.deltaTime;
        dashTimerCounter = Mathf.Clamp(dashTimerCounter, 0f, dashTimer);
    }

    private void HandleCrouching()
    {
        if (crouchingInput)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            if (grounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + -0.49f, transform.position.z);
                Speed *= 0.75f;
            }
        }
        if (decrouchingInput)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            Speed = WalkSpeed;
        }
    }
    private void HandleSlide()
    {
        if (start_slideInput && grounded && slideCooldownCounter == 0f)
        {
            slideTimerCounter = slideTimer;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, transform.position.y + -0.49f, transform.position.z);
            Sliding = true;
        }
        if (stop_slideInput)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            Sliding = false;
            slideCooldownCounter = slideCooldown;
        }
        slideCooldownCounter -= Time.deltaTime;
        slideCooldownCounter = Mathf.Clamp(slideCooldownCounter, 0f, slideCooldown);
    }
    private void Dash()
    {
        if (dashTimerCounter > 0f)
        {
            rb.AddForce(moveDirection.normalized * dashPower, ForceMode.Acceleration);
            Dashed = true;
        }
    }
    private void Slide()
    {
        if (!Sliding || slideTimerCounter <= 0f)
        {
            slideTimerCounter = Mathf.Lerp(slideTimerCounter, 0f, 0.5f);
            Sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
        if (slideTimerCounter > 0f && slideCooldownCounter <= 0f)
        {
            rb.AddForce(orientation.forward * slidePower, ForceMode.Acceleration);
        }
        slideTimerCounter -= Time.deltaTime;
        slideTimerCounter = Mathf.Clamp(slideTimerCounter, 0f, slideTimer);
    }
}
