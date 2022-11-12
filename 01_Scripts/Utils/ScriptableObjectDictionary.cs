using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject로 되어있는 Data들에서 특정 id를 통해 데이터에 접근하는 방식

public abstract class ScriptableObjectDictionary<TypeID, TypeData> : ScriptableObjectTable<TypeData>
{
    // key, value 형태로 데이터 접급하기 위한 리스트
    protected Dictionary<TypeID, TypeData> DataDictionary = new Dictionary<TypeID, TypeData>();

    // key를 가져오기 위한 함수를 override해서 정의해야 함.
    protected abstract TypeID GetID(TypeData data);

    private void Init()
    {
        foreach (TypeData data in Enums())
        {
            TypeID id = GetID(data);
            DataDictionary[id] = data;
        }
    }

    // id를 통해 원하는 정보를 열람하는 함수
    public TypeData GetDataOfId(TypeID id)
    { 
        if(DataDictionary.Count <= 0)
        {
            Init();
        }

        if(!DataDictionary.ContainsKey(id))
            return default;

        return DataDictionary[id];
    }
}

// How to use Sameple Code

// [CreateAssetMenu(fileName = "SampleTable", menuName = "Scriptable Object Asset/SampleTable")]
// public class SampleTable : ScriptableObjectDictionary<int, SampleInfo>
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
