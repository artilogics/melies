// Description : Custom Editor for cylinderPuzzle.cs
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

[CustomEditor(typeof(cylinderPuzzle))]
public class cylinderPuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector

    SerializedProperty puzzleType;
    SerializedProperty puzzleSubType;

    SerializedProperty HowManyCylinderPosition;
    SerializedProperty totalMovement;
    SerializedProperty CylinderSolutionList;
    SerializedProperty linkCylinder;
    SerializedProperty b_SelectCylindersToLink;
    //SerializedProperty lightList;

    SerializedProperty _NumberOfKey;
    SerializedProperty tilesList;
    SerializedProperty toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;
    SerializedProperty CylinderPositionList;
 
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



    public List<string> s_inputListJoystickButton = new List<string>();

    //bool b_Reset = false;

    public Transform spriteTransform;   // Use to display key sprite
    public Text tmpText;        // use to display Key Text
    public Transform objPIVOT;

    public string[] toolbarStrings = new string[] { "Generate","Link (Optional)", "Puzzle Solution", "Init Position", "Game Options" };


    public string[] cylinderTypeStrings             = new string[] { "Choose", "Cylinder", "Circle"};
    public string[] cylinderSubTypeCylinderStrings  = new string[] { "Vertical", "Horizontal" };
    public string[] cylinderSubTypeCircleStrings    = new string[] { "Nested", "Align" };

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

        cylinderPuzzle myScript = (cylinderPuzzle)target;

        puzzleType = serializedObject.FindProperty("puzzleType");
        puzzleSubType = serializedObject.FindProperty("puzzleSubType");


        HowManyCylinderPosition = serializedObject.FindProperty("HowManyCylinderPosition");

        totalMovement = serializedObject.FindProperty("totalMovement");

        CylinderSolutionList = serializedObject.FindProperty("CylinderSolutionList");
        linkCylinder = serializedObject.FindProperty("linkCylinder");
        b_SelectCylindersToLink = serializedObject.FindProperty("b_SelectCylindersToLink");
        //lightList = serializedObject.FindProperty("lightList");

        _NumberOfKey = serializedObject.FindProperty("_NumberOfKey");

        toolbarCurrentValue = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize = serializedObject.FindProperty("SquareSize");

        tilesList = serializedObject.FindProperty("tilesList");

        currentSelectedSprite = serializedObject.FindProperty("currentSelectedSprite");
        CylinderPositionList = serializedObject.FindProperty("CylinderPositionList");
     
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

        GUIStyle style_Yellow_01 = new GUIStyle(); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(); style_Yellow_Strong.normal.background = Tex_02;

        cylinderPuzzle myScript = (cylinderPuzzle)target;


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
                    loadCylinderPosition(myScript, CylinderSolutionList, null);
                   // Debug.Log("Changes 3");
                   
                }
                   
                if (toolbarCurrentValue.intValue == 3 ){
                    loadCylinderPosition(myScript, CylinderPositionList, null);
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

    private void otherSection(cylinderPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
        validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");



        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when Cylinder is pressed : ", GUILayout.Width(180));
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

        if (GUILayout.Button("Activate / Deactivate Ref Value"))
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in ts)
            {
                if (child != null && child.name == "Number" && child != myScript.transform)
                {
                    Undo.RegisterFullObjectHierarchyUndo(child.gameObject, child.gameObject.name);

                    if(child.gameObject.activeInHierarchy)
                        child.gameObject.SetActive(false);
                    else
                        child.gameObject.SetActive(true); 
                }
            }
 
        }

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

    private void displayGeneratePuzzleSection(cylinderPuzzle myScript, GUIStyle style_Orange)
    {
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Generate Cylinders/Circles. (Minimum 1 Cylinder/Circle / 2 Positions)", MessageType.Info);
        _helpBox(0);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Puzzle Type :", GUILayout.Width(150));
        puzzleType.intValue = EditorGUILayout.Popup(puzzleType.intValue, cylinderTypeStrings);
        EditorGUILayout.EndHorizontal();
       
        string s_subType = "";
        if(puzzleType.intValue != 0){
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sub Type :", GUILayout.Width(150));
            if (puzzleType.intValue == 1){
                puzzleSubType.intValue = EditorGUILayout.Popup(puzzleSubType.intValue, cylinderSubTypeCylinderStrings);
                s_subType = "Cylinder";
            }
          
            if (puzzleType.intValue == 2){
                puzzleSubType.intValue = EditorGUILayout.Popup(puzzleSubType.intValue, cylinderSubTypeCircleStrings);
                s_subType = "Circle";
            }
               
            EditorGUILayout.EndHorizontal(); 

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("How many " + s_subType + " :", GUILayout.Width(150));
            EditorGUILayout.PropertyField(_NumberOfKey, new GUIContent(""), GUILayout.Width(30));

            if (puzzleType.intValue == 1)
            {
                EditorGUILayout.LabelField("(1 minimum)");
                if (_NumberOfKey.intValue < 1)
                    _NumberOfKey.intValue = 1;
            }

            if (puzzleType.intValue == 2){
                if(puzzleSubType.intValue == 0){
                    EditorGUILayout.LabelField("(between 1 and 5)");
                    if (_NumberOfKey.intValue < 1)
                        _NumberOfKey.intValue = 1;

                    if (_NumberOfKey.intValue > 5)
                        _NumberOfKey.intValue = 5;
                }
                if (puzzleSubType.intValue == 1)
                {
                    EditorGUILayout.LabelField("(1 minimum)");
                    if (_NumberOfKey.intValue < 1)
                        _NumberOfKey.intValue = 1;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("How Many " + s_subType + " Position :", GUILayout.Width(150));


            if (puzzleType.intValue == 1)
            {
                if (HowManyCylinderPosition.intValue != 5 && HowManyCylinderPosition.intValue != 10)
                    HowManyCylinderPosition.intValue = 5;

                if(HowManyCylinderPosition.intValue == 5){
                    if (GUILayout.Button("5 positions", GUILayout.Width(100)))
                    {
                        HowManyCylinderPosition.intValue = 10;
                    }
                }
                    

                if (HowManyCylinderPosition.intValue == 10){
                    if (GUILayout.Button("10 positions", GUILayout.Width(100)))
                    {
                        HowManyCylinderPosition.intValue = 5;
                    } 
                }
            }

            if (puzzleType.intValue == 2)
            {
                EditorGUILayout.PropertyField(HowManyCylinderPosition, new GUIContent(""), GUILayout.Width(30));
                EditorGUILayout.LabelField("(2 minimum)");
                if (HowManyCylinderPosition.intValue < 2)
                    HowManyCylinderPosition.intValue = 2;
            }

            EditorGUILayout.EndHorizontal();
        }

       
        if (GUILayout.Button("Generate " + s_subType))
        {
            GenerateKeys(myScript);
        }
        EditorGUILayout.EndVertical();

    }

    private void LinkSection(cylinderPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("Section : Link Cylinders/Circles.", MessageType.Info);

        _helpBox(2);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Choose Parent Cylinder/Circles :", GUILayout.Width(180));
        if(!b_SelectCylindersToLink.boolValue)
            GUI.backgroundColor = _cRed;
        else
            GUI.backgroundColor = _cGray;
        if (GUILayout.Button("Activate Mode", GUILayout.Width(120)))
        {
            b_SelectCylindersToLink.boolValue = false;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Choose Linked Cylinder/Circles :", GUILayout.Width(180));

        if (b_SelectCylindersToLink.boolValue)
            GUI.backgroundColor = _cBlue;
        else
            GUI.backgroundColor = _cGray;
        if (GUILayout.Button("Activate Mode", GUILayout.Width(120)))
        {
            b_SelectCylindersToLink.boolValue = true;
        }
        EditorGUILayout.EndHorizontal();

      
        EditorGUILayout.EndVertical();


        displayCylinderInTheInspectorSectionLink(myScript, style_Yellow_01, 0);

        EditorGUILayout.LabelField("");


    }

    private void displaySolutionSection(cylinderPuzzle myScript, GUIStyle style_Yellow_01)
    {
        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("Section : Solution. " +
                                "\n\nCreate the solution by pressing the squares below.(Show modification in scene view).", MessageType.Info);


        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Solution"))
        {
            ResetPosition(myScript, CylinderSolutionList, null);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Yellow_01, 1);

        EditorGUILayout.LabelField("");


    }

    private void displaySelectSpriteSection(cylinderPuzzle myScript, GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section : Cylinders/Circles Initial Position. " +
                                "\n\nCreate the Cylinders/Circles initial position by pressing the squares below.(Show modification in scene view).", MessageType.Info);


        if (GUILayout.Button("Reset Position"))
        {
            ResetPosition(myScript,CylinderPositionList,null);
        }

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Blue, 0);
    }


    //--> Display square that represent puzzle object
    public void displayKeysInTheInspector(cylinderPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");

        int number = 0;

            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < _NumberOfKey.intValue; j++)
            {

                tmpText = null;
                if (tilesList.arraySize > number)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width((SquareSize.intValue/2) - 10));
                EditorGUILayout.LabelField(j.ToString(),GUILayout.Width(SquareSize.intValue/2));
                EditorGUILayout.EndHorizontal();
                    if (GUILayout.Button("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                    {
                        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform child in ts)
                        {
                            if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.parent.name) == number)
                            {
                                objPIVOT = child;
                            }
                        }

                        currentSelectedSprite.intValue = number;

                        float step = totalMovement.intValue / (HowManyCylinderPosition.intValue - 1);


                       if (WhichSection == 0)          //-> Section : Cylinder Init Position
                            {
                                Undo.RegisterFullObjectHierarchyUndo(objPIVOT, objPIVOT.name);

                                SerializedProperty position = CylinderPositionList.GetArrayElementAtIndex(currentSelectedSprite.intValue);

                                moveCylinder(myScript, position, null, step, false);        // Move Parent Cylinder
                                ap_MoveLinkedCylinder(myScript, step, 3);                          // Move Linked Cylinders
                            }

                            if (WhichSection == 1)          //-> Section : Solution 
                            {
                                Undo.RegisterFullObjectHierarchyUndo(objPIVOT, objPIVOT.name);
                                SerializedProperty position = CylinderSolutionList.GetArrayElementAtIndex(currentSelectedSprite.intValue);

                                moveCylinder(myScript, position, null, step, false);        // Move Parent Cylinder
                                ap_MoveLinkedCylinder(myScript, step, 2);                          // Move Linked Cylinders
                       }
                    }

                EditorGUILayout.EndVertical();

                    number++;

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndHorizontal();

    }

    private void moveCylinder(cylinderPuzzle myScript, SerializedProperty position, SerializedProperty directionUp, float step,bool b_OnlyMoveCylinder)
    {
        if (!b_OnlyMoveCylinder) position.intValue++;
            
        position.intValue = position.intValue % HowManyCylinderPosition.intValue;

        float newAngle = (360 / HowManyCylinderPosition.intValue) * position.intValue;

        if(puzzleType.intValue == 1)    // Cylinder
            objPIVOT.localEulerAngles = new Vector3(-newAngle, 0, 0);
        if (puzzleType.intValue == 2)   // Circle
            objPIVOT.localEulerAngles = new Vector3(90, 0, newAngle);

    }

    private void GenerateKeys(cylinderPuzzle myScript)
    {
        GameObject WhiteSpot = null;

        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in ts)
        {
            if (child != null && child.name.Contains("Cylinder") && child != myScript.transform)
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }


        tilesList.ClearArray();
        CylinderPositionList.ClearArray();
        CylinderSolutionList.ClearArray();
        linkCylinder.ClearArray();
        //lightList.ClearArray();

        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            tilesList.InsertArrayElementAtIndex(0);
            CylinderPositionList.InsertArrayElementAtIndex(0);
            CylinderSolutionList.InsertArrayElementAtIndex(0);
            linkCylinder.InsertArrayElementAtIndex(0);
            //lightList.InsertArrayElementAtIndex(0);
        }

        int number = 0;

        // 0 : Circle 0 | 1 : Circle 2 | 2 : Circle 3 | 4 : Circle 5 
        // 5 : Cylindre5xHorizontal | 6 : Cylindre5xVertical | 7 : Cylindre10xHorizontal | 8 : Cylindre10xVertical |

        if(puzzleType.intValue == 1){   // Cylinder
            if (puzzleSubType.intValue == 0 && HowManyCylinderPosition.intValue == 5)
                selectDefaultTile.intValue = 6;     // Cylinder 5x Vertical
            if (puzzleSubType.intValue == 0 && HowManyCylinderPosition.intValue == 10)
                selectDefaultTile.intValue = 8;     // Cylinder 10x Vertical

            if (puzzleSubType.intValue == 1 && HowManyCylinderPosition.intValue == 5)
                selectDefaultTile.intValue = 5;     // Cylinder 5x Horizontal
            if (puzzleSubType.intValue == 1 && HowManyCylinderPosition.intValue == 10)
                selectDefaultTile.intValue = 7;     // Cylinder 10x Vertical
        }
        if (puzzleType.intValue == 2)   // Circle
        {
            selectDefaultTile.intValue = 4;
        }

        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            GameObject newTile;
            if (puzzleType.intValue == 2 && puzzleSubType.intValue == 0){      // nested circle
                newTile = Instantiate(myScript.defaultTileList[i], myScript.gameObject.transform); 
            }
            else{                               // other circle and cylinder case
                newTile = Instantiate(myScript.defaultTileList[selectDefaultTile.intValue], myScript.gameObject.transform);

                // Position for each cylinder or Circle
                if (puzzleType.intValue == 1)
                {   // Cylinder
                    if (puzzleSubType.intValue == 0)   // vertical
                        newTile.transform.localPosition = new Vector3(.05f * i, -.25f, 0);
                    if (puzzleSubType.intValue == 1)   // Horizontal
                        newTile.transform.localPosition = new Vector3(.19f, -.05f * i, 0);
                } 
                if (puzzleType.intValue == 2)
                {   // Circle
                    newTile.transform.localPosition = new Vector3(.20f * i, 0, 0);
                } 
            }


            if (number < 10)
                newTile.name = "Cylinder_0" + number;
            else
                newTile.name = "Cylinder_" + number;


            newTile.transform.GetChild(0).name = number.ToString();

            Undo.RegisterCreatedObjectUndo(newTile, newTile.name);


            ts = newTile.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT_Cylinder"))
                {
                    child.name += "_" + number.ToString();
                    tilesList.GetArrayElementAtIndex(number).objectReferenceValue = child.gameObject;
                   
                }

                if (child != null && child.name.Contains("Cylinder_Light_Feedback"))
                {
                    child.name += "_" + number.ToString();
                    //lightList.GetArrayElementAtIndex(number).objectReferenceValue = child.GetComponent<MeshRenderer>();
                   
                }

                if (child != null && child.name.Contains("Text_Number"))
                {
                    child.gameObject.GetComponent<Text>().text = number.ToString();
                }


                if (child != null && child.name.Contains("Spot_White"))
                {
                    WhiteSpot = child.gameObject;
                }

            }


            number++;

            //-> Create sprites Position
            if(WhiteSpot != null){
                for (var j = 0; j < HowManyCylinderPosition.intValue - 1; j++)
                {
                    GameObject newWhiteSpot;
                    newWhiteSpot = Instantiate(WhiteSpot, WhiteSpot.transform.parent.transform);

                    Undo.RegisterCreatedObjectUndo(WhiteSpot, WhiteSpot.name);

                    newWhiteSpot.name = "Spot_White_" + (j + 1).ToString();


                    newWhiteSpot.transform.localEulerAngles = new Vector3(0, 0, (360 / HowManyCylinderPosition.intValue) * (j + 1));
                }

                Undo.DestroyObjectImmediate(WhiteSpot);
            }
        }

    }

    private void ResetPosition(cylinderPuzzle myScript,SerializedProperty _PositionList, SerializedProperty _DirectionList){

        for (var i = 0; i < _PositionList.arraySize;i++){
            _PositionList.GetArrayElementAtIndex(i).intValue = 0;
        }


        for (var i = 0; i < CylinderPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.parent.name) == i)
                {
                    objPIVOT = child;
                }
            }

            moveCylinder(myScript, _PositionList.GetArrayElementAtIndex(i), null, 0,true);
        }

    }

    private void loadCylinderPosition(cylinderPuzzle myScript, SerializedProperty _PositionList, SerializedProperty _DirectionList)
    {
        for (var i = 0; i < CylinderPositionList.arraySize; i++)
        {
            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") && child != myScript.transform && int.Parse(child.parent.parent.name) == i)
                {
                    objPIVOT = child;
                }
            }

            float newAngle = (360 / HowManyCylinderPosition.intValue) * _PositionList.GetArrayElementAtIndex(i).intValue;

            if(puzzleType.intValue == 1)    // Cylinder
                objPIVOT.localEulerAngles = new Vector3(-newAngle, 0, 0);
            if (puzzleType.intValue == 2)   // Circle
                objPIVOT.localEulerAngles = new Vector3(90, 0, newAngle);


        }
    }

    public void displayCylinderInTheInspectorSectionLink(cylinderPuzzle myScript, GUIStyle style_Blue, int WhichSection)
    {
        EditorGUILayout.LabelField("");

        int number = 0;

       // int raw = Mathf.RoundToInt(_NumberOfKey.intValue / _Column.intValue);

        //Debug.Log("raw : " + raw +  " : _Column :" + _Column.intValue + " : _NumberOfKey :" + _NumberOfKey.intValue);
       // for (var i = 0; i <= raw; i++)
       // {
            EditorGUILayout.BeginHorizontal();

            for (var j = 0; j < _NumberOfKey.intValue; j++)
            {

                tmpText = null;
                if (tilesList.arraySize > number)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(SquareSize.intValue));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width((SquareSize.intValue / 2) - 10));
                EditorGUILayout.LabelField(j.ToString(), GUILayout.Width(SquareSize.intValue / 2));
                EditorGUILayout.EndHorizontal();


                    if(ap_checkIfCylinderIsLinked(myScript, number))
                        GUI.backgroundColor = _cBlue;
                    else if (currentSelectedSprite.intValue == number)
                        GUI.backgroundColor = _cRed;
                    else
                        GUI.backgroundColor = _cGray;
                    if (GUILayout.Button("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                    {
                   
                        if (b_SelectCylindersToLink.boolValue){            // Select linked Cylinder
                            ap_linkCylinder(myScript,number);
                        }
                        else{                                           // Select Master Cylinder
                            currentSelectedSprite.intValue = number;
                        }
                    }
                    number++;

                    EditorGUILayout.EndVertical();
                }

            EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        //}
    }

    private bool ap_checkIfCylinderIsLinked(cylinderPuzzle myScript, int number)
    {
        bool result = false;
        for (var i = 0; i < myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList.Count;i++){
            if(myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList[i] == number){
                result = true;
                break;
             }
        }

        return result;
    }

    private void ap_linkCylinder(cylinderPuzzle myScript,int number){
        if (currentSelectedSprite.intValue != number)
        {
            Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
            //Debug.Log("Add Linked Cylinder : " + number);

            bool result = false;
            int listPosition = 0;
            for (var i = 0; i < myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList.Count; i++)
            {
                if (myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList[i] == number)
                {
                    result = true;
                    listPosition = i;
                    break;
                }
            }

            if (result)
                myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList.RemoveAt(listPosition);
            else
                myScript.linkCylinder[currentSelectedSprite.intValue]._CylinderList.Add(number); 
        }
       
    }



    private void ap_MoveLinkedCylinder(cylinderPuzzle myScript,float step,int WhichSection)
    {
        SerializedProperty CylinderList = linkCylinder.GetArrayElementAtIndex(currentSelectedSprite.intValue).FindPropertyRelative("_CylinderList");



        for (var i = 0; i < CylinderList.arraySize; i++){
           // Debug.Log("CylinderList : " + CylinderList.GetArrayElementAtIndex(i).intValue);

            Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

            foreach (Transform child in ts)
            {
                if (child != null && child.name.Contains("PIVOT") 
                    && child != myScript.transform 
                    && int.Parse(child.parent.parent.name) == CylinderList.GetArrayElementAtIndex(i).intValue)
                {
                    objPIVOT = child;
                }
            }

            if(WhichSection == 2){      // Section : Solution 
                moveCylinder(myScript,
                     CylinderSolutionList.GetArrayElementAtIndex(CylinderList.GetArrayElementAtIndex(i).intValue),
                     null,
                     step,
                     false);
            }
            if (WhichSection == 3)      // Section :Cylinder Init Position
            {
                moveCylinder(myScript,
                     CylinderPositionList.GetArrayElementAtIndex(CylinderList.GetArrayElementAtIndex(i).intValue),
                     null,
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
        string sType = "";
        if (puzzleType.intValue == 1)
            sType = "Cylinder";
        else
            sType = "Circle";

        if (helpBoxEditor.boolValue)
        {
            switch (value)
            {
                case 0:
                    EditorGUILayout.HelpBox("1-Choose the number of " + sType +"s." +
                                            "\n2-Choose the subtype" +
                                            "\n3-Choose the number of " + sType +
                                            "\n4- Choose the number of position" +
                                            "\n5-Press button 'Generate " + sType + "' to create the puzzle.", MessageType.Info);
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