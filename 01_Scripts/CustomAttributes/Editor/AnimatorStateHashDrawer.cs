#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;


namespace PahlBit
{
    [CustomPropertyDrawer(typeof(AnimatorStateHashAttribute))]
    public class AnimatorStateHashDrawer : PropertyDrawer
    {
        string[] mStates = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // int 타입이 아니면 기본 필드로 렌더
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use AnimatorStateHash with int.");
                return;
            }

            // BeginProperty/EndProperty 사용 (undo/drag 지원 등)
            EditorGUI.BeginProperty(position, label, property);

            if (mStates == null)
            {
                // 소유 오브젝트가 Component인지 확인
                var targetObject = property.serializedObject.targetObject;
                Component targetComponent = targetObject as Component;
                if (targetComponent == null)
                {
                    EditorGUI.PropertyField(position, property, label);
                    EditorGUI.EndProperty();
                    return;
                }

                Animator animator = targetComponent.GetComponentInParent<BaseObject>().AnimHelper.GetComponent<Animator>();
                if (animator == null || animator.runtimeAnimatorController == null)
                {
                    EditorGUI.PropertyField(position, property, label);
                    EditorGUI.EndProperty();
                    return;
                }

                var controller = animator.runtimeAnimatorController as AnimatorController;
                if (controller == null)
                {
                    EditorGUI.PropertyField(position, property, label);
                    EditorGUI.EndProperty();
                    return;
                }

                var stateList = new List<string>();
                foreach (var layer in controller.layers)
                {
                    CollectStatesRecursive(layer.stateMachine, stateList);
                }

                mStates = stateList.Distinct().ToArray();

            }

            if (mStates == null || mStates.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
                return;
            }

            // 현재 저장된 hash -> 이름 인덱스
            string currentName = mStates.FirstOrDefault(n => Animator.StringToHash(n) == property.intValue);
            int index = System.Array.IndexOf(mStates, currentName);
            if (index < 0) index = 0;

            // 드롭다운 표시: label.text (string) 오버로드 사용 — GUIContent 바로 전달하면 일부 버전에서 에러
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, label.text, index, mStates);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = Animator.StringToHash(mStates[newIndex]);
            }

            EditorGUI.EndProperty();
        }
        private static void CollectStatesRecursive(AnimatorStateMachine stateMachine, List<string> result)
        {
            // 현재 레벨 states
            foreach (var state in stateMachine.states)
            {
                result.Add(state.state.name);
            }

            // 하위 SubStateMachine 재귀 순회
            foreach (var subStateMachine in stateMachine.stateMachines)
            {
                CollectStatesRecursive(subStateMachine.stateMachine, result);
            }
        }
    }
}
#endif