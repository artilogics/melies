// Description : Menu_ManagerEditor.cs : Use in association with Menu_Manager.cs . Allow to create Menu Page. 
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Menu_Manager))]
public class Menu_ManagerEditor : Editor {
	public bool 			SeeInspector = false;							// use to draw default Inspector
	public bool 			b_CreatePage = false;							// use when a page is created
	public bool 			b_DeletePage = false;							// use when a page is deleted
	SerializedProperty 		List_GroupCanvas;								// Access property from MainMenu.cs script
	SerializedProperty 		b_DesktopOrMobile;
	SerializedProperty 		list_gameObjectByPage;
	SerializedProperty 		b_MoreOptions;
	SerializedProperty 		CurrentPage;

	private Texture2D MakeTex(int width, int height, Color col) {			// use to change the GUIStyle
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	//private Texture2D Tex_01;
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;

	private int 			whichCanvasToDelete = 0;

	void OnEnable () {
		// Setup the SerializedProperties.
		List_GroupCanvas 			= serializedObject.FindProperty ("List_GroupCanvas");
		b_DesktopOrMobile 			= serializedObject.FindProperty ("b_DesktopOrMobile");
		list_gameObjectByPage 		= serializedObject.FindProperty ("list_gameObjectByPage");
		b_MoreOptions 				= serializedObject.FindProperty ("b_MoreOptions");
		CurrentPage					= serializedObject.FindProperty ("CurrentPage");

		if (EditorPrefs.GetBool("AP_ProSkin") == true)
		{
			float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
			Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .2f));
			Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, 1f));
		}
		else
		{
			Tex_02 = MakeTex(2, 2, new Color(1, .92f, .0016f, .2f));
			Tex_03 = MakeTex(2, 2, new Color(1, .92f, .0016f, 1f));
		}
	}

	public override void OnInspectorGUI()
	{
		SeeInspector = EditorGUILayout.Foldout(SeeInspector,"Inspector");

		if(SeeInspector)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		Menu_Manager myScript = (Menu_Manager)target; 

		serializedObject.Update ();
		GUIStyle style_Yellow_Pastel = new GUIStyle();	style_Yellow_Pastel.normal.background = Tex_02;
		GUIStyle style_YellowHard = new GUIStyle();		style_YellowHard.normal.background = Tex_03;

		GUILayout.Label("");

// --> Button to setup Menu for Mobile or Desktop. We don't need the same option for Mobile and desktop
		if (b_DesktopOrMobile.boolValue) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Menu is ready for Mobile : ",GUILayout.Width(160));
			if (GUILayout.Button ("Press button to setup Menu for Desktop")) {
				b_DesktopOrMobile.boolValue = false;
				for (int m = 0; m < List_GroupCanvas.arraySize; m++) {
					for (int i = 0; i < list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize; i++) {


						if (!list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("Desktop").boolValue) {
							if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue) {
								SerializedObject serializedObject3 = new UnityEditor.SerializedObject (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue);
								serializedObject3.Update ();
								SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
								tmpSer2.boolValue = false;
								serializedObject3.ApplyModifiedProperties ();
							}
						} 
						else {
							if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue) {
								SerializedObject serializedObject3 = new UnityEditor.SerializedObject (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue);
								serializedObject3.Update ();
								SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
								tmpSer2.boolValue = true;
								serializedObject3.ApplyModifiedProperties ();
							}
						}
					}
				}
			}	
			EditorGUILayout.EndHorizontal ();
		} else {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Menu is ready for Desktop : ",GUILayout.Width(160));
			if (GUILayout.Button ("Press button to setup Menu for Mobile")) {
				b_DesktopOrMobile.boolValue = true;
				for (int m = 0; m < List_GroupCanvas.arraySize; m++) {
					for (int i = 0; i < list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize; i++) {


						if (!list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("Mobile").boolValue) {
							if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue) {
								SerializedObject serializedObject3 = new UnityEditor.SerializedObject (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue);
								serializedObject3.Update ();
								SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
								tmpSer2.boolValue = false;
								serializedObject3.ApplyModifiedProperties ();
							}
						} 
						else {
							if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue) {
								SerializedObject serializedObject3 = new UnityEditor.SerializedObject (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList").objectReferenceValue);
								serializedObject3.Update ();
								SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
								tmpSer2.boolValue = true;
								serializedObject3.ApplyModifiedProperties ();
							}
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal ();
		}




		GUILayout.Label("");
		GUILayout.Label("");

		EditorGUILayout.BeginVertical();
		for (int m = 0;m< List_GroupCanvas.arraySize;m++) {
			EditorGUILayout.BeginVertical(style_Yellow_Pastel);


				if (myScript.List_GroupCanvas [m].alpha == 1)
					EditorGUILayout.BeginHorizontal (style_YellowHard);
				else
					EditorGUILayout.BeginHorizontal ();


				


// --> Use to activate or deactivate a specific Menu part	
			string b_State = "Off";
			if(myScript.List_GroupCanvas[m].alpha == 1)b_State = "On";
			if(GUILayout.Button(b_State,GUILayout.Width(40)))
			{
				Undo.RegisterCompleteObjectUndo(myScript,"All Property Modif " + myScript.gameObject.name);

				for (int p = 0; p < myScript.List_GroupCanvas.Count; p++) {

					if (p == m) {
						Undo.RegisterCompleteObjectUndo (myScript.List_GroupCanvas [m], "All Property Modif" + List_GroupCanvas.GetArrayElementAtIndex (m).objectReferenceValue.name);

							myScript.List_GroupCanvas [m].alpha = 1;
							myScript.List_GroupCanvas [m].gameObject.SetActive (true);
							CurrentPage.intValue = m;
					} else {
						Undo.RegisterCompleteObjectUndo (myScript.List_GroupCanvas [p], "All Property Modif" + List_GroupCanvas.GetArrayElementAtIndex (p).objectReferenceValue.name);

							myScript.List_GroupCanvas [p].alpha = 0;
							myScript.List_GroupCanvas [p].gameObject.SetActive (false);
					}
				}

			}	

// --> the Canvas (Menu) 
			EditorGUILayout.PropertyField(List_GroupCanvas.GetArrayElementAtIndex(m), new GUIContent (""),GUILayout.Width(160));

			GUILayout.Label("",GUILayout.Width(10));
			if (!b_MoreOptions.GetArrayElementAtIndex (m).boolValue) {
				if (GUILayout.Button ("Options +", GUILayout.Width (70))) {
					b_MoreOptions.GetArrayElementAtIndex (m).boolValue = true;
				} 
			}else {
				if (GUILayout.Button ("Options -", GUILayout.Width (70))) {
					b_MoreOptions.GetArrayElementAtIndex (m).boolValue = false;
				} 
			}

			GUILayout.Label("",GUILayout.Width(20));

// --> Delete a Menu page
			if(GUILayout.Button("Delete page", GUILayout.Width (80)))
			{
				b_DeletePage = true;
				whichCanvasToDelete = m;
				break;
			}	

			GUILayout.Label("");


			EditorGUILayout.EndHorizontal();

// --> Section to add gameobject you want to activate or deactivate if you are on Mobile or on desktop
			if (b_MoreOptions.GetArrayElementAtIndex (m).boolValue) {
				GUILayout.Label ("");
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Add a new gameObject", GUILayout.Width (160))) {
					if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize == 0)							// Check if the array is == to zero
						list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").InsertArrayElementAtIndex (0);
					else {
						list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").InsertArrayElementAtIndex (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize - 1);
					}
					list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize - 1).FindPropertyRelative ("objList").objectReferenceValue = null;
					list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize - 1).FindPropertyRelative ("Desktop").boolValue = true;
					list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize - 1).FindPropertyRelative ("Mobile").boolValue = true;
				}

				EditorGUILayout.EndHorizontal();

				if (list_gameObjectByPage.arraySize > m && list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize > 0) {

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("GameObject activated on : ", GUILayout.Width (150));
					GUILayout.Label ("Desktop", GUILayout.Width (70));
					GUILayout.Label ("Mobile", GUILayout.Width (70));

					EditorGUILayout.EndHorizontal ();

					for (int i = 0; i < list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize; i++) {

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.PropertyField (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("objList"), new GUIContent (""), GUILayout.Width (100));
						GUILayout.Label ("", GUILayout.Width (60));
						EditorGUILayout.PropertyField (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("Desktop"), new GUIContent (""), GUILayout.Width (20));
						GUILayout.Label ("", GUILayout.Width (50));
						EditorGUILayout.PropertyField (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").GetArrayElementAtIndex (i).FindPropertyRelative ("Mobile"), new GUIContent (""), GUILayout.Width (20));
						GUILayout.Label ("", GUILayout.Width (50));
						if (GUILayout.Button ("-", GUILayout.Width (20))) {
							if (list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").arraySize > 0)							// Check if the array is == to zero
								list_gameObjectByPage.GetArrayElementAtIndex (m).FindPropertyRelative ("listOfMenuGameobject").DeleteArrayElementAtIndex(i);
						}
						EditorGUILayout.EndHorizontal ();
					}

				}
				GUILayout.Label("");
				GUILayout.Label("");
			}
			EditorGUILayout.EndVertical();
		}



		EditorGUILayout.EndVertical();

		GUILayout.Label("");
		GUILayout.Label("");

