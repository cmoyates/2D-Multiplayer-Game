using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinHeadAI : AIBase
{
    enum State 
    {
        Safe,
        Fleeing
    }

    [SerializeField]
    State state = State.Safe;
    Transform playerTransform;
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

        var hit = Physics2D.Raycast(transform.position, (playerTransform.position - transform.position), Mathf.Infinity, lineOfSightLayerMask);
        bool canSeePlayer = hit.collider.CompareTag("Player");

        switch (state)
        {
            case State.Safe:
                if (canSeePlayer) 
                {
                    state = State.Fleeing;
                }
                break;
            case State.Fleeing:
                movement = -Pathfind() * moveSpeed;
                // Steering behavior so the player can out-maneuver them
                Vector2 steering = movement - rb.velocity;
                steering *= steeringScale;
                rb.velocity = (rb.velocity + steering);
                if (!canSeePlayer)
                {
                    state = State.Safe;
                    rb.velocity = Vector2.zero;
                }
                break;
            default:
                break;
        }
    }
}
