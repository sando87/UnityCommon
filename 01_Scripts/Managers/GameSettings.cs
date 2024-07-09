using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 세팅에 관련된 순수한 기능 구현
/// 프로그램 전역적으로 접근 및 제어가능해야 하고
/// UI부분과는 독립적이여야함(다른 게임에서도 재활용 하도록 하기위해)
/// </summary>

public class GameSettings : SingletonMono<GameSettings>
{
    public int ScreenWidth { get; private set; } = 0;
    public int ScreenHeight { get; private set; } = 0;
    public bool IsFullScreenMode { get { return !UserSettingInfo.IsWindowMode; } }
    public event System.Action<int, int> EventChanged;

    private UserSettingInfo mUserSettingInfo = null;
    private UserSettingInfo UserSettingInfo
    {
        get
        {
            if (!IsLoaded)
            {
                ApplySettingsOnStart();
            }
            return mUserSettingInfo;
        }
        set => mUserSettingInfo = value;
    }

    public bool IsLoaded { get { return mUserSettingInfo != null; } }

    void Start()
    {
        ApplySettingsOnStart();
    }

    void Update()
    {
        if (!IsLoaded)
            return;

        // 해상도 변경시 시스템 변수가 바로 적용이 안되어 Update함수에서 매번 해상도 변경을 감지하여 UI에 처리한다.
        if (ScreenWidth != Screen.width || ScreenHeight != Screen.height)
        {
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
            EventChanged?.Invoke(ScreenWidth, ScreenHeight);
        }
    }

    public void ChangeNextResolution(bool isNext)
    {
        // 현재 전체화면 모드이면 해상도 변경은 안된다.
        if (!UserSettingInfo.IsWindowMode) return;

        Vector3Int[] resolutions = GetResolutions();
        UserSettingInfo.ResolutionIndex = isNext ? UserSettingInfo.ResolutionIndex - 1 : UserSettingInfo.ResolutionIndex + 1;
        if (UserSettingInfo.ResolutionIndex < 0)
            UserSettingInfo.ResolutionIndex = resolutions.Length - 1;
        else if (resolutions.Length <= UserSettingInfo.ResolutionIndex)
            UserSettingInfo.ResolutionIndex = 0;

        ApplyResolutionToSystem();

        SaveUserInfoToFile();
    }

    // 윈도우 모드 변경
    public void ToggleWindowMode()
    {
        // 현재 전체화면일 경우 윈도우 모드로 변경된다
        UserSettingInfo.IsWindowMode = !UserSettingInfo.IsWindowMode;

        ApplyResolutionToSystem();

        SaveUserInfoToFile();
    }

    void ApplyResolutionToSystem()
    {
        // system Screen 정보를 변경한다
        Vector3Int[] resolutions = GetResolutions();

        int resIdx = UserSettingInfo.IsWindowMode ? UserSettingInfo.ResolutionIndex : resolutions.Length - 1;
        int sysResIdx = resolutions[resIdx % resolutions.Length].z;
        Resolution res = Screen.resolutions[sysResIdx % Screen.resolutions.Length];
        Screen.SetResolution(res.width, res.height, !UserSettingInfo.IsWindowMode);
    }

    // 게임 시작할때마다 모니터가 매번 바뀔 수 있으므로
    // 매번 현재 모니터 상황에 맞는 해상도 인덱스를 계산하여 업데이트 한다.
    // 기존의 저장된 해상도 인덱스 정보가 수정되면 true를 반환
    bool SetValidateResolutionIndex()
    {
        // 현재 모니터 해상도중 유효한 리스트만 뽑아온다.
        Vector3Int[] resolutions = GetResolutions();

        int resIdx = UserSettingInfo.ResolutionIndex;
        if (resIdx < 0 || resolutions.Length <= resIdx)
        {
            UserSettingInfo.ResolutionIndex = resolutions.Length - 1;
            return true;
        }

        return false;
    }

    // 시스템에서 제공하는 Screen.resolutions 에서 1차적으로 지원가능한 해상도만 거른다.
    Vector3Int[] GetResolutions()
    {
        // Key의 Vector2Int의 x,y 는 width,height이고 Value Vector2Int는 refreshRate, index
        Dictionary<Vector2Int, Vector2Int> infos = new Dictionary<Vector2Int, Vector2Int>();
        for (int i = 0; i < Screen.resolutions.Length; ++i)
        {
            Resolution res = Screen.resolutions[i];
            // 가로 해상도가 1024보다 작은 해상도는 지원 안함
            if (res.width < 1024)
                continue;

            Vector2Int key = new Vector2Int(res.width, res.height);
            Vector2Int val = new Vector2Int(res.refreshRate, i);
            // 우선 refreshRate 다른 해상도는 하나로 취급(단 60Hz를 우선으로 취급)
            if (infos.ContainsKey(key))
            {
                int refreshRate = infos[key].x;
                if (refreshRate != 60)
                    infos[key] = val;
            }
            else
            {
                infos[key] = val;
            }
        }

        List<Vector3Int> rets = new List<Vector3Int>();
        foreach (var item in infos)
        {
            rets.Add(new Vector3Int(item.Key.x, item.Key.y, item.Value.y));
        }

        // 해상도의 width값을 기준으로 오름차순 정렬한다(그 이유는 제일 마지막위치에 있는 해상도가 default 해상도가 되기 때문에)
        rets.Sort((lbs, rbs) => { return lbs.x - rbs.x; });

        return rets.ToArray();
    }