// --> Button to Create a new Page
		if(GUILayout.Button("Create a new Menu page"))
		{
			b_CreatePage = true;
		}	



		GUILayout.Label("");
		GUILayout.Label("");


// --> Create a new page
		if(b_CreatePage)
		{
			b_CreatePage = false;

			for (int m = 0;m< List_GroupCanvas.arraySize;m++) {
				Undo.RegisterCompleteObjectUndo(myScript.List_GroupCanvas [m],"All Property Modif Player Sprite " + myScript.List_GroupCanvas [m].gameObject.name);
				//SerializedObject serializedObject0 = new UnityEditor.SerializedObject (myScript.List_GroupCanvas [m]);
				myScript.List_GroupCanvas [m].alpha = 0;
			}

			Undo.RegisterCompleteObjectUndo(myScript,"All Property Modif Player Sprite " + myScript.gameObject.name);



			GameObject NewGO = new GameObject("MyGO", typeof(RectTransform));
			NewGO.transform.SetParent(myScript.gameObject.transform);
			Undo.RegisterCreatedObjectUndo(NewGO,"NewGO");

			NewGO.AddComponent<CanvasRenderer>();
			NewGO.AddComponent<CanvasGroup>();

			NewGO.name = "Page_" + List_GroupCanvas.arraySize;

			NewGO.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
			NewGO.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

			NewGO.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
			NewGO.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
			NewGO.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
			NewGO.GetComponent<RectTransform>().localScale = new Vector3(1,1, 1);

			List_GroupCanvas.arraySize = List_GroupCanvas.arraySize + 1;
			List_GroupCanvas.GetArrayElementAtIndex(List_GroupCanvas.arraySize - 1).objectReferenceValue = NewGO.GetComponent<CanvasGroup>();
			b_MoreOptions.InsertArrayElementAtIndex(b_MoreOptions.arraySize - 1);
			b_MoreOptions.GetArrayElementAtIndex(b_MoreOptions.arraySize - 1).boolValue = false;


			list_gameObjectByPage.InsertArrayElementAtIndex(list_gameObjectByPage.arraySize - 1);
			list_gameObjectByPage.GetArrayElementAtIndex (list_gameObjectByPage.arraySize - 1).FindPropertyRelative ("listOfMenuGameobject").ClearArray ();



		}	
			
// --> delete last page
		if(b_DeletePage)
		{
			b_DeletePage = false;
			Undo.RegisterCompleteObjectUndo(myScript,"All Property Modif Player Sprite " + myScript.gameObject.name);

			List<CanvasGroup> 	tmpList = new List<CanvasGroup>();
			for (int i = 0; i < List_GroupCanvas.arraySize; i++) {
				if (i != whichCanvasToDelete) {
					tmpList.Add ((CanvasGroup)List_GroupCanvas.GetArrayElementAtIndex (i).objectReferenceValue);

				} else {
					Undo.DestroyObjectImmediate (myScript.List_GroupCanvas [i].gameObject);
					b_MoreOptions.DeleteArrayElementAtIndex (whichCanvasToDelete);
					list_gameObjectByPage.DeleteArrayElementAtIndex (whichCanvasToDelete);
				}
			}

			List_GroupCanvas.arraySize = List_GroupCanvas.arraySize - 1;

			for (int i = 0; i < tmpList.Count; i++) {
				List_GroupCanvas.GetArrayElementAtIndex(i).objectReferenceValue = tmpList[i];
			}
		}	
		serializedObject.ApplyModifiedProperties ();
	}
}
#endif