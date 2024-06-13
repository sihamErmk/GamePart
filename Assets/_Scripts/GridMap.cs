using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    private Vector2Int gridSize;
    public float cellSize = 1f;

    public Node[,] grid; // Make this public
    public List<Node> path; // Declare the path variable here

    void Start()
    {
        FindGridSize();
        CreateGrid(gridSize.x, gridSize.y);
    }

    void FindGridSize()
    {
        BoundsInt floorBounds = floorTilemap.cellBounds;
        BoundsInt wallBounds = wallTilemap.cellBounds;

        // Combine bounds of both tilemaps to cover the entire area
        BoundsInt combinedBounds = floorBounds;
        combinedBounds.xMin = Mathf.Min(floorBounds.xMin, wallBounds.xMin);
        combinedBounds.yMin = Mathf.Min(floorBounds.yMin, wallBounds.yMin);
        combinedBounds.xMax = Mathf.Max(floorBounds.xMax, wallBounds.xMax);
        combinedBounds.yMax = Mathf.Max(floorBounds.yMax, wallBounds.yMax);

        gridSize = new Vector2Int(combinedBounds.size.x, combinedBounds.size.y);
    }

    void CreateGrid(int width, int height)
    {
        grid = new Node[width, height];
        BoundsInt combinedBounds = wallTilemap.cellBounds;
        combinedBounds.xMin = Mathf.Min(floorTilemap.cellBounds.xMin, wallTilemap.cellBounds.xMin);
        combinedBounds.yMin = Mathf.Min(floorTilemap.cellBounds.yMin, wallTilemap.cellBounds.yMin);
        combinedBounds.xMax = Mathf.Max(floorTilemap.cellBounds.xMax, wallTilemap.cellBounds.xMax);
        combinedBounds.yMax = Mathf.Max(floorTilemap.cellBounds.yMax, wallTilemap.cellBounds.yMax);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x + combinedBounds.xMin, y + combinedBounds.yMin, 0);

                if (!floorTilemap.HasTile(cellPosition) && !wallTilemap.HasTile(cellPosition))
                {
                    continue; // Skip this cell if it's not part of floor or wall
                }

                Vector3 worldPoint = wallTilemap.CellToWorld(cellPosition) + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                bool walkable = !IsWall(cellPosition);
                grid[x, y] = new Node(walkable, new Vector2Int(x, y), worldPoint);
            }
        }
    }

    bool IsWall(Vector3Int cellPosition)
    {
        return wallTilemap.HasTile(cellPosition);
    }

    public Node GetNode(Vector3 worldPosition)
    {
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPosition);

        // Check if the cell position is within the bounds of the grid
        if (!IsWithinGridBounds(cellPosition))
        {
            Debug.LogWarning("World position is outside the bounds of the grid.");
            return null;
        }

        int x = cellPosition.x - wallTilemap.cellBounds.xMin;
        int y = cellPosition.y - wallTilemap.cellBounds.yMin;

        // Ensure that the grid array is not null and the indices are within its bounds
        if (grid != null && IsWithinGridIndices(x, y))
        {
            return grid[x, y];
        }
        else
        {
            Debug.LogWarning("Grid array is null or index is out of bounds.");
            return null;
        }
    }

    bool IsWithinGridBounds(Vector3Int cellPosition)
    {
        return cellPosition.x >= wallTilemap.cellBounds.xMin &&
               cellPosition.x < wallTilemap.cellBounds.xMax &&
               cellPosition.y >= wallTilemap.cellBounds.yMin &&
               cellPosition.y < wallTilemap.cellBounds.yMax;
    }

    bool IsWithinGridIndices(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Define the positions of potential neighboring nodes
        Vector2Int[] neighborPositions = new Vector2Int[]
        {
            new Vector2Int(node.gridPosition.x - 1, node.gridPosition.y), // Left
            new Vector2Int(node.gridPosition.x + 1, node.gridPosition.y), // Right
            new Vector2Int(node.gridPosition.x, node.gridPosition.y - 1), // Down
            new Vector2Int(node.gridPosition.x, node.gridPosition.y + 1)  // Up
        };

        // Iterate over the potential neighboring positions and add valid ones to the list
        foreach (Vector2Int pos in neighborPositions)
        {
            if (IsValidPosition(pos))
            {
                neighbours.Add(grid[pos.x, pos.y]);
            }
        }

        return neighbours;
    }

    private bool IsValidPosition(Vector2Int position)
    {
        // Check if the position is within the bounds of the grid
        return position.x >= 0 && position.x < grid.GetLength(0) &&
               position.y >= 0 && position.y < grid.GetLength(1);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        Vector3 origin = wallTilemap.transform.position;
        Vector3 cellOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
        Vector3 worldPosition = origin + new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, 0) + cellOffset;
        return worldPosition;
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPosition);
        int x = cellPosition.x - wallTilemap.cellBounds.xMin;
        int y = cellPosition.y - wallTilemap.cellBounds.yMin;
        return new Vector2Int(x, y);
    }

    public Node GetNearestWalkableNode(Vector3 worldPosition)
    {
        Node nearestNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Node node in grid)
        {
            if (node != null && node.walkable)
            {
                float distance = Vector3.Distance(node.worldPosition, worldPosition);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestNode = node;
                }
            }
        }

        return nearestNode;
    }

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n != null) // Only draw nodes that are not null
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellSize - 0.1f));
                }
            }
        }

        if (path != null)
        {
            foreach (Node n in path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellSize - 0.1f));
            }
        }
    }
}

public class Node
{
    public bool walkable;
    public Vector2Int gridPosition;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector2Int _gridPosition, Vector3 _worldPosition)
    {
        walkable = _walkable;
        gridPosition = _gridPosition;
        worldPosition = _worldPosition;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
    public bool ContainsEnemy(float enemyDetectionRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, enemyDetectionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }
}