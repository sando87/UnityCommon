using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

/// <summary>
/// csv파일의 테이블에 있는 하나의 record와 매핑되어 데이터를 로딩해 놓는 기능
/// Editor상의 기능으로 추가 삭제 편집 기능 추가
/// </summary>

public class RecordSuit : MonoBehaviour
{
    [Dropdown("IDList")]
    [OnValueChanged("OnIDChanged")]
    [SerializeField]
    string _ID = "";

    [SerializeField]
    SuitRawInfo _Info = null;

    void Start()
    {
        Init();
    }

    void Init()
    {
        _Info = DatabaseCSV<SuitRawInfo>.Instance.GetInfo(_ID);
    }

    [Button("Apply (Update to file)")]
    void ApplyRecord()
    {
        if(DatabaseCSV<SuitRawInfo>.Instance.HasInfo(_Info.ID))
        {
            DatabaseCSV<SuitRawInfo>.Instance.EditAndSave(_Info);
        }
    }
    
    [Button("Revert (Load from file)")]
    void RevertRecord()
    {
        Init();
    }
    
    [Button("Add New Record")]
    void AddNewRecord()
    {
        EditorPopupNewID.ShowWindow((string newID) => 
        {
            if(DatabaseCSV<SuitRawInfo>.Instance.HasInfo(newID))
            {
                EditorPopupMessageBox.ShowWindow("This ID already has.");
            }
            else
            {
                _Info = new SuitRawInfo();
                _Info.id = newID;
                DatabaseCSV<SuitRawInfo>.Instance.AddNewAndSave(_Info);
                _ID = newID;
            }
        });
    }
    
    List<string> IDList 
    { 
        get 
        { 
            DatabaseCSV<SuitRawInfo>.Instance.Init(Consts.SuitTableFilePath);
            List<string> list = DatabaseCSV<SuitRawInfo>.Instance.GetAllIDs();
            return list;
        }
    }

    void OnIDChanged()
    {
        Init();
    }

}
