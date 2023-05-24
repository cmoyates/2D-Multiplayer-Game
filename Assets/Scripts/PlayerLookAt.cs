using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookAt : MonoBehaviour
{
    Vector2 aim;
    public bool isKeyboard = true;

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
}
