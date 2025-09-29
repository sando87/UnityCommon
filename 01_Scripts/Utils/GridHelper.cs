using System;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static float GridSize { get; set; } = 1.0f;
    public static Vector2 WorldBase { get; set; } = Vector2.zero;

    public static Vector2 ToPosition(int idxX, int idxY)
    {
        float x = WorldBase.x + idxX * GridSize;
        float y = WorldBase.y + idxY * GridSize;
        return new Vector2(x, y);
    }
    public static Vector2 ToPosition(Vector2Int posIdx)
    {
        return ToPosition(posIdx.x, posIdx.y);
    }
    public static Vector2Int ToIndex(Vector2 worldPos)
    {
        return ToIndex(worldPos.x, worldPos.y);
    }
    public static Vector2Int ToIndex(float worldPosX, float worldPosY)
    {
        int idxX = Mathf.FloorToInt((worldPosX - WorldBase.x) / GridSize);
        int idxY = Mathf.FloorToInt((worldPosY - WorldBase.y) / GridSize);
        return new Vector2Int(idxX, idxY);
    }
    public static Vector2Int[] GetIndicies(Rect worldArea)
    {
        List<Vector2Int> rets = new List<Vector2Int>();

        Vector2Int minIdx = ToIndex(worldArea.min);
        Vector2Int maxIdx = ToIndex(worldArea.max);
        for (int y = minIdx.y; y <= maxIdx.y; y++)
        {
            for (int x = minIdx.x; x <= maxIdx.x; x++)
            {
                rets.Add(new Vector2Int(x, y));
            }
        }
        return rets.ToArray();
    }
    public static IEnumerable<Vector2Int> GetIndiciesEnum(Rect worldArea)
    {
        Vector2Int minIdx = ToIndex(worldArea.min);
        Vector2Int maxIdx = ToIndex(worldArea.max);
        for (int y = minIdx.y; y <= maxIdx.y; y++)
        {
            for (int x = minIdx.x; x <= maxIdx.x; x++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }
    public static Vector2Int[] GetNeighbors4Way(Vector2Int center)
    {
        List<Vector2Int> rets = new List<Vector2Int>();
        rets.Add(center + new Vector2Int(1, 0));
        rets.Add(center + new Vector2Int(0, 1));
        rets.Add(center + new Vector2Int(-1, 0));
        rets.Add(center + new Vector2Int(0, -1));
        return rets.ToArray();
    }
    public static IEnumerable<Vector2Int> GetNeighbors4WayEnum(Vector2Int center)
    {
        yield return (center + new Vector2Int(1, 0));
        yield return (center + new Vector2Int(0, 1));
        yield return (center + new Vector2Int(-1, 0));
        yield return (center + new Vector2Int(0, -1));
    }
    public static Vector2Int[] GetNeighbors8Way(Vector2Int center)
    {
        List<Vector2Int> rets = new List<Vector2Int>();
        rets.Add(center + new Vector2Int(1, 0));
        rets.Add(center + new Vector2Int(1, 1));
        rets.Add(center + new Vector2Int(0, 1));
        rets.Add(center + new Vector2Int(-1, 1));
        rets.Add(center + new Vector2Int(-1, 0));
        rets.Add(center + new Vector2Int(-1, -1));
        rets.Add(center + new Vector2Int(0, -1));
        rets.Add(center + new Vector2Int(1, -1));
        return rets.ToArray();
    }
}