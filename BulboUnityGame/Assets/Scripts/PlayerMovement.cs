using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Collider2D slideSmoothCollider;
    public PhysicsMaterial2D smoothMaterial;
    public PhysicsMaterial2D defaultMaterial;

    // Animations
    public Animator animator;

    // Moving
    float horizontalMove = 0f;

    // Goes faster than the default run for sliding then decreases at a rate
    public float reduceSlideScale = 2.75f;
    public float resetSlideScale = 2.75f;

    // Physics
    public float runSpeed = 40f;
    public float gravity = 3.0f;

    // For jumping
    bool jump = false;
    bool jumping = false;
    float jumpStart = 0.0f;

     // For sliding mechanic
    bool slide = false;

    // For jumping/sliding control due to human error
    public float maxJumpError = 0.01f;
    public float maxSlideError = 0.01f;

    // Wall jumping
    const float k_CeilingRadius = .2f;
    public LayerMask m_WhatIsGround;
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    int wallJumpCount = 0;
    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
     
        // Control Slide up and down
        if (Input.GetButtonDown("Slide") && controller.isGrounded() && horizontalMove != 0 && !jump)    
        {
            slide = true;
        }
        else if (Input.GetButtonUp("Slide"))
        {
            slide = false;
        }

        // Control jump
        if (Input.GetButtonDown("Jump"))
        {
            // Regular jumping
            if (controller.isGrounded())
            {
                jump = true;
                jumping = true;
                jumpStart = Time.time;

                slide = false;
            }
            // Wall jumping Right
            else if (Physics2D.OverlapCircle(wallCheckRight.position, k_CeilingRadius, m_WhatIsGround) && !controller.isGrounded())
            {
                if (wallJumpCount < 1)
                {
                    jump = true;
                    jumping = true;
                    jumpStart = Time.time;
                    wallJumpCount++;

                    controller.isWallJump();

                    slide = false;
                }
                
            }
            // Wall jumping Left
            else if (Physics2D.OverlapCircle(wallCheckLeft.position, k_CeilingRadius, m_WhatIsGround) && !controller.isGrounded())
            {
                if (wallJumpCount < 1)
                {
                    jump = true;
                    jumping = true;
                    jumpStart = Time.time;
                    wallJumpCount++;

                    controller.isWallJump();

                    slide = false;
                }
            }

        }
        // Reset ability to wall jump once on ground
        if (controller.isGrounded())
        {
            wallJumpCount = 0;
        }

        // Plays part in disabling the character from skiding on slopes
        if (Time.time - jumpStart > 0.1f)
        {
            jumping = false;
        }

        // Change if jumping or sliding based on time since the press of the button


        // Animator State machine parameters
        animator.SetBool("isSlide", controller.isSliding());  // Needs to check if it can stand 
        animator.SetBool("isJump", jump);
        animator.SetBool("isRun", horizontalMove != 0);
        animator.SetBool("isFall", (controller.isGrounded() == false && GetComponent<Rigidbody2D>().velocity.y < 0));
        animator.SetBool("onGround", controller.isGrounded());
    }

    private void FixedUpdate()
    {
        // Change to a smooth maerial when sliding
        if (slide == true)
        {
            slideSmoothCollider.sharedMaterial = smoothMaterial;
            GetComponent<Rigidbody2D>().mass = 1.0f;
            reduceSlideScale = controller.reduceSlide(reduceSlideScale);
        }
        else
        {
            slideSmoothCollider.sharedMaterial = defaultMaterial;
            GetComponent<Rigidbody2D>().mass = 1.0f;
            reduceSlideScale = controller.resetReduceSlide(resetSlideScale); // change to match
        }

        // Disable skiding on slopes
        if (controller.isGrounded() && horizontalMove == 0f && jumping == false && GetComponent<Rigidbody2D>().velocity.y > -1.0f )
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
            GetComponent<Rigidbody2D>().gravityScale = gravity;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        // Move the charcter
        controller.Move(horizontalMove * Time.fixedDeltaTime, slide , jump);

        jump = false;

    }
}
