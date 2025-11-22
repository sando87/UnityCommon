using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

/// <summary>
/// 클래스 개념 및 의도
/// 게임 전반적으로 Data접근이 필요한 모든 접근은 이 클래스를 통해 접근하도록 설계
/// 추상적(개념적)으로 정의 필요
/// 크게 Resource 폴더 하위의 ReadOnly형태의 정보들 접근 함수 제공
/// 예로 각종 csv, json형태의 테이블 정보들 접근하거나, 프리팹, 사운드클립과 같은 에셋에 접근 제공
/// 또한 UserSave파일과 같은 읽기/쓰기 데이터도 이 클래스를 통해 접근/제어 가능
/// </summary>

public static class DataManager
{

    // Chapter별 정보가 있는 테이블 정보 제공
    public static ChapterInfo[] GetAllChapterInfos()
    {
        return DatabaseUnityChapters.Instance.GetAllInfo();
    }
    public static ChapterInfo GetChapterInfo(string chapterName)
    {
        long id = ChapterInfo.NameToID(chapterName);
        if (!DatabaseUnityChapters.Instance.HasInfo(id))
            return null;

        return DatabaseUnityChapters.Instance.GetInfo(id);
    }





    ////////////////////////////////////////////////////////////////////

    /// Save데이터 파일 형태의 저장 및 수정이 가능한 데이터 접근/제어

    // 사용자 세팅 정보
    public static UserSettingInfo GetUserSettingData()
    {
        return SaveFileUtil<UserSettingInfo>.Load();
    }
    public static void SaveUserSettingData(UserSettingInfo data)
    {
        SaveFileUtil<UserSettingInfo>.Save(data);
    }
    public static void ResetUserSettingData()
    {
        SaveFileUtil<UserSettingInfo>.ResetUserDataFile();
    }

    // 사용자 게임 플레이 정보
    public static UserPlayInfo GetUserPlayData()
    {
        return SaveFileUtil<UserPlayInfo>.Load();
    }
    public static void SaveUserPlayData(UserPlayInfo data)
    {
        SaveFileUtil<UserPlayInfo>.Save(data);
    }
    public static void ResetUserPlayData()
    {
        SaveFileUtil<UserPlayInfo>.ResetUserDataFile();
    }
}