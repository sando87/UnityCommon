#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Unity Editor에서 Play시작하기전 자동으로 프로젝트 Refresh를 수행하도록 한다.
/// </summary>
[InitializeOnLoadAttribute]
public static class RefreshBeforePlay
{
    static RefreshBeforePlay()
    {
        EditorApplication.playModeStateChanged += PlayRefresh;
    }
    private static void PlayRefresh(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            AssetDatabase.Refresh();
        }
    }
}
#endif
