using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 게임상의 모든 데이터를 저장 로드 및 관리하는 클래스

/// 사용법 
/*******************************
[System.Serializable]
public class UserInfo : SaveableBase
{
    public const int LastestVersion = 3;
    public override bool IsOldVersion() { return CurrentVersion < LastestVersion; }

    public int CurrentVersion = LastestVersion;
    public int userID = 123;
    public string userName = "lee";
    public float key = 177.7f;
}
UserInfo filedata = GameFileManager<UserInfo>.Load();
GameFileManager<UserInfo>.Save(filedata);
********************************/

/// </summary>

public class GameFileManager<Filetype> where Filetype : SaveableBase, new()
{
    //json 파일의 저장 경로를 가져온다.
    //Mac 환경에서는 기본적으로 ~/Library/Application Support/Eggtart Inc/[Projectname] 하위에 존재함.
    //Windows 환경에서는 기본적으로 C:\Users\[PCName]\AppData\LocalLow\Eggtart Inc\[Projectname] 하위에 존재함.
    private static string GetPath()
    {
        return Application.persistentDataPath;
    }
    private static string GetFileName()
    {
        string filename = typeof(Filetype).Name;
//#if UNITY_EDITOR
        return filename + ".json";
//#else
//        return filename.GetHashCode().ToString("X4") + ".dat";
//#endif
    }
    public static void Save(Filetype mData)
    {
        string fullname = GetPath() + "/" + GetFileName();
        string jsonText = JsonUtility.ToJson(mData, true);
//#if UNITY_EDITOR
        MyUtils.SaveToFile(jsonText, fullname, false);
//#else
//        MyUtils.SaveToFile(jsonText, fullname, true);
//#endif
    }
    public static Filetype Load()
    {
        string fullname = GetPath() + "/" + GetFileName();
        if (!File.Exists(fullname))
        {
            //파일이 없을경우 최초 실행으로 간주하여 파일 생성
            Filetype initObj = new Filetype();
            Save(initObj);
            return initObj;
        }
        
        // 파일 암호화 처리(출시버전일 경우 암호화 처리함)
//#if UNITY_EDITOR
        bool ret = MyUtils.LoadFromFile(fullname, false, out string data);
//#else
//        bool ret = MyUtils.LoadFromFile(fullname, true, out string data);
//#endif

        // 파일 호환성 검사(이전 버전과 다르면 버전별로 처리해야하는 작업 수행)
        if (ret)
        {
            // 유저 데이터 구조가 이전 버전과의 호환성을 맞추기 위한 함수
            Filetype fileObj = JsonUtility.FromJson<Filetype>(data);
            if (fileObj.IsOldVersion())
            {
                fileObj.DoCompatibility();
            }
            return fileObj;
        }
        return null;
    }

    // 유저 데이터 파일 리셋
    public static void ResetUserDataFile()
    {
        Save(new Filetype());
    }

}

public abstract class SaveableBase
{
    public virtual bool IsOldVersion() { return false; }
    public virtual void DoCompatibility() {}
}