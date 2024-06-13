/*using UnityEngine;
using System.Collections.Generic;

public class EnemyPathfinding : MonoBehaviour
{
    public GridMap gridMap; // Reference to the GridMap script

    private Node[,] grid; // Reference to the grid created by GridMap

    void Start()
    {
        if (gridMap == null)
        {
            Debug.LogError("GridMap is not assigned!");
        }
        else
        {
            Debug.Log("GridMap is assigned.");
        }

        // Get the grid from the GridMap script
        //grid = gridMap.grid;
    }

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        // Get the nearest walkable nodes to the provided positions
      //  Node startNode = GetNearestWalkableNode(gridMap.GetWorldPosition(startPos));
      //  Node targetNode = GetNearestWalkableNode(gridMap.GetWorldPosition(targetPos));

        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Start or target node is null.");
            return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbours(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("No path found.");
        return null;
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        List<Vector2Int> waypoints = new List<Vector2Int>();
        foreach (Node node in path)
        {
            waypoints.Add(node.gridPosition);
        }

        return waypoints;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        return dstX + dstY;
    }

    private Node GetNearestWalkableNode(Vector3 worldPosition)
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

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Define the positions of potential neighboring nodes
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0), // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(0, 1)  // Up
        };

        // Iterate over the potential neighboring positions and add valid ones to the list
        foreach (Vector2Int offset in neighborOffsets)
        {
            Vector2Int neighborPos = node.gridPosition + offset;
            if (IsValidPosition(neighborPos))
            {
                neighbours.Add(grid[neighborPos.x, neighborPos.y]);
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
}
*/