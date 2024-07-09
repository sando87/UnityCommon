using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// 게임 전반의 Sound 재생처리를 전담한다.
/// 배경음, 효과음 재생처리
/// AudioSource는 자체 Pooling 사용
/// 재생 음원파일 로딩도 자체 캐싱 사용
/// 주요 함수는 2가지: PlaySFX(string clipname), PlayBGM(string clipname)
/// Resources/Sound/SFX 폴더 하위 오디오 파일 이름을 인자로 넣어줘야 한다.
/// Resources/Sound/BGM 폴더 하위 오디오 파일 이름을 인자로 넣어줘야 한다.
/// </summary>

public class SoundPlayManager : SingletonMono<SoundPlayManager>
{
    private const int PoolCount = 30;

    //배경 재생을 위한 전용 AudioSource
    private AudioSource mAudioSourceForBGM = null;

    //SFX 재생을 위한 AudioSource Pool Root
    private Transform mAudioSourcePool = null;

    //SFX 재생중인 AudioSource Root
    private Transform mAudioSourceUsing = null;

    // 오디오클립 캐시
    private Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();

    // 현재 재생되는 클립들을 넣어두는 컨테이너
    private Dictionary<string, PlayingClipInfo> mPlaylist = new Dictionary<string, PlayingClipInfo>();

    // SFX 음소거 제어(주로 세팅에서 이 변수를 제어한다)
    public bool IsMuteSFX { get; set; } = false;
    
    // SFX 볼륨 제어(주로 세팅에서 이 변수를 제어한다)
    public float VolumeSFX { get; set; } = 0.4f;
    
    // Background 음소거 제어(주로 세팅에서 이 변수를 제어한다)
    public bool IsMuteBGM
    {
        get { return mAudioSourceForBGM.mute; }
        set { mAudioSourceForBGM.mute = value; }
    }
    // Background 볼륨 제어(주로 세팅에서 이 변수를 제어한다)
    private float mVolumeBGM = 0.3f;
    public float VolumeBGM
    {
        get { return mVolumeBGM; }
        set { mVolumeBGM = value; mAudioSourceForBGM.volume = mVolumeBGM; }
    }
    
    public bool IsPlayingBGM { get; private set; } = false;

    protected override void Awake() 
    {
        base.Awake();

        InitSoundPlayManager();
    }

    // 사운드 플레이 매니저 초기화
    private void InitSoundPlayManager()
    {
        Debug.Log("InitSoundPlayManager() initialized");

        // 사운드 재생기 폴링을 위한 기본 구조 초기화
        mAudioSourceForBGM = gameObject.AddComponent<AudioSource>();
        mAudioSourceUsing = new GameObject("AudioSourceUsing").transform;
        mAudioSourceUsing.SetParent(transform);
        mAudioSourcePool = new GameObject("AudioSourcePool").transform;
        mAudioSourcePool.SetParent(transform);
        for (int i = 0; i < PoolCount; ++i)
        {
            GameObject child = new GameObject("AudioSource" + i);
            child.transform.SetParent(mAudioSourcePool);
            child.AddComponent<AudioSource>();
            child.SetActive(false);
        }
    }

    // 짧은 효과음 재생을 요청한다.
    // 이미 재생중인 경우 해당 클립을 멈추고 다시 재생한다.(isOverlappable이 false일 경우)
    // clipname 은 Resources/Sound/SFX 폴더 하위의 오디오파일(mp3, wav 등등)의 이름을 넣어줘야 한다.
    public void PlaySFX(string clipname, bool isOverlappable = false)
    {
        if (IsMuteSFX || clipname.Length <= 0)
            return;


        string fullname = "Sound/SFX/" + clipname;
        string key = fullname;

        if(isOverlappable)
        {
            // 동일한 사운드라도 무조건 재생시키려면 key값에 현재시간을 넣어서 무조건 다르게 하는 식으로 처리.
            key = DateTime.Now.Ticks.ToString();
        }
        else
        {
            // 뒤에 붙은 숫자를 떼어낸것을 key로 한다.
            // 즉 앞쪽에 있는 순수 clipname이 동일하면 같은 clip이라고 판단하고 기존꺼를 다시 재생한다.
            // 예) sound01.mp3, sound02.mp3 는 모두 동일한 sound라는 재생파일로 취급한다.
            int suffixNumIdx = GetIndexOfSuffixNumber(clipname);
            if(suffixNumIdx < clipname.Length)
            {
                key = "Sound/SFX/" + clipname.Substring(0, suffixNumIdx);
            }
        }

        // 이미 재생중인경우 해당클립을 멈추고 다시 재생시킴
        if (mPlaylist.ContainsKey(key))
        {
            PlayingClipInfo info = mPlaylist[key];
            info.source.clip = info.clip;
            info.source.volume = VolumeSFX;
            info.source.Play();
        }
        else
        {
            // 새로운 클립인 경우 source를 할당받아 목록에 등록 후 재생
            AudioClip clip = GetClip(fullname);
            if (clip == null)
                return;

            AudioSource source = GetAudioSourceFromPool();
            if (source != null)
            {
                PlayingClipInfo info = new PlayingClipInfo();
                info.key = key;
                info.clip = clip;
                info.source = source;
                mPlaylist[key] = info;
                
                info.source.clip = info.clip;
                info.source.volume = VolumeSFX;
                info.source.Play();
            }
        }
    }
    
