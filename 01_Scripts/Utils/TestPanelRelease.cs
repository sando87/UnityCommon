using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 릴리즈 빌드 후 인게임상태에서 빠른 진행 위한 테스트 기능들 구현됨..
/// </summary>

public class TestPanelRelease : MonoBehaviour
{
    private const float PressDuration = 2;

    private static TestPanelRelease mInst = null;
    private bool mIsShow = false;
    private GUIStyle mGuiStyle = null;

    void Awake() 
    {
        if(mInst == null)
        {
            mInst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(OnOffPanel());
    }

    IEnumerator OnOffPanel()
    {
        float pressingTime = 0;
        while(true)
        {
            yield return new WaitUntil(() => Keyboard.current.equalsKey.wasPressedThisFrame);
            
            pressingTime = 0;
            while(Keyboard.current.equalsKey.isPressed && pressingTime < PressDuration)
            {
                pressingTime += Time.deltaTime;
                yield return null;
            }

            mIsShow = pressingTime >= PressDuration;
            yield return null;
        }
    }

    // void Update()
    // {
    //     if(Keyboard.current.equalsKey.wasPressedThisFrame)
    //     {
    //         mIsShow = !mIsShow;
    //     }
    // }

    void OnGUI()
    {
        if(!mIsShow) return;

        if(mGuiStyle == null)
        {
            mGuiStyle = new GUIStyle(GUI.skin.button);
            mGuiStyle.fontSize = 30;
        }

        GUILayout.BeginVertical();

        if(GUILayout.Button("test패널 Off", mGuiStyle))
        {
            mIsShow = false;
        }

        if (GUILayout.Button("게임 데이터 초기화", mGuiStyle))
        {
            GameFileManager<UserPlayInfo>.ResetUserDataFile();
            GameFileManager<UserSettingInfo>.ResetUserDataFile();
        }

        foreach(Suit suit in MyUtils.EnumForeach<Suit>())
        {
            if(GUILayout.Button("Suit " + suit, mGuiStyle))
            {
                SuitItemCover[] allSuitItem = FindObjectsOfType<SuitItemCover>();
                foreach(SuitItemCover item in allSuitItem)
                {
                    item.SetPrivateFieldValue("_SuitType", suit);
                }
            }
        }

        if (GUILayout.Button("InGame 성공처리", mGuiStyle))
        {
            InGameStarter starter = InGameStarter.FindInstance();
            if(starter != null)
            {
                starter.ClearStage();
            }
        }

        GUILayout.EndVertical();
    }
}
