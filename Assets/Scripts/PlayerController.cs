using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    Material mat;

    bool invincible = false;

    public float screenShakeDuration = 0.05f;
    public float screenShakeMagnitude = 0.05f;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        
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
        if (!GameManager.Instance.IsGamePlaying()) return;

        // Update the animation speed
        anim.SetFloat("Speed", movement.magnitude);
        // Move the player according to the movement vector, the players speed, and whether or not the player was dashing
        rb.MovePosition(rb.position + movement * (moveSpeed * dashMultiplier) * Time.fixedDeltaTime);
    }

    public void TriggerDash(InputAction.CallbackContext context)
    {
        // If the player presses shift and the dash is not on cooldown, start dashing
        if (GameManager.Instance.IsGamePlaying() && context.performed && dashCooldown <= 0.0f && !isDashing)
        {
            StartCoroutine("Dash");
        }
    }

    public void Hurt(int damage)
    {
        if (invincible || damage == 0) return;
        SFXManager.Instance.PlayHurtSFX(transform.position);
        PlayerManager.Instance.RemoveHealth(damage);
        StartCoroutine(PlayerManager.Instance.GetHealth() <= 0 ? "Die" : "TakeDamage");
    }

    public IEnumerator TakeDamage()
    {
        invincible = true;
        CameraManager.Instance.ShakeScreen(screenShakeDuration, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        mat.SetInt("_Hurt", 0);
        mat.SetFloat("_Opacity", 0.3f);
        yield return new WaitForSecondsRealtime(1.0f);
        mat.SetFloat("_Opacity", 1.0f);
        invincible = false;
    }

    IEnumerator Die()
    {
        CameraManager.Instance.ShakeScreen(screenShakeDuration * 3, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        rb.velocity = Vector2.zero;
        anim.speed = 0;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        Time.timeScale = 1;
        //Instantiate(explosionPrefab, transform.position, Quaternion.Euler(90, 0, 0));
        GameManager.Instance.GameOver();
        Destroy(gameObject);
        yield return null;
    }

    public IEnumerator Dash()
    {
        // Shake the screen
        CameraManager.Instance.ShakeScreen(0.1f, 0.1f);
        SFXManager.Instance.PlayDashSFX(transform.position);
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
        if (collision.collider.CompareTag("Enemy"))
        {
            Hurt(collision.gameObject.GetComponent<AIBase>().damage);
        }
    }
}
