using System;
using System.Collections.Generic;
using UnityEngine;

public enum CornerDirection
{
    LT, Top, RT,
    LC, Center, RC,
    LB, Bottom, RB
}

public class GridContainer<T> where T : MonoBehaviour
{
    private T[] mItems = null;
    private const float yGap = 0.08f;
    public int Count { get { return CountX * CountY; } }
    public int CountX { get; private set; }
    public int CountY { get; private set; }
    public float GridSize { get; private set; }
    public Rect Area { get; private set; }
    public Vector2 WorldBase { get; private set; } = Vector2.zero;
    public T[] GetItems() { return mItems; }
    //초기 Grid 크기 및 위치 정보를 초기화 한다.
    public void Init(int countX, int countY, float gridSize, Vector2 baseWorldPos)
    {
        CountX = countX;
        CountY = countY;
        GridSize = gridSize;
        WorldBase = baseWorldPos;
        Rect area = new Rect();
        area.size = new Vector2(gridSize * CountX, gridSize * CountY - (yGap * (CountY - 1)));
        area.center = WorldBase;
        Area = area;
        mItems = new T[CountX * CountY];
    }
    //items요소를 넣으면 Grid형태로 관리하고 위치를 다음 위치에 배치한다.
    public void SetItem(T item, int idxX, int idxY)
    {
        int idx = ToIndex(idxX, idxY);
        item.transform.localPosition = ToPosition(idxX, idxY);
        item.transform.localScale = new Vector3(GridSize, GridSize, 1);
        mItems[idx] = item;
    }
    public void Reset()
    {
        CountX = 0;
        CountY = 0;
        GridSize = 1;
        WorldBase = Vector2.zero;
        Area = new Rect();
        mItems = null;
    }
    public int ToIndex(int idxX, int idxY) { return idxY * CountX + idxX; }
    public int ToIndexX(int idx) { return idx % CountX; }
    public int ToIndexY(int idx) { return idx / CountX; }
    public T GetItem(int idx)
    {
        if (idx < 0 || idx >= Count) return null;
        return mItems[idx];
    }
    public T GetItem(int idxX, int idxY)
    {
        if (idxX < 0 || idxX >= CountX || idxY < 0 || idxY >= CountY) return null;
        return mItems[ToIndex(idxX, idxY)];
    }
    public Vector3 ToPosition(int idxX, int idxY)
    {
        Vector3 off = new Vector3(Area.xMin + GridSize * 0.5f, Area.yMax - GridSize * 0.5f, 0);
        float x = idxX * GridSize;
        float y = idxY * GridSize - (yGap * idxY);
        float z = -yGap * idxY;
        return off + new Vector3(x, -y, z); //윗줄부터 0번째 줄로 Top-Down 방향으로 하기 위한 좌표계 변환
    }
    //주변 Grid 아이템들을 반환한다.
    public T[] GetAroundItems(int idxX, int idxY)
    {
        List<T> rets = new List<T>();
        for (int y = idxY - 1; y < idxY + 2; ++y)
        {
            for (int x = idxX - 1; x < idxX + 2; ++x)
            {
                if (x == idxX && y == idxY) continue;
                T item = GetItem(x, y);
                if (item != null)
                    rets.Add(item);
            }
        }
        return rets.ToArray();
    }
    //중간 아이템 기준으로 8개 방향(상하좌우대각선)의 아이템을 반환하다.
    public T GetItem(int idxX, int idxY, CornerDirection pos)
    {
        int offIdxX = ((int)pos % 3) - 1;
        int offIdxY = ((int)pos / 3) - 1;
        return GetItem(idxX + offIdxX, idxY + offIdxY);
    }
}