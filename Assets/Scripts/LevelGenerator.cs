using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject levelGenRectPrefab;
    [SerializeField]
    float rectSpawnRadius = 3.0f;
    [SerializeField]
    int rectCount = 15;
    [SerializeField]
    Vector2Int minMaxRectSize = Vector2Int.one;
    [SerializeField]
    int rectGap = 3;
    GameObject[] spawnedRects;
    bool[] rectsOverlapping;
    bool rectSpreadingComplete = false;
    int[] rectAreas;
    [SerializeField]
    float largestRectPercent = 0.3f;
    GameObject[] mainRoomRects;
    GameObject[] otherRoomRects;
    FancyTriangleMaker ftm;
    Vector2[] points;
    Vector2[][] edges;
    MST.Edge[] mstEdges;
    List<Vector2[]> hallwayLines = new List<Vector2[]>();
    [SerializeField]
    int hallwayWidth = 3;
    [SerializeField]
    Tilemap debugTilemap;
    [SerializeField]
    Tile redTile;
    [SerializeField]
    Tile greyTile;
    Vector2Int offset;
    [SerializeField]
    Tilemap[] levelTilemaps;
    [SerializeField]
    RuleTile[] levelRuleTiles;
    bool levelGenStarted = false;
    int[] startAndEndIndices = new int[2];
    Vector2[] mstPoints;

    // Start is called before the first frame update
    void Start()
    {
        ftm = GetComponent<FancyTriangleMaker>();

        spawnedRects = new GameObject[rectCount];
        rectsOverlapping = new bool[rectCount];
        rectAreas = new int[rectCount];

        StartLevelGeneration();
    }

    public void StartLevelGeneration() 
    {
        if (levelGenStarted) return;

        levelGenStarted = true;
        
        for (int i = 0; i < rectCount; i++)
        {
            spawnedRects[i] = Instantiate(levelGenRectPrefab, GetRandomPointWithinRadius(), Quaternion.identity);
            spawnedRects[i].name = "Rect " + i.ToString();
            spawnedRects[i].transform.SetParent(transform);
            LevelGenRect rectScript = spawnedRects[i].GetComponent<LevelGenRect>();
            rectScript.lg = this;
            rectScript.index = i;
            Vector2Int size = new Vector2Int(Random.Range(minMaxRectSize.x, minMaxRectSize.y), Random.Range(minMaxRectSize.x, minMaxRectSize.y));
            rectScript.SetSize(size, rectGap);
            rectsOverlapping[i] = true;
            rectAreas[i] = size.x * size.y;
        }
    }

    // Call this method to get a random point within the specified radius from the center point
    public Vector2 GetRandomPointWithinRadius()
    {
        // Generate random angle in radians
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        // Calculate random distance within the radius
        float randomDistance = Random.Range(0f, rectSpawnRadius);

        // Calculate the coordinates of the random point
        float x = Mathf.Cos(randomAngle) * randomDistance;
        float y = Mathf.Sin(randomAngle) * randomDistance;

        return new Vector2(x, y);
    }

    public void RectOverlapChange(int rectIndex, bool overlapping) 
    {
        if (rectSpreadingComplete) return;
        rectsOverlapping[rectIndex] = overlapping;
        bool noRectsOverlapping = true;
        for (int i = 0; i < rectCount; i++)
        {
            noRectsOverlapping = noRectsOverlapping && !rectsOverlapping[i];
        }
        if (noRectsOverlapping) 
        {
            Debug.Log("Rect Spreading Complete");
            rectSpreadingComplete = true;
            for (int i = 0; i < rectCount; i++)
            {
                spawnedRects[i].GetComponent<LevelGenRect>().Stop();
            }
            GenerateLevel();
        }
    }

    void GenerateLevel() 
    {
        GetMainRooms();
        GetDelaunayEdges();
        GetMinSpanningTree();
        GetStartAndEnd();
        GetHallwayLines();
        //GetOtherRooms();
        foreach (var roomRect in otherRoomRects)
        {
            roomRect.SetActive(false);
        }
        PlaceHallwayTiles();
        PlaceRoomTiles();
        SetupRooms();
        PopulateTilemap();

        //debugTilemap.ClearAllTiles();
        GameManager.Instance.StartCountdown();
    }

    void GetMainRooms() 
    {
        int[] rectIndices = new int[rectCount];
        for (int i = 0; i < rectCount; i++)
        {
            rectIndices[i] = i;
        }
        rectIndices = rectIndices.OrderByDescending(i => rectAreas[i]).ToArray();
        int cutoff = Mathf.FloorToInt(rectCount * largestRectPercent);
        mainRoomRects = new GameObject[cutoff];
        otherRoomRects = new GameObject[rectCount - cutoff];
        for (int i = 0; i < rectCount; i++)
        {
            if (i < cutoff)
            {
                mainRoomRects[i] = spawnedRects[rectIndices[i]];
            }
            else 
            {
                otherRoomRects[i - cutoff] = spawnedRects[rectIndices[i]];
            }
        }
    }

    void GetDelaunayEdges() 
    {
        points = new Vector2[mainRoomRects.Length];
        for (int i = 0; i < mainRoomRects.Length; i++)
        {
            points[i] = mainRoomRects[i].transform.position;
        }

        edges = ftm.MakeFancyTriangles(points).ToArray();
    }

    void GetMinSpanningTree() 
    {
        // Make an array with all of the edges being represented by arrays of two integers, each of which represents a vertex
        int[][] edgesWithVertIndecies = new int[edges.Length][];
        for (int i = 0; i < edges.Length; i++)
        {
            edgesWithVertIndecies[i] = new int[2];
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < mainRoomRects.Count(); k++)
                {
                    if (Vector2.SqrMagnitude((Vector2)mainRoomRects[k].transform.position - edges[i][j]) < 0.01f)
                    {
                        edgesWithVertIndecies[i][j] = k;
                        break;
                    }
                }
            }
        }

        // Get a minimum spanning tree from the edges and vertices
        mstEdges = MST.calculateMST(points, edgesWithVertIndecies).ToArray();
    }

    void GetStartAndEnd() 
    {
        mstPoints = mainRoomRects.Select(rect => (Vector2)rect.transform.position).ToArray();
        startAndEndIndices =  MST.GetFarthestPoints(mstEdges, mstPoints);
        Debug.Log("From " + mainRoomRects[startAndEndIndices[0]].name + " to " + mainRoomRects[startAndEndIndices[1]].name);

    }

    void GetHallwayLines() 
    {
        for (int i = 0; i < mstEdges.Length; i++)
        {
            float xDiff = Mathf.Abs(mainRoomRects[mstEdges[i].src].transform.position.x - mainRoomRects[mstEdges[i].dest].transform.position.x);
            float yDiff = Mathf.Abs(mainRoomRects[mstEdges[i].src].transform.position.y - mainRoomRects[mstEdges[i].dest].transform.position.y);
            Vector2 midwayPoint = new Vector3(0, 0, 0);
            bool xDiffSmaller = xDiff < yDiff;
            if (xDiffSmaller)
            {
                midwayPoint.x = mainRoomRects[mstEdges[i].dest].transform.position.x;
                midwayPoint.y = mainRoomRects[mstEdges[i].src].transform.position.y;
            }
            else
            {
                midwayPoint.x = mainRoomRects[mstEdges[i].src].transform.position.x;
                midwayPoint.y = mainRoomRects[mstEdges[i].dest].transform.position.y;
            }

            hallwayLines.Add(new Vector2[] { mainRoomRects[mstEdges[i].src].transform.position, midwayPoint });
            hallwayLines.Add(new Vector2[] { midwayPoint, mainRoomRects[mstEdges[i].dest].transform.position });
        }
    }

    void GetOtherRooms() 
    {
        // For every rectangle previously generated
        for (int i = 0; i < spawnedRects.Length; i++)
        {
            // If the rectangle is a main rectangle already, continue
            if (mainRoomRects.Contains(spawnedRects[i])) { continue; }
            // Otherwise, get the bounds of that rectangle
            Bounds rectBounds = spawnedRects[i].GetComponent<BoxCollider2D>().bounds;
            // Make an array of four Vec2s, with each one representing a corner of the rectangle
            Vector2[] corners = new Vector2[] { new Vector2(rectBounds.min.x, rectBounds.min.y), new Vector2(rectBounds.max.x, rectBounds.min.y),
                new Vector2(rectBounds.max.x, rectBounds.max.y), new Vector2(rectBounds.min.x, rectBounds.max.y) };
            // Assume that the rectangle is not valid
            bool isValid = false;
            // For every line previously drawn
            for (int j = 0; j < hallwayLines.Count; j++)
            {
                // If the line intersects one of the edges of the rectangle, change it's color and consider it valid
                Vector2 src = hallwayLines[j][0];
                Vector2 dest = hallwayLines[j][1];
                if (LineIntersect(src, dest, corners[0], corners[1]) || LineIntersect(src, dest, corners[1], corners[2])
                    || LineIntersect(src, dest, corners[2], corners[3]) || LineIntersect(src, dest, corners[3], corners[0]))
                {
                    isValid = true;
                    break;
                }
            }
            // If it's not valid after cycling through every line, deactivate it
            if (!isValid)
            {
                spawnedRects[i].SetActive(false);
            }
        }
    }

    // A function that checks if a line from point a to point b intersects a line from point c to point d
    bool LineIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        Vector2 r = b - a;
        Vector2 s = (d - c);
        float rxs = r.x * s.y - s.x * r.y;
        Vector2 cma = c - a;
        float t = (cma.x * s.y - s.x * cma.y) / rxs;
        float u = (cma.x * r.y - r.x * cma.y) / rxs;
        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    void PlaceRoomTiles() 
    {
        string endRoomName = mainRoomRects[startAndEndIndices[1]].name;
        for (int i = 0; i < spawnedRects.Length; i++)
        {
            GameObject roomRect = spawnedRects[i];
            if (!roomRect.activeInHierarchy) continue;

            // Get the bounds of the rectangles collider
            Bounds bounds = roomRect.GetComponent<BoxCollider2D>().bounds;

            Vector2Int minIntVec = new Vector2Int(Mathf.FloorToInt(bounds.min.x), Mathf.FloorToInt(bounds.min.y));
            Vector2Int maxIntVec = new Vector2Int(Mathf.FloorToInt(bounds.max.x), Mathf.FloorToInt(bounds.max.y));

            // Place a floor tile everywhere on the floor tilemap that the rectangle overlaps
            for (int x = minIntVec.x; x < maxIntVec.x; x++)
            {
                for (int y = minIntVec.y; y < maxIntVec.y; y++)
                {
                    Tile tile = roomRect.name.Equals(endRoomName) ? redTile : greyTile;
                    debugTilemap.SetTile(new Vector3Int(x, y), tile);
                }
            }

            roomRect.transform.position = new Vector3((maxIntVec.x - minIntVec.x) / 2.0f + minIntVec.x, (maxIntVec.y - minIntVec.y + 1) / 2.0f + minIntVec.y);
        }
    }

    void SetupRooms() 
    {
        List<MST.Edge>[] edgesFromPoints = MST.GetEdgesFromPointsArray(mstEdges, mstPoints);

        int[] stepsFromStart = MST.GetStepsFromPoint(startAndEndIndices[0], edgesFromPoints);
        bool[] isLeafArray = MST.GetIsLeafArray(edgesFromPoints);

        LevelManager.Instance.maxSteps = Mathf.Max(stepsFromStart);

        for (int i = 0; i < mainRoomRects.Length; i++)
        {
            GameObject roomRect = mainRoomRects[i];
            
            // Get rid of the level gen rect script
            Destroy(roomRect.GetComponent<LevelGenRect>());
            // 
            RoomScript roomScript = roomRect.AddComponent<RoomScript>();
            roomScript.steps = stepsFromStart[i];
            roomScript.leafRoom = isLeafArray[i];
            roomScript.isEndRoom = startAndEndIndices[1] == i;

            BoxCollider2D rectCol = roomRect.GetComponent<BoxCollider2D>();
            rectCol.isTrigger = true;
            rectCol.size += Vector2.up;
            rectCol.size -= Vector2.one * 2;
        }

        // Set up some stuff for the start and end rooms
        GameObject startRoom = mainRoomRects[startAndEndIndices[0]];

        LevelManager.Instance.LockRoom(startRoom, false);
        LevelManager.Instance.UnlockRoom();

        PlayerManager.Instance.SpawnPlayerAtPos(startRoom.transform.position);
        // Might need to change this later
        Destroy(startRoom.GetComponent<RoomScript>());
        Destroy(startRoom.GetComponent<BoxCollider2D>());

        LevelManager.Instance.endRoom = mainRoomRects[startAndEndIndices[1]];
    }

    void PlaceHallwayTiles() 
    {
        // For every "hallway line", place a line of floor tiles (of a specified width) following that line
        for (int i = 0; i < hallwayLines.Count; i++)
        {
            if (hallwayLines[i][0].x != hallwayLines[i][1].x)
            {
                int maxXPoint = Mathf.RoundToInt(Mathf.Max(hallwayLines[i][0].x, hallwayLines[i][1].x));
                int minXPoint = Mathf.RoundToInt(Mathf.Min(hallwayLines[i][0].x, hallwayLines[i][1].x));
                for (int x = minXPoint - 2; x < maxXPoint + 3; x++)
                {
                    for (int y = -hallwayWidth; y < 1 + hallwayWidth; y++)
                    {
                        debugTilemap.SetTile(new Vector3Int(x, y + Mathf.RoundToInt(hallwayLines[i][0].y), 0), greyTile);
                    }
                }
            }
            else
            {
                int maxYPoint = Mathf.RoundToInt(Mathf.Max(hallwayLines[i][0].y, hallwayLines[i][1].y));
                int minYPoint = Mathf.RoundToInt(Mathf.Min(hallwayLines[i][0].y, hallwayLines[i][1].y));
                for (int x = -hallwayWidth; x < 1 + hallwayWidth; x++)
                {
                    for (int y = minYPoint - 2; y < maxYPoint + 3; y++)
                    {
                        debugTilemap.SetTile(new Vector3Int(x + Mathf.RoundToInt(hallwayLines[i][0].x), y, 0), greyTile);
                    }
                }
            }
        }
    }

    void PopulateTilemap() 
    {
        HashSet<Vector2Int> inputTilePositions = new HashSet<Vector2Int>();

        Vector2Int mapSize = (Vector2Int)debugTilemap.size;
        offset = (Vector2Int)debugTilemap.origin;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(x + offset.x, y + offset.y, 0);
                if (debugTilemap.GetSprite(pos) != null)
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



        foreach (var tilePos in outputTilePositions)
        {
            for (int i = 0; i < levelTilemaps.Length; i++)
            {
                levelTilemaps[i].SetTile((Vector3Int)tilePos, levelRuleTiles[i]);
            }
        }
    }
}
