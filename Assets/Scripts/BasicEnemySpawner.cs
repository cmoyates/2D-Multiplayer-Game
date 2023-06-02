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

    private void Start()
    {
        mapGen = GetComponent<MapGeneratorSimple>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        timeUntilNextSpawn -= Time.deltaTime;

        if (timeUntilNextSpawn <= 0) 
        {
            Vector3Int spawnPos = (Vector3Int)mapGen.GetRandomValidPos();
            Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos + spawnPosFix, Quaternion.identity);
            timeBetweenSpawns *= 0.99f;
            timeUntilNextSpawn = timeBetweenSpawns;
        }
    }
}
