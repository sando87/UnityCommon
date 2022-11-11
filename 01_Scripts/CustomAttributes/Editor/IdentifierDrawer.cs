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
    static Dictionary<long, string> mIDObjectTable = new Dictionary<long, string>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(!Application.isPlaying && property.propertyType == SerializedPropertyType.Integer)
        {
            long id = property.longValue;
            if(id != 0 && !mIDObjectTable.ContainsKey(id))
            {
                mIDObjectTable[id] = GetGUID(property);
            }
        }
        
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(Application.isPlaying)
        {
            base.OnGUI(position, property, label);
            return;
        }
            
        if (property.propertyType != SerializedPropertyType.Integer)
            base.OnGUI(position, property, label);
        else
        {

            // 필드가 배열 속성일 경우와 아닌경우로 나뉘어 처리된다.
            if(property.name.Equals(property.propertyPath))
            {
                long newID = AssignNewID(property, property.longValue);
                if(newID != 0)
                {
                    property.longValue = newID;
                }
            }
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
            }

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    public long AssignNewID(SerializedProperty property, long preID)
    {
        string guid = GetGUID(property);
        if(mIDObjectTable.ContainsKey(preID))
        {
            if(!mIDObjectTable[preID].Equals(guid))
            {
                long newID = MyUtils.GUIDToLong(guid);
                mIDObjectTable[newID] = guid;
                return newID;
            }
        }
        else
        {
            long newID = MyUtils.GUIDToLong(guid);
            mIDObjectTable[newID] = guid;
            return newID;
        }
        return 0;
    }

    string GetGUID(SerializedProperty property)
    {
        MonoBehaviour mo = property.serializedObject.targetObject as MonoBehaviour;
        return MyUtils.GetGUIDFromPrefab(mo.gameObject);
    }
}

#endif