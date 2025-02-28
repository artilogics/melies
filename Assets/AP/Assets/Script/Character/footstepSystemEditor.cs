//Description : footstepSystemEditor. Work in association with footstepSystem. Allow to manage player foostep sound depending the surface
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(footstepSystem))]
public class footstepSystemEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			listFootstepSystem;										// serialized property
	SerializedProperty 			listCompareMangnitude;
	SerializedProperty aFall;

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

	private Texture2D 		Tex_01;														// 
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	public string selectedTag = "";

	public string newTagName = "";

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 				= serializedObject.FindProperty ("SeeInspector");

		listFootstepSystem			= serializedObject.FindProperty ("listFootstepSystem");
		listCompareMangnitude		= serializedObject.FindProperty ("listCompareMangnitude");
		aFall = serializedObject.FindProperty("aFall");
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
		footstepSystem myScript = (footstepSystem)target; 

		for (var i = 0; i < listCompareMangnitude.arraySize; i++) {
			EditorGUILayout.BeginVertical (style_Yellow_01);
//--> Choose the time between two steps
			EditorGUILayout.HelpBox ("Choose the time between two steps", MessageType.Info);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Time between two steps : ", GUILayout.Width (140));
			EditorGUILayout.PropertyField(listCompareMangnitude.GetArrayElementAtIndex(i).FindPropertyRelative("listTimeBetweenTwoStep"), new GUIContent (""), GUILayout.Width (40));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
		}

		GUILayout.Label("");

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fall Sound: ", GUILayout.Width(140));
		EditorGUILayout.PropertyField(aFall, new GUIContent(""));
		EditorGUILayout.EndHorizontal();

		GUILayout.Label("");

		

		for (var i = 0; i < listFootstepSystem.arraySize; i++) {
//--> Setup sound for each surface
			SerializedProperty footstepType = listFootstepSystem.GetArrayElementAtIndex (i);
			SerializedProperty refTag = listFootstepSystem.GetArrayElementAtIndex (i).FindPropertyRelative ("MaterialTag");


			EditorGUILayout.BeginVertical (style_Orange);
				EditorGUILayout.HelpBox ("When character is on surface with the Tag "
					+ "''" + refTag.stringValue + "''" + 
					" next footsteps are played.", MessageType.Info);
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Surface : ", GUILayout.Width (65));
					EditorGUILayout.LabelField ("");
					Rect r = GUILayoutUtility.GetLastRect();
					refTag.stringValue = EditorGUI.TagField(new Rect(r.x,r.y,110,18),new GUIContent (""),refTag.stringValue);
				EditorGUILayout.EndHorizontal ();

				for (var j = 0; j < footstepType.FindPropertyRelative("footstepSamples").arraySize; j++) {
					EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.PropertyField(footstepType.FindPropertyRelative("footstepSamples").GetArrayElementAtIndex(j)
						, new GUIContent (""), GUILayout.Width (200));


//--> Add/Remove footstep
						if (GUILayout.Button ("+", GUILayout.Width (20))) {
							AddSound (myScript,i, j);
						}
						if (footstepType.FindPropertyRelative("footstepSamples").arraySize > 1) {
							if (GUILayout.Button ("-", GUILayout.Width (20))) {
								RemoveSound (myScript,i, j);
							}
						}
					EditorGUILayout.EndHorizontal ();
				}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.LabelField ("");
		}
			

//--> Create a new surface
		EditorGUILayout.BeginVertical (style_Blue);
			EditorGUILayout.HelpBox ("This section allow to create a new footstep surface." +
			"\n1-Choose a name for the new surface" +
			"\n2-Press Create", MessageType.Info);
			
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Choose a name for the new surface : ", GUILayout.Width (200));

				newTagName = EditorGUILayout.TextField(newTagName, GUILayout.Width (85));
				if (GUILayout.Button ("Create", GUILayout.Width (85))) {
					CreateNewSurface (newTagName);
				}
			EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
	
		EditorGUILayout.LabelField ("");
		EditorGUILayout.LabelField ("");

		serializedObject.ApplyModifiedProperties ();
	}

//--> ADd new sound on a specific surface
	private void AddSound(footstepSystem myScript,int i,int j){
		Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		listFootstepSystem.GetArrayElementAtIndex (i).FindPropertyRelative ("footstepSamples").InsertArrayElementAtIndex (j);
		listFootstepSystem.GetArrayElementAtIndex (i).FindPropertyRelative ("footstepSamples").GetArrayElementAtIndex (j+1).objectReferenceValue = null;
	}

//--> Remove a specific sound on a specific surface
	private void RemoveSound(footstepSystem myScript,int i,int j){
		Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
		myScript.listFootstepSystem[i].footstepSamples.RemoveAt(j);

	}


//--> Create a new surface
	private void CreateNewSurface(string newTagName){
		UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
		if ((asset != null) && (asset.Length > 0))
		{
			SerializedObject so = new SerializedObject(asset[0]);
			SerializedProperty tags = so.FindProperty("tags");

			for (int i = 0; i < tags.arraySize; ++i)
			{
				//-> Tag already exist
				if (tags.GetArrayElementAtIndex(i).stringValue == newTagName)
					return;    
			}

			// Create a new tag
			tags.InsertArrayElementAtIndex(tags.arraySize-1);
			tags.GetArrayElementAtIndex(tags.arraySize-1).stringValue = newTagName;
			so.ApplyModifiedProperties();
			so.Update();


			//-> init Array
			listFootstepSystem.InsertArrayElementAtIndex (listFootstepSystem.arraySize - 1);
			listFootstepSystem.GetArrayElementAtIndex (listFootstepSystem.arraySize - 1).FindPropertyRelative ("footstepSamples").ClearArray ();
			listFootstepSystem.GetArrayElementAtIndex (listFootstepSystem.arraySize - 1).FindPropertyRelative ("footstepSamples").InsertArrayElementAtIndex (0);
			listFootstepSystem.GetArrayElementAtIndex (listFootstepSystem.arraySize - 1).FindPropertyRelative ("MaterialTag").stringValue = newTagName;

		}




	}


	void OnSceneGUI( )
	{
	}
}
#endif