using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    Material mat;

    bool invincible = false;

    public float screenShakeDuration = 0.05f;
    public float screenShakeMagnitude = 0.05f;

    public float moveSpeed = 1.0f;
    Vector2 movement = Vector3.zero;

    public float dashMultiplier = 1.0f;
    float dashCooldown = 0.0f;
    public float dashCooldownScale = 1.0f;
    bool isDashing = false;

    Vector2Int gridOffset;
    NetworkVariable<Vector2Int> gridPos = new NetworkVariable<Vector2Int>(Vector2Int.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        mat = sr.material;
    }

    public override void OnNetworkSpawn() 
    {
        // On the owner
        if (IsOwner) 
        {
            // Get the collider of the spawn area
            BoxCollider2D spawnAreaCollider = GameObject.FindGameObjectWithTag("SpawnArea").GetComponent<BoxCollider2D>();
            // Get a random position in that collider
            Vector3 spawnPos = Vector3.zero;
            spawnPos.x = spawnAreaCollider.gameObject.transform.position.x + spawnAreaCollider.offset.x + Random.Range(0.5f, spawnAreaCollider.size.x - 0.5f) - spawnAreaCollider.size.x / 2;
            spawnPos.y = spawnAreaCollider.gameObject.transform.position.y + spawnAreaCollider.offset.y + Random.Range(0.5f, spawnAreaCollider.size.y - 0.5f) - spawnAreaCollider.size.y / 2;
            // Move the player to that position
            transform.position = spawnPos;

            // Get the current grid offset
            gridOffset = EnemyPathfinder.Instance.GetOffset();
            // Get the initial pos on the grid
            UpdateGridPos();

            // Enable the player input component
            GetComponent<PlayerInput>().enabled = true;
            // Make the camera follow the player
            var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying() || !IsOwner) return;
        
        // If trying to move in the direction the sprite is not facing
        if (movement.x != 0 && (movement.x < 0) != sr.flipX)
        {
            // Flip the sprite on the local client
            sr.flipX = !sr.flipX;
            // Tell the server about it
            SetSpriteDirServerRpc(sr.flipX, NetworkObjectId);
        }

        // Update the dashes cooldown timer and the players color
        if (dashCooldown >= 0)
        {
            dashCooldown -= Time.deltaTime * dashCooldownScale;
        }

        // Update the gridPos
        UpdateGridPos();
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Get the movement direction as a Vector2
        movement = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePlaying() || !IsOwner) return;

        // Update the animation speed
        anim.SetFloat("Speed", movement.magnitude);
        // Move the player according to the movement vector, the players speed, and whether or not the player was dashing
        rb.MovePosition(rb.position + movement * (moveSpeed * dashMultiplier) * Time.fixedDeltaTime);
    }

    public void TriggerDash(InputAction.CallbackContext context)
    {
        // If the player presses shift and the dash is not on cooldown, start dashing
        if (context.performed && dashCooldown <= 0.0f && !isDashing && movement.sqrMagnitude > 0)
        {
            StartCoroutine("Dash");
        }
    }

    public void Hurt(int damage)
    {
        if (invincible || damage == 0) return;
        PlayerManager.Instance.RemoveHealth(damage);
        StartCoroutine(PlayerManager.Instance.GetHealth() <= 0 ? "Die" : "TakeDamage");
    }

    public IEnumerator TakeDamage()
    {
        invincible = true;
        ScreenShake.Instance.Shake(screenShakeDuration, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        mat.SetInt("_Hurt", 0);
        mat.SetFloat("_Opacity", 0.3f);
        SetShaderOpacityServerRpc(0.3f, NetworkObjectId);
        yield return new WaitForSecondsRealtime(0.5f);
        mat.SetFloat("_Opacity", 1.0f);
        SetShaderOpacityServerRpc(1.0f, NetworkObjectId);
        invincible = false;
    }

    IEnumerator Die()
    {
        ScreenShake.Instance.Shake(screenShakeDuration * 3, screenShakeMagnitude);
        mat.SetInt("_Hurt", 1);

        rb.velocity = Vector2.zero;
        if (IsOwner) 
        {
            anim.speed = 0;
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 3);
        Time.timeScale = 1;
        //Instantiate(explosionPrefab, transform.position, Quaternion.Euler(90, 0, 0));
        GameManager.Instance.GameOver();
        Destroy(gameObject);
        yield return null;
    }

    public IEnumerator Dash()
    {
        // Shake the screen
        ScreenShake.Instance.Shake(0.1f, 0.1f);
        // Make the player invincible and "dashing"
        invincible = true;
        mat.SetFloat("_Opacity", 0.3f);
        SetShaderOpacityServerRpc(0.3f, NetworkObjectId);
        isDashing = true;
        dashMultiplier = 2;
        dashCooldown = 0.7f;
        // Wait for a quarter of a second
        yield return new WaitForSeconds(0.25f);
        // Remove the invincibility and make the player no longer dashing
        isDashing = false;
        dashMultiplier = 1;
        invincible = false;
        mat.SetFloat("_Opacity", 1.0f);
        SetShaderOpacityServerRpc(1.0f, NetworkObjectId);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Hurt(collision.gameObject.GetComponent<EnemyAI>().damage);
        }
    }

    void UpdateGridPos() 
    {
        int gridX = Mathf.FloorToInt(transform.position.x) - gridOffset.x;
        int gridY = Mathf.FloorToInt(transform.position.y) - gridOffset.y;
        gridPos.Value = new Vector2Int(gridX, gridY);
    }

    public Vector2Int GetGridPos() 
    {
        return gridPos.Value;
    }

    #region Sprite Dir RPCs

    [ServerRpc]
    void SetSpriteDirServerRpc(bool flipped, ulong objectId) 
    {
        // Tell the all of the clients that a player sprite has changed direction
        SetSpriteDirClientRpc(flipped, objectId);
    }

    [ClientRpc]
    void SetSpriteDirClientRpc(bool flipped, ulong objectId) 
    {
        // If the player sprite that changed direction wasn't the local one
        if (NetworkObjectId == objectId && !IsOwner)
        {
            // Update it's direction
            sr.flipX = flipped;
        }
    }

    #endregion

    #region Shader Hurt RPCs

    /*[ServerRpc]
    void SetShaderHurtServerRpc(int hurt, ulong objectId)
    {
        // Tell the all of the clients that a player has been hurt
        SetShaderHurtClientRpc(hurt, objectId);
    }

    [ClientRpc]
    void SetShaderHurtClientRpc(int hurt, ulong objectId)
    {
        // If the player that was hurt wasn't the local one
        if (NetworkObjectId == objectId && !IsOwner)
        {
            // Update it's shader
            mat.SetInt("_Hurt", hurt);
        }
    }*/

    #endregion

    #region Shader Hurt RPCs

    [ServerRpc]
    void SetShaderOpacityServerRpc(float opacity, ulong objectId)
    {
        // Tell the all of the clients that a player has been hurt
        SetShaderOpacityClientRpc(opacity, objectId);
    }

    [ClientRpc]
    void SetShaderOpacityClientRpc(float opacity, ulong objectId)
    {
        // If the player that was hurt wasn't the local one
        if (NetworkObjectId == objectId && !IsOwner)
        {
            // Update it's shader
            mat.SetFloat("_Opacity", opacity);
        }
    }

    #endregion
}
