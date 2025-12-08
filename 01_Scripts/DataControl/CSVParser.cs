using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// CSV형태의 데이터파일 파싱한다
/// <사용법 예>
/// test.csv 파일
/// index, name, age
/// 0, kim, 20
/// 1, lee, 22
/// 2, park, 23
/// [System.Serializable]
/// public class Person
/// {
///     public int index;
///     public string name;
///     public int age;
/// }
/// Person[] persons = CSVParser<Person>.Deserialize(',', csvStringText);
/// </summary>


//csv 파일을 바로 구조체 형태까지 파싱해준다.
public class CSVParser<TEntity>//where TEntity : class
{
    private static char _csvSeparator;
    private static string CsvSeparator;
    public static string Serialize(char csvSeparator, List<TEntity> collection)
    {
        return Serialize(csvSeparator, collection.ToArray());
    }
    public static string Serialize(char csvSeparator, params TEntity[] collection)
    {
        _csvSeparator = csvSeparator;
        CsvSeparator = csvSeparator.ToString();
        return CSVParser<TEntity>.CustomSerialize(collection);
    }
    private static string CustomSerialize(params TEntity[] collection)
    {
        StringBuilder sbColumns = new StringBuilder();
        StringBuilder sbRows = new StringBuilder();
        FieldInfo[] pairs = typeof(TEntity).GetFields();
        MountCsvColumns(ref sbColumns, pairs);
        for (int j = 0; j < collection.Length; j++)
        {
            MountCsvRows(ref sbRows, collection[j], pairs);
        }
        return sbColumns.ToString() + sbRows.ToString();
    }
    private static void MountCsvColumns(ref StringBuilder sbColumns, params FieldInfo[] pairs)
    {
        for (int i = 0; i < pairs.Length; i++)
        {
            if (i == pairs.Length - 1)
            {
                sbColumns.AppendLine(pairs[i].Name + CsvSeparator);
            }
            else
            {
                sbColumns.Append(pairs[i].Name).Append(CsvSeparator);
            }
        }
    }
    private static void MountCsvRows(ref StringBuilder sbRows, TEntity obj, params FieldInfo[] pairs)
    {
        for (int i = 0; i < pairs.Length; i++)
        {
            string result = string.Empty;
            FieldInfo field = obj.GetType().GetField(pairs[i].Name);
            if (null != field.GetValue(obj))
            {
                result = ObjectToString(field.GetValue(obj), pairs[i].FieldType);
            }

            if (i == pairs.Length - 1)
            {
                sbRows.AppendLine(result + CsvSeparator);
            }
            else
            {
                sbRows.Append(result).Append(CsvSeparator);
            }
        }
    }
    public static TEntity[] Deserialize(char csvSeparator, string csvString)
    {
        _csvSeparator = csvSeparator;
        CsvSeparator = csvSeparator.ToString();
        List<TEntity> results = new List<TEntity>();
        foreach (TEntity ret in CustomDeserialize(csvString))
        {
            results.Add(ret);
        }
        return results.ToArray();
    }
    private static IEnumerable<TEntity> CustomDeserialize(string csvString)
    {
        var recordPattern = new Regex(
            @"(?:""(?:""""|[^""])*""(?:,|$))+",
            RegexOptions.Compiled | RegexOptions.Multiline);

        var fieldPattern = new Regex(
            @"""((?:[^""]|"""")*)""(?:,|$)",
            RegexOptions.Compiled);

        List<Match> matches = recordPattern.Matches(csvString).ToList();

        // string[] columnsName = arrayLinesCsv[0].Split(_csvSeparator);
        // string[] columnsName = System.Text.RegularExpressions.Regex.Split(arrayLinesCsv[0], pattern);
        List<string> columnsName = new List<string>();
        var parts = fieldPattern.Matches(matches[0].Value);
        foreach (Match part in parts)
        {
            string value = part.Groups[1].Value.Replace("\"\"", "\""); // 이중 따옴표 처리

            // 컬럼 이름에서 언더바(_) 아래는 지운다
            int clipIndex = value.IndexOf('_');
            if (clipIndex >= 0)
                value = value.Substring(0, clipIndex);

            columnsName.Add(value);
        }

        // for (int j = 0; j < columnsName.Length; j++)
        // {
        //     columnsName[j] = columnsName[j].Trim('"');
        // }

        Type tp = typeof(TEntity);
        FieldInfo[] fields = tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        PropertyInfo[] props = tp.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (int i = 1; i < matches.Count; i++)
        {
            object instance = Activator.CreateInstance(tp);

            // string[] columnsValue = arrayLinesCsv[i].Split(_csvSeparator);
            // string[] columnsValue = System.Text.RegularExpressions.Regex.Split(arrayLinesCsv[i], pattern);
            List<string> columnsValue = new List<string>();
            var subMatch = fieldPattern.Matches(matches[i].Value);
            foreach (Match sub in subMatch)
            {
                string value = sub.Groups[1].Value.Replace("\"\"", "\""); // 이중 따옴표 처리
                columnsValue.Add(value);
            }

            for (int j = 0; j < columnsValue.Count; j++)
            {
                if (columnsName[j].Length <= 0) continue;

                for (int x = 0; x < fields.Length; x++)
                {
                    if (columnsName[j] == fields[x].Name)
                    {
                        if (columnsValue[j] != null && columnsValue[j].Length > 0)
                        {
                            object realObject = StringToObject(columnsValue[j].Trim('"'), fields[x].FieldType);
                            fields[x].SetValue(instance, realObject);
                            break;
                        }
                    }
                }

                for (int x = 0; x < props.Length; x++)
                {
                    if (columnsName[j] == props[x].Name)
                    {
                        if (columnsValue[j] != null && columnsValue[j].Length > 0 && props[x].CanWrite)
                        {
                            object realObject = StringToObject(columnsValue[j].Trim('"'), props[x].PropertyType);
                            props[x].SetValue(instance, realObject);
                            break;
                        }
                    }
                }
            }
            yield return (TEntity)instance;
        }
    }

    // 범용 object 객체를 해당 타입에 따라 특정 형태의 string으로 변환
    private static string ObjectToString(object value, Type objType)
    {
        string result = string.Empty;
        // Field 타입의 base클래스가 UnityEngine.Object일 경우 guid로 변환된다
        if (MyUtils.IsSubType(objType, typeof(UnityEngine.Object)))
        {
#if UNITY_EDITOR
            result = MyUtils.GetGUIDFromAsset(value as UnityEngine.Object);
#endif
        }
        else
        {
            // Field 타입이 class이거나 struct 타입이면 byteArray의 hexString으로 변환
            TypeCode typeCode = Type.GetTypeCode(objType);
            if (typeCode == TypeCode.Object)
            {
                result = JsonUtility.ToJson(value);
                //Json안에 콤마가 있는경우 대체문자열 #으로 임시 변환하고 나중에 다시 복구할때 반대 수행
                result = result.Replace(CsvSeparator, "#");
            }
            else
            {
                // Field 타입이 기본타입이면 해당 타입에 맞는 string으로 변환
                result = Convert.ChangeType(value, typeCode).ToString();
            }
        }

        result = result.Replace("\r", "").Replace("\n", "").Replace(CsvSeparator, " ");
        return result;
    }

    // string데이터를 담고자하는 객체의 타입에 맞는 형태의 실제 object로 변환
    private static object StringToObject(string data, Type objType)
    {
        object result = null;
        // Field 타입의 base클래스가 UnityEngine.Object일 경우 guid정보에 맞는 실제 객체를 찾는다
        if (MyUtils.IsSubType(objType, typeof(UnityEngine.Object)))
        {
#if UNITY_EDITOR
            result = MyUtils.GetAssetFromGUID(data, objType);
#endif
        }
        else
        {
            // Field 타입이 class이거나 struct 타입이면 hexString -> byte[] -> realTypeobject
            TypeCode typeCode = Type.GetTypeCode(objType);
            if (typeCode == TypeCode.Object)
            {
                // 직렬화할때 임시로 바꿔뒀던 #을 다시 콤마로 변환 후 Json으로 역변환한다.
                data = data.Replace("#", CsvSeparator);
                result = JsonUtility.FromJson(data, objType);
            }
            else
            {
                // Field 타입이 기본타입이면 해당 타입에 맞는 string으로 변환
                result = Convert.ChangeType(data, typeCode);
            }
        }

        return result;
    }
}
