using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuddyAI : AIBase
{
    [SerializeField]
    float timeBetweenAttacks = 1.0f;
    float attackTimer = 0;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    float bulletForce = 1.0f;
    Transform playerTransform;
    [SerializeField]
    LayerMask lineOfSightLayerMask;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= timeBetweenAttacks) 
        {
            attackTimer = 0;
            anim.SetTrigger("Attack");
        }

        // Turn to face player
        if (playerTransform.position.x < transform.position.x != sr.flipX)
        {
            sr.flipX = !sr.flipX;
        }
    }

    public void Attack() 
    {
        var hit = Physics2D.CircleCast(transform.position, 1.2f, (playerTransform.position - transform.position), Mathf.Infinity, lineOfSightLayerMask);
        if (!hit.collider.CompareTag("Player")) return;

        Rigidbody2D bulletRB = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletRB.velocity = (playerTransform.position - transform.position).normalized * bulletForce;
    }
}
