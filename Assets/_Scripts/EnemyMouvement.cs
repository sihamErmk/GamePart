// EnemyMovement.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 patrolCenter;
    private float patrolRadius = 1.0f;
    private Vector3 targetPosition;
    private float speed = 2.0f;
    private bool isPatrolling = true;

    public Tilemap floorTilemap;
    public Transform player;
    public float detectionRange = 5.0f;

    private Pathfinding pathfinding;
    private List<Node> currentPath = null;
    private int pathIndex = 0;

    void Start()
    {
        pathfinding = GetComponent<Pathfinding>();
        SetNewTargetPosition();
    }

    public void SetPatrolArea(Vector3 center, float radius, Tilemap tilemap)
    {
        patrolCenter = center;
        patrolRadius = radius;
        floorTilemap = tilemap;
        SetNewTargetPosition();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            if (isPatrolling)
            {
                isPatrolling = true;              
                pathIndex = 0;
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
        else
        {
            if (!isPatrolling)
            {
                isPatrolling = true;
                SetNewTargetPosition();
            }
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        if ((targetPosition - transform.position).sqrMagnitude < 0.1f)
        {
            SetNewTargetPosition();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    private void SetNewTargetPosition()
    {
        Vector3Int randomTilePosition;
        TileBase tile;

        do
        {
            Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
            randomTilePosition = floorTilemap.WorldToCell(patrolCenter + new Vector3(randomDirection.x, randomDirection.y, 0));
            randomTilePosition.x = Mathf.Clamp(randomTilePosition.x, floorTilemap.cellBounds.xMin + 1, floorTilemap.cellBounds.xMax - 1);
            randomTilePosition.y = Mathf.Clamp(randomTilePosition.y, floorTilemap.cellBounds.yMin + 1, floorTilemap.cellBounds.yMax - 1);
            tile = floorTilemap.GetTile(randomTilePosition);
        } while (tile == null || !IsTileFarFromWalls(randomTilePosition));

        targetPosition = floorTilemap.GetCellCenterWorld(randomTilePosition);
    }

    private bool IsTileFarFromWalls(Vector3Int tilePosition)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector3Int neighborPosition = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z);
                TileBase neighborTile = floorTilemap.GetTile(neighborPosition);
                if (neighborTile == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    IEnumerator FollowPath()
    {
        while (currentPath != null && pathIndex < currentPath.Count)
        {
            Node targetNode = currentPath[pathIndex];
            Vector3 targetPosition = targetNode.worldPosition;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            pathIndex++;
        }
    }
}
