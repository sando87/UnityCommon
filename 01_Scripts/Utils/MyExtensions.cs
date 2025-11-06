using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

#endif

public static class MyExtensions
{
    public static void ExBitClear(this ref int val, int index)
    {
        val = val & ~(1 << index);
    }
    public static void ExBitSet(this ref int val, int index)
    {
        val = val | (1 << index);
    }
    public static bool ExIsBitSet(this int mask, int index)
    {
        return (mask & (1 << index)) > 0;
    }
    public static bool ExIsBitClear(this int mask, int index)
    {
        return !ExIsBitSet(mask, index);
    }

    public static T ExToEnum<T>(this string value)
    {
        // 변환 오류인 경우 디폴트 값 리턴
        if (!System.Enum.IsDefined(typeof(T), value))
        {
            return default;
        }
        return (T)System.Enum.Parse(typeof(T), value, true);
    }

    public static void ExSetMinimum(this ref int val, int minValue)
    {
        val = Mathf.Max(val, minValue);
    }
    public static void ExSetMaximum(this ref int val, int maxValue)
    {
        val = Mathf.Min(val, maxValue);
    }
    public static void ExSetMinimum(this ref float val, float minValue)
    {
        val = Mathf.Max(val, minValue);
    }
    public static void ExSetMaximum(this ref float val, float maxValue)
    {
        val = Mathf.Min(val, maxValue);
    }


    public static Vector3 ExRotateVector(this Vector3 vec, Vector3 axis, float degree)
    {
        return Quaternion.AngleAxis(degree, axis.normalized) * vec;
    }

    public static Vector3 ExClampRotate(this Vector3 vec, Vector3 refAxis, float maxDegree)
    {
        float deg = Vector3.Angle(vec, refAxis);
        if (deg < maxDegree)
            return vec;

        Vector3 upVec = Vector3.Cross(refAxis, vec);
        return refAxis.ExRotateVector(upVec, maxDegree);
    }

    public static string ExRemoveFileExtension(this string filename)
    {
        int idx = filename.Length - 1;
        for (; idx >= 0; idx--)
        {
            if (filename[idx].Equals('.'))
                break;
        }
        return idx > 0 ? filename.Substring(0, idx) : filename;
    }


