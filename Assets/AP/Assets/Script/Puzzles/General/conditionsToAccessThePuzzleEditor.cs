//Description : conditionsToAccessThePuzzleEditor : Custom Editor for conditionsToAccessThePuzzle.cs
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(conditionsToAccessThePuzzle))]
public class conditionsToAccessThePuzzleEditor : Editor {
	SerializedProperty			SeeInspector;											// use to draw default Inspector
    SerializedProperty          onlyFocusMode;

    SerializedProperty          methodsList;
    SerializedProperty          inventoryIDList;
    SerializedProperty          b_feedbackActivated;
    SerializedProperty          feedbackIDList;



    public EditorMethods        editorMethods;                                         // access the component EditorMethods
    private bool                b_addMethods = false;                                  // use when you press button +



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
	//private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	//private Texture2D 		Tex_05;



	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 			= serializedObject.FindProperty ("SeeInspector");
        onlyFocusMode           = serializedObject.FindProperty("onlyFocusMode");

        b_feedbackActivated     = serializedObject.FindProperty("b_feedbackActivated");
        feedbackIDList          = serializedObject.FindProperty("feedbackIDList");

        conditionsToAccessThePuzzle myScript = (conditionsToAccessThePuzzle)target; 

        editorMethods = new EditorMethods();
        methodsList = serializedObject.FindProperty("methodsList");
        inventoryIDList = serializedObject.FindProperty("inventoryIDList");

        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .2f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_04 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .2f));
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
        GUIStyle style_Blue 			= new GUIStyle();	style_Blue.normal.background 			= Tex_01;
		GUIStyle style_Purple 			= new GUIStyle();	style_Purple.normal.background 			= Tex_04;
        GUIStyle style_Orange 			= new GUIStyle();	style_Orange.normal.background 			= Tex_01; 
		GUIStyle style_Yellow_Strong 	= new GUIStyle();	style_Yellow_Strong.normal.background 	= Tex_02;

        if (!onlyFocusMode.boolValue)
        {

            conditionsToAccessThePuzzle myScript = (conditionsToAccessThePuzzle)target;

            //-> Display Inventory ID needed to start the puzzle
            displayNeededInventoryID(style_Yellow_01);


            //-> Show Custom Methods
            EditorGUILayout.BeginVertical(style_Orange);
            showCustomMethods(style_Purple, style_Blue);
            EditorGUILayout.EndVertical();

            //-> Display feedback ID used when the puzzle is not available
            displayFeedbackWhenPuzzleIsLocked(style_Yellow_01);

        }

		serializedObject.ApplyModifiedProperties ();
		EditorGUILayout.LabelField ("");
	}

//--> Display custom methods used to know if the puzzle could be activated
    private void showCustomMethods(GUIStyle style_Purple,GUIStyle style_Blue){
        conditionsToAccessThePuzzle myScript = (conditionsToAccessThePuzzle)target; 
        //-> Custom methods
        EditorGUILayout.BeginVertical(style_Purple);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Methods checked to activate the puzzle", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Only bool Method are allowed. Other methods are ignored.", MessageType.Info);
        
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

//--> Display Inventory ID needed to start the puzzle
    private void displayNeededInventoryID(GUIStyle style_Yellow_01)
    {
        //-> Display feedback
        EditorGUILayout.BeginVertical(style_Yellow_01);
            EditorGUILayout.BeginVertical(style_Yellow_01);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Object needed in the Inventory : ", EditorStyles.boldLabel);

                if (GUILayout.Button("Open Window Tab: w_Item"))
                {
                    EditorWindow.GetWindow(typeof(w_Item));
                }
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (inventoryIDList.arraySize == 0)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    addNewNeededID(0);
                }
            }

            for (var i = 0; i < inventoryIDList.arraySize; i++)
            {
                SerializedProperty selectedID = inventoryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    addNewNeededID(i);
                    break;
                }
                if(inventoryIDList.arraySize >0){
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        removeSelectedID(i);
                        break;
                    } 
                }

                EditorGUILayout.LabelField("ID :", GUILayout.Width(30));

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(selectedID, new GUIContent(""), GUILayout.Width(30));
                if (EditorGUI.EndChangeCheck())
                {
                    updateInventoryObject(i, selectedID.intValue);
                }
               
                checkIfIDExist(selectedID);
                EditorGUILayout.EndHorizontal();


            }
        EditorGUILayout.EndVertical();
    }

