using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float baseJumpForce = 5f;
    [SerializeField] public float maxJumpForce = 10f;
    [SerializeField] public float maxJumpTime = 7f;
    [SerializeField] public float chargedJumpMultiplier = 15f;
    [SerializeField] public float speed = 1.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] public float turnSmoothTime = 0.1f;
    [SerializeField] public float forwardJumpMultiplier = 35.0f;
    [SerializeField] public float climbSpeed = 5.0f;
    [SerializeField] private string climbableTag = "Climbable";
    [SerializeField] private Slider jumpForceSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float fallDamageThreshold = 10f;

    private Animator animator;
    private Rigidbody rb;
    private float turnSmoothVelocity;
    public bool isGrounded = false;
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
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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
        else if (!jumpKeyHeld && isGrounded)
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
            Debug.Log("Collision with Ground. isGrounded: " + isGrounded);

            // Handle fall damage
            if (rb.velocity.y < -fallDamageThreshold)
            {
                TakeDamage(Mathf.Abs(rb.velocity.y));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left Ground. isGrounded: " + isGrounded);
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
            Debug.Log("Entered Climbable area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(climbableTag))
        {
            isClimbing = false;
            climbableObject = null;
            rb.useGravity = true;
            Debug.Log("Exited Climbable area");
        }
    }

    private void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isClimbing && isGrounded)
        {
            jumpKeyHeld = true;
            isJumping = true;
            jumpTimeCounter = 0f;
            Debug.Log("Space key down. Jump started.");
        }
        else if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            jumpTimeCounter += Time.deltaTime;
            UpdateJumpForceSlider();
            Debug.Log("Space key held. Jump time counter: " + jumpTimeCounter);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            isGrounded = false;
            isJumping = false;
            jumpKeyHeld = false;

            StartJump();
            Debug.Log("Space key up. Jump executed.");
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
        rb.velocity = jumpDirection;

        jumpForceSlider.value = baseJumpForce;

        Debug.Log("Jump direction: " + jumpDirection + " Jump force: " + jumpForce);
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

        Debug.Log("Updated jump force slider: " + jumpForce);
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.velocity = moveDirection.normalized * speed;
            animator.SetBool("IsMoving", true);

            Debug.Log("Moving. Direction: " + direction + " Target angle: " + targetAngle + " Move direction: " + moveDirection);
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

        Debug.Log("Climbing. Vertical input: " + verticalInput + " Climb direction: " + climbDirection);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;

        Debug.Log("Took damage: " + amount + " Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
}
