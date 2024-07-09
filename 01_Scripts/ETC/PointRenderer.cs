using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PointRenderer : MonoBehaviour 
{
    [SerializeField] SpriteRenderer PointPrefab = null;

    private Color mColor = Color.white;
    private float mSize = 1;
    
    public static PointRenderer Draw(Vector3[] points, Color color, float size)
    {
        PointRenderer pointRendererPrefab = Resources.Load<PointRenderer>("Utilities/PointRenderer");
        PointRenderer rootObject = Instantiate(pointRendererPrefab, points[0], Quaternion.identity);
        rootObject.mColor = color;
        rootObject.mSize = size;

        foreach(Vector3 point in points)
        {
            rootObject.CreatePoint(point);
        }

        return rootObject;
    }

    public static PointRenderer DrawLocal(Vector3[] localPoints, Color color, float size, Transform root)
    {
        PointRenderer pointRendererPrefab = Resources.Load<PointRenderer>("Utilities/PointRenderer");
        PointRenderer rootObject = Instantiate(pointRendererPrefab, root);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.mColor = color;
        rootObject.mSize = size;

        foreach(Vector3 point in localPoints)
        {
            rootObject.CreateLocalPoint(point);
        }

        return rootObject;
    }
    
    
    public void ShowPoints(int startIndex, int count)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            bool isShow = startIndex <= i && i < (startIndex + count);
            transform.GetChild(i).gameObject.SetActive(isShow);
        }
    }

    public void UpdatePoints(Vector3[] points)
    {
        int maxCount = Mathf.Max(points.Length, transform.childCount);
        for(int i = 0; i < maxCount; ++i)
        {
            if(i < points.Length && i < transform.childCount)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                transform.GetChild(i).position = points[i];
            }
            else if(i >= points.Length && i < transform.childCount)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            else if(i < points.Length && i >= transform.childCount)
            {
                CreatePoint(points[i]);
            }
        }
    }

    private SpriteRenderer CreatePoint(Vector3 point)
    {
        SpriteRenderer sr = Instantiate(PointPrefab, point, Quaternion.identity, transform);
        sr.color = mColor;
        sr.transform.localScale = new Vector3(mSize, mSize, mSize);
        return sr;
    }

    private SpriteRenderer CreateLocalPoint(Vector3 localPt)
    {
        SpriteRenderer sr = Instantiate(PointPrefab, transform);
        sr.transform.localPosition = localPt;
        sr.transform.localScale = new Vector3(mSize, mSize, mSize);
        sr.color = mColor;
        return sr;
    }

    public void Disappear(bool isReverse = false)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            int idx = isReverse ? transform.childCount - 1 - i : i;
            transform.GetChild(idx).DOScale(0, 0.3f).From(mSize).SetDelay(0.05f * i);
        }
    }
    
    public void Appear(bool isReverse = false)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            int idx = isReverse ? transform.childCount - 1 - i : i;
            transform.GetChild(idx).DOScale(mSize, 0.3f).From(0).SetDelay(0.05f * i);
        }
    }
    
    // 궤적 생성
    // 호출시 1회만 흘러가는 느낌을 주기때문에 주기적으로 호출해줘야 함
    public void Flow(bool isReverse = false)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            int idx = isReverse ? transform.childCount - 1 - i : i;
            transform.GetChild(idx).localScale = Vector3.zero;
            transform.GetChild(idx).DOKill();
            transform.GetChild(idx).DOScale(mSize, 0.2f).From(0).SetLoops(2, LoopType.Yoyo).SetDelay(0.036f * i).SetEase(Ease.InOutQuint);
        }
    }

    public Vector3[] GetWorldPoints()
    {
        List<Vector3> rets = new List<Vector3>();
        foreach(Transform point in transform)
            rets.Add(point.position);

        return rets.ToArray();
    }
}