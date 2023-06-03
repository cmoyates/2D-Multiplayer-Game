using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnRoundChanged;

    private enum State 
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    float waitingToStartTimer = 0.0f;
    float countdownToStartTimer = 3.0f;

    [SerializeField]
    int round = 1;

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
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) 
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
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

    public void GameOver() 
    {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetCountdownToStartTimer() 
    {
        return countdownToStartTimer;
    }

    public int GetRound() 
    {
        return round;
    }

    public void NextRound() 
    {
        round++;
        OnRoundChanged?.Invoke(this, EventArgs.Empty);
    }
}
