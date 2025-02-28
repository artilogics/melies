// Description : w_Item.cs : This script is used to create a window tab that allow to add or remove entry available in the inventory
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
using UnityEngine.SceneManagement;


public class w_Item : EditorWindow
{
	
    public static w_Item 		instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	public Vector2					scrollPosAll;

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
	//SerializedProperty 				editorType;
	SerializedProperty 				showDefaultLanguage;
    SerializedProperty              b_ShowEye;


	public TextList					_textList;
	Vector2		 					scrollPos;

	public int 						firstText = 0;
	public int 						LastText = 50;

	public EditorManipulateTextList manipulateTextList;
	public bool 					b_noTextList = false;

	public bool 					b_inventoryAssetExist = true;


	// Add menu item named "Test Mode Panel" to the Window menu
    [MenuItem("Tools/AP/Datas Managers/Items (w_Item)",false,2)]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Item));
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

	private Texture2D 		Tex_01;
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	public string[] listItemType = new string[]{"Object","Multi page Item"};


	public List<string> _test = new List<string>(); 
	public int page = 0;
	public int numberOfIndexInAPage = 50;
	public int seachSpecificID = 0;

	public Color _cGreen = new Color(1f,.8f,.4f,1);
	public Color _cGray = new Color(.9f,.9f,.9f,1);

	public Texture2D eye;

    public GameObject newObj; // the current object select in the turnable. Use to setup scale and rotation in the 3D Viewer

	void OnEnable () {
		instance = this;

		manipulateTextList = new EditorManipulateTextList ();

		_MakeTexture ();

		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;


        string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wItem.asset";
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
		//editorType		            = serializedObject2.FindProperty ("editorType");
		showDefaultLanguage		    = serializedObject2.FindProperty ("showDefaultLanguage");
        b_ShowEye                   = serializedObject2.FindProperty("b_ShowEye");



		updateListOfTitle (currentLanguage.intValue);

		string objectEye = "Assets/AP/Assets/Textures/Edit/Eye.png";
		eye = AssetDatabase.LoadAssetAtPath (objectEye, typeof(UnityEngine.Object)) as Texture2D;
	}


	void OnGUI()
	{
		//--> Scrollview start : display all the entry
		//	scrollPos =
		//	EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Height (600));
		scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

		CheckTex ();
		GUIStyle style_Yellow_01 		= new GUIStyle ();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle ();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle ();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle ();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle ();	style_Yellow_Strong.normal.background 	= Tex_02;

		EditorGUILayout.BeginVertical(style_Purple);
		EditorGUILayout.HelpBox ("Inventory Tab : Setup Items (even if the item is not used in the inventory)",MessageType.Info);
		EditorGUILayout.EndVertical ();

// --> Display invertory data
		EditorGUILayout.BeginHorizontal();
		_textList = EditorGUILayout.ObjectField (_textList, typeof(UnityEngine.Object), true) as TextList;

		//-> Update Data File path
		if (GUILayout.Button ("Update", GUILayout.Width (50))) {
			updateDataFolder ();
		}
		EditorGUILayout.EndHorizontal ();

		if (_textList != null) {
			b_noTextList 		= false;

			serializedObject2.Update ();
            if (_test.Count != diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").arraySize)
                updateListOfTitle(0);


			int idEditorSize 	= _textList.idEditorSize;
			int titleEditorSize = _textList.titleEditorSize;
			int textEditorSize 	= _textList.textEditorSize;

			int first = fisrtTextDisplayedInEditor.intValue;


// --> See More Options
			moreOptionSection(style_Yellow_01,idEditorSize,titleEditorSize,textEditorSize);


// --> dropdown list Language
			dropdown_ChooseLanguage();

//--> Reseach Options
			researchOptions(first);

// --> Display List 50 by 50
			displayList(style_Yellow_01);

// --> Display selected ID form diary textList
			displaySelectedID (style_Orange,style_Yellow_01,idEditorSize,titleEditorSize,textEditorSize,selectedID.intValue);


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
				if (child.gameObject.name == "inventorySelectedTitle") {

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					child.gameObject.GetComponent<Text> ().text = "_";
				}
				if (child.gameObject.name == "inventorySelectedTxt") {
					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					child.gameObject.GetComponent<Text> ().text = "_";
				}
				if (child.gameObject.name == "Item_01") {
					//Debug.Log ("here");
					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					child.gameObject.GetComponent<Image> ().sprite = null;
				}
			}
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
		} else {
			if (EditorUtility.DisplayDialog ("INFO : Action none available"
				,"You need to have a CANVAS_MAINMENU in your hierarchy to do Clear Inventory Title + Page description + Sprite"
				,"Continue")) {

			}

		}
	}

