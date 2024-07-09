using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EditorGUITable;
using UnityEngine;

// ScriptableObject로 되어있는 Data들을 리스트를 돌거나 index를 통해 데이터에 접근하는 방식

public interface ISpecEditorWindow
{
    string[] GetFieldNames() { return null; } // 데이터 테이블의 컬럼 이름들이 반환
}

public class ScriptableObjectTable<T> : ScriptableObject, ISpecEditorWindow
{
    [ReorderableTable]
    [SerializeField]
    protected List<T> DataList = new List<T>();

    public T GetDataOfIndex(int index) { return DataList[index]; }
    public int Count { get { return DataList.Count; } }
    public IEnumerable<T> Enums()
    {
        for (int i = 0; i < DataList.Count; ++i)
            yield return DataList[i];
    }
    private string[] FieldNames()
    {
        List<string> rets = new List<string>();
        Type type = typeof(T);
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
            rets.Add(field.Name);

        return rets.ToArray();
    }
    string[] ISpecEditorWindow.GetFieldNames()
    {
        return FieldNames();
    }

}

// How to use Sameple Code

// [CreateAssetMenu(fileName = "SampleTable", menuName = "Scriptable Object Asset/SampleTable")]
// public class SampleTable : ScriptableObjectTable<SampleInfo>
// {
// }

// [System.Serializable]
// public class SampleInfo
// {
//     public int sampleID;
//     public string sampleName;
// }
