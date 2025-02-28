//Description : focusCamEffectEditor : focusCamEffect custom Editor
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

[CustomEditor(typeof(focusCamEffect))]
public class focusCamEffectEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector

	SerializedProperty 			targetFocusCamera;

	//SerializedProperty 			refOffset;
	//SerializedProperty 			speed;

	SerializedProperty 			b_Child;

    SerializedProperty          _audio;
    SerializedProperty          a_FocusIn;
    SerializedProperty          a_FocusOut; 
    SerializedProperty          volume_FocusIn;
    SerializedProperty          volume_FocusOut; 




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
		SeeInspector 		= serializedObject.FindProperty ("SeeInspector");
		targetFocusCamera 	= serializedObject.FindProperty ("targetFocusCamera");
		//refOffset 			= serializedObject.FindProperty ("refOffset");
		//speed 				= serializedObject.FindProperty ("speed");
		b_Child 			= serializedObject.FindProperty ("b_Child");

        _audio              = serializedObject.FindProperty("_audio");
        a_FocusIn           = serializedObject.FindProperty("a_FocusIn");
        a_FocusOut          = serializedObject.FindProperty("a_FocusOut");
        volume_FocusIn      = serializedObject.FindProperty("volume_FocusIn");
        volume_FocusOut     = serializedObject.FindProperty("volume_FocusOut");


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
            Tex_04 = MakeTex(2, 2, new Color(.88F, .88f, .88f, 1f));
            Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
        }
    }


	public override void OnInspectorGUI()
	{
		if(SeeInspector.boolValue)							// If true Default Inspector is drawn on screen
			DrawDefaultInspector();

		serializedObject.Update ();

        focusCamEffect myScript = (focusCamEffect)target;


		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("See Inspector :", GUILayout.Width (85));
		EditorGUILayout.PropertyField(SeeInspector, new GUIContent (""), GUILayout.Width (30));
		EditorGUILayout.EndHorizontal ();

		GUIStyle style_Yellow_01 		= new GUIStyle();	style_Yellow_01.normal.background 		= Tex_01; 
		GUIStyle style_Blue 			= new GUIStyle();	style_Blue.normal.background 			= Tex_03;
		GUIStyle style_Purple 			= new GUIStyle();	style_Purple.normal.background 			= Tex_04;
		GUIStyle style_Orange 			= new GUIStyle();	style_Orange.normal.background 			= Tex_05; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle();	style_Yellow_Strong.normal.background 	= Tex_02;


		//focusCamEffectEditor myScript = (focusCamEffectEditor)target; 

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("This Object is not the ingameGlobalManager :", GUILayout.Width(250));
        EditorGUILayout.PropertyField(b_Child, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginVertical (style_Purple);
        if (b_Child.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Target Focus Camera :", GUILayout.Width(220));
                EditorGUILayout.PropertyField(targetFocusCamera, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            if(targetFocusCamera.objectReferenceValue){
                if (GUILayout.Button("Create Test Camera"))
                {
                    GameObject camtest = new GameObject();
                    camtest.name = "camTest";
                    camtest.AddComponent<Camera>();
                    camtest.transform.SetParent(myScript.targetFocusCamera.transform);
                    camtest.transform.localPosition = Vector3.zero;
                    camtest.transform.localEulerAngles = Vector3.zero;
                    camtest.GetComponent<Camera>().fieldOfView = 50;
                    camtest.GetComponent<Camera>().nearClipPlane = 0.05f;
                    Undo.RegisterCreatedObjectUndo(camtest, "camtest");

                    Selection.activeTransform = camtest.transform.parent.transform;
                } 
            }

            if (myScript.targetFocusCamera 
                && myScript.targetFocusCamera.transform.childCount > 0
                && myScript.targetFocusCamera.transform.GetChild(0).name == "camTest"){
                if (GUILayout.Button("Delete Test Camera"))
                {
                    Undo.DestroyObjectImmediate(myScript.targetFocusCamera.transform.GetChild(0).gameObject);
                }  
            }
           
        }
        else
        {
          /*  EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Offset Acceleration to reach end position :", GUILayout.Width(240));
                EditorGUILayout.PropertyField(refOffset, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Movement speed :", GUILayout.Width(150));
                EditorGUILayout.PropertyField(speed, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
*/

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("AudioSource :", GUILayout.Width(150));
                EditorGUILayout.PropertyField(_audio, new GUIContent(""), GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Focus In :", GUILayout.Width(150));
                EditorGUILayout.PropertyField(a_FocusIn, new GUIContent(""), GUILayout.Width(100));

                EditorGUILayout.LabelField("vol :", GUILayout.Width(30));
                EditorGUILayout.PropertyField(volume_FocusIn, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Focus Out :", GUILayout.Width(150));
                EditorGUILayout.PropertyField(a_FocusOut, new GUIContent(""), GUILayout.Width(100));

                EditorGUILayout.LabelField("vol :", GUILayout.Width(30));
                EditorGUILayout.PropertyField(volume_FocusOut, new GUIContent(""));
            EditorGUILayout.EndHorizontal();


        }
        EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties ();
	}


	void OnSceneGUI( )
	{
	}
}
#endif