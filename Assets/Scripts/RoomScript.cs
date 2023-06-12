using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public bool isEndRoom = false;
    public bool leafRoom = false;
    public int steps = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            int difficulty = CalculateDifficulty();
            Bounds roomBounds = LevelManager.Instance.LockRoom(gameObject, leafRoom);
            
            Vector2Int roomBoundsOrigin = new Vector2Int(Mathf.RoundToInt(roomBounds.min.x), Mathf.RoundToInt(roomBounds.min.y)) - Vector2Int.one;
            int sizeX = Mathf.RoundToInt(roomBounds.max.x) - Mathf.RoundToInt(roomBounds.min.x) + 2;
            int sizeY = Mathf.RoundToInt(roomBounds.max.y) - Mathf.RoundToInt(roomBounds.min.y) + 3;
            Vector2Int roomBoundsSize = new Vector2Int(sizeX, sizeY);
            EnemyPathfinder.Instance.Initialize(roomBoundsOrigin, roomBoundsSize);
            
            if (isEndRoom)
            {
                EnemyManager.Instance.SpawnBoss(roomBounds);
            }
            else 
            {
                EnemyManager.Instance.SpawnEnemiesInBounds(roomBounds, difficulty);
            }

            Debug.Log("Difficulty: " + difficulty);
        }
    }

    int CalculateDifficulty() 
    {
        Vector2 size = GetComponent<BoxCollider2D>().size + Vector2.one * 2;
        float percentageThroughLevel = 1.0f * steps / LevelManager.Instance.maxSteps;
        Vector2Int minMaxRoomSize = LevelManager.Instance.minMaxRoomSize;
        int roomSizeDiff = minMaxRoomSize.y - minMaxRoomSize.x;
        float sizePercentage = (1.0f * Mathf.RoundToInt(Mathf.Sqrt(size.x * size.y)) - minMaxRoomSize.x) / roomSizeDiff;

        return Mathf.Max(Mathf.RoundToInt(LevelManager.Instance.globalDifficultyScale * (percentageThroughLevel /*+ 0.05f * (sizePercentage - 0.5f)*/)), 1);
    }
}
