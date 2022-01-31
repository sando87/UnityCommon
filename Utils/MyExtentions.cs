using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class MyExtentions
{
    public static void DoColor(this UISprite obj, Color toColor, float duration, System.Action eventEnd = null)
    {
        toColor.a = obj.alpha;
        DOTween.To(() => obj.color, x => obj.color = x, toColor, duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoAlpha(this UISprite obj, float toAlpha, float duration, System.Action eventEnd = null)
    {
        DOTween.To(() => obj.alpha, x => obj.alpha = x, toAlpha, duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoFillAmount(this UISprite obj, float fromValue, float toValue, float duration, System.Action eventEnd = null)
    {
        DOTween.To(() => obj.fillAmount, x => obj.fillAmount = x, toValue, duration)
        .From(fromValue)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoAlpha(this UILabel obj, float toAlpha, float duration, System.Action eventEnd = null)
    {
        DOTween.To(() => obj.alpha, x => obj.alpha = x, toAlpha, duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoNumber(this UILabel obj, int fromNumber, int toNumber, float duration, System.Action eventEnd = null)
    {
        int curNum = fromNumber;
        DOTween.To(
            () => curNum, 
            x => 
            { 
                curNum = x;
                obj.text = MyUtils.ToCommaEveryThree(curNum);
            }, 
            toNumber, 
            duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoAlpha(this UIWidget obj, float toAlpha, float duration, System.Action eventEnd = null)
    {
        DOTween.To(() => obj.alpha, x => obj.alpha = x, toAlpha, duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
    public static void DoValue(this UISlider obj, float toValue, float duration, System.Action eventEnd = null)
    {
        DOTween.To(() => obj.value, x => obj.value = x, toValue, duration)
        .OnComplete(() =>
        {
            eventEnd?.Invoke();
        });
    }
}
