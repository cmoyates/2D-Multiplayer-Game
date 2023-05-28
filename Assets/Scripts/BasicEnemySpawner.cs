using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public float timeBetweenSpawns = 5.0f;
    float timeUntilNextSpawn = 0f;
    MapGeneratorSimple mapGen;
    Vector3 spawnPosFix = new Vector3(0.5f, 0.5f, 0);

    private void Start()
    {
        mapGen = GetComponent<MapGeneratorSimple>();
    }

    IEnumerator SpawnLoop() 
    {
        while (true) 
        {
            
            yield return new WaitForSeconds(5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilNextSpawn -= Time.deltaTime;

        if (timeUntilNextSpawn <= 0) 
        {
            Vector3Int spawnPos = (Vector3Int)mapGen.GetRandomValidPos();
            Instantiate(enemy, spawnPos + spawnPosFix, Quaternion.identity);
            timeUntilNextSpawn = timeBetweenSpawns;
        }
    }
}
