// Description : w_UpdateScene.cs : This script allow to update object in the Hierarchy before Starting Editor Play Mode
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


public class w_UpdateScene : EditorWindow
{
	private Vector2 				scrollPosAll;
    private Vector2                 scrollPosSection;


    [MenuItem("Tools/AP/Update/Window update Scene (w_UpdateScene)")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_UpdateScene));
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

    public Color                _cRed2 = new Color(1f, .35f, 0f, 1f);
    public Color                _cRed = new Color(1f, .5f, 0f, .5f);
	public Color 				_cGray = new Color(.9f,.9f,.9f,1);


	public Texture2D eye;
    public Texture2D currentItemDisplay;
    public int intcurrentItemDisplay = 0;
    public bool b_UpdateProcessDone = false;
    public bool b_AllowUpdateScene = false;

    public windowMethods _windowMethods;

	void OnEnable () {
        _windowMethods = new windowMethods();

		_MakeTexture ();

       
	}


	void OnGUI()
	{
		//--> Scrollview
		scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

		//--> Window description
		CheckTex ();
		GUIStyle style_Yellow_01 		= new GUIStyle ();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle ();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle ();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle ();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle ();	style_Yellow_Strong.normal.background 	= Tex_02;

		//		
        GUI.backgroundColor = _cRed2;

//--> Update + Play Game
        if (GUILayout.Button("Update + Play Game", GUILayout.Height(30)))
        {
            b_UpdateProcessDone = false;
            _windowMethods.updateIDs();                             // Update Text IDs
            _windowMethods.saveLevelInfos();                        // Update save system
            _windowMethods.builinScene(true, b_UpdateProcessDone);  // Update buildInScene
        }

//--> Update Current Scene
        GUI.backgroundColor = _cRed;
        if (GUILayout.Button("Update Current Scene", GUILayout.Height(30)))
        {
            b_UpdateProcessDone = false;
            _windowMethods.updateIDs();                             // Update Text IDs
            _windowMethods.saveLevelInfos();                        // Update save system
            _windowMethods.builinScene(false,b_UpdateProcessDone);  // Update buildInScene
        }   


        GUI.backgroundColor = _cGray;

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("");

		
        EditorGUILayout.EndScrollView ();
	}

    void Update()
    {
        if(_windowMethods.bool_UpdateProcessDone){
            _windowMethods.bool_UpdateProcessDone = false;
            b_UpdateProcessDone = false;
            EditorApplication.isPlaying = true; 
        }
        if (b_UpdateProcessDone)
        {
            b_UpdateProcessDone = false;
            EditorApplication.isPlaying = true;
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

}
#endif
