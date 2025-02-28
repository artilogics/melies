// Description : Custom Editor for LogicsPuzzle.cs
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

[CustomEditor(typeof(LogicsPuzzle))]
public class LogicsPuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to drow default Inspector


    SerializedProperty HowManyLogics;
    SerializedProperty currentColor;
    SerializedProperty colorList;

    SerializedProperty LogicType;
    SerializedProperty LogicsUseOrFakeList;

    SerializedProperty LogicsInitPositionWhenStart;
    SerializedProperty LogicsAvailableWhenStart;
    SerializedProperty AxisAvailableWhenStart;

    //SerializedProperty LogicsSolutionList;
    //SerializedProperty linkLogics;
   

    SerializedProperty _Column;
    SerializedProperty pivotLogicList;
    SerializedProperty LogicList;
    SerializedProperty toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;
    SerializedProperty LogicsPositionList;

    SerializedProperty LogicsTypeList;
    SerializedProperty AxisTypeList;

    //SerializedProperty AxisRotationRight;

 
    SerializedProperty selectDefaultTile;
 
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
    public Color _cGreen = Color.green;

    public List<string> s_inputListJoystickButton = new List<string>();


    public Transform spriteTransform;   // Use to display key sprite
    public Text tmpText;        // use to display Key Text

    public string[] toolbarStrings = new string[] { "Puzzle Creation", "Puzzle Init Position","Sprite customization", "Game Options" };

    public string[] LogicsTypeStrings             = new string[] { "Empty", "Vertical","T","Elbow","No Move"};

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

    public EditorManipulate2DTexture manipulate2DTex;

    void OnEnable()
    {
        manipulate2DTex = new EditorManipulate2DTexture();
        // Setup the SerializedProperties.
        SeeInspector = serializedObject.FindProperty("SeeInspector");
        helpBoxEditor = serializedObject.FindProperty("helpBoxEditor");

        LogicsPuzzle myScript = (LogicsPuzzle)target;

        HowManyLogics = serializedObject.FindProperty("HowManyLogics");
        currentColor = serializedObject.FindProperty("currentColor");
        colorList = serializedObject.FindProperty("colorList");

        LogicType = serializedObject.FindProperty("LogicType");
        LogicsUseOrFakeList = serializedObject.FindProperty("LogicsUseOrFakeList");

        LogicsInitPositionWhenStart = serializedObject.FindProperty("LogicsInitPositionWhenStart");
        LogicsAvailableWhenStart = serializedObject.FindProperty("LogicsAvailableWhenStart");
        AxisAvailableWhenStart = serializedObject.FindProperty("AxisAvailableWhenStart");

        _Column = serializedObject.FindProperty("_Column");
        toolbarCurrentValue = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize = serializedObject.FindProperty("SquareSize");

        pivotLogicList = serializedObject.FindProperty("pivotLogicList");
        LogicList = serializedObject.FindProperty("LogicList");

        currentSelectedSprite = serializedObject.FindProperty("currentSelectedSprite");
        LogicsPositionList = serializedObject.FindProperty("LogicsPositionList");

        LogicsTypeList = serializedObject.FindProperty("LogicsTypeList");
        AxisTypeList = serializedObject.FindProperty("AxisTypeList");

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
        if (SeeInspector.boolValue)                         // If true Default Inspector is drown on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.LabelField("See Help Boxes :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(helpBoxEditor, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        GUIStyle style_Yellow_01 = new GUIStyle(); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(); style_Yellow_Strong.normal.background = Tex_02;

        LogicsPuzzle myScript = (LogicsPuzzle)target;


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
            if (pivotLogicList.arraySize > 0)
            {

                for (var i = 0; i < pivotLogicList.arraySize; i++)
                {
                    if (pivotLogicList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        b_TilesExist = false;
                        break;
                    }

                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (toolbarCurrentValue.intValue == 0){
                    loadLogicsPosition(myScript,  0);
                }
                  
                if (toolbarCurrentValue.intValue ==  1){
                    loadLogicsPosition(myScript, 1);
                }
            }
        



            // --> Display GeneratePuzzleSection
            if (toolbarCurrentValue.intValue == 0 && LogicsTypeList.arraySize == 0)
                displayGeneratePuzzleSection(myScript, style_Orange);

            // --> Display Other Section
          //  if (toolbarCurrentValue.intValue == 2)
            //    otherSection(myScript, style_Orange);

            // --> Display Other Section
            if (toolbarCurrentValue.intValue == 3)
                otherSection(myScript, style_Orange);


            if (pivotLogicList.arraySize > 0)
            {

                if (b_TilesExist)
                {
                    // --> Display Select Sprites
                    if (toolbarCurrentValue.intValue == 1)
                        displaySelectSpriteSection(myScript, style_Blue);
                    // --> Display Mix Section
                    if (toolbarCurrentValue.intValue == 0)
                        displaySolutionSection(myScript, style_Yellow_01);
                    if (toolbarCurrentValue.intValue == 2)
                        customizeSprite(myScript, style_Blue);
                }
                else
                {
                    if (toolbarCurrentValue.intValue == 0 || toolbarCurrentValue.intValue == 1)
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


    private void customizeSprite(LogicsPuzzle myScript, GUIStyle style_Blue)
    {
        EditorGUILayout.LabelField("");
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section :Select a tile and change its sprite.", MessageType.Info);

        _helpBox(1);

        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.LabelField("Current selected Tile : " + currentSelectedSprite.intValue, EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        if (LogicList.arraySize > currentSelectedSprite.intValue)
        {
            Transform[] ts = myScript.LogicList[currentSelectedSprite.intValue].GetComponentsInChildren<Transform>();


            bool b_SpriteExist = false;



            for (var i = 0; i < ts.Length; i++)
            {
                if (ts[i].name.Contains("Sprite"))
                {
                    spriteTransform = ts[i];
                    b_SpriteExist = true;
                    break;
                }
            }


            if (spriteTransform != null && b_SpriteExist)
            {
                GameObject objSprite = spriteTransform.gameObject;

                if (objSprite)
                {

                    EditorGUILayout.BeginHorizontal();
                    //-> Display srite thumbail 
                    Texture2D DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;
                    GUILayout.Label(DisplayTexture, GUILayout.Width(20), GUILayout.Height(20));

                    //-> Display sprite slot
                    SerializedObject serializedObject3 = new UnityEditor.SerializedObject(spriteTransform.GetComponent<SpriteRenderer>());
                    SerializedProperty m_Sprite = serializedObject3.FindProperty("m_Sprite");
                    serializedObject3.Update();
                    EditorGUILayout.PropertyField(m_Sprite, new GUIContent(""), GUILayout.Width(200));
                    serializedObject3.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();


                    //--> Display sprite Local scale
                    EditorGUILayout.BeginHorizontal();
                    SerializedObject serializedObject4 = new UnityEditor.SerializedObject(spriteTransform.transform);
                    SerializedProperty m_localScale = serializedObject4.FindProperty("m_LocalScale");
                    serializedObject4.Update();
                    EditorGUILayout.PropertyField(m_localScale, new GUIContent(""), GUILayout.Width(200));
                    serializedObject4.ApplyModifiedProperties();

                    if (GUILayout.Button("Apply to all", GUILayout.Width(80)))
                    {
                        for (var i = 0; i < LogicList.arraySize; i++)
                        {
                            Transform[] ts2 = myScript.LogicList[i].GetComponentsInChildren<Transform>();
                            spriteTransform = null;

                            for (var k = 0; k < ts2.Length; k++)
                            {
                                if (ts2[k].name.Contains("Grp_Sprite"))
                                {
                                    spriteTransform = ts2[k];
                                }
                            }

                            if (spriteTransform)
                            {
                                SerializedObject serializedObject5 = new UnityEditor.SerializedObject(spriteTransform.transform);
                                SerializedProperty m_localScale2 = serializedObject5.FindProperty("m_LocalScale");
                                serializedObject5.Update();
                                m_localScale2.vector3Value = m_localScale.vector3Value;
                                serializedObject5.ApplyModifiedProperties();
                            }

                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Sprites are available", MessageType.Info);
            }

        }

        EditorGUILayout.EndVertical();



        displayKeysInTheInspector(myScript, style_Blue, 2);



    }


    private void otherSection(LogicsPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
        validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when Logics is pressed : ", GUILayout.Width(200));
        EditorGUILayout.PropertyField(a_KeyPressed, new GUIContent(""), GUILayout.Width(100));
        GUILayout.Label("Volume : ", GUILayout.Width(60));
        a_KeyPressedVolume.floatValue = EditorGUILayout.Slider(a_KeyPressedVolume.floatValue, 0, 1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when puzzle is Reset : ", GUILayout.Width(200));
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

    private void displayGeneratePuzzleSection(LogicsPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.HelpBox("Section : Puzzle Creation." +
                                "\n\n-Press Button ''Generate the first Logic'' to start creating the puzzle.", MessageType.Info);

        EditorGUILayout.BeginVertical(style_Orange);
      
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Choose the number of Tile(s) : ", GUILayout.Width(200));
        EditorGUILayout.PropertyField(HowManyLogics, new GUIContent(""), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Choose the number of columns : ", GUILayout.Width(200));
        EditorGUILayout.PropertyField(_Column, new GUIContent(""), GUILayout.Width(50));
        GUILayout.Label("(minimum 1 column) ");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Puzzle"))
        {
            for(var i = 0;i < HowManyLogics.intValue;i++)
                GenerateKeys(myScript, 1, false);
        }
       
        EditorGUILayout.EndVertical();
    }


    private void displaySolutionSection(LogicsPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);

        EditorGUILayout.HelpBox("Section : Puzzle Creation. " +
                                "\n\n1-Select a Tile Type in the list by clicking on it (button between type number and its color)." +
                                "\n2-Click on a tile in the table to apply it."
                                , MessageType.Info);




        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Reset Puzzle (delete all Tiles)"))
        {
            DeleteLogic(myScript, 0);
        }

        GUILayout.Label("");

        EditorGUILayout.BeginVertical(style_Yellow_01);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add a new Color to the list"))
        {
            if(colorList.arraySize == 0){
                colorList.InsertArrayElementAtIndex(0);
                colorList.GetArrayElementAtIndex(0).colorValue = Color.white; 
            }
            else{
                colorList.InsertArrayElementAtIndex(colorList.arraySize - 1);
                colorList.GetArrayElementAtIndex(colorList.arraySize - 1).colorValue = Color.white; 
            }
           
        }

        EditorGUILayout.EndHorizontal();

        for (var i = 0; i < colorList.arraySize;i++){
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(i + " : ", GUILayout.Width(20));

            if (currentColor.intValue == i)
                GUI.backgroundColor = _cGreen;
            else
                GUI.backgroundColor = _cGray;
            if (GUILayout.Button("", GUILayout.Width(20)))
            {
                currentColor.intValue = i;
            }

            EditorGUILayout.PropertyField(colorList.GetArrayElementAtIndex(i), new GUIContent(""));

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = _cGray;


        EditorGUILayout.EndVertical();
     
       
        displayKeysInTheInspector(myScript, style_Yellow_01, 1);






    }

    private Color returnBackgroundColor(int value)
    {
        if (LogicType.intValue == value)
            return _cBlue;
        else
            return _cGray;
    }


    //--> return puzzle is solved
    private void returnIfPuzzleIsSolved(){
        //Debug.Log("No !!!");
    }


    private void displaySelectSpriteSection(LogicsPuzzle myScript, GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section : Puzzle Initial Position. " +
                                "\n\n2-Choose if the Tile position is the same as its axis by clicking the button ''Axis Pos/Init Pos''.", MessageType.Info);
        
        EditorGUILayout.EndVertical();
       
        displayKeysInTheInspector(myScript, style_Blue, 0);
    }


    //--> Display square that represent puzzle object
    public void displayKeysInTheInspector(LogicsPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");

        int number = 0;
        int row = Mathf.RoundToInt(pivotLogicList.arraySize / _Column.intValue);
            //EditorGUILayout.BeginHorizontal();

        for (var i = 0; i <= row; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < _Column.intValue; j++)
            {
                tmpText = null;
            if (pivotLogicList.arraySize > number)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));
                EditorGUILayout.BeginVertical();

                    if (WhichSection == 2) {        // Section : Sprite customization
                        Transform[] ts = myScript.LogicList[number].gameObject.GetComponentsInChildren<Transform>(true);

                       // Debug.Log("here : " + ts.Length);

                        Texture2D DisplayTexture = null;
                        Transform selectedChild = null;

                        foreach (Transform child in ts)
                        {
                            if (child != null && 
                                child.name.Contains("Grp_Sprite") && child != myScript.transform &&
                                child.GetComponent<SpriteRenderer>())
                            {
                                
                                DisplayTexture = (Texture2D)child.GetComponent<SpriteRenderer>().sprite.texture;
                                selectedChild = child.transform;
                            }
                        } 


                        EditorGUILayout.BeginVertical();

                        GUI.backgroundColor = colorList.GetArrayElementAtIndex(LogicsTypeList.GetArrayElementAtIndex(number).intValue).colorValue;
                        if (GUILayout.Button(DisplayTexture, GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                        {
                            currentSelectedSprite.intValue = number;

                         
                        } 
                        GUI.backgroundColor = _cGray;

                        if(selectedChild != null){
                            //-> Display sprite slot
                            SerializedObject serializedObject3 = new UnityEditor.SerializedObject(selectedChild.GetComponent<SpriteRenderer>());
                            SerializedProperty m_Sprite = serializedObject3.FindProperty("m_Sprite");
                            serializedObject3.Update();
                            EditorGUILayout.PropertyField(m_Sprite, new GUIContent(""), GUILayout.Width(SquareSize.intValue));
                            serializedObject3.ApplyModifiedProperties();
 
                        }
                        else{
                            EditorGUILayout.LabelField("Empty",GUILayout.Width(SquareSize.intValue));
                        }
                       



                        EditorGUILayout.EndVertical();

                    }
                    else{
                        GUI.backgroundColor = colorList.GetArrayElementAtIndex(LogicsTypeList.GetArrayElementAtIndex(number).intValue).colorValue;
                        if (GUILayout.Button("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                        {
                            currentSelectedSprite.intValue = number;

                            if (WhichSection == 1)          //-> Section : Puzzle Creation
                            {
                                LogicsTypeList.GetArrayElementAtIndex(number).intValue = currentColor.intValue;
                                Undo.RegisterFullObjectHierarchyUndo(myScript.LogicList[number], myScript.LogicList[number].name);
                                myScript.LogicList[number].GetComponent<Logic>().i_LogicType = currentColor.intValue;
                            }

                        } 
                        GUI.backgroundColor = _cGray;
                    }


                if (WhichSection == 0)          //-> Section : Puzzle Init Position
                {

                    string tmpSting = "";

                        if (LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue)
                            tmpSting = "Init Pos";
                        else
                            tmpSting = "Axis Pos";

                        if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                        if (LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue){
                            LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = false; 

                        }
                        else{
                            LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = true;
  
                        }

                            loadLogicsPosition(myScript, 1);
                        }


                    if (LogicsUseOrFakeList.GetArrayElementAtIndex(number).boolValue)
                    {
                        if (AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                            tmpSting = "Axis On";
                        else
                            tmpSting = "Axis Off";
                        if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                            if (AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = false;
                            else
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;

                            activateDeactivateAxis(myScript, number);

                        }

                        if (LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                            tmpSting = "Logic On";
                        else
                            tmpSting = "Logic Off";
                        if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                            if (LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                                LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = false;
                            else
                                LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;

                            activateDeactivateLogic(myScript, number);
                        }
                    }
                }


                if (WhichSection == 1)          //-> Section : Puzzle Creation
                    {
                        // EditorGUILayout.BeginHorizontal();
                        string s_LogicsUseOrFakeListState = "Fake";
                        if (!LogicsUseOrFakeList.GetArrayElementAtIndex(number).boolValue)
                            s_LogicsUseOrFakeListState = "Use";

                        if (GUILayout.Button(s_LogicsUseOrFakeListState, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                            if (LogicsUseOrFakeList.GetArrayElementAtIndex(number).boolValue)
                            {           // Fake
                                LogicsUseOrFakeList.GetArrayElementAtIndex(number).boolValue = false;
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                                LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = false;
                                LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                            }
                            else
                            {                                                                       // use
                                LogicsUseOrFakeList.GetArrayElementAtIndex(number).boolValue = true;
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                                LogicsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                                LogicsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                            }
                            activateDeactivateLogic(myScript, number);
                            activateDeactivateAxis(myScript, number);
                        }
                    }
                  

                    EditorGUILayout.EndVertical();

                    number++;

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
         }
    }


    private void activateDeactivateAxis(LogicsPuzzle myScript, int selectedObj)
    {
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);


        foreach (Transform child in ts)
        {
            if (child != null && child.name == selectedObj.ToString() && child.parent.name.Contains("Axis") && child != myScript.transform)
            {
                Undo.RegisterFullObjectHierarchyUndo(child.gameObject, child.name);
                if (AxisAvailableWhenStart.GetArrayElementAtIndex(selectedObj).boolValue)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false); 
            }
        }
    }

    private void activateDeactivateLogic(LogicsPuzzle myScript, int selectedObj)
    {
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);


        foreach (Transform child in ts)
        {
            if (child != null && child.name == selectedObj.ToString() && child.parent.name.Contains("Logics") && child != myScript.transform)
            {
                Undo.RegisterFullObjectHierarchyUndo(child.gameObject, child.name);
                if (LogicsAvailableWhenStart.GetArrayElementAtIndex(selectedObj).boolValue)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
    }


    private void ReplaceGrear(LogicsPuzzle myScript,int selectedLogic,int newLogic){
        //Debug.Log("NEw Logic : " + newLogic);

        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);

        GameObject tmpLogic = null;
        GameObject tmpAxis = null;

        foreach (Transform child in ts)
        {
            if (child != null && child.name == selectedLogic.ToString() && child.parent.name.Contains("Logics") && child != myScript.transform){
                GameObject newTile = Instantiate(myScript.defaultTileList[newLogic], child.parent.gameObject.transform);
                newTile.name = selectedLogic.ToString();
                Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                Undo.DestroyObjectImmediate(child.gameObject);
                tmpLogic = newTile;

                LogicList.GetArrayElementAtIndex(selectedLogic).objectReferenceValue = newTile.transform.gameObject;
            }
             
            if (child != null && child.name == selectedLogic.ToString() && child.parent.name.Contains("Axis") && child != myScript.transform)
            {
                int LogicDiamNumber = 0;                                     // Logic Pivot Size 1
                if (newLogic == 8 || newLogic == 9 || newLogic == 10)          // Logic Pivot Size 2
                {    
                    LogicDiamNumber = 1;
                    AxisTypeList.GetArrayElementAtIndex(selectedLogic).intValue = 2;
                }
                else if (newLogic == 11 || newLogic == 12 || newLogic == 13){  // Logic Pivot Size 3
                    LogicDiamNumber = 2;
                    AxisTypeList.GetArrayElementAtIndex(selectedLogic).intValue = 3;
                }   
                else if (newLogic == 14 || newLogic == 15){                   // Logic Pivot Size 4
                    LogicDiamNumber = 3;
                    AxisTypeList.GetArrayElementAtIndex(selectedLogic).intValue = 4;
                }
                else{
                    AxisTypeList.GetArrayElementAtIndex(selectedLogic).intValue = 1;
                }
                    

                GameObject newTile = Instantiate(myScript.defaultTileList[LogicDiamNumber], child.parent.gameObject.transform);
                newTile.name = selectedLogic.ToString();
                Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                Undo.DestroyObjectImmediate(child.gameObject);

                tmpAxis = newTile;

               
                newTile.transform.GetChild(0).GetChild(0).name = newTile.transform.GetChild(0).GetChild(0).name + "_" + selectedLogic;

                pivotLogicList.GetArrayElementAtIndex(selectedLogic).objectReferenceValue = newTile.transform.GetChild(0).GetChild(0).gameObject;


                LogicsUseOrFakeList.GetArrayElementAtIndex(selectedLogic).boolValue = false;
                LogicsInitPositionWhenStart.GetArrayElementAtIndex(selectedLogic).boolValue = true;
                LogicsAvailableWhenStart.GetArrayElementAtIndex(selectedLogic).boolValue = true;
                AxisAvailableWhenStart.GetArrayElementAtIndex(selectedLogic).boolValue = true;



            }
        }
        tmpLogic.transform.GetChild(0).transform.position = tmpAxis.transform.position;

    }

    private void GenerateKeys(LogicsPuzzle myScript,int howManyLogic, bool b_MultipleLogics)
    {
        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        //GameObject WhiteSpot = null;
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();
       
        int intListPosition = 0;
        if (pivotLogicList.arraySize != 0)
            intListPosition = pivotLogicList.arraySize - 1;

        pivotLogicList.InsertArrayElementAtIndex(intListPosition);
        LogicList.InsertArrayElementAtIndex(intListPosition);

        LogicsTypeList.InsertArrayElementAtIndex(intListPosition);
        AxisTypeList.InsertArrayElementAtIndex(intListPosition);

        LogicsUseOrFakeList.InsertArrayElementAtIndex(intListPosition);

        LogicsInitPositionWhenStart.InsertArrayElementAtIndex(intListPosition);
        LogicsAvailableWhenStart.InsertArrayElementAtIndex(intListPosition);
        AxisAvailableWhenStart.InsertArrayElementAtIndex(intListPosition);

        LogicsPositionList.InsertArrayElementAtIndex(intListPosition);

        string objName = "";
        GameObject tmpLogic = null;
        GameObject tmpAxis = null;

        for (var k = 0; k < 2; k++)
        {
            if(k == 0){
                objName = "Logics";
                selectDefaultTile.intValue = 0; // Default Logic
            }
            if (k == 1){
                objName = "Axis";
                selectDefaultTile.intValue = 1; // Default Logic
            }

            

            int row = Mathf.RoundToInt((pivotLogicList.arraySize - 1) / _Column.intValue);
            for (var i = 0; i < howManyLogic; i++)
            {
                GameObject newTile = Instantiate(myScript.defaultTileList[selectDefaultTile.intValue], myScript.gameObject.transform);

                if (k == 0){
                    tmpLogic = newTile;
                    newTile.transform.localPosition = new Vector3(.075f * ((LogicsTypeList.arraySize - 1) % _Column.intValue-1), .1f + .075f * row, 0);
                }
                else{
                    tmpAxis = newTile;
                    newTile.transform.localPosition = new Vector3(.075f * ((LogicsTypeList.arraySize - 1) % _Column.intValue - 1), -.075f * row, 0);
                }
                       

                    if (LogicsTypeList.arraySize <= 10)
                        newTile.name = objName + "_0" + (LogicsTypeList.arraySize - 1);
                    else
                        newTile.name = objName + "_" + (LogicsTypeList.arraySize - 1);

                    newTile.transform.GetChild(0).name = (LogicsTypeList.arraySize - 1).ToString();

                    Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                    ts = newTile.GetComponentsInChildren<Transform>();

                    foreach (Transform child in ts)
                    {
                        if (child != null && child.name.Contains("PIVOT_Logic"))
                        {
                            child.name += "_" + (LogicsTypeList.arraySize - 1).ToString();
                            pivotLogicList.GetArrayElementAtIndex(LogicsTypeList.arraySize - 1).objectReferenceValue = child.gameObject;
                        }

                        if (child != null && child.name.Contains("Logics_"))
                        {

                        LogicList.GetArrayElementAtIndex(LogicsTypeList.arraySize - 1).objectReferenceValue = child.transform.GetChild(0).gameObject;
                        }

                    }

                if (pivotLogicList.arraySize != 1)
                    intListPosition = pivotLogicList.arraySize-1;
                else
                    intListPosition = 0;
                
                LogicsTypeList.GetArrayElementAtIndex(intListPosition).intValue = 0;     // Gig Logic
                AxisTypeList.GetArrayElementAtIndex(intListPosition).intValue = 0;
            }
        }

        tmpLogic.transform.GetChild(0).transform.GetChild(0).transform.position = tmpAxis.transform.position;
    }

    public GameObject tmpDestroyLogic;

    private void DeleteLogic(LogicsPuzzle myScript, int LogicNumber)
    {
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);

        pivotLogicList.ClearArray();
        pivotLogicList.ClearArray();

        LogicList.ClearArray();
        LogicList.ClearArray();

        LogicsTypeList.ClearArray();
        AxisTypeList.ClearArray();

        LogicsUseOrFakeList.ClearArray();

        LogicsInitPositionWhenStart.ClearArray();
        LogicsAvailableWhenStart.ClearArray();
        AxisAvailableWhenStart.ClearArray();

        LogicsPositionList.ClearArray();

        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in ts)
            {
            if (child != null && child.name.Contains("Logics") && child != myScript.transform )
                    Undo.DestroyObjectImmediate(child.gameObject);
            if (child != null && child.name.Contains("Axis") && child != myScript.transform )
                    Undo.DestroyObjectImmediate(child.gameObject);
            }
       
    }


    private void ResetPosition(LogicsPuzzle myScript,SerializedProperty _PositionList, SerializedProperty _DirectionList){

        for (var i = 0; i < _PositionList.arraySize;i++){
            _PositionList.GetArrayElementAtIndex(i).intValue = 0;
        }

        for (var i = 0; i < LogicsPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.parent.name) == i)
                {
                   // objPIVOT = child;
                }
            }
        }

    }

    private void loadLogicsPosition(LogicsPuzzle myScript, int WhichSection)
    {
        for (var i = 0; i < LogicsPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("Logics_") && child != myScript.transform && int.Parse(child.GetChild(0).name) == i)
                {
                    // objPIVOT = child;
                    Undo.RegisterFullObjectHierarchyUndo(child.transform.GetChild(0).gameObject, child.transform.GetChild(0).name);
                    if (WhichSection == 0)  // Pivot Position
                    {
                        child.GetChild(0).transform.GetChild(0).transform.position = myScript.pivotLogicList[i].transform.position;
                       child.GetChild(0).transform.GetChild(0).transform.eulerAngles = myScript.pivotLogicList[i].transform.eulerAngles;
                       
                    }
                    else if (WhichSection == 1) // Outside position
                    {
                        if(LogicsInitPositionWhenStart.GetArrayElementAtIndex(i).boolValue){
                            child.GetChild(0).transform.GetChild(0).transform.localPosition = Vector3.zero;
                            child.GetChild(0).transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                        }
                        else{
                            child.GetChild(0).transform.GetChild(0).transform.position = myScript.pivotLogicList[i].transform.position; 
                            child.transform.transform.eulerAngles = myScript.pivotLogicList[i].transform.eulerAngles;
                  
                        }
                               

                        }
                    }
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
        string sType = "";
       // if (puzzleType.intValue == 1)
            sType = "Logic";
        //else
          //  sType = "Cercle";

        if (helpBoxEditor.boolValue)
        {
            switch (value)
            {
                case 0:
                    EditorGUILayout.HelpBox("1-Choose the number of " + sType +"s." +
                                            "\n2- Choose the number of column." +
                                            "\n3-Press button 'Generate " + sType + "' to create the puzzle.", MessageType.Info);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("1-Click on a Button below to access its parameters." +
                                            "\n2-Drag and drop a sprite in the slot next to the KEY thumbnail." +
                                            "\n3-Change its scale." +
                                            "\n4-Apply the same scale to all tiles by pressing button ''Apply to All''.", MessageType.Info);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("1-Press button 'Activate Mode' next to Choose Parent " + sType + ". Then choose the " + sType + " you want to link." +
                                            "\n(Press one of the square button to select a " + sType + ")" +
                                            "\n\n2-Press button 'Activate Mode' next to Choose Linked " + sType + ". Then choose the " + sType + " you want to link to the parent." +
                                            "\n (Press one of the square button to select a " + sType + ")." , MessageType.Info);
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