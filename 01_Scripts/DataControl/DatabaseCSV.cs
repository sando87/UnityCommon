using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스 개념
// 범용적으로 csv 포멧 파일 데이터 접근시 사용
//
// 사용법 예시
// 먼저 아래와 같이 csv 데이터 포멧에 맞는 구조체를 정의한 후
// [System.Serializable]
// public class CharactorInfo : ICSVFormat
// {
//     public int number;
//     public string name;
//     public int hp;

//     public int RowIndex { get; set; }
//     public long ID { get { return (long)number; } }
// }
//
// 위와 같이 csv 형태의 구조체 정의 후 아래와 같이 접근하여 사용
// T info = DatabaseCSV<CharactorInfo>.Instance.GetInfo(id);
// 참고로 데이터 파일이름은 구조체 class 이름으로 정의한다.(Resources/Database/CharactorInfo.csv)

public class DatabaseCSV<T> : Singleton<DatabaseCSV<T>> where T : ICSVFormat
{
    private bool mIsLoaded = false;
    private string mFullPathName = "";
    private List<T> mInfos = new List<T>();
    private Dictionary<string, T> mTable = new Dictionary<string, T>();

    // csv파일이 존재하는 path 예)"Assets/Resources/Database/"
    public void Init(string _filePath)
    {
        mInfos.Clear();
        mTable.Clear();

        string filename = typeof(T).Name;
        mFullPathName = _filePath + filename + ".csv";

        Load();
    }
    private void Load()
    {
        if (mTable.Count > 0)
            return;

        string filename = typeof(T).Name;
#if UNITY_EDITOR
        string spreadsheetId = "1pRpEq-zAwYvoB5N_D5H--NKltHOscvOcBu8uOAA3ph8"; // 구글 스프레드시트 id
        string sheetName = filename; // 구글 스프레드시트 시트 이름
        string csvFormatRawData = MyUtils.LoadGoogleSheetData(spreadsheetId, sheetName);
#else
        TextAsset ta = Resources.Load<TextAsset>("Database/" + filename);
        string csvFormatRawData = ta.text;
#endif

        T[] infos = CSVParser<T>.Deserialize(',', csvFormatRawData);
        for (int i = 0; i < infos.Length; ++i)
        {
            T info = infos[i];
            info.RowIndex = i;
            string key = info.ID;
            mTable[key] = info;
            mInfos.Add(info);
        }
        mIsLoaded = true;
    }
    public void EditAndSave(T info)
    {
#if UNITY_EDITOR
        if (mInfos == null || !mTable.ContainsKey(info.ID))
            return;

        mInfos[info.RowIndex] = info;
        mTable[info.ID] = info;

        string csvstring = CSVParser<T>.Serialize(',', mInfos);
        System.IO.File.WriteAllText(mFullPathName, csvstring);
#endif
    }
    public void AddNewAndSave(T info)
    {
#if UNITY_EDITOR
        if (mInfos == null || mTable.ContainsKey(info.ID))
            return;

        info.RowIndex = mInfos.Count;
        mInfos.Add(info);
        mTable[info.ID] = info;

        string csvstring = CSVParser<T>.Serialize(',', mInfos);
        System.IO.File.WriteAllText(mFullPathName, csvstring);
#endif
    }

    public bool HasInfo(string id)
    {
        return mTable.ContainsKey(id);
    }
    public T GetInfo(string id)
    {
        return mTable[id];
    }
    public T GetInfoOfIndex(int index)
    {
        return mInfos[index];
    }
    public T[] GetAllInfo()
    {
        return new List<T>(mTable.Values).ToArray();
    }
    public List<string> GetAllIDs()
    {
        List<string> ids = new List<string>();
        foreach (T info in mInfos)
            ids.Add(info.ID);
        return ids;
    }
    public IEnumerable<T> Enums()
    {
        foreach (var item in mTable)
            yield return item.Value;
    }
}


// 실제 구조체에서 아래 항목을 override해서 사용
public interface ICSVFormat
{
    string ID { get { return ""; } } // 데이터 접근을 위한 id값
    int RowIndex { get; set; } // 전체 csv 테이블상에서 각 Row의 인덱스정보
}

