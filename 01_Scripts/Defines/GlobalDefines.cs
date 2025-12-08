using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

// public enum LanguageType
// {
//     None,
//     English,
//     Japanese,
//     Korean,
//     Portuguese,
//     Russian,
//     ChineseSimplified,
//     Spanish,
//     ChineseTraditional,
// }

// [Serializable]
// public class UserSaveData
// {
//     // 이전버전과의 호환성을 위해 존재
//     [SerializeField] private int dataVersion = GamePlayData.DataVersion;
//     // 유저 ID
//     [SerializeField] private string UserID = "";
//     // 이용약관 동의 여부
//     [SerializeField] private bool isAgreedTerms = false;
//     //배경private음악 볼륨
//     [SerializeField] private float musicVolume = 0.5f;
//     //효과음 볼륨
//     [SerializeField] private float soundVolume = 0.5f;
//     //언어 설정
//     [SerializeField] private LanguageType language = LanguageType.English;


//     public int DataVersion { get => dataVersion; set { dataVersion = value; GamePlayData.Save(); } }
//     public string UserID1 { get => UserID; set { UserID = value; GamePlayData.Save(); } }
//     public bool IsAgreedTerms { get => isAgreedTerms; set { isAgreedTerms = value; GamePlayData.Save(); } }
//     public float MusicVolume { get => musicVolume; set { musicVolume = value; GamePlayData.Save(); } }
//     public float SoundVolume { get => soundVolume; set { soundVolume = value; GamePlayData.Save(); } }
//     public LanguageType Language { get => language; set { language = value; GamePlayData.Save(); } }


//     public void ResetUserSettingToDefault()
//     {
//         musicVolume = 0.5f;
//         soundVolume = 0.5f;

//         GamePlayData.Save();
//     }
// }

// public class Consts
// {
//     public const string VFXPath = "VFX/";
//     public const string SFXPath = "Sound/SFX/";
// }

// public class AnimParam
// {
//     public static readonly int ActionType = Animator.StringToHash("ActionType");
//     public static readonly int DoActionTrigger = Animator.StringToHash("DoActionTrigger");
//     public static readonly int VerticalDegreeIndex = Animator.StringToHash("VerticalDegreeIndex");
// }

// public enum AnimActionID
// {
//     Idle = 0,
//     Move = 1,
//     Death = 2,
//     AttackA = 3,
//     AttackB = 4,
//     AttackLoop = 5,
// }

// public class LayerID
// {
//     public static readonly int Player = LayerMask.NameToLayer("Player");
//     public static readonly int Enemies = LayerMask.NameToLayer("Enemies");
//     public static readonly int Projectiles = LayerMask.NameToLayer("Projectiles");
//     public static readonly int IngameParticles = LayerMask.NameToLayer("IngameParticles");
//     public static readonly int ThemeBackground = LayerMask.NameToLayer("BackgroundMap");
//     public static readonly int Undetectable = LayerMask.NameToLayer("Undetectable");
// }

// public class SortingLayerID
// {
//     public static readonly int Background = SortingLayer.NameToID("Background");
//     public static readonly int Default = SortingLayer.NameToID("Default");
//     public static readonly int Between = SortingLayer.NameToID("Between");
//     public static readonly int PlatformsBack = SortingLayer.NameToID("PlatformsBack");
//     public static readonly int Player = SortingLayer.NameToID("Player");
//     public static readonly int PlatformsFront = SortingLayer.NameToID("PlatformsFront");
//     public static readonly int Foreground = SortingLayer.NameToID("Foreground");
//     public static readonly int VisibleParticles = SortingLayer.NameToID("VisibleParticles");
//     public static readonly int UI = SortingLayer.NameToID("UI");
// }

// [System.Flags]
// public enum InputDetectType
// {
//     None = 0,
//     TriggerDown = 1 << 1, // 키를 누른 순간만 감지
//     TriggerUp = 1 << 2, // 키를 뗀 순간만 감지
//     KeepPressing = 1 << 3, // 키를 지속적으로 누르고 있을경우 계속 감지
//     All = ~0,
// }

public class Consts
{
    public const int PixelPerUnit = 32;
    public const float DistPerPixel = 1.0f / PixelPerUnit;
    public const float DistPerPixelHalf = DistPerPixel * 0.5f;
    public const float DistPerPixelx2 = DistPerPixel * 2;
    public const float BlockSize = 1.0f;
    public const string MapFilePath = "./Assets/00_MetaSuit/Resources/";
    public const string UserLogFilePath = "./Assets/00_MetaSuit/Resources/UserLog/";
    public const string MapThemePath = "MapThemes/";
    public const string VFXPath = "VFX/";
    public const string SFXPath = "Sound/SFX/InGame/";
    public const string PathAudioClipData = "Sound/AudioAsset/"; // AudioClipData 리소스 정보들이 있는 경로
    public const string SuitTableFilePath = "Assets/UnityCommon/Resources/Database/";
}


public enum AnimActionID
{
    Idle,
    Move,
    Damaged,
    Death,
    Rolling,
    Jump,
    HangingJump,
    HangingHold,

    AttackNormal,
    AttackSpecial,
    AttackMelee,

    Detected,
    Stun,
    SuitOn,
    SuitOff,
    SuitOver,
    Panic,
    IdlePatrol,
    General,
    Turn,
    SwitchOn,
    Prefix,
    Suffix,
    Appear,
    Respawn,
    Reload,

