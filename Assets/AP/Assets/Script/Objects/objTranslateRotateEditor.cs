//Description : objTranslateRotate : custom editor for objTranslateRotateEditor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CustomEditor(typeof(objTranslateRotate))]
public class objTranslateRotateEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty			seeInfo;
    SerializedProperty          b_EditrSection4;
	SerializedProperty 			movementType;
	SerializedProperty 			movementAxis;
	SerializedProperty 			constraintsAxisPosition;
	SerializedProperty 			constraintsAxisRotation;
	SerializedProperty 			objIcon;

	SerializedProperty 			translationSpeed;
	SerializedProperty 			targetPos;
	SerializedProperty 			targetEndPosition;
	SerializedProperty 			targetStartPosition;

	SerializedProperty 			objPivot;
	SerializedProperty 			startPosition;
	SerializedProperty 			endPosition;
	SerializedProperty 			_force;
	SerializedProperty 			_targetVelocity;

	SerializedProperty 			a_Open;
	SerializedProperty 			a_OpenVolume;
	SerializedProperty 			a_Close;
	SerializedProperty 			a_CloseVolume;

    SerializedProperty          pivotOffset;

	SerializedProperty 			b_FocusMode_Mobile;
	SerializedProperty 			b_FocusMode_Desktop;

	private bool 				b_addMethods = false;									// use when you press button +
	SerializedProperty 			moreOptionSection1;
	SerializedProperty 			methodsList;
	SerializedProperty 			inventoryIDList;
	SerializedProperty 			diaryIDList;
	SerializedProperty 			a_Locked;
	SerializedProperty 			a_LockedVolume;
	SerializedProperty 			a_Unlocked;
	SerializedProperty 			a_UnlockedVolume;

	SerializedProperty 			feedbackIDList;

	SerializedProperty 			activatedObjectList;

	SerializedProperty			b_VoiceOverActivated;
	SerializedProperty			b_feedbackActivated;

	SerializedProperty 			feedbackIDListUnlock;
	SerializedProperty			diaryIDListUnlock;

	SerializedProperty			b_playVoiceOverOnlyOneTime;

	SerializedProperty			b_VoiceOverActivatedUnlocked;
	SerializedProperty			b_feedbackActivatedUnlock;

	SerializedProperty			groupFollow;
    SerializedProperty          puzzle;

    SerializedProperty          b_FirstTimeStartClose;
    SerializedProperty          b_objStateOpen;


    SerializedProperty          doorOpen;
    SerializedProperty          doorPivotLeft;
    SerializedProperty          openInward;

   

	public 	EditorMethods		editorMethods;											// access the component EditorMethods

	public string[] optionsMovement = new string[]{"Rotation","Translation"};
	public string[] optionsAxis = new string[]{"X","Y","Z"};

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

    public Color _cGreen = Color.green;
    public Color _cGray = new Color(.9f, .9f, .9f, 1);


	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 				= serializedObject.FindProperty ("SeeInspector");
		seeInfo						= serializedObject.FindProperty ("seeInfo");
        b_EditrSection4             = serializedObject.FindProperty("b_EditrSection4");


		movementType				= serializedObject.FindProperty ("movementType");
		movementAxis				= serializedObject.FindProperty ("movementAxis");
		constraintsAxisPosition		= serializedObject.FindProperty ("constraintsAxisPosition");
		constraintsAxisRotation		= serializedObject.FindProperty ("constraintsAxisRotation");
		objIcon						= serializedObject.FindProperty ("objIcon");

		translationSpeed			= serializedObject.FindProperty ("translationSpeed");
		targetPos					= serializedObject.FindProperty ("targetPos");
		targetEndPosition			= serializedObject.FindProperty ("targetEndPosition");
		targetStartPosition			= serializedObject.FindProperty ("targetStartPosition");

		objPivot					= serializedObject.FindProperty ("objPivot");
		startPosition				= serializedObject.FindProperty ("startPosition");
		endPosition					= serializedObject.FindProperty ("endPosition");
		_force						= serializedObject.FindProperty ("_force");
		_targetVelocity				= serializedObject.FindProperty ("_targetVelocity");

		a_Open						= serializedObject.FindProperty ("a_Open");
		a_OpenVolume				= serializedObject.FindProperty ("a_OpenVolume");
    
        pivotOffset                 = serializedObject.FindProperty("pivotOffset");
   
        a_Close                     = serializedObject.FindProperty("a_Close");
        a_CloseVolume               = serializedObject.FindProperty("a_CloseVolume");

		b_FocusMode_Mobile 			= serializedObject.FindProperty ("b_FocusMode_Mobile");
		b_FocusMode_Desktop 		= serializedObject.FindProperty ("b_FocusMode_Desktop");

        puzzle                      = serializedObject.FindProperty("puzzle");

		moreOptionSection1 			= serializedObject.FindProperty ("moreOptionSection1");
		editorMethods 				= new EditorMethods ();
		methodsList					= serializedObject.FindProperty ("methodsList");
		inventoryIDList				= serializedObject.FindProperty ("inventoryIDList");
		a_Unlocked					= serializedObject.FindProperty ("a_Unlocked");
		a_UnlockedVolume				= serializedObject.FindProperty ("a_UnlockedVolume");
		a_Locked					= serializedObject.FindProperty ("a_Locked");
		a_LockedVolume				= serializedObject.FindProperty ("a_LockedVolume");
		diaryIDList					= serializedObject.FindProperty ("diaryIDList");
		feedbackIDList				= serializedObject.FindProperty ("feedbackIDList");
       
       
		diaryIDListUnlock			= serializedObject.FindProperty ("diaryIDListUnlock");
		feedbackIDListUnlock		= serializedObject.FindProperty ("feedbackIDListUnlock");

		activatedObjectList 		= serializedObject.FindProperty ("activatedObjectList");

		b_VoiceOverActivated 		= serializedObject.FindProperty ("b_VoiceOverActivated");
		b_feedbackActivated 		= serializedObject.FindProperty ("b_feedbackActivated");

		b_playVoiceOverOnlyOneTime	= serializedObject.FindProperty ("b_playVoiceOverOnlyOneTime");

		b_VoiceOverActivatedUnlocked	= serializedObject.FindProperty ("b_VoiceOverActivatedUnlocked");
		b_feedbackActivatedUnlock	= serializedObject.FindProperty ("b_feedbackActivatedUnlock");

		groupFollow					= serializedObject.FindProperty ("groupFollow");

        b_FirstTimeStartClose       = serializedObject.FindProperty("b_FirstTimeStartClose");
        b_objStateOpen = serializedObject.FindProperty("b_objStateOpen");

        doorOpen = serializedObject.FindProperty("doorOpen");
        doorPivotLeft = serializedObject.FindProperty("doorPivotLeft");
        openInward = serializedObject.FindProperty("openInward");

		if (EditorPrefs.GetBool("AP_ProSkin") == true)
		{
			float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
			Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
			Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
			Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
			Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .3f));
			Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .3f));
		}
		else
		{
			Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
			Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
			Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
			Tex_04 = MakeTex(2, 2, new Color(1, .3f, 1, .3f));
			Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
		}



		if (inventoryIDList.arraySize > 0) {
            Find_UniqueId_In_inventoryIDList ("wItem");}
		if (feedbackIDList.arraySize > 0) {
            Find_UniqueId_In_FeedbackIDList ("wFeedback");}
		if (diaryIDList.arraySize > 0) {
            Find_UniqueId_In_diaryIDList ("wTextnVoices");}
		if (feedbackIDListUnlock.arraySize > 0) {
            Find_UniqueId_In_FeedbackIDListUnlock ("wFeedback");}
		if (diaryIDListUnlock.arraySize > 0) {
            Find_UniqueId_In_diaryIDListUnlock ("wTextnVoices");}

	}

	TextList loadTextList(string s_Load){
		TextList _textList; 
		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;

		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/" + s_Load + ".asset";
		_textList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;
		return _textList;
	}

	// Update inventoryIDList
	public void Find_UniqueId_In_inventoryIDList(string s_new){
		TextList _textList; 
	
		_textList = loadTextList(s_new);

		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {
				
				if (_textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.inventoryIDList[0].uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					serializedObject.Update ();
					inventoryIDList.GetArrayElementAtIndex(0).FindPropertyRelative("ID").intValue = i;
					serializedObject.ApplyModifiedProperties ();
					break;
				}
			}
		}
	}

	// Update feedbackIDList
	public void Find_UniqueId_In_FeedbackIDList(string s_new){
		TextList _textList; 

		_textList = loadTextList(s_new);

		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {

				if (_textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.feedbackIDList[0].uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					serializedObject.Update ();
					feedbackIDList.GetArrayElementAtIndex(0).FindPropertyRelative("ID").intValue = i;
					serializedObject.ApplyModifiedProperties ();
					break;
				}
			}
		}
	}

	// Update diaryIDList
	public void Find_UniqueId_In_diaryIDList(string s_new){
		TextList _textList; 

		_textList = loadTextList(s_new);

		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {

				if (_textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.diaryIDList[0].uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					serializedObject.Update ();
					diaryIDList.GetArrayElementAtIndex(0).FindPropertyRelative("ID").intValue = i;
					serializedObject.ApplyModifiedProperties ();
					break;
				}
			}
		}
	}


	// Update feedbackIDListUnlock
	public void Find_UniqueId_In_FeedbackIDListUnlock(string s_new){
		TextList _textList; 

		_textList = loadTextList(s_new);

		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {

				if (_textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.feedbackIDListUnlock[0].uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					serializedObject.Update ();
					feedbackIDListUnlock.GetArrayElementAtIndex(0).FindPropertyRelative("ID").intValue = i;
					serializedObject.ApplyModifiedProperties ();
					break;
				}
			}
		}
	}

	// Update diaryIDListUnlock
	public void Find_UniqueId_In_diaryIDListUnlock(string s_new){
		TextList _textList; 

		_textList = loadTextList(s_new);

		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {

				if (_textList.diaryList [0]._languageSlot [i].uniqueItemID == myScript.diaryIDListUnlock[0].uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (myScript, myScript.name);
					serializedObject.Update ();
					diaryIDListUnlock.GetArrayElementAtIndex(0).FindPropertyRelative("ID").intValue = i;
					serializedObject.ApplyModifiedProperties ();
					break;
				}
			}
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

			EditorGUILayout.LabelField ("Show Help Boxes :", GUILayout.Width (120));
			EditorGUILayout.PropertyField(seeInfo, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();
		F_Infos (7);

		GUIStyle style_Yellow_01 		= new GUIStyle();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle();	style_Yellow_Strong.normal.background 	= Tex_02;

		objTranslateRotate myScript = (objTranslateRotate)target; 

		EditorGUILayout.LabelField ("");

        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("IMPORTANT : Objects inside group Grp_Door3DModels need to be tagged layerDoor", MessageType.Warning);
        EditorGUILayout.EndVertical();


        EditorGUILayout.LabelField("");
//--> Section 1-Movement
		MovementType (style_Blue,style_Yellow_Strong);

		EditorGUILayout.LabelField ("");

//--> Section 2-Translation parameters
		EditorGUILayout.BeginVertical (style_Yellow_Strong);
			if (movementType.intValue == 0) {		// Rotation
                rotationOption(myScript);
			} else {								// Translation
				translationOption();
			}
		EditorGUILayout.EndVertical ();

		EditorGUILayout.LabelField ("");

//--> Section 3-Audio Parameteres
		EditorGUILayout.BeginVertical (style_Blue);
			audioOptions ();
		EditorGUILayout.EndVertical ();

//--> Section 4-Focus Mode parameters
        EditorGUILayout.LabelField("");
        if(!b_EditrSection4.boolValue){
            
            EditorGUILayout.BeginVertical(style_Yellow_Strong);
            focusMode(myScript);
            EditorGUILayout.EndVertical();  
        }
        else{
            EditorGUILayout.BeginVertical(style_Yellow_Strong);
            EditorGUILayout.LabelField("4-Focus : Not available for door", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();  
           
        }
		

//--> Section 5-Lock Options
		EditorGUILayout.LabelField ("");
		LockOptions (style_Blue, style_Purple);

//--> Section 6-Unlock Options
		EditorGUILayout.LabelField ("");
		UnlockOptions (style_Yellow_01, style_Purple);


		serializedObject.ApplyModifiedProperties ();
	}

//--> Section 1-Movement
	void MovementType(GUIStyle style_Blue,GUIStyle style_Yellow_Strong){
		EditorGUILayout.BeginVertical (style_Blue);
			
			EditorGUILayout.LabelField ("1-Movement",EditorStyles.boldLabel);
			F_Infos (0);
			EditorGUI.BeginChangeCheck ();

	//-> Select Movement
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Select Movement type : ", GUILayout.Width (120));
				movementType.intValue = EditorGUILayout.Popup (movementType.intValue, optionsMovement);
			EditorGUILayout.EndHorizontal ();

	//-> Select Axis
			if (movementType.intValue == 1){
				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Select Axis : ", GUILayout.Width (120));
					movementAxis.intValue = EditorGUILayout.Popup (movementAxis.intValue, optionsAxis);
				EditorGUILayout.EndHorizontal ();

				if (EditorGUI.EndChangeCheck ()) {
					updateConstraints ();
				}
			}
			EditorGUILayout.LabelField ("");

	//-> Select Icon position
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Icon Position : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (objIcon, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical();
	}

//--> Section 4-Focus Mode parameters
	void focusMode(objTranslateRotate myScript){
		EditorGUILayout.LabelField ("4-Focus Mode parameters",EditorStyles.boldLabel);

        //if(puzzle.objectReferenceValue != null)
        //EditorGUILayout.HelpBox("VERY IMPORTANT : If Drawer, wardrobe or door are use in association with a puzzle : Check Focus Available for both Desktop and Mobile.", MessageType.Warning);

		F_Infos (4);

		if (!myScript.gameObject.GetComponent<focusCamEffect> ()) {

			if (GUILayout.Button ("Activate Focus Mode")) {
				Undo.RegisterFullObjectHierarchyUndo (myScript.gameObject,myScript.gameObject.name);
				Undo.AddComponent (myScript.gameObject, typeof(focusCamEffect));

				string objectPath = "Assets/AP/Assets/Prefab/Other/CamPosition.prefab";
				GameObject refCamPosition = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as GameObject;

				GameObject newCamPosition = Instantiate (refCamPosition, myScript.gameObject.GetComponent<focusCamEffect> ().transform);
				Undo.RegisterCreatedObjectUndo (newCamPosition, newCamPosition.name);

				newCamPosition.name = "CamPosition";

				myScript.gameObject.GetComponent<focusCamEffect> ().targetFocusCamera = newCamPosition.transform;
			}

		} else {
			//-> Choose if Focus mode available on mobile and desktop
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Focus available for : ", GUILayout.Width (120));
			EditorGUILayout.LabelField ("Desktop : ", GUILayout.Width (60));
			EditorGUILayout.PropertyField (b_FocusMode_Desktop, new GUIContent (""), GUILayout.Width (30));
		//	EditorGUILayout.LabelField ("Mobile : ", GUILayout.Width (60));
		//	EditorGUILayout.PropertyField (b_FocusMode_Mobile, new GUIContent (""), GUILayout.Width (30));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.LabelField ("");


			SerializedObject serializedObject2 = new UnityEditor.SerializedObject (myScript.gameObject.GetComponent<focusCamEffect> ());
			serializedObject2.Update ();
			SerializedProperty targetFocusCamera = serializedObject2.FindProperty ("targetFocusCamera");

			//-> Choose focus target
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Focus Target : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (targetFocusCamera, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

            //-> Use a puzzle inside a door or drawer using a focus
            if(b_FocusMode_Desktop.boolValue || b_FocusMode_Mobile.boolValue){
                EditorGUILayout.HelpBox("IMPORTANT : If you want to use a puzzle inside a drawer or a door using a focus. " +
                                   "You need to drag and drop the puzzle inside the next slot.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();


                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Puzzle : ", GUILayout.Width(120));
                EditorGUILayout.PropertyField(puzzle, new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    if(puzzle.objectReferenceValue != null){
                        b_FocusMode_Desktop.boolValue = true;
                        b_FocusMode_Mobile.boolValue = true;
                    }
                   
                }
                EditorGUILayout.EndHorizontal();
            }
           

			serializedObject2.ApplyModifiedProperties ();
		}
	}

//--> Section 3-Audio parameters
	void audioOptions(){
		EditorGUILayout.LabelField ("3-Audio parameters",EditorStyles.boldLabel);
		F_Infos (3);
	//-> Open sound
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Open : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (a_Open, new GUIContent (""));
			a_OpenVolume.floatValue = EditorGUILayout.Slider(a_OpenVolume.floatValue,0, 1); 
		EditorGUILayout.EndHorizontal ();

	//-> Close sound
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Close : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (a_Close, new GUIContent (""));
			a_CloseVolume.floatValue = EditorGUILayout.Slider(a_CloseVolume.floatValue,0, 1); 
		EditorGUILayout.EndHorizontal ();

	}

    //--> Section 2 (Case Rotation)
    void rotationOption(objTranslateRotate myScript)
    {

        EditorGUILayout.LabelField("2-Rotation parameters", EditorStyles.boldLabel);
        F_Infos(2);

     

        //-> Pivot
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pivot : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(objPivot, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        //-> groupFollow
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Group Follow : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(groupFollow, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        //-> Pivot Left or right
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pivot Left : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(doorPivotLeft, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();

        //-> The door Open Inward

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Inward : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(openInward, new GUIContent(""), GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();



        if (EditorGUI.EndChangeCheck())
        {
            if(doorPivotLeft.boolValue && !openInward.boolValue
              ||
               !doorPivotLeft.boolValue && openInward.boolValue){
                SerializedObject serializedObject10 = new UnityEditor.SerializedObject(myScript.GrpDoor.transform);
                serializedObject10.Update();
                SerializedProperty tmpPosition = serializedObject10.FindProperty("m_LocalPosition");

                tmpPosition.vector3Value = new Vector3(-pivotOffset.floatValue, 0, 0); 
                serializedObject10.ApplyModifiedProperties();

                SerializedObject serializedObject11 = new UnityEditor.SerializedObject(myScript.GrpFollow.transform);
                serializedObject11.Update();
                SerializedProperty tmpPosition2 = serializedObject11.FindProperty("m_LocalPosition");

                tmpPosition2.vector3Value = new Vector3(-pivotOffset.floatValue, 0, 0);
                serializedObject11.ApplyModifiedProperties();
            }

            if (doorPivotLeft.boolValue && openInward.boolValue
                ||
               !doorPivotLeft.boolValue && !openInward.boolValue)
            {
                SerializedObject serializedObject10 = new UnityEditor.SerializedObject(myScript.GrpDoor.transform);
                serializedObject10.Update();
                SerializedProperty tmpPosition = serializedObject10.FindProperty("m_LocalPosition");

                tmpPosition.vector3Value = new Vector3(pivotOffset.floatValue, 0, 0);
                serializedObject10.ApplyModifiedProperties();

                SerializedObject serializedObject11 = new UnityEditor.SerializedObject(myScript.GrpFollow.transform);
                serializedObject11.Update();
                SerializedProperty tmpPosition2 = serializedObject11.FindProperty("m_LocalPosition");

                tmpPosition2.vector3Value = new Vector3(pivotOffset.floatValue, 0, 0);
                serializedObject11.ApplyModifiedProperties();
            }


        }

        //-> Start position
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Close Position : ", GUILayout.Width(120));
        EditorGUILayout.LabelField(startPosition.floatValue.ToString(), GUILayout.Width(120));
        //EditorGUILayout.PropertyField (startPosition, new GUIContent (""), GUILayout.Width (120));
        if(!b_objStateOpen.boolValue)
            GUI.backgroundColor = _cGreen;
        if (GUILayout.Button("Set Position", GUILayout.Width(100)))
        {
            InitPosition("StartPosition");
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = _cGray;
        //-> End position
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Open Pos (0 to 150) : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(endPosition, new GUIContent(""), GUILayout.Width(120));

        if (b_objStateOpen.boolValue)
            GUI.backgroundColor = _cGreen;
        if (GUILayout.Button("Set Position", GUILayout.Width(100)))
        {
            InitPosition("EndPosition");
        }
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = _cGray;

        if (EditorGUI.EndChangeCheck())
        {
            if (endPosition.floatValue < 0)
                endPosition.floatValue = 0; 

            if (endPosition.floatValue > 150)
                endPosition.floatValue = 150; 
        }

        //-> Force apply
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Force : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(_force, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        //-> Velocity apply
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Velocity : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(_targetVelocity, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

      
	}


//--> Section 2 (Case Translation)
	void translationOption(){

		EditorGUILayout.LabelField ("2-Translation parameters",EditorStyles.boldLabel);
		F_Infos (1);
	//-> translation Speed
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Speed : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (translationSpeed, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();

	//-> translation pivot
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Pivot : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (targetPos, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();


	//-> translation groupFollow
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Group Follow : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (groupFollow, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();


	//-> translation start position
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Close Position : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (targetStartPosition, new GUIContent (""), GUILayout.Width (120));
			if (GUILayout.Button ("Set Position", GUILayout.Width (100))) {
				InitPosition ("StartPosition");
			}
		EditorGUILayout.EndHorizontal ();

	//-> translation end position
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Open Position : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (targetEndPosition, new GUIContent (""), GUILayout.Width (120));
			if (GUILayout.Button ("Set Position", GUILayout.Width (100))) {
				InitPosition ("EndPosition");
			}
		EditorGUILayout.EndHorizontal ();


	}


	void updateConstraints(){
		if (movementType.intValue == 0) {	// Rotation
			if (movementAxis.intValue == 0) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = true;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = false;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = true;
			}
			if (movementAxis.intValue == 1) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = true;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = false;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = true;

			}
			if (movementAxis.intValue == 2) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = true;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = false;
			}

		} else {							// Transation
			if (movementAxis.intValue == 0) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = false;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = true;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = true;
			}
			if (movementAxis.intValue == 1) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = false;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = true;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = true;

			}
			if (movementAxis.intValue == 2) {
				constraintsAxisPosition.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisPosition.GetArrayElementAtIndex (2).boolValue = false;

				constraintsAxisRotation.GetArrayElementAtIndex (0).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (1).boolValue = true;
				constraintsAxisRotation.GetArrayElementAtIndex (2).boolValue = true;
			}
		}
	}

//--> init object position when the movement is ended
	private void InitPosition(string sPos){
		objTranslateRotate myScript = (objTranslateRotate)target; 

		if (sPos == "EndPosition") {
			if (myScript.movementType == 0) {
				Undo.RegisterFullObjectHierarchyUndo (myScript.objPivot.gameObject, myScript.objPivot.name);
                if (myScript.movementAxis == 0){
                    if (doorPivotLeft.boolValue){
                        if (openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(-myScript.endPosition, myScript.objPivot.localEulerAngles.y, myScript.objPivot.localEulerAngles.z);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.endPosition, myScript.objPivot.localEulerAngles.y, myScript.objPivot.localEulerAngles.z);
                    }

                    if (!doorPivotLeft.boolValue)
                    {
                        if (!openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(-myScript.endPosition, myScript.objPivot.localEulerAngles.y, myScript.objPivot.localEulerAngles.z);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.endPosition, myScript.objPivot.localEulerAngles.y, myScript.objPivot.localEulerAngles.z);
                    }
                      
                }
                    
                if (myScript.movementAxis == 1){
                    if (doorPivotLeft.boolValue)
                    {
                        if (openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, -myScript.endPosition, myScript.objPivot.localEulerAngles.z);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.endPosition, myScript.objPivot.localEulerAngles.z);
                    }

                    if (!doorPivotLeft.boolValue)
                    {
                        if (!openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, -myScript.endPosition, myScript.objPivot.localEulerAngles.z);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.endPosition, myScript.objPivot.localEulerAngles.z);
                    }
                }
               
                if (myScript.movementAxis == 2){
                    if (doorPivotLeft.boolValue)
                    {
                        if (!openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.objPivot.localEulerAngles.y, -myScript.endPosition);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.objPivot.localEulerAngles.y, myScript.endPosition);
                    }

                    if (!doorPivotLeft.boolValue)
                    {
                        if (openInward.boolValue)
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.objPivot.localEulerAngles.y, -myScript.endPosition);
                        else
                            myScript.objPivot.localEulerAngles = new Vector3(myScript.objPivot.localEulerAngles.x, myScript.objPivot.localEulerAngles.y, myScript.endPosition);
                    }
                }
             

				if (myScript.groupFollow) {
					Undo.RegisterFullObjectHierarchyUndo (myScript.groupFollow, myScript.groupFollow.name);
					myScript.groupFollow.transform.localEulerAngles = myScript.objPivot.localEulerAngles;
				}

                b_FirstTimeStartClose.boolValue = false;
                b_objStateOpen.boolValue = true;
			}
			if (myScript.movementType == 1) {
				Undo.RegisterFullObjectHierarchyUndo (myScript.targetPos, myScript.targetPos.name);
				myScript.targetPos.localPosition = myScript.targetEndPosition.localPosition;

                if (myScript.groupFollow)
                {
                    Undo.RegisterFullObjectHierarchyUndo(myScript.groupFollow, myScript.groupFollow.name);
                    myScript.groupFollow.transform.localPosition = myScript.targetEndPosition.localPosition;
                }

                b_FirstTimeStartClose.boolValue = false;
                b_objStateOpen.boolValue = true;
			}
		} else if (sPos == "StartPosition") {
			if (myScript.movementType == 0) {
				Undo.RegisterFullObjectHierarchyUndo (myScript.objPivot.gameObject, myScript.objPivot.name);
				if (myScript.movementAxis == 0) {
					myScript.objPivot.localEulerAngles = new Vector3 (myScript.startPosition, myScript.objPivot.localEulerAngles.y, myScript.objPivot.localEulerAngles.z);

				}
				if (myScript.movementAxis == 1) {
					myScript.objPivot.localEulerAngles = new Vector3 (myScript.objPivot.localEulerAngles.x, myScript.startPosition, myScript.objPivot.localEulerAngles.z);
				}
				if (myScript.movementAxis == 2) {
					myScript.objPivot.localEulerAngles = new Vector3 (myScript.objPivot.localEulerAngles.x, myScript.objPivot.localEulerAngles.y, myScript.startPosition);
				}
				if (myScript.groupFollow) {
					Undo.RegisterFullObjectHierarchyUndo (myScript.groupFollow, myScript.groupFollow.name);
					myScript.groupFollow.transform.localEulerAngles = myScript.objPivot.localEulerAngles;
				}

                b_FirstTimeStartClose.boolValue = true;
                b_objStateOpen.boolValue = false;
			}
			if (myScript.movementType == 1) {
				Undo.RegisterFullObjectHierarchyUndo (myScript.targetPos, myScript.targetPos.name);
				myScript.targetPos.localPosition = myScript.targetStartPosition.localPosition;

                if (myScript.groupFollow)
                {
                    Undo.RegisterFullObjectHierarchyUndo(myScript.groupFollow, myScript.groupFollow.name);
                    myScript.groupFollow.transform.localPosition = myScript.targetStartPosition.localPosition;
                }

                b_FirstTimeStartClose.boolValue = true;
                b_objStateOpen.boolValue = false;
			}

		}

       

        if (sPos == "StartPosition")
            doorOpen.boolValue = false;
        else
            doorOpen.boolValue = true;

	}


//--> Section 5-Lock Options
	private void LockOptions(GUIStyle style_Blue,GUIStyle style_Purple){
		
		objTranslateRotate myScript = (objTranslateRotate)target; 

		EditorGUILayout.BeginVertical (style_Blue);

		EditorGUILayout.LabelField ("5-Lock Options",EditorStyles.boldLabel);
		F_Infos (5);
	//-> Play a sound
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Sound : ", GUILayout.Width (120));
			EditorGUILayout.PropertyField (a_Locked, new GUIContent (""));
			a_LockedVolume.floatValue = EditorGUILayout.Slider(a_LockedVolume.floatValue,0, 1); 

		EditorGUILayout.EndHorizontal ();


	//--> Play voice over + Subtitle
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Voice : ", GUILayout.Width (120));

			EditorGUILayout.PropertyField (b_VoiceOverActivated, new GUIContent (""), GUILayout.Width (30));

			for (var i = 0; i < diaryIDList.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("ID :" , GUILayout.Width (30));

				EditorGUI.BeginChangeCheck ();

				EditorGUILayout.PropertyField (diaryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent (""), GUILayout.Width (30));

				if (EditorGUI.EndChangeCheck ()) {
					updateDiaryVoice (i,diaryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue,true);
				}
		

				EditorGUILayout.LabelField ("Play Once :" , GUILayout.Width (70));
				EditorGUILayout.PropertyField (b_playVoiceOverOnlyOneTime, new GUIContent (""), GUILayout.Width (30));

                if (GUILayout.Button("Open window : w_textnVoice", GUILayout.Width(200)))
                {
                    EditorWindow.GetWindow(typeof(w_TextnVoice));
                }
                    
				EditorGUILayout.EndHorizontal ();
			}
		EditorGUILayout.EndHorizontal ();


	//--> Display feedback
		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Feedback text : ", GUILayout.Width (120));

			EditorGUILayout.PropertyField (b_feedbackActivated, new GUIContent (""), GUILayout.Width (30));

			for (var i = 0; i < feedbackIDList.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				//EditorGUILayout.LabelField (i + ":" , GUILayout.Width (20));
				EditorGUILayout.LabelField ("ID :" , GUILayout.Width (30));

				EditorGUI.BeginChangeCheck ();

				EditorGUILayout.PropertyField (feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent (""), GUILayout.Width (30));

				if (EditorGUI.EndChangeCheck ()) {
					updateFeedback (i,feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue,true);
				}

                EditorGUILayout.LabelField("", GUILayout.Width(104));
                if (GUILayout.Button("Open window : w_Feedback", GUILayout.Width(200)))
                {
                    EditorWindow.GetWindow(typeof(w_Feedback));
                }

				EditorGUILayout.EndHorizontal ();
			}


		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.LabelField ("");


	//--> Player need an object in the Inventory
		EditorGUILayout.BeginVertical(style_Purple);

		EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Player needs an object in the Inventory", GUILayout.Width (230));
			if(inventoryIDList.arraySize == 0)
			if (GUILayout.Button ("+", GUILayout.Width (30))) {
				AddInventoryObjectToList (inventoryIDList);
			}
		EditorGUILayout.EndHorizontal ();


		for (var i = 0; i < inventoryIDList.arraySize; i++) {
			EditorGUILayout.BeginHorizontal ();
			//EditorGUILayout.LabelField (i + ":" , GUILayout.Width (20));
			EditorGUILayout.LabelField ("Object ID :" , GUILayout.Width (70));

			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.PropertyField (inventoryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent (""), GUILayout.Width (30));

			if (EditorGUI.EndChangeCheck ()) {
				updateInventoryObject (i,inventoryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue);
			}


	//-> Show Unique ID 
			//EditorGUILayout.LabelField ("unique ID:" , GUILayout.Width (70));
			//EditorGUILayout.PropertyField (inventoryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID"), new GUIContent (""), GUILayout.Width (30));
			EditorGUILayout.LabelField ("" , GUILayout.Width (30));
			if (GUILayout.Button ("-", GUILayout.Width (30))) {
				RemoveInventoryObjectToList (i,inventoryIDList);
			}
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField ("");

	//-> Check If Objects in a List are all activated
		EditorGUILayout.BeginVertical(style_Purple);
			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label("Check if a puzzle is complete", GUILayout.Width (230));

				if (GUILayout.Button ("+", GUILayout.Width (30))) {
					AddInventoryObjectToList (activatedObjectList);
				}
			EditorGUILayout.EndHorizontal ();


			for (var i = 0; i < activatedObjectList.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (i + ":" , GUILayout.Width (20));
				EditorGUILayout.PropertyField (activatedObjectList.GetArrayElementAtIndex(i), new GUIContent (""), GUILayout.Width (150));

				if (GUILayout.Button ("-", GUILayout.Width (30))) {
					RemoveInventoryObjectToList (i,activatedObjectList);
				}
				EditorGUILayout.EndHorizontal ();
			}
		EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField ("");




	//-> Custom methods
		EditorGUILayout.BeginVertical(style_Purple);
			EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Custom Methods", GUILayout.Width (100));
				EditorGUILayout.PropertyField (moreOptionSection1, new GUIContent (""), GUILayout.Width (20));
			EditorGUILayout.EndHorizontal ();
	
			if (moreOptionSection1.boolValue) {
				EditorGUILayout.HelpBox ("(Only bool Methods are allowed. " +
				"Other methods are ignored. " +
				"\nIf all the methods return true the puzzle is activated)", MessageType.Info);
				if (GUILayout.Button ("+", GUILayout.Width (30))) {
					b_addMethods = true;
				}

				editorMethods.DisplayMethodsOnEditor (myScript.methodsList, methodsList, style_Blue);

				if (b_addMethods) {
					editorMethods.AddMethodsToList (methodsList);
					b_addMethods = false;
				}
			}
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical ();

	}

//--> Section 6-Unlock Options
	private void UnlockOptions(GUIStyle style_Yellow_01,GUIStyle style_Purple){

		objTranslateRotate myScript = (objTranslateRotate)target; 

		EditorGUILayout.BeginVertical (style_Yellow_01);

			EditorGUILayout.LabelField ("6-Unlock Options",EditorStyles.boldLabel);
			F_Infos (6);
	//-> Play sound
			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Sound : ", GUILayout.Width (120));
				EditorGUILayout.PropertyField (a_Unlocked, new GUIContent (""));
                a_UnlockedVolume.floatValue = EditorGUILayout.Slider(a_UnlockedVolume.floatValue,0, 1); 
			EditorGUILayout.EndHorizontal ();


	//-> Play voice over + Subtitle
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Voice : ", GUILayout.Width (120));

			EditorGUILayout.PropertyField (b_VoiceOverActivatedUnlocked, new GUIContent (""), GUILayout.Width (30));

			for (var i = 0; i < diaryIDListUnlock.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				//EditorGUILayout.LabelField (i + ":" , GUILayout.Width (20));
				EditorGUILayout.LabelField ("ID :" , GUILayout.Width (30));

				EditorGUI.BeginChangeCheck ();

				EditorGUILayout.PropertyField (diaryIDListUnlock.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent (""), GUILayout.Width (30));

				if (EditorGUI.EndChangeCheck ()) {
					updateDiaryVoice (i,diaryIDListUnlock.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue,false);
				}
		

                if (GUILayout.Button("Open window : w_textnVoice", GUILayout.Width(200)))
                {
                    EditorWindow.GetWindow(typeof(w_TextnVoice));
                }

				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndHorizontal ();


	//-> Display feedback
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Feedback text : ", GUILayout.Width (120));

			EditorGUILayout.PropertyField (b_feedbackActivatedUnlock, new GUIContent (""), GUILayout.Width (30));

			for (var i = 0; i < feedbackIDListUnlock.arraySize; i++) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("ID :" , GUILayout.Width (30));

				EditorGUI.BeginChangeCheck ();

				EditorGUILayout.PropertyField (feedbackIDListUnlock.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent (""), GUILayout.Width (30));

				if (EditorGUI.EndChangeCheck ()) {
					updateFeedback (i,feedbackIDListUnlock.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue,false);
				}

                if (GUILayout.Button("Open window : w_Feedback", GUILayout.Width(200)))
                {
                    EditorWindow.GetWindow(typeof(w_Feedback));
                }

				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical ();

		EditorGUILayout.LabelField ("");
	}

//--> Add object in inventoryObjList list
	public void AddInventoryObjectToList(SerializedProperty inventoryObjList){
		int index = 0;
		if (inventoryObjList.arraySize > 0)
			index = methodsList.arraySize;

		inventoryObjList.InsertArrayElementAtIndex (index);
	}

//--> Remove an object from inventoryObjList list
	public void RemoveInventoryObjectToList(int value, SerializedProperty inventoryObjList){
		inventoryObjList.DeleteArrayElementAtIndex (value);
	}

//--> Update inventory ID using unique ID
	private void updateInventoryObject (int i,int value){
		GameObject obj = GameObject.Find ("ingameGlobalManager");
		TextList currentInventory 	= Resources.Load(obj.GetComponent<ingameGlobalManager> ().dataFolder.currentDatasProjectFolder + "/TextList/wItem") as TextList;

		if (currentInventory) {
			inventoryIDList.GetArrayElementAtIndex (i).FindPropertyRelative ("uniqueID").intValue 
			= currentInventory.diaryList [0]._languageSlot [value].uniqueItemID;
		}
	}

//--> Update diary ID using unique ID
	private void updateDiaryVoice (int i,int value,bool lockVoice){
		GameObject obj = GameObject.Find ("ingameGlobalManager");
		TextList	currentDiary 		= Resources.Load(obj.GetComponent<ingameGlobalManager> ().dataFolder.currentDatasProjectFolder + "/TextList/wTextnVoices") as TextList;

		if (currentDiary) {
			if (lockVoice) {
				diaryIDList.GetArrayElementAtIndex (i).FindPropertyRelative ("uniqueID").intValue 
				= currentDiary.diaryList [0]._languageSlot [value].uniqueItemID;
			} else {
				diaryIDListUnlock.GetArrayElementAtIndex (i).FindPropertyRelative ("uniqueID").intValue 
				= currentDiary.diaryList [0]._languageSlot [value].uniqueItemID;
			}
		}
	}


//--> Update feedback ID using unique ID
	private void updateFeedback (int i,int value,bool lockFeedback){
		GameObject obj = GameObject.Find ("ingameGlobalManager");
		TextList	currentFeebback 		= Resources.Load(obj.GetComponent<ingameGlobalManager> ().dataFolder.currentDatasProjectFolder + "/TextList/wFeedback") as TextList;

		if (currentFeebback) {
			if (lockFeedback) {
				feedbackIDList.GetArrayElementAtIndex (i).FindPropertyRelative ("uniqueID").intValue 
				= currentFeebback.diaryList [0]._languageSlot [value].uniqueItemID;
			} else {
				feedbackIDListUnlock.GetArrayElementAtIndex (i).FindPropertyRelative ("uniqueID").intValue 
				= currentFeebback.diaryList [0]._languageSlot [value].uniqueItemID;
			}
		}
	}


	private void F_Infos(int value){
		if(seeInfo.boolValue){
			switch (value)
			{
            case 8:
               EditorGUILayout.HelpBox("When the object is movement you deactivate or keep activated the interactive UI Buttons", MessageType.None);
                    break;
			case 7:
				EditorGUILayout.HelpBox ("REMINDER :" +
					"\n" +
					"\n1-Inside the object ''pivot'' only add an object that don't need to be raycast (example : do not put Icon object, item objects...). " +
					"\n" +
					"\n2-Put inside ''Grp_Follow'' objects that need to move with drawer,door ...." +
					"\n" +
					"\n3-Put inside ''Grp_DontFollow'' objects that DONT need to move with drawer,door ....." +
					"\n" +
					"\n4-Inside ''pivot'' don't add object with Convex Mesh Collider.", MessageType.None);
				break;
			case 6:
				EditorGUILayout.HelpBox ("1-Unlock Sound : Drag and Drop audio clip and choose volume." +
					"\n2-If toggle = true a voice is played using the selected ID. " +
					"\nIMPORTANT you need to use a valid ID (ID need to exist in the diary Tab)." +
					"\n3-If toggle = true a text is displayed using the selected ID." +
					"\nIMPORTANT you need to use a valid ID (ID need to exist in the inventory Tab).", MessageType.Info);
				break;
			case 5:
				EditorGUILayout.HelpBox ("1-Lock Sound : Drag and Drop audio clip and choose volume." +
					"\n2-If toggle = true a voice is played using the selected ID. " +
					"\nIMPORTANT you need to use a valid ID (ID need to exist in the diary Tab)." +
					"\nIf Play Once = true the voice is played only the first time the player try to unlock the object" +
					"\n3-If toggle = true a text is displayed using the selected ID." +
					"\nIMPORTANT you need to use a valid ID (ID need to exist in the inventory Tab)." +
					"\n4-You could add object that needs to be in the player inventory (Use ID)" +
					"\n5-You cold drag and drop object that need to enabled in scene view to unlock the object." +
					"\n6-You could add custom methods (boolean methods) that need to return true to unlock object.", MessageType.Info);
				break;
			case 4:
				EditorGUILayout.HelpBox ("1-Choose if the focus mode is available for Mobile and or desktop." +
					"\n2-Drag and drop an object that represent the focus target position." +
					"\n3-Press button ''Setup Camera Position'' to update the camera focus position" +
					"\nAfter pressing the button, a new camera is created in scene view and your focus target position is auto-selected in the hierarchy." +
					"\nModify the position and the rotation." +
					"\nThen select in the Hierarchy, the group that containt the script objTranslateRotate.cs (this Object) and press ''Destroy Test Camera'' button section 4 ", MessageType.Info);
				break;
			case 3:
				EditorGUILayout.HelpBox ("1-Open Sound : Drag and Drop audio clip and choose volume." +
					"\n2-Open Sound : Drag and Drop audio clip and choose volume.", MessageType.Info);
				break;
			case 2:
				EditorGUILayout.HelpBox ("1-Drag and drop the object you want to rotate." +
					"\n2-Choose start Rotation -180 to 0." +
					"\n3-Choose end Rotation 0 to 360." +
					"\n4-Choose Rotation force and velocity.", MessageType.Info);
				break;
			case 1:
				EditorGUILayout.HelpBox ("1-Choose translation speed." +
					"\n2-Drag and drop the object you want to move." +
					"\n2-Drag and drop the object use to put inside objects that need to move with pivot." +
					"\n4-Drag and drop an object that represent the start position." +
					"\n5-Drag and drop an object that represent the final position.", MessageType.Info);
				break;
			case 0:
				EditorGUILayout.HelpBox ("1-Choose rotation or translation." +
				"\n2-Choose Axis (only one axis)." +
				"\n3-Drag and drop a gameObject that represent the UI Icon position.", MessageType.Info);
				break;
			}
		}
	}

	void OnSceneGUI( )
	{
	}
}
#endif