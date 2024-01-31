using System.Collections;
using UnityEngine;



public class Player : MonoBehaviour
{

    private float horizontalInput;
    private float verticalMovement;
    private float smoothInputSpeed = 0.1f; // Time to smooth input, adjust as needed
    private float currentVerticalInput = 0f;

    private bool controlsEnabled = true;
    private bool isGrounded = false;
    private bool wasGroundedLastFrame = false;
    private bool canJump = false;
    private bool jumpRequested = false;
    private bool isTouchingLeft = false;
    private bool isTouchingRight = false;
    private bool canClimb = false;
    private bool wallJumpRequested = false;
    private bool wallDetectionEnabled = true;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController nucleusAnimations;
    [SerializeField] private AnimatorOverrideController mitochondriaAnimations;
    [SerializeField] private AnimatorOverrideController gogliAnimations;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject trailEffect;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float wallJumpForce = 17f;
    [SerializeField] private float climbSpeed = 1f;

    // Ground Check Variables ///////////////////////////
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float checkRadius;
    /////////////////////////////////////////////////////

    // Wall check variables ///////////////////////////
    [SerializeField] private Transform leftClimbableCheck;
    [SerializeField] private Transform rightClimbableCheck;
    [SerializeField] private float wallCheckRadius;
    [SerializeField] private LayerMask whatIsClimbable;
    /////////////////////////////////////////////////////


    private void Start()
    {
        trailEffect.SetActive(false);
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            HandleMovement();

            // Ground Jump Input
            if (canJump && isGrounded && Input.GetButtonDown("Jump"))
            {
                jumpRequested = true;
            }

            // Wall Jump Input
            if (canClimb && (isTouchingLeft || isTouchingRight) && Input.GetButtonDown("Jump"))
            {
                wallJumpRequested = true;
            }

            wasGroundedLastFrame = isGrounded;

            if (!CheckIfGrounded())
            {
                CheckIfClimbing();
            }
        }

        // Smooth vertical input for climbing
        float targetVerticalInput = Input.GetAxis("Vertical");
        currentVerticalInput = Mathf.MoveTowards(currentVerticalInput, targetVerticalInput, smoothInputSpeed / Time.deltaTime);

