using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : MonoBehaviour
{
    protected EnemyPathfinder pathfinder;
    public float moveSpeed;
    public float steeringScale;
    protected Rigidbody2D rb;
    protected Vector2Int gridOffset;
    SpriteRenderer sr;
    Animator anim;
    public int maxHealth = 3;
    protected int health = 3;
    public int damage = 1;
    Material mat;
    Collider2D col;
    public float screenShakeDuration = 0.05f;
    public float screenShakeMagnitude = 0.05f;
    public int score;
    protected Vector2 movement = Vector2.zero;
    public GameObject[] lootPool;

    // Start is called before the first frame update
    protected void Start()
    {
        pathfinder = GameObject.Find("Enemy Pathfinder").GetComponent<EnemyPathfinder>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        mat = sr.material;

        gridOffset = pathfinder.GetOffset();

        health = maxHealth;
    }

    protected void FixedUpdate() 
    {
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
        //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        PlayerManager.Instance.AddScore(score);
        EnemyManager.Instance.EnemyKilled();
        CameraManager.Instance.RemoveFromTargetGroup(transform);
        Instantiate(lootPool[Random.Range(0, lootPool.Length)], transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }

    protected Vector2 Pathfind() 
    {
        Vector2 pathfindDir = Vector2.zero;
        
        // Get the enemy's position in a way that the pathfinding system can understand
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        pos -= gridOffset;
        // Get the direction to the player from the pathfinding system
        BitArray directions = pathfinder.GetDirArray(pos.x, pos.y);
        // Turn the array of bools into a Vec2 representing a direction ([0] = Up, [1] = Down, [2] = Left, [3] = Right)
        if (directions[0]) { pathfindDir.y -= 1; }
        if (directions[1]) { pathfindDir.y += 1; }
        if (directions[2]) { pathfindDir.x -= 1; }
        if (directions[3]) { pathfindDir.x += 1; }
        // Normalize the vector so the enemies aren't faster diagonally and multiply by the movement speed
        return pathfindDir.normalized;
    }
}
