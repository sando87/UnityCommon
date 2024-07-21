using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 게임 최초 시작시 전반적인 게임에 대한 초기화 루틴 수행
/// </summary>

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        // 게임 글로벌 리소스 데이터 초기화
        DatabaseCSV<SuitRawInfo>.Instance.Init(Consts.SuitTableFilePath);
    }
}
