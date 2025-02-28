// Description : w_ObjCreator.cs :  Allow to create ready to use door, drawer, trigger ...
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


public class w_ObjCreator : EditorWindow
{
	private Vector2 				scrollPosAll;
    private Vector2                 scrollPosSection;
    SerializedObject                serializedObject2;
    SerializedObject                serializedObject3;
    SerializedProperty              helpBoxEditor;
    SerializedProperty              currentTypeSelected;
    SerializedProperty              listOfTexture2DDoor;
    SerializedProperty              listOfTexture2DDrawer;
    SerializedProperty              listOfTexture2DWardrobe;
    SerializedProperty              listOfTexture2DActionTrigger;
    SerializedProperty              currentItemSubType;
    SerializedProperty              currentDoorSubType;
    SerializedProperty              currentDrawerSubType;
    SerializedProperty              currentWardrobeSubType;
    SerializedProperty              currentActionTriggerSubType;

    SerializedProperty              listOfPuzzles;
    SerializedProperty              clueSystem;
    SerializedProperty              listOfObjsDoor;
    SerializedProperty              listOfObjsDrawer;
    SerializedProperty              listOfObjsWardrobe;
    SerializedProperty              listOfObjsActionTrigger;
    SerializedProperty              listOfObjsCustomAction;

    SerializedProperty              b_TextProperties;
    SerializedProperty              b_VoiceProperties;
    SerializedProperty              b_Collider;
    SerializedProperty              b_SaveSystem;

    SerializedProperty              diaryList;
    SerializedProperty              diaryListVoice;

    SerializedProperty              b_ActivatedTheFirstTime;


	public EditorManipulateTextList manipulateTextList;
    public datasWindowReadyToUse    _windowReadyToUseDatas;

    public bool                     b_ProjectManagerAssetExist = true;

    public String[]                 arrTypeOfPrefab = new string[] {    "01 Puzzles",
                                                                        "02 Items (3D Viewer)",
                                                                        "03 Items (Text Viewer)",
                                                                        "04 Door", 
                                                                        "05 Drawer", 
                                                                        "06 Wardrobe", 
                                                                        "07 Action Trigger",
                                                                        "08 UI Text",
                                                                        "09 Reset Object",
                                                                        "10 ObjIsActivated",
                                                                        "11 Focus Info",
	                                                                    "12 Custom Actions"};


    public String[] arPuzzleName = new string[] {    "01-Sliding",
                                                     "02-Digicode",
                                                     "03-Lever",
                                                     "04-Cylinder",
                                                     "05-Pipe",
                                                     "06-Gear",
                                                     "07-Logic"};

    // public int                      currentTypeSelected = 0;

    public bool b_AvailableDIaryOrInventory = false;
    public bool b_PlayVoice = false;
    public string s_ObjectName = "Object Name";
    public string s_ObjectVoiceName = "Voice Name";

