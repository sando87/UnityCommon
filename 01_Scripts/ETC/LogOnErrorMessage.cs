using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;

// 유니티 에러 메시지발생하면 로그를 보내는 기능
// 최초 1회 발생시에만 보낸다(이유는 이후 발생되는 로그들은 많은양이 빠르게 발생될 수 있기 때문에 서버에 부하가 걸리므로)

public class LogOnErrorMessage : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += OnLog;
    }

    void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Log && type != LogType.Warning)
        {
            // if (GameSettings.Instance.IsBugReportOn)
            // {
            //     // 최초 1회만 에러 로그를 보낸다
            //     Application.logMessageReceived -= OnLog;

            //     BaseObject playerObj = InGameManager.Instance.GetPlayerAnyone();
            //     string message = "condition:" + condition + ", ";
            //     message += "stackTrace:" + stackTrace;

            //     InGameUtils.Log_UA(UnityAnalyticsManager.EventNameType.ErrorMsg, playerObj, null, message);
            // }
        }
    }

}