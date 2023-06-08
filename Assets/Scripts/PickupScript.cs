using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : Interactable
{
    [SerializeField]
    protected PickupSO pickupSO;

    protected void Start()
    {
        base.oneTimeUse = true;
    }

    protected override void Interaction()
    {
        pickupSO.Activate(transform.position);
        Destroy(gameObject);
    }
}
