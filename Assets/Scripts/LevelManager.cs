using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event EventHandler OnRoomLockUnlock;

    [SerializeField]
    Tilemap[] barrierTilemaps;
    [SerializeField]
    RuleTile[] barrierRuleTiles;
    public GameObject endRoom;
    public int maxSteps;
    public Vector2Int minMaxRoomSize;
    public int globalDifficultyScale = 50;
    Vector3 lockedRoomCenter = Vector3.zero;
    bool lockedRoomIsLeaf = false;
    Bounds lockedRoomBounds;
    public GameObject chestPrefab;
    [SerializeField]
    Tilemap debugTilemap;
    [SerializeField]
    Tile greenTile;
    [SerializeField]
    Tilemap collisionTilemap;
    bool isInLockedRoom = false;

    private void Awake()
    {
        Instance = this;
    }

    public Bounds LockRoom(GameObject roomRect, bool leafRoom) 
    {
        lockedRoomIsLeaf = leafRoom;
        lockedRoomCenter = roomRect.transform.position;

        BoxCollider2D roomCol = roomRect.GetComponent<BoxCollider2D>();
        lockedRoomBounds = roomCol.bounds;

        // Place a floor tile everywhere on the floor tilemap that the rectangle overlaps
        for (int i = 0; i < barrierTilemaps.Length; i++)
        {
            FillTiles(barrierTilemaps[i], lockedRoomBounds, barrierRuleTiles[i]);
        }
        
        Destroy(roomCol);

        isInLockedRoom = true;
        OnRoomLockUnlock?.Invoke(this, EventArgs.Empty);

        return lockedRoomBounds;
    }

    public void UnlockRoom() 
    {
        foreach (var barrierTilemap in barrierTilemaps)
        {
            barrierTilemap.ClearAllTiles();
        }

        FillTiles(debugTilemap, lockedRoomBounds, greenTile);

        if (lockedRoomIsLeaf) 
        {
            Instantiate(chestPrefab, lockedRoomCenter, Quaternion.identity);
        }

        isInLockedRoom = false;
        OnRoomLockUnlock?.Invoke(this, EventArgs.Empty);
    }

    void FillTiles(Tilemap tilemap, Bounds bounds, TileBase tile) 
    {
        for (int x = Mathf.FloorToInt(bounds.min.x) - 2; x < Mathf.FloorToInt(bounds.max.x) + 2; x++)
        {
            for (int y = Mathf.FloorToInt(bounds.min.y) - 2; y < Mathf.FloorToInt(bounds.max.y) + 2; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y), tile);
            }
        }
    }

    public Vector2Int GetRandomPosInRoom() 
    {
        while (true) 
        {
            int x = UnityEngine.Random.Range(Mathf.FloorToInt(lockedRoomBounds.min.x), Mathf.FloorToInt(lockedRoomBounds.max.x));
            int y = UnityEngine.Random.Range(Mathf.FloorToInt(lockedRoomBounds.min.y), Mathf.FloorToInt(lockedRoomBounds.max.y));
            Vector2Int pos = new Vector2Int(x, y);
            if (collisionTilemap.GetSprite((Vector3Int)pos) == null) return pos;
        }
    }

    public bool GetIsInLockedRoom() 
    {
        return isInLockedRoom;
    }
}
