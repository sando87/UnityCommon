using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : Singleton<LanguageController>
{
    // 기본 언어 관련 변수들
    private string defaultLanguageCode = "English";
    private string m_strLanguageCode = "English";

    // 랭귀지 번역 데이터
    public List<Dictionary<string, object>> languageDataFromCSVFile = new List<Dictionary<string, object>>();

    // 랭귀지 리스트 데이터 
    public List<Dictionary<string, object>> languageListFromCSVFile = new List<Dictionary<string, object>>();

    // 랭귀지 파일 로딩 처리
    private bool b_bLoadFromFile = false;

    //CheckDevice() 한수 실행 관련
    private static bool b_didCheckDevice = false;

    public event System.Action<LanguageType, LanguageType> EventLanguageChanged;


    private void _LoadFromFile()
    {
        // 이미 로딩했으면 리턴,
        if (b_bLoadFromFile == true) 
        {
            return;
        }

        // 언어 설정 로드
        _LoadListFromFile();

        // CSV 파일을 읽어와서 리스트에 넣어줌.
        languageDataFromCSVFile = CSVReader.Read("language_table");

        Debug.Log("GMS -언어 파일 길이 = " + languageDataFromCSVFile.Count);

        b_bLoadFromFile = true;
    }

    private void _LoadListFromFile()
    {
        // CSV 파일을 읽어와서 리스트에 넣어줌.
        languageListFromCSVFile = CSVReader.Read("language_list");
    }

    // 언어 리스트 리턴
    private List<Dictionary<string, object>> _GetLanguageList()
    {
        return languageListFromCSVFile;
    }

    // 언어 타입에 대한 각국가별 언어 string값을 반환
    public string GetLanguageString(LanguageType lang)
    {
        foreach (var each in languageListFromCSVFile)
        {
            string languageType = each["language"].ToString();
            LanguageType type = languageType.ToEnum<LanguageType>();
            if (lang == type)
            {
                return each["text"].ToString();
            }
        }
        return "";
    }

    // 언어 리스트 리턴
    public static List<Dictionary<string, object>> GetLanguageList()
    {
        return Instance._GetLanguageList();
    }

    // 랭귀지 파일 읽어옴.
    public static void LoadFromFile()
    {
        Instance._LoadFromFile();
    }

    // 디바이스 체크
    public static void CheckDevice()
    {
        // 이미 로딩했으면 리턴,
        if (b_didCheckDevice == true) 
        {
            return;
        }

        // 실행했다고 처리
        b_didCheckDevice = true;

        SetLanguage();
    }   

    // 언어 설정
    public static void SetLanguage()
    {
        Debug.Log("GMS - 설정된 언어 = " + GamePlayData.UserSaveData.Language);

        // 랭귀지 설정이 안되어 있는 경우
        if(GamePlayData.UserSaveData.Language == LanguageType.None)
        {
            // 현재 시스템 언어를 읽어옴
            SystemLanguage sl = Application.systemLanguage;

            string strValue = sl.ToString();

            Debug.Log("GMS - 시스템 언어 strValue = " + strValue);

            // // 테스트용 언어 변경 : https://docs.unity3d.com/ScriptReference/SystemLanguage.html
            // strValue = "a";
            // strValue = "Japanese";
            // strValue = "Korean";
            // strValue = "English";

            // 아래 7개국언어가 아닌 경우 영어를 기본 언어로 설정
            if( !strValue.Equals("English") && 
                !strValue.Equals("Japanese") && 
                !strValue.Equals("Chinese") && 
                !strValue.Equals("Spanish") && 
                !strValue.Equals("Portuguese") && 
                !strValue.Equals("Russian") && 
                !strValue.Equals("Korean"))
            {
                strValue = "English";        
            }
            
            // 중국어인 경우 간체인지 번체인지 확인 후 넣어줌, 혹시 둘다 아니면 영어로 리턴함
            if(strValue.Equals("Chinese"))
            {
                Debug.Log("GMS - 중국어 strValue = " + strValue);
                strValue = Instance.GetLanguageCodeForChinese();
                Debug.Log("GMS - 중국어 strValue = " + strValue);
            }

            // 랭귀시 설정
            SetUserLangueageData(strValue.ToEnum<LanguageType>());
            Debug.Log("GMS - CheckDevice() : 설정된 언어 1 = " + GamePlayData.UserSaveData.Language);
        }
        else
        {
            Debug.Log("GMS - CheckDevice() : 설정된 언어 2 = " + GamePlayData.UserSaveData.Language);
        }
    }

    public static void SetUserLangueageData(LanguageType language)
    {
        LanguageType previousLang = GamePlayData.UserSaveData.Language;
        GamePlayData.UserSaveData.Language = language;

        Instance.EventLanguageChanged?.Invoke(previousLang, language);
    }

    // 중국어인 경우 간체인지 번체인지 확인해서 리턴
    public string GetLanguageCodeForChinese()
    {
        // 데이터 리스트 내에 해당 스트링이 있는지 확인
        switch(Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                Debug.Log("GMS - ChineseSimplified" + Application.systemLanguage);
                return SystemLanguage.ChineseSimplified.ToString();

            case SystemLanguage.ChineseTraditional:
                Debug.Log("GMS - ChineseTraditional" + Application.systemLanguage);
                return SystemLanguage.ChineseTraditional.ToString();
        }

        // 해당 언어 둘다 없으면 영어로 표시
        return defaultLanguageCode;
    }

    // 입력된 스트링의 키 값을 찾아서 리턴 해줌
    public int GetTextCodeByString(string myTextString)
    {
        for(int i = 0 ; i < languageDataFromCSVFile.Count ; i++)
        {
            foreach (string keyVar in languageDataFromCSVFile[i].Keys) 
            { 
                if (languageDataFromCSVFile[i][keyVar].ToString().Equals(myTextString))
                {
                    return int.Parse(languageDataFromCSVFile[i]["code"].ToString());
                }
            }
        }

        return -1;
    }

    // 코드를 기반으로 변환된 언어로 리턴
    public string GetTextByCode(int i_textCode)
    {        
        // 리스트 루프를 돈다
        for(int i = 0 ; i < languageDataFromCSVFile.Count ; i++)
        {
            // 리스트 내 같은 문자열이 있으면 현재 선택된 언어로 리턴 해줌
            if((int.Parse(languageDataFromCSVFile[i]["code"].ToString())) == i_textCode)
            {
                return languageDataFromCSVFile[i][GamePlayData.UserSaveData.Language.ToString()].ToString();
            }
        }

        return "Error";
    }

    // 코드를 기반으로 변환된 언어로 리턴
    public string GetTextByString(string myTextString)
    {
        // 리스트 루프를 돈다
        for(int i = 0 ; i < languageDataFromCSVFile.Count ; i++)
        {
            // 리스트 내 같은 문자열이 있으면 현재 선택된 언어로 리턴 해줌
            if((languageDataFromCSVFile[i]["English"].ToString()).Equals(myTextString) == true)
            {
                // Debug.Log("[" + myTextString + "] : GMS -  matched same string : " + languageDataFromCSVFile[i][UserPlayDataManager.GetSelectedLanguage().ToString()].ToString());
                return languageDataFromCSVFile[i][GamePlayData.UserSaveData.Language.ToString()].ToString();
            }
        }

        return myTextString;
    }

    public string ConvertTextToTextCodeToLanguageTexts(string textWord)
    {
        // 텍스트를 가지고 텍스트 코드를 찾음
        int i_textCode = GetTextCodeByString(textWord);

        if (i_textCode > 0)
        {
            // 코드를 통해서 현재 언어에 맞는 텍스트를 받아옴
            return Instance.GetTextByCode(i_textCode);
        }

        return textWord;
    }
}
