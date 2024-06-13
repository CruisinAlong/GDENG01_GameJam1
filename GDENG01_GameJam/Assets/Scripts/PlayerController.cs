using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this line to use the UI components

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float baseJumpForce = 5f;
    [SerializeField] public float maxJumpForce = 10f;
    [SerializeField] public float maxJumpTime = 3f;
    [SerializeField] public float chargedJumpMultiplier = 1.5f;
    [SerializeField] public float speed = 10.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController controller;
    [SerializeField] public float turnSmoothTime = 0.1f;
    [SerializeField] public float forwardJumpMultiplier = 2.0f;
    [SerializeField] public float climbSpeed = 5.0f;
    [SerializeField] private string climbableTag = "Climbable";
    [SerializeField] private Slider jumpForceSlider;
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private float maxHealth = 100f; 
    [SerializeField] private float fallDamageThreshold = 10f;

    private Animator animator;
    private Rigidbody rb;
    private float turnSmoothVelocity;
    private bool isGrounded =false;
    private bool isJumping = false;
    private bool jumpKeyHeld = false;
    private float jumpTimeCounter;
    private bool isClimbing;
    private Collider climbableObject;
    private float currentHealth; 

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>();
        jumpForceSlider.minValue = baseJumpForce;
        jumpForceSlider.maxValue = maxJumpForce * chargedJumpMultiplier;
        jumpForceSlider.value = baseJumpForce;

        currentHealth = maxHealth; 
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void Update()
    {
        this.InputListener();

        if (isClimbing)
        {
            this.Climb();
        }
        else if (!jumpKeyHeld)
        {
            this.Move();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;

            // Handle fall damage
            if (rb.velocity.y < -fallDamageThreshold)
            {
                TakeDamage(Mathf.Abs(rb.velocity.y));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(climbableTag))
        {
            isClimbing = true;
            climbableObject = other;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(climbableTag))
        {
            isClimbing = false;
            climbableObject = null;
            rb.useGravity = true;
        }
    }

    private void InputListener()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isClimbing)
            {
                jumpKeyHeld = true;
                isJumping = true;
                jumpTimeCounter = 0f;
            }
            else if (Input.GetKey(KeyCode.Space) && isJumping)
            {
                jumpTimeCounter += Time.deltaTime;
                UpdateJumpForceSlider();
            }
            else if (Input.GetKeyUp(KeyCode.Space) && isJumping)
            {
                isGrounded = false;
                isJumping = false;
                jumpKeyHeld = false;
                isGrounded = false;
                isJumping = false;

                StartJump();
            }
        }
    }

    private void StartJump()
    {
        float clampedJumpTime = Mathf.Clamp(jumpTimeCounter, 0f, maxJumpTime);
        float jumpForce;

        if (clampedJumpTime >= maxJumpTime)
        {
            jumpForce = maxJumpForce * chargedJumpMultiplier;
        }
        else
        {
            jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, clampedJumpTime / maxJumpTime);
        }

        Vector3 forwardDirection = playerCamera.transform.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 jumpDirection = forwardDirection * forwardJumpMultiplier * (jumpForce / maxJumpForce) + Vector3.up * jumpForce;

        controller.Move(jumpDirection * Time.deltaTime);
        jumpForceSlider.value = baseJumpForce;
    }

    private void UpdateJumpForceSlider()
    {
        float clampedJumpTime = Mathf.Clamp(jumpTimeCounter, 0f, maxJumpTime);
        float jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, clampedJumpTime / maxJumpTime);
        if (clampedJumpTime >= maxJumpTime)
        {
            jumpForce *= chargedJumpMultiplier;
        }
        jumpForceSlider.value = jumpForce;
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f) 
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void Climb()
    {
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 climbDirection = new Vector3(0, verticalInput * climbSpeed * Time.deltaTime, 0);
        rb.MovePosition(transform.position + climbDirection);
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {

            Debug.Log("Player has died.");
        }
    }
}
