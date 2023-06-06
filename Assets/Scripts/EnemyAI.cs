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
    public int damage = 1;
    Material mat;
    Collider2D col;
    public float screenShakeDuration = 0.05f;
    public float screenShakeMagnitude = 0.05f;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.Find("Enemy Pathfinder").GetComponent<EnemyPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        mat = sr.material;

        gridOffset = new Vector2Int(pathfinder.walls.origin.x, pathfinder.walls.origin.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (health <= 0 || !GameManager.Instance.IsGamePlaying()) return;

        // Get the enemy's position in a way that the pathfinding system can understand
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        pos -= gridOffset;
        // Get the direction to the player from the pathfinding system
        BitArray directions = pathfinder.GetDirArray(pos.x, pos.y);
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

    public IEnumerator TakeDamage() 
    {
        CameraManager.Instance.ShakeScreen(screenShakeDuration, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        Time.timeScale = 1;
        mat.SetInt("_Hurt", 0);
    }

    public void Hurt() 
    {
        health--;
        SFXManager.Instance.PlayHurtSFX(transform.position);
        StartCoroutine(health <= 0 ? "Die" : "TakeDamage");
    }

    IEnumerator Die()
    {
        CameraManager.Instance.ShakeScreen(screenShakeDuration * 3, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        rb.velocity = Vector2.zero;
        anim.speed = 0;
        col.enabled = false;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        Time.timeScale = 1;
        //Instantiate(explosionPrefab, transform.position, Quaternion.Euler(90, 0, 0));
        PlayerManager.Instance.AddScore(score);
        CameraManager.Instance.RemoveFromTargetGroup(transform);
        Destroy(gameObject);
        yield return null;
    }
}
