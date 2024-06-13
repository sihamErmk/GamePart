using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FireSpawner : MonoBehaviour
{
    public GameObject[] firePrefabs;
    public Tilemap floorTilemap;
    public int fireCount = 40;
    public int minDistanceBetweenFires = 10;

    private HashSet<Vector3Int> firePositions = new HashSet<Vector3Int>();

    private void Start()
    {
        StartCoroutine(SpawnFiresRoutine());
    }
    public HashSet<Vector3Int> GetFirePositions()
    {
        return new HashSet<Vector3Int>(firePositions);
    }

    private IEnumerator SpawnFiresRoutine()
    {
        while (true)
        {
            if (firePositions.Count < fireCount)
            {
                GenerateFireSprite();
            }
            yield return null; // Wait for the next frame
        }
    }

    private void GenerateFireSprite()
    {
        // Generate a random position within the bounds of the Tilemap
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int randomPosition;

        // Loop until a valid position within the tilemap bounds is found
        do
        {
            randomPosition = new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), bounds.z);
        } while (!floorTilemap.HasTile(randomPosition));

        // Check if the random position is not already occupied by another fire
        if (!firePositions.Contains(randomPosition))
        {
            // Ensure minimum distance between fires
            if (IsFarEnoughFromOtherFires(randomPosition))
            {
                GameObject randomFirePrefab = firePrefabs[Random.Range(0, firePrefabs.Length)];
                Instantiate(randomFirePrefab, floorTilemap.CellToWorld(randomPosition), Quaternion.identity, transform);
                firePositions.Add(randomPosition);
            }
        }
    }


    private void ClearFires()
    {
        foreach (Vector3Int position in firePositions)
        {
            // Assuming fires have a "Fire" tag
            GameObject[] existingFires = GameObject.FindGameObjectsWithTag("Fire");
            foreach (GameObject fire in existingFires)
            {
                Destroy(fire);
            }
        }
        firePositions.Clear();
    }

    private bool IsFarEnoughFromOtherFires(Vector3Int position)
    {
        foreach (Vector3Int firePos in firePositions)
        {
            if (Vector3Int.Distance(position, firePos) < minDistanceBetweenFires)
            {
                return false;
            }
        }
        return true;
    }
}
