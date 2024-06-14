using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void OnAnimatorMove()
    {
        if (playerController.isGrounded)
        {
            transform.position = animator.rootPosition;
        }
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
    }

    public void SetMoving(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }
}
