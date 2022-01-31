using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVAgePointData
{
    public int index;
    public int ageIndex;
    public int buildingIndex;
    public int score;
    public string ageName;

    public string IdleAnimName { get { return "idle_" + ageName + "_" + buildingIndex; } }
    public string BuildAnimName { get { return "animation_" + ageName + "_" + buildingIndex; } }
    public string NextIdleAnimName { get { return "idle_" + ageName + "_" + (buildingIndex + 1); } }
}

public class CSVAgePointTable
{
    public static CSVAgePointData[] mTable = null;

    public static int Count { get { return mTable.Length; } }
    
    public static CSVAgePointData GetData(int tableIndex)
    {
        if(mTable == null)
        {
            Load();
        }

        if(mTable.Length <= tableIndex)
            return null;
            
        return mTable[tableIndex];
    }
    public static CSVAgePointData[] GetAgeDataList(int ageIndex)
    {
        if (mTable == null)
        {
            Load();
        }

        List<CSVAgePointData> rets = new List<CSVAgePointData>();
        foreach(CSVAgePointData data in mTable)
        {
            if(data.ageIndex == ageIndex)
                rets.Add(data);
        }

        return rets.ToArray();
    }
    private static void Load()
    {
        TextAsset ts = Resources.Load<TextAsset>("agePointTable");
        mTable = CSVParser<CSVAgePointData>.Deserialize(',', ts.text);
    }
}
