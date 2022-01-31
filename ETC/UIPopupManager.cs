using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팝업관련 UI를 띄우거나 팝업 Depth를 처리하는 등 여러 팝업들을 관리한다.
/// SomePopup의 프리팹 오브젝트가 등록되어 있어야 한다.
/// 사용예) UIPopupManager.Inst.Show<SomePopup>();
/// </summary>

public class UIPopupManager : SingletonMono<UIPopupManager>
{
    [SerializeField] PopupBase[] PopupPrefabs = null;

    private UIRoot CurrentRootUI { get { return FindObjectOfType<UIRoot>(); } }

    void Start()
    {
    }

    public T Show<T>() where T : PopupBase
    {
        // 미리 등록된 팝업 프리팹을 검색하여 찾다가 해당 컴포넌트를 가지고 있는 팝업이 있다면 팝업 생성
        foreach(PopupBase prefab in PopupPrefabs)
        {
            if(prefab.GetComponent<T>() != null)
            {
                // 팝업 생성
                PopupBase popup = Instantiate(prefab, CurrentRootUI.transform);

                // 새로 생성된 팝업이 제일 상위에 올라오도록 depth 조절한다.
                popup.GetComponent<UIPanel>().depth = GetMaxPanelDepth() + 1;
                popup.GetComponent<UIPanel>().useSortingOrder = true;
                popup.GetComponent<UIPanel>().sortingOrder = 2;
                return popup as T;
            }
        }
        LOG.warn();
        return null;
    }

    // 현재 UIRoot 하위에 있는 모든 패널들중 제일 높은 depth값을 반환
    private int GetMaxPanelDepth()
    {
        UIPanel[] childPandels = CurrentRootUI.GetComponentsInChildren<UIPanel>();
        int maxDepth = 0;
        foreach(UIPanel panel in childPandels)
        {
            maxDepth = Mathf.Max(maxDepth, panel.depth);
        }
        return maxDepth;
    }
}
