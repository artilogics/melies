// Description : w_ProjectManager.cs : This script is used to create a window tab that manager AP Global Project Setting
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
using System.IO;


public class w_ProjectManager : EditorWindow
{
    private Vector2 scrollPosAll;
    SerializedObject serializedObject2;
    SerializedProperty helpBoxEditor;
    SerializedProperty currentDatasProjectFolder;
    SerializedProperty int_CurrentDatasProjectFolder;
    SerializedProperty int_CurrentDatasSaveSystem;
    SerializedProperty s_newProjectName;

    SerializedProperty s_newLanguageName;
    SerializedProperty firstSceneBuildInIndex;
    SerializedProperty buildinList;
    SerializedProperty newSceneName;

    SerializedProperty editorType;

    SerializedProperty specificChar;

    public datasProjectManager _ProjectManagerDatas;
    public bool b_ProjectManagerAssetExist = true;

    public EditorManipulateTextList manipulateTextList;




    // Add menu item named "Test Mode Panel" to the Window menu
    [MenuItem("Tools/AP/Project Manager (w_ProjectManager)")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_ProjectManager));
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {                       // use to change the GUIStyle
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private Texture2D Tex_01;
    private Texture2D Tex_02;
    private Texture2D Tex_03;
    private Texture2D Tex_04;
    private Texture2D Tex_05;

    public string[] listItemType = new string[] { };

    public List<string> _test = new List<string>();
    public int page = 0;
    public int numberOfIndexInAPage = 50;
    public int seachSpecificID = 0;

    public Color _cGreen = new Color(1f, .8f, .4f, 1);
    public Color _cGray = new Color(.9f, .9f, .9f, 1);


    public Texture2D eye;

    public string[] listDatas2 = new string[] { "wTextnVoices", "wFeedback", "wUI", "wItem" };      // use to update w_Diary,w_Inventory...
    public string[] listSaveType = new string[] { "PlayerPrefs", ".dat" };



    void OnEnable()
    {
        manipulateTextList = new EditorManipulateTextList();

        _MakeTexture();


        string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as datasProjectManager;
        if (_ProjectManagerDatas)
        {
            serializedObject2 = new UnityEditor.SerializedObject(_ProjectManagerDatas);
            helpBoxEditor                   = serializedObject2.FindProperty("helpBoxEditor");
            currentDatasProjectFolder       = serializedObject2.FindProperty("currentDatasProjectFolder");
            int_CurrentDatasProjectFolder   = serializedObject2.FindProperty("int_CurrentDatasProjectFolder");
            int_CurrentDatasSaveSystem      = serializedObject2.FindProperty("int_CurrentDatasSaveSystem");
            s_newProjectName                = serializedObject2.FindProperty("s_newProjectName");
            s_newLanguageName               = serializedObject2.FindProperty("s_newLanguageName");
            firstSceneBuildInIndex          = serializedObject2.FindProperty("firstSceneBuildInIndex");
            buildinList                     = serializedObject2.FindProperty("buildinList");
            newSceneName                    = serializedObject2.FindProperty("newSceneName");
            specificChar                    = serializedObject2.FindProperty("specificChar");
        }
        else
        {
            b_ProjectManagerAssetExist = false;
        }

        updateProjectList();

        string objectEye = "Assets/AP/Assets/Textures/Edit/Eye.png";
        eye = AssetDatabase.LoadAssetAtPath(objectEye, typeof(UnityEngine.Object)) as Texture2D;
    }


    void OnGUI()
    {
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

        //--> Window description
        //GUI.backgroundColor = _cGreen;
        CheckTex();
        GUIStyle style_Yellow_01 = new GUIStyle(); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(); style_Yellow_Strong.normal.background = Tex_02;

        //		
        EditorGUILayout.BeginVertical(style_Purple);
        EditorGUILayout.HelpBox("Window Tab : Project Manager\nv1.0.6.11", MessageType.Info);
        EditorGUILayout.EndVertical();


        // --> Display data
        EditorGUILayout.BeginHorizontal();
        _ProjectManagerDatas = EditorGUILayout.ObjectField(_ProjectManagerDatas, typeof(UnityEngine.Object), true) as datasProjectManager;
        EditorGUILayout.EndHorizontal();

        if (_ProjectManagerDatas != null)
        {

            GUILayout.Label("");

            serializedObject2.Update();

            //--> Update the list of available projects
            updateProjectList();



            EditorGUILayout.LabelField("");

            //--> Section to create a new project
            currentNewProjectFolder(style_Yellow_01);

            EditorGUILayout.LabelField("");

            //--> Display informations about the current project
            currentProject(style_Blue, style_Purple);

            EditorGUILayout.LabelField("");

            //--> Display informations DataSaveSystem
            saveSystem(style_Yellow_01);

            EditorGUILayout.LabelField("");


            //--> Display the buildin Index for the first scene loaded when a new game is created
            currentFirstSceneBuildinIndex(style_Blue);

            EditorGUILayout.LabelField("");


            //--> Display the list of scene included in the save system
            ListOfSceneIncludedInTheSaveSystem(style_Yellow_01);


            serializedObject2.ApplyModifiedProperties();

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("");
        }
        EditorGUILayout.EndScrollView();
    }

    //--> Display informations about the current project
    private void currentProject(GUIStyle style_Blue, GUIStyle style_Purple)
    {
        EditorGUILayout.BeginVertical(style_Blue);

        EditorGUILayout.LabelField("2-Current Project :", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Project :", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();

        int_CurrentDatasProjectFolder.intValue = EditorGUILayout.Popup(int_CurrentDatasProjectFolder.intValue, listItemType.ToArray());

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterFullObjectHierarchyUndo(this, "Undo_Window");
            currentDatasProjectFolder.stringValue = listItemType[int_CurrentDatasProjectFolder.intValue];

            //-> Close data windows
            if (w_Item.instance) w_Item.instance.Close();
            if (w_TextnVoice.instance) w_TextnVoice.instance.Close();
            if (w_UI.instance) w_UI.instance.Close();
            if (w_Feedback.instance) w_Feedback.instance.Close();

            SetEditorBuildSettingsScenes("Assets/AP/Assets/Scenes/" + currentDatasProjectFolder.stringValue);

            PlayerPrefs.DeleteAll();
        }
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.LabelField("Current Folder : Assets/AP/Assets/Resources/" + currentDatasProjectFolder.stringValue);

        EditorGUILayout.BeginVertical(style_Purple);
        addANewLanguage();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }


    //--> Display informations DataSaveSystem
    private void saveSystem(GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);

        EditorGUILayout.LabelField("3-Data Save Sytem :", EditorStyles.boldLabel);

        if (int_CurrentDatasSaveSystem.intValue == 1)
            EditorGUILayout.HelpBox(".dat format : There is no space restriction. But it doesn't work on WebGL.", MessageType.Info);
        else
            EditorGUILayout.HelpBox("PlayerPrefs : There is space restriction (about 1Mo). But in return, it works on any platform.", MessageType.Info);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select Save Type :", GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();

        int_CurrentDatasSaveSystem.intValue = EditorGUILayout.Popup(int_CurrentDatasSaveSystem.intValue, listSaveType);
        
        if (GUILayout.Button("Show .Dat In Explorer"))
        {
            ShowDataInExplorer();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterFullObjectHierarchyUndo(this, "Undo_Window");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Even if .dat is selected, Settings like Inputs, graphics, sound, quality, game Options are saved in PlayerPrefs", MessageType.Warning);

        EditorGUILayout.EndVertical();
    }

    public void ShowDataInExplorer()
    {
        #region
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.TrimEnd(new[] { '\\', '/' });
        System.Diagnostics.Process.Start(itemPath);
        #endregion
    }

    //--> Display informations about the first scene loaded when a new game is created
    private void currentFirstSceneBuildinIndex(GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);

        EditorGUILayout.LabelField("4- Choose the scene loaded when a new game slot is created:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Buildin Index :", GUILayout.Width(100));
        EditorGUILayout.PropertyField(firstSceneBuildInIndex, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }



    //--> Section to create a new project
    private void currentNewProjectFolder(GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);

        EditorGUILayout.LabelField("1-Create a new Project :", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Project Name :", GUILayout.Width(100));
        EditorGUILayout.PropertyField(s_newProjectName, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Project datas will be saved in folder : " + "Assets/AP/Assets/Resources/" + s_newProjectName.stringValue);

        if (GUILayout.Button("Create New Project"))
        {
            duplicateFolder();
        }

        EditorGUILayout.EndVertical();
    }
   
	//--> Section to create a new project
	private void ListOfSceneIncludedInTheSaveSystem(GUIStyle style_Yellow_01){
		EditorGUILayout.BeginVertical (style_Yellow_01);

		EditorGUILayout.LabelField ("5-List of scenes included in the build in Settings and save system :",EditorStyles.boldLabel );

        string s_howManyScene = "";
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choose the name of the scene : ", GUILayout.Width(100));
        if(buildinList.arraySize < 10){
            s_howManyScene = "0" + buildinList.arraySize;
            EditorGUILayout.LabelField("0" + buildinList.arraySize + "_", GUILayout.Width(30));
        }
        else{
            s_howManyScene = buildinList.arraySize.ToString();
            EditorGUILayout.LabelField(buildinList.arraySize + "_", GUILayout.Width(30));
        }

               

            EditorGUILayout.PropertyField(newSceneName, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create new scene for the project"))
        {
            duplicateDefaultScene(s_howManyScene);
        }

        EditorGUILayout.HelpBox("IMPORTANT : After adding a new scene in the build in Settings, you need to update the List in the " +
                                "Project Manager", MessageType.Warning);



        if (GUILayout.Button("Update the list of scene"))
        {
            SetEditorBuildSettingsScenes("Assets/AP/Assets/Scenes/" + currentDatasProjectFolder.stringValue);
        }

		EditorGUILayout.HelpBox ("This section show the scenes included in the save system." +
			"When you add a scene to the Scene In Build (Edit -> Project Settings) don't forget to update the list of the scenes included in the Save system" +
			"By pressing the button ''Create a list of build in scene'' (Tools -> AP -> Update -> Save System -> )",MessageType.Warning);


		string s_ListName = "";
		for (var i = 0; i < buildinList.arraySize; i++) {
                s_ListName += buildinList.GetArrayElementAtIndex(i).stringValue;

                if (i < buildinList.arraySize - 1)
                    s_ListName += ", ";
		}

        EditorGUILayout.LabelField("List of scenes included in the Save System :", EditorStyles.boldLabel);


		EditorGUILayout.LabelField (s_ListName);
       

		EditorGUILayout.EndVertical ();
	}

    private void duplicateDefaultScene(string s_HowManyScene){
        FileUtil.CopyFileOrDirectory("Assets/AP/Assets/Scenes/NewEmptyScene/newScene.unity", "Assets/AP/Assets/Scenes/" + currentDatasProjectFolder.stringValue+ "/" 
                                     + s_HowManyScene +  "_" + newSceneName.stringValue +".unity");
        AssetDatabase.Refresh();            // Refresh Project Tab 

        SetEditorBuildSettingsScenes("Assets/AP/Assets/Scenes/" + currentDatasProjectFolder.stringValue);
    }


//-> Duplicate default folder to create a new project
	private void  duplicateFolder (){

        //-> Check if the name already exist
		bool b_Exist = false;
        foreach (string file in System.IO.Directory.GetDirectories("Assets/AP/Assets/Resources"))
		{
			string newString = file;
			newString = newString.Replace (".meta", "");
			newString = newString.Replace ("Assets/AP/Assets/Resources/", "");
			if (newString == s_newProjectName.stringValue) {
				b_Exist = true;
			}
		}

        //-> Prevent bug if name end with an empty space ' '
        string s_checkIfTheNameEndedWithAEmptySpace = s_newProjectName.stringValue;
        bool b_EmptySpace = false;
        if(s_checkIfTheNameEndedWithAEmptySpace[s_checkIfTheNameEndedWithAEmptySpace.Length-1]== ' '){
            b_EmptySpace = true;
        }

        //-> Create new Folder
        if (!b_Exist && !b_EmptySpace) {
			FileUtil.CopyFileOrDirectory("Assets/AP/Assets/Datas/Default", "Assets/AP/Assets/Resources/" + s_newProjectName.stringValue);
			AssetDatabase.Refresh ();			// Refresh Project Tab 

			int positionInArray = 0;
            for (var i = 0; i < System.IO.Directory.GetDirectories ("Assets/AP/Assets/Resources").Length; i++) {
                string newString = System.IO.Directory.GetDirectories ("Assets/AP/Assets/Resources")[i];
				newString = newString.Replace (".meta", "");
				newString = newString.Replace ("Assets/AP/Assets/Resources/", "");

				if(s_newProjectName.stringValue == newString){
					positionInArray = i;
					break;
				}
			}
				
			currentDatasProjectFolder.stringValue = s_newProjectName.stringValue;
			int_CurrentDatasProjectFolder.intValue = positionInArray;

			//-> Close data windows
            if (w_Item.instance)w_Item.instance.Close ();
			if (w_TextnVoice.instance)w_TextnVoice.instance.Close ();
			if (w_UI.instance)w_UI.instance.Close ();
			if (w_Feedback.instance)w_Feedback.instance.Close ();

            initBuildInSettings();

            PlayerPrefs.DeleteAll();

        } else if(b_Exist){
			if (EditorUtility.DisplayDialog ("INFO : Action not available"
				,"You must choose a name that doesn't already exist"
				,"Continue")) {

			}
		}
        else if (b_EmptySpace)
        {
            if (EditorUtility.DisplayDialog("INFO : Action not available"
                , "The name of your project could not end with an empty space ' ' ." +
                "\nExample : " + 
                "\n''My Project'' is OK " +
                "\n''My Project '' is WRONG"
                , "Continue"))
            {

            }
        }
	}


    private void initBuildInSettings(){
        FileUtil.CopyFileOrDirectory("Assets/AP/Assets/Scenes/Default", "Assets/AP/Assets/Scenes/" + s_newProjectName.stringValue);
        AssetDatabase.Refresh();            // Refresh Project Tab 
        SetEditorBuildSettingsScenes("Assets/AP/Assets/Scenes/" + s_newProjectName.stringValue);
    }

    public void SetEditorBuildSettingsScenes(string newPath)
    {

        List<SceneAsset> m_SceneAssets = new List<SceneAsset>();
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

        //Debug.Log(System.IO.Directory.GetFiles("Assets/AP/Assets/Scenes/" + s_newProjectName.stringValue).Length);
        int totalNum = 0;
        for (var i = 0; i < System.IO.Directory.GetFiles(newPath).Length; i++)
        {
            string newString = System.IO.Directory.GetFiles(newPath)[i];
            if(newString.Contains(".unity") && !newString.Contains(".meta")){
                totalNum++; 
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(newString, true));
            }
         
        }

        foreach (var sceneAsset in m_SceneAssets)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        builinScene(editorBuildSettingsScenes.ToArray());
    }


//--> Add new language for data section
	private void addANewLanguage(){
		EditorGUILayout.LabelField ("Add new language to the current Project : ");

		EditorGUILayout.HelpBox ("Each language must have a unique name.", MessageType.Warning);


		EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Choose Name : ", GUILayout.Width (100));
			EditorGUILayout.PropertyField (s_newLanguageName, new GUIContent (""));
		EditorGUILayout.EndHorizontal ();


		if (GUILayout.Button ("Add New language")) {
			for (var i = 0; i < listDatas2.Length; i++) {
				string objectPath = "Assets/AP/Assets/Resources/"+ currentDatasProjectFolder.stringValue + "/TextList/" + listDatas2[i] + ".asset";

                //Debug.Log(objectPath);

				TextList tmpTextList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;

				SerializedObject serializedObject3 = new UnityEditor.SerializedObject (tmpTextList);
				SerializedProperty diaryList = serializedObject3.FindProperty ("diaryList");
				SerializedProperty _listOfLanguage = serializedObject3.FindProperty ("listOfLanguage");

				serializedObject3.Update ();
			//-> Create new language
				manipulateTextList.Add_A_Language (_listOfLanguage, diaryList, s_newLanguageName.stringValue);
				serializedObject3.ApplyModifiedProperties ();
			}

            if (EditorUtility.DisplayDialog(
                "INFO : New Language added", 
                s_newLanguageName.stringValue + " is added to windows : w_TxtbVoice, w_Item, w_Feedback, w_UI"
                , "Continue"))
            {

            }
		}

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


	private void updateProjectList(){
		List<string> tmpList = new List<string> ();

        foreach (string file in System.IO.Directory.GetDirectories("Assets/AP/Assets/Resources"))
		{
            //Debug.Log(file);
			if (!file.Contains (".DS_Store")) {
				string newString = file;
				//newString = newString.Replace (".meta", "");
				newString = newString.Replace ("Assets/AP/Assets/Resources/", "");
                newString = newString.Replace("Assets/AP/Assets/Resources", "");
                newString = newString.Replace(specificChar.stringValue, "");


				//Debug.Log(newString); 
				tmpList.Add (newString);
			}
		}

		listItemType = tmpList.ToArray();

	}

    private void builinScene(EditorBuildSettingsScene[] newEditorBuilingSettings)
    {
        string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        datasProjectManager _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as datasProjectManager;

        if (_ProjectManagerDatas)
        {
          
            Undo.RegisterFullObjectHierarchyUndo(_ProjectManagerDatas, _ProjectManagerDatas.name);


            buildinList.ClearArray();
            for (var i = 0; i < newEditorBuilingSettings.Length; i++)
            {
                string scenName = EditorBuildSettings.scenes[i].path;
                scenName = scenName.Replace(".unity", "");

                for (var j = scenName.Length - 1; j > 1; j--)
                {
                    char tmp = scenName[j];

                    if (tmp == '/')
                    {
                        scenName = scenName.Substring(j + 1, scenName.Length - 1 - j);
                        break;
                    }
                }
                buildinList.InsertArrayElementAtIndex(0);
                buildinList.GetArrayElementAtIndex(0).stringValue = scenName;
            }
        }
        else
        {
            if (EditorUtility.DisplayDialog("Info : This action is not possible."
                , "You need an object datasProjectManager in your Project Tab : Assets/AP/Assets/Datas/ProjectManagerDatas.asset."
                , "Continue")) { }

        }

    }

	void OnSceneGUI( )
	{
	}


	public void _helpBox(int value){
		if (helpBoxEditor.boolValue) {
			switch (value) {
			case 0:
				EditorGUILayout.HelpBox ("",MessageType.Info);
				break;
			case 1:
				EditorGUILayout.HelpBox ("",MessageType.Info);
				break;
			case 2:
				EditorGUILayout.HelpBox("",MessageType.Warning);
				break;
			}
		}
	}
}
#endif