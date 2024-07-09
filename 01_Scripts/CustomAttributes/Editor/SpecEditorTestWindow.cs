using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorGUITable;
using UnityEditor.Rendering;
using System.Reflection;

public class SpecEditorTestWindow : EditorWindow 
{
    // SerializedObject mSO = null;
    // SerializedProperty mSP = null;

    // Vector2 _scrollPos = Vector2.zero;
    // GUITableState _tableState;

    // List<SelectorColumn> mColumns = new List<SelectorColumn>();

    // public List<SpecRecord> mRecords = new List<SpecRecord>();

    // [MenuItem("MetalSuits/SpecSuits")]
    // static void SpecSuits()
    // {
    //     ShowWindow("Suits");
    // }
    // [MenuItem("MetalSuits/SpecEnemies")]
    // static void SpecEnemies()
    // {
    //     ShowWindow("MapObjects/Enemies");
    // }
    // [MenuItem("MetalSuits/SpecProps")]
    // static void SpecProps()
    // {
    //     ShowWindow("MapObjects/Props/Interact");
    // }

    // private static void ShowWindow(string resourcePath)
    // {
    //     var window = GetWindow<SpecEditorTestWindow>();
    //     window.titleContent = new GUIContent("SpecEditorWindow");
    //     window.Init(resourcePath);
    //     window.Show();
    // }

    // void Init(string resourcePath)
    // {
    //     LoadSpecRecords(resourcePath);

    //     mSO = new SerializedObject(this);
    //     mSP = mSO.FindProperty("mRecords");


    //     mColumns.Add(new SelectObjectReferenceColumn("Enemy Prefab", TableColumn.Width(50f), TableColumn.EnabledCells(false), TableColumn.Optional(true)));

    //     mColumns.Add(new SelectFromFunctionColumn(
    //         sp =>
    //         {
    //             SpecRecord record = (SpecRecord)sp.objectReferenceValue;
    //             return new LabelCell(record.Path);
    //         },
    //         "PathTitle",
    //         TableColumn.Width(150f),
    //         TableColumn.Optional(true)));

    //     FieldInfo[] fields = typeof(SpecRecord).GetFields();
    //     foreach (FieldInfo field in fields)
    //     {
    //         if (field.IsPublic)
    //         {
    //             float width = Mathf.Clamp(field.Name.Length * 10, 50, 100);
    //             mColumns.Add(new SelectFromPropertyNameColumn(field.Name, field.Name, TableColumn.Width(width), TableColumn.Optional(true)));
    //         }
    //     }

    //     mColumns.Add(new SelectFromFunctionColumn(
    //         sp =>
    //         {
    //             SpecRecord record = (SpecRecord)sp.objectReferenceValue;
    //             if (record.IsTypeSuit())
    //                 return new ActionCell("Suit", (a) => SpecEditorSubWindow.ShowWindow(sp.objectReferenceValue, "mSuit"));
    //             else if (record.IsTypeEnemy())
    //                 return new ActionCell("Enemy", (a) => SpecEditorSubWindow.ShowWindow(sp.objectReferenceValue, "mEnemy"));
    //             else if (record.IsTypeProp())
    //                 return new ActionCell("Prop", (a) => SpecEditorSubWindow.ShowWindow(sp.objectReferenceValue, "mProp"));
    //             else if (record.IsTypeProjectile())
    //                 return new ActionCell("Projectile", (a) => SpecEditorSubWindow.ShowWindow(sp.objectReferenceValue, "mProjectile"));
    //             else
    //                 return new ActionCell("None", (a) => { });
    //         },
    //         "SubData",
    //         TableColumn.Width(100f),
    //         TableColumn.Optional(true)));

    //     mColumns.Add(new SelectFromFunctionColumn(
    //         sp => { return new ActionCell("Custom", (a) => SpecEditorSubWindow.ShowWindow(sp.objectReferenceValue, "custom")); },
    //         "Custom",
    //         TableColumn.Width(100f),
    //         TableColumn.Optional(true)));
    // }


