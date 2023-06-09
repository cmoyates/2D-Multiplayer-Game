using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State 
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    float countdownToStartTimer = 3.0f;


    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }



    public bool IsGamePlaying() 
    {
        return state == State.GamePlaying;
    }

    public bool IsGameOver() 
    {
        return state == State.GameOver;
    }

    public bool IsCountdownToStartActive() 
    {
        return state == State.CountdownToStart;
    }

    public bool IsWaitingToStart() 
    {
        return state == State.WaitingToStart;
    }

    public void GameOver() 
    {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetCountdownToStartTimer() 
    {
        return countdownToStartTimer;
    }

    public void StartCountdown() 
    {
        state = State.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void BossFight() 
    {
        SceneManager.LoadScene(2);
    }
}
