using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class MySerializer
{
    public static void GetMySerializableInfos(Transform parent, List<MySerializableInfo> rets, string pathName = "")
    {
        MonoBehaviour[] comps = parent.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour comp in comps)
        {
            FieldInfo[] serializableFields = GetMySerializableInfos(comp);
            foreach (FieldInfo field in serializableFields)
            {
                MySerializableInfo info = new MySerializableInfo();
                info.childPath = pathName;
                info.scriptName = comp.GetType().Name;

                MySerializableAttribute att = field.GetCustomAttribute<MySerializableAttribute>();
                if (att != null)
                    info.IsHide = att.hide;

                if (field.FieldType.Name.Equals(typeof(CustomEnumSelector).Name))
                {
                    CustomEnumSelector fieldObj = (CustomEnumSelector)field.GetValue(comp);
                    if (fieldObj.SelectList == null || fieldObj.SelectList.Length <= 0)
                        continue;

                    info.fieldType = fieldObj.ToFieldType();
                    info.fieldName = field.Name;
                    info.fieldValue = fieldObj.SelectedIndex.ToString();
                    info.isEnum = false;
                }
                else if (field.FieldType.Name.Equals(typeof(CustomStringSelector).Name))
                {
                    CustomStringSelector fieldObj = (CustomStringSelector)field.GetValue(comp);
                    if (fieldObj.SelectList == null || fieldObj.SelectList.Length <= 0)
                        continue;

                    info.fieldType = fieldObj.ToFieldType();
                    info.fieldName = field.Name;
                    info.fieldValue = fieldObj.SelectedName;
                    info.isEnum = false;
                }
                else if (field.FieldType.Name.Equals(typeof(IntSelectorButton).Name))
                {
                    IntSelectorButton fieldObj = (IntSelectorButton)field.GetValue(comp);
                    if (fieldObj.SelectList == null || fieldObj.SelectList.Length <= 0)
                        continue;

                    info.fieldType = fieldObj.ToFieldType();
                    info.fieldName = field.Name;
                    info.fieldValue = fieldObj.SelectedIndex.ToString();
                    info.isEnum = false;
                }
                else if (field.FieldType.Name.Equals(typeof(Rect).Name))
                {
                    Rect rt = (Rect)field.GetValue(comp);
                    info.fieldType = field.FieldType.Name;
                    info.fieldName = field.Name;
                    info.fieldValue = rt.center.x + "," + rt.center.y + "," + rt.size.x + "," + rt.size.y;
                    info.isEnum = false;
                }
                else if (field.FieldType.IsEnum)
                {
                    info.fieldType = typeof(int).Name;
                    info.fieldName = field.Name;
                    info.fieldValue = ((int)field.GetValue(comp)).ToString();
                    info.isEnum = true;
                    info.enumNames = String.Join("/", field.FieldType.GetEnumNames());
                }
                else
                {
                    info.fieldType = field.FieldType.Name;
                    info.fieldName = field.Name;
                    info.fieldValue = field.GetValue(comp).ToString();
                    info.isEnum = false;
                }

                rets.Add(info);
            }
        }

        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
                continue;

            string pathname = pathName.Length == 0 ? (child.name) : (pathName + "/" + child.name);
            GetMySerializableInfos(child, rets, pathname);
        }
    }

    private static FieldInfo[] GetMySerializableInfos(MonoBehaviour comp)
    {
        Type type = comp.GetType();

        FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.Instance);

        List<FieldInfo> rets = new List<FieldInfo>();
        foreach (FieldInfo field in fields)
        {
            foreach (CustomAttributeData attributeData in field.CustomAttributes)
            {
                if (attributeData.AttributeType == typeof(MySerializableAttribute))
                {
                    rets.Add(field);
                    break;
                }
            }
        }

        return rets.ToArray();
    }

    public static MonoBehaviour FindTargetObject(Transform parent, MySerializableInfo info)
    {
        Transform go = info.childPath.Length <= 0 ? parent : parent.Find(info.childPath);
        if (go == null) return null;

        MonoBehaviour[] comps = go.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour comp in comps)
        {
            if (comp.GetType().Name.Equals(info.scriptName))
            {
                return comp;
            }
        }
        return null;
    }
    public static void SetValue(MonoBehaviour obj, MySerializableInfo info)
    {
        object infoValue = ParseValue(info.fieldType, info.fieldValue);
        obj.ExSetPrivateFieldValue(info.fieldName, infoValue);
    }
    public static T GetValue<T>(MonoBehaviour obj, MySerializableInfo info)
    {
        return obj.ExGetPrivateFieldValue<T>(info.fieldName);
    }
    private static object ParseValue(string fieldType, string value)
    {
        if (fieldType.Equals(typeof(int).Name))
            return int.Parse(value);
        else if (fieldType.Equals(typeof(float).Name))
            return float.Parse(value);
        else if (fieldType.Equals(typeof(bool).Name))
            return bool.Parse(value);
        else if (fieldType.Equals(typeof(long).Name))
            return long.Parse(value);
        else if (fieldType.Equals(typeof(string).Name))
            return value;
        else if (fieldType.Equals(typeof(Vector2Int).Name))
        {
            string[] pieces = value.Split(new string[] { "(", ",", " ", ")" }, StringSplitOptions.RemoveEmptyEntries);
            return new Vector2Int(int.Parse(pieces[0]), int.Parse(pieces[1]));
        }
        else if (fieldType.Equals(typeof(Rect).Name))
        {
            string[] pieces = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Rect rt = new Rect();
            rt.size = new Vector2(float.Parse(pieces[2]), float.Parse(pieces[3]));
            rt.center = new Vector2(float.Parse(pieces[0]), float.Parse(pieces[1]));
            return rt;
        }
        else if (fieldType.Contains(typeof(CustomEnumSelector).Name))
        {
            CustomEnumSelector ret = new CustomEnumSelector();
            ret.FromFieldType(fieldType);
            ret.SelectedIndex = int.Parse(value);
            return ret;
        }

        return null;
    }
}

