using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool oneTimeUse;
    protected bool used = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!(oneTimeUse && used) && collision.collider.CompareTag("Player")) 
        {
            used = true;
            Interaction();
        }
    }

    protected abstract void Interaction();
}
