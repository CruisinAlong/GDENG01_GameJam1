using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 10f;
    [SerializeField] private float maxJumpTime = 3f;
    [SerializeField] private float chargedJumpMultiplier = 1.5f;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float forwardJumpMultiplier = 2.0f;
    [SerializeField] private float climbSpeed = 5.0f;
    [SerializeField] private string climbableTag = "Climbable";
    [SerializeField] private string checkTag = "Checkpoint";
    [SerializeField] private Slider jumpForceSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float fallDamageThreshold = 10f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
    private bool jumpKeyHeld;
    private float jumpTimeCounter;
    private bool isClimbing;
    private Collider climbableObject;
    private float currentHealth;

    private Vector3 moveDirection = Vector3.zero;
    private CheckpointManager checkpointManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpForceSlider.minValue = baseJumpForce;
        jumpForceSlider.maxValue = maxJumpForce * chargedJumpMultiplier;
        jumpForceSlider.value = baseJumpForce;

        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        checkpointManager = FindAnyObjectByType<CheckpointManager>();
        if (checkpointManager == null)
        {
            Debug.LogError("CheckpointManager not found in the scene.");
        }
    }

    void Update()
    {
        InputListener();

        if (isClimbing)
        {
            Climb();
        }
        else if (!jumpKeyHeld)
        {
            Move();
        }

    }

    private void Die()
    {
        Debug.Log("Died");
        TakeDamage(currentHealth); 
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;

            if (rb.velocity.y < -fallDamageThreshold)
            {
                TakeDamage(Mathf.Abs(rb.velocity.y));
            }
        }
        else
        {
            Debug.Log("Unrelated Collision GameObject");
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
        else if (other.CompareTag(checkTag))
        {
            checkpointManager.SetCheckpoint(other.transform.position);
            Debug.Log("Checkpoint reached!");
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
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
            jumpKeyHeld = false;
            isGrounded = false;
            isJumping = false;

            StartJump();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
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

        rb.velocity = new Vector3(jumpDirection.x, jumpDirection.y, jumpDirection.z);
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
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += playerCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= playerCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= playerCamera.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += playerCamera.transform.right;
        }


        moveDirection.y = 0;
        moveDirection.Normalize();

        Vector3 movement = moveDirection * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
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
            checkpointManager.PlayerDied();
        }
    }

    public void RespawnAtCheckpoint(Vector3 checkpointPosition)
    {
        transform.position = checkpointPosition;
        rb.velocity = Vector3.zero;
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
        Debug.Log("Respawned at checkpoint.");
    }
}
