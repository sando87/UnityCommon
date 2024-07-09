using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseUnityChapters : ScriptableObjectDictionary<DatabaseUnityChapters, ChapterInfo>
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/MetalSuits/CreateTable/Chapters Table")]
    public static void CreateNewAsset()
    {
        CreateNewScriptableAssetFile();
    }
#endif
}

[System.Serializable]
public class ChapterInfo : IUnityFormat
{
    public string NameID = "NameID"; // 이름
    public string DisplayName = "DisplayName"; // UI 표기용 이름
    public int Num = 0;   // 행성 순서
    public int SubNum = 0; // 하위 행성 번호
    public string RequiredChpater = ""; // 행성 해제를 위해 완료되어야 하는 행성 이름
    public int RequiredTrophyCount = 0; // 행성 해제를 위해 필요한 트로피 개수
    public string Description = "Description"; // UI 하단에 표기될 챕터 상세 설명
    
    public string DescCleared = "Cleared"; // 클리어 한 후 표시할 챕터 설명

    public string BGMFilename = "chapter_1_bgm"; // 챕터별 배경음악
    public string StageMapFiles = ""; // 실제 로딩할 인게임 파일들 이름(파일이름간 ","구분자로 분리)

    public string NoteIdentifiers = ""; // 행성 진입시 추가할 도감 데이터들(도감 Identifier들간 ","구분자로 분리);
    
    public int DialogueGIDOnCleared = -1;

    public Sprite  PlanetSprite = null; 

    public int RowIndex { get; set; }
    public long ID { get { return (long)NameID.GetHashCode(); } }

    public static long NameToID(string chapterName) { return (long)chapterName.GetHashCode(); }
    public string[] GetStageMaps()
    {
        return StageMapFiles.Split(",");
    }
     public string[] GetNoteIds()
    {
        return NoteIdentifiers.Split(",");
    }
}
