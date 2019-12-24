using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    float horizontalMove = 0f;

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
        Debug.Log(horizontalMove);
        if (Input.GetButtonDown("Slide") && controller.isGrounded())    
        {
            slide = true;
        }
        else if (Input.GetButtonUp("Slide"))
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
    }

    private void FixedUpdate()
    {
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
