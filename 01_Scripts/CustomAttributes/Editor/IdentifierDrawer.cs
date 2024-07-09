#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IdentifierAttribute))]
public class IdentifierDrawer : PropertyDrawer
{
    Dictionary<long, int> mIDTable = new Dictionary<long, int>();
    // static Dictionary<long, string> mIDObjectTable = new Dictionary<long, string>();

    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    // {
    //     if(!Application.isPlaying && property.propertyType == SerializedPropertyType.Integer)
    //     {
    //         long id = property.longValue;
    //         if(id != 0 && !mIDObjectTable.ContainsKey(id))
    //         {
    //             string guid = GetGUID(property);
    //             if(guid.Length > 0)
    //                 mIDObjectTable[id] = guid;
    //         }
    //     }

    //     return base.GetPropertyHeight(property, label);
    // }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Application.isPlaying)
        {
            base.OnGUI(position, property, label);
            return;
        }

        if (property.propertyType != SerializedPropertyType.Integer)
            base.OnGUI(position, property, label);
        else
        {
            // 필드가 배열 속성일 경우와 아닌경우로 나뉘어 처리된다.
            if (property.name.Equals(property.propertyPath))
            {
                MonoBehaviour mo = property.serializedObject.targetObject as MonoBehaviour;
                // IsPersistent는 디스크에 파일 형태로 존재하는지 여부 파악(Asset형태의 프리팹을 확인하는 방법)
                if (EditorUtility.IsPersistent(mo.gameObject))
                {
                    string guid = GetGUID(property);
                    if (guid.Length > 0)
                    {
                        long newResID = MyUtils.GUIDToLong(guid);
                        if (newResID != property.longValue)
                        {
                            property.longValue = newResID;
                            EditorUtility.SetDirty(mo.gameObject);
                        }
                    }
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
                        if (mIDTable[property.longValue] < arrayIndex)
                        {
                            long id = DateTime.Now.Ticks;
                            mIDTable[id] = arrayIndex;
                            property.longValue = id;
                        }
                        else if (mIDTable[property.longValue] > arrayIndex)
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

    // public long AssignNewID(SerializedProperty property, long preID)
    // {
    //     string guid = GetGUID(property);
    //     if(guid.Length <= 0)
    //         return 0;

    //     if(mIDObjectTable.ContainsKey(preID))
    //     {
    //         if(!mIDObjectTable[preID].Equals(guid))
    //         {
    //             long newID = MyUtils.GUIDToLong(guid);
    //             mIDObjectTable[newID] = guid;
    //             return newID;
    //         }
    //     }
    //     else
    //     {
    //         long newID = MyUtils.GUIDToLong(guid);
    //         mIDObjectTable[newID] = guid;
    //         return newID;
    //     }
    //     return 0;
    // }

    string GetGUID(SerializedProperty property)
    {
        MonoBehaviour mo = property.serializedObject.targetObject as MonoBehaviour;
        return MyUtils.GetGUIDFromPrefab(mo.gameObject);
    }
}

#endif