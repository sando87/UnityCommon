using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 각 Popup들의 부모 클래스
/// 각 Popup들의 공통 기능들은 이곳에서 처리한다.
/// 공통기능 : 전체화면 잠금, 뒤로가기(닫기), 생성 및 소멸시 애니메이션 효과, FadeInOut(알파제어), DimToDark 기능 등
/// </summary>

public class PopupBase : MonoBehaviour
{
    // 알파를 0으로 하면 화면 잠금이 안되는 현상이 있어서 최소한의 알파값을 줘야 화면 잠금을 할 수 있다.
    private const float minAlpha = 1f / 255f;

    // popup 최상단에 배치된 sprite
    // 전체화면 잠금, Dim과 같은 기능을 수행하는데 사용
    [SerializeField] UISprite TopOverlay = null;

    // 팝업 닫을경우에 대한 콜백함수 처리
    public System.Action EventClose { get; set; } = null;
    public int BaseDepth { get { return GetComponent<UIPanel>().depth; } }

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
        UIPanel rootPanel = GetComponent<UIPanel>();
        rootPanel.alpha = 0;
        DOTween.To(() => rootPanel.alpha, x => rootPanel.alpha = x, 1, 0.4f).OnComplete(() =>
        {
            // FadeIn 동작이 끝나면 UI는 잠금 해제
            UnLockUI();
        });
    }
    protected void FadeOut()
    {
        // FadeOut 동작 시작시 UI 잠금
        LockUI();
        UIPanel rootPanel = GetComponent<UIPanel>();
        rootPanel.alpha = 1;
        DOTween.To(() => rootPanel.alpha, x => rootPanel.alpha = x, 0, 0.4f);
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

    // 전체 화면 어둡게 처리
    protected void DimToDark()
    {
        TopOverlay.color = new Color(0, 0, 0, 0.9f);
        LockUI();
    }
    protected void DimOff()
    {
        TopOverlay.color = new Color(0, 0, 0, minAlpha);
        UnLockUI();
    }

}
