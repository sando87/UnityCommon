using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 씬전환을 부드럽게 시켜주는 기능(해당 기능은 저사양 장비에서 씬 전환시 화면 끊김 또는 메모리 최적화관련 이슈를 개선하기 위해 설계됨)
/// 첫번째 주요기능
/// - 전환하고자 하는 씬에 초기에 배치된 객체가 많아서 씬 로딩 자체가 느릴때 중간에 로딩씬을 두어서 부드럽게 넘어가도록 설계됨(안그러면 검은화면에서 꽤 오래 멈춘것처럼 보임)
/// 두번째 주요기능
/// - 1번씬에서 2번씬으로 전환시 원래데로라면 1번씬의 리소스와 2번씬의 리소스가 한꺼번에 메모리에 올라가야 할 만큼의 공간이 필요해서 문제가 되었는데
///   이 기능을 사용하면 2번씬 로딩하기 전에 1번씬에서 사용된 리소스를 모두 해제하고 2번씬을 진입하기 때문에 메모리가 그만큼 덜 필요해지게 된다.
/// </summary>

public class SceneSwtichManager : SingletonMono<SceneSwtichManager>
{
    [SerializeField] Image FadingImage = null;

    public bool IsLoaded { get; private set; } = false;

    // 특별한 절차없이 씬 전환시 FadeInOut만 해주면서 씬 전환이 바로 이루어진다(가벼운 씬 전환시 사용)
    public void LoadSceneImmediately(SceneEnum sceneEnum)
    {
        StartCoroutine(CoLoadSceneImmediately(sceneEnum));
    }
    IEnumerator CoLoadSceneImmediately(SceneEnum sceneEnum)
    {
        IsLoaded = false;
        yield return null;

        yield return CoFading(1, 0.25f);
        SceneManager.LoadScene((int)sceneEnum);

        yield return null;
        // 씬 전환시 안쓰는 메모리 전부 해제
        // ResourcesCache.ReleaseAllCachedObjects();
        // MapObjectDatabase.Instance.ReleasePrefabs();
        // Resources.UnloadUnusedAssets();
        // System.GC.Collect();
        // yield return null;

        yield return CoFading(0, 0.25f);

        IsLoaded = true;
    }

    // 씬 전환시 로딩화면을 중간에 보여주면서 전환한다(무거운 씬 전환시 사용)
    public void LoadSceneWithLoadingUI(SceneEnum sceneEnum)
    {
        StartCoroutine(CoLoadSceneWithLoadingUI(sceneEnum));
    }

    IEnumerator CoLoadSceneWithLoadingUI(SceneEnum sceneEnum)
    {
        IsLoaded = false;
        Scene curScene = SceneManager.GetActiveScene();
        
        yield return CoFading(1);
        AsyncOperation aoShort = SceneManager.LoadSceneAsync((int)SceneEnum.LoadingScene, LoadSceneMode.Additive);
        yield return new WaitUntil(() => aoShort.isDone);
        //yield return CoFading(0);
        FadingImage.color = new Color(0, 0, 0, 0);
        yield return new WaitForSecondsRealtime(0.2f);

        AsyncOperation aoOld = SceneManager.UnloadSceneAsync(curScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        yield return new WaitUntil(() => aoOld.isDone);
        yield return new WaitForSecondsRealtime(0.2f);

        yield return null;
        // 씬 전환시 안쓰는 메모리 전부 해제
        // ResourcesCache.ReleaseAllCachedObjects();
        // MapObjectDatabase.Instance.ReleasePrefabs();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        yield return null;

        AsyncOperation aoNew = SceneManager.LoadSceneAsync((int)sceneEnum, LoadSceneMode.Additive);
        yield return new WaitUntil(() => aoNew.isDone);

        //yield return CoFading(1);
        FadingImage.color = new Color(0, 0, 0, 1);
        AsyncOperation aoShort2 = SceneManager.UnloadSceneAsync((int)SceneEnum.LoadingScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        yield return new WaitUntil(() => aoShort2.isDone);
        yield return CoFading(0);

        IsLoaded = true;
    }

    IEnumerator CoFading(float alpha, float tweenDuration = 0.5f)
    {
        FadingImage.DOKill();
        FadingImage.DOFade(alpha, tweenDuration);
        yield return new WaitForSecondsRealtime(tweenDuration);
    }

    public SceneEnum GetCurrentScene()
    {
        return (SceneEnum)SceneManager.GetActiveScene().buildIndex;
    }
}

/// <summary>
/// 빌드시에 로드하는 씬 리스트와 매칭되는 이넘
/// 빌드설정에서의 씬 리스트와 이넘이 항상 매칭되도록 할 것.
/// </summary>
public enum SceneEnum
{
    CompanyLogo, // 0
    GameTitle,
    StoryDirection01,
    StoryDirection02,
    ChapterSelect, // 4
    InGame,
    Initialize,
    InGameTutorial, // 7
    ShootingGame, // 8
    Ending, // 9
    Credits, // 10
    StoryDirection03, // 11
    LoadingScene, // 12
}
