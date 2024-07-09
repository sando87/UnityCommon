using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

// 배열형태의 데이터를 Json으로 파싱할때 편하게 사용할수있는 헬퍼

public static class JsonHelpper
{
    public static string ToJsonArray<T>(T[] objs)
    {
        List<T> list = new List<T>();
        list.AddRange(objs);
        string ret = JsonConvert.SerializeObject(list, Formatting.Indented);
        return ret;
    }
    
    public static T[] FromJsonArray<T>(string stream)
    {
        List<T> list = JsonConvert.DeserializeObject<List<T>>(stream);
        return list.ToArray();
    }

    public static List<T> FromJsonList<T>(string stream)
    {
        List<T> list = JsonConvert.DeserializeObject<List<T>>(stream);
        return list;
    }
}
