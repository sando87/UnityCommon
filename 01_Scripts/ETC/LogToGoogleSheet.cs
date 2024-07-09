using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Networking;

// 구글 Form과 연동되어 데이터를 보내는 기능

// 구글 필드 수정을 위한 편집링크
// https://docs.google.com/forms/d/1ac4UvLZhDmZjtOgmoJ-ROiQXoFL5RNwqBLUaMt-XiyY/edit

public class LogToGoogleSheet : SingletonMono<LogToGoogleSheet>
{
    private const string GoogleFormURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfYBTPREXeVkeGr_QdPIFumCmgriDoQvcClgLfqaiwm-9C9kg/formResponse";

    // 아래 entry정보들은 위 URL 웹페이지 진입 후 마우스 오른쪽클릭->검사 메뉴로 들어가면 각 필드와 매핑되는 entry정보를 찾을 수 있다
    private const string Field_UTCTime = "entry.819340141";
    private const string Field_DeviceID = "entry.960706979";
    private const string Field_SessionID = "entry.940914066";
    private const string Field_Country = "entry.228461063";
    private const string Field_Platform = "entry.605889500";
    private const string Field_Version = "entry.1148761957";
    private const string Field_EventName = "entry.700129920";
    private const string Field_PosX = "entry.504415019";
    private const string Field_PosY = "entry.985548729";
    private const string Field_InteractObj = "entry.873971946";
    private const string Field_StageName = "entry.509129676";
    private const string Field_SuitType = "entry.97749310";
    private const string Field_Comments = "entry.3597794";

    public void SendLogToGoogleForm(GoogleFormParam param)
    {
        StartCoroutine(CoSendLog(param));
    }

    IEnumerator CoSendLog(GoogleFormParam param)
    {
        WWWForm form = new WWWForm();

        form.AddField(Field_UTCTime, param.utcTime);
        form.AddField(Field_DeviceID, param.deviceID);
        form.AddField(Field_SessionID, param.sessionID);
        form.AddField(Field_Country, param.userCountry);
        form.AddField(Field_Platform, param.platform);
        form.AddField(Field_Version, param.version);

        form.AddField(Field_EventName, param.eventName);
        form.AddField(Field_PosX, param.PositionX);
        form.AddField(Field_PosY, param.PositionY);
        form.AddField(Field_InteractObj, param.interactObject);
        form.AddField(Field_StageName, param.stageName);
        form.AddField(Field_SuitType, param.suitType);
        form.AddField(Field_Comments, param.comments);

        using(UnityWebRequest www = UnityWebRequest.Post(GoogleFormURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.Log(www.error);
        }
    }
}
public class GoogleFormParam
{
    public string utcTime = "";
    public string deviceID = "";
    public string sessionID = "";
    public string userCountry = "";
    public string platform = "";
    public string version = "";

    public string eventName = "";
    public string PositionX = "";
    public string PositionY = "";
    public string interactObject = "";
    public string stageName = "";
    public string suitType = "";
    public string comments = "";
}
