using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GridMap gridMap;
    private CoinManager coinManager;
    private PlayerMoving player;

    public List<Node> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        Node startNode = gridMap.GetNode(startPos);
        Node targetNode = gridMap.GetNode(targetPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogError("Start node or target node is null. Cannot find path.");
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

            foreach (Node neighbour in gridMap.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null; // No valid path found
    }

    public void SetCoinManager(CoinManager manager)
    {
        coinManager = manager;
    }

    public List<Vector3Int> FindReachableCoins(Vector3Int startPos, float maxDistance)
    {
        List<Vector3Int> reachableCoins = new List<Vector3Int>();

        HashSet<Vector3Int> coinPositions = coinManager.GetCoinPositions();
        foreach (Vector3Int coinPosition in coinPositions)
        {
            float distanceToCoin = Vector3Int.Distance(startPos, coinPosition);
            if (distanceToCoin <= maxDistance)
            {
                reachableCoins.Add(coinPosition);
            }
        }

        return reachableCoins;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        return dstX + dstY;
    }

    // Add coin collection logic to be executed when the player reaches a coin
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coinManager.CollectCoin(other.gameObject);
            other.gameObject.SetActive(false); // Deactivate collected coin
        }
    }

    public List<Node> FindPathWithEnemyAvoidance(Vector3Int startPos, Vector3Int targetPos, float enemyDetectionRadius)
    {
        Node startNode = gridMap.GetNode(startPos);
        Node targetNode = gridMap.GetNode(targetPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogError("Start node or target node is null. Cannot find path.");
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

            foreach (Node neighbour in gridMap.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Check if the neighbour is within enemy detection radius

                // Convert the enemy position to integer grid coordinates
                // Check if the neighbour is within enemy detection radius
                // Check if the neighbour is within enemy detection radius
                // Check if the neighbour is within enemy detection radius
                Vector3 enemyPosition = player.enemy.transform.position; // Define the enemy position

                // Convert the enemy position to Vector3Int to match the grid coordinates
                Vector3Int enemyGridPosition = new Vector3Int(
                    Mathf.RoundToInt(enemyPosition.x),
                    Mathf.RoundToInt(enemyPosition.y),
                    Mathf.RoundToInt(enemyPosition.z)
                );

                // Convert the neighbour's grid position to Vector3Int
                Vector3Int neighbourGridPosition = new Vector3Int(
                    neighbour.gridPosition.x,
                    neighbour.gridPosition.y,
                    0 // Assuming z-coordinate is not used in your grid system
                );

                // Calculate the distance between the neighbour and the enemy in grid coordinates
                float distanceToEnemy = Vector3Int.Distance(neighbourGridPosition, enemyGridPosition);

                if (distanceToEnemy <= enemyDetectionRadius)
                {
                    continue; // Skip this neighbour if within enemy detection radius
                }



                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null; // No valid path found
    }

    public List<List<Node>> FindAllPaths(Vector3Int startPos, Vector3Int targetPos)
    {
        List<List<Node>> allPaths = new List<List<Node>>();
        // Implement your pathfinding algorithm here to find all paths
        return allPaths;
    }
}