	// Add menu item named "Test Mode Panel" to the Window menu
    [MenuItem("Tools/AP/Object Creator (w_ObjCreator)")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_ObjCreator));
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

	public string[] 			listItemType = new string[]{};

	public List<string> 		_test = new List<string>(); 
	public int 					page = 0;
	public int 					numberOfIndexInAPage = 50;
	public int 					seachSpecificID = 0;

    public Color 				_cGreen = Color.green;
	public Color 				_cGray = new Color(.9f,.9f,.9f,1);


	public Texture2D eye;
    public Texture2D currentItemDisplay;
    public int intcurrentItemDisplay = 0;


	void OnEnable () {
		manipulateTextList = new EditorManipulateTextList ();
        //createObject = new createObjectVariousMethods();

		_MakeTexture ();

        string objectPath = "Assets/AP/Assets/Datas/wObjCreator.asset";
        _windowReadyToUseDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as datasWindowReadyToUse;
        if (_windowReadyToUseDatas)
        {
            serializedObject2   = new UnityEditor.SerializedObject(_windowReadyToUseDatas);
            helpBoxEditor       = serializedObject2.FindProperty("helpBoxEditor");
            currentTypeSelected = serializedObject2.FindProperty("currentTypeSelected");

            listOfTexture2DDoor = serializedObject2.FindProperty("listOfTexture2DDoor");
            listOfTexture2DDrawer = serializedObject2.FindProperty("listOfTexture2DDrawer");
            listOfTexture2DWardrobe = serializedObject2.FindProperty("listOfTexture2DWardrobe");
            listOfTexture2DActionTrigger = serializedObject2.FindProperty("listOfTexture2DActionTrigger");
            currentDoorSubType = serializedObject2.FindProperty("currentDoorSubType");

            currentItemSubType = serializedObject2.FindProperty("currentItemSubType");
            currentDrawerSubType = serializedObject2.FindProperty("currentDrawerSubType");
            currentWardrobeSubType = serializedObject2.FindProperty("currentWardrobeSubType");
            currentActionTriggerSubType = serializedObject2.FindProperty("currentActionTriggerSubType");


            listOfPuzzles = serializedObject2.FindProperty("listOfPuzzles");
            clueSystem = serializedObject2.FindProperty("clueSystem");
            listOfObjsDoor = serializedObject2.FindProperty("listOfObjsDoor");
            listOfObjsDrawer = serializedObject2.FindProperty("listOfObjsDrawer");
            listOfObjsWardrobe = serializedObject2.FindProperty("listOfObjsWardrobe");
            listOfObjsActionTrigger = serializedObject2.FindProperty("listOfObjsActionTrigger");

            listOfObjsCustomAction = serializedObject2.FindProperty("listOfObjsCustomAction");

            b_TextProperties = serializedObject2.FindProperty("b_TextProperties");
            b_VoiceProperties = serializedObject2.FindProperty("b_VoiceProperties");
            b_Collider = serializedObject2.FindProperty("b_Collider");
            b_SaveSystem = serializedObject2.FindProperty("b_SaveSystem");

            b_ActivatedTheFirstTime = serializedObject2.FindProperty("b_ActivatedTheFirstTime");

        }
        else
        {
            b_ProjectManagerAssetExist = false;
        }

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
        EditorGUILayout.HelpBox ("Window Tab : Create Objects :",MessageType.Info);
		EditorGUILayout.EndVertical ();

        // --> Display data
        EditorGUILayout.BeginHorizontal();
        _windowReadyToUseDatas = EditorGUILayout.ObjectField(_windowReadyToUseDatas, typeof(UnityEngine.Object), true) as datasWindowReadyToUse;
        EditorGUILayout.EndHorizontal();

        if (_windowReadyToUseDatas != null)
        {
            serializedObject2.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("See HelpBoxes :", GUILayout.Width(85));
            EditorGUILayout.PropertyField(helpBoxEditor, new GUIContent(""), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            _helpBox(16);

            displayALLTypeOfPrefab(style_Purple,style_Yellow_01);

            displayOptions(style_Orange,style_Yellow_01);

            serializedObject2.ApplyModifiedProperties();

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("");
        }
		
        EditorGUILayout.EndScrollView ();
	}

	
	void OnInspectorUpdate()
	{
		Repaint();
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




    private void displayALLTypeOfPrefab(GUIStyle style_Purple,GUIStyle style_Yellow_01)
    {
        _helpBox(0);
        EditorGUILayout.BeginVertical(style_Purple);
       
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select Category : ", GUILayout.Width(110));
        EditorGUI.BeginChangeCheck();

        currentTypeSelected.intValue = EditorGUILayout.Popup(currentTypeSelected.intValue, arrTypeOfPrefab);

        if (EditorGUI.EndChangeCheck())
        {
            currentTypeSelected.intValue = currentTypeSelected.intValue;
            currentItemDisplay = null;
        }
        EditorGUILayout.EndHorizontal();
       
      
        EditorGUILayout.EndVertical();
    }



    private void puzzleList(GUIStyle style_Purple, GUIStyle style_Yellow_01){
        
        EditorGUILayout.BeginVertical(style_Purple);

        _helpBox(19);

        EditorGUILayout.HelpBox("if an Object is currently selected in the Hierarchy the puzzle will be created at this position.", MessageType.Info);

        // arPuzzleName

        for (var i = 0; i < arPuzzleName.Length;i++){
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(arPuzzleName[i], GUILayout.Width(110));
            if (GUILayout.Button("Create", GUILayout.Width(50)))
            {
                Vector3 instantiatePosition = Vector3.zero;
                if (Selection.activeTransform != null)
                    instantiatePosition = Selection.activeTransform.position;
                GameObject newPuzzle = Instantiate((GameObject)listOfPuzzles.GetArrayElementAtIndex(i).objectReferenceValue, instantiatePosition, Quaternion.identity, null);
                Undo.RegisterCreatedObjectUndo(newPuzzle, newPuzzle.name);

                Selection.activeGameObject = newPuzzle;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Clue system", GUILayout.Width(110));
        if (GUILayout.Button("Create", GUILayout.Width(50)))
        {
            Vector3 instantiatePosition = Vector3.zero;
            if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<conditionsToAccessThePuzzle>()){
                instantiatePosition = Selection.activeTransform.position;
                GameObject newClueSystem = Instantiate((GameObject)clueSystem.objectReferenceValue, instantiatePosition, Quaternion.identity, Selection.activeTransform);
                Undo.RegisterCreatedObjectUndo(newClueSystem, newClueSystem.name);
                newClueSystem.name = "ClueBox";
                Selection.activeGameObject = newClueSystem;
            }
            else{
                if (EditorUtility.DisplayDialog("INFO : Action none available"
                , "You need to select a puzzle in the Hierarchy."
                , "Continue"))
                {

                }
            }
                
           
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
    }

    private void F_FocusInfo(GUIStyle style_Purple)
    {

        EditorGUILayout.BeginVertical(style_Purple);

        _helpBox(20);


        EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                Vector3 instantiatePosition = Vector3.zero;
                if (Selection.activeTransform != null)
                    instantiatePosition = Selection.activeTransform.position;
                GameObject newPuzzle = Instantiate((GameObject)listOfPuzzles.GetArrayElementAtIndex(7).objectReferenceValue, instantiatePosition, Quaternion.identity, null);
                Undo.RegisterCreatedObjectUndo(newPuzzle, newPuzzle.name);
                newPuzzle.name = "FocusOnly";
                Selection.activeGameObject = newPuzzle;
            }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
    }


    private void AP_F_CustomAction(GUIStyle style_Purple){
        EditorGUILayout.BeginVertical(style_Purple);
        GUILayout.Label("Custom actions");
        EditorGUILayout.EndVertical();

        _helpBox(21);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
      
        EditorGUILayout.BeginVertical(style_Purple);

        scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection, GUILayout.Height(60));
     

        if(intcurrentItemDisplay == 0)
            GUI.backgroundColor = _cGreen;
        if (GUILayout.Button("Empty")) { intcurrentItemDisplay = 0; }
        GUI.backgroundColor = _cGray;

        //GUILayout.Label("");

        if (intcurrentItemDisplay == 1)
            GUI.backgroundColor = _cGreen;
        if (GUILayout.Button("Light switch")) { intcurrentItemDisplay = 1; }

        GUI.backgroundColor = _cGray;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
               
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            GameObject tmpObj = Instantiate((GameObject)listOfObjsCustomAction.GetArrayElementAtIndex(intcurrentItemDisplay).objectReferenceValue);
            Undo.RegisterCreatedObjectUndo(tmpObj, tmpObj.name);
            Selection.activeGameObject = tmpObj;
        }
    }


    private void displayOptions(GUIStyle style_Orange, GUIStyle style_Yellow_01){
        //Debug.Log(currentTypeSelected);
        switch (currentTypeSelected.intValue)
        {
            case 11:
                AP_F_CustomAction(style_Orange);
                break;
            case 10:
                F_FocusInfo(style_Orange);
                break;
            case 9:
                F_SaveIfObjectActivated(style_Orange);
                break;
            case 8:
                F_ResetObject(style_Orange);
                break;
            case 7:
                F_UIText(style_Orange);
                break;
            case 6:
                F_Trigger(style_Orange);
                break;
            case 5:
                F_Wardrobe(style_Orange);
                break;
            case 4:
                F_Drawer(style_Orange);
                break;
            case 3:
                F_Door(style_Orange);
                break;
            case 2:
                F_Items(style_Yellow_01, style_Orange, "Section Items (Text Viewer)", 0);
                break;
            case 1:
                F_Items(style_Orange, style_Yellow_01, "Section Items (3D Viewer)", 1);
                break;
            case 0:
                puzzleList(style_Orange, style_Yellow_01);
                break;
            
        }
    }


    //--> Reset Object
    private void F_ResetObject(GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("BoxCollider :", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_Collider, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TextProperties :", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_TextProperties, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("VoiceProperties :", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_VoiceProperties, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Data :", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_SaveSystem, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();



        if (GUILayout.Button("Remove Selected Component"))
        {

            if (b_Collider.boolValue)
            {
                if (Selection.activeGameObject.GetComponent<BoxCollider>())
                    Undo.DestroyObjectImmediate(Selection.activeGameObject.GetComponent<BoxCollider>());
            }


            if (b_TextProperties.boolValue)
            {
                if (Selection.activeGameObject.GetComponent<TextProperties>())
                    Undo.DestroyObjectImmediate(Selection.activeGameObject.GetComponent<TextProperties>());
            }
            if (b_VoiceProperties.boolValue)
            {
                if (Selection.activeGameObject.GetComponent<VoiceProperties>())
                    Undo.DestroyObjectImmediate(Selection.activeGameObject.GetComponent<VoiceProperties>());
            }
           
            if (b_SaveSystem.boolValue)
            {
                if (Selection.activeGameObject.GetComponent<SaveData>())
                    Undo.DestroyObjectImmediate(Selection.activeGameObject.GetComponent<SaveData>());
                if (Selection.activeGameObject.GetComponent<isObjectActivated>())
                    Undo.DestroyObjectImmediate(Selection.activeGameObject.GetComponent<isObjectActivated>());
            }
        }
        EditorGUILayout.EndVertical();
    }

    //--> UI Text
    private void F_UIText(GUIStyle style_Orange)
    {

        if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Text>())
        {

            if (currentItemSubType.intValue == 0)
                _helpBox(6);
            else if (currentItemSubType.intValue == 1)
                _helpBox(5);

            EditorGUILayout.BeginVertical(style_Orange);
            GUILayout.Label("Section UI Text Button and Interface");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginVertical(style_Orange);

            scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection);
            GUILayout.Label("Currently " + Selection.activeGameObject.name + " is selected in the Hierarchy.");

                if (!Selection.activeGameObject.GetComponent<TextProperties>())
                {
                    _helpBox(18);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    s_ObjectName = EditorGUILayout.TextField(s_ObjectName);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("Text component is already setup for this object", MessageType.Info);

                }

            
            if (Selection.activeGameObject)
            {
                if (!Selection.activeGameObject.GetComponent<TextProperties>())
                {
                    _helpBox(11);

                    if (GUILayout.Button("Create"))
                    {
                        F_AutoSetupObject(2);
                    }
                }
                _helpBox(17);
            }
           
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical(style_Orange);
            GUILayout.Label("Section Items");


            EditorGUILayout.HelpBox("You need to select a UI object in the Hierarchy with the component Text attached to it .", MessageType.Warning);
            EditorGUILayout.EndVertical();
        }
    }

    //--> Items
    private void F_Items(GUIStyle style_Orange, GUIStyle style_Yellow_01,string s_GroupName,int currentItemSub)
    {
        currentItemSubType.intValue = currentItemSub;
        if (Selection.activeGameObject)
        {

            if (currentItemSubType.intValue == 0)
                _helpBox(6);
            else if (currentItemSubType.intValue == 1)
                _helpBox(5);
            
            EditorGUILayout.BeginVertical(style_Orange);
            GUILayout.Label(s_GroupName);
            EditorGUILayout.EndVertical();

            EditorGUILayout.HelpBox("INFO : Currently " + Selection.activeGameObject.name + " is selected in the Hierarchy." +
                                    "\n", MessageType.Info);
           
            EditorGUILayout.BeginVertical();


            if(currentItemSubType.intValue == 0)
                EditorGUILayout.BeginVertical(style_Orange);
            else
                EditorGUILayout.BeginVertical(style_Orange); 
                

            scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection);
          
            //-> Diary
            if (currentItemSubType.intValue == 0)
            {

                if (!Selection.activeGameObject.GetComponent<TextProperties>())
                {
                    _helpBox(9);
                    EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Diary", GUILayout.Width(100));
                        b_AvailableDIaryOrInventory = EditorGUILayout.Toggle(b_AvailableDIaryOrInventory, GUILayout.Width(20));
                    EditorGUILayout.EndHorizontal();
                    _helpBox(10);
                    EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Name", GUILayout.Width(100));
                        s_ObjectName = EditorGUILayout.TextField(s_ObjectName);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("Text component is already setup for this object", MessageType.Info);

                }
            }
            //-> Inventory
            else if (currentItemSubType.intValue == 1)
            {
                if (!Selection.activeGameObject.GetComponent<TextProperties>())
                {
                    _helpBox(7);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Inventory", GUILayout.Width(100));
                    b_AvailableDIaryOrInventory = EditorGUILayout.Toggle(b_AvailableDIaryOrInventory, GUILayout.Width(20));
                    EditorGUILayout.EndHorizontal();
                    _helpBox(8);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    s_ObjectName = EditorGUILayout.TextField(s_ObjectName);
                    EditorGUILayout.EndHorizontal();
                }
                else{
                    EditorGUILayout.HelpBox("Text component is already setup for this object", MessageType.Info);
      
                }
            }

            if (Selection.activeGameObject)
            {
               
                if (!Selection.activeGameObject.GetComponent<TextProperties>()){
                    _helpBox(11);

                    if (GUILayout.Button("Apply"))
                    {
                        int intEditorType = 0;
                        if (currentItemSubType.intValue == 0)   // wTextnVoices
                            intEditorType = 0;
                        if (currentItemSubType.intValue == 1)   // wItem
                            intEditorType = 1;

                        F_AutoSetupObject(intEditorType);
                    } 


                }
              
                if (currentItemSubType.intValue == 0)
                    _helpBox(13);
                else if (currentItemSubType.intValue == 1)
                    _helpBox(12);

               


                if (Selection.activeGameObject.GetComponent<TextProperties>())
                {
                    if (!Selection.activeGameObject.GetComponent<VoiceProperties>())
                    {
                        
                        EditorGUILayout.BeginVertical(style_Yellow_01);
                        _helpBox(14);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Name", GUILayout.Width(100));
                        s_ObjectVoiceName = EditorGUILayout.TextField(s_ObjectVoiceName);
                        EditorGUILayout.EndHorizontal();
                        if (GUILayout.Button("Add a voice component"))
                        {
                            F_AutoSetupVoice(0);
                        }
                        EditorGUILayout.EndVertical();
                        _helpBox(15);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Voice component is already setup for this object", MessageType.Info);

                    }
                }
            }


            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }
        else{
            EditorGUILayout.BeginVertical(style_Orange);
            GUILayout.Label("Section Items");
          

            EditorGUILayout.HelpBox("You need to select an object in the Hierarchy.",MessageType.Warning);
            EditorGUILayout.EndVertical();
        }
           

    }

    private TextList loadData(int intEditorType){
        Debug.Log(intEditorType);
        string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        datasProjectManager _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as datasProjectManager;

        string s_dataType = "wTextnVoices.asset";
        if (intEditorType == 1) s_dataType = "wItem.asset";
        if (intEditorType == 2) s_dataType = "wUI.asset";
        if (intEditorType == 3) s_dataType = "wFeedback.asset";



        string objectPath2 = "Assets/AP/Assets/Resources/" + _ProjectManagerDatas.currentDatasProjectFolder + "/TextList/" + s_dataType;

        //Debug.Log (objectPath2);
        TextList tmpTextList = AssetDatabase.LoadAssetAtPath(objectPath2, typeof(UnityEngine.Object)) as TextList;
        return tmpTextList;
    }

    private void F_AutoSetupObject(int intEditorType){

        //Debug.Log("Auto Setup");
        if (Selection.activeObject)
        {
            Undo.RegisterFullObjectHierarchyUndo(Selection.activeObject, Selection.activeObject.name);
            Selection.activeObject.name = s_ObjectName;
        }

        //--> Create Box Collider
        if (intEditorType != 2){            // w_Item and w_TextnVoice not w_UI
            if (!Selection.activeTransform.GetComponent<Collider>())
            {
                Undo.AddComponent(Selection.activeGameObject, typeof(BoxCollider));
                Selection.activeTransform.GetComponent<Collider>().isTrigger = true;
            }

            if (!Selection.activeTransform.GetComponent<Renderer>())
            {
                Undo.AddComponent(Selection.activeGameObject, typeof(MeshRenderer));
            }

        }

        //--> Create Text Properties
        if (!Selection.activeTransform.GetComponent<TextProperties>())
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(TextProperties));
            TextProperties item = Selection.activeTransform.GetComponent<TextProperties>();

           

            //-> Choose the data you want to connect to TextProperties
            Selection.activeGameObject.GetComponent<TextProperties>().editorType = intEditorType;   // Data wItem

            //-> Create a new entry in the data Selected
            serializedObject2 = new UnityEditor.SerializedObject(loadData(intEditorType));
            diaryList = serializedObject2.FindProperty("diaryList");

            int numberOfEntry = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").arraySize;
            //-> Create a new entry in data using s_ObjectName for the gameobject name
            manipulateTextList.AddTextEntry(null, diaryList, -1, "",s_ObjectName);  

            //-> Setup the selected object
            diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(numberOfEntry).FindPropertyRelative("showInInventory").boolValue = b_AvailableDIaryOrInventory;
            Undo.RegisterFullObjectHierarchyUndo(item, item.name);
            item.uniqueID = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(numberOfEntry).FindPropertyRelative("uniqueItemID").intValue;

            item.b_UpdateID = true;

            item.gameObject.tag = "Item";      // Select a tag for the object
            item.gameObject.isStatic = false;  // Put the selected object static
        }

        if(intEditorType != 2) {            // w_Item and w_TextnVoice not w_UI
            //-> Add Save System components
            if (!Selection.activeTransform.GetComponent<SaveData>())
            { 
                Undo.AddComponent(Selection.activeGameObject, typeof(isObjectActivated));
                Selection.activeTransform.GetComponent<isObjectActivated>().firstTimeEnabledObject = true;
            }
            if (!Selection.activeTransform.GetComponent<SaveData>())
            { Undo.AddComponent(Selection.activeGameObject, typeof(SaveData)); }


            //-> find all the MonoBehaviour in a gameObject and select the scriptisObjectActvated.cs
            MonoBehaviour[] comp3 = Selection.activeTransform.GetComponents<MonoBehaviour>();

            for (var i = 0; i < comp3.Length; i++)
            {
                if (comp3[i].GetType().ToString() == "isObjectActivated")
                {
                    Selection.activeTransform.GetComponent<SaveData>().isObjectActivatedIndex = i;
                    Selection.activeTransform.GetComponent<SaveData>().b_isObjectActivated = true;

                    Debug.Log("Method " + i + " : " + comp3[i].GetType().ToString());
                    break;
                }
            }

        }

    }

    private void F_SaveIfObjectActivated(GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object Activated the first Time :", GUILayout.Width(180));
        EditorGUILayout.PropertyField(b_ActivatedTheFirstTime, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();



        if (GUILayout.Button("Apply"))
        {
            if (Selection.activeTransform)
            {
                //-> Add Save System components
                if (!Selection.activeTransform.GetComponent<SaveData>())
                {
                    Undo.AddComponent(Selection.activeGameObject, typeof(isObjectActivated));
                    Selection.activeTransform.GetComponent<isObjectActivated>().firstTimeEnabledObject = b_ActivatedTheFirstTime.boolValue;
                }
                if (!Selection.activeTransform.GetComponent<SaveData>())
                { Undo.AddComponent(Selection.activeGameObject, typeof(SaveData)); }


                //-> find all the MonoBehaviour in a gameObject and select the scriptisObjectActvated.cs
                MonoBehaviour[] comp3 = Selection.activeTransform.GetComponents<MonoBehaviour>();

                for (var i = 0; i < comp3.Length; i++)
                {
                    if (comp3[i].GetType().ToString() == "isObjectActivated")
                    {
                        Selection.activeTransform.GetComponent<SaveData>().isObjectActivatedIndex = i;
                        Selection.activeTransform.GetComponent<SaveData>().b_isObjectActivated = true;

                        Debug.Log("Method " + i + " : " + comp3[i].GetType().ToString());
                        break;
                    }
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("INFO : Action none available"
                , "You need to select an object in the Hierarchy."
                , "Continue"))
                {

                }
            }
        }

        EditorGUILayout.EndVertical();
    }



    private void F_AutoSetupVoice(int intEditorType)
    {

        Debug.Log("Auto Setup");
        //--> Create Box Collider
      
        //--> Create Text Properties
        if (!Selection.activeTransform.GetComponent<VoiceProperties>())
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(VoiceProperties));
            VoiceProperties item = Selection.activeTransform.GetComponent<VoiceProperties>();



            //-> Choose the data you want to connect to TextProperties
            Selection.activeGameObject.GetComponent<VoiceProperties>().editorType = intEditorType;   // Data wItem

            //-> Create a new entry in the data Selected
            serializedObject3 = new UnityEditor.SerializedObject(loadData(intEditorType));
            diaryList = serializedObject3.FindProperty("diaryList");
            SerializedProperty _b_EditSubtitle = serializedObject3.FindProperty("b_EditSubtitle");

            serializedObject3.Update();
            int numberOfEntry = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").arraySize;
            //Debug.Log("here : " + numberOfEntry);

            _b_EditSubtitle.boolValue = false;

            //-> Create a new entry in data using s_ObjectName for the gameobject name
            manipulateTextList.AddTextEntry(null, diaryList, -1, "", s_ObjectVoiceName);

            //-> Setup the selected object
            Undo.RegisterFullObjectHierarchyUndo(item, item.name);

            item.uniqueID = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(numberOfEntry).FindPropertyRelative("uniqueItemID").intValue;

            serializedObject3.ApplyModifiedProperties();

        }
    }

    //--> Auto Setup Trigger Play Voice
    private void F_AutoSetupTriggerPlayVoice(int intEditorType)
    {

        //Debug.Log("Auto Setup");
        //--> Create Box Collider

        //--> Create Text Properties
        if (!Selection.activeTransform.GetComponent<TextProperties>())
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(TextProperties));
            TextProperties item = Selection.activeTransform.GetComponent<TextProperties>();



            //-> Choose the data you want to connect to TextProperties
            Selection.activeGameObject.GetComponent<TextProperties>().editorType = intEditorType;   // Data wItem

            //-> Create a new entry in the data Selected
            serializedObject3 = new UnityEditor.SerializedObject(loadData(intEditorType));
            diaryList = serializedObject3.FindProperty("diaryList");
            SerializedProperty _b_EditSubtitle = serializedObject3.FindProperty("b_EditSubtitle");

            serializedObject3.Update();
            int numberOfEntry = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").arraySize;
            //Debug.Log("here : " + numberOfEntry);

            _b_EditSubtitle.boolValue = false;

            //-> Create a new entry in data using s_ObjectName for the gameobject name
            manipulateTextList.AddTextEntry(null, diaryList, -1, "", s_ObjectVoiceName);

            //-> Setup the selected object
            Undo.RegisterFullObjectHierarchyUndo(item, item.name);

            item.uniqueID = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(numberOfEntry).FindPropertyRelative("uniqueItemID").intValue;

            serializedObject3.ApplyModifiedProperties();


            int HowManyEntry = diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").arraySize;



            for (var i = 0; i < HowManyEntry; i++)
            {
                if (diaryList.GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(i).FindPropertyRelative("uniqueItemID").intValue == item.uniqueID)
                {
                    //if (child.name == "DebugText")
                    //Debug.Log (child.name + " : " + HowManyEntry);


                    Undo.RegisterFullObjectHierarchyUndo(item, item.name);

                    SerializedObject serializedObject4 = new UnityEditor.SerializedObject(item.GetComponent<TextProperties>());
                    SerializedProperty m_managerID = serializedObject4.FindProperty("managerID");

                    serializedObject4.Update();
                    m_managerID.intValue = i;
                    serializedObject4.ApplyModifiedProperties();

                    break;
                }
            }

        }
    }

