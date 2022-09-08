#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Animations;

[CustomPropertyDrawer(typeof(IdentifierAttribute))]
public class IdentifierDrawer : PropertyDrawer
{
    Dictionary<long, UnityEngine.Object> mIDTable = new Dictionary<long, UnityEngine.Object>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
            base.OnGUI(position, property, label);
        else
        {
            if (property.longValue <= 0)
            {
                long id = DateTime.Now.Ticks;
                mIDTable[id] = property.serializedObject.targetObject;
                property.longValue = id;
            }
            else
            {
                if(mIDTable.ContainsKey(property.longValue))
                {
                    if(mIDTable[property.longValue] != property.serializedObject.targetObject)
                    {
                        long id = DateTime.Now.Ticks;
                        mIDTable[id] = property.serializedObject.targetObject;
                        property.longValue = id;
                    }
                }
                else
                {
                    mIDTable[property.longValue] = property.serializedObject.targetObject;
                }

            }

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}

#endif