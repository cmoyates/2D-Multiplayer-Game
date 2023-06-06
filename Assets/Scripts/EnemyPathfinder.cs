using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyPathfinder : MonoBehaviour
{
    public static EnemyPathfinder Instance { get; private set; }

    class Node
    {
        // Info about the node
        public int x = 0;
        public int y = 0;
        public int distance = 0;

        // Constructors
        public Node(int xPos, int yPos, int dis)
        {
            x = xPos;
            y = yPos;
            distance = dis;
        }
        public Node() { }

        // SetNode function to get around having to create new nodes every time
        public void SetNode(int xPos, int yPos, int dis)
        {
            x = xPos;
            y = yPos;
            distance = dis;
        }
    }

    int[][] m_grid;
    public int[][] m_distance;
    public BitArray[][] m_directions;
    List<Node> m_open;
    int width;
    int height;
    GameObject player;
    public Tilemap walls;
    public Vector2Int offset;
    bool active = false;
    public Tilemap debugMap;
    public Tile debugTile;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get references to the player (and the players AI if it's a bot), the walls tilemap, and the dungeon generator
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Initialize(Vector2Int origin, Vector2Int size)
    {
        // Get the offset needed to convert a world position to a grid position, as well as the width and height of the grid
        offset = origin;
        width = size.x;
        height = size.y;

        Debug.Log(walls.origin);
        Debug.Log(walls.size);

        // Create the "open list" and populate it with nodes
        m_open = new List<Node>();
        for (int i = 0; i < width * height; i++)
        {
            m_open.Add(new Node());
        }

        // Populate the grid, the distances and the directions with their starting variables
        m_grid = new int[width][];
        m_distance = new int[width][];
        m_directions = new BitArray[width][];

        // Get the position of the player on the grid
        int gx = Mathf.FloorToInt(player.transform.position.x) - offset.x;
        int gy = Mathf.FloorToInt(player.transform.position.y) - offset.y;

        for (int x = 0; x < width; x++)
        {
            m_grid[x] = new int[height];
            m_distance[x] = new int[height];
            m_directions[x] = new BitArray[height];
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x + offset.x, y + offset.y, 0);
                m_grid[x][y] = (walls.GetSprite(pos) == null && walls.GetTile(pos) != null) ? 0 : 1;
                if (m_grid[x][y] == 0)
                {
                    debugMap.SetTile(pos, debugTile);
                }
                if (x == gx && y == gy) { m_grid[x][y] = 7; }
                m_distance[x][y] = 0;
                m_directions[x][y] = new BitArray(4);
            }
        }

        active = true;
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePlaying() || !active) return;

        // Run the direction calculations
        DirectionGridCalc();
    }

    void DirectionGridCalc()
    {
        // Reset all of the directions that were calculated previously
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                m_distance[x][y] = (m_grid[x][y] == 1) ? -2 : -1;
                m_directions[x][y] = new BitArray(4);
            }
        }

        // Get the player's position on the grid
        int gx = Mathf.FloorToInt(player.transform.position.x) - offset.x;
        int gy = Mathf.FloorToInt(player.transform.position.y) - offset.y;

        // Starting node added to open
        m_open[0].SetNode(gx, gy, 0);
        m_distance[gx][gy] = 0;
        int openCount = 1;
        int openIndex = 0;

        // While open is not empty
        while (openCount - openIndex != 0)
        {
            // Current node is popped from open
            int x = m_open[openIndex].x, y = m_open[openIndex].y, dist = m_open[openIndex].distance;
            ++openIndex;
            // Have a bitset available to record the directions
            BitArray dirBits = new BitArray(4);
            // Expand to the neighbor nodes

            // Check that the current node is not at the top of the grid
            if (y != 0)
            {
                // If the node above has an assigned distance, do the direction calculation for up
                if ((m_distance[x][y - 1] + 1u) != 0)
                {
                    dirBits[0] = !((m_distance[x][y - 1] + 1u - dist) != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x, y - 1, dist + 1);
                    m_distance[x][y - 1] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the bottom of the grid
            if (height - (y + 1) != 0)
            {
                // If the node below has an assigned distance, do the direction calculation for down
                if ((m_distance[x][y + 1] + 1u) != 0)

                {
                    dirBits[1] = !(m_distance[x][y + 1] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x, y + 1, dist + 1);
                    m_distance[x][y + 1] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the far left of the grid
            if (x != 0)
            {
                // If the node to the left has an assigned distance, do the direction calculation for left
                if ((m_distance[x - 1][y] + 1u) != 0)
                {
                    dirBits[2] = !(m_distance[x - 1][y] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x - 1, y, dist + 1);
                    m_distance[x - 1][y] = dist + 1;
                    ++openCount;
                }
            }
            // Check that the current node is not at the far right of the grid
            if (width - (x + 1) != 0)
            {
                // If the node to the right has an assigned distance, do the direction calculation for right
                if ((m_distance[x + 1][y] + 1u) != 0)
                {
                    dirBits[3] = !(m_distance[x + 1][y] + 1u - dist != 0);
                }
                // Otherwise add it to the open list and set the distance
                else
                {
                    m_open[openCount].SetNode(x + 1, y, dist + 1);
                    m_distance[x + 1][y] = dist + 1;
                    ++openCount;
                }
            }
            // Assign the calcualted directions to the appropriate place on the directions grid
            m_directions[x][y] = dirBits;
        }
    }

    public BitArray GetDirArray(int x, int y) 
    {
        return m_directions[x][y];
    }

    public void DeactivatePathfinding() 
    {
        active = false;
    }

    public Vector2Int GetOffset() 
    {
        return offset;
    }
}