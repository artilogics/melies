//Description : TriggerManagerEditor : TriggerManager custom editor
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(TriggerManager))]
public class TriggerManagerEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	SerializedProperty 			TriggerType;
	SerializedProperty 			playOnce;
	SerializedProperty 			playOnlyIfNoOtherVoiceOverIsPlayed;
	SerializedProperty 			b_DisabledPlayerMovement;
	SerializedProperty 			DisabledMovementTimer;

	SerializedProperty 			BuildInSceneIndex;
	SerializedProperty 			spawnPointName;


    SerializedProperty methodsList;


    public EditorMethods editorMethods;                                         // access the component EditorMethods
    private bool b_addMethods = false;                                  // use when you press button +


	public string[] arrTriggerType = new string[]{"Play A Voice","Spawn Point","Custom Method","Nothing"};

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

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 			= serializedObject.FindProperty ("SeeInspector");

		TriggerType		= serializedObject.FindProperty ("TriggerType");
		playOnce		= serializedObject.FindProperty ("playOnce");
		playOnlyIfNoOtherVoiceOverIsPlayed		= serializedObject.FindProperty ("playOnlyIfNoOtherVoiceOverIsPlayed");
		b_DisabledPlayerMovement		= serializedObject.FindProperty ("b_DisabledPlayerMovement");
		DisabledMovementTimer		= serializedObject.FindProperty ("DisabledMovementTimer");

		BuildInSceneIndex		= serializedObject.FindProperty ("BuildInSceneIndex");
		spawnPointName		= serializedObject.FindProperty ("spawnPointName");

		TriggerManager myScript = (TriggerManager)target; 

        editorMethods = new EditorMethods();
        methodsList = serializedObject.FindProperty("methodsList");


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



		TriggerManager myScript = (TriggerManager)target; 

		EditorGUILayout.BeginVertical (style_Orange);
		//EditorGUILayout.HelpBox ("This script allow to setup the fake Joystick cursor behaviour", MessageType.Info);


		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Trigger Type : ", GUILayout.Width (180));
		TriggerType.intValue = EditorGUILayout.Popup (TriggerType.intValue, arrTriggerType);
		EditorGUILayout.EndHorizontal ();

        //-> Play a voice
		if (TriggerType.intValue == 0) {
			//--> playOnce
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Play Once : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (playOnce, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			//--> playOnlyIfNoOtherVoiceOverIsPlayed
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Only If No Other Voice Is Played : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (playOnlyIfNoOtherVoiceOverIsPlayed, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			//--> b_DisabledPlayerMovement
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Disabled Player Movement : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (b_DisabledPlayerMovement, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			//--> DisabledMovementTimer
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Duration (seconds) : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (DisabledMovementTimer, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();
		}

        //-> Spawn points
		if (TriggerType.intValue == 1) {
			//--> BuildInSceneIndex
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Scenes In Build Index : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (BuildInSceneIndex, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();

			//--> spawnPointName
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Spawn Point Name : ", GUILayout.Width (180));
			EditorGUILayout.PropertyField (spawnPointName, new GUIContent (""));
			EditorGUILayout.EndHorizontal ();
		}

        //-> Call custom method
        if (TriggerType.intValue == 2)
        {
            //--> playOnce
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Play Once : ", GUILayout.Width(180));
            EditorGUILayout.PropertyField(playOnce, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            //--> b_DisabledPlayerMovement
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Disabled Player Movement : ", GUILayout.Width(180));
            EditorGUILayout.PropertyField(b_DisabledPlayerMovement, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            //--> DisabledMovementTimer
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Duration (seconds) : ", GUILayout.Width(180));
            EditorGUILayout.PropertyField(DisabledMovementTimer, new GUIContent(""));
            EditorGUILayout.EndHorizontal();


            showCustomMethods(style_Purple,style_Blue);
        }

		EditorGUILayout.EndVertical ();
       
		serializedObject.ApplyModifiedProperties ();

		EditorGUILayout.LabelField ("");
	}


    private void showCustomMethods(GUIStyle style_Purple,GUIStyle style_Blue){
        TriggerManager myScript = (TriggerManager)target; 
        //-> Custom methods
        EditorGUILayout.BeginVertical(style_Purple);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Custom Methods", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Method with one argument is allowed." 
            + "\nArgument type allowed : int, GameObject, string, float, Audio Clip", MessageType.Info);
        
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                b_addMethods = true;
            }

            editorMethods.DisplayMethodsOnEditor(myScript.methodsList, methodsList, style_Blue);

            if (b_addMethods)
            {
                editorMethods.AddMethodsToList(methodsList);
                b_addMethods = false;
            }

        EditorGUILayout.EndVertical();

    }


	void OnSceneGUI( )
	{
	}
}
#endif