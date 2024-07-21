using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorGUITable;
using UnityEditor.Rendering;
using UnityEditor.PackageManager.UI;

// 단순한 Data형태의 class 형태를 새로운 윈도우창에서 띄워서 수정하고자 할 때 사용

public class EditorPopupNewID : EditorWindow 
{
    string mText = "";
    System.Action<string> mEventReturn = null;

    // 내가 보고자 하는 data class 를 소유하고 있는 부모 객체를 target인자로 넣고 그 필드 이름을 같이 넣어줘야 함
    public static void ShowWindow(System.Action<string> eventNewID)
    {
        var window = GetWindow<EditorPopupNewID>();
        window.titleContent = new GUIContent("New ID");
        window.Init(eventNewID);
        window.Show();
    }

    public void Init(System.Action<string> eventNewID) { mEventReturn = eventNewID;}

    private void OnGUI()
    {
        mText = EditorGUILayout.TextField("Assign New ID", mText);

        if(GUILayout.Button("Add New ID"))
        {
            mEventReturn?.Invoke(mText);
            this.Close();
        }
    }
}
