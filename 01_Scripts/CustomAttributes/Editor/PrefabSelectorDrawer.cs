#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabSelectorAttribute))]
public class PrefabSelectorDrawer : PropertyDrawer
{
    string[] list = null;
    int idx = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            base.OnGUI(position, property, label);
        else
        {
            if (list == null)
            {
                List<string> names = new List<string>();
                names.Add("<None>");
                string path = ((PrefabSelectorAttribute)attribute).path;
                GameObject[] prefabs = Resources.LoadAll<GameObject>(path);
                foreach (GameObject prefab in prefabs)
                    names.Add(prefab.name);

                list = names.ToArray();
                idx = 0;
            }

            if(list != null && list.Length > 0)
            {
                idx = GetIndex(property.stringValue);

                EditorGUI.BeginChangeCheck();
                idx = EditorGUI.Popup(position, label.text, idx, list);

                if (EditorGUI.EndChangeCheck())  //Inspector창에서 콤보박스 선택시 진입
                {
                    property.stringValue = idx == 0 ? "" : list[idx];
                }
            }
        }
    }

    private int GetIndex(string name)
    {
        if(list == null) return 0;

        for (int i = 0; i < list.Length; ++i)
        {
            if (list[i].Equals(name))
                return i;
        }
        return 0;
    }
}

#endif