using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GamePlayData
{
    public const int DataVersion = 1;
    public const string FILENAME = "userdata.json"; //저장 파일 이름
    public const string FILENAME_ENC = "ud.dat"; //암호화된 파일 이름

    private static UserSaveData mUserSaveData = new UserSaveData();

    public static UserSaveData UserSaveData { get { return mUserSaveData; } }

    //json 파일의 저장 경로를 가져온다.
    //Mac 환경에서는 기본적으로 ~/Library/Application Support/Eggtart Inc/[Projectname] 하위에 존재함.
    private static string GetPath()
    {
#if UNITY_EDITOR
        return Application.persistentDataPath + "/" + FILENAME;
#elif UNITY_ANDROID || UNITY_IPHONE
        return Application.persistentDataPath + "/" + FILENAME_ENC;
#endif
    }
    public static void Save()
    {
        string path = GetPath();
        string jsonText = JsonUtility.ToJson(mUserSaveData, true);
#if UNITY_EDITOR
        MyUtils.SaveToFile(jsonText, path, false);
#elif UNITY_ANDROID || UNITY_IPHONE
        MyUtils.SaveToFile(jsonText, path, true);
#endif
    }
    public static void Load()
    {
        string path = GetPath();
        if (!File.Exists(path))
        {
            //파일이 없을경우 최초 실행으로 간주하여 파일 생성
            Save();
            return;
        }
#if UNITY_EDITOR
        bool ret = MyUtils.LoadFromFile(path, false, out string data);
#elif UNITY_ANDROID || UNITY_IPHONE
        bool ret = MyUtils.LoadFromFile(path, true, out string data);
#endif
        if (ret)
        {
            mUserSaveData = JsonUtility.FromJson<UserSaveData>(data);
            if (mUserSaveData.DataVersion != GamePlayData.DataVersion)
            {
                KeepCompatibility();
            }
        }
    }
    private static void KeepCompatibility()
    {
        if (mUserSaveData.DataVersion == GamePlayData.DataVersion)
            return;

        // 이전버전값에 따라 현재버전에서 잘 작동하도록 데이터 변환 처리를 수행한다.
        int previousVersion = mUserSaveData.DataVersion;
        switch (previousVersion)
        {
            case 0: break;
            case 1: break;
            case 2: break;
            default: break;
        }
    }

#if UNITY_EDITOR
    // 유저 데이터 파일 리셋
    public static void ResetUserDataFile()
    {
        mUserSaveData = new UserSaveData();
        Save();
    }
#endif

}
