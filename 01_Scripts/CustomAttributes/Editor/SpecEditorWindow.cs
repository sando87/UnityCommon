using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpecEditorWindow : EditorWindow 
{
    // string mTableName = "";
    // SerializedObject mSO = null;
    // SerializedProperty mDataList = null;
    // private Vector2 _scrollPos = Vector2.zero;
    // private float mTotalWidth = 0;

    // [MenuItem("MetalSuits/OpenEditorSpecsSuit")]
    // static void OpenEditorSpecsSuit()
    // {
    //     ScriptableObject soAsset = (ScriptableObject)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/Resources/Database/DatabaseUnitySuits.asset", typeof(ScriptableObject));
    //     Selection.activeObject = soAsset;
    //     ShowWindow();
    // }
    // [MenuItem("MetalSuits/OpenEditorSpecsEnemy")]
    // static void OpenEditorSpecsEnemy()
    // {
    //     ScriptableObject soAsset = (ScriptableObject)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/Resources/Database/DatabaseUnityEnemies.asset", typeof(ScriptableObject));
    //     Selection.activeObject = soAsset;
    //     ShowWindow();
    // }
    // [MenuItem("MetalSuits/OpenEditorSpecsProps")]
    // static void OpenEditorSpecsProps()
    // {
    //     ScriptableObject soAsset = (ScriptableObject)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/Resources/Database/DatabaseUnityProps.asset", typeof(ScriptableObject));
    //     Selection.activeObject = soAsset;
    //     ShowWindow();
    // }

    // [MenuItem("MetalSuits/ExportSpecsTablesToCSV")]
    // static void ExportSpecsTablesToCSV()
    // {
    //     string filename = "";
    //     string fullname = "";
    //     string csvstring = "";

    //     EnemySpecInfo[] enemies = DataManager.GetAllEnemySpecInfos();
    //     filename = "DatabaseUnityEnemies";
    //     fullname = "./Assets/00_MetaSuit/Resources/Database/" + filename + ".csv";
    //     csvstring = CSVParser<EnemySpecInfo>.Serialize(',', enemies);
    //     System.IO.File.WriteAllText(fullname, csvstring);
    //     LOG.trace("Export Enemies");

    //     PropsSpecInfo[] props = DataManager.GetAllPropsSpecInfos();
    //     filename = "DatabaseUnityProps";
    //     fullname = "./Assets/00_MetaSuit/Resources/Database/" + filename + ".csv";
    //     csvstring = CSVParser<PropsSpecInfo>.Serialize(',', props);
    //     System.IO.File.WriteAllText(fullname, csvstring);
    //     LOG.trace("Export props");

    //     SuitSpecInfo[] suits = DataManager.GetAllSuitSpecInfos();
    //     filename = "DatabaseUnitySuits";
    //     fullname = "./Assets/00_MetaSuit/Resources/Database/" + filename + ".csv";
    //     csvstring = CSVParser<SuitSpecInfo>.Serialize(',', suits);
    //     System.IO.File.WriteAllText(fullname, csvstring);
    //     LOG.trace("Export suits");
    // }
    // [MenuItem("MetalSuits/IxportSpecsTablesToCSV")]
    // static void IxportSpecsTablesToCSV()
    // {
    //     string filename = "";
    //     TextAsset ta = null;

    //     filename = "DatabaseUnityEnemies";
    //     ta = Resources.Load<TextAsset>("Database/" + filename);
    //     if(ta != null)
    //     {
    //         EnemySpecInfo[] enemies = CSVParser<EnemySpecInfo>.Deserialize(',', ta.text);
    //         DatabaseUnityEnemies.Instance.Save(enemies);
    //         LOG.trace("Import Enemies");
    //     }

    //     filename = "DatabaseUnityProps";
    //     ta = Resources.Load<TextAsset>("Database/" + filename);
    //     if (ta != null)
    //     {
    //         PropsSpecInfo[] props = CSVParser<PropsSpecInfo>.Deserialize(',', ta.text);
    //         DatabaseUnityProps.Instance.Save(props);
    //         LOG.trace("Import props");
    //     }

    //     filename = "DatabaseUnitySuits";
    //     ta = Resources.Load<TextAsset>("Database/" + filename);
    //     if (ta != null)
    //     {
    //         SuitSpecInfo[] suits = CSVParser<SuitSpecInfo>.Deserialize(',', ta.text);
    //         DatabaseUnitySuits.Instance.Save(suits);
    //         LOG.trace("Import suits");
    //     }
    // }


    // [MenuItem("Assets/MetalSuits/OpenSpecEditWindow")]
    // private static void ShowWindow()
    // {
    //     var window = GetWindow<SpecEditorWindow>();
    //     window.titleContent = new GUIContent("SpecWindow");
    //     window.Show();
    // }
    // [MenuItem("Assets/MetalSuits/OpenSpecEditWindow", true)]
    // private static bool IsValidate()
    // {
    //     return (Selection.activeObject as ScriptableObject) != null;
    // }

    // private void OnGUI() 
    // {
    //     ScriptableObject selectedSO = Selection.activeObject as ScriptableObject;
    //     if(selectedSO == null)
    //         return;

    //     string tableName = selectedSO.GetType().Name;
    //     if(mTableName.Length == 0 || !mTableName.Equals(tableName) || mSO == null || mDataList == null)
    //     {
    //         mTableName = tableName;
    //         mSO = new SerializedObject(selectedSO);
    //         mDataList = mSO.FindProperty("DataList");

    //         ISpecEditorWindow sw = (ISpecEditorWindow) selectedSO;
    //         string[] names = sw.GetFieldNames();
    //         mTotalWidth = 150;
    //         foreach(string name in names)
    //         {
    //             GUI.skin.button.CalcMinMaxWidth(new GUIContent(name), out float minWidth, out float maxWidth);
    //             mTotalWidth += minWidth;
    //         }
    //     }

    //     EditorGUILayout.BeginVertical();
    //     _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
    //     EditorGUILayout.LabelField(mTableName, GUILayout.Width(mTotalWidth), GUILayout.Height(30));
        
    //     // if(GUILayout.Button("LoadFromFile"))
    //     // {
    //     //     mSO.Update();
    //     // }
    //     // if(GUILayout.Button("SaveToFile"))
    //     // {
    //     //     mSO.ApplyModifiedProperties();
    //     //     AssetDatabase.SaveAssets();
    //     //     AssetDatabase.Refresh();
    //     // }
    //     // if(GUILayout.Button("ApplyToInGame"))
    //     // {
    //     //     if(Application.isPlaying)
    //     //     {
    //     //         //현재 값들을 ingame상의 모든 specBase 객체에 적용
    //     //     }
    //     // }

    //     // 테이블 전시
    //     for (int i = 0; i < mDataList.arraySize; i++)
    //     {
    //         EditorGUILayout.PropertyField(mDataList.GetArrayElementAtIndex(i));
    //     }
        
        
    //     EditorGUILayout.EndScrollView();
    //     EditorGUILayout.EndVertical();
    // }

    // // 윈도우 창을 닫을때 자동 세이브 된다
    // private void OnDestroy() 
    // {
    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();
    // }
}
