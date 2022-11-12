#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[ExecuteAlways]
public class ShortcutInEditor : MonoBehaviour
{
    // 단축키 Cmd + e 로 현재 인스펙터 잠금 기능 
    [MenuItem("/Users/Toggle InspectorLock %e")]
    public static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
    
    // 단축키 Cmd + w 로 현재 포커스된 View 최대/최소화 토글
    [MenuItem("/Users/Toggle View Maximizing %w")]
    public static void ToggleGameViewMaximize()
    {
        UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
    }

    // void Update()
    // {
    //     if(!Application.isPlaying)
    //     {
    //         // UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    //         // UnityEditor.SceneView.RepaintAll();
    //     }
    // }
}
#endif