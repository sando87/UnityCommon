using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomInspectorExample))]
public class CustomInspectorEditor : Editor
{
    public CustomInspectorExample selected;
    SerializedProperty property;
    
    private void OnEnable()
    {
        if (AssetDatabase.Contains(target))
        {
            selected = null;
        }
        else
        {
            selected = (CustomInspectorExample)target;
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
