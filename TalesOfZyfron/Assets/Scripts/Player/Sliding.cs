using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Sliding : NetworkBehaviour
{
    // References
    public CapsuleCollider capsuleCollider1;
    public CapsuleCollider capsuleCollider2;
    public Camera camera;
    public Transform orientation;
    public Transform Player;
    private Rigidbody rb;
    private NewPlayerController pc;
    private KeybindManager keybind;
    public Animator animator;

    // Slide
    public float slideForce = 15f;
    private float slideScale = 0.5f;
    private float playerBaseScale;
    private float slideTimer;
    public float maxSlideTime = 4f;
    private PlayerViewBobbing view;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.None;
    private float horizontalInput;
    private float verticalInput;
    private bool stopSlide = false;

    private void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<NewPlayerController>();
        playerBaseScale = Player.localScale.y;
        keybind = GetComponent<KeybindManager>();
        slideKey = keybind.slideKey;
        view = GetComponent<PlayerViewBobbing>();

    }
    private void Update()
    {
        if (!IsOwner) return;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && pc.isGrounded)
        {
            StartSlide();
            animator.SetBool("IsSliding", true);
        }
        if (Input.GetKeyUp(slideKey) && pc.isSliding){
            stopSlide = true;
           
        }
        if (stopSlide == true && !pc.ceiling)
        {
            stopSlide = false;
            StopSlide();
            animator.SetBool("IsSliding", false);
        }
       
            
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (pc.isSliding) 
        {
            SlidingMovement(); 
        }
        
    }
    /*private void StartSlide()
    {
        pc.isSliding = true;
        camera.transform.localPosition = new Vector3(0, 0.10f, 0);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideTimer = maxSlideTime;
        capsuleCollider1.height = 0.5f;
        capsuleCollider1.center = new Vector3(capsuleCollider1.center.x, -0.5f, capsuleCollider1.center.z);
        capsuleCollider2.height = 0.5f;
        capsuleCollider2.center = new Vector3(capsuleCollider2.center.x, -0.5f, capsuleCollider2.center.z);


    }
    private void StopSlide()
    {
        pc.isSliding = false;
        camera.transform.localPosition = new Vector3(0, 0.75f, 0);
        capsuleCollider1.height = 2;
        capsuleCollider1.center = new Vector3(capsuleCollider1.center.x, 0, capsuleCollider1.center.z);
        capsuleCollider2.height = 2;
        capsuleCollider2.center = new Vector3(capsuleCollider2.center.x, 0, capsuleCollider2.center.z);

    }*/
    private IEnumerator SmoothSlide(bool startSlide)
{
    float targetHeight = startSlide ? 0.5f : 2f;
    float targetCenterY = startSlide ? -0.5f : 0f;
    float targetCameraY = startSlide ? 0.10f : 0.75f;
    Vector3 targetCapsuleCenter1 = new Vector3(capsuleCollider1.center.x, targetCenterY, capsuleCollider1.center.z);
    Vector3 targetCapsuleCenter2 = new Vector3(capsuleCollider2.center.x, targetCenterY, capsuleCollider2.center.z);

    float duration = 0.2f; // Duration of the transition
    float elapsedTime = 0f;

    // Initial values
    float startHeight = capsuleCollider1.height;
    Vector3 startCenter1 = capsuleCollider1.center;
    Vector3 startCenter2 = capsuleCollider2.center;
    float startCameraY = camera.transform.localPosition.y;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;

        capsuleCollider1.height = Mathf.Lerp(startHeight, targetHeight, t);
        capsuleCollider1.center = Vector3.Lerp(startCenter1, targetCapsuleCenter1, t);
        capsuleCollider2.height = Mathf.Lerp(startHeight, targetHeight, t);
        capsuleCollider2.center = Vector3.Lerp(startCenter2, targetCapsuleCenter2, t);
        camera.transform.localPosition = new Vector3(0, Mathf.Lerp(startCameraY, targetCameraY, t), 0);

        yield return null;
    }

    // Ensure final values are set
    capsuleCollider1.height = targetHeight;
    capsuleCollider1.center = targetCapsuleCenter1;
    capsuleCollider2.height = targetHeight;
    capsuleCollider2.center = targetCapsuleCenter2;
    camera.transform.localPosition = new Vector3(0, targetCameraY, 0);

    if (startSlide)
    {
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
}

private void StartSlide()
{
    view.enabled = false;
    pc.isSliding = true;
    slideTimer = maxSlideTime;
    StartCoroutine(SmoothSlide(true));
}

private void StopSlide()
{
    pc.isSliding = false;
    StartCoroutine(SmoothSlide(false));
    view.enabled = true;
}
    
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // sliding normal
        if (!pc.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        else
        {
            rb.AddForce(pc.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
            
        }

        if (slideTimer <= 0 && !pc.ceiling)
            StopSlide();
    }
}



