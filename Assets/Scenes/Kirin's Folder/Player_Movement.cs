// Kirin Hardinger
// September 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    float speed = 5f;
    float speedLimiter = 0.7f;
    float inputHorizontal, inputVertical;
    bool facingRight = true;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
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
}