//--> Door
    private void F_Door(GUIStyle style_Orange){
        EditorGUILayout.BeginVertical(style_Orange);
        GUILayout.Label("Section Door");
        EditorGUILayout.EndVertical();

        _helpBox(1);

        EditorGUILayout.BeginHorizontal();
       

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotation")) { currentDoorSubType.intValue = 0; currentItemDisplay = null;}
        if (GUILayout.Button("Translation")) {currentDoorSubType.intValue = 1; currentItemDisplay = null;}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical(style_Orange);

        scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection, GUILayout.Height(120));
        //-> Rotation
        if(currentDoorSubType.intValue == 0){
            if (GUILayout.Button("1 Door Pivot Left")) { currentItemDisplay = (Texture2D)listOfTexture2DDoor.GetArrayElementAtIndex(0).objectReferenceValue; intcurrentItemDisplay = 0; }
            if (GUILayout.Button("1 Door Pivot Right")) { currentItemDisplay = (Texture2D)listOfTexture2DDoor.GetArrayElementAtIndex(1).objectReferenceValue; intcurrentItemDisplay = 1; }
            if (GUILayout.Button("2 Doors")) { currentItemDisplay = (Texture2D)listOfTexture2DDoor.GetArrayElementAtIndex(2).objectReferenceValue; intcurrentItemDisplay = 2; }
        }
        //-> Translation
        else if (currentDoorSubType.intValue == 1)
        {
            if (GUILayout.Button("1 Door Translate Left/Right")) { currentItemDisplay = (Texture2D)listOfTexture2DDoor.GetArrayElementAtIndex(3).objectReferenceValue; intcurrentItemDisplay = 3; }
            if (GUILayout.Button("1 Door Translate Up/Down")) { currentItemDisplay = (Texture2D)listOfTexture2DDoor.GetArrayElementAtIndex(4).objectReferenceValue; intcurrentItemDisplay = 4; }
        }
      
       
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (currentItemDisplay != null)
            GUILayout.Label(currentItemDisplay, GUILayout.Width(122), GUILayout.Height(150));
        else
            GUILayout.Label("\n\n<-Select an object \nto create", GUILayout.Width(120), GUILayout.Height(120));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            GameObject tmpObj = Instantiate((GameObject)listOfObjsDoor.GetArrayElementAtIndex(intcurrentItemDisplay).objectReferenceValue);
            Undo.RegisterCreatedObjectUndo(tmpObj, tmpObj.name);
            Selection.activeGameObject = tmpObj;
        }
    }

