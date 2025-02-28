//Description : infoUIEditor : custom editor for infoUI
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(infoUI))]
public class infoUIEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			animationCurve;
	SerializedProperty 			stayDuration;

	SerializedProperty 			diarySprite;
	SerializedProperty 			inventorySprite;
	SerializedProperty 			feedbackSprite;

	SerializedProperty 			s_Inventory;
	SerializedProperty 			s_InventoryVolume;

	SerializedProperty 			s_Diary;
	SerializedProperty 			s_DiaryVolume;

	SerializedProperty 			s_feedback;
	SerializedProperty 			s_feedbackVolume;

    SerializedProperty          inventoryID;
    SerializedProperty          diaryID;


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
		SeeInspector 		= serializedObject.FindProperty ("SeeInspector");

		animationCurve		= serializedObject.FindProperty ("animationCurve");
		stayDuration		= serializedObject.FindProperty ("stayDuration");

		diarySprite			= serializedObject.FindProperty ("diarySprite");
		inventorySprite		= serializedObject.FindProperty ("inventorySprite");
		feedbackSprite		= serializedObject.FindProperty ("feedbackSprite");

		s_Inventory			= serializedObject.FindProperty ("s_Inventory");
		s_InventoryVolume	= serializedObject.FindProperty ("s_InventoryVolume");

		s_Diary				= serializedObject.FindProperty ("s_Diary");
		s_DiaryVolume		= serializedObject.FindProperty ("s_DiaryVolume");

		s_feedback			= serializedObject.FindProperty ("s_feedback");
		s_feedbackVolume	= serializedObject.FindProperty ("s_feedbackVolume");

        inventoryID = serializedObject.FindProperty("inventoryID");
        diaryID = serializedObject.FindProperty("diaryID");



		infoUI myScript = (infoUI)target; 

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

		Tex_01 = MakeTex(2, 2, new Color(1,.8f,0.2F,.4f)); 
		Tex_02 = MakeTex(2, 2, new Color(1,.8f,0.2F,.4f)); 
		Tex_03 = MakeTex(2, 2, new Color(.3F,.9f,1,.5f));
		Tex_04 = MakeTex(2, 2, new Color(1,.3f,1,.3f)); 
		Tex_05 = MakeTex(2, 2, new Color(1,.5f,0.3F,.4f)); 
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

		GUIStyle style_Yellow_01 		= new GUIStyle(GUI.skin.box);	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle(GUI.skin.box);	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle(GUI.skin.box);	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle(GUI.skin.box);	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle(GUI.skin.box);	style_Yellow_Strong.normal.background 	= Tex_02;



		infoUI myScript = (infoUI)target; 
	


		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.HelpBox ("Info Box : Global Parameters", MessageType.Info);

//--> Animation Curve
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Animation Curve : ", GUILayout.Width (100));
		EditorGUILayout.PropertyField(animationCurve, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();

//-> Duration
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Duration : ", GUILayout.Width (100));
		EditorGUILayout.PropertyField(stayDuration, new GUIContent (""), GUILayout.Width (50));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();

//--> Diary Box
		EditorGUILayout.BeginVertical (style_Yellow_01);
			EditorGUILayout.HelpBox ("Diary Box : ", MessageType.Info);

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sprite : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(diarySprite, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
                GUILayout.Label ("Default ID (w_UI) : ", GUILayout.Width (100));
                EditorGUILayout.PropertyField(diaryID, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sound : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(s_Diary, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Volume : ", GUILayout.Width (100));
				s_DiaryVolume.floatValue =  EditorGUILayout.Slider(s_DiaryVolume.floatValue,0,1);
			EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();

//--> Inventory Box
		EditorGUILayout.BeginVertical (style_Yellow_01);
			EditorGUILayout.HelpBox ("Inventory Box : ", MessageType.Info);
	
			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sprite : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(inventorySprite, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
                GUILayout.Label ("Default ID (w_UI) : ", GUILayout.Width (100));
                EditorGUILayout.PropertyField(inventoryID, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sound : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(s_Inventory, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Volume : ", GUILayout.Width (100));
				s_InventoryVolume.floatValue =  EditorGUILayout.Slider(s_InventoryVolume.floatValue,0,1);
			EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();


//--> Feedback Box
		EditorGUILayout.BeginVertical (style_Yellow_01);

			EditorGUILayout.HelpBox ("Feedback Box : ", MessageType.Info);

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sprite : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(feedbackSprite, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Default Sound : ", GUILayout.Width (100));
				EditorGUILayout.PropertyField(s_feedback, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Volume : ", GUILayout.Width (100));
				s_feedbackVolume.floatValue =  EditorGUILayout.Slider(s_feedbackVolume.floatValue,0,1);
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