using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public event EventHandler EnemiesRemainingChanged;

    public GameObject[] enemies;
    public float timeBetweenSpawns = 5.0f;
    float timeUntilNextSpawn = 0f;
    MapGeneratorSimple mapGen;
    Vector3 spawnPosFix = new Vector3(0.5f, 0.5f, 0);
    Transform playerTransform;
    public float playerSafeRadius = 3.0f;
    int currentRoundSpawnCount = 0;
    int currentRoundSpawnTotal = 0;
    int currentRoundEnemiesKilled = 0;

    [System.Serializable]
    class RoundSpawnDistribution 
    {
        [SerializeField]
        public GameObject[] distribution;
    }

    [SerializeField]
    RoundSpawnDistribution[] roundSpawnDistributions;

    [SerializeField]
    int[] roundSpawnCounts;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //GameManager.Instance.OnRoundChanged += GameManager_OnRoundChanged;

        currentRoundSpawnTotal = roundSpawnCounts[0];

        mapGen = GetComponent<MapGeneratorSimple>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void GameManager_OnRoundChanged(object sender, System.EventArgs e)
    {
        //currentRoundSpawnTotal = roundSpawnCounts[GameManager.Instance.GetRound() - 1];
        currentRoundSpawnCount = 0;
        currentRoundEnemiesKilled = 0;
        EnemiesRemainingChanged.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying() || currentRoundSpawnCount == currentRoundSpawnTotal) return;

        timeUntilNextSpawn -= Time.deltaTime;

        if (timeUntilNextSpawn <= 0) 
        {
            bool validSpawnPosFound = false;
            Vector3 spawnPos = Vector3.zero;

            while (!validSpawnPosFound) 
            {
                spawnPos = (Vector3Int)mapGen.GetRandomValidPos() + spawnPosFix;
                validSpawnPosFound = Vector3.Distance(playerTransform.position, spawnPos) >= playerSafeRadius;
            }

            //Instantiate(roundSpawnDistributions[GameManager.Instance.GetRound() - 1].distribution[UnityEngine.Random.Range(0, roundSpawnDistributions[GameManager.Instance.GetRound()-1].distribution.Length)], spawnPos, Quaternion.identity);
            //timeBetweenSpawns *= 0.99f;
            timeUntilNextSpawn = timeBetweenSpawns;
            currentRoundSpawnCount++;
        }
    }

    public void EnemyKilled() 
    {
        currentRoundEnemiesKilled++;
        EnemiesRemainingChanged.Invoke(this, EventArgs.Empty);
        if (currentRoundEnemiesKilled >= currentRoundSpawnTotal) 
        {
            //GameManager.Instance.NextRound();
        }
    }

    public int GetEnemiesRemaining() 
    {
        return currentRoundSpawnTotal - currentRoundEnemiesKilled;
    }
}