//--> Drawer
    private void F_Drawer(GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        GUILayout.Label("Section Drawer");
        EditorGUILayout.EndVertical(); 

        _helpBox(2);

        EditorGUILayout.BeginHorizontal();
       



        EditorGUILayout.BeginVertical(style_Orange);

        scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection, GUILayout.Height(120));
        //-> Rotation
        if (currentDrawerSubType.intValue == 0)
        {
            if (GUILayout.Button("1 Drawer")) { currentItemDisplay = (Texture2D)listOfTexture2DDrawer.GetArrayElementAtIndex(0).objectReferenceValue; intcurrentItemDisplay = 0; }
            if (GUILayout.Button("2 Drawers")) { currentItemDisplay = (Texture2D)listOfTexture2DDrawer.GetArrayElementAtIndex(1).objectReferenceValue; intcurrentItemDisplay = 1; }
            if (GUILayout.Button("3 Drawers")) { currentItemDisplay = (Texture2D)listOfTexture2DDrawer.GetArrayElementAtIndex(2).objectReferenceValue; intcurrentItemDisplay = 2; }
        }
       


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        if (currentItemDisplay != null)
            GUILayout.Label(currentItemDisplay, GUILayout.Width(122), GUILayout.Height(150));
        else
            GUILayout.Label("\n\n<-Select an object \nto create", GUILayout.Width(120), GUILayout.Height(120));


        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            GameObject tmpObj = Instantiate((GameObject)listOfObjsDrawer.GetArrayElementAtIndex(intcurrentItemDisplay).objectReferenceValue);
            Undo.RegisterCreatedObjectUndo(tmpObj, tmpObj.name);
            Selection.activeGameObject = tmpObj;
        }
    }

