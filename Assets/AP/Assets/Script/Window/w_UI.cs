// Description : w_UI.cs : This script is used to create a window tab that allow to add or remove entry (only a title)
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


public class w_UI : EditorWindow
{
	public static w_UI 		instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

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
	SerializedProperty 				editorType;
	SerializedProperty 				showDefaultLanguage;
    SerializedProperty              b_UI_DisplayTextArea;

	public TextList					_textList;
	Vector2		 					scrollPos;

	public int 						firstText = 0;
	public int 						LastText = 50;

	public EditorManipulateTextList manipulateTextList;
	public bool 					b_noTextList = false;

	public bool 					b_inventoryAssetExist = true;


	// Add menu item named "Test Mode Panel" to the Window menu
    [MenuItem("Tools/AP/Datas Managers/UI Texts (w_UI)",false,4)]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(w_UI));
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

	void OnEnable () {
		instance = this;
		manipulateTextList = new EditorManipulateTextList ();

		_MakeTexture ();

		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;


		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wUI.asset";
		_textList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;
		if (_textList && _datasProjectManager) {

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

        editorType		            = serializedObject2.FindProperty ("editorType");
		showDefaultLanguage		    = serializedObject2.FindProperty ("showDefaultLanguage");
        b_UI_DisplayTextArea        = serializedObject2.FindProperty("b_UI_DisplayTextArea");

		updateListOfTitle (currentLanguage.intValue);

		subtitleSystem = new EditorSubtitleSystem ();

		string objectEye = "Assets/AP/Assets/Textures/Edit/Eye.png";
		eye = AssetDatabase.LoadAssetAtPath (objectEye, typeof(UnityEngine.Object)) as Texture2D;
	}


	void OnGUI()
	{
//--> Scrollview
		scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

//--> Window description
		//GUI.backgroundColor = _cGreen;
		CheckTex ();
		GUIStyle style_Yellow_01 		= new GUIStyle ();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle ();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle ();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle ();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle ();	style_Yellow_Strong.normal.background 	= Tex_02;

		//		
		EditorGUILayout.BeginVertical(style_Purple);
			EditorGUILayout.HelpBox ("Window Tab : Texts for UI Buttons and UI Interface",MessageType.Info);
		EditorGUILayout.EndVertical ();

		//GUI.backgroundColor = _cGray;
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
			//GUILayout.Label ("");
		
//--> Reseach Options
			researchOptions(first);

// --> Display List 50 by 50
			displayList(style_Yellow_01);

// --> Display selected ID form diary textList
			displaySelectedID (style_Orange,style_Blue,style_Yellow_01,idEditorSize,titleEditorSize,textEditorSize,selectedID.intValue);

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
			EditorGUILayout.LabelField ("Select Langue : ", GUILayout.Width (110));
			EditorGUI.BeginChangeCheck ();

			currentLanguage.intValue = EditorGUILayout.Popup(currentLanguage.intValue, _textList.listOfLanguage.ToArray(), GUILayout.Width (100));

			if (EditorGUI.EndChangeCheck ()) {
				updateListOfTitle (currentLanguage.intValue);
			}
		EditorGUILayout.EndHorizontal ();
	}

	public void researchOptions(int first){
		
		int last = 0;
		if (languageSlot.arraySize < howManyEntryDisplayed.intValue)
			last = languageSlot.arraySize;
		else
			last = first + howManyEntryDisplayed.intValue;

		if (languageSlot.arraySize == 1)
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
		GameObject tmpObj = GameObject.Find ("txtTitle");

		if (tmpObj) {
			Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);
			tmpObj.GetComponent<Text> ().text = "Item Title";
		}

		tmpObj = GameObject.Find ("txtDescription");

		if (tmpObj) {
			Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);
			tmpObj.GetComponent<Text> ().text = "item Description";
		}

		GameObject tmpObj3 = GameObject.Find ("Item_01");

		if (tmpObj3) {
			Undo.RegisterFullObjectHierarchyUndo (tmpObj3, tmpObj3.name);
			tmpObj3.GetComponent<Image> ().sprite = null;
		}

		UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
	}

