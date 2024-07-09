using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 유저의 게임 플레이 정보를 파일로 저장하는 구조체
/// </summary>

[System.Serializable]
public class UserPlayInfo : SaveableBase
{
    // 게임 파일 버전
    public string Version = "0";
    public bool IsNormalCleared = false;
    public bool IsHardCleared = false;
    public bool IsHellCleared = false;

    // 스토리모드 슬롯별 게임 플레이 데이타
    public UserSlotInfo[] StoryModeSlots = new UserSlotInfo[3];

    public UserPlayInfo()
    {
        this.Version = Application.version;

        for (int i = 0; i < StoryModeSlots.Length; ++i)
            StoryModeSlots[i] = new UserSlotInfo();
    }

    [JsonIgnore]
    public bool IsAllSlotEmptyOnStoryMode
    {
        get { return StoryModeSlots[0].IsEmpty && StoryModeSlots[1].IsEmpty && StoryModeSlots[2].IsEmpty; }
    }

    public override bool IsOldVersion()
    {
        if (float.TryParse(Version, out float fileVersion))
        {
            if (float.TryParse(Application.version, out float appVersion))
            {
                return fileVersion < appVersion;
            }
        }
        return base.IsOldVersion();
    }
}