//--> Display selected text an sprite in scene view
	public void DisplayTextInSceneView(int i){
		GameObject tmpObj = GameObject.Find ("inventorySelectedTitle");

    	if (tmpObj) {
    		Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);
    			tmpObj.GetComponent<Text> ().text = diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue;
    	}

    		GameObject tmpObj2 = GameObject.Find ("inventorySelectedTxt");

    	if (tmpObj2) {
    		Undo.RegisterFullObjectHierarchyUndo (tmpObj2, tmpObj2.name);
    			tmpObj2.GetComponent<Text> ().text = diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diaryText").GetArrayElementAtIndex (0).stringValue;
    	}

    	GameObject tmpObj3 = GameObject.Find ("Item_01");

    	if (tmpObj3) {
    		Undo.RegisterFullObjectHierarchyUndo (tmpObj3, tmpObj3.name);
    			tmpObj3.GetComponent<Image> ().sprite = (Sprite)diaryList.GetArrayElementAtIndex (currentLanguage.intValue).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("diarySprite").GetArrayElementAtIndex (0).objectReferenceValue;

            float rand = UnityEngine.Random.Range(1.0000f, 1.0002f);
            tmpObj3.GetComponent<RectTransform>().localScale = new Vector3(rand, rand, rand);
        }


    		if (tmpObj == null) {
    			if (EditorUtility.DisplayDialog (
                    "Info : You need to Activate the Inventory UI in the Hierarchy Tab to use this button.",
    				"1-Select Canvas_MainMenu in the Hierarchy Tab" +
    				"\n2-Activate Inventory canvas in script Menu_Manager",
    				"Continue")) {
    			}
    		}

    	UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
	}


	private GameObject currentViewerTestObject;
	//--> Display 3D viewer in scene Tab to test a Gameobject
	public void Display3DViewer(int i,bool available){
		GameObject tmpObj = null;


		if (diaryList.GetArrayElementAtIndex (0).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("refGameObject").objectReferenceValue == null
		    && Selection.activeObject == null) {
			if (EditorUtility.DisplayDialog ("Info : No Object are selected.", "" +
			    "\nOptions 1 : Choose prefab for this inventory Entry." +
			    "\nOption 2 : Select a GameObject in the Hierarchy"
				, "Continue")) {

			}
		} else {

			GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

			foreach (GameObject go in allObjects) {
				Transform[] Children = go.GetComponentsInChildren<Transform> (true);
				foreach (Transform child in Children) {
					if (child.name == "CamShowObject") {
						tmpObj = child.gameObject;
						break;
					}
				}
			}


			if (tmpObj) {
				Undo.RegisterFullObjectHierarchyUndo (tmpObj, tmpObj.name);


                //-> Check if an object is already in the turntable
                Transform[] childrenTurntable = tmpObj.GetComponent<investigationCam>().turntable.GetComponentsInChildren<Transform>();

                if(childrenTurntable.Length >1){
                    Selection.activeObject = newObj;
                }
                else{
                    if (!tmpObj.gameObject.activeSelf)
                    {
                        tmpObj.gameObject.SetActive(true);
                    }
                    newObj = null;
                    if (!available && !AssetDatabase.Contains(Selection.activeObject)
                        || available && diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue != null)
                    {
                        if (available)
                        {
                            newObj = Instantiate((GameObject)diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue,
                                tmpObj.GetComponent<investigationCam>().turntable.transform);
                        }
                        else
                        {
                            newObj = Instantiate((GameObject)Selection.activeObject,
                                tmpObj.GetComponent<investigationCam>().turntable.transform);
                            newObj.layer = 8;

                            newObj.transform.localPosition = new Vector3(0, 0, 0);
                        }


                        Transform[] children = newObj.GetComponentsInChildren<Transform>();

                        foreach (Transform child in children)
                        {
                            child.gameObject.layer = 8;
                        }

                        Undo.RegisterCreatedObjectUndo(newObj, newObj.name);

                        //  currentGameObject = newObj;
                        float newScale = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("prefabSizeInViewer").floatValue;
                        Vector3 newEulerAngle = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("prefabRotationInViewer").vector3Value;

                        newObj.transform.localScale = new Vector3(newScale, newScale, newScale);
                        newObj.transform.localEulerAngles = newEulerAngle;
                        newObj.transform.localPosition = new Vector3(0, 0, 0);

                        Selection.activeObject = newObj;
                    }
                    else
                    {
                        if (!available)
                        {
                            if (EditorUtility.DisplayDialog(
                                "Info :  Action not Possible.",
                                "You need to select an object in the Hierarhy first.",
                                "Continue")) { }
                        }
                        else
                        {
                            if (EditorUtility.DisplayDialog(
                                "Info : Action not Possible.",
                                "You need to create an prefab for this entry first.",
                                "Continue")) { }
                        }
                    } 
                }
			}

			if (tmpObj == null) {
				if (EditorUtility.DisplayDialog ("Info : You need to Activate the Inventory UI in the Hierarchy Tab to use this button."
				, "1-Select Canvas_MainMenu in the Hierarchy Tab" +
				   "\n2-Activate Inventory canvas in script Menu_Manager"
				, "Continue")) {

				}
			}
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
		}
	}



    //--> Update the list of entry
	private void updateListOfTitle(int currentLanguage){
		Undo.RegisterFullObjectHierarchyUndo (this, "Undo_Window");
		SerializedProperty newList = diaryList.GetArrayElementAtIndex (currentLanguage).FindPropertyRelative ("_languageSlot");
		_test.Clear ();
		for (var i = 0; i < newList.arraySize; i++) {
			_test.Add (newList.GetArrayElementAtIndex (i).FindPropertyRelative ("diaryTitle").GetArrayElementAtIndex (0).stringValue);
		}
	}

    //--> Find the ProjectManagerDatas.asset in the project folder
	public void updateDataFolder(){
		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;

		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wItem.asset";
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
		//editorType					= serializedObject2.FindProperty ("editorType");
		showDefaultLanguage 		= serializedObject2.FindProperty ("showDefaultLanguage");

		updateListOfTitle (currentLanguage.intValue);
	}
		

	private void displaySelectedID(GUIStyle style_Orange,GUIStyle style_Yellow_01,int idEditorSize,int titleEditorSize,int textEditorSize,int value){
	//--> Display information for a specific ID

	EditorGUILayout.BeginHorizontal ();

	EditorGUILayout.BeginVertical ();

	
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
						skip = true;
						//break;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			if (currentLanguage.intValue == 0)
				_helpBox (2);


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

				EditorGUILayout.HelpBox("The Next section allow you to setup the selected entry ", MessageType.Info);

				Rect r = GUILayoutUtility.GetLastRect();

				EditorGUILayout.BeginHorizontal();

				/////////////////
				//--> Diplay informations for each inventory entry
				EditorGUI.BeginChangeCheck();
				if (_textList.diaryList[currentLanguage.intValue]._languageSlot.Count > i)
				{
					EditorGUILayout.EndHorizontal();

					//if (b_DisplayText) {
					for (var j = 0; j < _textList.diaryList[currentLanguage.intValue]._languageSlot[i].diaryTitle.Count; j++)
					{
						EditorGUILayout.BeginVertical();


						EditorGUILayout.BeginHorizontal();

						SerializedProperty availableInInventory
						= diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("showInInventory");



						EditorGUILayout.LabelField("", GUILayout.Width(0));
						int eyePosition = 22;
						if (showDefaultLanguage.boolValue)
							eyePosition = 22 + 18;


						if (availableInInventory.boolValue)
						{
							r = GUILayoutUtility.GetLastRect();
							if (b_ShowEye.boolValue)
								if (GUI.Button(new Rect(35, r.y + eyePosition, 18, 18), eye, GUIStyle.none))
								{
									DisplayTextInSceneView(i);
									break;
								}
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

							GameObject tmpObj = GameObject.Find("inventorySelectedTitle");

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

						EditorGUILayout.BeginVertical(style_Orange);
						if (diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("itemType").intValue == 0)
						{   // Only if it is an object not a multi page type
							//--> object avalaible or not in the diary 

							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(45));
							EditorGUILayout.LabelField("Available in Inventory :", GUILayout.Width(125));

							EditorGUILayout.PropertyField(availableInInventory, new GUIContent(""), GUILayout.Width(100));
							EditorGUILayout.EndHorizontal();
						}
						else
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("");
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();


						if (!availableInInventory.boolValue && diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue != null)
						{
							diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue = null;
						}

						if (availableInInventory.boolValue)
						{
							//--> Display inventory prefab
							_helpBox(5);
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(45));
							EditorGUILayout.LabelField("Prefab :", GUILayout.Width(125));
							EditorGUI.BeginChangeCheck();
							if (diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue != null)
								EditorGUILayout.LabelField(diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue.name, GUILayout.Width(100));

							if (currentLanguage.intValue == 0)
							{
								if (GUILayout.Button("Create", GUILayout.Width(70)))
								{

									if (Selection.activeGameObject != null)
									{

										string localPath = "Assets/AP/Assets/Inventory/" + Selection.activeGameObject.name + ".prefab";
										if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
										{
											if (EditorUtility.DisplayDialog("Are you sure?",
												   "The prefab already exists. Do you want to use it?" +
												   "\n\nIf you want to create a new object rename the object in the Hierarchy with a unique name.",
												   "Yes",
												   "No"))
											{

												string objectPath = "Assets/AP/Assets/Inventory/" + Selection.activeGameObject.name + ".prefab";
												GameObject _prefab = AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject)) as GameObject;
												if (_prefab)
												{
													diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue = _prefab;
													_prefab.layer = 8;  // layer : ObjectViewerCam

													_prefab.isStatic = false;

													Transform[] children = _prefab.GetComponentsInChildren<Transform>();

													foreach (Transform child in children)
													{
														child.gameObject.layer = 8;
														child.gameObject.isStatic = false;
													}
												}
												selectLayerAndStatic(Selection.activeGameObject, 0, false);
											}
										}
										else
										{
											//UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab (localPath);
											//PrefabUtility.ReplacePrefab (Selection.activeGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
											PrefabUtility.SaveAsPrefabAsset(Selection.activeGameObject, localPath);

											string objectPath = "Assets/AP/Assets/Inventory/" + Selection.activeGameObject.name + ".prefab";
											GameObject _prefab = AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject)) as GameObject;
											if (_prefab)
											{
												diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("refGameObject").objectReferenceValue = _prefab;
												_prefab.layer = 8; // layer : ObjectViewerCam

												Transform[] children = _prefab.GetComponentsInChildren<Transform>();

												foreach (Transform child in children)
												{
													child.gameObject.layer = 8;
												}
											}

											selectLayerAndStatic(Selection.activeGameObject, 0, false);
										}
										//	
									}
									else
									{
										if (EditorUtility.DisplayDialog("Info", "You need to select an object in the Hierarchy Tab", "Continue"))
										{

										}
									}
								}
							}

							EditorGUILayout.EndHorizontal();
						}

						if (diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("showInInventory").boolValue)
						{

							//--> Display inventory sprite
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(45));
							EditorGUILayout.LabelField("Sprite :", GUILayout.Width(125));

							EditorGUILayout.PropertyField(diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diarySprite").GetArrayElementAtIndex(j), new GUIContent(""), GUILayout.Width(100));

							EditorGUILayout.EndHorizontal();

						}
						EditorGUI.BeginChangeCheck();
						string tmpString = _textList.diaryList[currentLanguage.intValue]._languageSlot[i].diaryText[j];
						EditorGUI.BeginChangeCheck();


						if (diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("showInInventory").boolValue)
						{

							EditorGUILayout.BeginHorizontal();
							//--> display description
							EditorGUILayout.LabelField("", GUILayout.Width(45));
							EditorGUILayout.LabelField("Description :", GUILayout.Width(125));
							tmpString = GUILayout.TextArea(tmpString, GUILayout.Width(textEditorSize));
							if (EditorGUI.EndChangeCheck())
							{
								diaryList.GetArrayElementAtIndex(currentLanguage.intValue).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("diaryText").GetArrayElementAtIndex(j).stringValue = tmpString;
								GameObject tmpObj = GameObject.Find("inventorySelectedTxt");

								if (tmpObj)
								{
									Undo.RegisterFullObjectHierarchyUndo(tmpObj, tmpObj.name);
									tmpObj.GetComponent<Text>().text = null;
								}
							}
							EditorGUILayout.EndHorizontal();

						}

						EditorGUILayout.EndVertical();


						//--> Options to setup the selected OBject in the 3D Viewer

						EditorGUILayout.BeginVertical(style_Yellow_01);

						if (availableInInventory.boolValue)
							_helpBox(6);
						else
							_helpBox(7);

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.Width(45));

						string tmpTxt = "Create an instance of the prefab to test in the 3D viewer";
						if (!availableInInventory.boolValue)
							tmpTxt = "Test the selected object in the Hierarchy in the 3D viewer";

						if (GUILayout.Button(tmpTxt, GUILayout.Width(330)))
						{
							Display3DViewer(i, availableInInventory.boolValue);
							//break;
						}
						EditorGUILayout.EndHorizontal();




						if (Selection.activeGameObject && Selection.activeGameObject.transform.parent && Selection.activeGameObject.transform.parent.name == "Turntable")
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("", GUILayout.Width(45));
							if (GUILayout.Button("Update and finish modifications", GUILayout.Width(330)))
							{
								if (Selection.activeGameObject)
								{
									diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("prefabRotationInViewer").vector3Value = Selection.activeGameObject.transform.localEulerAngles;
									diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("prefabSizeInViewer").floatValue = Selection.activeGameObject.transform.localScale.x;
									Undo.DestroyObjectImmediate(Selection.activeGameObject);
									if (EditorUtility.DisplayDialog("INFO : Rotation and scale are saved"
										, ""
										, "Continue"))
									{ }
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						/*
						//--> display objet size in viewer
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("", GUILayout.Width (45));
						EditorGUILayout.LabelField ("Scale :", GUILayout.Width (50));

						EditorGUILayout.PropertyField (diaryList.GetArrayElementAtIndex (0).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("prefabSizeInViewer"), new GUIContent (""), GUILayout.Width (30));

						//--> display object size in viewer
						EditorGUILayout.LabelField ("", GUILayout.Width (10));
						EditorGUILayout.LabelField ("Euler Angle :", GUILayout.Width (70));



						EditorGUILayout.PropertyField (diaryList.GetArrayElementAtIndex (0).FindPropertyRelative ("_languageSlot").GetArrayElementAtIndex (i).FindPropertyRelative ("prefabRotationInViewer"), new GUIContent (""), GUILayout.Width (150));
						EditorGUILayout.EndHorizontal ();
						*/
						if (availableInInventory.boolValue)
							_helpBox(8);
						else
							_helpBox(9);

						EditorGUILayout.LabelField("");
						EditorGUILayout.EndVertical();
					}

				}
				EditorGUILayout.EndVertical();
			}
		

	}



	EditorGUILayout.EndVertical ();
	/////////////



	EditorGUILayout.EndHorizontal ();
	}


    //--> Search an ID in the List
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

		if(fTest % numberOfIndexInAPage == iTest)
			firstValueInPage  = true;

		if (firstValueInPage) {
			page = iTest;
			scrollPos.y = 0;
		}
	}

    //--> Select a layer and if the gameobject is static or not
	private void selectLayerAndStatic (GameObject newObj,int layerNumber, bool b_Static){
		//PrefabUtility.DisconnectPrefabInstance (newObj);
		newObj.layer = layerNumber;
		newObj.isStatic = b_Static;

		Transform[] children = newObj.GetComponentsInChildren<Transform> ();

		foreach(Transform child in children ){
			child.gameObject.layer = layerNumber;
			child.gameObject.isStatic = b_Static;
		}
	}


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
			Tex_04 = MakeTex(2, 2, new Color(.8f, 1f, .9F, 1f));
			Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
		}
	}


    /*
	static void CreateNew(GameObject obj, string localPath)
	{
		UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
		PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}
	*/

	public void _helpBox(int value){
		if (helpBoxEditor.boolValue) {
			switch (value) {
    			case 0:
    				EditorGUILayout.HelpBox ("The next section allows to search and choose a specific ID",MessageType.Info);
    				break;
    			case 1:
    				EditorGUILayout.HelpBox ("The next section allows to setup the selected ID",MessageType.Info);
    				break;
    			case 2:
    				EditorGUILayout.HelpBox("Next buttons allows to move the entry in the List." +
    					"\nVery Important !!! After Moving an entry don't forget to Update the scene. More info in the documentation.",MessageType.Warning);
    				break;
    			case 3:
    				EditorGUILayout.HelpBox("The Next section allows you to setup the selected entry ",MessageType.Info);
    				break;
    			case 4:
    				EditorGUILayout.HelpBox("You MUST have a COLLIDER attached to the selected gameObject.",MessageType.Warning);
    				break;
    			
                case 5:
                        EditorGUILayout.HelpBox("-Prefab : Create a prefab. This prefab is displayed in the 3D Viewer." +
                                                "\n-Sprite : Choose a sprite that represent this Item in the Inventory Menu." +
                                                "\n-Description : Write a description for this Item. This description is displayed in the Inventory.", MessageType.Info);
                   break;

                case 6:
                    EditorGUILayout.HelpBox("This section allows to setup the Item in the 3D Viewer." +
                                            "\n1-Select your Item in the Hierarchy." +
                                            "\n2-Press button ''Create an instance of the prefab to test in the 3D Viewer''" +
                                            "\n3-Select the ''Scene Tab'' (click on the icon named Scene)" +
                                            "\n4-Press ''F'' button to activate the focus mode." +
                                            "\n5-Change Object Scale and Rotation. (No translation. Keep the Object position to x=0 y=0 z=0" +
                                            "\n6-Press button ''Update and finish modifications'' to save your modifications", MessageType.Info);
                    break;
                case 7:
                    EditorGUILayout.HelpBox("This section allows to setup the Item in the 3D Viewer." +
                                            "\n1-Select your Item in the Hierarchy." +
                                            "\n2-Press button ''test the selected object in the Hierarchy in the 3D Viewer''" +
                                            "\n3-Select the ''Scene Tab'' (click on the icon named Scene)" +
                                            "\n4-Press ''F''  button to activate the focus mode." +
                                            "\n5-Change Object Scale and Rotation. (No translation. Keep the Object position to  x=0 y=0 z=0" +
                                            "\n6-Press button ''Update and finish modifications'' to save your modifications", MessageType.Info);
                    break;
                case 8:
                    EditorGUILayout.HelpBox("If your object is no longuer select, " +
                                            "Press button ''Create an instance of the prefab to test in the 3D Viewer''", MessageType.Warning);
                    break;

                case 9:
                    EditorGUILayout.HelpBox("If your object is no longuer select, " +
                                            "Press button ''test the selected object in the Hierarchy in the 3D Viewer''", MessageType.Warning);
                    break;
            }

		}
	}

	void OnSceneGUI( )
	{
	}
}
#endif