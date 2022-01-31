using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

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
            string result =
            (null != obj.GetType().GetField(pairs[i].Name).GetValue(obj)) ?
            Convert.ChangeType(obj.GetType().GetField(pairs[i].Name).GetValue(obj), Type.GetTypeCode(pairs[i].FieldType)).ToString() :
            string.Empty;
            result = result.Replace("\r", "").Replace("\n", "").Replace(CsvSeparator, " ");
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
        string[] arrayLinesCsv = csvString.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        string[] columnsName = arrayLinesCsv[0].Split(_csvSeparator);
        Type tp = typeof(TEntity);
        FieldInfo[] fields = tp.GetFields();
        for (int i = 1; i < arrayLinesCsv.Length; i++)
        {
            object instance = Activator.CreateInstance(tp);
            string[] columnsValue = arrayLinesCsv[i].Split(_csvSeparator);
            for (int j = 0; j < columnsValue.Length; j++)
            {
                if (columnsName[j].Length <= 0) continue;
                for (int x = 0; x < fields.Length; x++)
                {
                    if (columnsName[j] == fields[x].Name)
                    {
                        if (columnsValue[j] != null && columnsValue[j].Length > 0)
                        {
                            fields[x].SetValue(instance, Convert.ChangeType(columnsValue[j], Type.GetTypeCode(fields[x].FieldType)));
                            break;
                        }
                    }
                }
            }
            yield return (TEntity)instance;
        }
    }
}
