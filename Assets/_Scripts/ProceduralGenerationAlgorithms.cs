using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGenerationAlgorithms 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);

        var previousposition = startPosition;

        for (int i=0;i< walkLength; i++)
        {

            var newPosition = previousposition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousposition = newPosition;

        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition ,int corriderLength)
    {
        List<Vector2Int> corrider = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corrider.Add(currentPosition);
        for (int i= 0; i < corriderLength; i++)
        {
            currentPosition += direction;
            corrider.Add(currentPosition);

        }
        return corrider;

    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit,int minWidth,int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count> 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >=minWidth)
            {
                if(Random.value < .5f)
                {
                    if(room.size.y >= minHeight * 2)
                    {
                        SplitHorizontaly(minHeight,roomsQueue,room);
                    }
                    else if(room.size.x >= minWidth *2 )
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }else if (room.size.x >= minWidth && room.size.y >=minHeight)
                    {
                        roomsList.Add(room);
                    }
                }else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontaly(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                    {
                        roomsList.Add(room);
                    }
                }

            }
        }
        return roomsList;

    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontaly(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);//
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}


public static class Direction2D
{
    //List definition 
    public static List<Vector2Int> cardinaleDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1),//UP
        new Vector2Int(1,0),//right
        new Vector2Int(0,-1),//Down
        new Vector2Int(-1,0)// left 
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1),//UP-right
        new Vector2Int(1,-1),//right-down
        new Vector2Int(-1,-1),//Down-left
        new Vector2Int(-1,1)// left-up
    };
    public static List<Vector2Int> eightDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0,1),//UP
        new Vector2Int(1,1),//UP-right
        new Vector2Int(1,0),//right
        new Vector2Int(1,-1),//right-down
        new Vector2Int(0,-1),//Down
        new Vector2Int(-1,-1),//Down-left
        new Vector2Int(-1,0),// left 
        new Vector2Int(-1,1)// left-up
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinaleDirectionsList[Random.Range(0, cardinaleDirectionsList.Count)];
    }

}
