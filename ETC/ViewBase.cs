using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 모든 View(항상 떠있는 전체화면 UI)들의 부모 클래스로 공통 기능 구현
/// 공통기능 : View간 화면전환, 전체화면 잠금, DimToDark, 공통 Tween 제어 등
/// </summary>

public class ViewBase : MonoBehaviour
{
    // 알파를 0으로 하면 화면 잠금이 안되는 현상이 있어서 최소한의 알파값을 줘야 화면 잠금을 할 수 있다.
    private const float minAlpha = 1f / 255f;
    private const float ViewFadeInOutDuration = 0.5f;

    // 최상단에 배치된 sprite
    // 전체화면 잠금, Dim과 같은 기능을 수행하는데 사용
    [SerializeField] UISprite TopOverlay = null;

    protected virtual void Start()
    {
        // 처음 View 시작시 FadeIn으로 효과
        LockUI();
        FadeInOutTopOverlay(1, 0, ViewFadeInOutDuration, () => 
        {
            // FadeIn으로 재생 완료되면 화면잠금 해제
            UnLockUI();
        });
    }

    protected void LockUI()
    {
        TopOverlay.alpha = Mathf.Max(minAlpha, TopOverlay.alpha);
        TopOverlay.GetComponent<BoxCollider2D>().enabled = true;
    }
    protected void UnLockUI()
    {
        TopOverlay.GetComponent<BoxCollider2D>().enabled = false;
    }
    protected void DarkToDim()
    {
        LockUI();
        TopOverlay.alpha = 0.5f;
    }
    protected void BackFromDim()
    {
        TopOverlay.alpha = minAlpha;
        UnLockUI();
    }

    /// <summary>
    /// 전체 화면 어둡게 또는 밝게 처리(Fade In out)
    /// </summary>
    /// <param name="alphaFrom">알파시작값</param>
    /// <param name="alphaTo">알파끝값</param>
    /// <param name="duration">재생시간</param>
    /// <param name="onEndFading">재생완료후 이벤트 콜백</param>
    protected void FadeInOutTopOverlay(float alphaFrom, float alphaTo, float duration, Action onEndFading = null)
    {
        // 알파를 0으로 하면 화면 잠금이 풀리기 때문에 알파를 0으로 주면 안됨다.
        alphaFrom = Mathf.Max(minAlpha, alphaFrom);
        alphaTo = Mathf.Max(minAlpha, alphaTo);

        TopOverlay.color = new Color(0, 0, 0, alphaFrom);
        DOTween.To(() => TopOverlay.alpha, x => TopOverlay.alpha = x, alphaTo, duration)
        .OnComplete(() => onEndFading?.Invoke());
    }

    // View 화면 전환
    public void SwitchViewTo<T>() where T : ViewBase
    {
        if (typeof(T) == typeof(ViewCompanyLogo))
        {
            // View 전환시 FadeOut완료 후 전환
            LockUI();
            FadeInOutTopOverlay(0, 1, ViewFadeInOutDuration, () =>
            {
                SceneManager.LoadScene("0_Company_LoadingScene");
            });
        }
        else if (typeof(T) == typeof(ViewGameLoading))
        {
            // View 전환시 FadeOut완료 후 전환
            LockUI();
            FadeInOutTopOverlay(0, 1, ViewFadeInOutDuration, () =>
            {
                SceneManager.LoadScene("1_Game_LoadingScene");
            });
        }
        else if(typeof(T) == typeof(ViewMain))
        {
            // View 전환시 FadeOut완료 후 전환
            LockUI();
            FadeInOutTopOverlay(0, 1, ViewFadeInOutDuration, () =>
            {
                SceneManager.LoadScene("2_MainScene");
            });
        }
        else if (typeof(T) == typeof(ViewIngame))
        {
            // View 전환시 FadeOut완료 후 전환
            LockUI();
            FadeInOutTopOverlay(0, 1, ViewFadeInOutDuration, () =>
            {
                SceneManager.LoadScene("3_InGame");
            });
        }
    }
}
