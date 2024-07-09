using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 유저의 슬롯 정보
/// </summary>

[System.Serializable]
public class UserSlotInfo
{
    // 슬롯 생성할 당시 시간
    public long CreateTimeTick = 0;

    // 마지막으로 플레이했던 챕터 이름
    public string LastPlayedChapterName = "";

    public List<long> AcquiredCollections = new List<long>();
    public List<string> UnlockedNoteDatas = new List<string>();

    public Dictionary<string, StageSaveInfo> StageInfos = new Dictionary<string, StageSaveInfo>();

    [JsonIgnore]
    public bool IsEmpty { get { return CreateTimeTick == 0; } }

    [JsonIgnore]
    private string[]  unlockNoteDatasOnInitialized = {
        "History001",
        "History002",
        "History003",
        "History004",
        "History005",
        "History006",
        "History007",
        "History008",
        "History009",
        "History010",
        "History011",
        "History012",
        "History013",
        "History014",
        "History015",
        "History016",
        "Planet015"
    };

    // Release : 정식출시때 데모버전 사용자 데이터 삭제를 위해 처리 필요
    // [JsonIgnore]
    // public bool IsEmpty { get { return CreateTimeTick == 0 || IsOldVersion(); } }

    public void InitSlotData()
    {
        this.CreateTimeTick = System.DateTime.Now.Ticks;
        LastPlayedChapterName = "";
        this.StageInfos = new Dictionary<string, StageSaveInfo>();
        this.AcquiredCollections = new List<long>();
        this.UnlockedNoteDatas = unlockNoteDatasOnInitialized.ToList();
    }
    public void SetEmpty()
    {
        this.CreateTimeTick = 0;
        LastPlayedChapterName = "";
        this.AcquiredCollections = new List<long>();
        this.UnlockedNoteDatas = new List<string>();
    }

    public bool IsClearStage(string stageName)
    {
        if(StageInfos.ContainsKey(stageName))
            return StageInfos[stageName].FastestPlayTime > 0;
        
        return false;
    }
    public bool IsDoneTutorial()
    {
        return IsClearStage("");
    }
    public int GetClearedStageCountInChapter(string chapterName)
    {
        ChapterInfo info = DataManager.GetChapterInfo(chapterName);
        if (info == null)
            return 0;

        string[] stages = info.GetStageMaps();
        int clearedCount = 0;
        foreach (string stageName in stages)
        {
            if (IsClearStage(stageName))
                clearedCount++;
            else
                break;
        }

        return clearedCount;
    }
    public float GetChpaterProgressRate(string chapterName)
    {
        ChapterInfo info = DataManager.GetChapterInfo(chapterName);
        if(info == null)
            return 0;

        string[] stages = info.GetStageMaps();
        if (stages == null || stages.Length <= 0)
            return 0;

        int clearedCount = GetClearedStageCountInChapter(chapterName);

        if (clearedCount == 0)
            return 0;
        else if(clearedCount == stages.Length)
            return 1;
        else
            return (float)clearedCount / stages.Length;
    }

    public string GetChapterInProgress()
    {
        ChapterInfo[] infos = DataManager.GetAllChapterInfos();
        foreach(ChapterInfo info in infos)
        {
            if(info.SubNum > 0)
                continue;
                
            float rate = GetChpaterProgressRate(info.NameID);
            if(0 <= rate && rate < 1)
                return info.NameID;
        }
        return "";
    }

    // 슬롯의 전체 진행도 정보 가져오기
    public float GetTotalProgressRate()
    {
        if(IsEmpty)
            return 0;

        ChapterInfo[] chapters = DataManager.GetAllChapterInfos();
        int totalStageCount = GetTotalStageCount();

        // 전체 챕터에 있는 모든 스테이지에 대해 완료된 스테이지의 비율 구하기
        int clearedStageCount = 0;
        foreach (var item in StageInfos)
        {
            StageSaveInfo stageInfo = item.Value;
            if(stageInfo.FastestPlayTime > 0)
                clearedStageCount++;
        }

        float rate = clearedStageCount == totalStageCount ? 1 : (float)clearedStageCount / totalStageCount;
        return rate;
    }

    int GetTotalStageCount()
    {
        ChapterInfo[] chapters = DataManager.GetAllChapterInfos();
        int totalStageCount = 0;
        foreach(ChapterInfo chapter in chapters)
            totalStageCount += chapter.GetStageMaps().Length;

        return totalStageCount;
    }


    public void ClearStage(string stageName, float clearTime)
    {
        if(StageInfos.ContainsKey(stageName))
        {
            StageSaveInfo info = StageInfos[stageName];
            info.FastestPlayTime = info.FastestPlayTime <= 0 ? clearTime : Mathf.Min(clearTime, info.FastestPlayTime);
            StageInfos[stageName] = info;
        }
        else
        {
            StageSaveInfo info = new StageSaveInfo();
            info.FastestPlayTime = clearTime;
            StageInfos[stageName] = info;
        }
    }

    public bool IsGainTrophy(string stageName, long trophyID)
    {
        if (StageInfos.ContainsKey(stageName))
        {
            StageSaveInfo info = StageInfos[stageName];
            return info.TrophyIDs.Contains(trophyID);
        }

        return false;
    }

    public void SaveTrophyID(string stageName, long trophyID)
    {
        if (StageInfos.ContainsKey(stageName))
        {
            StageSaveInfo info = StageInfos[stageName];
            info.TrophyIDs.Add(trophyID);
        }
        else
        {
            StageSaveInfo info = new StageSaveInfo();
            info.TrophyIDs.Add(trophyID);
            StageInfos[stageName] = info;
        }
    }
    public int GetTrophyCount(string stageName)
    {
        if (StageInfos.ContainsKey(stageName))
        {
            StageSaveInfo info = StageInfos[stageName];
            return info.TrophyIDs.Count;
        }
        return 0;
    }
    public int GetTrophyTotalCount()
    {
        int count = 0;
        foreach(var stageInfo in StageInfos)
        {
            StageSaveInfo info = stageInfo.Value;
            count += info.TrophyIDs.Count;
        }
        return count;
    }

    public bool IsGainCollection(long collectionID)
    {
        return AcquiredCollections.Contains(collectionID);
    }

    public void SaveCollection(long collectionID)
    {
        AcquiredCollections.Add(collectionID);
    }
    public int GetCollectionCount()
    {
        return AcquiredCollections.Count;
    }
    public void SetAllChapterCleared()
    {
        ChapterInfo[] chapters = DataManager.GetAllChapterInfos();
        foreach(ChapterInfo chapter in chapters)
        {
            string[] stages = chapter.GetStageMaps();
            foreach (string stage in stages)
                ClearStage(stage, 60 * 5); // 플레이타임 5분으로 스테이지 클리어
        }
        LastPlayedChapterName = chapters.Last().NameID;
    }
}


[System.Serializable]
public class StageSaveInfo
{
    public float FastestPlayTime = 0;
    public List<long> TrophyIDs = new List<long>();
    //public List<long> CollectionIDs = new List<long>();
}