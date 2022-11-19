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

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        // '='누르면 테스트 패널 사라짐
        // if(Input.GetKeyDown(KeyCode.Equals))
        // {
        //     mIsShow = !mIsShow;
        // }
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
