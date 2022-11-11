using System;
using System.Collections.Generic;
using UnityEngine;

public enum CornerDirection
{
    LT, Top, RT,
    LC, Center, RC,
    LB, Bottom, RB
}

public class GridContainer
{
    public int Count { get { return CountX * CountY; } }
    
    public int FromIndexX { get { return MinIndex.x; } }
    public int ToIndexX { get { return MaxIndex.x; } }
    public int FromIndexY { get { return MinIndex.y; } }
    public int ToIndexY { get { return MaxIndex.y; } }

    public int CountX { get { return MaxIndex.x - MinIndex.x + 1; } }
    public int CountY { get { return MaxIndex.y - MinIndex.y + 1; } }
    
    public float GridSize { get; private set; }
    
    public Rect Area 
    { 
        get
        {
            Rect area = new Rect();
            Vector2 halfGridSize = new Vector2(GridSize, GridSize) * 0.5f;
            Vector2 minPos = ToPosition(MinIndex.x, MinIndex.y) - halfGridSize;
            Vector2 maxPos = ToPosition(MaxIndex.x, MaxIndex.y) + halfGridSize;
            area.min = minPos;
            area.max = maxPos;
            return area;
        } 
    }
    public Vector2 WorldBase { get; private set; } = Vector2.zero;

    private Dictionary<Vector2Int, GameObject> mItems = new Dictionary<Vector2Int, GameObject>();
    public Vector2Int MinIndex { get; private set; } = Vector2Int.zero;
    public Vector2Int MaxIndex { get; private set; } = Vector2Int.zero;

    //초기 Grid 크기 및 위치 정보를 초기화 한다.
    public void Init(Vector2Int minIndex, Vector2Int maxIndex, float gridSize, Vector2 baseWorldPos)
    {
        GridSize = gridSize;
        WorldBase = baseWorldPos;
        MinIndex = minIndex;
        MaxIndex = maxIndex;
    }
    //items요소를 넣으면 Grid형태로 관리하고 위치를 다음 위치에 배치한다.
    public void SetItem(GameObject item, int idxX, int idxY)
    {
        if(idxX < MinIndex.x || MaxIndex.x < idxX || idxY < MinIndex.y || MaxIndex.y < idxY) 
        {
            LOG.warn();
            return;
        }

        mItems[new Vector2Int(idxX, idxY)] = item;
        item.transform.localPosition = ToPosition(idxX, idxY);
    }
    public void ExtendRight(int count)
    {
        MaxIndex = new Vector2Int(Math.Max(MinIndex.x, MaxIndex.x + count), MaxIndex.y);
    }
    public void ExtendLeft(int count)
    {
        MinIndex = new Vector2Int(Math.Min(MaxIndex.x, MinIndex.x - count), MinIndex.y);
    }
    public void ExtendTop(int count)
    {
        MaxIndex = new Vector2Int(MaxIndex.x, Math.Max(MinIndex.y, MaxIndex.y + count));
    }
    public void ExtendBottom(int count)
    {
        MinIndex = new Vector2Int(MinIndex.x, Math.Min(MaxIndex.y, MinIndex.y - count));
    }
    public void Reset()
    {
        GridSize = 1;
        WorldBase = Vector2.zero;
        MinIndex = Vector2Int.zero;
        MaxIndex = Vector2Int.zero;
        mItems.Clear();
    }
    public GameObject GetItem(int idxX, int idxY)
    {
        if (idxX < MinIndex.x || MaxIndex.x < idxX || idxY < MinIndex.y || MaxIndex.y < idxY)
        {
            LOG.warn();
            return null;
        }

        if(!mItems.ContainsKey(new Vector2Int(idxX, idxY)))
        {
            return null;
        }

        return mItems[new Vector2Int(idxX, idxY)];
    }
    public Vector2 ToPosition(int idxX, int idxY)
    {
        float x = WorldBase.x + idxX * GridSize;
        float y = WorldBase.y + idxY * GridSize;
        return new Vector2(x, y);
    }
    public Vector2Int ToIndex(float worldPosX, float worldPosY)
    {
        worldPosX += GridSize * 0.5f;
        worldPosY += GridSize * 0.5f;
        if (worldPosX < 0) worldPosX -= GridSize;
        if (worldPosY < 0) worldPosY -= GridSize;
        int x = (int)(worldPosX - WorldBase.x);
        int y = (int)(worldPosY - WorldBase.y);
        return new Vector2Int(x, y);
    }
    //주변 Grid 아이템들을 반환한다.
    public GameObject[] GetAroundItems(int idxX, int idxY)
    {
        List<GameObject> rets = new List<GameObject>();
        for (int y = idxY - 1; y < idxY + 2; ++y)
        {
            for (int x = idxX - 1; x < idxX + 2; ++x)
            {
                if (x == idxX && y == idxY) continue;
                GameObject item = GetItem(x, y);
                if (item != null)
                    rets.Add(item);
            }
        }
        return rets.ToArray();
    }
    //중간 아이템 기준으로 8개 방향(상하좌우대각선)의 아이템을 반환하다.
    public GameObject GetItem(int idxX, int idxY, CornerDirection pos)
    {
        int offIdxX = ((int)pos % 3) - 1;
        int offIdxY = ((int)pos / 3) - 1;
        return GetItem(idxX + offIdxX, idxY + offIdxY);
    }
}