    // private void OnGUI() 
    // {
    //     EditorGUILayout.BeginVertical();
    //     _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
    //     EditorGUILayout.LabelField("SpecEditorTestWindow", GUILayout.Width(200), GUILayout.Height(30));

    //     if(GUILayout.Button("ExportCSV", GUILayout.Width(200), GUILayout.Height(30)))
    //     {
    //         ExportSpecsTablesToCSV();
    //     }
    //     if (GUILayout.Button("ImportCSV", GUILayout.Width(200), GUILayout.Height(30)))
    //     {
    //         ImportSpecsTablesToCSV();
    //     }

    //     DrawObjectsTable();

    //     EditorGUILayout.EndScrollView();
    //     EditorGUILayout.EndVertical();
    // }

    // void DrawObjectsTable()
    // {
    //     _tableState = GUITableLayout.DrawTable(_tableState, mSP, mColumns);

    // }
    // void LoadSpecRecords(string resourcePath)
    // {
    //     mRecords.Clear();

    //     GameObject[] objs = Resources.LoadAll<GameObject>(resourcePath);
    //     foreach (GameObject obj in objs)
    //     {
    //         List<SpecRecord> subSpecs = new List<SpecRecord>();
    //         obj.transform.FindChildAll(subSpecs);

    //         foreach (SpecRecord record in subSpecs)
    //         {
    //             string path = record.gameObject.name;
    //             Transform curTr = record.transform;
    //             while (curTr != obj.transform)
    //             {
    //                 curTr = curTr.parent;
    //                 if (curTr == null)
    //                     break;

    //                 path = curTr.name + "/" + path;
    //             }

    //             record.Path = path;
    //         }

    //         mRecords.AddRange(subSpecs);
    //     }
    // }



    // void ExportSpecsTablesToCSV()
    // {
    //     if(mRecords.Count <= 0 )
    //         return;

    //     string fullname = "./Assets/00_MetaSuit/Resources/Database/SpecTable.csv";

    //     List<string> lines = new List<string>();

    //     // 첫 행은 컬럼 이름정보 기입
    //     lines.Add(SpecRecord.ToCSVColumnName());
        
    //     foreach(SpecRecord record in mRecords)
    //     {
    //         lines.Add(record.ToCSVString());
    //     }

    //     System.IO.File.WriteAllLines(fullname, lines.ToArray());

    //     EditorUtility.DisplayDialog("ExportToCSV", "Complete!! : " + fullname, "Close");
    // }
    
    // void ImportSpecsTablesToCSV()
    // {
    //     if (mRecords.Count <= 0)
    //         return;

    //     string fullname = "./Assets/00_MetaSuit/Resources/Database/SpecTable.csv";
    //     if(!System.IO.File.Exists(fullname))
    //     {
    //         EditorUtility.DisplayDialog("ImportFromCSV", "No File : " + fullname, "Close");
    //         return;
    //     }

    //     string[] lines = System.IO.File.ReadAllLines(fullname);
    //     if(lines == null || lines.Length <= 0)
    //     {
    //         EditorUtility.DisplayDialog("ImportFromCSV", "File Error : " + fullname, "Close");
    //         return;
    //     }

    //     foreach(string line in lines)
    //     {
    //         // 첫번째 행은 컬럼 이름데이터 이므로 패스
    //         if (line.Contains("NameID"))
    //             continue;

    //         string[] fields = line.Split(",");
    //         string nameID = fields[0];
    //         SpecRecord record = mRecords.Find((record) => { return record.Path.Equals(nameID); });
    //         if(record == null)
    //             continue;

    //         record.FromCSVString(fields);
    //     }

    //     EditorUtility.DisplayDialog("ImportFromCSV", "Complete!! : " + fullname, "Close");
    //     SaveSpecAssetData();
    // }

    // void SaveSpecAssetData()
    // {
    //     for (int i = 0; i < mRecords.Count; i++)
    //     {
    //         EditorUtility.SetDirty(mRecords[i].gameObject);
    //     }

    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();
    // }


    // void OnDestroy()
    // {
    //     SaveSpecAssetData();
    // }
}
