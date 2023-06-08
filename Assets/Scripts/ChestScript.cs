using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : Interactable
{
    [SerializeField]
    Animator anim;
    [SerializeField]
    GameObject[] lootPool;

    private void Start()
    {
        base.oneTimeUse = true;
    }

    protected override void Interaction()
    {
        anim.SetTrigger("Open");
        Instantiate(lootPool[Random.Range(0, lootPool.Length)], transform.position, Quaternion.identity);
    }
}
