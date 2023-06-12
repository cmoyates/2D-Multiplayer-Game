using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SnailBossAI : ImpAI
{
    [SerializeField]
    Tilemap floorOverlayMap;
    [SerializeField]
    Tile goopTile;
    [SerializeField]
    Transform gooperTransform;
    Vector3Int prevTilePos = Vector3Int.zero;
    [SerializeField]
    float goopSpeedMul = 1.5f;
    float potentialGoopSpeedMul = 1;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        floorOverlayMap = GameObject.Find("Floor Overlay NOCOL").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    new private void FixedUpdate()
    {
        if (health <= 0 || !GameManager.Instance.IsGamePlaying()) return;

        Vector3Int tilePos = floorOverlayMap.WorldToCell(gooperTransform.position);
        if (tilePos != prevTilePos) 
        {
            if (floorOverlayMap.GetTile(tilePos) == goopTile)
            {
                potentialGoopSpeedMul = goopSpeedMul;
            }
            else 
            {
                floorOverlayMap.SetTile(tilePos, goopTile);
                potentialGoopSpeedMul = 1;
            }

            prevTilePos = tilePos;
        }


        movement = Pathfind() * moveSpeed * potentialGoopSpeedMul;
        // Steering behavior so the player can out-maneuver them
        Vector2 steering = movement - rb.velocity;
        steering *= steeringScale;
        rb.velocity = (rb.velocity + steering);

        base.FixedUpdate();
    }
}
