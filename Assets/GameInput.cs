using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    /*public Vector2 GetMovementVector() 
    {
        
    }*/
}
