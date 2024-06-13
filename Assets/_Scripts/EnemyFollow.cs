/*
using UnityEngine;
using System.Collections.Generic;

public class EnemyFollow : MonoBehaviour
{
    public float speed = 7f;
    public float stoppingDistance = 1f;
    public Pathfinding pathfinding; // Reference to the Pathfinding script
    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    private void Update()
    {
        if (playerTransform != null && pathfinding != null)
        {
            // Find path from enemy's current position to player's position
            Vector3Int startPosition = pathfinding.GetGridPosition(transform.position);
            Vector3Int targetPosition = pathfinding.GetGridPosition(playerTransform.position);

            List<Node> path = pathfinding.FindPath(startPosition, targetPosition);

            if (path != null && path.Count > 1)
            {
                // Move along the path
                Node nextNode = path[1];
                Vector3 nextPosition = pathfinding.GetWorldPosition(nextNode.gridPosition);
                Vector3 direction = (nextPosition - transform.position).normalized;
                Vector3 moveAmount = direction * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, nextPosition) < stoppingDistance)
                {
                    // Move to the next position in the path
                    transform.position = nextPosition;
                }
                else
                {
                    // Move towards the next position
                    transform.Translate(moveAmount);
                }
            }
        }
    }
}*/
