using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 실제 구조체에서 아래 항목을 override해서 사용
public interface IUnityFormat
{
    long ID { get { return -1; } } // 데이터 접근을 위한 id값
    int RowIndex { get; set; } // 전체 테이블상에서 각 Row의 인덱스정보
}

// ScriptableObject로 되어있는 Data들에서 특정 id를 통해 데이터에 접근하는 방식

public class ScriptableObjectDictionary<TypeInst, TypeItem> : ScriptableObjectTable<TypeItem>
where TypeInst : ScriptableObject
where TypeItem : IUnityFormat
{
    private static TypeInst mInst = null;
    public static TypeInst Instance
    {
        get
        {
            if (mInst == null)
                Load();
            return mInst;
        }
    }

    // key, value 형태로 데이터 접급하기 위한 리스트
    protected Dictionary<long, TypeItem> mTable = new Dictionary<long, TypeItem>();

    public static void CreateNewScriptableAssetFile()
    {
#if UNITY_EDITOR
        MyUtils.CreateScriptableObjectFileAsset<TypeInst>("Assets/00_MetaSuit/Resources/Database/");
#endif        
    }

    private static void Load()
    {
        string filename = typeof(TypeInst).Name;
        mInst = Resources.Load<TypeInst>("Database/" + filename);
        (mInst as ScriptableObjectDictionary<TypeInst, TypeItem>).InitData();
    }
    public void InitData()
    {
        for (int i = 0; i < Count; ++i)
        {
            TypeItem data = GetDataOfIndex(i);
            data.RowIndex = i;
            long id = data.ID;
            mTable[id] = data;
        }
    }
    public void Save(TypeItem info)
    {
#if UNITY_EDITOR
        if (mTable.Count <= 0)
            return;

        DataList[info.RowIndex] = info;
        mTable[info.ID] = info;
        Save();
#endif
    }
    public void Save(TypeItem[] infos)
    {
#if UNITY_EDITOR
        DataList.Clear();
        DataList.AddRange(infos);
        Save();

        mTable.Clear();
        InitData();
#endif
    }
    public void Save()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(mInst);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    // id를 통해 원하는 정보를 열람하는 함수
    public bool HasInfo(long id)
    {
        return mTable.ContainsKey(id);
    }
    public TypeItem GetInfo(long id)
    {
        return mTable[id];
    }
    public TypeItem[] GetAllInfo()
    {
        return DataList.ToArray();
    }
}

