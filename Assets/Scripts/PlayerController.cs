using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    Material mat;

    int health = 3;
    public int maxHealth = 3;
    bool invincible = false;
    public float screenShakeDuration = 0.05f;
    public float screenShakeMagnitude = 0.05f;
    TMP_Text healthText;

    public float moveSpeed = 1.0f;
    Vector2 movement = Vector3.zero;

    public float dashMultiplier = 1.0f;
    float dashCooldown = 0.0f;
    public float dashCooldownScale = 1.0f;
    bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        mat = sr.material;

        healthText = GameObject.Find("Health Text").GetComponent<TMP_Text>();
        healthText.text = "Health: " + health.ToString() + "/" + maxHealth.ToString();
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

        // Update the dashes cooldown timer and the players color
        if (dashCooldown >= 0)
        {
            dashCooldown -= Time.deltaTime * dashCooldownScale;
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

    public void TriggerDash(InputAction.CallbackContext context)
    {
        // If the player presses shift and the dash is not on cooldown, start dashing
        if (context.performed && dashCooldown <= 0.0f && !isDashing)
        {
            StartCoroutine("Dash");
        }
    }

    public void Hurt()
    {
        health--;
        healthText.text = "Health: " + health.ToString() + "/" + maxHealth.ToString();
        StartCoroutine(health <= 0 ? "Die" : "TakeDamage");
    }

    public IEnumerator TakeDamage()
    {
        invincible = true;
        ScreenShake.Instance.Shake(screenShakeDuration, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        mat.SetInt("_Hurt", 0);
        mat.SetFloat("_Opacity", 0.3f);
        yield return new WaitForSecondsRealtime(0.5f);
        mat.SetFloat("_Opacity", 1.0f);
        invincible = false;
    }

    IEnumerator Die()
    {
        ScreenShake.Instance.Shake(screenShakeDuration * 3, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        rb.velocity = Vector2.zero;
        anim.speed = 0;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        Time.timeScale = 1;
        //Instantiate(explosionPrefab, transform.position, Quaternion.Euler(90, 0, 0));

        Destroy(gameObject);
        yield return null;
    }

    public IEnumerator Dash()
    {
        // Shake the screen
        ScreenShake.Instance.Shake(0.1f, 0.1f);
        // Make the player invincible and "dashing"
        invincible = true;
        mat.SetFloat("_Opacity", 0.3f);
        isDashing = true;
        dashMultiplier = 2;
        dashCooldown = 0.7f;
        // Wait for a quarter of a second
        yield return new WaitForSeconds(0.25f);
        // Remove the invincibility and make the player no longer dashing
        isDashing = false;
        dashMultiplier = 1;
        invincible = false;
        mat.SetFloat("_Opacity", 1.0f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !invincible)
        {
            Hurt();
        }
    }
}
