#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class AP_MethodModule
{
    private bool b_addMethods = false;                                  // use when you press button +

    // Display a list of custom method in the custom editor
    public void displayMethodList(string _Title,
                                  EditorMethods editorMethods,
                                  SerializedProperty _methods, 
                                  List<EditorMethodsList.MethodsList> myScriptMethods, 
                                  GUIStyle style_Color01,
                                  GUIStyle style_Color02,
                                  string helpBoxText)
    {
        #region
        //-> Custom methods
        EditorGUILayout.BeginVertical(style_Color01);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(_Title, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);

        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            b_addMethods = true;
        }

        editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02);

        if (b_addMethods)
        {
            editorMethods.AddMethodsToList(_methods);
            b_addMethods = false;
        }

        EditorGUILayout.EndVertical();
        #endregion
    }

    public void displayOneMethod(string _Title,
                                  EditorMethods editorMethods,
                                  SerializedProperty _methods,
                                  List<EditorMethodsList.MethodsList> myScriptMethods,
                                  GUIStyle style_Color01,
                                  GUIStyle style_Color02,
                                  string helpBoxText)
    {
        #region
        //-> Custom methods
        EditorGUILayout.BeginVertical(style_Color01);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(_Title, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);

        if (_methods.arraySize < 1)
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                b_addMethods = true;
            }

        editorMethods.DisplayMethodsOnEditor(myScriptMethods, _methods, style_Color02);

        if (b_addMethods)
        {
            editorMethods.AddMethodsToList(_methods);
            b_addMethods = false;
        }

        EditorGUILayout.EndVertical();
        #endregion
    }

  
}
#endif
