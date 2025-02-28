//Description : JoystickReticuleEditor : JoystickReticule custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(JoystickReticule))]
public class JoystickReticuleEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			sensibilityJoystick;
	SerializedProperty 			animationCurveJoystick;

	SerializedProperty 			horizontalJoystick;
	SerializedProperty 			verticaJloystick;



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

	private Texture2D 		Tex_01;
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 			= serializedObject.FindProperty ("SeeInspector");

		sensibilityJoystick		= serializedObject.FindProperty ("sensibilityJoystick");
		animationCurveJoystick	= serializedObject.FindProperty ("animationCurveJoystick");

		horizontalJoystick		= serializedObject.FindProperty ("horizontalJoystick");
		verticaJloystick			= serializedObject.FindProperty ("verticaJloystick");

		JoystickReticule myScript = (JoystickReticule)target; 

		GameObject tmp = GameObject.Find ("InputsManager");
		if(tmp){
			objCanvasInput = tmp;
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[0].buttonsList.Count;i++){
				s_inputListJoystickAxis.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [0].buttonsList [i].name);
			}
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList.Count;i++){
				s_inputListJoystickButton.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [1].buttonsList [i].name);
			}



			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[2].buttonsList.Count;i++){
				s_inputListKeyboardAxis.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [2].buttonsList [i].name);
			}
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[3].buttonsList.Count;i++){
				s_inputListKeyboardButton.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [3].buttonsList [i].name);
			}

		}

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



		JoystickReticule myScript = (JoystickReticule)target; 

		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.HelpBox ("This script allow to setup the fake Joystick cursor behaviour", MessageType.Info);

//--> Joystick sensibility
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Joystick sensibility : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField(sensibilityJoystick, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();

//--> Animation Curve
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Animation Curve : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField(animationCurveJoystick, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();

//--> Joystick Input to move fake cursor Horizontaly
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Horizontal : ", GUILayout.Width (120));
			horizontalJoystick.intValue = EditorGUILayout.Popup (horizontalJoystick.intValue, s_inputListJoystickAxis.ToArray ());
		EditorGUILayout.EndHorizontal ();

//--> Joystick Input to move fake cursor verticaly
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Vertical : ", GUILayout.Width (120));
			verticaJloystick.intValue = EditorGUILayout.Popup (verticaJloystick.intValue, s_inputListJoystickAxis.ToArray ());
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();

		serializedObject.ApplyModifiedProperties ();

		EditorGUILayout.LabelField ("");
	}



	void OnSceneGUI( )
	{
	}
}
#endif