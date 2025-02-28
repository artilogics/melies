// Description : Custom Editor for LeverPuzzle.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(LeverPuzzle))]
public class LeverPuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector
     
    SerializedProperty HowManyLeverPosition;
    SerializedProperty startAngle;
    SerializedProperty totalMovement;
    SerializedProperty LeverSolutionList;
    SerializedProperty linkLever;
    SerializedProperty b_SelectLeversToLink;
    SerializedProperty lightList;

    SerializedProperty _NumberOfKey;
    SerializedProperty _Column;
    SerializedProperty tilesList;
    SerializedProperty toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;

    SerializedProperty LeverPositionList;

    SerializedProperty selectDefaultTile;
    SerializedProperty LeverDirectionUpList;
    SerializedProperty LeverDirectionUpSolutionList;

    SerializedProperty helpBoxEditor;
    SerializedProperty validationButtonJoystick;

    SerializedProperty a_KeyPressed;
    SerializedProperty a_KeyPressedVolume;
    SerializedProperty a_Reset;
    SerializedProperty a_ResetVolume;

    SerializedProperty popUpObject;
    SerializedProperty popupSpeed;

    SerializedProperty b_feedbackActivated;
    SerializedProperty feedbackIDList;

    public Color _cBlue = new Color(0f, .9f, 1f, .5f);
    public Color _cRed = new Color(1f, .5f, 0f, .5f);
    public Color _cGray = new Color(.9f, .9f, .9f, 1);



    public List<string> s_inputListJoystickButton = new List<string>();

    //bool b_Reset = false;

    public Transform spriteTransform;   // Use to display key sprite
    public Text tmpText;        // use to display Key Text
    public Transform objPIVOT;

    public string[] toolbarStrings = new string[] { "Generate","Link (Optional)", "Puzzle Solution", "Lever Init Position", "Game Options" };



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

    void OnEnable()
    {
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        helpBoxEditor = serializedObject.FindProperty("helpBoxEditor");

        LeverPuzzle myScript = (LeverPuzzle)target;

        HowManyLeverPosition = serializedObject.FindProperty("HowManyLeverPosition");
        startAngle = serializedObject.FindProperty("startAngle");
        totalMovement = serializedObject.FindProperty("totalMovement");
        LeverDirectionUpList = serializedObject.FindProperty("LeverDirectionUpList");
        LeverDirectionUpSolutionList = serializedObject.FindProperty("LeverDirectionUpSolutionList");

        LeverSolutionList = serializedObject.FindProperty("LeverSolutionList");
        linkLever = serializedObject.FindProperty("linkLever");
        b_SelectLeversToLink = serializedObject.FindProperty("b_SelectLeversToLink");
        lightList = serializedObject.FindProperty("lightList");


        _NumberOfKey = serializedObject.FindProperty("_NumberOfKey");

        _Column = serializedObject.FindProperty("_Column");
        toolbarCurrentValue = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize = serializedObject.FindProperty("SquareSize");

        tilesList = serializedObject.FindProperty("tilesList");

        currentSelectedSprite = serializedObject.FindProperty("currentSelectedSprite");
        LeverPositionList = serializedObject.FindProperty("LeverPositionList");

        selectDefaultTile = serializedObject.FindProperty("selectDefaultTile");

        validationButtonJoystick = serializedObject.FindProperty("validationButtonJoystick");

        a_KeyPressed = serializedObject.FindProperty("a_KeyPressed");
        a_KeyPressedVolume = serializedObject.FindProperty("a_KeyPressedVolume");
        a_Reset = serializedObject.FindProperty("a_Reset");
        a_ResetVolume = serializedObject.FindProperty("a_ResetVolume");

        popUpObject = serializedObject.FindProperty("popUpObject");
        popupSpeed = serializedObject.FindProperty("popupSpeed");

        feedbackIDList = serializedObject.FindProperty("feedbackIDList");
        b_feedbackActivated = serializedObject.FindProperty("b_feedbackActivated");

        GameObject tmp = GameObject.Find("InputsManager");
        if (tmp)
        {

            for (var i = 0; i < tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList.Count; i++)
            {
                s_inputListJoystickButton.Add(tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList[i].name);
            }

        }


        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
            Tex_03 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
            Tex_04 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
            Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
        }
    }


    public override void OnInspectorGUI()
    {
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.LabelField("See Help Boxes :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBoxEditor, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        GUIStyle style_Yellow_01 = new GUIStyle(GUI.skin.box); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(GUI.skin.box); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(GUI.skin.box); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(GUI.skin.box); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(GUI.skin.box); style_Yellow_Strong.normal.background = Tex_02;

        LeverPuzzle myScript = (LeverPuzzle)target;


        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Puzzle could not be edited in play mode", MessageType.Info);
        }
        else
        {
            // --> Display Tab sections in the Inspector
            EditorGUI.BeginChangeCheck();
            toolbarCurrentValue.intValue = GUILayout.Toolbar(toolbarCurrentValue.intValue, toolbarStrings);

            bool b_TilesExist = true;
            if (tilesList.arraySize > 0)
            {

                for (var i = 0; i < tilesList.arraySize; i++)
                {
                    if (tilesList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        b_TilesExist = false;
                        break;
                    }

                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                if ((toolbarCurrentValue.intValue == 0) || (toolbarCurrentValue.intValue == 1) || (toolbarCurrentValue.intValue == 2)){
                    loadLeverPosition(myScript, LeverSolutionList, LeverDirectionUpSolutionList);
                   // Debug.Log("Changes 3");
                   
                }
                   
                if (toolbarCurrentValue.intValue == 3 ){
                    loadLeverPosition(myScript, LeverPositionList, LeverDirectionUpList);
                    //Debug.Log("Changes 0 1 2");
                }
                    
            }
        



            // --> Display GeneratePuzzleSection
            if (toolbarCurrentValue.intValue == 0)
                displayGeneratePuzzleSection(myScript, style_Orange);

            // --> Display Link Section
            if (toolbarCurrentValue.intValue == 1)
                LinkSection(myScript, style_Orange);

            // --> Display Other Section
            if (toolbarCurrentValue.intValue == 4)
                otherSection(myScript, style_Orange);


            if (tilesList.arraySize > 0)
            {

                if (b_TilesExist)
                {
                    // --> Display Select Sprites
                    if (toolbarCurrentValue.intValue == 3)
                        displaySelectSpriteSection(myScript, style_Blue);
                    // --> Display Mix Section
                    if (toolbarCurrentValue.intValue == 2)
                        displaySolutionSection(myScript, style_Yellow_01);
                }
                else
                {
                    if (toolbarCurrentValue.intValue == 3 || toolbarCurrentValue.intValue == 2)
                        puzzleNeedToBeGenerated();
                }


            }

        }


        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");


    }

    private void puzzleNeedToBeGenerated()
    {
        EditorGUILayout.HelpBox("You need to generate the puzzle first.", MessageType.Error);

    }

    private void otherSection(LeverPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
        validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when Lever is pressed : ", GUILayout.Width(180));
        EditorGUILayout.PropertyField(a_KeyPressed, new GUIContent(""), GUILayout.Width(100));
        GUILayout.Label("Volume : ", GUILayout.Width(60));
        a_KeyPressedVolume.floatValue = EditorGUILayout.Slider(a_KeyPressedVolume.floatValue, 0, 1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when puzzle is Reset : ", GUILayout.Width(180));
        EditorGUILayout.PropertyField(a_Reset, new GUIContent(""), GUILayout.Width(100));
        GUILayout.Label("Volume : ", GUILayout.Width(60));
        a_ResetVolume.floatValue = EditorGUILayout.Slider(a_ResetVolume.floatValue, 0, 1);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("");

        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Actions when the puzzle start the first time : ", EditorStyles.boldLabel);


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("PopUp an object : ", GUILayout.Width(100));
        EditorGUILayout.PropertyField(popUpObject, new GUIContent(""), GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("PopUp Speed : ", GUILayout.Width(100));
        EditorGUILayout.PropertyField(popupSpeed, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        //-> Display feedback ID used when the puzzle is not available
        displayFeedbackWhenPuzzleIsLocked();



        EditorGUILayout.EndVertical();
    }

    private void displayGeneratePuzzleSection(LeverPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Generate Levers. (Minimum 1 Lever / 2 Positions)", MessageType.Info);
        _helpBox(0);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("How many Levers :", GUILayout.Width(150));
        EditorGUILayout.PropertyField(_NumberOfKey, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("How Many Lever Position :", GUILayout.Width(150));
        EditorGUILayout.PropertyField(HowManyLeverPosition, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Column :", GUILayout.Width(150));
        EditorGUILayout.PropertyField(_Column, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Choose Lever Type :", GUILayout.Width(150));
        if (GUILayout.Button("Next", GUILayout.Width(50)))
        {
            selectDefaultTile.intValue++;
            selectDefaultTile.intValue = selectDefaultTile.intValue % myScript.defaultTileList.Count;
        }

        string s_Type = myScript.defaultTileList[selectDefaultTile.intValue].name;
        s_Type = s_Type.Replace("Default", "");

        EditorGUILayout.LabelField("Current : " + s_Type);

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Generate Levers"))
        {
            GenerateKeys(myScript);
        }
        EditorGUILayout.EndVertical();

    }

    private void LinkSection(LeverPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("Section : Link Levers (optional).", MessageType.Info);

        _helpBox(2);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Choose Parent Lever :", GUILayout.Width(120));
        if(!b_SelectLeversToLink.boolValue)
            GUI.backgroundColor = _cRed;
        else
            GUI.backgroundColor = _cGray;
        if (GUILayout.Button("Activate Mode", GUILayout.Width(120)))
        {
            b_SelectLeversToLink.boolValue = false;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Choose Linked Lever :", GUILayout.Width(120));

        if (b_SelectLeversToLink.boolValue)
            GUI.backgroundColor = _cBlue;
        else
            GUI.backgroundColor = _cGray;
        if (GUILayout.Button("Activate Mode", GUILayout.Width(120)))
        {
            b_SelectLeversToLink.boolValue = true;
        }
        EditorGUILayout.EndHorizontal();

      
        EditorGUILayout.EndVertical();


        displayLeverInTheInspectorSectionLink(myScript, style_Yellow_01, 0);

        EditorGUILayout.LabelField("");


    }

    private void displaySolutionSection(LeverPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("Section : Puzzle Solution. \nCreate the solution by pressing the the squares below.(Show modification in scene view).", MessageType.Info);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Solution"))
        {
            ResetPosition(myScript, LeverSolutionList, LeverDirectionUpSolutionList);
           
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("");

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Yellow_01, 1);

        EditorGUILayout.LabelField("");
    }

    private void displaySelectSpriteSection(LeverPuzzle myScript, GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section : Levers Initial Position. \nCreate the levers initial position by pressing the the squares below.(Show modification in scene view).", MessageType.Info);

        if (GUILayout.Button("Reset Position"))
        {
            ResetPosition(myScript,LeverPositionList,LeverDirectionUpList);
        }

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Blue, 0);
    }

    public void displayKeysInTheInspector(LeverPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");
        int number = 0;

        int raw = Mathf.RoundToInt(_NumberOfKey.intValue / _Column.intValue);


        //Debug.Log("raw : " + raw +  " : _Column :" + _Column.intValue + " : _NumberOfKey :" + _NumberOfKey.intValue);
        for (var i = 0; i <= raw; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < _Column.intValue; j++)
            {

                tmpText = null;
                if (tilesList.arraySize > number)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));


                    if (GUILayout.Button("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                    {
                        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

                        foreach (Transform child in ts)
                        {
                            if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.name) == number)
                            {
                                objPIVOT = child;
                            }
                        }

                        currentSelectedSprite.intValue = number;

                        float step = totalMovement.intValue / (HowManyLeverPosition.intValue - 1);


                       if (WhichSection == 0)          //-> Section : Lever Init Position
                            {
                                Undo.RegisterFullObjectHierarchyUndo(objPIVOT, objPIVOT.name);

                                SerializedProperty position = LeverPositionList.GetArrayElementAtIndex(currentSelectedSprite.intValue);
                                SerializedProperty directionUp = LeverDirectionUpList.GetArrayElementAtIndex(currentSelectedSprite.intValue);

                                moveLever(myScript, position, directionUp, step, false);        // Move Parent Lever
                                ap_MoveLinkedLever(myScript, step, 3);                          // Move Linked levers
                              
                            }

                            if (WhichSection == 1)          //-> Section : Solution 
                            {
                                Undo.RegisterFullObjectHierarchyUndo(objPIVOT, objPIVOT.name);
                                SerializedProperty position = LeverSolutionList.GetArrayElementAtIndex(currentSelectedSprite.intValue);
                                SerializedProperty directionUp = LeverDirectionUpSolutionList.GetArrayElementAtIndex(currentSelectedSprite.intValue);

                                moveLever(myScript, position, directionUp, step, false);        // Move Parent Lever
                                ap_MoveLinkedLever(myScript, step, 2);                          // Move Linked levers
                       }
                    }
                    number++;

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

    }

    private void moveLever(LeverPuzzle myScript, SerializedProperty position, SerializedProperty directionUp, float step,bool b_OnlyMoveLever)
    {
        if (directionUp.boolValue)
        {
            if(!b_OnlyMoveLever)position.intValue--;
            //Debug.Log("Here : " + objPIVOT.name + " : " + newAngle);
        }
        else
        {
            if (!b_OnlyMoveLever)position.intValue++;
            //Debug.Log("Here : " + objPIVOT.name + " : " + newAngle);
        }


        if (!b_OnlyMoveLever)
        {
            if (position.intValue == 0)
            {
                directionUp.boolValue = false;

            }


            if (position.intValue == HowManyLeverPosition.intValue - 1)
            {
                directionUp.boolValue = true;
            }
        }

        float newAngle = startAngle.intValue - step * position.intValue;
        objPIVOT.localEulerAngles = new Vector3(newAngle, 0, 0);

    }

    private void GenerateKeys(LeverPuzzle myScript)
    {
        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in ts)
        {
            if (child != null && child.name.Contains("Lever") && child != myScript.transform)
            {
                //if (tilesList.GetArrayElementAtIndex(i).objectReferenceValue)
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }


        tilesList.ClearArray();
        LeverPositionList.ClearArray();
        LeverDirectionUpList.ClearArray();
        LeverSolutionList.ClearArray();
        LeverDirectionUpSolutionList.ClearArray();
        linkLever.ClearArray();
        lightList.ClearArray();

        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            tilesList.InsertArrayElementAtIndex(0);
            LeverPositionList.InsertArrayElementAtIndex(0);
            LeverDirectionUpList.InsertArrayElementAtIndex(0);
            LeverSolutionList.InsertArrayElementAtIndex(0);
            LeverDirectionUpSolutionList.InsertArrayElementAtIndex(0);
            linkLever.InsertArrayElementAtIndex(0);
            lightList.InsertArrayElementAtIndex(0);
        }

        int number = 0;
        int raw = 0;
        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            GameObject newTile = Instantiate(myScript.defaultTileList[selectDefaultTile.intValue], myScript.gameObject.transform);

            if (raw != 0)
                newTile.transform.localPosition = new Vector3(.19f * (i % _Column.intValue), -.25f * raw, 0);
            else
                newTile.transform.localPosition = new Vector3(.19f * i, -.25f * raw, 0);

            //Debug.Log("number : " + number);
            if (number < 10)
                newTile.name = "Lever_0" + number;
            else
                newTile.name = "Lever_" + number;


            newTile.transform.GetChild(0).name = number.ToString();

            Undo.RegisterCreatedObjectUndo(newTile, newTile.name);


            ts = newTile.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT_Lever"))
                {
                    child.name += "_" + number.ToString();
                    tilesList.GetArrayElementAtIndex(number).objectReferenceValue = child.gameObject;
                   
                }

                if (child != null && child.name.Contains("Lever_Light_Feedback"))
                {
                    child.name += "_" + number.ToString();
                    lightList.GetArrayElementAtIndex(number).objectReferenceValue = child.GetComponent<MeshRenderer>();
                   
                }
            }

            if (i % _Column.intValue == _Column.intValue - 1)
                raw++;

            number++;
        }

        ResetPosition(myScript, LeverPositionList, LeverDirectionUpList);
        ResetPosition(myScript, LeverSolutionList, LeverDirectionUpSolutionList);
    }

    private void ResetPosition(LeverPuzzle myScript,SerializedProperty _PositionList, SerializedProperty _DirectionList){

        for (var i = 0; i < _PositionList.arraySize;i++){
            //Debug.Log("here");
            _PositionList.GetArrayElementAtIndex(i).intValue = 0;
            _DirectionList.GetArrayElementAtIndex(i).boolValue = false;
        }


        for (var i = 0; i < LeverPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.name) == i)
                {
                    objPIVOT = child;
                }
            }

            moveLever(myScript, _PositionList.GetArrayElementAtIndex(i), _DirectionList.GetArrayElementAtIndex(i), 0,true);
        }

    }

    private void loadLeverPosition(LeverPuzzle myScript, SerializedProperty _PositionList, SerializedProperty _DirectionList)
    {
        for (var i = 0; i < LeverPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.name) == i)
                {
                    objPIVOT = child;
                }
            }

            float step = totalMovement.intValue / (HowManyLeverPosition.intValue - 1);

            float newAngle = startAngle.intValue - step * _PositionList.GetArrayElementAtIndex(i).intValue;
            objPIVOT.localEulerAngles = new Vector3(newAngle, 0, 0);

        }
    }

    public void displayLeverInTheInspectorSectionLink(LeverPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");

        int number = 0;

        int raw = Mathf.RoundToInt(_NumberOfKey.intValue / _Column.intValue);

        //Debug.Log("raw : " + raw +  " : _Column :" + _Column.intValue + " : _NumberOfKey :" + _NumberOfKey.intValue);
        for (var i = 0; i <= raw; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < _Column.intValue; j++)
            {

                tmpText = null;
                if (tilesList.arraySize > number)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));

                    if(ap_checkIfLeverIsLinked(myScript, number))
                        GUI.backgroundColor = _cBlue;
                    else if (currentSelectedSprite.intValue == number)
                        GUI.backgroundColor = _cRed;
                    else
                        GUI.backgroundColor = _cGray;
                    if (GUILayout.Button("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                    {
                   
                        if (b_SelectLeversToLink.boolValue){            // Select linked lever
                            ap_linkLever(myScript,number);
                        }
                        else{                                           // Select Master lever
                            currentSelectedSprite.intValue = number;
                        }
                    }
                    number++;

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private bool ap_checkIfLeverIsLinked(LeverPuzzle myScript, int number)
    {
        bool result = false;
        for (var i = 0; i < myScript.linkLever[currentSelectedSprite.intValue]._leverList.Count;i++){
            if(myScript.linkLever[currentSelectedSprite.intValue]._leverList[i] == number){
                result = true;
                break;
             }
        }

        return result;
    }

    private void ap_linkLever(LeverPuzzle myScript,int number){
        if (currentSelectedSprite.intValue != number)
        {
            Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
            //Debug.Log("Add Linked Lever : " + number);

            bool result = false;
            int listPosition = 0;
            for (var i = 0; i < myScript.linkLever[currentSelectedSprite.intValue]._leverList.Count; i++)
            {
                if (myScript.linkLever[currentSelectedSprite.intValue]._leverList[i] == number)
                {
                    result = true;
                    listPosition = i;
                    break;
                }
            }

            if (result)
                myScript.linkLever[currentSelectedSprite.intValue]._leverList.RemoveAt(listPosition);
            else
                myScript.linkLever[currentSelectedSprite.intValue]._leverList.Add(number); 
        }
       
    }



    private void ap_MoveLinkedLever(LeverPuzzle myScript,float step,int WhichSection)
    {
        SerializedProperty leverList = linkLever.GetArrayElementAtIndex(currentSelectedSprite.intValue).FindPropertyRelative("_leverList");



        for (var i = 0; i < leverList.arraySize; i++){
           // Debug.Log("leverList : " + leverList.GetArrayElementAtIndex(i).intValue);

            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") 
                    && child != myScript.transform 
                    && int.Parse(child.parent.name) == leverList.GetArrayElementAtIndex(i).intValue)
                {
                    objPIVOT = child;
                }
            }

            if(WhichSection == 2){      // Section : Solution 
                moveLever(myScript,
                     LeverSolutionList.GetArrayElementAtIndex(leverList.GetArrayElementAtIndex(i).intValue),
                     LeverDirectionUpSolutionList.GetArrayElementAtIndex(leverList.GetArrayElementAtIndex(i).intValue),
                     step,
                     false);
            }
            if (WhichSection == 3)      // Section :Lever Init Position
            {
                moveLever(myScript,
                     LeverPositionList.GetArrayElementAtIndex(leverList.GetArrayElementAtIndex(i).intValue),
                     LeverDirectionUpList.GetArrayElementAtIndex(leverList.GetArrayElementAtIndex(i).intValue),
                     step,
                     false);
            }
        }
    }

    //--> display Feedback When Puzzle Is Locked
    private void displayFeedbackWhenPuzzleIsLocked()
    {
        //--> Display feedback
        EditorGUILayout.BeginVertical();
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

            if (GUILayout.Button("Open Window Tab: w_Feedback"))
            {
                EditorWindow.GetWindow(typeof(w_Feedback));
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



    void OnSceneGUI()
    {
    }

    public void _helpBox(int value)
    {
        if (helpBoxEditor.boolValue)
        {
            switch (value)
            {
                case 0:
                    EditorGUILayout.HelpBox("1-Choose the number of Levers." +
                                            "\n2-Choose the number of positions for a lever." +
                                            "\n3-Choose the number of Column." +
                                            "\n4- Choose the lever 3D model Type." +
                                            "\n5-Press button 'Generate Levers' to create the puzzle.", MessageType.Info);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("1-Click on a Button below to access his parameters." +
                                            "\n2-Drag and drop a sprite in the slot next to the KEY thumbnail." +
                                            "\n3-Change his scale." +
                                            "\n4-Apply the same scale to all tiles by pressing button ''Apply to All''." + 
                                            "\n5-Choose the text displayed in the scene view inside the KEY." +
                                            "\n6-Choose the value displayed on the result screen in the scene view.", MessageType.Info);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("1-Press button 'Activate Mode' next to Choose Parent Lever. Then choose the lever you want to link." +
                                            "\n(Press one of the square button to select a lever)" +
                                            "\n\n3-Press button 'Activate Mode' next to Choose Linked Lever. Then choose the lever you want to link to the parent." +
                                            "\n(Press one of the square button to select a lever)." , MessageType.Info);
                    break;
            }
        }
    }

   /* void OnInspectorUpdate()
    {
        Repaint();
    }
*/

}
#endif