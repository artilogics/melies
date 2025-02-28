//Description : SaveDataEditor : SaveData custom editor
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

[CustomEditor(typeof(AP_Clue))]
public class AP_ClueEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
	

    SerializedProperty clueList;

	//private bool 				b_addMethods = false;									// use you press button +

	public 	EditorMethods		editorMethods;
	public CallMethods 			callMethods;

	public manipulateSave _manipulateSave;



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

        clueList = serializedObject.FindProperty("clueList");

        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .1f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .3f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(.9f, .9f, 0.9F, 1f));
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


        //AP_Clue myScript = (AP_Clue)target; 

        displayClueList(style_Yellow_01);
		serializedObject.ApplyModifiedProperties ();
	}

    private void displayClueList(GUIStyle style_Yellow_01){
        AP_Clue myScript = (AP_Clue)target; 
        for (var i = 0; i < clueList.arraySize;i++){
            EditorGUILayout.BeginVertical(style_Yellow_01);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clue " + i + ":", GUILayout.Width(50));

            //EditorGUILayout.LabelField("Lock :", GUILayout.Width(40));
            //EditorGUILayout.PropertyField(clueList.GetArrayElementAtIndex(i).FindPropertyRelative("b_Lock"), new GUIContent(""), GUILayout.Width(30));



            if (GUILayout.Button("Add New Language", GUILayout.Width(120)))
            {
                clueList.GetArrayElementAtIndex(i).FindPropertyRelative("txt_Clue").arraySize++;
                clueList.GetArrayElementAtIndex(i).FindPropertyRelative("spriteClue").arraySize++;
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                clueList.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();


            for (var j = 0; j < clueList.GetArrayElementAtIndex(i).FindPropertyRelative("txt_Clue").arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    clueList.GetArrayElementAtIndex(i).FindPropertyRelative("txt_Clue").DeleteArrayElementAtIndex(j);
                    clueList.GetArrayElementAtIndex(i).FindPropertyRelative("spriteClue").DeleteArrayElementAtIndex(j);
                    break;
                }
                EditorGUI.BeginChangeCheck();
                string tmpString = EditorGUILayout.TextArea(clueList.GetArrayElementAtIndex(i).FindPropertyRelative("txt_Clue").GetArrayElementAtIndex(j).stringValue, GUILayout.Height(50));


                if (EditorGUI.EndChangeCheck())
                {
                    clueList.GetArrayElementAtIndex(i).FindPropertyRelative("txt_Clue").GetArrayElementAtIndex(j).stringValue = tmpString;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(20));
                EditorGUILayout.PropertyField(clueList.GetArrayElementAtIndex(i).FindPropertyRelative("spriteClue").GetArrayElementAtIndex(j), new GUIContent(""));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
       

        if (GUILayout.Button("Add New Clue"))
        {
            clueList.arraySize++;
        }


    }






	void OnSceneGUI( )
	{
	}
}
#endif