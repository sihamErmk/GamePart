using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureManager : MonoBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    public Tilemap floorTile;
    public float minTreasureDistance = 10f;

    private bool treasureGenerated = false;

    public void GenerateTreasure(Vector3Int playerStartPosition)
    {
        if (treasureGenerated)
        {
            Debug.LogWarning("Treasure has already been generated.");
            return;
        }

        BoundsInt bounds = floorTile.cellBounds;
        HashSet<Vector3Int> generatedPositions = new HashSet<Vector3Int>();

        Vector3Int treasurePosition = GetDistantPosition(bounds, generatedPositions, playerStartPosition);
        if (treasurePosition == Vector3Int.zero)
        {
            Debug.LogError("Failed to generate a distant position for the treasure.");
            return;
        }

        Vector3 worldPosition = floorTile.CellToWorld(treasurePosition) + floorTile.tileAnchor;
        Debug.Log("Treasure generated at: " + worldPosition);

        GameObject treasure = Instantiate(treasurePrefab, worldPosition, Quaternion.identity, transform);
        if (treasure != null)
        {
            Debug.Log("Treasure instantiated successfully.");
            treasureGenerated = true;
            FindObjectOfType<PlayerMoving>().SetTreasure(treasure.transform); // Set the treasure in PlayerMoving
            FindObjectOfType<PlayerMoving>().UpdateTreasurePosition(treasurePosition); // Update its position
        }
        else
        {
            Debug.LogError("Failed to instantiate treasure.");
        }
    }

    private Vector3Int GetDistantPosition(BoundsInt roomBounds, HashSet<Vector3Int> generatedPositions, Vector3Int playerPosition)
    {
        Vector3Int distantPosition;
        int attempts = 0;
        const int maxAttempts = 100;

        Debug.Log("Room bounds: " + roomBounds);
        Debug.Log("Player position: " + playerPosition);

        do
        {
            distantPosition = new Vector3Int(
                Random.Range(roomBounds.xMin, roomBounds.xMax),
                Random.Range(roomBounds.yMin, roomBounds.yMax),
                roomBounds.z
            );
            attempts++;
        }
        while ((generatedPositions.Contains(distantPosition) || !floorTile.HasTile(distantPosition) ||
                Vector3Int.Distance(distantPosition, playerPosition) < minTreasureDistance) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Max attempts reached, couldn't find a distant position for the treasure.");
            return Vector3Int.zero;
        }

        Debug.Log("Distant position found after " + attempts + " attempts: " + distantPosition);
        return distantPosition;
    }
}
