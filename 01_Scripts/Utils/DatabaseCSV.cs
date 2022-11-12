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
    private Dictionary<long, T> mTable = new Dictionary<long, T>();

    public DatabaseCSV()
    {
        Load();
    }
    private void Load()
    {
        if(mTable.Count > 0)
            return;

        string filename = typeof(T).Name;
        TextAsset ta = Resources.Load<TextAsset>("Database/" + filename);
        T[] infos = CSVParser<T>.Deserialize(',', ta.text);
        for(int i = 0; i < infos.Length; ++i)
        {
            T info = infos[i];
            info.RowIndex = i;
            long key = info.ID >= 0 ? info.ID : info.RowIndex;
            mTable[key] = info;
        }
    }

    public T GetInfo(long id)
    {
        return mTable[id];
    }
    public T[] GetAllInfo()
    {
        return new List<T>(mTable.Values).ToArray();
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
    long ID { get; } // 데이터 접근을 위한 id값 (음수면 ID방식 disable)
    int RowIndex { get; set; } // 전체 csv 테이블상에서 각 Row의 인덱스정보
}

