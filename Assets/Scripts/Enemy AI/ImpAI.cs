using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpAI : AIBase
{
    new void Start()
    {
        base.Start();
    }

    new void FixedUpdate()
    {
        if (health <= 0 || !GameManager.Instance.IsGamePlaying()) return;

        movement = Pathfind() * moveSpeed;
        // Steering behavior so the player can out-maneuver them
        Vector2 steering = movement - rb.velocity;
        steering *= steeringScale;
        rb.velocity = (rb.velocity + steering);

        base.FixedUpdate();
    }
}
