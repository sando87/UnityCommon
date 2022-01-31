using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 각 Popup들의 부모 클래스
/// 각 Popup들의 공통 기능들은 이곳에서 처리한다.
/// 공통기능 : 전체화면 잠금, 뒤로가기(닫기), 생성 및 소멸시 애니메이션 효과, FadeInOut(알파제어), DimToDark 기능 등
/// </summary>

public class PopupBase : MonoBehaviour
{
    // popup 최상단에 배치된 sprite
    // 전체화면 잠금, Dim과 같은 기능을 수행하는데 사용
    [SerializeField] Image TopOverlay = null;

    // 팝업 닫을경우에 대한 콜백함수 처리
    public System.Action EventClose { get; set; } = null;

    protected virtual void Start()
    {
        FadeIn();
    }

    // 팝업 닫기 수행
    public virtual void OnBtnBack()
    {
        EventClose?.Invoke();
        ClosePopup();
    }

    protected void ClosePopup()
    {
        Destroy(gameObject);
    }

    protected void FadeIn()
    {
        // FadeIn 시작시 어두운 화면이므로 UI는 잠근 상태로 시작
        LockUI();
        TopOverlay.DOFade(0, 0.4f).From(1)
        .OnComplete(() => 
        {
            // FadeIn 동작이 끝나면 UI는 잠금 해제
            UnLockUI();
        });
    }
    protected void FadeOut()
    {
        // FadeOut 동작 시작시 UI 잠금
        LockUI();
        TopOverlay.DOFade(1, 0.4f).From(0)
        .OnComplete(() =>
        {
            // FadeIn 동작이 끝나면 UI는 잠금 해제
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

    // 전체 화면 어둡게 처리
    protected void DimToDark()
    {
        TopOverlay.color = new Color(0, 0, 0, 0.9f);
        LockUI();
    }
    protected void DimOff()
    {
        TopOverlay.color = new Color(0, 0, 0, 0);
        UnLockUI();
    }

}
