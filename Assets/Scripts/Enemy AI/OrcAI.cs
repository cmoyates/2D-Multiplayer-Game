using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcAI : AIBase
{
    private enum State 
    {
        Hunting,
        Charging,
        Stunned
    }

    [SerializeField]
    private State state = State.Stunned;
    Transform playerTransform;
    [SerializeField]
    LayerMask lineOfSightLayerMask;
    Vector2 playerDir = Vector2.zero;
    [SerializeField]
    float stunDuration = 2.0f;
    float currentStunDuration = 0.0f;
    [SerializeField]
    float chargeSpeedMul = 3.0f;
    [SerializeField]
    float chargeBounceDrag = 3.0f;
    [SerializeField]
    float playerDistThreshold = 5.0f;
    [SerializeField]
    bool dumb = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    new void FixedUpdate() 
    {
        if (health <= 0 || !GameManager.Instance.IsGamePlaying()) return;

        switch (state)
        {
            case State.Hunting:
                movement = Pathfind() * moveSpeed;
                // Steering behavior so the player can out-maneuver them
                Vector2 steering = movement - rb.velocity;
                steering *= steeringScale;
                rb.velocity = (rb.velocity + steering);

                var hit = Physics2D.CircleCast(transform.position, 1.0f, (playerTransform.position - transform.position), Mathf.Infinity, lineOfSightLayerMask);
                if (hit.collider.CompareTag("Player") && (dumb || Vector2.Distance(transform.position, playerTransform.position) <= playerDistThreshold)) 
                {
                    SFXManager.Instance.PlayDashSFX(transform.position);
                    playerDir = (playerTransform.position - transform.position).normalized;
                    state = State.Charging;
                }

                break;
            case State.Charging:
                movement = playerDir * moveSpeed * chargeSpeedMul;
                rb.velocity = movement;

                if (!dumb && Vector2.Distance(transform.position, playerTransform.position) > playerDistThreshold) 
                {
                    state = State.Hunting;
                }

                break;
            case State.Stunned:
                movement = Vector2.zero;
                //rb.velocity = movement;

                currentStunDuration += Time.fixedDeltaTime;
                if (currentStunDuration >= stunDuration) 
                {
                    rb.drag = 0;
                    state = State.Hunting;
                }
                break;
            default:
                break;
        }

        base.FixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == State.Charging && collision.collider.CompareTag("LevelCollision")) 
        {
            state = State.Stunned;
            rb.drag = chargeBounceDrag;
            currentStunDuration = 0;
            Vector2 normal = collision.GetContact(0).normal;
            float dot = Vector2.Dot(collision.relativeVelocity, normal);
            Vector2 bounceVec = collision.relativeVelocity - (normal * dot * 2);
            rb.velocity = Vector2.zero;
            rb.AddForce(-bounceVec * moveSpeed * chargeSpeedMul * 0.2f, ForceMode2D.Impulse);
            
        }
    }
}
