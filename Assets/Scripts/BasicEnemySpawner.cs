using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public float timeBetweenSpawns = 5.0f;
    float timeUntilNextSpawn = 0f;
    MapGeneratorSimple mapGen;
    Vector3 spawnPosFix = new Vector3(0.5f, 0.5f, 0);
    Transform playerTransform;
    public float playerSafeRadius = 3.0f;

    private void Start()
    {
        mapGen = GetComponent<MapGeneratorSimple>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

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

            Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos, Quaternion.identity);
            timeBetweenSpawns *= 0.99f;
            timeUntilNextSpawn = timeBetweenSpawns;
        }
    }
}
