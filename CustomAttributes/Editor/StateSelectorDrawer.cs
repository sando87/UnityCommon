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
                    List<string> names = new List<string>();
                    List<string> clips = new List<string>();
                    List<float> clipLengths = new List<float>();
                    List<int> actionIDs = new List<int>();
                    List<int> layerIndicies = new List<int>();

                    for (int layerIndex = 0; layerIndex < ac.layers.Length; ++layerIndex)
                    {
                        AnimatorStateMachine sm = ac.layers[layerIndex].stateMachine;
                        AnimatorStateTransition[] trans = sm.anyStateTransitions;
                        foreach (AnimatorStateTransition tr in trans)
                        {
                            names.Add(tr.destinationState.name);
                            clips.Add(tr.destinationState.motion.name);
                            clipLengths.Add(tr.destinationState.motion.averageDuration);
                            layerIndicies.Add(layerIndex);

                            foreach (var condi in tr.conditions)
                            {
                                if (Animator.StringToHash(condi.parameter) == AnimParam.ActionType)
                                {
                                    actionIDs.Add((int)condi.threshold);
                                    break;
                                }
                            }
                        }
                    }

                    stateNameList = names.ToArray();
                    animClipList = clips.ToArray();
                    animClipLengthList = clipLengths.ToArray();
                    actionIDList = actionIDs.ToArray();
                    layerIdxList = layerIndicies.ToArray();
                    idx = 0;
                }

                string currentStateName = property.serializedObject.FindProperty("AnimState.StateName").stringValue;
                idx = GetIndex(currentStateName);

                EditorGUI.BeginChangeCheck();
                idx = EditorGUI.Popup(position, label.text, idx, stateNameList);

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

    private int GetIndex(string stateName)
    {
        if(stateNameList == null) return 0;

        for (int i = 0; i < stateNameList.Length; ++i)
        {
            if (stateNameList[i].Equals(stateName))
                return i;
        }
        return 0;
    }
}

#endif