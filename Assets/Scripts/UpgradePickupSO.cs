using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickups/Upgrade")]
public class UpgradePickupSO : PickupSO
{
    public enum UpgradeType 
    {
        Attack,
        Movement,
        Defence,
        Special
    }
    float[] upgradeTypeColors = {
        0.0f,  // Red
        0.66f, // Blue
        0.33f, // Green
        0.16f  // Yellow
    };

    public UpgradeType upgradeType;

    public override void Activate(Vector3 pos)
    {
        PlayerManager.Instance.AddUpgrade(this);
        SFXManager.Instance.PlayPickupUpgradeClip(pos, 0.75f); ;
    }

    public float GetColorHue() 
    {
        return upgradeTypeColors[(int)upgradeType];
    }
}
