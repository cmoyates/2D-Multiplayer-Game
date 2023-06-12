using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool oneTimeUse;
    protected bool used = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryInteract(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryInteract(collision);
    }

    void TryInteract(Collider2D col) 
    {
        if (!(oneTimeUse && used) && col.CompareTag("Player"))
        {
            used = true;
            Interaction();
        }
    }

    protected abstract void Interaction();
}
