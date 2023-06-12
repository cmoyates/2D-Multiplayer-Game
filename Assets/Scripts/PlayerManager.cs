using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public event EventHandler OnHealthChanged;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnUpgradeAdded;

    public int maxHealth = 3;
    [SerializeField]
    int health = 0;
    int score = 0;
    GameObject player;
    Rigidbody2D playerRB;
    PlayerController playerController;
    PlayerLookAt playerLookAt;
    SpriteRenderer playerSR;
    int primaryDamage = 1;
    [SerializeField]
    GameObject specialBulletPrefab;


    private void Awake()
    {
        Instance = this;

        health = maxHealth;
        score = 0;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();
        playerLookAt = player.GetComponentInChildren<PlayerLookAt>();
        playerSR = player.GetComponent<SpriteRenderer>();

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying()) 
        {
            playerRB.velocity = Vector2.zero;
            playerRB.simulated = true;
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddScore(int additionalScore)
    {
        score += additionalScore;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
    }



    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth() 
    {
        return maxHealth;
    }

    #region Health

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddHealth(int additionalHealth)
    {
        health += additionalHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveHealth(int removedHealth)
    {
        health -= removedHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddMaxHealth(int additionalHealth) 
    {
        maxHealth += additionalHealth;
        health = maxHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    public void SpawnPlayerAtPos(Vector3 pos) 
    {
        playerRB.simulated = false;
        player.transform.position = pos;
    }

    public void AddUpgrade(UpgradePickupSO newUpgrade) 
    {
        Debug.Log("Upgrade Collected: " + newUpgrade.name);
        OnUpgradeAdded.Invoke(newUpgrade, EventArgs.Empty);

        switch (newUpgrade.upgradeType)
        {
            case UpgradePickupSO.UpgradeType.Attack:
                primaryDamage++;
                break;
            case UpgradePickupSO.UpgradeType.Movement:
                playerController.moveSpeed *= 1.25f;
                break;
            case UpgradePickupSO.UpgradeType.Defence:
                AddMaxHealth(1);
                break;
            case UpgradePickupSO.UpgradeType.Special:
                primaryDamage += 5;
                playerSR.color = Color.red;
                playerLookAt.bulletForce = 120;
                playerLookAt.screenShakeMagnitude = 4;
                playerLookAt.bulletPrefab = specialBulletPrefab;
                playerLookAt.recoilMultiplier = 20;
                playerLookAt.kickbackMul = 4;
                playerController.moveSpeed *= 1.5f;
                playerController.dashCooldownScale *= 4;
                break;
            default:
                break;
        }
    }

    public int GetPrimaryDamage() 
    {
        return primaryDamage;
    }
}
