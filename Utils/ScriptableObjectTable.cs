using System.Collections;
using System.Collections.Generic;
using EditorGUITable;
using UnityEngine;

// ScriptableObject로 되어있는 Data들을 리스트를 돌거나 index를 통해 데이터에 접근하는 방식

public class ScriptableObjectTable<T> : ScriptableObject
{
    //[Table]
    [SerializeField]
    protected List<T> DataList = new List<T>();

    public T GetDataOfIndex(int index) { return DataList[index]; }
    public int Count { get { return DataList.Count; } }
    public IEnumerable<T> Enums()
    {
        for (int i = 0; i < DataList.Count; ++i)
            yield return DataList[i];
    }
}

// How to use Sameple Code

// [CreateAssetMenu(fileName = "SampleTable", menuName = "Scriptable Object Asset/SampleTable")]
// public class SampleTable : ScriptableObjectTable<SampleInfo>
// {
//      private static SampleTable mInst = null;
//      public static SampleTable Inst
//      {
//          get
//          {
//              if (mInst == null)
//              {
//                  mInst = Resources.Load<SampleTable>("Database/SampleTable");
//              }
//              return mInst;
//          }
//      }
// }

// [System.Serializable]
// public class SampleInfo
// {
//     public int sampleID;
//     public string sampleName;
// }
