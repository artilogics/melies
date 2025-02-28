//Description : VoicePropertiesEditor : VoiceProperties custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(VoiceProperties))]
public class VoicePropertiesEditor : Editor {
	SerializedProperty			SeeInspector;						// use to draw default Inspector
	SerializedProperty			managerID;							// 
	SerializedProperty 			uniqueID;							// 
	SerializedProperty 			_textList;							// 
	SerializedProperty 			b_PlayOnce;

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
	SerializedObject 		serializedObject2;
    SerializedObject        serializedObject3;

	SerializedProperty 		_Title;

	void OnEnable () {

	//-> Load the needed TextList 
		VoiceProperties myScript = (VoiceProperties)target; 
		string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as datasProjectManager;

        string s_dataType = "wTextnVoices.asset";
        if (myScript.editorType == 1) s_dataType = "wItem.asset";
        if (myScript.editorType == 2) s_dataType = "wUI.asset";
        if (myScript.editorType == 3) s_dataType = "wFeedback.asset";

		string objectPath2 = "Assets/AP/Assets/Resources/" + _ProjectManagerDatas.currentDatasProjectFolder + "/TextList/" + s_dataType;

		TextList tmpTextList = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as TextList;
		myScript.textList = tmpTextList;

	//-> Setup the SerializedProperties.
		SeeInspector 	= serializedObject.FindProperty ("SeeInspector");
		managerID 		= serializedObject.FindProperty ("managerID");
		uniqueID		= serializedObject.FindProperty ("uniqueID");
		_textList		= serializedObject.FindProperty ("textList");
		b_PlayOnce      = serializedObject.FindProperty ("b_PlayOnce");

		Find_UniqueId_In_The_TextList ();

		Tex_01 = MakeTex(2, 2, new Color(.3f,.8f,0.2F,.8f)); 
	}


	public override void OnInspectorGUI()
	{
		GUIStyle style_Yellow_01 		= new GUIStyle(GUI.skin.box);	style_Yellow_01.normal.background 		= Tex_01; 

		VoiceProperties myScript = (VoiceProperties)target; 
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

			serializedObject.Update ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("See Inspector :", GUILayout.Width (85));
			EditorGUILayout.PropertyField (SeeInspector, new GUIContent (""), GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();


		EditorGUILayout.BeginVertical (style_Yellow_01);
		EditorGUILayout.HelpBox ("This script is used to play voice over and Subtitles when this object is selected in game.", MessageType.Info);
		
        buttonOpenWindowToEditEntry(myScript);


        EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Manager :", GUILayout.Width (85));
		EditorGUILayout.PropertyField (_textList, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();

		if (_textList.objectReferenceValue != null) {




			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Manager ID :", GUILayout.Width (85));
			EditorGUILayout.PropertyField (managerID, new GUIContent (""), GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();
			if (EditorGUI.EndChangeCheck ()) {
				//Debug.Log ("here");
				updateInventory (managerID.intValue);
			}


			serializedObject2 = new UnityEditor.SerializedObject (_textList.objectReferenceValue);
			if (managerID.intValue < serializedObject2.FindProperty ("diaryList").GetArrayElementAtIndex (0).FindPropertyRelative ("_languageSlot").arraySize
				&& managerID.intValue >= 0) {

			_Title = serializedObject2.FindProperty ("diaryList").GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(managerID.intValue).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex(0);
			//if (myScript.textList.diaryList.Count > 0) {
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Item Title :", GUILayout.Width (85));
				EditorGUILayout.LabelField (_Title.stringValue.ToString());
			EditorGUILayout.EndHorizontal ();
			//}
			} else {
				EditorGUILayout.HelpBox ("This ID doesn't exist", MessageType.Warning);

			}


			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Play sound Only the first Time", GUILayout.Width (200));
			EditorGUILayout.PropertyField (b_PlayOnce, new GUIContent (""), GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();


		}
		EditorGUILayout.EndVertical ();	

			EditorGUILayout.LabelField ("");

			serializedObject.ApplyModifiedProperties ();
		
	}

// --> Update the uniqueID when the managerID is change in the Inspector
	private void updateInventory(int value){																
		VoiceProperties myScript = (VoiceProperties)target; 
		int HowManyEntry = myScript.textList.diaryList [0]._languageSlot.Count;
		for (var i = 0; i < HowManyEntry; i++) {
			//if (myScript.textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.uniqueID) {
			if (i == value) {
				Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
				//Debug.Log ("Position : " + i);
				uniqueID.intValue = myScript.textList.diaryList [0]._languageSlot [i].uniqueItemID;

				if (myScript.gameObject.GetComponent<Text>()) {
					myScript.gameObject.GetComponent<Text>().text = myScript.textList.diaryList [0]._languageSlot [i].diaryTitle[0];
				}
				break;
			}
		}
	}


	public void Find_UniqueId_In_The_TextList(){
		VoiceProperties myScript = (VoiceProperties)target; 

		if (_textList.objectReferenceValue != null) {

			int HowManyEntry = myScript.textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {
				if (myScript.textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.uniqueID) {
					//if (i == myScript.managerID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					//Debug.Log ("Position : " + i);
					myScript.managerID = i;
					break;
				}
			}
		}
	}

    private void ShowWindow(VoiceProperties myScript)
    {
        //Select ID
        serializedObject3 = new UnityEditor.SerializedObject(_textList.objectReferenceValue);

        SerializedProperty selectedID = serializedObject3.FindProperty("selectedID");

        serializedObject3.Update();
        selectedID.intValue = managerID.intValue;
        serializedObject3.ApplyModifiedProperties();



        //Show existing window instance. If one doesn't exist, make one.
        if (myScript.editorType == 0) EditorWindow.GetWindow(typeof(w_TextnVoice));
        if (myScript.editorType == 1) EditorWindow.GetWindow(typeof(w_Item));
        if (myScript.editorType == 2) EditorWindow.GetWindow(typeof(w_UI));
        if (myScript.editorType == 3) EditorWindow.GetWindow(typeof(w_Feedback));

    }

    private void buttonOpenWindowToEditEntry(VoiceProperties myScript)
    {
        
        string s_EditWindow = "Open Window Tab : ";
        //Show existing window instance. If one doesn't exist, make one.
        if (myScript.editorType == 0) s_EditWindow += "w_textnVoice";
        if (myScript.editorType == 1) s_EditWindow += "w_Item";
        if (myScript.editorType == 2) s_EditWindow += "w_UI";
        if (myScript.editorType == 3) s_EditWindow += "w_Feedback";

        s_EditWindow += " to edit the selected entry.";

        if (GUILayout.Button(s_EditWindow))
        {
            ShowWindow(myScript);
        }

    }


	void OnSceneGUI( )
	{
	}
}
#endif