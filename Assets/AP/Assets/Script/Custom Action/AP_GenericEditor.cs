//Description : AP_GenericEditor : AP_Generic editor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(AP_Generic))]
public class AP_GenericEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector


    SerializedProperty b_feedbackActivated;
    SerializedProperty feedbackIDList;

    SerializedProperty b_VoiceOverActivated;
    SerializedProperty diaryIDList;
    SerializedProperty methodsList;

    public EditorMethods editorMethods;                                         // access the component EditorMethods
    public AP_MethodModule methodModule;


	private Texture2D MakeTex(int width, int height, Color col) {						// use to change the GUIStyle
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	private Texture2D 		Tex_01;
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 		= serializedObject.FindProperty ("SeeInspector");
	
        feedbackIDList = serializedObject.FindProperty("feedbackIDList");
        b_feedbackActivated = serializedObject.FindProperty("b_feedbackActivated");


        b_VoiceOverActivated = serializedObject.FindProperty("b_VoiceOverActivated");
        diaryIDList = serializedObject.FindProperty("diaryIDList");

        methodsList = serializedObject.FindProperty("methodsList");

        editorMethods = new EditorMethods();
        methodModule = new AP_MethodModule();


        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .1f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
            Tex_04 = MakeTex(2, 2, new Color(.88F, .88f, .88f, 1f));
            Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
        }
    }


	public override void OnInspectorGUI()
	{
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		serializedObject.Update ();

        AP_Generic myScript = (AP_Generic)target;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See Inspector :", GUILayout.Width (85));
		EditorGUILayout.PropertyField(SeeInspector, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();

		GUIStyle style_Yellow_01 		= new GUIStyle();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle();	style_Yellow_Strong.normal.background 	= Tex_02;

        /*
        EditorGUILayout.BeginVertical (style_Yellow_01);
        displayFeedbackWhenPuzzleIsLocked(style_Yellow_01);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(style_Orange);
        displayVoiceOverWhenPuzzleIsLocked(style_Orange);
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("");*/
        displayAllTheMethods_FocusStarts(style_Yellow_01, style_Yellow_01);

		serializedObject.ApplyModifiedProperties ();
	}


    //--> display Feedback When Puzzle Is Locked
    private void displayFeedbackWhenPuzzleIsLocked(GUIStyle style_Yellow_01)
    {
        //--> Display feedback
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.LabelField("Display a Text when the focus is activated : ",EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Feedback text : ", GUILayout.Width(90));

        EditorGUILayout.PropertyField(b_feedbackActivated, new GUIContent(""), GUILayout.Width(30));

        for (var i = 0; i < feedbackIDList.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID :", GUILayout.Width(30));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent(""), GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck())
            {
                updateFeedback(i, feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue);
            }

            if (GUILayout.Button("Open Window Tab: w_Feedback"))
            {
                EditorWindow.GetWindow(typeof(w_Feedback));
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.EndVertical();
    }

    //--> display Feedback When Puzzle Is Locked
    private void displayVoiceOverWhenPuzzleIsLocked(GUIStyle style_Orange)
    {
        //--> Display feedback
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.LabelField("Play a Voice Over when he focus is activated : ", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Voice Over : ", GUILayout.Width(90));

        EditorGUILayout.PropertyField(b_VoiceOverActivated, new GUIContent(""), GUILayout.Width(30));

        for (var i = 0; i < diaryIDList.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID :", GUILayout.Width(30));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(diaryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent(""), GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck())
            {
                updateVoiceOver(i, diaryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue);
            }

            if (GUILayout.Button("Open Window Tab: w_TextnVoice"))
            {
                EditorWindow.GetWindow(typeof(w_TextnVoice));
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }


    //--> Update feedback ID using unique ID
    private void updateVoiceOver(int i, int value)
    {
        GameObject obj = GameObject.Find("ingameGlobalManager");
        TextList currentFeedback = Resources.Load(obj.GetComponent<ingameGlobalManager>().dataFolder.currentDatasProjectFolder + "/TextList/wTextnVoices") as TextList;

        if (currentFeedback)
        {
            diaryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue
                = currentFeedback.diaryList[0]._languageSlot[value].uniqueItemID;
        }
    }   


    //--> Update feedback ID using unique ID
    private void updateFeedback(int i, int value)
    {
        GameObject obj = GameObject.Find("ingameGlobalManager");
        TextList currentFeedback = Resources.Load(obj.GetComponent<ingameGlobalManager>().dataFolder.currentDatasProjectFolder + "/TextList/wFeedback") as TextList;

        if (currentFeedback)
        {
            feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue
                = currentFeedback.diaryList[0]._languageSlot[value].uniqueItemID;
        }
    }


    //--> display a list of methods call when the Focus starts
    private void displayAllTheMethods_FocusStarts(GUIStyle style_Yellow_01, GUIStyle style_Blue)
    {
        #region
        //--> Display feedback
        AP_Generic myScript = (AP_Generic)target;

        methodModule.displayMethodList("Methods call when focus player click on the UI Button:",
                                       editorMethods,
                                       methodsList,
                                       myScript.methodsList,
                                       style_Blue,
                                       style_Yellow_01,
                                       "The methods are called in the same order as the list. " +
                                       "\nAll methods must be boolean methods. " +
                                       "\nOther methods will be ignored.");

        #endregion
    }

	void OnSceneGUI( )
	{
	}
}
#endif