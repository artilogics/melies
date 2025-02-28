//Description : ingameGlobalManagerEditor : ingameGlobalManager custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(ingameGlobalManager))]
public class ingameGlobalManagerEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			backButtonDesktop;
	SerializedProperty 			backButtonJoystick;
	SerializedProperty 			pauseButtonDesktop;


	public List<string> s_inputListJoystickAxis = new List<string> ();
	public List<string> s_inputListJoystickButton = new List<string> ();
	public List<string> s_inputListKeyboardAxis = new List<string> ();
	public List<string> s_inputListKeyboardButton = new List<string> ();


	public GameObject objCanvasInput;

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
		

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 		= serializedObject.FindProperty ("SeeInspector");

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

		serializedObject.ApplyModifiedProperties ();
	}




	void OnSceneGUI( )
	{
	}
}
#endif