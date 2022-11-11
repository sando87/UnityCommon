using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class LockInspector
{
    // 단축키 Cmd + e 로 현재 인스펙터 잠금 기능 
    [MenuItem("/Users/Toggle InspectorLock %e")]
    private static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}
#endif