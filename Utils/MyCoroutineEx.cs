using System;
using System.Collections;
using UnityEngine;

public static class MyCoroutineEx
{
    public static void CoKillAll(this Transform comp)
    {
        MyCoroutineManager.Instance.StopMyCoroutine(comp);
    }
    public static void CoKill(this Transform comp, Coroutine coroutine)
    {
        MyCoroutineManager.Instance.StopMyCoroutine(comp, coroutine);
    }
    public static void CoKill(this Transform comp, string coroutineName)
    {
        MyCoroutineManager.Instance.StopMyCoroutine(comp, coroutineName);
    }

    public static Coroutine CoMoveTo(this Transform comp, Vector3 dest, float duration, Action end = null, string name = "")
    {
        return MyCoroutineManager.Instance.StartMyCoroutine(comp, MoveTest(comp, dest, duration, end), name);
    }
    public static Coroutine CoMoveTo(this Transform comp, Transform target, float duration, Action end = null, string name = "")
    {
        return MyCoroutineManager.Instance.StartMyCoroutine(comp, MoveTarget(comp, target, duration, end), name);
    }

    public static IEnumerator MoveTest(Transform obj, Vector3 dest, float duration, Action EventEnd = null)
    {
        Vector3 startPos = obj.position;
        float time = 0;
        while (time < duration)
        {
            float rate = time / duration;
            obj.position = startPos * (1 - rate) + dest * rate;
            time += Time.deltaTime;
            yield return null;
        }
        EventEnd?.Invoke();
    }
    public static IEnumerator MoveTarget(Transform obj, Transform target, float duration, Action EventEnd = null)
    {
        Vector3 startPos = obj.position;
        Vector3 destPos = target.position;
        float time = 0;
        while (time < duration)
        {
            float rate = time / duration;
            destPos = target != null ? target.position : destPos;
            obj.position = startPos * (1 - rate) + destPos * rate;
            yield return null;
            time += Time.deltaTime;
        }
        EventEnd?.Invoke();
    }
}