    AttackNormal2,
    AttackNormal3,
    AttackChargeLoop,

    ReadySpecial,
    Hide, // BossBaron 에서 Appear, Hide를 Digout, Digin 으로 사용
    Transform, // BossCrab의 crab-robo 폼체인지에 사용
}

public class LayerID
{
    public static readonly int Player = LayerMask.NameToLayer("Player");
    public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
    public static readonly int TerrainHard = LayerMask.NameToLayer("TerrainHard");
    public static readonly int TerrainWeak = LayerMask.NameToLayer("TerrainWeak");
    public static readonly int Props = LayerMask.NameToLayer("Props");

    public static readonly int Platforms = LayerMask.NameToLayer("Platforms");
    public static readonly int PlatformsThin = LayerMask.NameToLayer("PlatformsThin");

    public static readonly int Rope = LayerMask.NameToLayer("Rope");
    public static readonly int Water = LayerMask.NameToLayer("Water");
    // public static readonly int DarkFog = LayerMask.NameToLayer("DarkFog");

    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int InGameSystem = LayerMask.NameToLayer("InGameSystem");
    public static readonly int UnSelectable = LayerMask.NameToLayer("UnSelectable");


    // public static readonly int Projectiles = LayerMask.NameToLayer("Projectiles");
    // public static readonly int Dangerous = LayerMask.NameToLayer("Dangerous");
    // public static readonly int IngameParticles = LayerMask.NameToLayer("IngameParticles");
    // public static readonly int ThemeBackground = LayerMask.NameToLayer("ThemeBackground");
    // public static readonly int PlatformsBack = LayerMask.NameToLayer("PlatformsBack");
    // public static readonly int Hitable = LayerMask.NameToLayer("Hitable");
    // public static readonly int HitablePlayerOnly = LayerMask.NameToLayer("HitablePlayerOnly");
    // public static readonly int HitablePlayerProps = LayerMask.NameToLayer("HitablePlayerProps");
}

public class AnimParam
{
    public static readonly int ActionType = Animator.StringToHash("ActionType");
    public static readonly int ActionSubType = Animator.StringToHash("ActionSubType");
    public static readonly int HandType = Animator.StringToHash("HandType");
    public static readonly int DoActionTrigger = Animator.StringToHash("DoActionTrigger");
    public static readonly int DoHandTrigger = Animator.StringToHash("DoHandTrigger");
    public static readonly int VelocityY = Animator.StringToHash("VelocityY");
    public static readonly int IsHangingRight = Animator.StringToHash("IsHangingRight");
    public static readonly int IsSitDown = Animator.StringToHash("IsSitDown");
    public static readonly int IsRun = Animator.StringToHash("IsRun");
    public static readonly int DerivedType = Animator.StringToHash("DerivedType");
    public static readonly int NomalAttackSpeed = Animator.StringToHash("NomalAttackSpeed");
    public static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int IsPhaseSecond = Animator.StringToHash("IsPhaseSecond");
    public static readonly int IsLeftSide = Animator.StringToHash("IsLeftSide");
    public static readonly int IsLoop = Animator.StringToHash("IsLoop");
    public static readonly int IsLanding = Animator.StringToHash("IsLanding");
}

class RandomSequence
{
    // 0 ~ maxValue사이의 랜덤값을 겹치지 않게 반환

    private int[] mNumbers = null;
    private int mIndex = 0;
    public RandomSequence(int maxValue)
    {
        System.Random ran = new System.Random();
        List<int> values = new List<int>();
        for (int i = 0; i <= maxValue; ++i)
            values.Add(i);
        values.Sort((a, b) => { return ran.Next(-1, 1); });
        mNumbers = values.ToArray();
        mIndex = 0;
    }
    public int GetNext()
    {
        mIndex = (mIndex + 1) % mNumbers.Length;
        return mNumbers[mIndex];
    }
}

public enum DamageKind
{
    Normal, Fire
}
public struct DamageProp
{
    public DamageKind type;
    public float damage;
    public DamageProp(float _damage) { this.damage = _damage; type = DamageKind.Normal; }
    public DamageProp(float _damage, DamageKind _type) { this.damage = _damage; this.type = _type; }

    // DamageProp형을 float형으로 암시적 형변환 가능 예) float damage = new DamageProp(_damage);
    public static implicit operator float(DamageProp info) => info.damage;

    // float형을 DamageProp로 암시적 형변환 가능 예) DamageProp info = 1.0f;
    public static implicit operator DamageProp(float damage) => new DamageProp(damage);

    public static DamageProp operator +(DamageProp a, DamageProp b)
        => new DamageProp(a.damage + b.damage, a.type);
    public static DamageProp operator +(DamageProp a, float _damage)
        => new DamageProp(a.damage + _damage, a.type);

    public static DamageProp operator -(DamageProp a, DamageProp b)
        => new DamageProp(a.damage - b.damage, a.type);
    public static DamageProp operator -(DamageProp a, float _damage)
        => new DamageProp(a.damage - _damage, a.type);

    public override string ToString() => $"{damage}";
}

[System.Serializable]
public class SuitRawInfo : ICSVFormat
{
    [System.NonSerialized]
    public string id = "";
    public string name = "";
    public float hp = 0;

    public int RowIndex { get; set; }
    public long ID { get { return ICSVFormat.ToID(id); } }
}