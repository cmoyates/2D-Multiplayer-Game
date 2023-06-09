using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookAt : MonoBehaviour
{
    [SerializeField]
    Vector2 aim;
    public bool isKeyboard = true;
    public GameObject bulletPrefab;
    public float bulletForce = 1.0f;
    public float screenShakeDuration = 1.0f;
    public float screenShakeMagnitude = 1.0f;
    PlayerController playerController;
    [SerializeField]
    Rigidbody2D playerRB;
    public float recoilMultiplier = 5.0f;
    SpriteRenderer playerSR;
    Animator playerAnim;
    bool playerAnimForward = true;
    public float kickbackMul =  1.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        playerRB = GetComponentInParent<Rigidbody2D>();
        playerSR = GetComponentInParent<SpriteRenderer>();
        playerAnim = GetComponentInParent<Animator>();

        // The player starts the game aiming down
        aim = Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the weapon look in the direction
        transform.right = aim;

        // If trying to aim in the direction the sprite is not facing
        if (aim.x != 0 && (aim.x < 0) != playerSR.flipX)
        {
            // Flip the sprite
            playerSR.flipX = !playerSR.flipX;
            
        }
        if (aim.x != 0 && (aim.x > 0 == playerController.GetMovement().x > 0) != playerAnimForward) 
        {
            playerAnimForward = !playerAnimForward;
            playerAnim.SetBool("Forward", playerAnimForward);
        }
    }

    public void SetAim(InputAction.CallbackContext context)
    {
        // If it's not trying to set the aim to (0, 0)
        if (context.ReadValue<Vector2>() != Vector2.zero)
        {
            // Set the aim to the value from the input system
            aim = context.ReadValue<Vector2>();
            // If the player is using a keyboard
            if (isKeyboard)
            {
                // Get the vector from the mouse position to the position of the player
                aim = Camera.main.ScreenToWorldPoint(aim) - transform.position;
                aim.Normalize();
            }
        }
    }

    public void OnControlsChanged(PlayerInput playerInput)
    {
        // Check if the player is using a keyboard
        isKeyboard = playerInput.currentControlScheme.Equals("Keyboard");
        if (isKeyboard)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Debug.Log("Now playing using: " + playerInput.currentControlScheme);
    }

    public void TriggerShoot(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePlaying() && !playerController.IsDashing() && context.performed)
        {
            // Shoot in that direction
            Shoot(aim);
        }
    }

    public void Shoot(Vector2 lookDir)
    {
        // Play the shooting sound and shake the screen
        SFXManager.Instance.PlayShootSFX(transform.position);
        //StartCoroutine(ss.Shake(shakeDuration, shakeMagnitude));
        CameraManager.Instance.ShakeScreen(screenShakeDuration, screenShakeMagnitude);

        // Spawn the bullet
        lookDir *= 0.5f;
        Vector3 spawnPos = new Vector3(lookDir.x + transform.position.x, lookDir.y + transform.position.y, 0);
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        // Add the appropriate force to the bullet
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(lookDir * bulletForce, ForceMode2D.Impulse);
        playerRB.AddForce(-lookDir * recoilMultiplier, ForceMode2D.Impulse);
        CameraManager.Instance.Kickback(lookDir * kickbackMul);
    }
}
