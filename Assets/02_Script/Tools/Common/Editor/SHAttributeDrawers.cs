using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
#endif

// 인스펙터에 함수이름을 버튼형식으로 노출시킵니다.
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class FuncButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        InspectorGUIToFunctionButton();
    }

    void InspectorGUIToFunctionButton()
    {
#if UNITY_EDITOR
        Type pType              = target.GetType();
        MethodInfo[] pMethods   = pType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        for (int iLoop = 0; iLoop < pMethods.Length; ++iLoop)
        {
            MethodInfo pMethod  = pMethods[iLoop];
            object[] pAttribute = pMethod.GetCustomAttributes(typeof(FuncButton), true);
            if (0 >= pAttribute.Length)
                continue;

            if (true == GUILayout.Button(pMethod.Name))
            {
                if (false == Application.isPlaying)
                {
                    EditorUtility.DisplayDialog(pMethod.Name, "실행 후 이용해 주세요!!", "확인");
                    return;
                }

                ((Component)target).SendMessage(pMethod.Name, SendMessageOptions.DontRequireReceiver);
            }
        }
#endif
    }
}

// 인스펙터에 노출되는 필드를 읽기전용으로 노출합니다.
[CustomPropertyDrawer(typeof(ReadOnlyField))]
public class ReadOnlyFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
#if UNITY_EDITOR
        GUI.enabled = false;
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                EditorGUI.LabelField(position, label.text, prop.intValue.ToString());
                break;

            case SerializedPropertyType.Boolean:
                EditorGUI.LabelField(position, label.text, prop.boolValue.ToString());
                break;

            case SerializedPropertyType.Float:
                EditorGUI.LabelField(position, label.text, prop.floatValue.ToString("0.00000"));
                break;

            case SerializedPropertyType.String:
            case SerializedPropertyType.Character:
                EditorGUI.LabelField(position, label.text, prop.stringValue);
                break;

            case SerializedPropertyType.Color:
                EditorGUI.ColorField(position, label.text, prop.colorValue);
                break;

            case SerializedPropertyType.ObjectReference:
                EditorGUI.ObjectField(position, label.text, prop.objectReferenceValue, typeof(System.Object), true);
                break;
                
            case SerializedPropertyType.Vector2:
                EditorGUI.Vector2Field(position, label.text, prop.vector2Value);
                break;

            case SerializedPropertyType.Vector3:
                EditorGUI.Vector3Field(position, label.text, prop.vector3Value);
                break;

            case SerializedPropertyType.Vector4:
                EditorGUI.Vector4Field(position, label.text, prop.vector4Value);
                break;

            case SerializedPropertyType.Quaternion:
                EditorGUI.LabelField(position, label.text, prop.quaternionValue.ToString());
                break;

            case SerializedPropertyType.Rect:
                EditorGUI.RectField(position, label.text, prop.rectValue);
                break;
                
            case SerializedPropertyType.ArraySize:
                EditorGUI.LabelField(position, label.text, prop.arraySize.ToString());
                break;
                
            case SerializedPropertyType.AnimationCurve:
                EditorGUI.CurveField(position, label.text, prop.animationCurveValue);
                break;

            case SerializedPropertyType.Bounds:
                EditorGUI.BoundsField(position, label.text, prop.boundsValue);
                break;

            case SerializedPropertyType.Gradient:
            default:
                EditorGUI.LabelField(position, label.text, "(not supported)");
                break;
        }
        GUI.enabled = true;
#endif
    }
}