//--> Display selected text and sprite in scene view
	public void DisplayTextInSceneView(int i){
	GameObject tmpObj = GameObject.Find ("txtTitle");

	if (tmpObj) {
		Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);
		tmpObj.GetComponent<Text> ().text = languageSlot.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue;
	}

	GameObject tmpObj2 = GameObject.Find ("txtDescription");

	if (tmpObj2) {
		Undo.RegisterFullObjectHierarchyUndo (tmpObj2, tmpObj2.name);
		tmpObj2.GetComponent<Text> ().text = languageSlot.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryText").GetArrayElementAtIndex (0).stringValue;
	}

	GameObject tmpObj3 = GameObject.Find ("Item_01");

	if (tmpObj3) {
		Undo.RegisterFullObjectHierarchyUndo (tmpObj3, tmpObj3.name);
		tmpObj3.GetComponent<Image> ().sprite = (Sprite)languageSlot.GetArrayElementAtIndex (i).FindPropertyRelative ("diarySprite").GetArrayElementAtIndex (0).objectReferenceValue;
	}


	UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
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

		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wUI.asset";
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
		editorType		= serializedObject2.FindProperty ("editorType");
		showDefaultLanguage = serializedObject2.FindProperty ("showDefaultLanguage");

		updateListOfTitle (currentLanguage.intValue);
	}

	private void SearchID (int value){
		if (value < languageSlot.arraySize) {
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
			Debug.Log ("Info : ID doesn't exist. IDs are between 0 and " + (languageSlot.arraySize - 1).ToString());
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
		
// --> Display Selected ID 
	private void displaySelectedID(GUIStyle style_Orange,GUIStyle style_Blue,GUIStyle style_Yellow_01,int idEditorSize,int titleEditorSize,int textEditorSize,int value){

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
						//Debug.Log("i: " + i);
						selectedID.intValue--;
						if (i == languageSlot.arraySize-1) {
							selectedID.intValue =  languageSlot.arraySize - 1;
						}
						//break;
						skip = true;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

           
				if (currentLanguage.intValue == 0)
					_helpBox(2);


				EditorGUILayout.BeginHorizontal();


				if (currentLanguage.intValue == 0)
					EditorGUILayout.LabelField("Move entry in the list : ", GUILayout.Width(150));


				if (_textList.diaryList[currentLanguage.intValue]._languageSlot.Count > i)
				{




					//--> allow to move entry down or up
					if (currentLanguage.intValue == 0)
					{
						if (GUILayout.Button("v Move Down v", GUILayout.Width(150)))
						{
							if (selectedID.intValue < languageSlot.arraySize - 1)
							{
								manipulateTextList.moveNextTextEntry(null, diaryList, i);
								selectedID.intValue++;
							}
							updateListOfTitle(currentLanguage.intValue);
						}

						if (GUILayout.Button("^ Move Up ^", GUILayout.Width(150)))
						{
							if (selectedID.intValue > 0)
							{
								manipulateTextList.moveLastTextEntry(null, diaryList, i);
								selectedID.intValue--;
							}
							updateListOfTitle(currentLanguage.intValue);
						}
					}



					EditorGUILayout.EndHorizontal();
				}
				else
				{
					EditorGUILayout.EndHorizontal();
				}


				EditorGUILayout.EndVertical();


			if (!skip)
			{
				EditorGUILayout.LabelField("", GUILayout.Width(40));

				EditorGUILayout.BeginVertical(style_Orange);
				_helpBox(1);

				EditorGUILayout.BeginHorizontal();


				//> Button to test ID in scene view
				EditorGUILayout.LabelField("", GUILayout.Width(45));
				EditorGUILayout.LabelField("Title : ", GUILayout.Width(40));


				SerializedProperty _serialTitle = diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryTitle").GetArrayElementAtIndex(0);
				string tmpStringTitle = _serialTitle.stringValue;
				EditorGUI.BeginChangeCheck();

				if (!b_UI_DisplayTextArea.boolValue)
					tmpStringTitle = GUILayout.TextField(tmpStringTitle, GUILayout.Width(titleEditorSize));
				else
					tmpStringTitle = GUILayout.TextArea(tmpStringTitle, GUILayout.Width(titleEditorSize));
				if (EditorGUI.EndChangeCheck())
				{
					//> ID Title
					_serialTitle.stringValue = tmpStringTitle;
					//_textList.diaryList [currentLanguage.intValue]._languageSlot [i].diaryTitle [0] = tmpStringTitle;

					GameObject tmpObj = GameObject.Find("txtTitle");

					if (tmpObj)
					{
						Undo.RegisterFullObjectHierarchyUndo(tmpObj, tmpObj.name);
						tmpObj.GetComponent<Text>().text = _serialTitle.stringValue;
					}
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
			}

			
			GUILayout.Label ("");
		}
		EditorGUILayout.EndVertical ();

	}

//> Apply the ID Property to the selected GameObject in the Hierarchy
	private void ApplyPropertiesToGameObject(int value){

		if (!Selection.activeTransform.GetComponent<TextProperties> ()) {
			Undo.AddComponent (Selection.activeGameObject, typeof(TextProperties));
		} 
		TextProperties item = Selection.activeTransform.GetComponent<TextProperties> ();

		Undo.RegisterFullObjectHierarchyUndo(item,item.name);

		item.uniqueID = languageSlot.GetArrayElementAtIndex (value).FindPropertyRelative ("uniqueItemID").intValue;
		item.managerID = value;
		item.textList = _textList;
		item.editorType = editorType.intValue;


		Text newText = Selection.activeTransform.GetComponent<Text> ();
		if (newText) {

			SerializedProperty _Title = diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (value).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0);

			newText.text = _Title.stringValue;
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
			Tex_04 = MakeTex(2, 2, new Color(.4f, 1f, .9F, 1f));
			Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
		}
	}

	void OnSceneGUI( )
	{
	}


	public void _helpBox(int value){
		if (helpBoxEditor.boolValue) {
			switch (value) {
			case 0:
				EditorGUILayout.HelpBox ("The next section allow to search and choose a specific ID",MessageType.Info);
				break;
			case 1:
				EditorGUILayout.HelpBox ("The next section allow to setup the selected ID",MessageType.Info);
				break;
			case 2:
				EditorGUILayout.HelpBox("Next buttons allow to move the entry in the List." +
					"\nVery Important !!! After Moving an entry don't forget to Update the scene. More info in the documentation.",MessageType.Warning);
				break;
			}
		}
	}

}
#endif