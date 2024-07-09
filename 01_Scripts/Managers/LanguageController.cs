using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 번역 관련 처리를 모두 처리한다.
/// 언어 테이블 파일 로딩 및 번역 수행
/// </summary>


public class LanguageController : Singleton<LanguageController>
{
    // 기본 언어 관련 변수들
    private string defaultLanguageCode = "English";
    // private string m_strLanguageCode = "English";

    // 랭귀지 번역 데이터
    public List<LanguageTableRow> mLanguageTableList = new List<LanguageTableRow>();

    // 랭귀지 리스트 데이터 
    public List<Dictionary<string, object>> languageListFromCSVFile = new List<Dictionary<string, object>>();

    // 랭귀지 파일 로딩 처리
    private bool b_bLoadFromFile = false;

    public event System.Action<LanguageType, LanguageType> EventLanguageChanged;

    public LanguageType CurrentLanguage { get; private set; } = LanguageType.None;


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
        TextAsset ta = Resources.Load("languages_table", typeof(TextAsset)) as TextAsset;
        // mLanguageTableList = JsonConvert.DeserializeObject<List<LanguageTableRow>>(ta.text);

        Debug.Log("AOS -언어 파일 길이 = " + mLanguageTableList.Count);

        b_bLoadFromFile = true;
    }

    private void _LoadListFromFile()
    {
        // CSV 파일을 읽어와서 리스트에 넣어줌.
        // languageListFromCSVFile = CSVReader.Read("language_list");
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


    // 게임 초기 랭귀지 설정이 안되어 있는 경우 시스템 언어정보를 읽어온다.
    public static LanguageType GetSystemOSLanguage()
    {
        // 현재 시스템 언어를 읽어옴
        SystemLanguage sl = Application.systemLanguage;
        Debug.Log("AOS - 시스템 언어 : " + sl.ToString());

        LanguageType langType = LanguageType.English;
        switch (sl)
        {
            case SystemLanguage.English: langType = LanguageType.English; break;
            case SystemLanguage.Japanese: langType = LanguageType.Japanese; break;
            case SystemLanguage.Korean: langType = LanguageType.Korean; break;
            case SystemLanguage.Portuguese: langType = LanguageType.Portuguese; break;
            case SystemLanguage.Russian: langType = LanguageType.Russian; break;
            case SystemLanguage.ChineseSimplified: langType = LanguageType.ChineseSimplified; break;
            case SystemLanguage.Spanish: langType = LanguageType.Spanish; break;
            case SystemLanguage.ChineseTraditional: langType = LanguageType.ChineseTraditional; break;
            case SystemLanguage.German: langType = LanguageType.German; break;
            case SystemLanguage.French: langType = LanguageType.French; break;
            case SystemLanguage.Italian: langType = LanguageType.Italian; break;
            case SystemLanguage.Indonesian: langType = LanguageType.Indonesian; break;
            default: langType = LanguageType.English; break;
        }
        return langType;
    }

    public void SetUserLangueageData(LanguageType language)
    {
        LanguageType previousLang = CurrentLanguage;
        CurrentLanguage = language;
        Debug.Log("설정된 언어 : " + language);

        Instance.EventLanguageChanged?.Invoke(previousLang, language);
    }

    // 중국어인 경우 간체인지 번체인지 확인해서 리턴
    public string GetLanguageCodeForChinese()
    {
        // 데이터 리스트 내에 해당 스트링이 있는지 확인
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                Debug.Log("AOS - ChineseSimplified" + Application.systemLanguage);
                return SystemLanguage.ChineseSimplified.ToString();

            case SystemLanguage.ChineseTraditional:
                Debug.Log("AOS - ChineseTraditional" + Application.systemLanguage);
                return SystemLanguage.ChineseTraditional.ToString();
        }

        // 해당 언어 둘다 없으면 영어로 표시
        return defaultLanguageCode;
    }

    // 입력된 스트링의 키 값을 찾아서 리턴 해줌
    public int GetTextCodeByString(string myTextString)
    {
        for (int i = 0; i < mLanguageTableList.Count; i++)
        {
            foreach (LanguageType langType in System.Enum.GetValues(typeof(LanguageType)))
            {
                if (langType == LanguageType.None)
                {
                    continue;
                }
                string text = GetTextFromType(i, langType);
                if (text != null && text.Equals(myTextString))
                {
                    return mLanguageTableList[i].code;
                }
            }
        }

        return -1;
    }

    // 코드를 기반으로 변환된 언어로 리턴
    public string GetTextByCode(int i_textCode)
    {
        // 리스트 루프를 돈다
        for (int i = 0; i < mLanguageTableList.Count; i++)
        {
            // 리스트 내 같은 문자열이 있으면 현재 선택된 언어로 리턴 해줌
            if (mLanguageTableList[i].code == i_textCode)
            {
                string ret = GetTextFromType(i, CurrentLanguage);
                if (ret != null)
                    return ret;
            }
        }

        return "Error";
    }

    // 코드를 기반으로 변환된 언어로 리턴
    public string GetTextByString(string myTextString)
    {
        // 리스트 루프를 돈다
        for (int i = 0; i < mLanguageTableList.Count; i++)
        {
            // 리스트 내 같은 문자열이 있으면 현재 선택된 언어로 리턴 해줌
            if (mLanguageTableList[i].English.Equals(myTextString))
            {
                // Debug.Log("[" + myTextString + "] : AOS -  matched same string : " + languageDataFromCSVFile[i][UserPlayDataManager.GetSelectedLanguage().ToString()].ToString());
                string ret = GetTextFromType(i, CurrentLanguage);
                if (ret != null)
                    return ret;
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
            string translatedWord = Instance.GetTextByCode(i_textCode);
            if (translatedWord.Trim().Length == 0 || translatedWord.Equals("Error"))
            {
                Debug.LogWarning("!!!! AOSS - 번역데이터 비어있음 : " + textWord);
                return textWord;
            }
            return translatedWord;
        }
        else
        {
            Debug.LogWarning("!!!! AOSS - 번역파일에 없음 : " + textWord);
        }

        return textWord;
    }

    private string GetTextFromType(int idx, LanguageType type)
    {
        switch (type)
        {
            case LanguageType.English: return mLanguageTableList[idx].English;
            case LanguageType.Japanese: return mLanguageTableList[idx].Japanese;
            case LanguageType.Korean: return mLanguageTableList[idx].Korean;
            case LanguageType.Portuguese: return mLanguageTableList[idx].Portuguese;
            case LanguageType.Russian: return mLanguageTableList[idx].Russian;
            case LanguageType.ChineseSimplified: return mLanguageTableList[idx].ChineseSimplified;
            case LanguageType.Spanish: return mLanguageTableList[idx].Spanish;
            case LanguageType.ChineseTraditional: return mLanguageTableList[idx].ChineseTraditional;
            case LanguageType.German: return mLanguageTableList[idx].German;
            case LanguageType.French: return mLanguageTableList[idx].French;
            case LanguageType.Italian: return mLanguageTableList[idx].Italian;
            case LanguageType.Indonesian: return mLanguageTableList[idx].Indonesian;
        }
        return null;
    }
}


public class LanguageTableRow
{
    public int code = 0;
    public string division = "";
    public string note = "";
    public string English = "";
    public string Korean = "";
    public string Japanese = "";
    public string Spanish = "";
    public string German = "";
    public string French = "";
    public string Portuguese = "";
    public string ChineseTraditional = "";
    public string ChineseSimplified = "";
    public string Russian = "";
    public string Italian = "";
    public string Indonesian = "";
}

// 지원하는 언어 종류
public enum LanguageType
{
    None,
    English,
    Japanese,
    Korean,
    Portuguese,
    Russian,
    ChineseSimplified,
    Spanish,
    ChineseTraditional,
    German,
    French,
    Italian,
    Indonesian
}
