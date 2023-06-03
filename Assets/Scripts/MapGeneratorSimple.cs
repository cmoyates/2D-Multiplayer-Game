using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneratorSimple : MonoBehaviour
{
    public Tilemap debugMap;
    public Tilemap[] levelMaps;
    public RuleTile[] levelRuleTiles;
    Vector2Int offset;
    List<Vector2Int> validPosList;
    public bool justPathfinding = false;

    // Start is called before the first frame update
    void Awake()
    {
        HashSet<Vector2Int> inputTilePositions = new HashSet<Vector2Int>();

        Vector2Int mapSize = (Vector2Int)debugMap.size;
        offset = (Vector2Int)debugMap.origin;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(x + offset.x, y + offset.y, 0);
                if (debugMap.GetSprite(pos) != null)
                {
                    inputTilePositions.Add((Vector2Int)pos);
                }
            }
        }

        HashSet<Vector2Int> outputTilePositions = new HashSet<Vector2Int>();



        foreach (var tilePos in inputTilePositions) 
        {
            outputTilePositions.Add(tilePos);

            outputTilePositions.Add(tilePos + Vector2Int.left + Vector2Int.up + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.up + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.right + Vector2Int.up + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.left + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.right + Vector2Int.up);
            outputTilePositions.Add(tilePos + Vector2Int.left);
            outputTilePositions.Add(tilePos + Vector2Int.right);
            outputTilePositions.Add(tilePos + Vector2Int.left + Vector2Int.down);
            outputTilePositions.Add(tilePos + Vector2Int.down);
            outputTilePositions.Add(tilePos + Vector2Int.right + Vector2Int.down);
        }



        if (!justPathfinding) 
        {
            foreach (var tilePos in outputTilePositions)
            {
                for (int i = 0; i < levelMaps.Length; i++)
                {
                    levelMaps[i].SetTile((Vector3Int)tilePos, levelRuleTiles[i]);
                }
            }
        }

        debugMap.gameObject.SetActive(false);

        validPosList = new List<Vector2Int>(inputTilePositions);
    }

    public Vector2Int GetRandomValidPos() 
    {
        return validPosList[Random.Range(0, validPosList.Count - 1)];
    }
}
