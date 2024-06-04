using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxJumpTime = 0.5f;
    [SerializeField] private float speed = 10.0f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
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
        this.Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void InputListener()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJump();
        }
        else if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            ContinueJump();
        }
        else if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            EndJump();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Forward");
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

    private void StartJump()
    {
        isJumping = true;
        jumpTimeCounter = maxJumpTime;
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private void ContinueJump()
    {
        if (jumpTimeCounter > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            isJumping = false;
        }
    }

    private void EndJump()
    {
        isJumping = false;
    }

    private void Move()
    {
        moveDirection = Vector3.zero;

        switch (currentDir)
        {
            case Direction.FORWARD:
                moveDirection = Vector3.forward;
                break;
            case Direction.BACKWARD:
                moveDirection = Vector3.back;
                break;
            case Direction.LEFT:
                moveDirection = Vector3.left;
                break;
            case Direction.RIGHT:
                moveDirection = Vector3.right;
                break;
        }

        Vector3 movement = moveDirection * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
}
