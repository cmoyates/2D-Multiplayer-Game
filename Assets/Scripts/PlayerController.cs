using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 movement = Vector3.zero;
    Rigidbody2D rb;
    public float moveSpeed = 1.0f;
    public float dashMultiplier = 2.0f;
    SpriteRenderer sr;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // If trying to move in the direction the sprite is not facing
        if (movement.x != 0 && (movement.x < 0) != sr.flipX)
        {
            // Flip the sprite
            sr.flipX = !sr.flipX;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the movement direction as a Vector2
        movement = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Update the animation speed
        anim.SetFloat("Speed", movement.magnitude);
        // Move the player according to the movement vector, the players speed, and whether or not the player was dashing
        rb.MovePosition(rb.position + movement * (moveSpeed * dashMultiplier) * Time.fixedDeltaTime);
    }
}
