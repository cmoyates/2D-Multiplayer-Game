using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [System.Serializable]
    class DifficultySpawnDistribution 
    {
        [SerializeField]
        public GameObject[] distribution;
    }

    //[SerializeField]
    //DifficultySpawnDistribution[] difficultySpawnDistributions;

    [SerializeField]
    GameObject[] enemies;
    [SerializeField]
    int[] enemyCosts;
    Transform playerTransform;
    [SerializeField]
    float playerSafeRadius = 3.0f;
    [SerializeField]
    int currentEnemyCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void EnemyKilled() 
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0) 
        {
            EnemyPathfinder.Instance.DeactivatePathfinding();
            LevelManager.Instance.UnlockRoom();
        }
    }

    public void SpawnEnemiesInBounds(Bounds bounds, int difficulty) 
    {
        currentEnemyCount = 0;
        int remainingCost = difficulty;

        while (remainingCost > 0) 
        {
            bool validSpawnPosFound = false;
            Vector3 spawnPos = Vector3.zero;

            while (!validSpawnPosFound)
            {
                spawnPos.x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                spawnPos.y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
                validSpawnPosFound = Vector3.Distance(playerTransform.position, spawnPos) >= playerSafeRadius;
            }

            int enemyIndex = UnityEngine.Random.Range(0, enemies.Length);
            if (enemyCosts[enemyIndex] > remainingCost) 
            {
                remainingCost = 0;
                continue;
            }
            remainingCost -= enemyCosts[enemyIndex];
            Transform enemyTransform = Instantiate(enemies[enemyIndex], spawnPos, Quaternion.identity).transform;
            CameraManager.Instance.AddToTargetGroup(enemyTransform);
            currentEnemyCount++;
        }
    }
}
