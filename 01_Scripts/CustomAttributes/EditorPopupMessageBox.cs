using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorGUITable;
using UnityEditor.Rendering;
using UnityEditor.PackageManager.UI;

// 단순한 Data형태의 class 형태를 새로운 윈도우창에서 띄워서 수정하고자 할 때 사용

public class EditorPopupMessageBox : EditorWindow 
{
    string mMessage = "";

    // 내가 보고자 하는 data class 를 소유하고 있는 부모 객체를 target인자로 넣고 그 필드 이름을 같이 넣어줘야 함
    public static void ShowWindow(string message)
    {
        var window = GetWindow<EditorPopupMessageBox>();
        window.titleContent = new GUIContent("MessageBox");
        window.Init(message);
        window.Show();
    }

    public void Init(string _message) { mMessage = _message;}

    private void OnGUI()
    {
        EditorGUILayout.LabelField(mMessage);

        if(GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
