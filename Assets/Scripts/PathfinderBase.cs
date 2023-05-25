using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfinderBase : MonoBehaviour
{
    public Tilemap bgTopMap;
    public Tilemap bgBottomMap;
    public Tilemap colTopMap;
    public Tilemap colBottomMap;
    public Tilemap debugMap;
    public Tile debugTile;
    Vector2Int offset;

    // Start is called before the first frame update
    void Start()
    {
        Vector2Int mapSize = (Vector2Int)bgBottomMap.size;
        offset = (Vector2Int)bgBottomMap.origin;

        Debug.Log(mapSize);

        int tileCount = 0;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(x + offset.x, y + offset.y, 0);
                if (bgBottomMap.GetSprite(pos) != null) 
                {
                    tileCount++;
                    Debug.Log(bgBottomMap.GetSprite(pos).name);
                    debugMap.SetTile(pos, debugTile);
                }
            }
        }

        Debug.Log(tileCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
