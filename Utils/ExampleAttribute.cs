#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// How to use
/// [MySelector] string stateName;
/// </summary>

[AttributeUsage(AttributeTargets.Field)]
public class MySelectorAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(MySelectorAttribute))]
public class MySelectorDrawer : PropertyDrawer
{
    string[] list = null;
    int idx = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            base.OnGUI(position, property, label);
        else
        {
            idx = GetIndex(property.stringValue);

            EditorGUI.BeginChangeCheck();
            idx = EditorGUI.Popup(position, label.text, idx, list);

            if (EditorGUI.EndChangeCheck())  //Inspector창에서 콤보박스 선택시 진입
            {
                if (idx > 0)
                {
                    property.stringValue = list[idx];
                    // property.serializedObject.FindProperty("ActionID").intValue = actionIDList[idx];
                    // property.serializedObject.FindProperty("AnimationClipName").stringValue = animClipList[idx];
                }
            }
        }
    }

    private int GetIndex(string stateName)
    {
        if(list == null) return 0;

        for (int i = 0; i < list.Length; ++i)
        {
            if (list[i].Equals(stateName))
                return i;
        }
        return 0;
    }
}

#endif