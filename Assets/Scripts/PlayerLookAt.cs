using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookAt : MonoBehaviour
{
    Vector2 aim;
    public bool isKeyboard = true;
    public GameObject bulletPrefab;
    public float bulletForce = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // The player starts the game aiming down
        aim = Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the weapon look in the direction
        transform.right = aim;
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
        Debug.Log("Now playing using: " + playerInput.currentControlScheme);
    }

    public void TriggerShoot(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePlaying() && context.performed)
        {
            // Shoot in that direction
            Shoot(aim);
        }
    }

    public void Shoot(Vector2 lookDir)
    {
        // Play the shooting sound and shake the screen
        //audioSource.PlayOneShot(audioSource.clip);
        //StartCoroutine(ss.Shake(shakeDuration, shakeMagnitude));

        // Spawn the bullet
        lookDir *= 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(lookDir.x + transform.position.x, lookDir.y + transform.position.y, 0), Quaternion.identity);
        // Add the appropriate force to the bullet
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(lookDir * bulletForce, ForceMode2D.Impulse);
    }
}
