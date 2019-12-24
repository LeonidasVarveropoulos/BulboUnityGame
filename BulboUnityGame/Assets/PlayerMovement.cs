using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Collider2D slideSmoothCollider;
    public PhysicsMaterial2D smoothMaterial;
    public PhysicsMaterial2D defaultMaterial;

    public Animator animator;

    float horizontalMove = 0f;

    float m_CrouchSpeed = 2.75f; // Goes faster than the default run for sliding TWO PLACES TO CHANGE

    public float runSpeed = 40f;
    public float gravity = 3.0f;

    bool jump = false;
    bool jumping = false;
    float jumpStart = 0.0f;

    bool slide = false;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
     
        if (Input.GetButtonDown("Slide") && controller.isGrounded() && horizontalMove != 0)    
        {
            slide = true;
        }
        else if (Input.GetButtonUp("Slide") || !controller.isGrounded())
        {
            slide = false;
        }

        if (Input.GetButtonDown("Jump") && slide == false)
        {
            jump = true;
            jumping = true;
            jumpStart = Time.time;

        }

        // Plays part in disabling the character from sliding on slopes
        if (Time.time - jumpStart > 1.0f)
        {
            jumping = false;
        }

        // Animator State machine parameters
        animator.SetBool("isSlide", slide);
        animator.SetBool("isJump", jump);
        animator.SetBool("isRun", horizontalMove != 0);
        animator.SetBool("isFall", (controller.isGrounded() == false && GetComponent<Rigidbody2D>().velocity.y < 0));
        animator.SetBool("onGround", controller.isGrounded());
    }

    private void FixedUpdate()
    {
        // Change to a smooth maerial when sliding
        Debug.Log(slide);
        if (slide == true)
        {
            slideSmoothCollider.sharedMaterial = smoothMaterial;
            GetComponent<Rigidbody2D>().mass = .1f;
            m_CrouchSpeed = controller.reduceSlide(m_CrouchSpeed);
        }
        else
        {
            slideSmoothCollider.sharedMaterial = defaultMaterial;
            GetComponent<Rigidbody2D>().mass = 1.0f;
            m_CrouchSpeed = controller.resetReduceSlide(2.75f); // change to match
        }
        // Move the charcter

        // Disable sliding on slopes
        if (controller.isGrounded() && horizontalMove == 0f && jumping == false && GetComponent<Rigidbody2D>().velocity.y > -1.0f )
        {
            GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
            GetComponent<Rigidbody2D>().gravityScale = gravity;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        controller.Move(horizontalMove * Time.fixedDeltaTime, slide , jump);

        jump = false;

    }
}
