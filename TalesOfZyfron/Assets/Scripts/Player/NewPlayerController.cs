using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class NewPlayerController : NetworkBehaviour
{
    // Callables
    
    private SwordSwingScript swordSwingScript;
    public CapsuleCollider capsuleCollider;
    public CapsuleCollider capsuleCollider2;
    public Rigidbody rb;
    private KeybindManager keybind;
    private PlayerHealth playerHealth;
    public GameObject loadingScreen, deathScreen;
    public Animator animator; // Added Animator
   
   [Header("Crouch")]
   public GameObject Bean;
   public Camera camera;
   public AudioSource audioSource;
   public List<AudioClip> footsteps;
   public AudioClip slide_sound;


    //Movement
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;


    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    public Transform orientation;

    //Input
    private KeyCode sprintKey = KeyCode.None;
    private KeyCode jumpKey = KeyCode.None;
    private KeyCode forwardKey = KeyCode.None;
    private KeyCode crouchKey = KeyCode.None;
    private KeyCode left = KeyCode.None;
    private KeyCode right = KeyCode.None;
    private KeyCode backward = KeyCode.None;
    private KeyCode respawn = KeyCode.None;






    //Ground
    [Header("GroundCheck")]
    public LayerMask groundLayer;
    public bool isGrounded;
    public float groundDrag;

    [Header("Jump")]
    public float jumpForce;
    float playerJumpInput;
    public bool readyToJump;
    public float airMult;

    [Header("Crouch")]
    public bool crouchInput;
    public float crouchSpeed;
    private float crouchScale = 0.5f;
    private float playerBaseScale;
    private bool stopCrouch;
    public bool isCrouch;

    [Header("Slope")]
    private float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public bool ceiling;


    //Player
    public float playerHeight;

    public MovementState currState;

    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air,
        dead

    }
    public bool isSliding;
    public bool isWallRunning;
    private PlayerViewBobbing view;
    


    // Start is called before the first frame update
    void Start()
    {
    
        if (!IsOwner) return;
        
        
        //Init player
        rb = GetComponent<Rigidbody>();
        keybind = GetComponent<KeybindManager>();
        playerHealth = GetComponent<PlayerHealth>();
        deathScreen.SetActive(false);
        loadingScreen.SetActive(false);

        
        rb.freezeRotation = true;
        readyToJump = true;
        playerBaseScale = transform.localScale.y;

        //Define Keybinds
        jumpKey = keybind.jumpKey;
        crouchKey = keybind.crouchKey;
        sprintKey = keybind.sprintKey;
        forwardKey = keybind.forwardKey;
        left = keybind.leftKey;
        right = keybind.rightKey;
        backward = keybind.backwardKey;
        respawn = keybind.respawn;
        swordSwingScript = GetComponent<SwordSwingScript>();
        stopCrouch = false;
        view = GetComponent<PlayerViewBobbing>();

        
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        //DebugBool();
        PlayerInput();
        StateHandler();
        SpeedControl();
        GroundCheck();
        CeillingCheck();
        Drag();
        HandleFootsteps();
        if (transform.position.y <= -60)
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(0,1,0);
        }
        if (Input.GetKeyDown(respawn))
        {
            GameObject gm = GameObject.FindGameObjectWithTag("Game Manager");
            if (gm != null)
            {
                GameManager gms = gm.GetComponent<GameManager>();
                gms.LoadSceneClientRpc("Level Boss");
            }
        }
    }
    //FixedUpdate is called once every time
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MovePlayer();
    }

    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded){
            animator.SetBool("IsJumping", true);
            Jump();
        }
                
        if (Input.GetKeyDown(crouchKey) && isGrounded)
            Crouch(true);
        if (Input.GetKeyUp(crouchKey) && isGrounded)
            stopCrouch = true;
        if (stopCrouch && !ceiling)
        {
            Crouch(false);
            view.enabled = true;
            stopCrouch=false;
        }

    }
    private void StateHandler()
    {
        //Dead
        if(playerHealth.isDead)
        {
            currState = MovementState.dead;
        }
        // Wallrun
        if(isWallRunning)
        {
            currState = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        
        else if (Input.GetKey(KeyCode.J)){
            animator.SetBool("Dance",true);
        }
        // Slide
        else if(isSliding)
        {
            currState = MovementState.sliding;
            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;

        }
        // Crouch
        else if (isCrouch && isGrounded)
        {
            currState = MovementState.crouching ;
            desiredMoveSpeed = crouchSpeed;
            animator.SetBool("IsCrouching",true);
        }
        // Sprint
        else if (isGrounded && Input.GetKey(sprintKey) && Input.GetKey(forwardKey))
        {
    
            currState = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingRight", false);
            animator.SetBool("IsWalkingBack", false);
            animator.SetBool("IsJumping",false);
            animator.SetBool("Dance",false);
        }
        // walk
        else if (isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            currState = MovementState.walking;
            animator.SetBool("IsRunning", false);
            if (Input.GetKey(forwardKey)){
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingBack", false);
                animator.SetBool("IsJumping",false);
                animator.SetBool("Dance",false);
            }
            if (Input.GetKey(backward)){
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingBack", true);
                animator.SetBool("IsJumping",false);
                animator.SetBool("Dance",false);
            }
            if (Input.GetKey(left)){
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsWalkingLeft", true);
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsWalkingBack", false);
                animator.SetBool("IsJumping",false);
                animator.SetBool("Dance",false);
            }
            if (Input.GetKey(right)){
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingRight", true);
                animator.SetBool("IsWalkingBack", false);
                animator.SetBool("IsJumping",false);            
                animator.SetBool("Dance",false);
            }
            
    
            
            desiredMoveSpeed = walkSpeed;
        }
        else if (isGrounded)
        {
            currState = MovementState.idle;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingRight", false);
            animator.SetBool("IsWalkingBack", false);
            animator.SetBool("IsCrouching",false);
            animator.SetBool("IsJumping",false);
            animator.SetBool("IsSliding",false);
            animator.SetBool("Dance",false);
            desiredMoveSpeed = 0;
        }
        // air
        else
     
        {
            currState = MovementState.air;
            animator.SetBool("IsJumping", true);
            animator.SetBool("Dance",false);
        }
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;

    }
    private void MovePlayer()
    {
        //Movement
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if (isGrounded) // On ground movement
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10);

        }
        else if (!isGrounded) // On air movement
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10* airMult);

        }
        if (!isWallRunning) rb.useGravity = !OnSlope();
        
    }
    private void SpeedControl()
    {
        //Liming speed on slope
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed) 
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        //Limiting maximum Velocity
        else
        {
            //Limiting maximum Velocity
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }

    private void CeillingCheck()
    {
   
        ceiling = Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f, groundLayer);

    }
    private void GroundCheck()
    {
        // Checks if player is grounded 
        
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

    // Reset the IsJumping animation bool when the player lands
        if (!wasGrounded && isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
    }
    private void Drag()
    {
        // Apply drag based on ground state
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }
    private void Jump()
    {
            exitingSlope = true;
            readyToJump = false;

            // Reset y velocity 
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            // Perform the jump

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            
            Invoke(nameof(ResetJump), 0.25f) ;//JumpCooldown
            
        
    }
    private void ResetJump() 
    {
        // Reset jump availability
        readyToJump = true;
        exitingSlope = false;
    }
    /*private void Crouch(bool crouch)
    {
        isCrouch = crouch;
        if (crouch)
        {
            //Bean.transform.localScale = new Vector3(transform.localScale.x,crouchScale, transform.localScale.z);
            capsuleCollider.height = 0.5f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, -0.5f, capsuleCollider.center.z);
            capsuleCollider2.height = 0.5f;
            capsuleCollider2.center = new Vector3(capsuleCollider2.center.x, -0.5f, capsuleCollider2.center.z);
            camera.transform.localPosition = new Vector3(0, 0.10f, 0);
            rb.AddForce(Vector3.down *5f, ForceMode.Impulse);
        }
        
        else
        {
            capsuleCollider.height = 2;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, 0, capsuleCollider.center.z);
            capsuleCollider2.height = 2f;
            capsuleCollider2.center = new Vector3(capsuleCollider2.center.x, 0, capsuleCollider2.center.z);
            //Bean.transform.localScale = new Vector3(transform.localScale.x, playerBaseScale, transform.localScale.z);
            camera.transform.localPosition = new Vector3(0, 0.75f, 0);
        }
    }*/
    private IEnumerator SmoothCrouch(bool crouch)
{
    
    
    float targetHeight = crouch ? 0.5f : 2f;
    float targetCenterY = crouch ? -0.5f : 0f;
    float targetCameraY = crouch ? 0.10f : 0.75f;
    Vector3 targetCapsuleCenter = new Vector3(capsuleCollider.center.x, targetCenterY, capsuleCollider.center.z);
    Vector3 targetCapsuleCenter2 = new Vector3(capsuleCollider2.center.x, targetCenterY, capsuleCollider2.center.z);

    float duration = 0.2f; // Duration of the transition
    float elapsedTime = 0f;

    // Initial values
    float startHeight = capsuleCollider.height;
    Vector3 startCenter = capsuleCollider.center;
    Vector3 startCenter2 = capsuleCollider2.center;
    float startCameraY = camera.transform.localPosition.y;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;

        capsuleCollider.height = Mathf.Lerp(startHeight, targetHeight, t);
        capsuleCollider.center = Vector3.Lerp(startCenter, targetCapsuleCenter, t);
        capsuleCollider2.height = Mathf.Lerp(startHeight, targetHeight, t);
        capsuleCollider2.center = Vector3.Lerp(startCenter2, targetCapsuleCenter2, t);
        camera.transform.localPosition = new Vector3(0, Mathf.Lerp(startCameraY, targetCameraY, t), 0);

        yield return null;
    }

    // Ensure final values are set
    capsuleCollider.height = targetHeight;
    capsuleCollider.center = targetCapsuleCenter;
    capsuleCollider2.height = targetHeight;
    capsuleCollider2.center = targetCapsuleCenter2;
    camera.transform.localPosition = new Vector3(0, targetCameraY, 0);

    if (crouch)
    {
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    
}

private void Crouch(bool crouch)
{
    
    view.enabled = false;
    isCrouch = crouch;
    StartCoroutine(SmoothCrouch(crouch));
    
}
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }
    public void ResetPlayerPosition()
    {
        transform.position = Vector3.zero; 
        transform.rotation = Quaternion.identity;
    }
    public void SetLoadingScreen(bool val)
    {
       loadingScreen.SetActive(val);
        
    }
    public void DebugBool(){
        Debug.Log(animator.GetBool("IsWalking")+": IsWalking");
        Debug.Log(animator.GetBool("IsRunning")+": IsRunning");
        Debug.Log(animator.GetBool("IsWalkingLeft")+": IswlakingLeft");
        Debug.Log(animator.GetBool("IsWalkingRight")+": IsWalkingRight");
        Debug.Log(animator.GetBool("IsCrouching")+": IsCrouching");
        Debug.Log(animator.GetBool("IsJumping")+": IsJumping");
    }
