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

    // animation
    Animator animator;
    string currentState;

    void Start() {
        // set components
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    void Update() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() {
        // Prevents diagonal movement from being faster than regular movement
        if(inputHorizontal != 0 && inputVertical != 0) {
            inputHorizontal *= speedLimiter;
            inputVertical *= speedLimiter;
        }

        // Flips sprite for right/left movement change
        // if moving left and still facing right, flip
        // if moving right and not facing right, flip
        if (inputHorizontal < 0 && facingRight || (inputHorizontal > 0 && !facingRight)) {
            Flip();
        }

        // set velocity and MOVE!!!
        rb.velocity = new Vector2(inputHorizontal * speed, inputVertical * speed);
    }

    private void Flip() {
        if(facingRight) {
            sr.flipX = true;
        } else {
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
