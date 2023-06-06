using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    Tilemap[] barrierTilemaps;
    [SerializeField]
    RuleTile[] barrierRuleTiles;
    public GameObject endRoom;

    private void Awake()
    {
        Instance = this;
    }

    public Bounds LockRoom(GameObject roomRect) 
    {
        BoxCollider2D roomCol = roomRect.GetComponent<BoxCollider2D>();
        Bounds bounds = roomCol.bounds;

        // Place a floor tile everywhere on the floor tilemap that the rectangle overlaps
        for (int x = Mathf.RoundToInt(bounds.min.x) - 2; x < Mathf.RoundToInt(bounds.max.x) + 2; x++)
        {
            for (int y = Mathf.RoundToInt(bounds.min.y) - 2; y < Mathf.RoundToInt(bounds.max.y) + 3; y++)
            {
                for (int i = 0; i < barrierTilemaps.Length; i++)
                {
                    barrierTilemaps[i].SetTile(new Vector3Int(x, y, 0), barrierRuleTiles[i]);
                }
            }
        }

        Destroy(roomCol);

        return bounds;
    }

    public void UnlockRoom() 
    {
        foreach (var barrierTilemap in barrierTilemaps)
        {
            barrierTilemap.ClearAllTiles();
        }
    }
}
