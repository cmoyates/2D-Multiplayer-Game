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

    // Start is called before the first frame update
    void Awake()
    {
        Vector2Int mapSize = (Vector2Int)debugMap.size;
        offset = (Vector2Int)debugMap.origin;


        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(x + offset.x, y + offset.y, 0);
                if (debugMap.GetSprite(pos) != null)
                {
                    for (int i = 0; i < levelMaps.Length; i++)
                    {
                        levelMaps[i].SetTile(pos, levelRuleTiles[i]);
                    }
                }
            }
        }

        debugMap.gameObject.SetActive(false);
    }
}
