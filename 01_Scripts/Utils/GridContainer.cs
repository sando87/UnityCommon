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
    public float GridSize { get; private set; }
    public Vector2 WorldBase { get; private set; } = Vector2.zero;

    private GameObject mRootObject = null;
    private Dictionary<Vector2Int, GameObject> mItems = new Dictionary<Vector2Int, GameObject>();

    //초기 Grid 크기 및 위치 정보를 초기화 한다.
    public void Init(float gridSize, Vector2 baseWorldPos)
    {
        GridSize = gridSize;
        WorldBase = baseWorldPos;
        mRootObject = new GameObject();
        mRootObject.name = "GridRoot";
    }
    public IEnumerable<KeyValuePair<Vector2Int, GameObject>> Enums()
    {
        foreach (var item in mItems)
            yield return item;
    }
    //items요소를 넣으면 Grid형태로 관리하고 위치를 다음 위치에 배치한다.
    public void SnapAddItem(GameObject item, Vector2Int posIdx)
    {
        if(!mItems.ContainsKey(posIdx))
        {
            mItems[posIdx] = new GameObject(posIdx.ToString());
            mItems[posIdx].transform.SetParent(mRootObject.transform);
            mItems[posIdx].transform.position = ToPosition(posIdx.x, posIdx.y);
        }

        item.transform.SetParent(mItems[posIdx].transform);
        item.transform.localPosition = Vector3.zero;
    }
    public void SnapAddItem(GameObject item, Vector2 worldPos)
    {
        Vector2Int posIdx = ToIndex(worldPos.x, worldPos.y);
        if(!mItems.ContainsKey(posIdx))
        {
            mItems[posIdx] = new GameObject(posIdx.ToString());
            mItems[posIdx].transform.SetParent(mRootObject.transform);
            mItems[posIdx].transform.position = ToPosition(posIdx.x, posIdx.y);
        }

        item.transform.SetParent(mItems[posIdx].transform);
        item.transform.localPosition = Vector3.zero;
    }
    public void Reset()
    {
        if(mRootObject != null)
        {
            GameObject.Destroy(mRootObject);
            mRootObject = null;
        }
        GridSize = 1;
        WorldBase = Vector2.zero;
        mItems.Clear();
    }
    public GameObject GetRootFrame(Vector2Int posIdx)
    {
        if(!mItems.ContainsKey(posIdx))
        {
            return null;
        }

        return mItems[posIdx];
    }
    public GameObject GetRootFrame(float worldPosX, float worldPosY)
    {
        Vector2Int posIdx = ToIndex(worldPosX, worldPosY);
        if(!mItems.ContainsKey(posIdx))
        {
            return null;
        }

        return mItems[posIdx];
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
        int x = (int)((worldPosX - WorldBase.x) / GridSize);
        int y = (int)((worldPosY - WorldBase.y) / GridSize);
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
                GameObject rootFrame = GetRootFrame(x, y);
                if (rootFrame != null)
                {
                    foreach(Transform child in rootFrame.transform)
                        rets.Add(child.gameObject);
                }
            }
        }
        return rets.ToArray();
    }
    //중간 아이템 기준으로 8개 방향(상하좌우대각선)의 아이템을 반환하다.
    public GameObject GetNeighborRootFrame(int idxX, int idxY, CornerDirection pos)
    {
        int offIdxX = ((int)pos % 3) - 1;
        int offIdxY = ((int)pos / 3) - 1;
        return GetRootFrame(idxX + offIdxX, idxY + offIdxY);
    }
}