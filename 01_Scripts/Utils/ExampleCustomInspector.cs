#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExampleClass : MonoBehaviour
{
    public SystemLanguage language;
    public string langName = "";

    void Start() { }
    void Update() { }
}

[CustomEditor(typeof(ExampleClass))]
public class ExampleClassEditor : Editor
{
    public ExampleClass selected;
    SerializedProperty property;
    
    private void OnEnable()
    {
        if (AssetDatabase.Contains(target))
        {
            selected = null;
        }
        else
        {
            selected = (ExampleClass)target;
        }
    }

    public override void OnInspectorGUI()
    {
        if (selected == null)
            return;

        selected.language = (SystemLanguage)EditorGUILayout.EnumPopup("언어 선택", selected.language);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("langName"));

        //base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("------------------------------------------------------------------------");
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif