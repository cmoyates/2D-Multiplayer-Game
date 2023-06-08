using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickups/Health")]
public class HealthPickupSO : PickupSO
{
    public int healthToRestore = 1;

    public override void Activate(Vector3 pos)
    {
        if (PlayerManager.Instance.GetMaxHealth() - PlayerManager.Instance.GetHealth() >= healthToRestore) 
        {
            PlayerManager.Instance.AddHealth(healthToRestore);
        }
        SFXManager.Instance.PlayPickupHealthClip(pos, 0.2f);
    }
}