private void HandleFootsteps()
{
    if (isSliding)
    {
        if (!audioSource.isPlaying || audioSource.clip != slide_sound)
        {
            audioSource.clip = slide_sound;
            audioSource.loop = true;
            audioSource.pitch = 1f; // Ensure the pitch for sliding sound is always 1
            audioSource.Play();
        }
    }
    else if (currState == MovementState.walking || currState == MovementState.sprinting || currState == MovementState.crouching)
    {
        if (!audioSource.isPlaying || audioSource.clip == slide_sound)
        {
            PlayRandomFootstep();
        }

        if (currState == MovementState.walking)
        {
            audioSource.pitch = 1f; // Normal speed for walking
            SetAudioPitch(1f);
        }
        else if (currState == MovementState.sprinting)
        {
            audioSource.pitch = 1.5f; // Faster speed for sprinting
            SetAudioPitch(1f / 1.5f);
        }
    }
    else
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}

    private void PlayRandomFootstep()
{
    System.Random random = new System.Random();
    int index = random.Next(footsteps.Count);
    audioSource.clip = footsteps[index];
    audioSource.loop = false; // Ensure the clip does not loop
    audioSource.Play();
}
    private void SetAudioPitch(float pitch)
    {
        if (audioSource.outputAudioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", pitch);
        }
    }
}
