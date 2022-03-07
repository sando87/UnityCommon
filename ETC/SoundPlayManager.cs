using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 게임 전반의 Sound 재생처리를 전담한다.
/// 배경음, 효과음 재생처리
/// AudioSource는 자체 Pooling 사용
/// 재생 음원파일 로딩도 자체 캐싱 사용
/// 주요 함수는 2가지: PlaySFX(string clipname), PlayBGM(string clipname)
/// Resources/Sound/SFX 폴더 하위 오디오 파일 이름을 인자로 넣어줘야 한다.
/// Resources/Sound/BGM 폴더 하위 오디오 파일 이름을 인자로 넣어줘야 한다.
/// </summary>

public class SoundPlayManager : CSingletonMono<SoundPlayManager>
{
    //배경 재생을 위한 전용 AudioSource
    [SerializeField] private AudioSource AudioSourceForBGM = null;
    //SFX 재생을 위한 AudioSource Pool Root
    [SerializeField] private Transform AudioSourcePool = null;
    //SFX 재생중인 AudioSource Root
    [SerializeField] private Transform AudioSourceUsing = null;

    // 오디오클립 캐시
    private Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();

    // 재생 후처리를 위해 playlist를 잠시 넣어두는 컨테이너
    private Dictionary<string, AudioClip> mPlaylist = new Dictionary<string, AudioClip>();

    // SFX 음소거 제어(주로 세팅에서 이 변수를 제어한다)
    public bool IsMuteSFX { get; set; } = false;
    // SFX 볼륨 제어(주로 세팅에서 이 변수를 제어한다)
    public float VolumeSFX { get; set; } = 1;
    // Background 음소거 제어(주로 세팅에서 이 변수를 제어한다)
    public bool IsMuteBGM
    {
        get { return AudioSourceForBGM.mute; }
        set { AudioSourceForBGM.mute = value; }
    }
    // Background 볼륨 제어(주로 세팅에서 이 변수를 제어한다)
    private float mVolumeBGM = 1;
    public float VolumeBGM
    {
        get { return mVolumeBGM; }
        set { mVolumeBGM = value; AudioSourceForBGM.volume = mVolumeBGM; }
    }

    // 짧은 효과음 재생을 요청한다.
    // 바로 재생하지 않고 playlist에 넣어준 뒤 LateUpate에서 실제 재생처리를 한다.
    // 후처리 재생한 이유는 동일한 음원파일을 한 프레임에 재생시 소리가 갑자기 커지는 것 방지하기 위함.
    // clipname 은 Resources/Sound/SFX 폴더 하위의 오디오파일(mp3, wav 등등)의 이름을 넣어줘야 한다.
    public void PlaySFX(string clipname)
    {
        if (IsMuteSFX)
            return;

        string fullname = "Sound/SFX/" + clipname;
        if (!mPlaylist.ContainsKey(fullname))
        {
            mPlaylist[fullname] = GetClip(fullname);
        }
    }

    // 배경음악을 재생한다.(Fade in)
    // clipname 은 Resources/Sound/BGM 폴더 하위의 오디오파일(mp3, wav 등등)의 이름을 넣어줘야 한다.
    public void PlayBGM(string clipname, float fadeInSec = 0)
    {
        string fullname = "Sound/BGM/" + clipname;
        AudioClip clip = GetClip(fullname);
        PlayBGM(clip, fadeInSec);
    }
    
    public void PlayBGM(AudioClip clip, float fadeInSec = 0)
    {
        if(fadeInSec > 0)
        {
            AudioSourceForBGM.clip = clip;
            AudioSourceForBGM.loop = true;
            AudioSourceForBGM.volume = 0;
            AudioSourceForBGM.Play();
            AudioSourceForBGM.DOFade(mVolumeBGM, fadeInSec);
        }
        else
        {
            AudioSourceForBGM.clip = clip;
            AudioSourceForBGM.loop = true;
            AudioSourceForBGM.volume = mVolumeBGM;
            AudioSourceForBGM.Play();
        }
    }

    // 배경음악 멈춤(Fade out)
    public void StopBGM(float fadeoutSec = 0)
    {
        if(fadeoutSec > 0)
        {
            AudioSourceForBGM.DOFade(0, fadeoutSec).OnComplete(() =>
            {
                AudioSourceForBGM.volume = mVolumeBGM;
                AudioSourceForBGM.Stop();
            });
        }
        else
        {
            AudioSourceForBGM.Stop();
        }
    }
    // 배경음악 일시정지
    public void PauseBGMMusic()
    {
        AudioSourceForBGM.Pause();
    }
    // 배경음악 재개
    public void ResumeBGMMusic()
    {
        AudioSourceForBGM.UnPause();
    }

    // AudioSource를 Pool에서 꺼낸다(자체 풀 사용)
    private AudioSource GetAudioSourceFromPool()
    {
        Transform target = AudioSourcePool.GetChild(0);
        target.gameObject.SetActive(true);
        target.SetParent(AudioSourceUsing);
        AudioSource src = target.GetComponent<AudioSource>();
        src.Stop();
        return src;
    }
    // 사용된 AudioSource를 Pool로 반납한다(자체 풀 사용)
    private void ReturnAudioSourceToPool(AudioSource source)
    {
        source.Stop();
        source.transform.SetParent(AudioSourcePool);
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
                Debug.LogError("No Audio Clip : " + name);
                return null;
            }

            mAudioClips[name] = clip;
            return clip;
        }
    }

    // SFX 효과음 재생은 실제 여기서 재생처리
    void LateUpdate() 
    {
        // 재생 후처리 수행(실제 재생 수행)
        foreach(var playlist in mPlaylist)
        {
            // 오디오 클립과 그걸 재생할 플레이어(AudioSource)를 Pool에서 할당받아 재생한다.
            AudioClip clip = playlist.Value;
            AudioSource player = GetAudioSourceFromPool();
            player.clip = clip;
            player.volume = VolumeSFX;
            player.Play();

            // 사운드 클립이 모두 재생되면(length초 뒤에) 플레이어(AudioSource) 자동 반납
            float length = clip.length;
            this.ExInvoke(length, () =>
            {
                player.clip = null;
                ReturnAudioSourceToPool(player);
            });
        }

        mPlaylist.Clear();
    }

    

}
