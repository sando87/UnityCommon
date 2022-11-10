using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// 릴리즈 빌드 후 인게임상태에서 빠른 진행 위한 테스트 기능들 구현됨..
/// </summary>

public class TestPanel : MonoBehaviour
{
    [SerializeField] int _WaveNumber = 1;
    [SerializeField] int _Level = 1;
    [Dropdown("GetUnitIDs")]
    public long UnitID;
    private DropdownList<long> GetUnitIDs()
    {
        DropdownList<long> units = new DropdownList<long>();
        foreach(var unit in UserCharactors.Inst.Enums())
            units.Add(unit.name, unit.ID);

        return units;
    }

    private bool mIsShow = true;
    private GUIStyle mGuiStyle = null;

    void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
    }
    

    void Update()
    {
        // '='누르면 테스트 패널 사라짐
        if(Input.GetKeyDown(KeyCode.Equals))
        {
            mIsShow = !mIsShow;
        }
    }

    [Button]
    void CreateNewUnit()
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

    // void OnGUI()
    // {
    //     if(!mIsShow) return;

    //     if(mGuiStyle == null)
    //     {
    //         mGuiStyle = new GUIStyle(GUI.skin.button);
    //         mGuiStyle.fontSize = 30;
    //     }

    //     GUILayout.BeginVertical();

    //     if (GUILayout.Button("'='키 눌러서 test패널 On/Off", mGuiStyle))
    //     {
    //         mIsShow = !mIsShow;
    //     }

    //     if (GUILayout.Button("게임 데이터 초기화", mGuiStyle))
    //     {
    //     }

    //     if (GUILayout.Button("기능 1", mGuiStyle))
    //     {
    //     }

    //     if (GUILayout.Button("기능 2", mGuiStyle))
    //     {
    //     }

    //     if (GUILayout.Button("InGame 성공처리", mGuiStyle))
    //     {
    //     }

    //     GUILayout.EndVertical();
    // }
}
