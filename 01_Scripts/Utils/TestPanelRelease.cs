using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 릴리즈 빌드 후 인게임상태에서 빠른 진행 위한 테스트 기능들 구현됨..
/// </summary>

public class TestPanelRelease : MonoBehaviour
{
    [SerializeField] int _WaveNumber = 1;
    [SerializeField] int _Level = 1;
    [Dropdown("GetUnitIDs")]
    public long UnitID;
    private DropdownList<long> GetUnitIDs()
    {
        DropdownList<long> units = new DropdownList<long>();
        foreach (var unit in UserCharactors.Inst.Enums())
            units.Add(unit.name, unit.ID);

        return units;
    }

    private bool mIsShow = true;
    private GUIStyle mGuiStyle = null;
    private InGameSystem InGameSystem { get { return InGameSystem.Instance; } }
    private RUIFormInGame InGameUI { get { return FindObjectOfType<RUIFormInGame>(); } }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            InGameUI.InvokePrivateMethod("OnClickRaiseMineral", null);
        if (Keyboard.current.oKey.wasPressedThisFrame)
            InGameUI.InvokePrivateMethod("OnClickCreateUnit", null);
        if (Keyboard.current.pKey.wasPressedThisFrame)
            InGameUI.InvokePrivateMethod("OnClickToggleUpgradePanel", null);
        if (Keyboard.current.uKey.wasPressedThisFrame)
            InGameUI.InvokePrivateMethod("OnClickRaiseRarePercent", null);

