using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject로 되어있는 Data들에서 특정 id를 통해 데이터에 접근하는 방식

public abstract class ScriptableObjectDictionary<T1, T2> : ScriptableObjectTable<T2>
{
    // key, value 형태로 데이터 접급하기 위한 리스트
    protected Dictionary<T1, T2> DataDictionary = new Dictionary<T1, T2>();

    // key를 가져오기 위한 함수를 override해서 정의해야 함.
    protected abstract T1 GetID(T2 data);

    private void Init()
    {
        foreach (T2 data in Enums())
        {
            T1 id = GetID(data);
            DataDictionary[id] = data;
        }
    }

    // id를 통해 원하는 정보를 열람하는 함수
    public T2 GetDataOfId(T1 id) 
    { 
        if(DataDictionary.Count <= 0)
        {
            Init();
        }

        return DataDictionary[id];
    }
}

// How to use Sameple Code

// [CreateAssetMenu(fileName = "SampleTable", menuName = "Scriptable Object Asset/SampleTable")]
// public class SampleTable : ScriptableObjectTable<SampleInfo>
// {
//      protected override int GetID(SampleInfo data)
//      {
//          return data.sampleID;
//      }
// }

// [System.Serializable]
// public class SampleInfo
// {
//     public int sampleID;
//     public string sampleName;
// }
