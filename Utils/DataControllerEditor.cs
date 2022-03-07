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
                ViewBase currentView = FindObjectOfType<ViewBase>();
                if(currentView != null) // 현재 Main 화면이면
                {
                    currentView.UpdateUIState();
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
                ViewBase currentView = FindObjectOfType<ViewBase>();
                if (currentView != null) // 현재 Main 화면이면
                {
                    currentView.UpdateUIState();
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

}
#endif