#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(StateSelectorAttribute))]
public class StateSelectorDrawer : PropertyDrawer
{
    string[] stateNameList = null;
    string[] animClipList = null;
    float[] animClipLengthList = null;
    int[] layerIdxList = null;
    int[] actionIDList = null;
    string[] enumNames = null;
    int idx = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Generic)
            base.OnGUI(position, property, label);
        else
        {
            AnimatorController ac = (AnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Anims/_common/UserUnitAnimBase.controller", typeof(AnimatorController));
            if(ac != null)
            {
                if(stateNameList == null)
                {
                    enumNames = System.Enum.GetNames(typeof(AnimActionID));
                    int enumCount = MyUtils.EnumCount<AnimActionID>();
                    int[] actionIDs = new int[enumCount];
                    for (int i = 0; i < enumCount; ++i)
                        actionIDs[i] = -1;
                    string[] names = new string[enumCount];
                    string[] clips = new string[enumCount];
                    float[] clipLengths = new float[enumCount];
                    int[] layerIndicies = new int[enumCount];

                    for (int layerIndex = 0; layerIndex < ac.layers.Length; ++layerIndex)
                    {
                        AnimatorStateMachine sm = ac.layers[layerIndex].stateMachine;
                        AnimatorStateTransition[] trans = sm.anyStateTransitions;
                        foreach (AnimatorStateTransition tr in trans)
                        {
                            int actionID = -1;
                            foreach (var condi in tr.conditions)
                            {
                                if (Animator.StringToHash(condi.parameter) == AnimParam.ActionType)
                                {
                                    actionID = (int)condi.threshold;
                                    break;
                                }
                            }

                            if(actionID < 0 || actionIDs[actionID] >= 0)
                                continue;

                            actionIDs[actionID] = (actionID);
                            names[actionID] = (tr.destinationState.name);
                            clips[actionID] = (tr.destinationState.motion.name);
                            clipLengths[actionID] = (tr.destinationState.motion.averageDuration);
                            layerIndicies[actionID] = (layerIndex);
                        }
                    }

                    stateNameList = names;
                    animClipList = clips;
                    animClipLengthList = clipLengths;
                    actionIDList = actionIDs;
                    layerIdxList = layerIndicies;
                    idx = 0;
                }

                int currentActionID = property.serializedObject.FindProperty("AnimState.AnimatorParamActionType").intValue;
                idx = GetIndex(currentActionID);

                EditorGUI.BeginChangeCheck();
                idx = EditorGUI.Popup(position, label.text, idx, enumNames);

                //if (EditorGUI.EndChangeCheck())  //Inspector창에서 콤보박스 선택시 진입
                {
                    //if(idx > 0)
                    {
                        property.serializedObject.FindProperty("AnimState.StateName").stringValue = stateNameList[idx];
                        property.serializedObject.FindProperty("AnimState.AnimatorParamActionType").intValue = actionIDList[idx];
                        property.serializedObject.FindProperty("AnimState.AnimationClipName").stringValue = animClipList[idx];
                        property.serializedObject.FindProperty("AnimState.MotionName").stringValue = property.serializedObject.targetObject.GetType().ToString();
                        property.serializedObject.FindProperty("AnimState.LayerIndex").intValue = layerIdxList[idx];
                        property.serializedObject.FindProperty("AnimState.ClipLength").floatValue = animClipLengthList[idx];
                    }
                }
            }
            else
            {
                LOG.warn();
            }
        }
    }

    private int GetIndex(int actionID)
    {
        if(actionIDList == null) return 0;

        for (int i = 0; i < actionIDList.Length; ++i)
        {
            if (actionIDList[i] == actionID)
                return i;
        }
        return 0;
    }
}

#endif