using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickups/Coin")]
public class CoinSO : PickupSO
{
    public int coinScore = 50;
    public override void Activate(Vector3 pos) 
    {
        PlayerManager.Instance.AddScore(coinScore);
        SFXManager.Instance.PlayPickupCoinClip(pos, 0.2f);
    }
}
