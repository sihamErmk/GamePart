using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator 
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions,TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirection(floorPositions, Direction2D.cardinaleDirectionsList);
        var cornerWallPositions = FindWallsInDirection(floorPositions, Direction2D.diagonalDirectionsList);
        CraeteBasucWall(tilemapVisualizer, basicWallPositions,floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach(var position in cornerWallPositions)
        {
            string neghiboursBinaryType = "";
            foreach(var direction in Direction2D.eightDirectionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neghiboursBinaryType += "1";
                }
                else
                {
                    neghiboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PainSingleWallCorner(position, neghiboursBinaryType);

        }
    }

    private static void CraeteBasucWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {

        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach(var direction in Direction2D.cardinaleDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PainSingleBasciWall(position,neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirection(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach(var position in floorPositions)
        {
            foreach(var direction in directionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition)==false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
        
    }
}