//--> Wardrobe
    private void F_Wardrobe(GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        GUILayout.Label("Section Wardrobe");
        EditorGUILayout.EndVertical(); 

        _helpBox(3);

        EditorGUILayout.BeginHorizontal();
       



        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotation")) { currentWardrobeSubType.intValue = 0; currentItemDisplay = null; }
        if (GUILayout.Button("Translation")) { currentWardrobeSubType.intValue = 1; currentItemDisplay = null; }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginVertical(style_Orange);
       
       

        scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection, GUILayout.Height(120));
        //-> Rotation
        if (currentWardrobeSubType.intValue == 0)
        {
            if (GUILayout.Button("Wardrobe 2 Doors")) { currentItemDisplay = (Texture2D)listOfTexture2DWardrobe.GetArrayElementAtIndex(2).objectReferenceValue; intcurrentItemDisplay = 2; }
            if (GUILayout.Button("Wardrobe 1 Door Pivot Left")) { currentItemDisplay = (Texture2D)listOfTexture2DWardrobe.GetArrayElementAtIndex(0).objectReferenceValue; intcurrentItemDisplay = 0; }
            if (GUILayout.Button("Wardrobe 1 Door Pivot Right")) { currentItemDisplay = (Texture2D)listOfTexture2DWardrobe.GetArrayElementAtIndex(1).objectReferenceValue; intcurrentItemDisplay = 1; }
           
        }
        //-> Translation
        else if (currentWardrobeSubType.intValue == 1)
        {
            if (GUILayout.Button("Wardrobe 2 Doors")) { currentItemDisplay = (Texture2D)listOfTexture2DWardrobe.GetArrayElementAtIndex(3).objectReferenceValue; intcurrentItemDisplay = 3; }
        }


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (currentItemDisplay != null)
            GUILayout.Label(currentItemDisplay, GUILayout.Width(122), GUILayout.Height(150));
        else
            GUILayout.Label("\n\n<-Select an object \nto create", GUILayout.Width(120), GUILayout.Height(120));


        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            GameObject tmpObj = Instantiate((GameObject)listOfObjsWardrobe.GetArrayElementAtIndex(intcurrentItemDisplay).objectReferenceValue);
            Undo.RegisterCreatedObjectUndo(tmpObj, tmpObj.name);
            Selection.activeGameObject = tmpObj;
        }
    }

