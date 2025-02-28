//Description : MM_MenuInputsEditor : custom editor for MM_MenuInputs
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;


[CustomEditor(typeof(MM_MenuInputs))]
public class MM_MenuInputsEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			remapButtons;
	SerializedProperty 			b_PC;
	SerializedProperty 			boolValues;
	SerializedProperty 			floatValues;

	public 	EditorMethods		editorMethods;

	public List<string> s_inputListJoystickAxis = new List<string> ();
	public List<string> s_inputListJoystickButton = new List<string> ();
	public List<string> s_inputListKeyboardAxis = new List<string> ();
	public List<string> s_inputListKeyboardButton = new List<string> ();


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

		remapButtons	= serializedObject.FindProperty ("remapButtons");
		b_PC			= serializedObject.FindProperty ("b_PC");
		boolValues		= serializedObject.FindProperty ("boolValues");
		floatValues 	= serializedObject.FindProperty ("floatValues");

		editorMethods 		= new EditorMethods ();


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
		MM_MenuInputs myScript = (MM_MenuInputs)target; 

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


		GUILayout.Label("");


		//--> Default Input Choice
		EditorGUILayout.BeginVertical (style_Yellow_01);
		if (b_PC.boolValue) {
			if (GUILayout.Button ("Defaut inputs are setup for PC")) {
				b_PC.boolValue = false;
			}
		} else {
			if (GUILayout.Button ("Defaut inputs are setup for Mac")) {
				b_PC.boolValue = true;
			}
		}
		EditorGUILayout.EndVertical ();

		GUILayout.Label("");

	//--> Joystick Axis
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Joystick Axis : ", GUILayout.Width (150));
			GUILayout.Label("Default PC Axis : ", GUILayout.Width (110));
			GUILayout.Label("Default Mac Axis : ", GUILayout.Width (110));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < remapButtons.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (0).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (0).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeInput (0,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Input : Axis", GUILayout.Width (380))) {
			addNewInputAxis(0,myScript);
		}

		EditorGUILayout.EndVertical ();


	//--> Joystick Buttons
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginHorizontal ();
       
		GUILayout.Label("Joystick Buttons : ", GUILayout.Width (150));
		GUILayout.Label("Default PC Buttons : ", GUILayout.Width (110));
		GUILayout.Label("Default Mac Buttons : ", GUILayout.Width (110));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < remapButtons.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (1).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (1).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeInput (1,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Input : Button", GUILayout.Width (380))) {
			addNewInputButton (1,myScript);
		}
		EditorGUILayout.EndVertical ();

		GUILayout.Label("");

	//--> Keyboard Axis
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Keyboard Axis : ", GUILayout.Width (150));
		GUILayout.Label("Default PC Axis : ", GUILayout.Width (110));
		GUILayout.Label("Default Mac Axis : ", GUILayout.Width (110));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < remapButtons.GetArrayElementAtIndex (2).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (2).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (2).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (2).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeInput (2,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Input : Axis", GUILayout.Width (380))) {
			addNewInputAxis(2,myScript);
		}
		EditorGUILayout.EndVertical ();


	//--> Keyboard Buttons
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Keyboard Buttons : ", GUILayout.Width (150));
		GUILayout.Label("Default PC Buttons : ", GUILayout.Width (110));
		GUILayout.Label("Default Mac Buttons : ", GUILayout.Width (110));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < remapButtons.GetArrayElementAtIndex (3).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (3).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (3).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			EditorGUILayout.PropertyField (remapButtons.GetArrayElementAtIndex (3).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (110));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeInput (3,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Input : Button", GUILayout.Width (380))) {
			addNewInputButton (3,myScript);
		}
		EditorGUILayout.EndVertical ();

		GUILayout.Label("");

	//--> bool values Joystick
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Toggles value Joystick : ", GUILayout.Width (150));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < boolValues.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (boolValues.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (boolValues.GetArrayElementAtIndex (0).FindPropertyRelative ("b_Values").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (20));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeBoolean (0,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Boolean", GUILayout.Width (380))) {
			addNewBoolean(0,myScript);
		}
		EditorGUILayout.EndVertical ();


		//--> float values Keyboard
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginVertical (style_Orange);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Toggles value Keyboard : ", GUILayout.Width (150));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < boolValues.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (boolValues.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (boolValues.GetArrayElementAtIndex (1).FindPropertyRelative ("b_Values").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (20));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeBoolean (1,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Boolean", GUILayout.Width (380))) {
			addNewBoolean(1,myScript);
		}
		EditorGUILayout.EndVertical ();

		GUILayout.Label("");

		//--> float values Joystick
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Sliders value Joystick : ", GUILayout.Width (150));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < floatValues.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (floatValues.GetArrayElementAtIndex (0).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (floatValues.GetArrayElementAtIndex (0).FindPropertyRelative ("b_Values").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (40));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeFloat (0,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Float", GUILayout.Width (380))) {
			addNewFloat(0,myScript);
		}
		EditorGUILayout.EndVertical ();

		//--> bool values Keyboard
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Slider value Keyboard : ", GUILayout.Width (150));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
		for (var i = 0; i < floatValues.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
            GUILayout.Label(i + " : ", GUILayout.Width(20));
			EditorGUILayout.PropertyField (floatValues.GetArrayElementAtIndex (1).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (150));
			EditorGUILayout.PropertyField (floatValues.GetArrayElementAtIndex (1).FindPropertyRelative ("b_Values").GetArrayElementAtIndex (i), new GUIContent (""), GUILayout.Width (40));
			if (GUILayout.Button ("-", GUILayout.Width (20))) {
				removeFloat (1,i,myScript);
				break;
			}
			EditorGUILayout.EndHorizontal ();

		}
		if (GUILayout.Button ("Add New Float", GUILayout.Width (380))) {
			addNewFloat(1,myScript);
		}
		EditorGUILayout.EndVertical ();

		serializedObject.ApplyModifiedProperties ();



	}


	public void addNewBoolean(int value,MM_MenuInputs myScript){

		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "AddBool");
		if (boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize > 0) {
			boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize - 1);
			boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").InsertArrayElementAtIndex (boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").arraySize - 1);
		} else {
			boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (0);
			boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").InsertArrayElementAtIndex (0);
		}
		
        GameObject newButton = null;
        if(value == 0)
            newButton = Instantiate(myScript.J_ref_Toggle, myScript.J_ref_Toggle.gameObject.transform.parent);
        else
            newButton = Instantiate(myScript.K_ref_Toggle, myScript.K_ref_Toggle.gameObject.transform.parent); 


        newButton.name = "NewToggle";
        Undo.RegisterCreatedObjectUndo(newButton, newButton.name);
        newButton.SetActive(true);

        boolValues.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").GetArrayElementAtIndex(boolValues.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").arraySize - 1).objectReferenceValue
                    = newButton.transform.GetChild(1).gameObject;

        newButton.transform.SetSiblingIndex(2);
	}

	public void removeBoolean(int value,int number,MM_MenuInputs myScript){
		//MM_MenuInputs myScript = (MM_MenuInputs)target; 

		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "removeBool");

		if(boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex(number).objectReferenceValue != null)
			boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		boolValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").DeleteArrayElementAtIndex (number);
	}

	public void addNewFloat(int value,MM_MenuInputs myScript){
		//MM_MenuInputs myScript = (MM_MenuInputs)target; 

		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "AddBool");
		if (floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize > 0) {
			floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize - 1);
			floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").InsertArrayElementAtIndex (floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").arraySize - 1);
		} else {
			floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (0);
			floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").InsertArrayElementAtIndex (0);
		}
		

        GameObject newButton = null;

        if(value == 0)
            newButton = Instantiate(myScript.J_ref_Slider, myScript.J_ref_Slider.gameObject.transform.parent);
        else
            newButton = Instantiate(myScript.K_ref_Slider, myScript.K_ref_Slider.gameObject.transform.parent);

        newButton.name = "NewSlider";
        Undo.RegisterCreatedObjectUndo(newButton, newButton.name);
        newButton.SetActive(true);

        floatValues.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").GetArrayElementAtIndex(floatValues.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").arraySize - 1).objectReferenceValue
                    = newButton.transform.GetChild(0).gameObject;


        newButton.transform.SetSiblingIndex(2);
	}

	public void removeFloat(int value,int number,MM_MenuInputs myScript){
		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "removeBool");

		if(floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex(number).objectReferenceValue != null)
			floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		floatValues.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Values").DeleteArrayElementAtIndex (number);
	}


	public void addNewInputAxis(int value,MM_MenuInputs myScript){
		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "AddInputAxis");

		if (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize > 0) {
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").arraySize - 1);
		}
		else {
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").InsertArrayElementAtIndex (0);
		}

   		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").arraySize - 1).stringValue = "_";
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").arraySize - 1).boolValue = false;
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").arraySize - 1).boolValue = true;
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").arraySize - 1).stringValue = "_";
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").arraySize - 1).stringValue = "_";

        GameObject newButton = null;


        if (value == 0)
                newButton = Instantiate(myScript.J_ref_Button, myScript.J_ref_Button.gameObject.transform.parent);
        else if(value == 2)
                newButton = Instantiate(myScript.K_ref_Button, myScript.K_ref_Button.gameObject.transform.parent);


        newButton.name = "NewAxis";
        Undo.RegisterCreatedObjectUndo(newButton, newButton.name);
        newButton.SetActive(true);


        remapButtons.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").GetArrayElementAtIndex(remapButtons.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").arraySize - 1).objectReferenceValue
                    = newButton.transform.GetChild(2).gameObject;

        newButton.transform.SetSiblingIndex(2);
	}

	public void addNewInputButton(int value,MM_MenuInputs myScript){
		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "addInputButton");

		if (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize > 0) {
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").arraySize - 1);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").InsertArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").arraySize - 1);
		}
		else {
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").InsertArrayElementAtIndex (0);
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").InsertArrayElementAtIndex (0);
		}

		
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").arraySize - 1).stringValue = "_";
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").arraySize - 1).boolValue = false;
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").arraySize - 1).boolValue = false;
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").arraySize - 1).stringValue = "_";
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").GetArrayElementAtIndex (remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").arraySize - 1).stringValue = "_";

        GameObject newButton = null;

        if (value == 1)
            newButton = Instantiate(myScript.J_ref_Button, myScript.J_ref_Button.gameObject.transform.parent);
        else if (value == 3)
            newButton = Instantiate(myScript.K_ref_Button, myScript.K_ref_Button.gameObject.transform.parent);

        newButton.name = "NewButton";
        Undo.RegisterCreatedObjectUndo(newButton,newButton.name);
        newButton.SetActive(true);

        remapButtons.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").GetArrayElementAtIndex(remapButtons.GetArrayElementAtIndex(value).FindPropertyRelative("buttonsList").arraySize - 1).objectReferenceValue 
                    = newButton.transform.GetChild(2).gameObject;

        newButton.transform.SetSiblingIndex(2);

	}

	public void removeInput(int value,int number,MM_MenuInputs myScript){
		Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject, "removeInput");

		if(remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").GetArrayElementAtIndex(number).objectReferenceValue != null)
			remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("buttonsList").DeleteArrayElementAtIndex (number);

		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("inputNameList").DeleteArrayElementAtIndex (number);
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("stateList").DeleteArrayElementAtIndex (number);
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("b_Axis").DeleteArrayElementAtIndex (number);
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNamePC").DeleteArrayElementAtIndex (number);
		remapButtons.GetArrayElementAtIndex (value).FindPropertyRelative ("defaultNameMac").DeleteArrayElementAtIndex (number);
	
	}


	void OnSceneGUI( )
	{
	}
}
#endif