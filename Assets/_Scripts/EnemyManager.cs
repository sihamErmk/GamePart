using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    public int maxRegularEnemiesPerRoom = 2;
    public bool spawnBoss = true;
    public Tilemap floorTile;
    public int borderMargin = 1;
    private bool bossSpawned = false;

    private List<Vector3Int> validPositions = new List<Vector3Int>();
    private Transform playerTransform;

    private void Start()
    {
        Debug.Log("EnemyManager Start method called");

        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            Debug.Log("CoinManager found");

            HashSet<Vector3Int> coinPositions = coinManager.GetCoinPositions();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("Player found");

                playerTransform = player.transform;
                BoundsInt roomBounds = floorTile.cellBounds;
                GenerateEnemiesInRoom(roomBounds, coinPositions);

            }
            else
            {
                Debug.LogError("Player not found in the scene. Enemies cannot be generated.");
            }
        }
        else
        {
            Debug.LogError("CoinManager not found in the scene. Enemies cannot be generated.");
        }
    }
    public List<Vector3Int> enemyPositions = new List<Vector3Int>();

    public void RegisterEnemy(Vector3Int position)
    {
        enemyPositions.Add(position);
    }

    public void RemoveEnemy(Vector3Int position)
    {
        enemyPositions.Remove(position);
    }

    public List<Vector3Int> GetEnemyPositions()
    {
        return new List<Vector3Int>(enemyPositions);
    }

    public void GenerateEnemiesInRoom(BoundsInt roomBounds, HashSet<Vector3Int> coinPositions)
    {
        ClearEnemies();
        HashSet<Vector3Int> generatedPositions = new HashSet<Vector3Int>();
        CollectValidPositions(roomBounds);

        for (int i = 0; i < maxRegularEnemiesPerRoom; i++)
        {
            Vector3Int randomPosition = GetRandomValidPosition(generatedPositions, coinPositions);
            if (randomPosition != Vector3Int.zero)
            {
                InstantiateEnemy(enemyPrefab, randomPosition);
                generatedPositions.Add(randomPosition);
            }
        }

        if (spawnBoss && !bossSpawned)
        {
            Vector3Int bossPosition = GetFarValidPosition(generatedPositions, coinPositions);
            if (bossPosition != Vector3Int.zero)
            {
                InstantiateEnemy(bossPrefab, bossPosition);
                bossSpawned = true;
            }
        }
    }

    private void CollectValidPositions(BoundsInt roomBounds)
    {
        validPositions.Clear();
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();
        int minDistanceSquared = 15 * 15;

        for (int x = roomBounds.xMin + borderMargin; x <= roomBounds.xMax - borderMargin; x++)
        {
            for (int y = roomBounds.yMin + borderMargin; y <= roomBounds.yMax - borderMargin; y++)
            {
                Vector3Int position = new Vector3Int(x, y, roomBounds.z);

                bool isValidPosition = true;
                for (int dx = -borderMargin; dx <= borderMargin; dx++)
                {
                    for (int dy = -borderMargin; dy <= borderMargin; dy++)
                    {
                        Vector3Int surroundingPosition = new Vector3Int(x + dx, y + dy, roomBounds.z);
                        if (!floorTile.HasTile(surroundingPosition))
                        {
                            isValidPosition = false;
                            break;
                        }
                    }
                    if (!isValidPosition)
                    {
                        break;
                    }
                }

                bool isFarFromOtherEnemies = true;
                foreach (Vector3Int occupied in occupiedPositions)
                {
                    if ((occupied - position).sqrMagnitude < minDistanceSquared)
                    {
                        isFarFromOtherEnemies = false;
                        break;
                    }
                }

                if (isValidPosition && isFarFromOtherEnemies)
                {
                    validPositions.Add(position);
                    occupiedPositions.Add(position);
                }
            }
        }
    }

    private Vector3Int GetRandomValidPosition(HashSet<Vector3Int> generatedPositions, HashSet<Vector3Int> coinPositions)
    {
        List<Vector3Int> availablePositions = new List<Vector3Int>(validPositions);
        availablePositions.RemoveAll(pos => generatedPositions.Contains(pos) || coinPositions.Contains(pos));

        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("No valid positions available for generating enemies.");
            return Vector3Int.zero;
        }

        return availablePositions[Random.Range(0, availablePositions.Count)];
    }

    private Vector3Int GetFarValidPosition(HashSet<Vector3Int> generatedPositions, HashSet<Vector3Int> coinPositions)
    {
        List<Vector3Int> availablePositions = new List<Vector3Int>(validPositions);
        availablePositions.RemoveAll(pos => generatedPositions.Contains(pos) || coinPositions.Contains(pos));

        Vector3 playerPosition = playerTransform.position;
        Vector3Int farthestPosition = Vector3Int.zero;
        float maxDistanceSquared = 0;

        foreach (Vector3Int position in availablePositions)
        {
            float distanceSquared = (floorTile.CellToWorld(position) - playerPosition).sqrMagnitude;
            if (distanceSquared > maxDistanceSquared)
            {
                maxDistanceSquared = distanceSquared;
                farthestPosition = position;
            }
        }

        return farthestPosition;
    }

    private void InstantiateEnemy(GameObject enemyPrefab, Vector3Int position)
    {
        Debug.Log("Instantiating enemy at position: " + position);
        GameObject enemyObject = Instantiate(enemyPrefab, floorTile.CellToWorld(position) + floorTile.tileAnchor, Quaternion.identity, transform);

        Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.SetPlayerTransform(playerTransform);
        }
        else
        {
            Debug.LogError("Enemy component not found on enemy prefab!");
        }
    }

    private void ClearEnemies()
    {
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
        }
    }
}