[System.Serializable]
public struct MySerializableInfo
{
    public string childPath;
    public string scriptName;
    public string fieldType;
    public string fieldName;
    public string fieldValue;
    public bool isEnum;
    public string enumNames;

    public bool IsHide { get; set; }
}

[System.Serializable]
public class CustomEnumSelector
{
    //public int SelectedIndex = 0;
    public string[] SelectList = null;

    public int SelectedIndex { get; set; } = 0;

    public string ToFieldType()
    {
        string list = String.Join(",", SelectList);
        return typeof(CustomEnumSelector).Name + "(" + list + ")";
    }
    public void FromFieldType(string typeString)
    {
        string[] peices = typeString.Split(new string[] { "(", ")" }, StringSplitOptions.None);
        SelectList = peices[1].Split(new string[] { "," }, StringSplitOptions.None);
    }
}

[System.Serializable]
public class CustomStringSelector
{
    public string[] SelectList = null;

    public int SelectedIndex { get { return GetIndex(); } }
    public string SelectedName { get; set; } = "";

    public string ToFieldType()
    {
        string list = String.Join(",", SelectList);
        return typeof(CustomStringSelector).Name + "(" + list + ")";
    }
    public void FromFieldType(string typeString)
    {
        string[] peices = typeString.Split(new string[] { "(", ")" }, StringSplitOptions.None);
        SelectList = peices[1].Split(new string[] { "," }, StringSplitOptions.None);
    }
    int GetIndex()
    {
        for (int i = 0; i < SelectList.Length; ++i)
            if (SelectList[i].Equals(SelectedName))
                return i;
        return 0;
    }
}

[System.Serializable]
public class IntSelectorButton
{
    public int[] SelectList = new int[4] { 0, 90, 180, 270 };

    public int SelectedIndex { get; set; } = 0;

    public int CurValue { get { return SelectList[SelectedIndex]; } }

    public string ToFieldType()
    {
        string list = String.Join(",", SelectList);
        return typeof(IntSelectorButton).Name + "(" + list + ")";
    }
    public void FromFieldType(string typeString)
    {
        string[] peices = typeString.Split(new string[] { "(", ")" }, StringSplitOptions.None);
        string[] values = peices[1].Split(new string[] { "," }, StringSplitOptions.None);
        List<int> rets = new List<int>();
        foreach (string val in values)
            rets.Add(int.Parse(val));
        SelectList = rets.ToArray();
    }
}
