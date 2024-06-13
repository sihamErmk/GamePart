using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab; // Prefab for the coin object
    public int maxCoinsPerRoom = 5; // Maximum number of coins per room
    public Tilemap floorTile;

    private HashSet<Vector3Int> coinPositions = new HashSet<Vector3Int>();

    // Function to generate coins in a room
    public void GenerateCoinsInRoom(BoundsInt roomBounds)
    {
        Debug.Log("Generating coins in room...");

        ClearCoins(); // Remove existing coins before generating new ones
        coinPositions.Clear(); // Clear coin positions

        HashSet<Vector3Int> generatedPositions = new HashSet<Vector3Int>();

        for (int i = 0; i < maxCoinsPerRoom; i++)
        {
            Vector3Int randomPosition = GetRandomPositionInsideRoom(roomBounds, generatedPositions);

            // Debug log the random position
            Debug.Log("Coin spawn position: " + randomPosition);

            // Instantiate the coinPrefab at the random position within the room
            GameObject coin = Instantiate(coinPrefab, floorTile.CellToWorld(randomPosition) + floorTile.tileAnchor, Quaternion.identity, transform);

            // Track generated positions to avoid duplicates
            generatedPositions.Add(randomPosition);
            coinPositions.Add(randomPosition);

            // Attach a CoinScript to handle coin interaction
            CoinScript coinScript = coin.GetComponent<CoinScript>();
            if (coinScript == null)
            {
                coinScript = coin.AddComponent<CoinScript>();
            }
            coinScript.SetCoinManager(this); // Pass reference to the CoinManager
        }
    }

    // Function to get a random position inside the room bounds
    private Vector3Int GetRandomPositionInsideRoom(BoundsInt roomBounds, HashSet<Vector3Int> generatedPositions)
    {
        Vector3Int randomPosition;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            randomPosition = new Vector3Int(
                Random.Range(roomBounds.xMin + 1, roomBounds.xMax - 1),
                Random.Range(roomBounds.yMin + 1, roomBounds.yMax - 1),
                roomBounds.z  // Use the z position of the room bounds
            );
            attempts++;
        }
        while (generatedPositions.Contains(randomPosition) || !floorTile.HasTile(randomPosition) && attempts < maxAttempts);

        // If the loop exceeds maxAttempts, it means it couldn't find a valid position.
        // You can handle it as needed, here we'll just log a warning.
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Max attempts reached, couldn't find a valid position for the coin.");
        }

        return randomPosition;
    }

    // Function to clear existing coins
    private void ClearCoins()
    {
        GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in existingCoins)
        {
            Destroy(coin); // Destroy the coin GameObject
        }
    }

    // Method to initialize dynamic programming calculation
    public int MaxCoinsFromStart()
    {
        BoundsInt bounds = floorTile.cellBounds;
        HashSet<Vector3Int> allCoinPositions = GetCoinPositions();

        // Start dynamic programming from each coin position
        int maxCoins = 0;
        foreach (Vector3Int coinPosition in allCoinPositions)
        {
            HashSet<Vector3Int> remainingCoins = new HashSet<Vector3Int>(allCoinPositions);
            remainingCoins.Remove(coinPosition);

            int coinsFromStart = MaxCoinsFromState(coinPosition, remainingCoins);
            if (coinsFromStart > maxCoins)
                maxCoins = coinsFromStart;
        }

        return maxCoins;
    }

    private Dictionary<Vector3Int, int> memoization = new Dictionary<Vector3Int, int>();

    // Method to calculate the maximum number of coins from a given state
    public int MaxCoinsFromState(Vector3Int playerPosition, HashSet<Vector3Int> remainingCoins)
    {
        // Base case: If no remaining coins, return 0
        if (remainingCoins.Count == 0)
            return 0;

        // Check if the result is already memoized
        if (memoization.ContainsKey(playerPosition))
            return memoization[playerPosition];

        int maxCoins = 0;

        // Iterate through all possible next positions
        foreach (Vector3Int nextPosition in remainingCoins)
        {
            // Simulate collecting the coin at nextPosition
            HashSet<Vector3Int> nextRemainingCoins = new HashSet<Vector3Int>(remainingCoins);
            nextRemainingCoins.Remove(nextPosition);

            // Calculate the maximum number of coins from the next state
            int coinsFromNextState = MaxCoinsFromState(nextPosition, nextRemainingCoins);

            // Update maxCoins if collecting the coin at nextPosition yields more coins
            if (coinsFromNextState + 1 > maxCoins)
                maxCoins = coinsFromNextState + 1;
        }

        // Memoize the result
        memoization[playerPosition] = maxCoins;

        return maxCoins;
    }

    // Method to handle coin collection
    public void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        // Update score or UI elements here
        Debug.Log("Coin collected!");
    }
    public void CoinCollected(Vector3 position)
    {
        // Handle coin collection logic here
        Debug.Log("Coin collected at position: " + position);
    }

    // Method to remove a coin position
    private void RemoveCoinPosition(Vector3Int position)
    {
        coinPositions.Remove(position);
    }

    // Method to get coin positions
    public HashSet<Vector3Int> GetCoinPositions()
    {
        return new HashSet<Vector3Int>(coinPositions);
    }

    private void Start()
    {
        BoundsInt bounds = floorTile.cellBounds;
        GenerateCoinsInRoom(bounds);

    }
}
