using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스 개념
// 범용적으로 json포멧의 테이블 형태의 파일 데이터 접근시 사용
//
// 사용법 예시
// 먼저 아래와 같이 json테이블 데이터 포멧에 맞는 구조체를 정의한 후
// [System.Serializable]
// public class CharactorInfo : IJsonFormat
// {
//     public int number;
//     public string name;
//     public int hp;

//     public int RowIndex { get; set; }
//     public long ID { get { return (long)number; } }
// }
//
// 위와 같은 형태의 구조체 정의 후 아래와 같이 접근하여 사용
// T info = DatabaseJSON<CharactorInfo>.Instance.GetInfo(id);
// 참고로 데이터 파일이름은 구조체 class 이름으로 정의한다. Ex) Resources/Database/CharactorInfo.json

public class DatabaseJSON<T> : Singleton<DatabaseJSON<T>> where T : IJsonFormat
{
    private T[] mInfos = null;
    private Dictionary<long, T> mTable = new Dictionary<long, T>();

    public DatabaseJSON()
    {
        Load();
    }
    private void Load()
    {
        if(mTable.Count > 0)
            return;

        string filename = typeof(T).Name;
        TextAsset ta = Resources.Load<TextAsset>("Database/" + filename);
        mInfos = JsonHelpper.FromJsonArray<T>(ta.text);
        for(int i = 0; i < mInfos.Length; ++i)
        {
            T info = mInfos[i];
            info.RowIndex = i;
            long key = info.ID;
            mTable[key] = info;
        }
    }
    public void Save(T info)
    {
#if UNITY_EDITOR
        if(mInfos == null || mInfos.Length <= 0)
            return;
        
        mInfos[info.RowIndex] = info;
        mTable[info.ID] = info;
        string filename = typeof(T).Name;
        string fullname = "./Assets/00_MetaSuit/Resources/Database/" + filename + ".json";
        string jsonString = JsonHelpper.ToJsonArray<T>(mInfos);
        System.IO.File.WriteAllText(fullname, jsonString);
#endif
    }

    public T GetInfo(long id)
    {
        return mTable[id];
    }
    public T GetInfoOfIndex(int index)
    {
        return mInfos[index];
    }
    public T[] GetAllInfo()
    {
        return mInfos;
    }
    public IEnumerable<T> Enums()
    {
        foreach (var item in mTable)
            yield return item.Value;
    }
}


// 실제 구조체에서 아래 항목을 override해서 사용
public interface IJsonFormat
{
    long ID { get { return -1; } } // 데이터 접근을 위한 id값
    int RowIndex { get; set; } // 전체 테이블상에서 각 Row의 인덱스정보
}

