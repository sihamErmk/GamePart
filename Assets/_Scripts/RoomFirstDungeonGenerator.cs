using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenrator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField][Range(0, 10)] public int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;
    private List<BoundsInt> roomOrigins = new List<BoundsInt>();

    private CoinManager coinManager; // Reference to CoinManager
    private EnemyManager enemyManager; // Reference to EnemyManager

    public BoundsInt FirstRoom { get; private set; }
    public BoundsInt LastRoom { get; private set; }

    private void Awake()
    {
        coinManager = FindObjectOfType<CoinManager>(); // Find the CoinManager object in the scene
        enemyManager = FindObjectOfType<EnemyManager>(); // Find the EnemyManager object in the scene
    }

    public void PlayRunProceduralGeneration()
    {
        tileMapVisulizer.Clear();
        RunProceduralGenartion();
        GenerateCoinsInDungeon();
        GenerateEnemiesInDungeon();
    }

    protected override void RunProceduralGenartion()
    {
        CreateRooms();
        GenerateCoinsInDungeon();
        GenerateEnemiesInDungeon();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = randomWalkRooms ? CreateRoomsRandomly(roomsList) : CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tileMapVisulizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tileMapVisulizer);

        roomOrigins = roomsList; // Store room origins for enemy and coin generation

        // Identify the first and last room
        if (roomsList.Count > 0)
        {
            FirstRoom = roomsList[0];
            LastRoom = roomsList[roomsList.Count - 1];
        }
    }

    private void GenerateCoinsInDungeon()
    {
        if (coinManager != null)
        {
            foreach (var room in roomOrigins)
            {
                coinManager.GenerateCoinsInRoom(room); // Generate coins in each room
            }
        }
        else
        {
            Debug.LogWarning("CoinManager not found in the scene.");
        }
    }

    private void GenerateEnemiesInDungeon()
    {
        if (enemyManager != null)
        {
            HashSet<Vector3Int> coinPositions = new HashSet<Vector3Int>();

            // Collect all coin positions
            foreach (var room in roomOrigins)
            {
                for (int x = room.xMin; x < room.xMax; x++)
                {
                    for (int y = room.yMin; y < room.yMax; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, room.z);
                        if (coinManager.floorTile.HasTile(position))
                        {
                            coinPositions.Add(position);
                        }
                    }
                }
            }

            foreach (var room in roomOrigins)
            {
                enemyManager.GenerateEnemiesInRoom(room, coinPositions); // Generate enemies in each room
            }
        }
        else
        {
            Debug.LogWarning("EnemyManager not found in the scene.");
        }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset)
                    && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            position.y += (destination.y > position.y) ? 1 : -1;
            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            position.x += (destination.x > position.x) ? 1 : -1;
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    floor.Add((Vector2Int)room.min + new Vector2Int(col, row));
                }
            }
        }

        return floor;
    }
}
