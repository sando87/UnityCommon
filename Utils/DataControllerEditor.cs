using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DataControllerEditor : EditorWindow
{
    public UserSaveData mUserData = null;

    [MenuItem("/Users/DataControllerEditor %t")]
    private static void ShowWindow()
    {
        var window = GetWindow<DataControllerEditor>();
        window.titleContent = new GUIContent("DataControllerEditor");
        GamePlayData.Load();
        window.mUserData = GamePlayData.UserSaveData;
        window.Show();

    }

    private void OnGUI()
    {
        int buttonGap = 5;
        GUILayout.BeginVertical();

        GUILayout.Space(buttonGap);
        GUILayout.Label("===========================================");
        GUILayout.Space(buttonGap);

        if (GUILayout.Button("Building Complete"))
        {
            if( EditorApplication.isPlaying ) //플레이중이면
            {
                ViewMain mainView = FindObjectOfType<ViewMain>();
                if(mainView != null) // 현재 Main 화면이면
                {
                    int agesTableIndex = GamePlayData.UserSaveData.AgeTableIndex;
                    CSVAgePointData curAgeInfo = CSVAgePointTable.GetData(agesTableIndex);
                    if (curAgeInfo != null) 
                    {
                        GamePlayData.UserSaveData.IngameScore = curAgeInfo.score - GamePlayData.UserSaveData.CurrentBuildingScore;
                        mainView.InvokePrivateMethod("UpdateAgeState", null);
                    }
                }
            }
        }

        GUILayout.Space(buttonGap);
        if (GUILayout.Button("Reserve1"))
        {
        }

        GUILayout.Space(buttonGap);
        if (GUILayout.Button("Reserve2"))
        {
        }

        GUILayout.Space(buttonGap);
        GUILayout.Label("===========================================");
        GUILayout.Space(buttonGap);

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty textProperty = serializedObject.FindProperty("mUserData");
        EditorGUILayout.PropertyField(textProperty);
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(buttonGap);

        if (GUILayout.Button("Apply"))
        {
            // 현재 사용자 데이터 파일로 세이브
            GamePlayData.Save();

            if (EditorApplication.isPlaying) //플레이중이면
            {
                ViewMain mainView = FindObjectOfType<ViewMain>();
                if (mainView != null) // 현재 Main 화면이면
                {
                    mainView.InvokePrivateMethod("UpdateAgeState", null);
                }
            }
        }
        if (GUILayout.Button("Refresh"))
        {
            // 사용자 데이터 재로딩
            GamePlayData.Load();
            mUserData = GamePlayData.UserSaveData;
        }

        if(GUILayout.Button("Reset"))
        {
            // 사용자 데이터 초기화 기능
            GamePlayData.ResetUserDataFile();
            mUserData = GamePlayData.UserSaveData;
        }

        GUILayout.EndVertical();
    }

    // 단축키 Cmd + e 로 현재 인스펙터 잠금 기능 
    [MenuItem("/Users/Toggle Inspector Lock %e")]
    private static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}
#endif