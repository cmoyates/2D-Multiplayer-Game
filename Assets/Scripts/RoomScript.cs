using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    int difficulty = 0;
    public bool isEndRoom = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 size = GetComponent<BoxCollider2D>().size + Vector2.one * 2;
        difficulty = Mathf.RoundToInt(Mathf.Sqrt(size.x * size.y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            Bounds roomBounds = LevelManager.Instance.LockRoom(gameObject);
            if (isEndRoom) 
            {
                GameManager.Instance.BossFight();
            }
            Vector2Int roomBoundsOrigin = new Vector2Int(Mathf.RoundToInt(roomBounds.min.x), Mathf.RoundToInt(roomBounds.min.y)) - Vector2Int.one;
            int sizeX = Mathf.RoundToInt(roomBounds.max.x) - Mathf.RoundToInt(roomBounds.min.x) + 2;
            int sizeY = Mathf.RoundToInt(roomBounds.max.y) - Mathf.RoundToInt(roomBounds.min.y) + 3;
            Vector2Int roomBoundsSize = new Vector2Int(sizeX, sizeY);
            EnemyPathfinder.Instance.Initialize(roomBoundsOrigin, roomBoundsSize);
            EnemyManager.Instance.SpawnEnemiesInBounds(roomBounds, difficulty);
            Debug.Log("Difficulty: " + difficulty);
        }
    }
}
