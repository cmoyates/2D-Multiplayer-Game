using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    EnemyPathfinder pathfinder;
    public float moveSpeed;
    public float steeringScale;
    Rigidbody2D rb;
    Vector2Int gridOffset;
    SpriteRenderer sr;
    Animator anim;
    public int health = 3;

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.Find("Enemy Pathfinder").GetComponent<EnemyPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        gridOffset = new Vector2Int(pathfinder.floor.origin.x, pathfinder.floor.origin.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (health <= 0) return;

        // Get the enemy's position in a way that the pathfinding system can understand
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        pos -= gridOffset;
        // Get the direction to the player from the pathfinding system
        BitArray directions = pathfinder.m_directions[pos.x][pos.y];
        // Turn the array of bools into a Vec2 representing a direction ([0] = Up, [1] = Down, [2] = Left, [3] = Right)
        Vector2 movement = new Vector2Int(0, 0);
        if (directions[0]) { movement.y -= 1; }
        if (directions[1]) { movement.y += 1; }
        if (directions[2]) { movement.x -= 1; }
        if (directions[3]) { movement.x += 1; }
        // Normalize the vector so the enemies aren't faster diagonally and multiply by the movement speed
        movement.Normalize();
        movement *= moveSpeed;
        // Steering behavior so the player can out-maneuver them
        Vector2 steering = movement - rb.velocity;
        steering *= steeringScale;
        rb.velocity = (rb.velocity + steering);

        // If trying to move in the direction the sprite is not facing
        if (movement.x != 0 && (movement.x < 0) != sr.flipX)
        {
            // Flip the sprite
            sr.flipX = !sr.flipX;
        }

        // Update the animation speed
        anim.SetFloat("Speed", rb.velocity.magnitude);
    }
}
