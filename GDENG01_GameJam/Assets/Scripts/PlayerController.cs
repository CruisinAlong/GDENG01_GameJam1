using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 10f; // Maximum jump force
    [SerializeField] private float maxJumpTime = 0.5f; // Maximum time to reach max jump force
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Camera playerCamera;  // Reference to the camera
    [SerializeField] private float forwardJumpMultiplier = 2.0f; // Multiplier for forward jump distance

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
    private bool jumpKeyHeld;
    private float jumpTimeCounter;

    private Vector3 moveDirection = Vector3.zero;

    private enum Direction
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        NONE
    }

    private Direction currentDir = Direction.NONE;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.InputListener();

        if (!jumpKeyHeld)
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
        }
    }

    private void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpKeyHeld = true;
            isJumping = true;
            jumpTimeCounter = 0f;
        }
        else if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            // Increase jump time counter while space is held
            jumpTimeCounter += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            jumpKeyHeld = false;
            StartJump();
        }

        if (!jumpKeyHeld)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentDir = Direction.FORWARD;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                currentDir = Direction.BACKWARD;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                currentDir = Direction.LEFT;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                currentDir = Direction.RIGHT;
            }
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                currentDir = Direction.NONE;
            }
        }
    }

    private void StartJump()
    {
        // Cap the jump time to the maximum allowed time
        float clampedJumpTime = Mathf.Clamp(jumpTimeCounter, 0f, maxJumpTime);

        // Calculate the jump force based on how long the space bar was held
        float jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, clampedJumpTime / maxJumpTime);

        // Calculate forward jump direction and force
        Vector3 jumpDirection = playerCamera.transform.forward * forwardJumpMultiplier * (clampedJumpTime / maxJumpTime) + Vector3.up * jumpForce;

        // Apply the jump force
        rb.velocity = new Vector3(jumpDirection.x, jumpDirection.y, jumpDirection.z);
    }

    private void Move()
    {
        moveDirection = Vector3.zero;

        switch (currentDir)
        {
            case Direction.FORWARD:
                moveDirection = playerCamera.transform.forward;
                break;
            case Direction.BACKWARD:
                moveDirection = -playerCamera.transform.forward;
                break;
            case Direction.LEFT:
                moveDirection = -playerCamera.transform.right;
                break;
            case Direction.RIGHT:
                moveDirection = playerCamera.transform.right;
                break;
        }

        moveDirection.y = 0; // Ensure the movement is horizontal
        Vector3 movement = moveDirection.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
}
