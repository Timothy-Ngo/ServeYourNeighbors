// Kirin Hardinger
// September 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    // movement
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float speed = 7f;
    float speedLimiter = 0.7f;
    float inputHorizontal, inputVertical;
    bool facingRight = true;
    bool freeze = false;

    // animation
    Animator animator;
    string currentState;

    Animator outfitAnimator;

    void Start() {
        // set components
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        outfitAnimator = Player.inst.outfitObject.GetComponent<Animator>();
    }

    void Update() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }

    public void Freeze()
    {
        freeze = true;
    }

    public void Unfreeze()
    {
        freeze = false;
    }

    private void FixedUpdate() {
        if (!freeze)
        {
            
            // Prevents diagonal movement from being faster than regular movement
            if (inputHorizontal != 0 && inputVertical != 0)
            {
                inputHorizontal *= speedLimiter;
                inputVertical *= speedLimiter;
            }


            OnStateEnter(outfitAnimator, outfitAnimator.GetCurrentAnimatorStateInfo(0), 8);
            

            // Flips sprite for right/left movement change
            // if moving left and still facing right, flip
            // if moving right and not facing right, flip
            if (inputHorizontal < 0 && facingRight || (inputHorizontal > 0 && !facingRight))
            {
                Flip();
            }

            // set velocity and MOVE!!!
            rb.velocity = new Vector2(inputHorizontal * speed, inputVertical * speed);
        } else
        {
            
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void OnStateEnter(Animator animator, AnimatorStateInfo animatorInfo, int layermask)
    {
        if (inputHorizontal == 0 && inputVertical == 0)
        {
            ChangeAnimationState("Idle");
        }
        else
        {
            ChangeAnimationState("Walk");
        }
    }

    private void Flip() {
        if(facingRight) {
            Player.inst.outfitSR.flipX = true;
            sr.flipX = true;
        } else {
            Player.inst.outfitSR.flipX = false;
            sr.flipX = false;
        }
        facingRight = !facingRight;
    }

    private void ChangeAnimationState(string newState) {
        if (currentState == newState) {
            return;
        }

        //plays new state and sets current state to new
        animator.Play(newState);
        currentState = newState;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
