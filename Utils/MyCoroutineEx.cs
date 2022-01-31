using System;
using System.Collections;
using UnityEngine;

public static class MyCoroutineEx
{
    public static void CoKillAll(this Transform comp)
    {
        MyCoroutineManager.Inst.StopMyCoroutine(comp);
    }
    public static void CoKill(this Transform comp, Coroutine coroutine)
    {
        MyCoroutineManager.Inst.StopMyCoroutine(comp, coroutine);
    }
    public static void CoKill(this Transform comp, string coroutineName)
    {
        MyCoroutineManager.Inst.StopMyCoroutine(comp, coroutineName);
    }

    public static Coroutine CoMoveTo(this Transform comp, Vector3 dest, float duration, Action end = null, string name = "")
    {
        return MyCoroutineManager.Inst.StartMyCoroutine(comp, MoveTest(comp, dest, duration, end), name);
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
}
