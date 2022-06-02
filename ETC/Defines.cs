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
    [SerializeField] private bool isAgreedTerms = false;
    //배경private음악 볼륨
    [SerializeField] private float musicVolume = 0.5f;
    //효과음 볼륨
    [SerializeField] private float soundVolume = 0.5f;
    //언어 설정
    [SerializeField] private LanguageType language = LanguageType.English;


    public int DataVersion { get => dataVersion; set { dataVersion = value; GamePlayData.Save(); } }
    public string UserID1 { get => UserID; set { UserID = value; GamePlayData.Save(); } }
    public bool IsAgreedTerms { get => isAgreedTerms; set { isAgreedTerms = value; GamePlayData.Save(); } }
    public float MusicVolume { get => musicVolume; set { musicVolume = value; GamePlayData.Save(); } }
    public float SoundVolume { get => soundVolume; set { soundVolume = value; GamePlayData.Save(); } }
    public LanguageType Language { get => language; set { language = value; GamePlayData.Save(); } }


    public void ResetUserSettingToDefault()
    {
        musicVolume = 0.5f;
        soundVolume = 0.5f;

        GamePlayData.Save();
    }
}

public class Consts
{
    public const int PixelPerUnit = 24;
    public const float DistPerPixel = 1.0f / PixelPerUnit;
    public const float BlockSize = 1.0f;
    public const float SkinDepth = 1.0f / PixelPerUnit;
    public const string MapFilePath = "./Assets/00_MetaSuit/Resources/";
    public const string MapThemePath = "MapThemes/";
    public const string VFXPath = "VFX/";
    public const string SFXPath = "Sound/SFX/InGame/";
    public const int RefWidth = 640;
    public const int RefHeight = 360;
    public const float GalobalGravity = -50.0f;
    public const float DropDistance = Consts.BlockSize * 0.2f;
}

public class AnimParam
{
    public static readonly int ActionType = Animator.StringToHash("ActionType");
    public static readonly int HandType = Animator.StringToHash("HandType");
    public static readonly int DoActionTrigger = Animator.StringToHash("DoActionTrigger");
    public static readonly int DoHandTrigger = Animator.StringToHash("DoHandTrigger");
    public static readonly int VelocityY = Animator.StringToHash("VelocityY");
    public static readonly int IsHangingRight = Animator.StringToHash("IsHangingRight");
    public static readonly int IsSitDown = Animator.StringToHash("IsSitDown");
    public static readonly int IsRun = Animator.StringToHash("IsRun");
}

public enum AnimActionID
{
    Idle = 0,
    Move = 1,
    Damaged = 2,
    Death = 3,
    Rolling = 4,
    Jump = 5,
    HangingJump = 7,
    HangingHold = 9,
    Throw = 11,
    Detected = 12,
    TransformGo = 13,
    TransformAttack = 14,
    TransformBack = 15,
    AttackBase = 16,
    Stun = 17,
    AttackBase2 = 18,

    HandIdle = 20,
    Attack1 = 21,
    Attack2 = 22,

    TransformIntro2 = 23,
    TransformAttack2 = 24,
    TransformOutro2 = 25,

    SuitOn = 30,
    SuitOff = 31,
    SuitOver = 32,
}

public class LayerID
{
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Enemies = LayerMask.NameToLayer("Enemies");
    public static readonly int Projectiles = LayerMask.NameToLayer("Projectiles");
    public static readonly int IngameParticles = LayerMask.NameToLayer("IngameParticles");
    public static readonly int ThemeBackground = LayerMask.NameToLayer("ThemeBackground");
    public static readonly int PlatformsBack = LayerMask.NameToLayer("PlatformsBack");
    public static readonly int Platforms = LayerMask.NameToLayer("Platforms");
    public static readonly int Props = LayerMask.NameToLayer("Props");
    public static readonly int Undetectable = LayerMask.NameToLayer("Undetectable");
    public static readonly int MapEditorEtc = LayerMask.NameToLayer("MapEditorEtc");
    public static readonly int MapEditorFrame = LayerMask.NameToLayer("MapEditorFrame");
}

public class SortingLayerID
{
    public static readonly int Background = SortingLayer.NameToID("Background");
    public static readonly int Default = SortingLayer.NameToID("Default");
    public static readonly int Between = SortingLayer.NameToID("Between");
    public static readonly int PlatformsBack = SortingLayer.NameToID("PlatformsBack");
    public static readonly int Player = SortingLayer.NameToID("Player");
    public static readonly int PlatformsFront = SortingLayer.NameToID("PlatformsFront");
    public static readonly int Foreground = SortingLayer.NameToID("Foreground");
    public static readonly int VisibleParticles = SortingLayer.NameToID("VisibleParticles");
    public static readonly int UI = SortingLayer.NameToID("UI");
}
