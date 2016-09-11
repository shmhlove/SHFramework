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
public class SHDrawerToShowFunc : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        InspectorGUIToFunctionButton();
    }

    void InspectorGUIToFunctionButton()
    {
#if UNITY_EDITOR_WIN
        Type pType              = target.GetType();
        MethodInfo[] pMethods   = pType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        for (int iLoop = 0; iLoop < pMethods.Length; ++iLoop)
        {
            MethodInfo pMethod  = pMethods[iLoop];
            object[] pAttribute = pMethod.GetCustomAttributes(typeof(SHAttributeToShowFunc), true);
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