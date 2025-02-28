// Description : Custom Editor for actionsWhenPuzzleIsSolved.cs
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(actionsWhenPuzzleIsSolved))]
public class actionsWhenPuzzleIsSolvedEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
    SerializedProperty          onlyFocusMode;
    SerializedProperty          methodsList;
    SerializedProperty          listOfEvent;

    SerializedProperty          a_puzzleSolved;
    SerializedProperty          a_puzzleSolvedVolume;

    SerializedProperty          objectActivatedWhenPuzzleIsSolved;


    public EditorMethods        editorMethods;                                         // access the component EditorMethods
  
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
		SeeInspector 			= serializedObject.FindProperty ("SeeInspector");
        onlyFocusMode           = serializedObject.FindProperty("onlyFocusMode");

        actionsWhenPuzzleIsSolved myScript = (actionsWhenPuzzleIsSolved)target; 

        editorMethods = new EditorMethods();
        methodsList = serializedObject.FindProperty("methodsList");
        listOfEvent = serializedObject.FindProperty("listOfEvent");

        a_puzzleSolved = serializedObject.FindProperty("a_puzzleSolved");
        a_puzzleSolvedVolume = serializedObject.FindProperty("a_puzzleSolvedVolume");

        objectActivatedWhenPuzzleIsSolved = serializedObject.FindProperty("objectActivatedWhenPuzzleIsSolved");


        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .6f));
            Tex_04 = MakeTex(2, 2, new Color(1, .3f, 1, .3f));
            Tex_05 = MakeTex(2, 2, new Color(.3F, .9f, 1, .3f));
        }

       
	}


	public override void OnInspectorGUI()
	{
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		serializedObject.Update ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See Inspector :", GUILayout.Width (85));
		EditorGUILayout.PropertyField(SeeInspector, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();

		GUIStyle style_Yellow_01 		= new GUIStyle();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle();	style_Yellow_Strong.normal.background 	= Tex_02;

        actionsWhenPuzzleIsSolved myScript = (actionsWhenPuzzleIsSolved)target;

        if (!onlyFocusMode.boolValue)
        {

            EditorGUILayout.BeginVertical(style_Orange);

            //--> Actions after the puzzle is solved
            EditorGUILayout.LabelField("Actions after the puzzle is solved.", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("");



            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Object activated when puzzle is solved : ", GUILayout.Width(220));
            EditorGUILayout.PropertyField(objectActivatedWhenPuzzleIsSolved, new GUIContent(""), GUILayout.Width(130));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Play Audio : ", GUILayout.Width(70));
            EditorGUILayout.PropertyField(a_puzzleSolved, new GUIContent(""), GUILayout.Width(130));
            GUILayout.Label("Volume : ", GUILayout.Width(60));
            a_puzzleSolvedVolume.floatValue = EditorGUILayout.Slider(a_puzzleSolvedVolume.floatValue, 0, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            for (var i = 0; i < listOfEvent.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(style_Blue);
                EditorGUILayout.BeginVertical(style_Blue);
                EditorGUILayout.BeginHorizontal();

                if (listOfEvent.arraySize > 1)
                {
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        removeEntry(i, myScript);
                        break;
                    }
                }

                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    addEntry(i, myScript);
                    break;
                }

                EditorGUILayout.LabelField("Step " + i + " : ",EditorStyles.boldLabel);
               


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();

                //-> Step duration
                //EditorGUILayout.LabelField("", GUILayout.Width(15));
                EditorGUILayout.LabelField("Step duration : ", GUILayout.Width(90));
                EditorGUILayout.PropertyField(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("duration"), new GUIContent(""), GUILayout.Width(20));

                EditorGUILayout.EndHorizontal();

                //-> Use a feedback Camera
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Use a feedback Camera :", GUILayout.Width(160));

                EditorGUILayout.PropertyField(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("feedbackCamera"), new GUIContent(""), GUILayout.Width(100));

                if (listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("feedbackCamera").objectReferenceValue != null)
                {
                    string Switch = "Switch Off";

                    if (myScript.listOfEvent[i].feedbackCamera != null
                       && !myScript.listOfEvent[i].feedbackCamera.activeSelf)
                        Switch = "Switch On";

                    if (GUILayout.Button(Switch, GUILayout.Width(70)))
                    {
                        Undo.RegisterFullObjectHierarchyUndo(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("feedbackCamera").objectReferenceValue,
                                                             listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("feedbackCamera").objectReferenceValue.name);
                        if (Switch == "Switch On")
                        {
                            myScript.listOfEvent[i].feedbackCamera.SetActive(true);
                            Selection.activeGameObject = myScript.listOfEvent[i].feedbackCamera;
                        }
                        else
                        {
                            myScript.listOfEvent[i].feedbackCamera.SetActive(false);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                //-> Open a door or drawer
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Open a Door or a Drawer : ", GUILayout.Width(160));
                EditorGUILayout.PropertyField(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("objTranslationOrRotation"), new GUIContent(""), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                //-> Popup Object
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Object Popup : ", GUILayout.Width(160));
                EditorGUILayout.PropertyField(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("objChangeScale"), new GUIContent(""), GUILayout.Width(100));

                if (myScript.listOfEvent[i].objChangeScale != null)
                    EditorGUILayout.PropertyField(listOfEvent.GetArrayElementAtIndex(i).FindPropertyRelative("animCurve"), new GUIContent(""));

                EditorGUILayout.EndHorizontal();
                //-> Call custom Methods
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("Custom Method :", GUILayout.Width(100));


                if (GUILayout.Button("Clear", GUILayout.Width(40)))
                {
                    clearSlectedCustomMethods(i, myScript, true);
                }

                EditorGUILayout.LabelField("", GUILayout.Width(12));

                EditorGUILayout.PropertyField(methodsList.GetArrayElementAtIndex(i).FindPropertyRelative("obj"), new GUIContent(""), GUILayout.Width(100));

                if (EditorGUI.EndChangeCheck())
                {
                    //Debug.Log("Here");
                    clearSlectedCustomMethods(i, myScript, false);
                }

                showCustomMethods(style_Purple, style_Blue, i);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

        }

		serializedObject.ApplyModifiedProperties ();
		EditorGUILayout.LabelField ("");
	}

//--> Add new entry
    private void addEntry(int i,actionsWhenPuzzleIsSolved myScript){
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        myScript.listOfEvent.Insert(i+1,new actionsWhenPuzzleIsSolved.ListOfEvent());
        myScript.methodsList.Add(new EditorMethodsList.MethodsList());
    }

//--> Remove an entry
    private void removeEntry(int i,actionsWhenPuzzleIsSolved myScript)
    {
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        myScript.listOfEvent.RemoveAt(i);
        myScript.methodsList.RemoveAt(i);
    }

//--> Display on screen custom methods
    private void showCustomMethods(GUIStyle style_Purple,GUIStyle style_Blue,int i){
        actionsWhenPuzzleIsSolved myScript = (actionsWhenPuzzleIsSolved)target; 
        //-> Custom methods
        editorMethods.DisplaySelectedMethodsOnEditor(myScript.methodsList, methodsList, style_Blue,i);
    }


    private void clearSlectedCustomMethods(int value, actionsWhenPuzzleIsSolved myScript,bool b_Clear){

        SerializedObject serializedObject2 = new UnityEditor.SerializedObject(myScript);

        SerializedProperty obj = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("obj");
        SerializedProperty scriptRef = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("scriptRef");
        SerializedProperty indexScript = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("indexScript");
        SerializedProperty indexMethod = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("indexMethod");
        SerializedProperty methodInfoName = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("methodInfoName");
        SerializedProperty intValue = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("intValue");
        SerializedProperty floatValue = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("floatValue");
        SerializedProperty stringValue = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("stringValue");
        SerializedProperty objValue = serializedObject2.FindProperty("methodsList").GetArrayElementAtIndex(value).FindPropertyRelative("objValue");



        serializedObject2.Update();
        if (b_Clear)
            obj.objectReferenceValue = null;

        scriptRef.objectReferenceValue = null;
        indexScript.intValue = 0;
        indexMethod.intValue = 0;
        methodInfoName.stringValue = "";
        intValue.intValue = 0;
        floatValue.floatValue = 0;
        stringValue.stringValue = "";
        objValue.objectReferenceValue = null;

        serializedObject2.ApplyModifiedProperties();
    }


	void OnSceneGUI( )
	{
	}
}
#endif