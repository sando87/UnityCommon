using System;
using UnityEngine;

public enum LanguageType
{
    None,
    English,
    Japanese,
    Korean,
    Portuguese,
    Russian,
    ChineseSimplified,
    Spanish,
    ChineseTraditional,
}

[Serializable]
public class UserSaveData
{
    // 이전버전과의 호환성을 위해 존재
    [SerializeField] private int dataVersion = GamePlayData.DataVersion;
    // 유저 ID
    [SerializeField] private string UserID = "";
    // 이용약관 동의 여부
    [SerializeField] private bool b_termsOfUseAgreement = false;
    //창모드인지 여부
    [SerializeField] private bool isWindowMode = false;
    //현재 설정된 해상도
    [SerializeField] private int resolutionIndex = 0;
    //현재 설정된 언어
    [SerializeField] private LanguageType language = LanguageType.English;
    //인게임 카드를 드래그하여 옮길수 있는지
    [SerializeField] private bool cardDraggable = false;
    //인게임 카드 배치 왼쪽인지 오른쪽인지
    [SerializeField] private bool cardIsLeftSide = false;
    //인게임 배경 투명 여부
    [SerializeField] private bool ingameBackIsTranslucent = false;
    //배경private음악 볼륨
    [SerializeField] private float musicVolume = 0.5f;
    //효과음 볼륨
    [SerializeField] private float soundVolume = 0.5f;
    //현재 진행중인 Age Index
    [SerializeField] private int ageTableIndex = 0;
    //현재 진행중인 Age 빌딩에서 획득한 score
    [SerializeField] private int currentBuildingScore = 0;
    //Ingame 획득한 전체 score
    [SerializeField] private int ingameScore = 0;
    //Ingame 시작시 선택될 카드 Deck 세트 Index
    [SerializeField] private int ingameStartDeckIndex = 0;
    //Ingame DrawMode 3장씩인지 여부
    [SerializeField] private bool isThreeDrawMode = false;
    //현재 선택한 카드 전면테마 인덱스
    [SerializeField] private int cardFrontThemeIndex = 0;
    //현재 선택한 카드 후면테마 인덱스
    [SerializeField] private int cardBackThemeIndex = 0;
    
    //Ingame 총 플레이 횟수
    [SerializeField] private int ingameTotalPlayCount = 0;
    //Ingame 승리 횟수
    [SerializeField] private int ingameWinPlayCount = 0;
    //연승 횟수
    [SerializeField] private int winStreak = 0;
    //Ingame 최대 연승 횟수
    [SerializeField] private int bestWinStreak = 0;
    //Ingame 총 플레이 시간
    [SerializeField] private int totalPlayTime = 0;
    //Ingame 최단시간 플레이 시간
    [SerializeField] private int fastestPlayTime = 0;
    

    public int DataVersion { get => dataVersion; set { dataVersion = value; GamePlayData.Save(); } }
    public string UserID1 { get => UserID; set { UserID = value; GamePlayData.Save(); } }
    public bool B_termsOfUseAgreement { get => b_termsOfUseAgreement; set { b_termsOfUseAgreement = value; GamePlayData.Save(); } }
    public bool IsWindowMode { get => isWindowMode; set { isWindowMode = value; GamePlayData.Save(); } }
    public int ResolutionIndex { get => resolutionIndex; set { resolutionIndex = value; GamePlayData.Save(); } }
    public LanguageType Language { get => language; set { language = value; GamePlayData.Save(); } }
    public bool CardDraggable { get => cardDraggable; set { cardDraggable = value; GamePlayData.Save(); } }
    public bool CardIsLeftSide { get => cardIsLeftSide; set { cardIsLeftSide = value; GamePlayData.Save(); } }
    public bool IngameBackIsTranslucent { get => ingameBackIsTranslucent; set { ingameBackIsTranslucent = value; GamePlayData.Save(); } }
    public float MusicVolume { get => musicVolume; set { musicVolume = value; GamePlayData.Save(); } }
    public float SoundVolume { get => soundVolume; set { soundVolume = value; GamePlayData.Save(); } }
    public int AgeTableIndex { get => ageTableIndex; set { ageTableIndex = value; GamePlayData.Save(); } }
    public int CurrentBuildingScore { get => currentBuildingScore; set { currentBuildingScore = value; GamePlayData.Save(); } }
    public int IngameScore { get => ingameScore; set { ingameScore = value; GamePlayData.Save(); } }
    public int IngameStartDeckIndex { get => ingameStartDeckIndex; set { ingameStartDeckIndex = value; GamePlayData.Save(); } }
    public bool IsThreeDrawMode { get => isThreeDrawMode; set { isThreeDrawMode = value; GamePlayData.Save(); } }
    public int CardFrontThemeIndex { get => cardFrontThemeIndex; set { cardFrontThemeIndex = value; GamePlayData.Save(); } }
    public int CardBackThemeIndex { get => cardBackThemeIndex; set { cardBackThemeIndex = value; GamePlayData.Save(); } }

    public int IngameTotalPlayCount { get => ingameTotalPlayCount; set { ingameTotalPlayCount = value; GamePlayData.Save(); } }
    public int IngameWinPlayCount { get => ingameWinPlayCount; set { ingameWinPlayCount = value; GamePlayData.Save(); } }
    public int WinStreak { get => winStreak; set { winStreak = value; GamePlayData.Save(); } }
    public int BestWinStreak { get => bestWinStreak; set { bestWinStreak = value; GamePlayData.Save(); } }
    public int TotalPlayTime { get => totalPlayTime; set { totalPlayTime = value; GamePlayData.Save(); } }
    public int FastestPlayTime { get => fastestPlayTime; set { fastestPlayTime = value; GamePlayData.Save(); } }

    public void ResetUserSettingToDefault()
    {
        isWindowMode = false;
        resolutionIndex = 0;
        language = LanguageType.English;
        cardDraggable = false;
        cardIsLeftSide = false;
        ingameBackIsTranslucent = false;
        musicVolume = 0.5f;
        soundVolume = 0.5f;
        cardFrontThemeIndex = 0;
        cardBackThemeIndex = 0;

        GamePlayData.Save();
    }
}