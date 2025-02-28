// Description : w_TextnVoice.cs : This script is used to create a window tab that allow to add or remove entry available in the diary
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;


public class w_TextnVoice : EditorWindow
{
	public static w_TextnVoice 		instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	private Vector2 				scrollPosAll;
	SerializedObject 				serializedObject2;
	SerializedProperty 				helpBoxEditor;
	SerializedProperty 				diaryList;
	SerializedProperty 				languageSlot;
	SerializedProperty 				_listOfLanguage;
	SerializedProperty 				showMoreOptions;
	SerializedProperty 				fisrtTextDisplayedInEditor;
	SerializedProperty 				currentLanguage;
	SerializedProperty 				howManyEntryDisplayed;
	SerializedProperty 				selectedID;
	SerializedProperty 				b_EditSubtitle;
	SerializedProperty 				currentAudioSubtitleEdit;
	SerializedProperty 				editorType;
	SerializedProperty 				showDefaultLanguage;
	SerializedProperty 				multipleVoiceOver;
    SerializedProperty              b_ShowEye;


	public TextList					_textList;
	Vector2		 					scrollPos;

	public int 						firstText = 0;
	public int 						LastText = 50;

	public EditorManipulateTextList manipulateTextList;
	public bool 					b_noTextList = false;

	public bool 					b_inventoryAssetExist = true;


