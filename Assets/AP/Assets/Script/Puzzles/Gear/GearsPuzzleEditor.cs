// Description : Custom Editor for GearsPuzzle.cs
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

[CustomEditor(typeof(GearsPuzzle))]
public class GearsPuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector

    SerializedProperty GearType;
    SerializedProperty GearsUseOrFakeList;

    SerializedProperty GearsInitPositionWhenStart;
    SerializedProperty GearsAvailableWhenStart;
    SerializedProperty AxisAvailableWhenStart;

    SerializedProperty GearsSolutionList; 

    SerializedProperty _Column;
    SerializedProperty pivotGearList;
    SerializedProperty GearList;
    SerializedProperty toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;
    SerializedProperty GearsPositionList;

    SerializedProperty GearsTypeList;
    SerializedProperty AxisTypeList;

    SerializedProperty AxisRotationRight;

 
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

    public string[] toolbarStrings = new string[] { "Puzzle Creation", "Puzzle Init Position", "Game Options" };

    public string[] GearsTypeStrings             = new string[] { "Empty", "Vertical","T","Elbow","No Move"};

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
        manipulate2DTex             = new EditorManipulate2DTexture();
        // Setup the SerializedProperties.
        SeeInspector                = serializedObject.FindProperty("SeeInspector");
        helpBoxEditor               = serializedObject.FindProperty("helpBoxEditor");

        GearsPuzzle myScript = (GearsPuzzle)target;

        GearType                    = serializedObject.FindProperty("GearType");
        GearsUseOrFakeList          = serializedObject.FindProperty("GearsUseOrFakeList");

        GearsInitPositionWhenStart  = serializedObject.FindProperty("GearsInitPositionWhenStart");
        GearsAvailableWhenStart     = serializedObject.FindProperty("GearsAvailableWhenStart");
        AxisAvailableWhenStart      = serializedObject.FindProperty("AxisAvailableWhenStart");

        GearsSolutionList           = serializedObject.FindProperty("GearsSolutionList");
        _Column                     = serializedObject.FindProperty("_Column");
        toolbarCurrentValue         = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize                  = serializedObject.FindProperty("SquareSize");

        pivotGearList               = serializedObject.FindProperty("pivotGearList");
        GearList                    = serializedObject.FindProperty("GearList");

        currentSelectedSprite       = serializedObject.FindProperty("currentSelectedSprite");
        GearsPositionList           = serializedObject.FindProperty("GearsPositionList");

        GearsTypeList               = serializedObject.FindProperty("GearsTypeList");
        AxisTypeList                = serializedObject.FindProperty("AxisTypeList");

        AxisRotationRight           = serializedObject.FindProperty("AxisRotationRight");

     
        selectDefaultTile           = serializedObject.FindProperty("selectDefaultTile");

        validationButtonJoystick    = serializedObject.FindProperty("validationButtonJoystick");

        a_KeyPressed                = serializedObject.FindProperty("a_KeyPressed");
        a_KeyPressedVolume          = serializedObject.FindProperty("a_KeyPressedVolume");
        a_Reset                     = serializedObject.FindProperty("a_Reset");
        a_ResetVolume               = serializedObject.FindProperty("a_ResetVolume");

        popUpObject                 = serializedObject.FindProperty("popUpObject");
        popupSpeed                  = serializedObject.FindProperty("popupSpeed");

        feedbackIDList              = serializedObject.FindProperty("feedbackIDList");
        b_feedbackActivated         = serializedObject.FindProperty("b_feedbackActivated");
    
        GameObject tmp = GameObject.Find("InputsManager");
        if (tmp)
        {
            for (var i = 0; i < tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList.Count; i++)
            {s_inputListJoystickButton.Add(tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList[i].name);}
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

        GUIStyle style_Yellow_01 = new GUIStyle(); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(); style_Yellow_Strong.normal.background = Tex_02;

        GearsPuzzle myScript = (GearsPuzzle)target;


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
            if (pivotGearList.arraySize > 0)
            {

                for (var i = 0; i < pivotGearList.arraySize; i++)
                {
                    if (pivotGearList.GetArrayElementAtIndex(i).objectReferenceValue == null)
                    {
                        b_TilesExist = false;
                        break;
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (toolbarCurrentValue.intValue == 0){
                    loadGearsPosition(myScript,  0);
                }
                  
                if (toolbarCurrentValue.intValue ==  1){
                    loadGearsPosition(myScript, 1);
                }
            }
        
            // --> Display GeneratePuzzleSection
            if (toolbarCurrentValue.intValue == 0 && GearsTypeList.arraySize == 0)
                displayGeneratePuzzleSection(myScript, style_Orange);

            // --> Display Other Section
            if (toolbarCurrentValue.intValue == 2)
                otherSection(myScript, style_Orange);


            if (pivotGearList.arraySize > 0)
            {

                if (b_TilesExist)
                {
                    // --> Display Select Sprites
                    if (toolbarCurrentValue.intValue == 1)
                        displaySelectSpriteSection(myScript, style_Blue);
                    // --> Display Mix Section
                    if (toolbarCurrentValue.intValue == 0)
                        displaySolutionSection(myScript, style_Yellow_01);
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

    private void otherSection(GearsPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
        validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when Gears is pressed : ", GUILayout.Width(180));
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

    private void displayGeneratePuzzleSection(GearsPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.HelpBox("Section : Puzzle Creation." +
                                "\n\n-Press Button ''Generate the first Gear'' to start creating the puzzle.", MessageType.Info);

        EditorGUILayout.BeginVertical(style_Orange);
      
      
        if (GUILayout.Button("Generate the first Gear"))
        {
            GenerateKeys(myScript, 1, false);
        }
       
        EditorGUILayout.EndVertical();
    }

    private void displaySolutionSection(GearsPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);

        EditorGUILayout.HelpBox("Section : Puzzle Creation. " +
                                "\n\n1-Select a Gear Type in the list by clicking on it." +
                                "\n2-Click on a tile in the table to apply it." +
                                "\n" +
                                "\n3-If you want to use the gear Axis in the solution the button bellow the gear button need to display ''Use''" +
                                "\n4-If you want to use the gear Axis as a fake Axis the button bellow the gear button need to display ''Fake''" +
                                "\n" 
                                , MessageType.Info);

        //_helpBox(2);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Gear"))
        {
            GenerateKeys(myScript, 1, false);
        }
       
        if (GUILayout.Button("Delete all Gears"))
        {
            DeleteGear(myScript, 0);
        }

        EditorGUILayout.EndHorizontal();

        //-> Diameter 1
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pivot Size ", GUILayout.Width(60));
        EditorGUILayout.LabelField("| Gear Size ", GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("1 ->", GUILayout.Width(30));
        GUILayout.Label(myScript.GearSprite[0], GUILayout.Width(30), GUILayout.Height(30));
        GUI.backgroundColor = returnBackgroundColor(5);
        if (GUILayout.Button(myScript.GearSprite[5], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 5;
        }
        GUI.backgroundColor = returnBackgroundColor(6);
        if (GUILayout.Button(myScript.GearSprite[6], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 6;
        }
        GUI.backgroundColor = returnBackgroundColor(7);
        if (GUILayout.Button(myScript.GearSprite[7], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 7;
        }

       
        GUI.backgroundColor = _cGray;
        EditorGUILayout.EndHorizontal();

        //-> Diameter 2

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("2 ->", GUILayout.Width(30));
        GUILayout.Label(myScript.GearSprite[1], GUILayout.Width(30), GUILayout.Height(30));
        GUI.backgroundColor = returnBackgroundColor(8);
        if (GUILayout.Button(myScript.GearSprite[8], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 8;
        }
        GUI.backgroundColor = returnBackgroundColor(9);
        if (GUILayout.Button(myScript.GearSprite[9], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 9;
        }
        GUI.backgroundColor = returnBackgroundColor(10);
        if (GUILayout.Button(myScript.GearSprite[10], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 10;
        }


        GUI.backgroundColor = _cGray;
        EditorGUILayout.EndHorizontal();

        //-> Diameter 3

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("3 ->", GUILayout.Width(30));
        GUILayout.Label(myScript.GearSprite[2], GUILayout.Width(30), GUILayout.Height(30));
        EditorGUILayout.LabelField("", GUILayout.Width(30));
        GUI.backgroundColor = returnBackgroundColor(11);
        if (GUILayout.Button(myScript.GearSprite[11], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 11;
        }
        GUI.backgroundColor = returnBackgroundColor(12);
        if (GUILayout.Button(myScript.GearSprite[12], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 12;
        }
        GUI.backgroundColor = returnBackgroundColor(13);
        if (GUILayout.Button(myScript.GearSprite[13], GUILayout.Width(30), GUILayout.Height(30)))
        {
            GearType.intValue = 13;
        }
        EditorGUILayout.EndHorizontal();

            //-> Diameter 4

            EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("4 ->", GUILayout.Width(30));
            GUILayout.Label(myScript.GearSprite[3], GUILayout.Width(30), GUILayout.Height(30));
            EditorGUILayout.LabelField("", GUILayout.Width(64));
            GUI.backgroundColor = returnBackgroundColor(14);
        if (GUILayout.Button(myScript.GearSprite[14], GUILayout.Width(30), GUILayout.Height(30)))
            {
                GearType.intValue = 14;
            }
            GUI.backgroundColor = returnBackgroundColor(15);
        if (GUILayout.Button(myScript.GearSprite[15], GUILayout.Width(30), GUILayout.Height(30)))
            {
                GearType.intValue = 15;
            }
            EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = _cGray;

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Yellow_01, 1);
    }

    private Color returnBackgroundColor(int value)
    {
        if (GearType.intValue == value)
            return _cBlue;
        else
            return _cGray;
    }


    //--> return puzzle is solved
    private void returnIfPuzzleIsSolved(){
        //Debug.Log("No !!!");
    }


    private void displaySelectSpriteSection(GearsPuzzle myScript, GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section : Puzzle Initial Position. " +
                                "\n\n1-Choose the gear rotation by clicking the button ''Left Rot/Right Rot''." +
                                "\n2-Choose if the gear position is the same as his axis by clicking the button ''Axis Pos/Init Pos''." +
                                "\n3-Deactivate/Activate a Axis by clicking the button ''Axis on/Axis Off''." +
                                "\n4-Deactivate/Activate a Gear by clicking the button ''Gear on/Gear Off''." +
                                "\n", MessageType.Info);
        
        EditorGUILayout.EndVertical();
       
        displayKeysInTheInspector(myScript, style_Blue, 0);
    }


    //--> Display square that represent puzzle object
    public void displayKeysInTheInspector(GearsPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");

        int number = 0;

            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < GearsPositionList.arraySize; j++)
            {
                tmpText = null;
            if (pivotGearList.arraySize > number)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));
                EditorGUILayout.BeginVertical();

                if (GUILayout.Button(myScript.GearSprite[GearsTypeList.GetArrayElementAtIndex(number).intValue], GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                //if (GUILayout.Button(myScript.GearSprite[GearsTypeList.GetArrayElementAtIndex(number).intValue], GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                {
                    Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();
                    foreach (Transform child in ts)
                    {
                        if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.parent.name) == number)
                        {
                            //objPIVOT = child;
                        }
                    }
                    currentSelectedSprite.intValue = number;

                    if (WhichSection == 1)          //-> Section : Puzzle Creation
                    {
                        GearsTypeList.GetArrayElementAtIndex(number).intValue = GearType.intValue;
                        ReplaceGrear(myScript, number, GearsTypeList.GetArrayElementAtIndex(number).intValue);
                    }

                    if (WhichSection == 0)          //-> Section : Puzzle Init Position
                    {
                        
                    }
                }


                if (WhichSection == 1)          //-> Section : Puzzle Creation
                {
                   // EditorGUILayout.BeginHorizontal();
                        string s_GearsUseOrFakeListState = "Fake";
                        if (!GearsUseOrFakeList.GetArrayElementAtIndex(number).boolValue)
                            s_GearsUseOrFakeListState = "Use";

                        if (GUILayout.Button(s_GearsUseOrFakeListState, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                            if (GearsUseOrFakeList.GetArrayElementAtIndex(number).boolValue){           // Fake
                                GearsUseOrFakeList.GetArrayElementAtIndex(number).boolValue = false;
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                                GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                            }
                            else{                                                                       // use
                                GearsUseOrFakeList.GetArrayElementAtIndex(number).boolValue = true;
                                AxisAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                                GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;
                            }
                            activateDeactivateGear(myScript, number);
                            activateDeactivateAxis(myScript, number);
                        }
                }
                if (WhichSection == 0)          //-> Section : Puzzle Init Position
                {

                    string tmpSting = "";

                    if (AxisRotationRight.GetArrayElementAtIndex(number).boolValue)
                        tmpSting = "Right Rot";
                    else
                        tmpSting = "Left Rot";

                    if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                    {
                        if (AxisRotationRight.GetArrayElementAtIndex(number).boolValue)
                        {
                            AxisRotationRight.GetArrayElementAtIndex(number).boolValue = false;

                        }
                        else
                        {
                            AxisRotationRight.GetArrayElementAtIndex(number).boolValue = true;

                        }
                    }

                    tmpSting = "";

                        if (GearsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue)
                            tmpSting = "Init Pos";
                        else
                            tmpSting = "Axis Pos";

                        if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                        if (GearsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue){
                            GearsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = false; 

                        }
                        else{
                            GearsInitPositionWhenStart.GetArrayElementAtIndex(number).boolValue = true;
  
                        }

                            loadGearsPosition(myScript, 1);
                        }


                    if (GearsUseOrFakeList.GetArrayElementAtIndex(number).boolValue)
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

                        if (GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                            tmpSting = "Gear On";
                        else
                            tmpSting = "Gear Off";
                        if (GUILayout.Button(tmpSting, GUILayout.Width(SquareSize.intValue), GUILayout.Height(20)))
                        {
                            if (GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue)
                                GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = false;
                            else
                                GearsAvailableWhenStart.GetArrayElementAtIndex(number).boolValue = true;

                            activateDeactivateGear(myScript, number);
                        }
                    }
                }

                    EditorGUILayout.EndVertical();

                    number++;

                    EditorGUILayout.EndVertical();
                }
            }
        

            EditorGUILayout.EndHorizontal();

           

    }


    private void activateDeactivateAxis(GearsPuzzle myScript, int selectedObj)
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

    private void activateDeactivateGear(GearsPuzzle myScript, int selectedObj)
    {
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);


        foreach (Transform child in ts)
        {
            if (child != null && child.name == selectedObj.ToString() && child.parent.name.Contains("Gears") && child != myScript.transform)
            {
                Undo.RegisterFullObjectHierarchyUndo(child.gameObject, child.name);
                if (GearsAvailableWhenStart.GetArrayElementAtIndex(selectedObj).boolValue)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
    }


    private void ReplaceGrear(GearsPuzzle myScript,int selectedGear,int newGear){
        //Debug.Log("NEw Gear : " + newGear);

        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);

        GameObject tmpGear = null;
        GameObject tmpAxis = null;

        foreach (Transform child in ts)
        {
            if (child != null && child.name == selectedGear.ToString() && child.parent.name.Contains("Gears") && child != myScript.transform){
                GameObject newTile = Instantiate(myScript.defaultTileList[newGear], child.parent.gameObject.transform);
                newTile.name = selectedGear.ToString();
                Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                Undo.DestroyObjectImmediate(child.gameObject);
                tmpGear = newTile;

                GearList.GetArrayElementAtIndex(selectedGear).objectReferenceValue = newTile.transform.gameObject;
            }
             
            if (child != null && child.name == selectedGear.ToString() && child.parent.name.Contains("Axis") && child != myScript.transform)
            {
                int gearDiamNumber = 0;                                     // Gear Pivot Size 1
                if (newGear == 8 || newGear == 9 || newGear == 10)          // Gear Pivot Size 2
                {    
                    gearDiamNumber = 1;
                    AxisTypeList.GetArrayElementAtIndex(selectedGear).intValue = 2;
                }
                else if (newGear == 11 || newGear == 12 || newGear == 13){  // Gear Pivot Size 3
                    gearDiamNumber = 2;
                    AxisTypeList.GetArrayElementAtIndex(selectedGear).intValue = 3;
                }   
                else if (newGear == 14 || newGear == 15){                   // Gear Pivot Size 4
                    gearDiamNumber = 3;
                    AxisTypeList.GetArrayElementAtIndex(selectedGear).intValue = 4;
                }
                else{
                    AxisTypeList.GetArrayElementAtIndex(selectedGear).intValue = 1;
                }
                    

                GameObject newTile = Instantiate(myScript.defaultTileList[gearDiamNumber], child.parent.gameObject.transform);
                newTile.name = selectedGear.ToString();
                Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                Undo.DestroyObjectImmediate(child.gameObject);

                tmpAxis = newTile;

               
                newTile.transform.GetChild(0).GetChild(0).name = newTile.transform.GetChild(0).GetChild(0).name + "_" + selectedGear;

                pivotGearList.GetArrayElementAtIndex(selectedGear).objectReferenceValue = newTile.transform.GetChild(0).GetChild(0).gameObject;


                GearsUseOrFakeList.GetArrayElementAtIndex(selectedGear).boolValue = false;
                GearsInitPositionWhenStart.GetArrayElementAtIndex(selectedGear).boolValue = true;
                GearsAvailableWhenStart.GetArrayElementAtIndex(selectedGear).boolValue = true;
                AxisAvailableWhenStart.GetArrayElementAtIndex(selectedGear).boolValue = true;
            }
        }
        tmpGear.transform.GetChild(0).transform.position = tmpAxis.transform.position;

    }

    private void GenerateKeys(GearsPuzzle myScript,int howManyGear, bool b_MultipleGears)
    {
        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        //GameObject WhiteSpot = null;
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();
       
        int intListPosition = 0;
        if (pivotGearList.arraySize != 0)
            intListPosition = pivotGearList.arraySize - 1;

        pivotGearList.InsertArrayElementAtIndex(intListPosition);
        GearList.InsertArrayElementAtIndex(intListPosition);

        GearsTypeList.InsertArrayElementAtIndex(intListPosition);
        AxisTypeList.InsertArrayElementAtIndex(intListPosition);

        AxisRotationRight.InsertArrayElementAtIndex(intListPosition);

        GearsUseOrFakeList.InsertArrayElementAtIndex(intListPosition);

        GearsInitPositionWhenStart.InsertArrayElementAtIndex(intListPosition);
        GearsAvailableWhenStart.InsertArrayElementAtIndex(intListPosition);
        AxisAvailableWhenStart.InsertArrayElementAtIndex(intListPosition);

        GearsPositionList.InsertArrayElementAtIndex(intListPosition);
        GearsSolutionList.InsertArrayElementAtIndex(intListPosition);

        string objName = "";
        GameObject tmpGear = null;
        GameObject tmpAxis = null;

        for (var k = 0; k < 2; k++)
        {
            if(k == 0){
                objName = "Gears";
                selectDefaultTile.intValue = 16; // Default Gear
            }
            if (k == 1){
                objName = "Axis";
                selectDefaultTile.intValue = 17; // Default Gear
            }

            

            int number = 0;
            int raw = 0;
            for (var i = 0; i < howManyGear; i++)
            {
                GameObject newTile = Instantiate(myScript.defaultTileList[selectDefaultTile.intValue], myScript.gameObject.transform);

                if (k == 0){
                    tmpGear = newTile;
                    newTile.transform.localPosition = new Vector3(.18f * (GearsTypeList.arraySize - 1), .2f, 0);
                }
                else{
                    tmpAxis = newTile;
                    newTile.transform.localPosition = new Vector3(.18f * (GearsTypeList.arraySize - 1), 0, 0);
                }
                       

                    if (GearsTypeList.arraySize <= 10)
                        newTile.name = objName + "_0" + (GearsTypeList.arraySize - 1);
                    else
                        newTile.name = objName + "_" + (GearsTypeList.arraySize - 1);

                    newTile.transform.GetChild(0).name = (GearsTypeList.arraySize - 1).ToString();

                    Undo.RegisterCreatedObjectUndo(newTile, newTile.name);
                    ts = newTile.GetComponentsInChildren<Transform>();

                    foreach (Transform child in ts)
                    {
                        if (child != null && child.name.Contains("PIVOT_Gear"))
                        {
                            child.name += "_" + (GearsTypeList.arraySize - 1).ToString();
                            pivotGearList.GetArrayElementAtIndex(GearsTypeList.arraySize - 1).objectReferenceValue = child.gameObject;
                        }

                        if (child != null && child.name.Contains("Gears_"))
                        {

                        GearList.GetArrayElementAtIndex(GearsTypeList.arraySize - 1).objectReferenceValue = child.transform.GetChild(0).gameObject;
                        }
                    }

                if (pivotGearList.arraySize != 1)
                    intListPosition = pivotGearList.arraySize-1;
                else
                    intListPosition = 0;
                
                GearsTypeList.GetArrayElementAtIndex(intListPosition).intValue = 15;     // Gig Gear
                AxisTypeList.GetArrayElementAtIndex(intListPosition).intValue = 4;

                if (i % _Column.intValue == _Column.intValue - 1)
                    raw++;
                number++;
            }
        }

        tmpGear.transform.GetChild(0).transform.GetChild(0).transform.position = tmpAxis.transform.position;
    }

    public GameObject tmpDestroyGear;

    private void DeleteGear(GearsPuzzle myScript, int gearNumber)
    {
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);

        pivotGearList.ClearArray();
        pivotGearList.ClearArray();

        GearList.ClearArray();
        GearList.ClearArray();

        GearsTypeList.ClearArray();
        AxisTypeList.ClearArray();
        AxisRotationRight.ClearArray();

        GearsUseOrFakeList.ClearArray();

        GearsInitPositionWhenStart.ClearArray();
        GearsAvailableWhenStart.ClearArray();
        AxisAvailableWhenStart.ClearArray();

        GearsPositionList.ClearArray();
        GearsSolutionList.ClearArray();

        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in ts)
            {
            if (child != null && child.name.Contains("Gears") && child != myScript.transform )
                    Undo.DestroyObjectImmediate(child.gameObject);
            if (child != null && child.name.Contains("Axis") && child != myScript.transform )
                    Undo.DestroyObjectImmediate(child.gameObject);
            }
    }


    private void ResetPosition(GearsPuzzle myScript,SerializedProperty _PositionList, SerializedProperty _DirectionList){
        for (var i = 0; i < _PositionList.arraySize;i++){
            _PositionList.GetArrayElementAtIndex(i).intValue = 0;
        }

        for (var i = 0; i < GearsPositionList.arraySize; i++)
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

    private void loadGearsPosition(GearsPuzzle myScript, int WhichSection)
    {
        for (var i = 0; i < GearsPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("Gears_") && child != myScript.transform && int.Parse(child.GetChild(0).name) == i)
                {
                    // objPIVOT = child;
                    Undo.RegisterFullObjectHierarchyUndo(child.transform.GetChild(0).gameObject, child.transform.GetChild(0).name);
                    if (WhichSection == 0)
                    {
                        child.GetChild(0).transform.GetChild(0).transform.position = myScript.pivotGearList[i].transform.position;
                    }
                    else if (WhichSection == 1)
                    {
                            if(GearsInitPositionWhenStart.GetArrayElementAtIndex(i).boolValue)
                                child.GetChild(0).transform.GetChild(0).transform.localPosition = Vector3.zero;
                            else
                                child.GetChild(0).transform.GetChild(0).transform.position = myScript.pivotGearList[i].transform.position;

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
            sType = "Gear";
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
                    EditorGUILayout.HelpBox("1-Click on a Button below to access his parameters." +
                                            "\n2-Drag and drop a sprite in the slot next to the KEY thumbnail." +
                                            "\n3-Change his scale." +
                                            "\n4-Apply the same scale to all tiles by pressing button ''Apply to All''." + 
                                            "\n5-Choose the text displayed in the scene view inside the KEY." +
                                            "\n6-Choose the value displayed on the result screen in the scene view.", MessageType.Info);
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