    public static Transform ExFindChildAll(this Transform parent, int layerID)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.layer == layerID)
                return child;

            if (child.childCount > 0)
            {
                Transform ret = child.ExFindChildAll(layerID);
                if (ret != null)
                    return ret;
            }
        }
        return null;
    }

    public static Transform ExFindChildAll(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
                return child;

            if (child.childCount > 0)
            {
                Transform ret = child.ExFindChildAll(name);
                if (ret != null)
                    return ret;
            }
        }
        return null;
    }

    public static void ExFindChildAll(this Transform parent, string name, List<Transform> rets)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
                rets.Add(child);

            if (child.childCount > 0)
                child.ExFindChildAll(name, rets);
        }
    }

    public static void ExFindChildAll<T>(this Transform me, List<T> rets, bool skipSubPrefab = false) where T : Component
    {
#if UNITY_EDITOR
        if (skipSubPrefab)
        {
            // 해당 객체가 하위 프리팹이면
            if (PrefabUtility.IsAnyPrefabInstanceRoot(me.gameObject))
                return;
        }
#endif

        T[] comps = me.GetComponents<T>();
        foreach (T comp in comps)
            rets.Add(comp);

        foreach (Transform child in me)
        {
            child.ExFindChildAll<T>(rets, skipSubPrefab);
        }
    }



    public static T ExGetPrivatePropertyValue<T>(this object obj, string propName)
    {
        if (obj == null) return default;
        PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (pi == null) return default;
        return (T)pi.GetValue(obj, null);
    }
    public static T ExGetPrivateFieldValue<T>(this object obj, string propName)
    {
        if (obj == null) return default;
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) return default;
        return (T)fi.GetValue(obj);
    }
    public static void ExSetPrivatePropertyValue<T>(this object obj, string propName, T val)
    {
        Type t = obj.GetType();
        if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            return;
        t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
    }
    public static void ExSetPrivateFieldValue<T>(this object obj, string propName, T val)
    {
        if (obj == null) return;
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) return;
        fi.SetValue(obj, val);
    }
    public static void ExInvokePrivateMethod(this object obj, string methodName, object[] methodParam)
    {
        MethodInfo dynMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (dynMethod == null) return;
        dynMethod.Invoke(obj, methodParam);
    }


    // 동일한 스크린 좌표계의 rect 정보로 UI의 위치와 크기를 배치시킨다.
    public static void ExSetRect(this RectTransform uiRect, RectTransform targetUI)
    {
        uiRect.sizeDelta = targetUI.rect.size;
        uiRect.position = targetUI.position;
    }
    // UI상의 RectTransform 영역을 World공간의 Rect로 변환.
    public static Rect ExToWorldRect(this RectTransform uiRect, Camera worldCamera)
    {
        Rect ret = new Rect();
        Vector2 worldSize = MyUtils.ScreenSizeToWorldSize(uiRect.rect.size, worldCamera);
        ret.size = worldSize;
        ret.center = uiRect.transform.position;
        return ret;
    }
    // world좌표계의 rect 위치로 UI의 위치와 크기를 배치시킨다.
    public static void ExFromWorldRect(this RectTransform uiRect, Rect worldRect, Camera _cam)
    {
        // 실제 장비 해상도 기준의 스크린 좌표값
        Vector2 hardwareScreenSize = MyUtils.WorldSizeToScreenSize(worldRect.size, _cam);
        // 유니티상에 설정된 레퍼런스 해상도 기준의 스크린 좌표값으로 변환 작업
        Vector2 sizeDelta = MyUtils.ScreenSizeToCavausScaledSize(hardwareScreenSize, uiRect.GetComponentInParent<CanvasScaler>());
        // 현재 UI RectTransform의 Inspector상에 들어가는 width, height 최종값
        uiRect.sizeDelta = sizeDelta;
        uiRect.position = worldRect.center;
    }

    public static Rect ExLimitRectMovement(this Rect targetRect, Rect limitArea)
    {
        Rect retArea = targetRect;
        Vector2 cenPos = retArea.center;
        if (retArea.width < limitArea.width)
            cenPos.x = Mathf.Clamp(cenPos.x, limitArea.xMin + retArea.width * 0.5f, limitArea.xMax - retArea.width * 0.5f);
        else
            cenPos.x = limitArea.center.x;

        if (retArea.height < limitArea.height)
            cenPos.y = Mathf.Clamp(cenPos.y, limitArea.yMin + retArea.height * 0.5f, limitArea.yMax - retArea.height * 0.5f);
        else
            cenPos.y = limitArea.center.y;

        retArea.center = cenPos;
        return retArea;
    }


    public static void ExSetWorldPosX(this Transform tr, float val)
    {
        tr.position = new Vector3(val, tr.position.y, tr.position.z);
    }
    public static void ExSetWorldPosY(this Transform tr, float val)
    {
        tr.position = new Vector3(tr.position.x, val, tr.position.z);
    }
    public static void ExSetWorldPosZ(this Transform tr, float val)
    {
        tr.position = new Vector3(tr.position.x, tr.position.y, val);
    }
    public static void ExSetLocalPosX(this Transform tr, float val)
    {
        tr.localPosition = new Vector3(val, tr.localPosition.y, tr.localPosition.z);
    }
    public static void ExSetLocalPosY(this Transform tr, float val)
    {
        tr.localPosition = new Vector3(tr.localPosition.x, val, tr.localPosition.z);
    }
    public static void ExSetLocalPosZ(this Transform tr, float val)
    {
        tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, val);
    }
    public static void ExSetWorldPositionXY(this Transform tr, Vector2 val)
    {
        tr.position = new Vector3(val.x, val.y, tr.position.z);
    }
    public static void ExSetLocalPositionXY(this Transform tr, Vector2 val)
    {
        tr.localPosition = new Vector3(val.x, val.y, tr.localPosition.z);
    }
    public static Vector2 ExToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    public static Vector3 ExToVector3(this Vector2 vec, float z = 0)
    {
        return new Vector3(vec.x, vec.y, z);
    }
    public static void ExSetAlpha(this Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
    public static void ExSetAlpha(this SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
    public static void ExSetAlpha(this Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
    public static void ExSetAlpha(this TextMesh text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }











    public static Coroutine ExForAWhileCoroutine(this MonoBehaviour mono, float duration, Action<float> func)
    {
        return mono.StartCoroutine(CoExForSecondsCall(func, duration));
    }
    private static IEnumerator CoExForSecondsCall(Action<float> EventUpdate, float duration)
    {
        float eclipsedTime = 0;
        while (eclipsedTime < duration)
        {
            float rate = eclipsedTime / duration;
            EventUpdate?.Invoke(rate);
            yield return null;
            eclipsedTime += Time.deltaTime;
        }
        EventUpdate?.Invoke(1);
    }
    public static Coroutine ExRepeatCoroutine(this MonoBehaviour mono, float interval, Action func, int repeatCount = -1)
    {
        return mono.StartCoroutine(CoExRepeatCall(func, interval, repeatCount));
    }
    private static IEnumerator CoExRepeatCall(Action EventEnd, float interval, int repeatCount)
    {
        bool isInfiniteMode = repeatCount < 0;
        int count = 0;
        while (isInfiniteMode || count < repeatCount)
        {
            EventEnd?.Invoke();

            if (interval > 0)
                yield return newWaitForSeconds.Cache(interval);
            else
                yield return null;

            count++;
        }
    }
    public static void ExValueTweenCoroutine(this MonoBehaviour mono, float from, float to, float duration, Action<float> eventValue)
    {
        mono.StartCoroutine(CoExValueTween(from, to, duration, eventValue));
    }
    private static IEnumerator CoExValueTween(float from, float to, float duration, Action<float> eventValue)
    {
        float time = 0;
        float currentValue = from;
        while (time < duration)
        {
            float rate = time / duration;
            currentValue = from * (1 - rate) + to * (rate);
            eventValue?.Invoke(currentValue);
            yield return null;
            time += Time.deltaTime;
        }
        eventValue?.Invoke(to);
    }
    public static Coroutine ExConditionCoroutine(this MonoBehaviour mono, Func<bool> cond, Action func)
    {
        return mono.StartCoroutine(CoExConditionCall(cond, func));
    }
    private static IEnumerator CoExConditionCall(Func<bool> EventCondition, Action EventEnd)
    {
        yield return new WaitUntil(() => EventCondition.Invoke());
        EventEnd?.Invoke();
    }
    public static Coroutine ExAfterFrameCoroutine(this MonoBehaviour mono, Action func)
    {
        return mono.StartCoroutine(CoExAfterFrameCall(func));
    }
    private static IEnumerator CoExAfterFrameCall(Action EventEnd)
    {
        yield return null;
        EventEnd?.Invoke();
    }
    public static Coroutine ExDelayedCoroutine(this MonoBehaviour mono, float delay, Action func)
    {
        return mono.StartCoroutine(CoExDelayCall(func, delay));
    }
    private static IEnumerator CoExDelayCall(Action EventEnd, float delay)
    {
        yield return newWaitForSeconds.Cache(delay);
        EventEnd?.Invoke();
    }
    public static Coroutine ExDelayedCoroutineUnSacled(this MonoBehaviour mono, float delay, Action func)
    {
        return mono.StartCoroutine(CoExDelayCallUnSacled(func, delay));
    }
    private static IEnumerator CoExDelayCallUnSacled(Action EventEnd, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        EventEnd?.Invoke();
    }



    public static void ExSortRandomly<T>(this List<T> list)
    {
        if (list.Count <= 1)
            return;

        System.Random ran = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int idx = ran.Next(n + 1);
            T val = list[idx];
            list[idx] = list[n];
            list[n] = val;
        }
    }
    public static void ExSortInCloseOrder(this List<Transform> list, Vector3 refPositioin)
    {
        list.Sort((a, b) =>
        {
            return (a.position - refPositioin).sqrMagnitude > (b.position - refPositioin).sqrMagnitude ? 1 : -1;
        });
    }
    public static bool ExIsOutOfRange<T>(this T[] list, int index)
    {
        return index < 0 || index >= list.Length;
    }
    public static bool ExIsOutOfRange<T>(this List<T> list, int index)
    {
        return index < 0 || index >= list.Count;
    }



    public static Vector3 ExCenter(this BoxCollider box)
    {
        return box.transform.TransformPoint(box.center);
    }
    public static Vector3 ExExtents(this BoxCollider box)
    {
        Vector3 extents = box.ExFrontHead() - box.ExCenter();
        extents.x = Mathf.Abs(extents.x);
        extents.y = Mathf.Abs(extents.y);
        extents.z = 0.5f;
        return extents;
    }
    public static Rect ExToWorldRect(this BoxCollider box)
    {
        Rect rect = new Rect();
        rect.size = box.size;
        rect.center = box.ExCenter();
        return rect;
    }
    public static Rect ExToRect(this Bounds bounds)
    {
        Rect rect = new Rect();
        rect.size = bounds.size.ExToVector2();
        rect.center = bounds.center.ExToVector2();
        return rect;
    }
    public static Rect ExToRect(this BoxCollider2D col)
    {
        Rect rect = new Rect();
        rect.size = col.size;
        rect.center = col.transform.position.ExToVector2() + col.offset;
        return rect;
    }
    public static Vector3 ExFront(this BoxCollider box, float offset = 0)
    {
        float extentsX = (box.size.x * 0.5f) + offset;
        Vector3 localForwardPos = box.center + new Vector3(extentsX, 0, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExBack(this BoxCollider box, float offset = 0)
    {
        float extentsX = (box.size.x * 0.5f) + offset;
        Vector3 localForwardPos = box.center + new Vector3(-extentsX, 0, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExHead(this BoxCollider box, float offset = 0)
    {
        float extentsY = (box.size.y * 0.5f) + offset;
        Vector3 localForwardPos = box.center + new Vector3(0, extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExFoot(this BoxCollider box, float offset = 0)
    {
        float extentsY = (box.size.y * 0.5f) + offset;
        Vector3 localForwardPos = box.center + new Vector3(0, -extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExFrontHead(this BoxCollider box, float offsetX = 0, float offsetY = 0)
    {
        float extentsX = (box.size.x * 0.5f) + (offsetX);
        float extentsY = (box.size.y * 0.5f) + (offsetY);
        Vector3 localForwardPos = box.center + new Vector3(extentsX, extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExFrontFoot(this BoxCollider box, float offsetX = 0, float offsetY = 0)
    {
        float extentsX = (box.size.x * 0.5f) + (offsetX);
        float extentsY = (box.size.y * 0.5f) + (offsetY);
        Vector3 localForwardPos = box.center + new Vector3(extentsX, -extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExBackFoot(this BoxCollider box, float offsetX = 0, float offsetY = 0)
    {
        float extentsX = (box.size.x * 0.5f) + (offsetX);
        float extentsY = (box.size.y * 0.5f) + (offsetY);
        Vector3 localForwardPos = box.center + new Vector3(-extentsX, -extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ExBackHead(this BoxCollider box, float offsetX = 0, float offsetY = 0)
    {
        float extentsX = (box.size.x * 0.5f) + (offsetX);
        float extentsY = (box.size.y * 0.5f) + (offsetY);
        Vector3 localForwardPos = box.center + new Vector3(-extentsX, extentsY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Bounds ExGetWorldBounds2D(this BoxCollider box)
    {
        Vector2 corner1 = box.ExFrontHead();
        Vector2 corner2 = box.ExBackHead();
        Vector2 corner3 = box.ExFrontFoot();
        Vector2 corner4 = box.ExBackFoot();
        float minX = MyUtils.Min(corner1.x, corner2.x, corner3.x, corner4.x);
        float minY = MyUtils.Min(corner1.y, corner2.y, corner3.y, corner4.y);
        float maxX = MyUtils.Max(corner1.x, corner2.x, corner3.x, corner4.x);
        float maxY = MyUtils.Max(corner1.y, corner2.y, corner3.y, corner4.y);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, box.size.z);
        Bounds bounds = new Bounds(box.ExCenter(), size);
        return bounds;
    }



    public static Vector3 ExRight(this Bounds bound, float offset = 0)
    {
        return bound.center += (Vector3.right * (bound.extents.x + offset));
    }
    public static Vector3 ExLeft(this Bounds bound, float offset = 0)
    {
        return bound.center += (Vector3.left * (bound.extents.x + offset));
    }
    public static Vector3 ExTop(this Bounds bound, float offset = 0)
    {
        return bound.center += (Vector3.up * (bound.extents.y + offset));
    }
    public static Vector3 ExBottom(this Bounds bound, float offset = 0)
    {
        return bound.center += (Vector3.down * (bound.extents.y + offset));
    }
    public static Vector3 ExRightTop(this Bounds bound, float offsetX = 0, float offsetY = 0)
    {
        return bound.center += new Vector3(bound.extents.x + offsetX, bound.extents.y + offsetY, 0);
    }
    public static Vector3 ExLeftTop(this Bounds bound, float offsetX = 0, float offsetY = 0)
    {
        return bound.center += new Vector3(-(bound.extents.x + offsetX), bound.extents.y + offsetY, 0);
    }
    public static Vector3 ExRightBottom(this Bounds bound, float offsetX = 0, float offsetY = 0)
    {
        return bound.center += new Vector3(bound.extents.x + offsetX, -(bound.extents.y + offsetY), 0);
    }
    public static Vector3 ExLeftBottom(this Bounds bound, float offsetX = 0, float offsetY = 0)
    {
        return bound.center += new Vector3(-(bound.extents.x + offsetX), -(bound.extents.y + offsetY), 0);
    }

}

