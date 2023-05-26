using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnLoop");
    }

    IEnumerator SpawnLoop() 
    {
        while (true) 
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