	// Add menu item named "Test Mode Panel" to the Window menu
    [MenuItem("Tools/AP/Datas Managers/Texts and Voices (w_TextnVoice)",false,1)]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(w_TextnVoice));
	}

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

	private Texture2D 			Tex_01;
	private Texture2D 			Tex_02;
	private Texture2D 			Tex_03;
	private Texture2D 			Tex_04;
	private Texture2D 			Tex_05;

	public string[] 			listItemType = new string[]{"Object","Multi page Item"};

	public List<string> 		_test = new List<string>(); 
	public int 					page = 0;
	public int 					numberOfIndexInAPage = 50;
	public int 					seachSpecificID = 0;

	public Color 				_cGreen = new Color(1f,.8f,.4f,1);
	public Color 				_cGray = new Color(.9f,.9f,.9f,1);

	public EditorSubtitleSystem subtitleSystem;

	public Texture2D eye;
	public string s_subState = "";

	void OnEnable () {
		instance = this;
		manipulateTextList = new EditorManipulateTextList ();

		_MakeTexture ();

		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;


		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wTextnVoices.asset";
		_textList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;


		if (_textList) {

		} else {
			b_inventoryAssetExist = false;
		}
		serializedObject2 			= new UnityEditor.SerializedObject (_textList);
		helpBoxEditor				= serializedObject2.FindProperty ("helpBoxEditor");

		diaryList 					= serializedObject2.FindProperty ("diaryList");
		currentLanguage				= serializedObject2.FindProperty ("currentLanguage");
		languageSlot 				= diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot");
		_listOfLanguage 			= serializedObject2.FindProperty ("listOfLanguage");
		showMoreOptions 			= serializedObject2.FindProperty ("showMoreOptions");
		fisrtTextDisplayedInEditor 	= serializedObject2.FindProperty ("fisrtTextDisplayedInEditor");
		howManyEntryDisplayed		= serializedObject2.FindProperty ("howManyEntryDisplayed");
		selectedID					= serializedObject2.FindProperty ("selectedID");
		b_EditSubtitle				= serializedObject2.FindProperty ("b_EditSubtitle");
		currentAudioSubtitleEdit	= serializedObject2.FindProperty ("currentAudioSubtitleEdit");
		editorType					= serializedObject2.FindProperty ("editorType");
		showDefaultLanguage			= serializedObject2.FindProperty ("showDefaultLanguage");

		multipleVoiceOver 			= serializedObject2.FindProperty ("multipleVoiceOver");

        b_ShowEye                   = serializedObject2.FindProperty("b_ShowEye");



		updateListOfTitle (currentLanguage.intValue);

		subtitleSystem = new EditorSubtitleSystem ();

		string objectEye = "Assets/AP/Assets/Textures/Edit/Eye.png";
		eye = AssetDatabase.LoadAssetAtPath (objectEye, typeof(UnityEngine.Object)) as Texture2D;
	}


	void OnGUI()
	{
//--> Scrollview
		scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

		CheckTex ();
		GUIStyle style_Yellow_01 		= new GUIStyle ();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle ();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle ();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle ();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle ();	style_Yellow_Strong.normal.background 	= Tex_02;

		//		
		EditorGUILayout.BeginVertical(style_Purple);
		EditorGUILayout.HelpBox ("Diary Tab : Setup Texts and Audio files for diary, 2D Text and subtitles",MessageType.Info);
		EditorGUILayout.EndVertical ();


// --> Display data
		EditorGUILayout.BeginHorizontal();
		_textList = EditorGUILayout.ObjectField (_textList, typeof(UnityEngine.Object), true) as TextList;

		//-> Update Data File path
		if (GUILayout.Button ("Update", GUILayout.Width (50))) {
			updateDataFolder ();
		}
		EditorGUILayout.EndHorizontal ();

		if (_textList != null) {
			b_noTextList 		= false;

			//GUILayout.Label ("");

		
			serializedObject2.Update ();
            if (_test.Count != diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").arraySize)
                updateListOfTitle(0);


			int idEditorSize 	= _textList.idEditorSize;
			int titleEditorSize = _textList.titleEditorSize;
			int textEditorSize 	= _textList.textEditorSize;
			int first 			= fisrtTextDisplayedInEditor.intValue;

// --> See More Options
			moreOptionSection(style_Yellow_01,idEditorSize,titleEditorSize,textEditorSize);

// --> dropdown list Language
			//GUILayout.Label ("");
			dropdown_ChooseLanguage();

		
//--> Reseach Options
			researchOptions(first);

// --> Display List 50 by 50
			displayList(style_Yellow_01);

// --> Display selected ID form diary textList
			displaySelectedIDDiary (style_Orange,style_Blue,style_Yellow_01,idEditorSize,titleEditorSize,textEditorSize,selectedID.intValue);

			serializedObject2.ApplyModifiedProperties ();

			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("");
		}
		EditorGUILayout.EndScrollView ();
	}
		
	void OnInspectorUpdate()
	{
		Repaint();
	}


//--> moreOptionSection
	public void moreOptionSection(GUIStyle style_Yellow_01,int idEditorSize,int titleEditorSize,int textEditorSize){
		EditorGUILayout.BeginVertical (style_Yellow_01);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See Help Boxes :", GUILayout.Width (100));
		EditorGUILayout.PropertyField (helpBoxEditor, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See More Options :", GUILayout.Width (100));
		EditorGUILayout.PropertyField (showMoreOptions, new GUIContent (""), GUILayout.Width (30));

		EditorGUILayout.EndHorizontal ();

		bool _showNameOptions = _textList.showNameOptions;

		if (showMoreOptions.boolValue) {
			EditorGUI.BeginChangeCheck ();
//> Title Editor Size
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Title Editor Size :", GUILayout.Width (110));
				titleEditorSize = EditorGUILayout.IntField (titleEditorSize, GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();
//> Text Editor Size
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Text Editor Size :", GUILayout.Width (110));
				textEditorSize = EditorGUILayout.IntField (textEditorSize, GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();

//> Allow multiple voice over
		/*	EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Allow multiple voice over language :", GUILayout.Width (110));
				multipleVoiceOver.boolValue = EditorGUILayout.Toggle (multipleVoiceOver.boolValue, GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();
*/


//> Language Options
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Language Options :", GUILayout.Width (110));
				_showNameOptions = EditorGUILayout.Toggle (_showNameOptions, GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();


			if (_showNameOptions) {
				EditorGUILayout.HelpBox ("Please : If you want to create a new language use the Project Manager Tab. " +
					"\nMenu : Tools -> AP -> Project Manager", MessageType.Warning);

				for (var i = 0; i < _listOfLanguage.arraySize; i++) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("", GUILayout.Width (30));
					EditorGUILayout.LabelField ("Language " + i + " : ", GUILayout.Width (75));
					_listOfLanguage.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextField (_listOfLanguage.GetArrayElementAtIndex(i).stringValue, GUILayout.Width (100));


					if(i>0){
						if (GUILayout.Button ("-", GUILayout.Width (20))) {
//> Remove a language
							currentLanguage.intValue = 0;
							languageSlot 				= diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot");
							manipulateTextList.Remove_A_Language (_listOfLanguage,diaryList,i);
						}
					}
					EditorGUILayout.EndHorizontal ();

				}
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("", GUILayout.Width (30));
				if (GUILayout.Button ("Add a language", GUILayout.Width (180))) {
//> Add a language
					manipulateTextList.Add_A_Language (_listOfLanguage,diaryList);
				}
				EditorGUILayout.EndHorizontal ();
			}

			if (EditorGUI.EndChangeCheck ()) {
				Undo.RegisterFullObjectHierarchyUndo (_textList, _textList.name);
				_textList.idEditorSize = idEditorSize;
				_textList.titleEditorSize = titleEditorSize;
				_textList.textEditorSize = textEditorSize;
				_textList.showNameOptions = _showNameOptions;
			}

		}
		EditorGUILayout.EndVertical ();
	}

// --> dropdown list Language
	public void dropdown_ChooseLanguage(){
		_helpBox (0);
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Select Language : ", GUILayout.Width (110));
			EditorGUI.BeginChangeCheck ();

			currentLanguage.intValue = EditorGUILayout.Popup(currentLanguage.intValue, _textList.listOfLanguage.ToArray(), GUILayout.Width (100));

			if (EditorGUI.EndChangeCheck ()) {
				updateListOfTitle (currentLanguage.intValue);
			}
		EditorGUILayout.EndHorizontal ();
	}

	public void researchOptions(int first){
		int last = 0;
		if (diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize < howManyEntryDisplayed.intValue)
			last = languageSlot.arraySize;
		else
			last = first + howManyEntryDisplayed.intValue;

		if (diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize == 1)
			selectedID.intValue = 0;


		EditorGUILayout.BeginHorizontal ();
//> last 50 entry
		GUILayout.Label ("Page :", GUILayout.Width (40));
		if (GUILayout.Button ("<", GUILayout.Width (20))) {
			if (page>0)
				page--;
		}

		int modulotest = _test.Count/numberOfIndexInAPage;
		string modulotestString = (modulotest+1).ToString();
		if (modulotest < 10)
			modulotestString = "0" + modulotestString;

		if (_test.Count % numberOfIndexInAPage > 0)
			modulotest++;
		if (page + 1 < 10)
			GUILayout.Label ("0"+(page+1).ToString() +"/" + modulotestString , GUILayout.Width (40));
		else
			GUILayout.Label ((page+1).ToString() +"/" + modulotestString , GUILayout.Width (40));

//> next 50 entry
		if (GUILayout.Button (">", GUILayout.Width (20))) {
			if (_test.Count > (page + 1) * numberOfIndexInAPage)
				page++;
		}

//--> Button find an ID
		GUILayout.Label ("Find ID :" , GUILayout.Width (50));
		seachSpecificID = EditorGUILayout.IntField(seachSpecificID, GUILayout.Width (30));
		if (GUILayout.Button ("Search", GUILayout.Width (50))) {
			SearchID (seachSpecificID);
		}

		EditorGUILayout.EndHorizontal ();
	}

//--> Display list 50 by 50 
	public void displayList(GUIStyle style_Yellow_01){
		int _fisrt = page * numberOfIndexInAPage;
		int _last = 0;
		if (_test.Count < (page+1) * numberOfIndexInAPage) {
			_last = _test.Count;
		}
		else
			_last =  _fisrt + numberOfIndexInAPage;

		EditorGUILayout.BeginVertical(style_Yellow_01);
		scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Height(198));

		for (var i = _fisrt; i < _fisrt+numberOfIndexInAPage; i++) {
			if (_test.Count > i) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label (i.ToString (), GUILayout.Width (30), GUILayout.Height (18));

				if (selectedID.intValue == i)
					GUI.backgroundColor = _cGreen;
				else
					GUI.backgroundColor = _cGray;


// Select an ID
				if (GUILayout.Button (_test [i], GUILayout.Height (18))) {
					selectedID.intValue = i;
					b_EditSubtitle.boolValue = false;
					s_subState = "Edit Subtitle";
				}

				EditorGUILayout.EndHorizontal ();
			} else {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("", GUILayout.Width (30), GUILayout.Height (18));

				EditorGUILayout.EndHorizontal ();
			}
		}
		GUI.backgroundColor = _cGray;
		EditorGUILayout.EndScrollView ();
		EditorGUILayout.EndVertical ();
	}
		
// --> init inventory in the scene view
	public void ResetTest(){
		GameObject tmpObj = GameObject.Find ("Canvas_MainMenu");

		if (tmpObj) {
			
			Transform[] arrCanvas = tmpObj.GetComponentsInChildren<Transform> (true);
			foreach (Transform child in arrCanvas) {
				if (child.gameObject.name == "Journal_01_txt") {
					
					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					child.gameObject.GetComponent<Text> ().text = "_";
				}
				if (child.gameObject.name == "diarySelectedTxt") {
					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					child.gameObject.GetComponent<Text> ().text = "_";
				}
			}
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
		} else {
			if (EditorUtility.DisplayDialog ("INFO : Action not available"
				,"You need to have a CANVAS_MAINMENU in your hierarchy to do clear Title + Page description"
				,"Continue")) {

			}

		}




	}

//--> Display selected text and sprite in scene view
	public void DisplayTextInSceneView(int i,bool b_Message){
        GameObject tmpDR_Background = GameObject.Find("DoNotDelete");



        if(tmpDR_Background){
            randomScaleDoNotDelete(tmpDR_Background);

            float randomScale = UnityEngine.Random.Range(
           tmpDR_Background.GetComponent<RectTransform>().localScale.x,
           tmpDR_Background.GetComponent<RectTransform>().localScale.x + 0.0002f);



            tmpDR_Background.GetComponent<RectTransform>().localScale = new Vector3(
                randomScale,
                tmpDR_Background.GetComponent<RectTransform>().localScale.y,
                tmpDR_Background.GetComponent<RectTransform>().localScale.z
            );

		GameObject tmpObj = GameObject.Find ("Journal_01_txt");

		if (tmpObj) {
			Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);
			tmpObj.GetComponent<Text> ().text = diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue;
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
		}

		if (tmpObj == null && b_Message) {
			if (EditorUtility.DisplayDialog ("INFO : You need to Activate the DIARY_ALL_ENTRY UI"
				,"1-Select Canvas_MainMenu in the Hierarchy Tab" +
				"\n" +
				"\n2-Activate DIARY_ALL_ENTRY canvas in script Menu_Manager"
				,"Continue")) {

			}
		}
        }
        else{
            if (EditorUtility.DisplayDialog("INFO : Action not Possible."
                , "You need the gameobject ''DoNotDelete'' in your scene"
                , "Continue"))
            {

            }

        }
	}

    private void randomScaleDoNotDelete(GameObject tmpDR_Background)
    {
        if (tmpDR_Background)
        {
            float randomScale = UnityEngine.Random.Range(
           tmpDR_Background.GetComponent<RectTransform>().localScale.x,
           tmpDR_Background.GetComponent<RectTransform>().localScale.x + 0.0002f);



            tmpDR_Background.GetComponent<RectTransform>().localScale = new Vector3(
                randomScale,
                tmpDR_Background.GetComponent<RectTransform>().localScale.y,
                tmpDR_Background.GetComponent<RectTransform>().localScale.z
            );

        }
    }



	//--> Display selected page in scene view. Diary Page Need to be Activate in Canvas_MainMenu
	public void DisplayPageInSceneView(int i,int j,bool b_Message){
        GameObject tmpDR_Background = GameObject.Find("DoNotDelete");

        if (tmpDR_Background)
        {
            randomScaleDoNotDelete(tmpDR_Background);

            GameObject tmpObj2 = GameObject.Find("diarySelectedTxt");

            if (tmpObj2)
            {
                Undo.RegisterFullObjectHierarchyUndo(tmpObj2, tmpObj2.name);
                tmpObj2.GetComponent<Text>().text = diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryText").GetArrayElementAtIndex(j).stringValue;
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }

            if (tmpObj2 == null && b_Message)
            {
                if (EditorUtility.DisplayDialog("INFO : You need to Activate the DIARY_READER UI"
                    , "1-Select Canvas_MainMenu in the Hierarchy Tab" +
                        "\n" +
                    "\n2-Activate DIARY_READER canvas in script Menu_Manager"
                    , "Continue"))
                {

                }

            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("INFO : Action not Possible."
                , "You need the gameobject ''DoNotDelete'' in your scene"
                , "Continue"))
            {

            }

        }



	}





		
	private void updateListOfTitle(int currentLanguage){
		Undo.RegisterFullObjectHierarchyUndo (this, "Undo_Window");
		SerializedProperty newList = diaryList.GetArrayElementAtIndex (currentLanguage).FindPropertyRelative ("_languageSlot");
		_test.Clear ();
		for (var i = 0; i < newList.arraySize; i++) {
			_test.Add (newList.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue);
		}
	}

	private void updateDataFolder(){
		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;

        string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wTextnVoices.asset";
		_textList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;

		serializedObject2 			= new UnityEditor.SerializedObject (_textList);
		helpBoxEditor				= serializedObject2.FindProperty ("helpBoxEditor");
		diaryList 					= serializedObject2.FindProperty ("diaryList");
		currentLanguage				= serializedObject2.FindProperty ("currentLanguage");
		languageSlot 				= diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot");

		_listOfLanguage 			= serializedObject2.FindProperty ("listOfLanguage");
		showMoreOptions 			= serializedObject2.FindProperty ("showMoreOptions");
		fisrtTextDisplayedInEditor 	= serializedObject2.FindProperty ("fisrtTextDisplayedInEditor");
		howManyEntryDisplayed		= serializedObject2.FindProperty ("howManyEntryDisplayed");
		selectedID					= serializedObject2.FindProperty ("selectedID");
		editorType					= serializedObject2.FindProperty ("editorType");
		showDefaultLanguage 		= serializedObject2.FindProperty ("showDefaultLanguage");

		updateListOfTitle (currentLanguage.intValue);
	}

	private void SearchID (int value){
		if (value < diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize) {
			float fTest = (float)value / numberOfIndexInAPage;
			int iTest = Mathf.CeilToInt (fTest);
			bool firstValueInPage = false;

			if (fTest % numberOfIndexInAPage == iTest)
				firstValueInPage = true;

			if (!firstValueInPage) {
				page = iTest - 1;
				scrollPos.y = (float)(value - (numberOfIndexInAPage * page)) * 20.045f;
			} else {
				page = iTest;
				scrollPos.y = 0;
			}

			selectedID.intValue = value;
		} else {
			Debug.Log ("Info : ID doesn't exist. IDs are between 0 and " + (diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize - 1).ToString());
		}
	}

// --> When a new Page List is created the list go to the next page automatically
	private void SearchIDAfterIDCreation (int value){
		float fTest = (float)value/numberOfIndexInAPage;
		int iTest = Mathf.CeilToInt (fTest);
		bool firstValueInPage = false;
		//Debug.Log (iTest);

		if(fTest%numberOfIndexInAPage == iTest)
			firstValueInPage  = true;

		if (firstValueInPage) {
			page = iTest;
			scrollPos.y = 0;
		}


	}
		
// --> Display Selected ID Diary
	private void displaySelectedIDDiary(GUIStyle style_Orange,GUIStyle style_Blue,GUIStyle style_Yellow_01,int idEditorSize,int titleEditorSize,int textEditorSize,int value){


		EditorGUILayout.BeginVertical ( );

		for (var i = value; i < value+1; i++) {
			EditorGUILayout.BeginVertical (style_Yellow_01);

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("ID " + i.ToString () + " is selected.",EditorStyles.boldLabel, GUILayout.Width (120));

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			bool skip = false;
			if (currentLanguage.intValue == 0) {
				EditorGUILayout.LabelField ("In the Inventory : ", GUILayout.Width (150));

				//--> add entry in inventory
				if (GUILayout.Button ("Add new Entry", GUILayout.Width (150))) {
                    manipulateTextList.AddTextEntry (null, diaryList, i, "New description","New Title");
					updateListOfTitle (currentLanguage.intValue);
					selectedID.intValue++;
					SearchIDAfterIDCreation (selectedID.intValue);
					//break;
				}

				if (_textList.diaryList [currentLanguage.intValue]._languageSlot.Count > 1) {
					//> Remove ID in the index
					if (GUILayout.Button ("Deleted the selected Entry", GUILayout.Width (150))) {
						manipulateTextList.removeTextEntry (null, diaryList, i);
						updateListOfTitle (currentLanguage.intValue);
						if (i == diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize) {
							selectedID.intValue =  diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize - 1;
						}
						//break;
						skip = true;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			if (currentLanguage.intValue == 0) 
				_helpBox (1);


			EditorGUILayout.BeginHorizontal ();


			if (currentLanguage.intValue == 0) 
				EditorGUILayout.LabelField ("Move entry in the list : ", GUILayout.Width (150));


			if (_textList.diaryList [currentLanguage.intValue]._languageSlot.Count > i) {




				//--> allow to move entry down or up
				if (currentLanguage.intValue == 0) {
					if (GUILayout.Button ("v Move Down v", GUILayout.Width (150))) {
						if (selectedID.intValue < diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").arraySize - 1) {
							manipulateTextList.moveNextTextEntry (null, diaryList, i);
							selectedID.intValue++;
						}
						updateListOfTitle (currentLanguage.intValue);
					}

					if (GUILayout.Button ("^ Move Up ^", GUILayout.Width (150))) {
						if (selectedID.intValue > 0) {
							manipulateTextList.moveLastTextEntry (null, diaryList, i);
							selectedID.intValue--;
						}
						updateListOfTitle (currentLanguage.intValue);
					}
				}



				EditorGUILayout.EndHorizontal ();
			} else {
				EditorGUILayout.EndHorizontal ();
			}


			EditorGUILayout.EndVertical ();

			EditorGUILayout.LabelField ("", GUILayout.Width (40));


            if (!skip)
            {

				EditorGUILayout.BeginVertical(style_Orange);


				_helpBox(2);


				// EditorGUILayout.LabelField("", GUILayout.Width(30));
				//--> visualize rentry in the scebe view
				Rect r = GUILayoutUtility.GetLastRect();


				/*EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Clear Texts in Game Tab (Title + Page) : ", GUILayout.Width (220));

				//EditorGUILayout.LabelField ("", GUILayout.Width (50));
				if (GUILayout.Button ("Clear", GUILayout.Width (80))) {
					ResetTest ();
				}

				EditorGUILayout.LabelField ("", GUILayout.Width (40));
				EditorGUILayout.EndHorizontal ();
				*/
				//EditorGUILayout.LabelField ("", GUILayout.Width (90));


				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(0));
				//--> visualize page in the scene view
				r = GUILayoutUtility.GetLastRect();

				if (b_ShowEye.boolValue)
					if (GUI.Button(new Rect(40, r.y, 18, 18), eye, GUIStyle.none))
					{
						DisplayTextInSceneView(i, true);
						break;
					}
				//> ID Title
				EditorGUILayout.LabelField("", GUILayout.Width(45));
				EditorGUILayout.LabelField("Title : ", GUILayout.Width(40));

				SerializedProperty _serialTitle = diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryTitle").GetArrayElementAtIndex(0);

				string tmpStringTitle = _serialTitle.stringValue;
				EditorGUI.BeginChangeCheck();
				tmpStringTitle = GUILayout.TextField(tmpStringTitle, GUILayout.Width(titleEditorSize));
				if (EditorGUI.EndChangeCheck())
				{

					_serialTitle.stringValue = tmpStringTitle;

					DisplayTextInSceneView(i, false);
					updateListOfTitle(currentLanguage.intValue);

				}

				EditorGUILayout.PropertyField(showDefaultLanguage, new GUIContent(""), GUILayout.Width(30));

				EditorGUILayout.EndHorizontal();

				if (showDefaultLanguage.boolValue)
				{
					//> Display Title : default language
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("First Language:", GUILayout.Width(85));
					EditorGUILayout.LabelField(diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryTitle").GetArrayElementAtIndex(0).stringValue);
					EditorGUILayout.EndHorizontal();
				}


				EditorGUILayout.EndVertical();



				EditorGUILayout.LabelField("");


				if (s_subState == "Close")
					EditorGUILayout.BeginVertical(style_Blue);
				else
					EditorGUILayout.BeginVertical(style_Orange);

				EditorGUILayout.BeginVertical(style_Orange);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(45));
				EditorGUILayout.LabelField("Available in Diary : ", GUILayout.Width(120));

				SerializedProperty availableInDiary
				= diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("showInInventory");

				EditorGUILayout.PropertyField(availableInDiary, new GUIContent(""), GUILayout.Width(100));


				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(50));
				EditorGUILayout.LabelField("Audio Priority : ", GUILayout.Width(120));

				SerializedProperty audioPriority
				= diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("audioPriority");

				EditorGUILayout.PropertyField(audioPriority, new GUIContent(""), GUILayout.Width(20));

				//
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.LabelField("");

				for (var j = 0; j < diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryText").arraySize; j++)
				{
					if (b_EditSubtitle.boolValue && currentAudioSubtitleEdit.intValue == j || !b_EditSubtitle.boolValue)
					{

						EditorGUILayout.BeginVertical();
						SerializedProperty _serialText = diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryText").GetArrayElementAtIndex(j);

						string tmpString = _serialText.stringValue;
						EditorGUI.BeginChangeCheck();

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.Width(0));
						//--> visualize page in the scene view
						r = GUILayoutUtility.GetLastRect();

						if (b_ShowEye.boolValue)
							if (GUI.Button(new Rect(40, r.y, 18, 18), eye, GUIStyle.none))
							{
								DisplayPageInSceneView(i, j, true);
								break;
							}

						EditorGUILayout.LabelField("", GUILayout.Width(48));


						GUILayout.Label("Page: " + j, GUILayout.Width(50));





						tmpString = GUILayout.TextArea(tmpString, GUILayout.Width(textEditorSize));
						if (EditorGUI.EndChangeCheck())
						{
							//> ID Text (multi pages)
							_serialText.stringValue = tmpString;
							DisplayPageInSceneView(i, j, false);

						}
						//> Add new Page
						bool isPageDelete = false;
						if (currentLanguage.intValue == 0)
						{
							if (!b_EditSubtitle.boolValue)
							{
								if (GUILayout.Button("+", GUILayout.Width(20)))
								{
									manipulateTextList.AddNewPageEntry_In_MultiPages_Item(null, diaryList, i, j);
									//break;
								}
								//> Remove a page
								if (diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryText").arraySize > 1)
								{
									if (GUILayout.Button("-", GUILayout.Width(20)))
									{
										manipulateTextList.RemovePageEntry_In_MultiPages_Item(null, diaryList, i, j);
										isPageDelete = true;
										//break;
									}
								}
							}

						}

						EditorGUILayout.EndHorizontal();

                        if (!isPageDelete)
                        {
							EditorGUILayout.HelpBox("The Next Section : Audio + subtitle ONLY works when the entry is use for Voice Over. " +
												"It doesn't work for Text Item.", MessageType.Warning);
							EditorGUILayout.BeginHorizontal();
							GUILayout.Label("", GUILayout.Width(105));
							//> Display audio clip
							if (multipleVoiceOver.boolValue)
								EditorGUILayout.PropertyField(diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(100));
							else
							{
								if (currentLanguage.intValue == 0)
								{
									EditorGUILayout.PropertyField(diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(100));
								}
								else if (diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j).objectReferenceValue != null)
								{
									EditorGUILayout.LabelField(diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j).objectReferenceValue.name, GUILayout.Width(105));
								}
								else
								{
									EditorGUILayout.LabelField("No Audio Clip", GUILayout.Width(105));

								}
							}
							//> Button to display Subtitle editor
							//	if (diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (j).objectReferenceValue != null) {
							s_subState = "Edit Subtitle";
							if (b_EditSubtitle.boolValue)
								s_subState = "Close";

							if (GUILayout.Button(s_subState, GUILayout.Width(100)))
							{
								if (b_EditSubtitle.boolValue)
								{
									if (currentAudioSubtitleEdit.intValue == j)
									{
										b_EditSubtitle.boolValue = false;
									}
									else
									{
										currentAudioSubtitleEdit.intValue = j;
									}
								}
								else
								{
									currentAudioSubtitleEdit.intValue = j;
									b_EditSubtitle.boolValue = true;
								}
								subtitleSystem.callStopAudio();
							}
							//}
							EditorGUILayout.EndHorizontal();

							_helpBox(3);

							if (b_EditSubtitle.boolValue && currentAudioSubtitleEdit.intValue == j)
							{
								AudioClip _clip;

								if (multipleVoiceOver.boolValue)
								{
									_clip = (AudioClip)diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j).objectReferenceValue;
								}
								else
								{
									_clip = (AudioClip)diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryAudioClip").GetArrayElementAtIndex(j).objectReferenceValue;
								}

								if (_clip != null)
								{
									EditorGUILayout.BeginVertical(style_Blue);
									//> Display the subtitle editor
									if (multipleVoiceOver.boolValue)
									{
										//_clip = (AudioClip)diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (j).objectReferenceValue;
										subtitleSystem.DisplaySubtitleSystem(
											_clip,
											tmpString,
											diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diarySub").GetArrayElementAtIndex(j),
											diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diarySub").GetArrayElementAtIndex(j),
											style_Blue, currentLanguage.intValue,
											diaryList,
											i,
											j);

									}
									else
									{
										//_clip = (AudioClip)diaryList.GetArrayElementAtIndex (0).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryAudioClip").GetArrayElementAtIndex (j).objectReferenceValue;
										subtitleSystem.DisplaySubtitleSystem(
											_clip,
											tmpString,
											diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diarySub").GetArrayElementAtIndex(j),
											diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diarySub").GetArrayElementAtIndex(j),
											style_Blue, currentLanguage.intValue,
											diaryList,
											i,
											j);

									}
									EditorGUILayout.EndVertical();
								}

							}
						}
						

						EditorGUILayout.EndVertical();


					}
				}

				EditorGUILayout.EndVertical();
				GUILayout.Label("");
			}


		}
		EditorGUILayout.EndVertical ();

    }


	private void ApplyPropertiesToGameObject(int value){
		if (!Selection.activeTransform.GetComponent<Collider> ()) {
			if (EditorUtility.DisplayDialog ("INFO : Action none available"
				, "You need to have a Collider attached to the selected Object. Collider could be :" +
			    "\n" +
			    "\nBox Collider, mesh collider, capsule collider, sphere collider."

				, "Continue")) {

			}
		} else {
			Undo.RegisterFullObjectHierarchyUndo (Selection.activeTransform, Selection.activeTransform.name);
			if (Selection.activeTransform.GetComponent<TextProperties> ())
				DestroyImmediate (Selection.activeTransform.GetComponent<TextProperties> ());

			if (Selection.activeTransform.GetComponent<isObjectActivated> ())
				DestroyImmediate (Selection.activeTransform.GetComponent<isObjectActivated> ());

			if (Selection.activeTransform.GetComponent<SaveData> ())
				DestroyImmediate (Selection.activeTransform.GetComponent<SaveData> ());



			if (!Selection.activeTransform.GetComponent<TextProperties> ()) {
				Undo.AddComponent (Selection.activeGameObject, typeof(TextProperties));
			} 
			TextProperties item = Selection.activeTransform.GetComponent<TextProperties> ();

			Undo.RegisterFullObjectHierarchyUndo (item, item.name);

			item.uniqueID = diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (value).FindPropertyRelative ("uniqueItemID").intValue;
			item.managerID = value;
			item.textList = _textList;
			item.gameObject.tag = "Item";
			item.gameObject.isStatic = false;
			//Debug.Log (editorType.intValue);

			item.editorType = editorType.intValue;


			Selection.activeTransform.GetComponent<Collider> ().isTrigger = true;

			if (!Selection.activeTransform.GetComponent<isObjectActivated> ()
			    && diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (value).FindPropertyRelative ("showInInventory").boolValue) {
				Undo.AddComponent (Selection.activeGameObject, typeof(isObjectActivated));

				if (!Selection.activeTransform.GetComponent<SaveData> ()) {
					Undo.AddComponent (Selection.activeGameObject, typeof(SaveData));
				} 

				MonoBehaviour[] comp3 = Selection.activeTransform.GetComponents<MonoBehaviour> ();												// find all the MonoBehaviour in a gameObject											

				for (var i = 0; i < comp3.Length; i++) {
					if (comp3 [i].GetType ().ToString () == "isObjectActivated") {
						Selection.activeTransform.GetComponent<SaveData> ().isObjectActivatedIndex = i;
						Selection.activeTransform.GetComponent<SaveData> ().b_isObjectActivated = true;

						Debug.Log ("Method " + i + " : " + comp3 [i].GetType ().ToString ());
						break;
					}
				}
			} 
		}

	}

