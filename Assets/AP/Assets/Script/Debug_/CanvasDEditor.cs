//Description : CanvasDEditor : CanvasD custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(CanvasD))]
public class CanvasDEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
    SerializedProperty obj;
    SerializedProperty puzzle;
    SerializedProperty txtFeedback;
    SerializedProperty b_MobileButtons;


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

        obj = serializedObject.FindProperty("obj");
        puzzle = serializedObject.FindProperty("puzzle");
        txtFeedback = serializedObject.FindProperty("txtFeedback");
        b_MobileButtons = serializedObject.FindProperty("b_MobileButtons");


        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .3f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
            Tex_04 = MakeTex(2, 2, new Color(1, .3f, 1, .3f));
            Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
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


		GUILayout.Label("");
		CanvasD myScript = (CanvasD)target;

        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Everything is unlocked :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(obj, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Puzzle Automatically Solved :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(puzzle, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Debug Text :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(txtFeedback, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("UI Debug Button :", GUILayout.Width(160));

        string newString = "Activated";
        if (b_MobileButtons.boolValue)
            newString = "Deactivated";

        if (GUILayout.Button(newString))
        {
            Undo.RegisterFullObjectHierarchyUndo(myScript.btn_Puzzle, myScript.btn_Puzzle.name);
            Undo.RegisterFullObjectHierarchyUndo(myScript.btn_Objects, myScript.btn_Objects.name);

            if (!b_MobileButtons.boolValue){
                myScript.btn_Puzzle.SetActive(false);
                myScript.btn_Objects.SetActive(false);
                b_MobileButtons.boolValue = true;
            }else
            {
                myScript.btn_Puzzle.SetActive(true);
                myScript.btn_Objects.SetActive(true);
                b_MobileButtons.boolValue = false;
            }

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField ("");

		serializedObject.ApplyModifiedProperties ();
	}



	void OnSceneGUI( )
	{
	}
}
#endif