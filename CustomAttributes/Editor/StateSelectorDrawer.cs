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
    int[] layerIdxList = null;
    int[] actionIDList = null;
    int idx = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
            base.OnGUI(position, property, label);
        else
        {
            AnimatorController ac = (AnimatorController)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/03_Animation/Man_Controller_Base.controller", typeof(AnimatorController));
            if(ac != null)
            {
                if(stateNameList == null)
                {
                    List<string> names = new List<string>();
                    List<string> clips = new List<string>();
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
                            layerIndicies.Add(layerIndex);

                            foreach (var condi in tr.conditions)
                            {
                                if (Animator.StringToHash(condi.parameter) == AnimParam.ActionType
                                 || Animator.StringToHash(condi.parameter) == AnimParam.HandType)
                                {
                                    actionIDs.Add((int)condi.threshold);
                                    break;
                                }
                            }
                        }
                    }

                    stateNameList = names.ToArray();
                    animClipList = clips.ToArray();
                    actionIDList = actionIDs.ToArray();
                    layerIdxList = layerIndicies.ToArray();
                    idx = 0;
                }

                idx = GetIndex(property.stringValue);

                EditorGUI.BeginChangeCheck();
                idx = EditorGUI.Popup(position, label.text, idx, stateNameList);

                //if (EditorGUI.EndChangeCheck())  //Inspector창에서 콤보박스 선택시 진입
                {
                    //if(idx > 0)
                    {
                        property.stringValue = stateNameList[idx];
                        property.serializedObject.FindProperty("AnimatorParamActionType").intValue = actionIDList[idx];
                        property.serializedObject.FindProperty("AnimationClipName").stringValue = animClipList[idx];
                        property.serializedObject.FindProperty("MotionName").stringValue = property.serializedObject.targetObject.GetType().ToString();
                        property.serializedObject.FindProperty("LayerIndex").intValue = layerIdxList[idx];
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