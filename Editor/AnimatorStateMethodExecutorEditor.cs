using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Utils.Animation
{
    [CustomEditor(typeof(AnimatorStateMethodExecutor))]
    public class AnimatorStateMethodExecutorEditor : Editor
    {
        private AnimatorStateMethodExecutor executor;
        private string[] methodNames;
        private MethodInfo[] methods;

        private SerializedProperty animatorProperty;
        private SerializedProperty methodListProperty;

        private void OnEnable()
        {
            executor = (AnimatorStateMethodExecutor)target;
            methods = typeof(AnimationHelper).GetMethods(BindingFlags.Public | BindingFlags.Static);
            methodNames = new string[methods.Length];
            for (int i = 0; i < methods.Length; i++)
            {
                methodNames[i] = methods[i].Name;
            }

            animatorProperty = serializedObject.FindProperty("Animator");
            methodListProperty = serializedObject.FindProperty("MethodList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(animatorProperty);

            if (executor.Animator != null)
            {
                for (int i = 0; i < methodListProperty.arraySize; i++)
                {
                    SerializedProperty methodProperty = methodListProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    DrawAnimationStateNameField(methodProperty);
                    DrawExecuteTimeField(methodProperty);
                    DrawMethodSelectionZone(methodProperty);

                    EditorGUILayout.EndVertical();
                }

                // Кнопка для добавления нового метода
                if (GUILayout.Button("Add Method"))
                {
                    methodListProperty.arraySize++;
                }

                if (methodListProperty.arraySize > 0)
                {
                    // Кнопка для удаления последнего метода
                    if (GUILayout.Button("Remove Last Method"))
                    {
                        methodListProperty.arraySize--;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMethodSelectionZone(SerializedProperty methodProperty)
        {
            SerializedProperty selectedMethodNameProperty = methodProperty.FindPropertyRelative("SelectedMethodName");
            SerializedProperty selectedMethodArgumentsProperty = methodProperty.FindPropertyRelative("SelectedMethodArguments");
            SerializedProperty previousSelectedMethodNameProperty = methodProperty.FindPropertyRelative("PreviousSelectedMethodName");

            // 1. Select the method from the dropdown
            int selectedMethodIndex = GetSelectedMethodIndex(selectedMethodNameProperty.stringValue);
            selectedMethodIndex = EditorGUILayout.Popup("Method", selectedMethodIndex, methodNames);

            if (selectedMethodIndex < 0) return;

            selectedMethodNameProperty.stringValue = methodNames[selectedMethodIndex];
            MethodInfo selectedMethod = methods[selectedMethodIndex];
            ParameterInfo[] parameters = selectedMethod.GetParameters();

            // 2. Initialize arguments if the method changed
            if (selectedMethodNameProperty.stringValue != previousSelectedMethodNameProperty.stringValue)
            {
                selectedMethodArgumentsProperty.ClearArray();
                for (int i = 0; i < parameters.Length; i++)
                {
                    // Add a new empty element to the array
                    selectedMethodArgumentsProperty.InsertArrayElementAtIndex(i);
                    SerializedProperty argumentProperty = selectedMethodArgumentsProperty.GetArrayElementAtIndex(i);

                    // Initialize the argument based on the parameter type
                    InitializeArgument(argumentProperty, parameters[i].ParameterType);
                }
                previousSelectedMethodNameProperty.stringValue = selectedMethodNameProperty.stringValue;
            }

            // 3. Draw fields for each parameter
            for (int j = 0; j < parameters.Length; j++)
            {
                SerializedProperty argumentProperty = selectedMethodArgumentsProperty.GetArrayElementAtIndex(j);
                DrawArgumentField(argumentProperty, parameters[j]);
            }
        }

        private void InitializeArgument(SerializedProperty argumentProperty, Type parameterType)
        {
            SerializedProperty typeProperty = argumentProperty.FindPropertyRelative("Type");
            if (parameterType == typeof(float))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.Float;
                argumentProperty.FindPropertyRelative("FloatValue").floatValue = 0f;
            }
            else if (parameterType == typeof(int))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.Int;
                argumentProperty.FindPropertyRelative("IntValue").intValue = 0;
            }
            else if (parameterType == typeof(string))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.String;
                argumentProperty.FindPropertyRelative("StringValue").stringValue = "";
            }
            else if (parameterType == typeof(Vector3))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.Vector3;
                argumentProperty.FindPropertyRelative("Vector3Value").vector3Value = Vector3.zero;
            }
            else if (parameterType == typeof(bool))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.Bool;
                argumentProperty.FindPropertyRelative("BoolValue").boolValue = false;
            }
            else if (parameterType == typeof(GameObject))
            {
                typeProperty.enumValueIndex = (int)SerializableArgument.ArgumentType.GameObject;
                argumentProperty.FindPropertyRelative("GameObjectValue").objectReferenceValue = null;
            }
            // ... Handle other types similarly
        }

        private void DrawArgumentField(SerializedProperty argumentProperty, ParameterInfo parameter)
        {
            SerializedProperty typeProperty = argumentProperty.FindPropertyRelative("Type");
            var parameterType = parameter.ParameterType;

            if (parameterType == typeof(float))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("FloatValue"), new GUIContent(parameter.Name));
            }
            else if (parameterType == typeof(int))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("IntValue"), new GUIContent(parameter.Name));
            }
            else if (parameterType == typeof(string))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("StringValue"), new GUIContent(parameter.Name));
            }
            else if (parameterType == typeof(Vector3))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("Vector3Value"), new GUIContent(parameter.Name));
            }
            else if (parameterType == typeof(bool))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("BoolValue"), new GUIContent(parameter.Name));
            }
            else if (parameterType == typeof(GameObject))
            {
                EditorGUILayout.PropertyField(argumentProperty.FindPropertyRelative("GameObjectValue"), new GUIContent(parameter.Name));
            }
            // ... Handle other types similarly
        }

        private void DrawExecuteTimeField(SerializedProperty methodProperty)
        {
            SerializedProperty executeTimeProperty = methodProperty.FindPropertyRelative("ExecuteTime");
            executeTimeProperty.enumValueIndex = (int)(AnimatorStateEventType)EditorGUILayout.EnumPopup("Execute time", (AnimatorStateEventType)executeTimeProperty.enumValueIndex);
        }

        private void DrawAnimationStateNameField(SerializedProperty methodProperty)
        {
            SerializedProperty animationStateNameProperty = methodProperty.FindPropertyRelative("AnimationStateName");
            var animatorStates = GetAnimatorStateInfo();
            var statesName = animatorStates.Select(x => x.name).ToArray();
            int selectedIndex = Mathf.Max(0, Array.IndexOf(statesName, animationStateNameProperty.stringValue));
            selectedIndex = EditorGUILayout.Popup("Animation state", selectedIndex, statesName);
            animationStateNameProperty.stringValue = statesName[selectedIndex];
        }

        private List<AnimatorState> GetAnimatorStateInfo()
        {
            AnimatorController ac = executor.Animator.runtimeAnimatorController as AnimatorController;
            AnimatorControllerLayer[] acLayers = ac.layers;
            List<AnimatorState> allStates = new List<AnimatorState>();
            foreach (AnimatorControllerLayer i in acLayers)
            {
                ChildAnimatorState[] animStates = i.stateMachine.states;
                foreach (ChildAnimatorState j in animStates)
                    allStates.Add(j.state);
            }
            return allStates;
        }

        private int GetSelectedMethodIndex(string methodName)
        {
            for (int i = 0; i < methodNames.Length; i++)
            {
                if (methodNames[i] == methodName)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
