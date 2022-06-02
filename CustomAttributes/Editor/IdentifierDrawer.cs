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
    Dictionary<long, int> mIDTable = new Dictionary<long, int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
            base.OnGUI(position, property, label);
        else
        {
            string[] pieces = property.propertyPath.Split(new char[] { '[', ']' });
            int arrayIndex = int.Parse(pieces[1]);

            if (property.longValue <= 0)
            {
                long id = DateTime.Now.Ticks;
                mIDTable[id] = arrayIndex;
                property.longValue = id;
            }
            else
            {
                if (mIDTable.ContainsKey(property.longValue))
                {
                    if(mIDTable[property.longValue] < arrayIndex)
                    {
                        long id = DateTime.Now.Ticks;
                        mIDTable[id] = arrayIndex;
                        property.longValue = id;
                    }
                    else if(mIDTable[property.longValue] > arrayIndex)
                    {
                        mIDTable[property.longValue] = arrayIndex;
                    }
                }
                else
                {
                    mIDTable[property.longValue] = arrayIndex;
                }
            }            

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}

#endif