//--> If texture2D == null recreate the texture (use for color in the custom editor)
	private void CheckTex (){
		if (Tex_01 == null || Tex_02 == null || Tex_03 == null || Tex_04 == null || Tex_05 == null) {
			_MakeTexture ();
		}
	}

	private void _MakeTexture (){
		if (EditorPrefs.GetBool("AP_ProSkin") == true)
		{
			float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
			Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
			Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
			Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
			Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, 1f));
			Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
		}
		else
		{
			Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
			Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
			Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
			Tex_04 = MakeTex(2, 2, new Color(.5f, 1f, .4F, 1f));
			Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
		}
	}

	public void _helpBox(int value){
		if (helpBoxEditor.boolValue) {
			switch (value) {
			case 0:
				EditorGUILayout.HelpBox ("The next section allow to search and choose a specific ID",MessageType.Info);
				break;
			case 1:
				EditorGUILayout.HelpBox("Next buttons allow to move the entry in the List." +
					"\nVery Important !!! After Moving an entry don't forget to Update the scene. More info in the documentation.",MessageType.Warning);
				break;
			case 2:
				EditorGUILayout.HelpBox("The Next section allow you to setup the selected entry ",MessageType.Info);
				break;
            case 3:
                EditorGUILayout.HelpBox("IMPORTANT :  Always press the button ''Edit Subtitle'' " +
                                        "if you want to activate the subtitles for this entry ", MessageType.Warning);
                break;
			}
		}
	}


	void OnSceneGUI( )
	{
	}
}
#endif