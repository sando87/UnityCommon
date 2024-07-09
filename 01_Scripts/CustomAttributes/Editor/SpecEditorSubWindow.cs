using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorGUITable;
using UnityEditor.Rendering;

// 단순한 Data형태의 class 형태를 새로운 윈도우창에서 띄워서 수정하고자 할 때 사용

public class SpecEditorSubWindow : EditorWindow 
{
    SerializedObject mSO = null;
    SerializedProperty mSP = null;

    // 내가 보고자 하는 data class 를 소유하고 있는 부모 객체를 target인자로 넣고 그 필드 이름을 같이 넣어줘야 함
    public static void ShowWindow(UnityEngine.Object target, string fieldName)
    {
        var window = GetWindow<SpecEditorSubWindow>();
        window.titleContent = new GUIContent("Edit Data");
        window.Init(target, fieldName);
        window.Show();
    }

    void Init(UnityEngine.Object target, string fieldName)
    {
        mSO = new SerializedObject(target);
        mSP = mSO.FindProperty(fieldName);
    }

    private void OnGUI() 
    {
        EditorGUILayout.LabelField("Edit Data", GUILayout.Width(200), GUILayout.Height(30));

        EditorGUILayout.PropertyField(mSP);

        mSO.ApplyModifiedProperties();
    }
}
