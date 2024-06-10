using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseJumpForce = 5f;
    [SerializeField] private float maxJumpForce = 10f;
    [SerializeField] private float maxJumpTime = 3f;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float forwardJumpMultiplier = 2.0f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
    private bool jumpKeyHeld;
    private float jumpTimeCounter;

    private Vector3 moveDirection = Vector3.zero;

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
            jumpTimeCounter += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            jumpKeyHeld = false;
            StartJump();
        }
    }

    private void StartJump()
    {
        float clampedJumpTime = Mathf.Clamp(jumpTimeCounter, 0f, maxJumpTime);
        float jumpForce = Mathf.Lerp(baseJumpForce, maxJumpForce, clampedJumpTime / maxJumpTime);

        Vector3 forwardDirection = playerCamera.transform.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 jumpDirection = forwardDirection * forwardJumpMultiplier * (jumpForce / maxJumpForce) + Vector3.up * jumpForce;

        rb.velocity = new Vector3(jumpDirection.x, jumpDirection.y, jumpDirection.z);
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
        moveDirection.Normalize(); // Normalize to ensure consistent movement speed in diagonal directions

        Vector3 movement = moveDirection * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
}