//--> add New Needed ID
    private void addNewNeededID(int i){
        conditionsToAccessThePuzzle myScript = (conditionsToAccessThePuzzle)target;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        if(i == 0)
            myScript.inventoryIDList.Insert(i, new conditionsToAccessThePuzzle.idList());
        else
            myScript.inventoryIDList.Insert(i+1,new conditionsToAccessThePuzzle.idList());
    }

//--> remove Selected ID
    private void removeSelectedID(int i){
        conditionsToAccessThePuzzle myScript = (conditionsToAccessThePuzzle)target;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        myScript.inventoryIDList.RemoveAt(i);
    }

//--> checkIfIDExist
    private void checkIfIDExist(SerializedProperty selectedID){
        string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
        datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath(objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;


        string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/wItem.asset";
        TextList _textList = AssetDatabase.LoadAssetAtPath(objectPath, typeof(UnityEngine.Object)) as TextList;
        if (_textList)
        {
            SerializedObject serializedObject2 = new UnityEditor.SerializedObject(_textList);

            if (selectedID.intValue < serializedObject2.FindProperty("diaryList").GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").arraySize
                && selectedID.intValue >= 0)
            {
                SerializedProperty _Title = serializedObject2.FindProperty("diaryList").GetArrayElementAtIndex(0).FindPropertyRelative("_languageSlot").GetArrayElementAtIndex(selectedID.intValue).FindPropertyRelative("diaryTitle").GetArrayElementAtIndex(0);

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Item :", GUILayout.Width(40));
                    EditorGUILayout.LabelField(_Title.stringValue.ToString());
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("This ID doesn't exist", MessageType.Warning);

            }
        }
    }

//--> display Feedback When Puzzle Is Locked
    private void displayFeedbackWhenPuzzleIsLocked(GUIStyle style_Yellow_01){
        //--> Display feedback
        EditorGUILayout.BeginVertical(style_Yellow_01);

            EditorGUILayout.BeginVertical(style_Yellow_01);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Display feedback if the puzzle is locked : ", EditorStyles.boldLabel);

                if (GUILayout.Button("Open Window Tab: w_Feedback" ))
                {
                    EditorWindow.GetWindow(typeof(w_Feedback));
                }
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Feedback text : ", GUILayout.Width(90));

            EditorGUILayout.PropertyField(b_feedbackActivated, new GUIContent(""), GUILayout.Width(30));

            for (var i = 0; i < feedbackIDList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID :", GUILayout.Width(30));

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID"), new GUIContent(""), GUILayout.Width(30));
                if (EditorGUI.EndChangeCheck())
                {
                    updateFeedback(i, feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue);
                }
               
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }


//--> Update feedback ID using unique ID
    private void updateFeedback(int i, int value)
    {
        GameObject obj = GameObject.Find("ingameGlobalManager");
        TextList currentFeedback = Resources.Load(obj.GetComponent<ingameGlobalManager>().dataFolder.currentDatasProjectFolder + "/TextList/wFeedback") as TextList;

        if (currentFeedback)
        {
            feedbackIDList.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue
                = currentFeedback.diaryList[0]._languageSlot[value].uniqueItemID;
        }
    }
   
//--> Update inventory ID using unique ID
    private void updateInventoryObject(int i, int value)
    {
        GameObject obj = GameObject.Find("ingameGlobalManager");
        TextList currentInventory = Resources.Load(obj.GetComponent<ingameGlobalManager>().dataFolder.currentDatasProjectFolder + "/TextList/wItem") as TextList;

        if (currentInventory)
        {
            inventoryIDList.GetArrayElementAtIndex(i).FindPropertyRelative("uniqueID").intValue
            = currentInventory.diaryList[0]._languageSlot[value].uniqueItemID;
        }
    }


	void OnSceneGUI( )
	{
	}
}
#endif