    // 모든 설정 리셋
    public void ResetSettingsToDefault()
    {
        // sjlee : 데모용 빌드 수정
        PlayerPrefs.DeleteAll();
        Debug.Log("모든 데이터 리셋");
        DataManager.ResetUserPlayData();
        DataManager.ResetUserSettingData();

        // 초기 데이터로 다시 세팅 후 저장
        UserSettingInfo = new UserSettingInfo();
        SetValidateResolutionIndex();

        if (UserSettingInfo.Lang == LanguageType.None)
            UserSettingInfo.Lang = LanguageController.GetSystemOSLanguage();

        SaveUserInfoToFile();

        // 초기화된 정보를 게임 시스템에 적용
        ApplyUserSettingInfoToSystem();
    }

    // 게임 시작시 현재 사용자 세팅정보를 로딩하여 Game시스템에 적용
    public void ApplySettingsOnStart()
    {
        if (IsLoaded)
            return;
        // 로컬 파일에 있는 사용자 정보 로딩
        LoadUserInfoFromFile();

        // 모니터 환경이 달라지면 해상도 정보 수정 후 다시 저장
        if (SetValidateResolutionIndex())
        {
            SaveUserInfoToFile();
        }

        // 게임 최초 실행시 설정된 언어가 없는 경우 system OS의 설정된 언어로 초기화
        if (UserSettingInfo.Lang == LanguageType.None)
        {
            UserSettingInfo.Lang = LanguageController.GetSystemOSLanguage();
            SaveUserInfoToFile();
        }

        // 초기화된 정보를 게임 시스템에 적용
        ApplyUserSettingInfoToSystem();
    }

    // 현재 로딩된 사용자 정보들로 system 기능에 모두 적용한다.
    private void ApplyUserSettingInfoToSystem()
    {
        // 해상도 세팅
        ApplyResolutionToSystem();

        // 언어 세팅
        ApplyLanguageToSystem();

        // 볼륨 세팅
        ApplySoundVolumeToSystem();

        ApplyVSyncToSystem();

        ApplyVibrationToSystem();
    }

    void LoadUserInfoFromFile()
    {
        UserSettingInfo = DataManager.GetUserSettingData();
    }

    void SaveUserInfoToFile()
    {
        DataManager.SaveUserSettingData(UserSettingInfo);
    }


    public float VolumeBGM { get { return UserSettingInfo.VolumeBGM; } }
    public float VolumeSFX { get { return UserSettingInfo.VolumeSFX; } }

    public void SetVolumeBGM(float volume)
    {
        // 배경음 볼륨 정보 사용자 데이터에 저장
        UserSettingInfo.VolumeBGM = Mathf.Clamp(volume, 0, 1);
        SaveUserInfoToFile();

        // 실제 사운드 음량 조절
        ApplySoundVolumeToSystem();
    }

    public void SetVolumeSFX(float volume)
    {
        // 배경음 볼륨 정보 사용자 데이터에 저장
        UserSettingInfo.VolumeSFX = Mathf.Clamp(volume, 0, 1);
        SaveUserInfoToFile();

        // 실제 사운드 음량 조절
        ApplySoundVolumeToSystem();
    }

    void ApplySoundVolumeToSystem()
    {
        SoundPlayManager.Instance.VolumeBGM = UserSettingInfo.VolumeBGM;
        SoundPlayManager.Instance.VolumeSFX = UserSettingInfo.VolumeSFX;
    }



    public string LanguageName { get { return LanguageController.Instance.GetLanguageString(UserSettingInfo.Lang); } }
    public LanguageType LanguageType { get { return UserSettingInfo.Lang; } }

    public void SetLanguage(LanguageType lang)
    {
        UserSettingInfo.Lang = lang;
        SaveUserInfoToFile();

        ApplyLanguageToSystem();
    }
    void ApplyLanguageToSystem()
    {
        LanguageController.Instance.SetUserLangueageData(UserSettingInfo.Lang);
    }


    public bool VSync { get { return UserSettingInfo.VSync; } }

    public void SetVSync(bool vsync)
    {
        UserSettingInfo.VSync = vsync;
        SaveUserInfoToFile();

        ApplyVSyncToSystem();
    }
    void ApplyVSyncToSystem()
    {
        Debug.Log("AAAAA : VSync = " + UserSettingInfo.VSync);
        QualitySettings.vSyncCount = UserSettingInfo.VSync ? 1 : 0;
    }


    public bool Vibration { get { return UserSettingInfo.Vibration; } }

    public void SetVibration(bool vibration)
    {
        UserSettingInfo.Vibration = vibration;
        SaveUserInfoToFile();

        ApplyVibrationToSystem();
    }
    void ApplyVibrationToSystem()
    {
        // InputVibrationManager.Instance.IsDisabled = !UserSettingInfo.Vibration;
    }


    public bool NeedCheckBugReport { get { return UserSettingInfo.BugReportState == -1; } }
    public bool IsBugReportOn { get { return UserSettingInfo.BugReportState == 1; } }

    public void SetBugReportAgreement(bool isAgreed)
    {
        UserSettingInfo.BugReportState = isAgreed ? 1 : 0;
        SaveUserInfoToFile();
    }
}

[System.Serializable]
public class UserSettingInfo : SaveableBase
{
    public int ResolutionIndex = -1;
    public bool IsWindowMode = false;
    public LanguageType Lang = LanguageType.None;
    public float VolumeBGM = 0.5f;
    public float VolumeSFX = 0.5f;
    public bool VSync = true;
    public bool Vibration = true;
    public int BugReportState = -1; // -1:아직 결정되지 않음, 0:동의안함, 1:동의함
}