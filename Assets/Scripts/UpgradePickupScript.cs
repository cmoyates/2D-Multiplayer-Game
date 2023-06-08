using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickupScript : PickupScript
{
    SpriteRenderer sr;
    Material mat;

    new void Start()
    {
        UpgradePickupSO upgradePickupSO = (UpgradePickupSO)pickupSO;
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
        mat.SetFloat("_Hue", upgradePickupSO.GetColorHue());
    }

    protected override void Interaction()
    {
        pickupSO.Activate(transform.position);
        Destroy(gameObject);
    }
}
