// Description : w_Version
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


public class w_Version : EditorWindow
{
	private Vector2 				scrollPosAll;
    private Vector2                 scrollPosSection;


    [MenuItem("Tools/AP/Other/Version")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Version));
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


	public string[] 			listItemType = new string[]{};

	public List<string> 		_test = new List<string>(); 
	public int 					page = 0;
	public int 					numberOfIndexInAPage = 50;
	public int 					seachSpecificID = 0;


	public Texture2D eye;
    public Texture2D currentItemDisplay;
    public int intcurrentItemDisplay = 0;
    public bool b_UpdateProcessDone = false;
    public bool b_AllowUpdateScene = false;

    public windowMethods _windowMethods;

	void OnEnable () {
        _windowMethods = new windowMethods();

        if (!EditorPrefs.HasKey("AP_DarkIntensity"))
            EditorPrefs.SetFloat("AP_DarkIntensity", .4f);
	}


	void OnGUI()
	{
		//--> Scrollview
		scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);

		EditorGUILayout.LabelField("Current Version:", EditorStyles.boldLabel);


		EditorGUILayout.HelpBox("Version 1.1.3 (2022.3.10f1)" +
					"\nSolve an issue with gamepad inside gear puzzle." +
					"\nSolve menu issues during the game." +
					"\nSolve an issue in the diary (last page button).", MessageType.Info);

		EditorGUILayout.LabelField("Old Version:", EditorStyles.boldLabel);

		EditorGUILayout.HelpBox("Version 1.1.2 (2020.3.0f1)" +
				"\nUpdate to Unity 2020.3.0f1 (LTS)" +
				"\nDiary: Bug correction when there are more than 9 items in the diary", MessageType.Info);


		EditorGUILayout.HelpBox("Version 1.1 (2020.3.0f1)" +
					"\nUpdate to Unity 2020.3.0f1 (LTS)" +
					"\nBug corrections (Subtitle system (Methods have changed), Clue system (Can't access Inventory and Diary Menu), Puzzle detection, remove character don't destroy on load" +
					" prevent a bug with save system)", MessageType.Info);

		EditorGUILayout.HelpBox("Version 1.1: (Character Update)" +
			"\nVersion 2020_1_0 (002)" +
			"\nNew Character Features (Jump, Run, Flashlight)" +
			"\nNew Default Inputs (WASD)" +
			"\nNew Input UI Remapper" +
			"\nNew Layer Added" +
			"\nDocumentation Updated + New PDF named Update_1_1 + PDF WhatsNew has been removed", MessageType.Info);

		EditorGUILayout.HelpBox("First Person Narrative Adventures + Complete Puzzle Engine:" +
			    "\nversion 2020.1.0f1 (update 1)" +
				"\nImprove gamepad integration", MessageType.Info);

		EditorGUILayout.HelpBox("First Person Narrative Adventures + Complete Puzzle Engine:" +
				"\nversion 2020.1.0f1", MessageType.Info);


		EditorGUILayout.HelpBox("First Person Narrative Adventures + Complete Puzzle Engine:" +
            "\nversion 2019.3.0 (Pro Skin Update)",MessageType.Info);


      EditorGUILayout.HelpBox("What’s new in V1.0.6:" +
		"\n-The documentation has been separated into five parts." +
		"\n- New Module Clue/ Hint Module." +
		"\nMore info in Doc Part 2 Section 6.8.6 Clue / Hint system" +
		"\n" +
		"\nWhat’s new in V1.0.5:" +
		"\n-New Module call Custom Action(Allows to create custom action when the player press a UI Icon) Read the new Section for more info:" +
		"\n-Doc Part 5 Section 7.7 - Create a lamp that can be turned On and Off" +
		"\n- Doc Part 5 Section How to create a boolean method" +
		"\n- Doc Part 5 Section How to use a boolean method" +
		"\n- Improve crouch" +
		"\n- Mobile Input corrections", MessageType.Info) ;

		
        EditorGUILayout.EndScrollView ();
	}

  

}
#endif
