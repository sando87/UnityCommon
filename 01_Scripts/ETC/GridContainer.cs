using System;
using System.Collections.Generic;
using UnityEngine;

public class GridContainer
{
    public float GridSize { get; private set; } = 1;
    public Vector2 WorldBase { get; private set; } = Vector2.zero;

    public GridContainer(float gridSize, Vector2 worldBase)
    {
        GridSize = gridSize;
        WorldBase = worldBase;
    }

    //초기 Grid 크기 및 위치 정보를 초기화 한다.
    public void Init(float gridSize, Vector2 baseWorldPos)
    {
        GridSize = gridSize;
        WorldBase = baseWorldPos;
    }
    public Vector2 ToPosition(int idxX, int idxY)
    {
        float x = WorldBase.x + idxX * GridSize;
        float y = WorldBase.y + idxY * GridSize;
        return new Vector2(x, y);
    }
    public Vector2 ToPosition(Vector2Int posIdx)
    {
        return ToPosition(posIdx.x, posIdx.y);
    }
    public Vector2Int ToIndex(Vector2 worldPos)
    {
        return ToIndex(worldPos.x, worldPos.y);
    }
    public Vector2Int ToIndex(float worldPosX, float worldPosY)
    {
        float offsetX = worldPosX - WorldBase.x;
        float offsetY = worldPosY - WorldBase.y;
        int x = offsetX >= 0 ? (int)(offsetX / GridSize) : (int)(offsetX / GridSize) - 1;
        int y = offsetY >= 0 ? (int)(offsetY / GridSize) : (int)(offsetY / GridSize) - 1;
        return new Vector2Int(x, y);
    }
    public Vector2Int SnapToCloseIndex(float worldPosX, float worldPosY)
    {
        return ToIndex(worldPosX + (GridSize * 0.5f), worldPosY + (GridSize * 0.5f));
    }
    public Vector2Int[] GetIndicies(Rect worldArea)
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
    public Vector2Int[] GetNeighbors4Way(Vector2Int center)
    {
        List<Vector2Int> rets = new List<Vector2Int>();
        rets.Add(center + new Vector2Int(1, 0));
        rets.Add(center + new Vector2Int(0, 1));
        rets.Add(center + new Vector2Int(-1, 0));
        rets.Add(center + new Vector2Int(0, -1));
        return rets.ToArray();
    }
    public IEnumerable<Vector2Int> GetNeighbors4WayEnum(Vector2Int center)
    {
        yield return (center + new Vector2Int(1, 0));
        yield return (center + new Vector2Int(0, 1));
        yield return (center + new Vector2Int(-1, 0));
        yield return (center + new Vector2Int(0, -1));
    }
    public Vector2Int[] GetNeighbors8Way(Vector2Int center)
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
    public void Reset()
    {
        GridSize = 1;
        WorldBase = Vector2.zero;
    }
}