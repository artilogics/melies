// Description : Custom Editor for DigicodePuzzle.cs
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

[CustomEditor(typeof(DigicodePuzzle))]
public class DigicodePuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector

    SerializedProperty _NumberOfKey;
    SerializedProperty _Column;
    SerializedProperty tilesList;
    SerializedProperty  toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;
    SerializedProperty keyStringList;
    SerializedProperty resultCode;

    SerializedProperty VisualizeSprite;

    SerializedProperty helpBoxEditor;
    SerializedProperty validationButtonJoystick;

    SerializedProperty a_KeyPressed;
    SerializedProperty a_KeyPressedVolume;
    SerializedProperty a_Reset;
    SerializedProperty a_ResetVolume;
    SerializedProperty a_WrongCode;
    SerializedProperty a_WrongCodeVolume;


    SerializedProperty popUpObject;
    SerializedProperty popupSpeed;

    SerializedProperty b_feedbackActivated;
    SerializedProperty feedbackIDList;

    public List<string> s_inputListJoystickButton = new List<string>();

    public Transform spriteTransform;   // Use to display key sprite
    public Text tmpText;        // use to display Key Text
    public Transform objText;

    public string[] toolbarStrings = new string[] { "Generate", "Customization", "Puzzle Solution","Game Options" };



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

    public Color _cBlue = new Color(0f, .9f, 1f, .5f);
    public Color _cRed = new Color(1f, .5f, 0f, .5f);
    public Color _cGray = new Color(.9f, .9f, .9f, 1);
    public Color _cGreen = Color.green;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        SeeInspector                = serializedObject.FindProperty("SeeInspector");
        helpBoxEditor               = serializedObject.FindProperty("helpBoxEditor");

        DigicodePuzzle myScript     = (DigicodePuzzle)target;

        _NumberOfKey                = serializedObject.FindProperty("_NumberOfKey");

        _Column                     = serializedObject.FindProperty("_Column");
        toolbarCurrentValue         = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize                  = serializedObject.FindProperty("SquareSize");

        tilesList                   = serializedObject.FindProperty("tilesList");

        currentSelectedSprite       = serializedObject.FindProperty("currentSelectedSprite");
        keyStringList               = serializedObject.FindProperty("keyStringList");
        VisualizeSprite             = serializedObject.FindProperty("VisualizeSprite");

        validationButtonJoystick    = serializedObject.FindProperty("validationButtonJoystick");

        a_KeyPressed                = serializedObject.FindProperty("a_KeyPressed");
        a_KeyPressedVolume          = serializedObject.FindProperty("a_KeyPressedVolume");
        a_Reset                     = serializedObject.FindProperty("a_Reset");
        a_ResetVolume               = serializedObject.FindProperty("a_ResetVolume");
        a_WrongCode                 = serializedObject.FindProperty("a_WrongCode");
        a_WrongCodeVolume           = serializedObject.FindProperty("a_WrongCodeVolume");

        popUpObject                 = serializedObject.FindProperty("popUpObject");
        popupSpeed                  = serializedObject.FindProperty("popupSpeed");

        feedbackIDList              = serializedObject.FindProperty("feedbackIDList");
        b_feedbackActivated         = serializedObject.FindProperty("b_feedbackActivated");

        resultCode              = serializedObject.FindProperty("resultCode");


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

        DigicodePuzzle myScript = (DigicodePuzzle)target;

       
        if(Application.isPlaying){
            EditorGUILayout.HelpBox("Puzzle could not be edited in play mode", MessageType.Info);
        }
        else{
            // --> Display Tab sections in the Inspector
          
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

            // --> Display GeneratePuzzleSection
            if (toolbarCurrentValue.intValue == 0)
                displayGeneratePuzzleSection(myScript,style_Orange);

            // --> Display Other Section
            if (toolbarCurrentValue.intValue == 3)
                otherSection(myScript, style_Orange);


            if(tilesList.arraySize > 0){
              
                if(b_TilesExist){
                    // --> Display Select Sprites
                    if (toolbarCurrentValue.intValue == 1)
                        displaySelectSpriteSection(myScript, style_Blue);
                    // --> Display Mix Section
                    if (toolbarCurrentValue.intValue == 2)
                        displaySolutionSection(myScript, style_Yellow_01);  
                }
                else{
                    if (toolbarCurrentValue.intValue == 1 || toolbarCurrentValue.intValue == 2)
                        puzzleNeedToBeGenerated();  
                }
            }
        }


        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");
    }

    private void puzzleNeedToBeGenerated(){
        EditorGUILayout.HelpBox("You need to generate the puzzle first.", MessageType.Error);

    }

    private void otherSection(DigicodePuzzle myScript, GUIStyle  style_Orange){
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
        validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        GUILayout.Label("");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when Key is pressed : ", GUILayout.Width(180));
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

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Play Audio when code is wrong : ", GUILayout.Width(180));
        EditorGUILayout.PropertyField(a_WrongCode, new GUIContent(""), GUILayout.Width(100));
        GUILayout.Label("Volume : ", GUILayout.Width(60));
        a_WrongCodeVolume.floatValue = EditorGUILayout.Slider(a_WrongCodeVolume.floatValue, 0, 1);
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

    private void displayGeneratePuzzleSection(DigicodePuzzle myScript,GUIStyle style_Orange){
        
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Section : Generate Keys. (Minimum : 1)", MessageType.Info);
        _helpBox(0);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Column :", GUILayout.Width(100));
        EditorGUILayout.PropertyField(_Column, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("How many Keys :", GUILayout.Width(100));
        EditorGUILayout.PropertyField(_NumberOfKey, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Keys"))
        {
            GenerateKeys(myScript);
        }
        EditorGUILayout.EndVertical();

    }


    private void displaySolutionSection(DigicodePuzzle myScript, GUIStyle style_Yellow_01)
    {
          EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("Section : Puzzle Solution.", MessageType.Info);

        _helpBox(2);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Solution"))
        {
            resultCode.stringValue = "";
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("When you write the solution :" +
                                "\n\nCase 1 : Write directly the solution in the next field." +
                                "\nCase 2 : unselect the next field if needed then use the button below to write the solution.", MessageType.Info);
      
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Puzzle Solution :", GUILayout.Width(100));
        EditorGUILayout.PropertyField(resultCode, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Yellow_01, 1);
       
        EditorGUILayout.LabelField("");
    }



    private void displaySelectSpriteSection(DigicodePuzzle myScript,GUIStyle style_Blue)
    {
         EditorGUILayout.LabelField("");
        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Section :Select a tile and change its sprite.", MessageType.Info);

        _helpBox(1);

        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.LabelField("Current selected Tile : " + currentSelectedSprite.intValue,EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        if (tilesList.arraySize > currentSelectedSprite.intValue)
        {
            Transform[] ts = myScript.tilesList[currentSelectedSprite.intValue].GetComponentsInChildren<Transform>();

          
            bool b_SpriteExist = false;



            for (var i = 0; i < ts.Length;i++){
                if (ts[i].name.Contains("Sprite"))
                {
                    spriteTransform = ts[i];
                    b_SpriteExist = true;
                    break;
                } 
            }

         
            if(spriteTransform != null  && b_SpriteExist){
                GameObject objSprite = spriteTransform.gameObject;

                if (objSprite)
                {
                   
                    EditorGUILayout.BeginHorizontal();
                        //-> Display srite thumbail 
                        Texture2D DisplayTexture = null;

                        if(spriteTransform.GetComponent<SpriteRenderer>().sprite)
                            DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;

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
                        for (var i = 0; i < tilesList.arraySize; i++){
                            Transform[] ts2 = myScript.tilesList[i].GetComponentsInChildren<Transform>();
                            spriteTransform = null;

                            for (var k = 0; k < ts2.Length; k++)
                            {
                                if (ts2[k].name.Contains("Sprite"))
                                {
                                    spriteTransform = ts2[k];
                                }
                            }

                            if(spriteTransform){
                                SerializedObject serializedObject5 = new UnityEditor.SerializedObject(spriteTransform.transform);
                                SerializedProperty m_localScale2 = serializedObject5.FindProperty("m_LocalScale");
                                serializedObject5.Update();
                                m_localScale2.vector3Value = m_localScale.vector3Value;
                                serializedObject5.ApplyModifiedProperties();
                            }
                           
                        }
                    }
                    EditorGUILayout.EndHorizontal();


                    //--> Display Text on Key
                    Transform[] ts3 = myScript.tilesList[currentSelectedSprite.intValue].GetComponentsInChildren<Transform>();

                    for (var i = 0; i < ts3.Length; i++)
                    {
                        if (ts3[i].name.Contains("Text")){
                            tmpText = ts3[i].GetComponent<Text>();}
                    }

                    if(tmpText != null ){
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Text displayed on KEY:", GUILayout.Width(200));

                        SerializedObject serializedObject6 = new UnityEditor.SerializedObject(tmpText);
                        SerializedProperty m_Text = serializedObject6.FindProperty("m_Text");
                        serializedObject6.Update();


                        m_Text.stringValue = EditorGUILayout.TextField(m_Text.stringValue, GUILayout.Width(80));

                        serializedObject6.ApplyModifiedProperties();

                        EditorGUILayout.EndHorizontal();  
                    }
                    else{
                        EditorGUILayout.HelpBox("Text : Not available (Object doesn't exist in the Hierarchy).", MessageType.Info);

                    }


                    //--> Display Text on Result Screen
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Text displayed on Result Screen :", GUILayout.Width(200));
                    EditorGUILayout.PropertyField(keyStringList.GetArrayElementAtIndex(currentSelectedSprite.intValue), new GUIContent(""), GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                } 
            }
            else
            {
                EditorGUILayout.HelpBox("No Sprites are available", MessageType.Info);
            }

        }

        EditorGUILayout.EndVertical();

        displayKeysInTheInspector(myScript, style_Blue,0);
    }

    private void displayKeysInTheInspector(DigicodePuzzle myScript, GUIStyle style_Blue,int WhichSection){
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

                    Transform[] ts3 = myScript.tilesList[number].GetComponentsInChildren<Transform>();

                    for (var k = 0; k < ts3.Length; k++)
                    {
                        if (ts3[k].name.Contains("Text"))
                        {
                            tmpText = ts3[k].GetComponent<Text>();
                        }
                    }

                    if(tmpText)
                        EditorGUILayout.LabelField("Key : " + tmpText.text as String, GUILayout.Width(SquareSize.intValue));
                    else
                        EditorGUILayout.LabelField("Key : No Text" , GUILayout.Width(SquareSize.intValue));


                    EditorGUILayout.LabelField("Result : " + keyStringList.GetArrayElementAtIndex(number).stringValue as String, GUILayout.Width(SquareSize.intValue));
                    Transform[] ts = myScript.tilesList[number].GetComponentsInChildren<Transform>();
                    spriteTransform = null;

                    for (var k = 0; k < ts.Length; k++)
                    {
                        if (ts[k].name.Contains("Sprite"))
                        {
                            spriteTransform = ts[k];
                        }
                        if (ts[k].name.Contains("Text"))
                        {
                            objText  = ts[k];
                        }
                    }

                    if (spriteTransform && VisualizeSprite.boolValue || objText && !VisualizeSprite.boolValue)
                    {
                        GameObject objSprite = spriteTransform.gameObject;

                        if (objSprite && VisualizeSprite.boolValue || objText && !VisualizeSprite.boolValue)
                        {


                            Texture2D DisplayTexture = null;


                            if(spriteTransform.GetComponent<SpriteRenderer>().sprite)
                                DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;

                            if (VisualizeSprite.boolValue){
                                if (currentSelectedSprite.intValue != number)
                                    GUI.backgroundColor = _cGray;
                                else
                                    GUI.backgroundColor = _cBlue;
                                if (GUILayout.Button(DisplayTexture, GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                                {
                                    currentSelectedSprite.intValue = number;
                                    if (WhichSection == 1)
                                    {       //-> Section : Solution 
                                        resultCode.stringValue += keyStringList.GetArrayElementAtIndex(currentSelectedSprite.intValue).stringValue;
                                    }
                                } 
                            }
                            else{
                                if (currentSelectedSprite.intValue != number)
                                    GUI.backgroundColor = _cGray;
                                else
                                    GUI.backgroundColor = _cBlue;
                                if (GUILayout.Button(keyStringList.GetArrayElementAtIndex(number).stringValue, GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                                {
                                    currentSelectedSprite.intValue = number;
                                    if (WhichSection == 1)
                                    {       //-> Section : Solution 
                                        resultCode.stringValue += keyStringList.GetArrayElementAtIndex(currentSelectedSprite.intValue).stringValue;
                                    }
                                } 
                            }
                        }

                        number++;
                    }
                    else 
                    {
                        if (GUILayout.Button("No Sprite", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                        {
                            currentSelectedSprite.intValue = number;
                            if (WhichSection == 1)          //-> Section : Solution 
                            {
                                resultCode.stringValue += keyStringList.GetArrayElementAtIndex(currentSelectedSprite.intValue).stringValue;
                            }
                        }
                        number++;
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
  
    }

    private void GenerateKeys(DigicodePuzzle myScript){
        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in ts)
        {
            if (child != null && child.name.Contains("Key"))
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }


        tilesList.ClearArray();
        keyStringList.ClearArray();

        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            tilesList.InsertArrayElementAtIndex(0);
            keyStringList.InsertArrayElementAtIndex(0);
        }

        int number = 0;
        int raw = 0;
        for (var i = 0; i < _NumberOfKey.intValue; i++)
        {
            GameObject newTile = Instantiate(myScript.defaultTile, myScript.gameObject.transform);

            if(raw != 0)
                newTile.transform.localPosition = new Vector3(.25f * (i % _Column.intValue), -.25f * raw, 0);
            else
                newTile.transform.localPosition = new Vector3(.25f * i, -.25f * raw, 0);   

            //Debug.Log("number : " + number);
            if (number < 10)
                newTile.name = "Key_0" + number;
            else
                newTile.name = "Key_" + number;


            newTile.transform.GetChild(0).name = number.ToString();

            Undo.RegisterCreatedObjectUndo(newTile, newTile.name);

            tilesList.GetArrayElementAtIndex(number).objectReferenceValue = newTile;
            //positionList.GetArrayElementAtIndex(number).intValue = number;

            if (i % _Column.intValue == _Column.intValue -1 )
                raw++;

            number++;
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
                    EditorGUILayout.HelpBox("1-Choose the number of Column." +
                                            "\n2-Choose the number of keys." +
                                            "\n3-Press button 'Generate' to create the puzzle.", MessageType.Info);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("1-Click on a Button below to access its parameters." +
                                            "\n2-Drag and drop a sprite in the slot next to the KEY thumbnail." +
                                            "\n3-Change its scale." +
                                            "\n4-Apply the same scale to all tiles by pressing button ''Apply to All''." + 
                                            "\n5-Choose the text displayed in the scene view inside the KEY." +
                                            "\n6-Choose the value displayed on the result screen in the scene view.", MessageType.Info);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("1-Create the code by pressing the buttons below." +
                                            "\n" +
                                            "\nNote : Reset the solution by pressing button 'Reset Solution'.", MessageType.Info);
                    break;
                
            }
        }
    }

}
#endif