        if (Keyboard.current.qKey.wasPressedThisFrame)
            InGameSystem.UpgradeWeapon(UpgradeType.Melee);
        if (Keyboard.current.wKey.wasPressedThisFrame)
            InGameSystem.UpgradeWeapon(UpgradeType.Gun);
        if (Keyboard.current.eKey.wasPressedThisFrame)
            InGameSystem.UpgradeWeapon(UpgradeType.Magic);
        if (Keyboard.current.rKey.wasPressedThisFrame)
            InGameSystem.UpgradeWeapon(UpgradeType.TypeD);
        if (Keyboard.current.tKey.wasPressedThisFrame)
            InGameSystem.UpgradeWeapon(UpgradeType.Missile);

        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
            {Time.timeScale += 0.2f; LOG.trace(Time.timeScale);}
        if (Keyboard.current.rightBracketKey.wasPressedThisFrame)
            {Time.timeScale -= 0.2f; LOG.trace(Time.timeScale);}
    }

    [Button]
    public void CreateNewUnit()
    {
        BaseObject newUnit = InGameSystem.Instance.CreateUnit(UnitID);
        newUnit.SpecProp.Level = _Level;
    }

    [Button]
    void MoveToWave()
    {
        InGameSystem.Instance.NextWaveNumberForTest = _WaveNumber;
    }
    [Button]
    void EarnKillPoint()
    {
        InGameSystem.Instance.KillPoint += 10;
    }
    [Button]
    void EarnMineral()
    {
        InGameSystem.Instance.Mineral += 100;
    }
    [Button("RaiseMineral ( i )")]
    void RaiseMineral()
    {
        InGameUI.InvokePrivateMethod("OnClickRaiseMineral", null);
    }
    [Button("CreateUnit ( o )")]
    void CreateUnit()
    {
        InGameUI.InvokePrivateMethod("OnClickCreateUnit", null);
    }
    [Button("ToggleUpgradePanel ( p )")]
    void ToggleUpgradePanel()
    {
        InGameUI.InvokePrivateMethod("OnClickToggleUpgradePanel", null);
    }
    [Button("SpeedUp ( [ )")]
    public void SpeedUp()
    {
        Time.timeScale += 0.2f;
        LOG.trace(Time.timeScale);
    }
    [Button("SpeedDown ( ] )")]
    public void SpeedDown()
    {
        Time.timeScale -= 0.2f;
        LOG.trace(Time.timeScale);
    }
    [Button("RaiseRarePercent ( u )")]
    void RaiseRarePercent()
    {
        InGameUI.InvokePrivateMethod("OnClickRaiseRarePercent", null);
    }
    [Button("WeaponUpgrade ( qwert )")]
    void WeaponUpgrade()
    {
        
    }
    [Button("NextStep")]
    public void NextStep()
    {
        if (_Level == 1)
        {
            _Level = 2;
            _WaveNumber = 5;
            InGameSystem.Instance.NextWaveNumberForTest = _WaveNumber;
            InGameSystem.Instance.MineralStep = 4;
            InGameSystem.Instance.Mineral = 0;
            InGameSystem.Instance.KillPoint = 0;
            InGameSystem.Instance.CurrentRarePercentIndex = 0;
            UpgradeType highestUpgradeType = InGameSystem.Instance.GetHighestUpgradePower();
            InGameSystem.Instance.UpgradePower[highestUpgradeType] = 20;
        }
        else if (_Level == 2)
        {
            _Level = 3;
            _WaveNumber = 11;
            InGameSystem.Instance.NextWaveNumberForTest = _WaveNumber;
            InGameSystem.Instance.MineralStep = 7;
            InGameSystem.Instance.Mineral = 0;
            InGameSystem.Instance.KillPoint = 0;
            InGameSystem.Instance.CurrentRarePercentIndex = 1;
            UpgradeType highestUpgradeType = InGameSystem.Instance.GetHighestUpgradePower();
            InGameSystem.Instance.UpgradePower[highestUpgradeType] = 70;
        }
        else if (_Level == 3)
        {
            _Level = 4;
            _WaveNumber = 20;
            InGameSystem.Instance.NextWaveNumberForTest = _WaveNumber;
            InGameSystem.Instance.MineralStep = 12;
            InGameSystem.Instance.Mineral = 0;
            InGameSystem.Instance.KillPoint = 0;
            InGameSystem.Instance.CurrentRarePercentIndex = 2;
            UpgradeType highestUpgradeType = InGameSystem.Instance.GetHighestUpgradePower();
            InGameSystem.Instance.UpgradePower[highestUpgradeType] = 120;
        }
        else if (_Level == 4)
        {
            _Level = 5;
            _WaveNumber = 30;
            InGameSystem.Instance.NextWaveNumberForTest = _WaveNumber;
            InGameSystem.Instance.MineralStep = 20;
            InGameSystem.Instance.Mineral = 0;
            InGameSystem.Instance.KillPoint = 0;
            InGameSystem.Instance.CurrentRarePercentIndex = 3;
            UpgradeType highestUpgradeType = InGameSystem.Instance.GetHighestUpgradePower();
            InGameSystem.Instance.UpgradePower[highestUpgradeType] = 200;
        }
    }
    [Button("NewUnit")]
    public void NewUnit()
    {
        UpgradeType highestUpgradeType = InGameSystem.Instance.GetHighestUpgradePower();
        InGameSystem.Instance.CreateRandomNewUnit(highestUpgradeType, _Level);
    }

    // private const float PressDuration = 2;

    // private static TestPanelRelease mInst = null;
    // private bool mIsShow = false;
    // private GUIStyle mGuiStyle = null;

    // void Awake() 
    // {
    //     if(mInst == null)
    //     {
    //         mInst = this;
    //         DontDestroyOnLoad(this.gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    // void Start()
    // {
    //     StartCoroutine(OnOffPanel());
    // }

    // // 3초간 누르고 있으면 Test패널 킨다.
    // IEnumerator OnOffPanel()
    // {
    //     float pressingTime = 0;
    //     while(true)
    //     {
    //         yield return new WaitUntil(() => Keyboard.current.equalsKey.wasPressedThisFrame);

    //         pressingTime = 0;
    //         while(Keyboard.current.equalsKey.isPressed && pressingTime < PressDuration)
    //         {
    //             pressingTime += Time.deltaTime;
    //             yield return null;
    //         }

    //         mIsShow = pressingTime >= PressDuration;
    //         yield return null;
    //     }
    // }

    // void OnGUI()
    // {
    //     if(!mIsShow) return;

    //     if(mGuiStyle == null)
    //     {
    //         mGuiStyle = new GUIStyle(GUI.skin.button);
    //         mGuiStyle.fontSize = 30;
    //     }

    //     GUILayout.BeginVertical();

    //     if(GUILayout.Button("test패널 Off", mGuiStyle))
    //     {
    //         mIsShow = false;
    //     }

    //     if (GUILayout.Button("게임 데이터 초기화", mGuiStyle))
    //     {
    //     }

    //     if (GUILayout.Button("InGame 성공처리", mGuiStyle))
    //     {
    //     }

    //     GUILayout.EndVertical();
    // }
}