//--> Trigger
    private void F_Trigger(GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        GUILayout.Label("Section Trigger");
        EditorGUILayout.EndVertical(); 

        _helpBox(4);

        EditorGUILayout.BeginHorizontal();
       



        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Position")) { currentActionTriggerSubType.intValue = 0; currentItemDisplay = null; }
        if (GUILayout.Button("Actions")) { currentActionTriggerSubType.intValue = 1; currentItemDisplay = null; }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginVertical(style_Orange);



        scrollPosSection = EditorGUILayout.BeginScrollView(scrollPosSection, GUILayout.Height(140));
        //-> Rotation
        if (currentActionTriggerSubType.intValue == 0)
        {
            if (GUILayout.Button("Spawn First Scene")) {    currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(0).objectReferenceValue;intcurrentItemDisplay = 0; }
            if (GUILayout.Button("Teleporter")) {           currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(1).objectReferenceValue;intcurrentItemDisplay = 1; }
            if (GUILayout.Button("Respawn")) {              currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(2).objectReferenceValue;intcurrentItemDisplay = 2; }

            GUILayout.Label("");
            if (GUILayout.Button("Spawn First Scene + Play Voice + Screen Fade Out")) { currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(5).objectReferenceValue; intcurrentItemDisplay = 5; }
            if (GUILayout.Button("End of the Game + Screen Fade In")) { currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(6).objectReferenceValue; intcurrentItemDisplay = 6; }

        }
        //-> Translation
        else if (currentActionTriggerSubType.intValue == 1)
        {
            if (GUILayout.Button("Trigger : Play a Voice")) {  currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(3).objectReferenceValue;intcurrentItemDisplay = 3; }
            if (GUILayout.Button("Trigger : Custom method")) { currentItemDisplay = (Texture2D)listOfTexture2DActionTrigger.GetArrayElementAtIndex(4).objectReferenceValue;intcurrentItemDisplay = 4; }
        }


        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (currentItemDisplay != null)
            GUILayout.Label(currentItemDisplay,GUILayout.Width(122), GUILayout.Height(150));
        else
            GUILayout.Label("\n\n<-Select an object \nto create", GUILayout.Width(120), GUILayout.Height(120));


        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create"))
        {
            GameObject tmpObj = Instantiate((GameObject)listOfObjsActionTrigger.GetArrayElementAtIndex(intcurrentItemDisplay).objectReferenceValue);
            Undo.RegisterCreatedObjectUndo(tmpObj, tmpObj.name);
            Selection.activeGameObject = tmpObj;

            F_AutoSetupTriggerPlayVoice(0);
        }
    }

    void OnSceneGUI(GUIStyle style_Orange )
	{
	}

    public void _helpBox(int value)
    {
        if (helpBoxEditor.boolValue)
        {
            switch (value)
            {
                case 0:
                    EditorGUILayout.HelpBox("Select the category of object you want to create", MessageType.Info);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("1-Select if your door use Rotation or Translation." +
                                            "\n2-Select a pre-made door" +
                                            "\n3-Press ''Create''" +
                                            "\n(A new door is created in scene view).", MessageType.Info);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("1-Select a pre-made drawer" +
                                            "\n2-Press ''Create''" +
                                            "\n(A new drawer is created in scene view).", MessageType.Info);
                    break;
                case 3:
                    EditorGUILayout.HelpBox("1-Select if your Wardrobe use Rotation or Translation." +
                                            "\n2-Select a pre-made Wardrobe" +
                                            "\n3-Press ''Create''" +
                                            "\n(A new Wardrobe is created in scene view).", MessageType.Info);
                    break;
                case 4:
                    EditorGUILayout.HelpBox("When the Player go through a Trigger an action is activated." +
                                            "\nSpawn Position : Use to go from one Scene to another scene." +
                                            "\nActions : Use to Play a sound or do a custom action" +
                                            "\n1-Select the trigger you want to use." +
                                            "\n2-Press ''Create''" +
                                            "\n(A new trigger is created in scene view)", MessageType.Info);
                    break;
                case 5:
                    EditorGUILayout.HelpBox("This Section allow to setup an object that can be displayed in the 3D Viewer.", MessageType.Info);
                    break;
                case 6:
                    EditorGUILayout.HelpBox("This Section allow to setup an object that display a 2D text " +
                                            "on screen.", MessageType.Info);
                    break;
                case 7:
                    EditorGUILayout.HelpBox("If Inventory checkbox == True, Object will be available in the Inventory.", MessageType.Info);
                    break;
                case 8:
                    EditorGUILayout.HelpBox("Choose the Title used for this Item.", MessageType.Info);
                    break;
                case 9:
                    EditorGUILayout.HelpBox("If Diary checkbox == True, text Document will be available in the Diary.", MessageType.Info);
                    break;
                case 10:
                    EditorGUILayout.HelpBox("Choose the Title used for this Text Document.", MessageType.Info);
                    break;
                case 11:
                    EditorGUILayout.HelpBox("Press ''Apply'' to apply this setup to the selected" +
                                            " object in the hierarchy.", MessageType.Info);
                    break;
                case 12:
                    EditorGUILayout.HelpBox("REMEMBER : After pressing ''Apply'' open the window w_Item " +
                                            "to setup the 3D Items you want to show in the 3D Viewer.", MessageType.Warning);
                    break;
                case 13:
                    EditorGUILayout.HelpBox("REMEMBER : After pressing ''Create'' open the window w_TextnVoice " +
                                            "to setup the text display on screen by this object.", MessageType.Warning);
                    break;
                case 14:
                    EditorGUILayout.HelpBox("The Next Section allow to play a voice when the player select this " +
                                            "object during the game." +
                                            "\n1-Choose a name for your voice." +
                                            "\n2-Press ''Add a voice'' to add the voice component to the current " +
                                            "selected object in the Hierachy", MessageType.Info);
                    break;
                case 15:
                    EditorGUILayout.HelpBox("IMPORTANT : After pressing ''Add a voice component'' open the window w_TextnVoice " +
                                            "to setup the voice played by this object.", MessageType.Warning);
                    break;

                case 16:
                    EditorGUILayout.HelpBox("IMPORTANT : After creating object don't forget to update the current scene " +
                                            "using the button ''Update Current Scene'' in window w_UpdateScene.", MessageType.Warning);
                    break;
                case 17:
                    EditorGUILayout.HelpBox("REMEMBER : After pressing ''Create'' open the window w_UI " +
                                            "to setup the text display on screen by this object.", MessageType.Warning);
                    break;
                case 18:
                    EditorGUILayout.HelpBox("Choose the Title used for this UI text.", MessageType.Info);
                    break;
                case 19:
                    EditorGUILayout.HelpBox("Create a puzzle in the Hierachy by clicking one of the buttons below.", MessageType.Info);
                    break;

                case 20:
                    EditorGUILayout.HelpBox("Create an object that allow to Zoom In / Zoom Out on a specific position.", MessageType.Info);
                    break;

                case 21:
                    EditorGUILayout.HelpBox("Create custom actions or use ready to use action prefabs." +
                                            "\n\n1-Press the button corresponding to the action that needs to be created." +
                                            "\n2-Press ''create''" +
                                            "\n(A new action object is created in scene view)", MessageType.Info);
                    break;
            }
        }
    }

	
}
#endif