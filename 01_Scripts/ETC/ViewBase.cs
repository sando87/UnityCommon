using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] Image TopOverlay = null;

    protected virtual void Start()
    {
        // 처음 View 시작시 FadeIn으로 효과
        LockUI();
        TopOverlay.DOFade(0, 0.5f).From(1)
        .OnComplete(() => 
        {
            // FadeIn으로 재생 완료되면 화면잠금 해제
            UnLockUI();
        });
    }

    protected void LockUI()
    {
        TopOverlay.raycastTarget = true;
    }
    protected void UnLockUI()
    {
        TopOverlay.raycastTarget = false;
    }
    protected void DarkToDim()
    {
        LockUI();
        TopOverlay.color = new Color(0, 0, 0, 0.5f);
    }
    protected void BackFromDim()
    {
        TopOverlay.color = new Color(0, 0, 0, 0);
        UnLockUI();
    }

    // View 화면 전환
    public void SwitchViewTo<T>() where T : ViewBase
    {
    }

    public virtual void UpdateUIState()
    {

    }
}