    public void PlayUISFX(string clipname)
    {
        if (IsMuteSFX || clipname.Length <= 0)
            return;

        PlaySFX("UI/" + clipname);
    }
    
    public void PlayInGameSFX(string clipname, bool isOverlappable = false)
    {
        if (IsMuteSFX || clipname.Length <= 0)
            return;

        PlaySFX("InGame/" + clipname, isOverlappable);
    }
    public void PlayInGameSFXs(string[] clipnames, bool isOverlappable = false)
    {
        if (clipnames == null || clipnames.Length <= 0)
            return;

        int idx = UnityEngine.Random.Range(0, clipnames.Length);
        PlayInGameSFX(clipnames[idx], isOverlappable);
    }

    // 배경음악을 재생한다.(Fade in)
    // clipname 은 Resources/Sound/BGM 폴더 하위의 오디오파일(mp3, wav 등등)의 이름을 넣어줘야 한다.
    public void PlayBGM(string clipname, float fadeInSec = 0)
    {
        string fullname = "Sound/BGM/" + clipname;
        AudioClip clip = GetClip(fullname);
        if (clip != null)
        {
            PlayBGM(clip, fadeInSec);
        }
    }
    
    public void PlayBGM(AudioClip clip, float fadeInSec = 0)
    {
        IsPlayingBGM = true;
        if(fadeInSec > 0)
        {
            mAudioSourceForBGM.clip = clip;
            mAudioSourceForBGM.loop = true;
            mAudioSourceForBGM.volume = 0;
            mAudioSourceForBGM.DOKill();
            mAudioSourceForBGM.Play();
            mAudioSourceForBGM.DOFade(mVolumeBGM, fadeInSec);
        }
        else
        {
            mAudioSourceForBGM.clip = clip;
            mAudioSourceForBGM.loop = true;
            mAudioSourceForBGM.volume = mVolumeBGM;
            mAudioSourceForBGM.DOKill();
            mAudioSourceForBGM.Play();
        }
    }

    // 배경음악 멈춤(Fade out)
    public void StopBGM(float fadeoutSec = 0)
    {
        IsPlayingBGM = false;
        if(fadeoutSec > 0)
        {
            mAudioSourceForBGM.DOKill();
            mAudioSourceForBGM.DOFade(0, fadeoutSec).OnComplete(() =>
            {
                mAudioSourceForBGM.volume = mVolumeBGM;
                mAudioSourceForBGM.Stop();
            });
        }
        else
        {
            mAudioSourceForBGM.DOKill();
            mAudioSourceForBGM.Stop();
        }
    }

    // 배경음악 일시정지
    public void PauseBGMMusic()
    {
        mAudioSourceForBGM.DOKill();
        mAudioSourceForBGM.Pause();
    }

    // 배경음악 재개
    public void ResumeBGMMusic()
    {
        mAudioSourceForBGM.DOKill();
        mAudioSourceForBGM.UnPause();
    }

    // AudioSource를 Pool에서 꺼낸다(자체 풀 사용)
    private AudioSource GetAudioSourceFromPool()
    {
        if(mAudioSourcePool.childCount <= 0)
            return null;

        Transform target = mAudioSourcePool.GetChild(0);
        target.gameObject.SetActive(true);
        target.SetParent(mAudioSourceUsing);
        AudioSource src = target.GetComponent<AudioSource>();
        src.Stop();
        return src;
    }

    // 사용된 AudioSource를 Pool로 반납한다(자체 풀 사용)
    private void ReturnAudioSourceToPool(AudioSource source)
    {
        source.Stop();
        source.transform.SetParent(mAudioSourcePool);
        source.gameObject.SetActive(false);
    }

    // 오디오 클립 리소스를 가져온다(캐시사용, 없을시 로딩)
    private AudioClip GetClip(string name)
    {
        // 캐시여부 확인하여 이전에 가져온적이 있다면 캐싱
        if(mAudioClips.ContainsKey(name))
        {
            return mAudioClips[name];
        }
        else
        {
            // 이전에 가져온적이 없으면 Resources폴더에서 로딩해온다.
            AudioClip clip = Resources.Load<AudioClip>(name);
            if(clip == null)
            {
                //Debug.LogError("No Audio Clip : " + name);
                return null;
            }

            mAudioClips[name] = clip;
            return clip;
        }
    }

    // 현재 재생중인 클립들이 모두 재생완료되면 pool반납하고 재생목록에서 삭제한다.
    void LateUpdate() 
    {
        if(mPlaylist.Count <= 0)
            return;

        PlayingClipInfo[] infos = new List<PlayingClipInfo>(mPlaylist.Values).ToArray();
        foreach(PlayingClipInfo info in infos)
        {
            if(!info.source.isPlaying)
            {
                ReturnAudioSourceToPool(info.source);
                mPlaylist.Remove(info.key);
            }
        }
    }

    // 입력된 string의 뒤쪽에 붙은 숫자의 위치를 반환
    // 예) "abc_123"이 입력되면 4가 반환됨
    private int GetIndexOfSuffixNumber(string name)
    {
        for(int i = name.Length - 1; i >= 0; i--)
        {
            char ch = name[i];
            if (ch < '0' || '9' < ch)
                return i + 1;
        }
        return 0;
    }
}

public class PlayingClipInfo
{
    public string key = "";
    public AudioClip clip = null;
    public AudioSource source = null;
}