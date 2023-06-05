using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public event EventHandler OnHealthChanged;
    public event EventHandler OnScoreChanged;

    public int maxHealth = 3;
    int health = 0;
    int score = 0;
    GameObject player;
    Rigidbody2D playerRB;

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

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying()) 
        {
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

    public void SpawnPlayerAtPos(Vector3 pos) 
    {
        playerRB.simulated = false;
        player.transform.position = pos;
    }
}
