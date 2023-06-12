using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerAI : AIBase
{
    private enum State
    {
        Invisible,
        Attacking
    }

    [SerializeField]
    float timeBetweenTeleports = 1.0f;
    float teleportTimer = 0;
    [SerializeField]
    float invisDuration = 1.0f;
    float invisTimer = 0;
    [SerializeField]
    float waitDuration = 0;
    [SerializeField]
    float timeBetweenShots = 1.0f;
    float shotTimer = 0;
    [SerializeField]
    State state = State.Invisible;
    Transform playerTransform;
    [SerializeField]
    GameObject enemyBulletPrefab;
    [SerializeField]
    float bulletForceMul = 1.0f;
    [SerializeField]
    LayerMask lineOfSightLayerMask;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    new private void FixedUpdate()
    {
        if (health <= 0 || !GameManager.Instance.IsGamePlaying()) return;

        switch (state)
        {
            case State.Invisible:
                invisTimer += Time.fixedDeltaTime;
                if (invisTimer >= invisDuration) 
                {
                    transform.position = (Vector3Int)LevelManager.Instance.GetRandomPosInRoom();
                    invisTimer = 0;
                    state = State.Attacking;
                    sr.enabled = true;
                    col.enabled = true;
                    rb.velocity = Vector2Int.zero;
                    SFXManager.Instance.PlayTeleportClip(transform.position);
                }
                break;
            case State.Attacking:
                teleportTimer += Time.fixedDeltaTime;
                if (teleportTimer >= waitDuration && teleportTimer < timeBetweenTeleports + waitDuration) 
                {
                    shotTimer += Time.fixedDeltaTime;
                    if (shotTimer >= timeBetweenShots) 
                    {
                        Attack();
                        shotTimer = 0;
                    }
                }
                else if (teleportTimer >= timeBetweenTeleports + (waitDuration * 2))
                {
                    teleportTimer = 0;
                    state = State.Invisible;
                    sr.enabled = false;
                    col.enabled = false;
                    rb.velocity = Vector2Int.zero;
                    SFXManager.Instance.PlayTeleportClip(transform.position);
                }
                break;
            default:
                break;
        }

        if (playerTransform.position.x < transform.position.x != sr.flipX) 
        {
            sr.flipX = !sr.flipX;
        }
    }

    void Attack() 
    {
        var hit = Physics2D.CircleCast(transform.position, 1.2f, (playerTransform.position - transform.position), Mathf.Infinity, lineOfSightLayerMask);
        if (!hit.collider.CompareTag("Player")) return;

        Rigidbody2D bulletRB = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        bulletRB.AddForce((playerTransform.position - transform.position).normalized * bulletForceMul, ForceMode2D.Impulse);
    }
}