        UpdateAnimationParameters();
    }

    private void FixedUpdate()
    {
        if (controlsEnabled)
        {
            // Apply ground jump force
            if (jumpRequested)
            {
                HandleJumping();
                jumpRequested = false;
            }

            // Apply wall jump force
            if (wallJumpRequested)
            {
                HandleWallJumping();
                wallJumpRequested = false;
            }

            if (!isGrounded)
            {
                HandleClimbing();
            }

            // Update the vertical movement after physics calculations
            verticalMovement = rb.velocity.y;
        }
    }


    private void UpdateAnimationParameters()
    {
        // Update the isClimbing parameter for the animator
        bool isClimbing = canClimb && (isTouchingLeft || isTouchingRight) && !isGrounded;
        animator.SetBool("isClimbing", isClimbing);

        // Update the horizontal speed parameter for the animator
        animator.SetFloat("speed", Mathf.Abs(horizontalInput));

        // Update the isRunning parameter based on the player's horizontal velocity
        animator.SetBool("isRunning", Mathf.Abs(rb.velocity.x) > 0.01f); // Use a small threshold to account for floating point imprecision

        // Update the vertical velocity parameter for the animator
        animator.SetFloat("verticalVelocity", verticalMovement);

        // Update the isGrounded parameter for the animator
        animator.SetBool("isGrounded", isGrounded);


        // Set the climbSpeed parameter based on the direction of climbing
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");
            // Set climbSpeed to positive when moving up, negative when moving down, and zero when idle
            animator.SetFloat("climbSpeed", verticalInput);
        }

        // Check if the player was not grounded in the last frame but is grounded in the current frame, indicating a landing
        if (!wasGroundedLastFrame && isGrounded)
        {
            // Trigger the landing animation
            animator.SetTrigger("Land");

            AudioManager.One.PlayLandSound();
        }

        // Check if the player was grounded in the last frame but is not grounded in the current frame, indicating a jump has started
        if (wasGroundedLastFrame && !isGrounded)
        {
            // Trigger the jump animation
            animator.SetTrigger("Jump");
        }

        // Check for falling condition: not grounded and vertical velocity is negative (moving downwards)
        if (!isGrounded && rb.velocity.y < 0 && !isClimbing)
        {
            // Set the animator to the falling state
            animator.SetBool("isFalling", true);
        }
        else if (isGrounded)
        {
            // If the player is grounded, they are not falling
            animator.SetBool("isFalling", false);
        }


        // Adjust animator speed only if not climbing
        if (!isClimbing)
        {
            animator.speed = Mathf.Clamp(rb.velocity.magnitude / speed, 0.5f, 1.5f);
        }
        else
        {
            // Reset animator speed to 1 when climbing
            animator.speed = 1f;
        }
        
        
        Debug.Log("Animator Speed: " + animator.speed);
    
    }

    private void CheckIfClimbing()
    {
        if (!wallDetectionEnabled) return;

        isTouchingLeft = Physics2D.OverlapCircle(leftClimbableCheck.position, wallCheckRadius, whatIsClimbable);
        isTouchingRight = Physics2D.OverlapCircle(rightClimbableCheck.position, wallCheckRadius, whatIsClimbable);
    }

    private bool CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        return isGrounded;
    }

    private void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        // Flip player sprite based on direction
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;

            if (isGrounded)
            {
                AudioManager.One.PlayWalkingSound();
            }
        }
       
    }

    private void HandleJumping()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void HandleWallJumping()
    {
        if (!wallDetectionEnabled) return;

        Vector2 jumpDirection = isTouchingLeft ? Vector2.right : Vector2.left;
        // Increase the y-component of the jump force for more upward motion
        Vector2 forceToAdd = new Vector2(jumpDirection.x * wallJumpForce, jumpForce + wallJumpForce);
        rb.AddForce(forceToAdd, ForceMode2D.Impulse);

        StartCoroutine(DisableWallDetectionTemporarily());
    }

    private void HandleClimbing()
    {
        if (canClimb && (isTouchingLeft || isTouchingRight))
        {
            float verticalInput = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);

            // If the player is moving upwards, play the climbing animation forward
            if (verticalInput > 0)
            {
                animator.SetFloat("climbSpeed", 1);
            }
            // If the player is moving downwards, play the climbing animation backwards
            else if (verticalInput < 0)
            {
                animator.SetFloat("climbSpeed", -1);
            }
            // If the player is not moving vertically, show the first frame of the climbing animation
            else
            {
                animator.SetFloat("climbSpeed", 0);
            }
        }
    }


    #region UPGRADES

    // Call this method when the nucleus is collected
    public void UpgradeToNucleus()
    {
        AudioManager.One.PlayUpgradeSound();
        
        canJump = true;
        animator.runtimeAnimatorController = nucleusAnimations;
    }

    // Call this method when the mitochondria is collected
    public void UpgradeToMitochondria()
    {
        AudioManager.One.PlayUpgradeSound();

        speed = 8.0f;
        trailEffect.SetActive(true);
        animator.runtimeAnimatorController = mitochondriaAnimations;
    }

    // Call this method when the gogli is collected
    public void UpgradeToGogli()
    {
        AudioManager.One.PlayUpgradeSound();

        canClimb = true;
        animator.runtimeAnimatorController = gogliAnimations;
    }

    #endregion


    // Add a parameter for the portal's center position
    public void EnterPortal(Vector3 portalCenter)
    {
        StartCoroutine(EnterPortalRoutine(portalCenter));

        AudioManager.One.PlayEndPortalSound();
    }

    private IEnumerator DisableWallDetectionTemporarily()
    {
        wallDetectionEnabled = false;
        yield return new WaitForSeconds(0.2f); // Adjust the time as needed
        wallDetectionEnabled = true;
    }

    private IEnumerator EnterPortalRoutine(Vector3 portalCenter)
    {
        controlsEnabled = false; // Disable player controls
        rb.velocity = Vector2.zero; // Reset velocity to stop movement
        rb.isKinematic = true; // Make Rigidbody kinematic to ignore physics forces

        // Move towards the portal center
        float moveDuration = 0.2f; // Duration to move towards the portal center
        float moveElapsed = 0;

        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            float moveFraction = moveElapsed / moveDuration;

            // Interpolate position towards the portal center
            transform.position = Vector3.Lerp(transform.position, portalCenter, moveFraction);

            yield return null;
        }

        // Start shrinking and rotating
        Vector3 originalScale = transform.localScale; // Remember original scale
        Quaternion rotation = Quaternion.Euler(0, 0, 360); // Full circle rotation

        float shrinkDuration = 1.0f; // Duration of the shrinking and rotating animation
        float shrinkElapsed = 0;

        while (shrinkElapsed < shrinkDuration)
        {
            shrinkElapsed += Time.deltaTime;
            float shrinkFraction = shrinkElapsed / shrinkDuration;

            // Rotate and scale down
            transform.Rotate(0, 0, rotation.eulerAngles.z * Time.deltaTime);
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, shrinkFraction);

            yield return null;
        }

        gameObject.SetActive(false); // Make the player invisible/disappear
        rb.isKinematic = false; // Revert Rigidbody to non-kinematic after animation
    }



    private void OnEnable()
    {
        // Subscribe to events
        EventManager.OnNucleusPickedUp += UpgradeToNucleus;
        EventManager.OnMitochondriaPickedUp += UpgradeToMitochondria;
        EventManager.OnGogliPickedUp += UpgradeToGogli;
        EventManager.OnPortalReached += EnterPortal;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.OnNucleusPickedUp -= UpgradeToNucleus;
        EventManager.OnMitochondriaPickedUp -= UpgradeToMitochondria;
        EventManager.OnGogliPickedUp -= UpgradeToGogli;
        EventManager.OnPortalReached -= EnterPortal;
    }



    // This function is called even if the script is not active.
    // This makes it useful for visualizing physics checks in the Scene view.
    void OnDrawGizmos()
    {
        // Draw a red circle where the ground check is happening
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(leftClimbableCheck.position, wallCheckRadius);
        Gizmos.DrawWireSphere(rightClimbableCheck.position, wallCheckRadius);
    }
}
