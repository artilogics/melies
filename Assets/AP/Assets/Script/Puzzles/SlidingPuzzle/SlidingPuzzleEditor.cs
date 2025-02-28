// Description : Custom Editor for SlidingPuzzle.cs
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(SlidingPuzzle))]
public class SlidingPuzzleEditor : Editor
{
    SerializedProperty SeeInspector;                                            // use to draw default Inspector

    SerializedProperty _Raw;
    SerializedProperty _Column;
    SerializedProperty tilesList;
    SerializedProperty  toolbarCurrentValue;
    SerializedProperty SquareSize;
    SerializedProperty currentSelectedSprite;
    SerializedProperty positionList;
    SerializedProperty randomNumber;
    SerializedProperty helpBoxEditor;
    SerializedProperty validationButtonJoystick;

    SerializedProperty a_TileMove;
    SerializedProperty a_TileMoveVolume;
    SerializedProperty a_Reset;
    SerializedProperty a_ResetVolume;

    SerializedProperty popUpObject;
    SerializedProperty popupSpeed;

    SerializedProperty b_feedbackActivated;
    SerializedProperty feedbackIDList;

    public List<string> s_inputListJoystickButton = new List<string>();

    bool b_Reset = false;
  
    public string[] toolbarStrings = new string[] { "Generate", "Customization", "Init Position","Game Options" };

    public Transform spriteTransform;


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

        SlidingPuzzle myScript = (SlidingPuzzle)target;

        _Raw                        = serializedObject.FindProperty("_Raw");
        _Column                     = serializedObject.FindProperty("_Column");
        toolbarCurrentValue         = serializedObject.FindProperty("toolbarCurrentValue");
        SquareSize                  = serializedObject.FindProperty("SquareSize");

        tilesList                   = serializedObject.FindProperty("tilesList");

        currentSelectedSprite       = serializedObject.FindProperty("currentSelectedSprite");
        positionList                = serializedObject.FindProperty("positionList");
        randomNumber                = serializedObject.FindProperty("randomNumber");


        validationButtonJoystick    = serializedObject.FindProperty("validationButtonJoystick");

        a_TileMove                  = serializedObject.FindProperty("a_TileMove");
        a_TileMoveVolume            = serializedObject.FindProperty("a_TileMoveVolume");
        a_Reset                     = serializedObject.FindProperty("a_Reset");
        a_ResetVolume               = serializedObject.FindProperty("a_ResetVolume");


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

        SlidingPuzzle myScript = (SlidingPuzzle)target;

       
        if(Application.isPlaying){
            EditorGUILayout.HelpBox("Puzzle could not be edited in play mode", MessageType.Info);
        }
        else{
//--> Display Tab sections in the Inspector
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

//--> Update the Inspector view
            if (EditorGUI.EndChangeCheck())
            {
                if ((toolbarCurrentValue.intValue == 1 && b_TilesExist) || (toolbarCurrentValue.intValue == 0 && b_TilesExist))
                    InitSelectSpritesPosition(myScript);
                if (toolbarCurrentValue.intValue == 2 && b_TilesExist)
                    InitMixPosition(myScript);
            }
        


// --> Display Generate Section
            if (toolbarCurrentValue.intValue == 0)
                displayGeneratePuzzleSection(myScript,style_Orange);

// --> Display Other Section
            if (toolbarCurrentValue.intValue == 3)
                otherSection(myScript, style_Orange);
            
            if(tilesList.arraySize > 0){
                if(b_TilesExist){
// --> Display Customization Section
                    if (toolbarCurrentValue.intValue == 1)
                        displaySelectSpriteSection(myScript, style_Blue);
// --> Display Init Position Section
                    if (toolbarCurrentValue.intValue == 2)
                        displayMixSection(myScript, style_Yellow_01);  
                }
//--> Puzzle coun't be displayed
                else{
                    if (toolbarCurrentValue.intValue == 1 || toolbarCurrentValue.intValue == 2)
                        puzzleNeedToBeGenerated();  
                }
            }

        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.LabelField("");
    }


//--> Puzzle coun't be displayed
    private void puzzleNeedToBeGenerated(){
        EditorGUILayout.HelpBox("You need to generate the puzzle first.", MessageType.Error);
    }

//--> Section Other Options
    private void otherSection(SlidingPuzzle myScript, GUIStyle  style_Orange){
        EditorGUILayout.BeginVertical(style_Orange);

            EditorGUILayout.HelpBox("Section : Other Options.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Validation Button Joystick : ", GUILayout.Width(150));
                validationButtonJoystick.intValue = EditorGUILayout.Popup(validationButtonJoystick.intValue, s_inputListJoystickButton.ToArray());
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("");


            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Play Audio when tile move : ", GUILayout.Width(180));
                EditorGUILayout.PropertyField(a_TileMove, new GUIContent(""), GUILayout.Width(100));
                GUILayout.Label("Volume : ", GUILayout.Width(60));
                a_TileMoveVolume.floatValue = EditorGUILayout.Slider(a_TileMoveVolume.floatValue, 0, 1);
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

//--> Section Generate
    private void displayGeneratePuzzleSection(SlidingPuzzle myScript,GUIStyle style_Orange){
        EditorGUILayout.BeginVertical(style_Orange);
            EditorGUILayout.HelpBox("Section : Generate Tiles. (Minimum : Raw * Column >= 4)", MessageType.Info);
            _helpBox(0);

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Column :", GUILayout.Width(85));
                EditorGUILayout.PropertyField(_Column, new GUIContent(""), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Row :", GUILayout.Width(85));
                EditorGUILayout.PropertyField(_Raw, new GUIContent(""), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Tiles"))
            {
                GenerateTile(myScript);
            }
        EditorGUILayout.EndVertical();
    }



//--> Section Init Position
    private void displayMixSection(SlidingPuzzle myScript, GUIStyle style_Yellow_01)
    {
       EditorGUILayout.BeginVertical(style_Yellow_01);
            EditorGUILayout.HelpBox("Section : Mix Tiles.", MessageType.Info);

            _helpBox(2);

//-> Mixing Parameters
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Random Mixing"))
                {RandomMix(myScript);}  

                EditorGUILayout.PropertyField(randomNumber, new GUIContent(""), GUILayout.Width(45));

                if (GUILayout.Button("Update Scene View"))
                {InitMixPosition(myScript);}  

                if (GUILayout.Button("Reset Mixing"))
                {ResetPosition(myScript);}

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");
        EditorGUILayout.EndVertical();
       

        EditorGUILayout.LabelField("");

//-> Display tiles and sprites in the Inspector
        if(!b_Reset){
            int number = 0;
            for (var i = 0; i < _Raw.intValue; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (var j = 0; j < _Column.intValue; j++)
                {
                    if (myScript.positionList[number] != -1)
                    {
                        if (tilesList.arraySize >= myScript.positionList[number])
                        {
                            Transform[] ts = myScript.tilesList[myScript.positionList[number]].GetComponentsInChildren<Transform>();

                            spriteTransform = null;
                            bool b_Sprite = false;
                            for (var k = 0; k < ts.Length; k++)
                            {
                                if (ts[k].name.Contains("Sprite"))
                                {
                                    spriteTransform = ts[k];
                                    b_Sprite = true;
                                }
                            }

                            if (myScript.tilesList[myScript.positionList[number]].transform.GetChild(0).transform.childCount > 0 && b_Sprite)
                            {
                                GameObject objSprite = spriteTransform.gameObject;

                                if (objSprite)
                                {
                                    Texture2D DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;
                                    if (GUILayout.Button(DisplayTexture, GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                                    {
                                        currentSelectedSprite.intValue = number;
                                        MoveTile(myScript, currentSelectedSprite.intValue, true);
                                    }
                                }
                                number++;
                            }
                            else
                            {
                                if (GUILayout.Button("No Sprite", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                                {
                                    currentSelectedSprite.intValue = number;
                                    MoveTile(myScript, currentSelectedSprite.intValue, true);
                                }
                                number++;
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue));
                        number++;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.LabelField("");

    }


//--> Reset Tile Mixing
    private void ResetPosition(SlidingPuzzle myScript){
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        myScript.currentSelectedSprite = -2;
        for (var i = 0; i < myScript.positionList.Count; i++)
        {
            myScript.positionList[i] = i;
        }
        myScript.positionList[myScript.positionList.Count - 1] = -1;
        myScript.currentSelectedSprite = 0;

        int number = 0;
        for (var i = 0; i < _Raw.intValue; i++)
        {
            for (var j = 0; j < _Column.intValue; j++)
            {
                if (i == _Raw.intValue - 1 && j == _Column.intValue - 1){}
                else
                {myScript.tilesList[number].transform.localPosition = new Vector3(.25f * j, -.25f * i, 0);}

                number++;
            }
        }

    }

//--> Update Tile position in scene view
    private void InitMixPosition(SlidingPuzzle myScript)
    {
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        for (var i = 0; i < myScript.positionList.Count; i++)
        {
            if(myScript.positionList[i] != -1){
                int numRaw = i / _Column.intValue;
                int numColumn = i % _Column.intValue;

                myScript.tilesList[myScript.positionList[i]].transform.localPosition = new Vector3(.25f * numColumn, -.25f * numRaw, 0); 
            } 
        }
    }

//--> Init tile position when tab is clicked
    private void InitSelectSpritesPosition(SlidingPuzzle myScript)
    {
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        for (var i = 0; i < myScript.tilesList.Count; i++)
        {
                int numRaw = i / _Column.intValue;
                int numColumn = i % _Column.intValue;

                myScript.tilesList[i].transform.localPosition = new Vector3(.25f * numColumn, -.25f * numRaw, 0);
        }
    }

//--> Random Mixing method
    private void RandomMix(SlidingPuzzle myScript){
        for (var i = 0; i < randomNumber.intValue; i++)
        {
            int value = UnityEngine.Random.Range(0, tilesList.arraySize +1 );
            MoveTile(myScript,value,false);
        }
        InitMixPosition(myScript);
       
    }

//--> Move Tile : Scene view and Inspector
    private string MoveTile(SlidingPuzzle myScript,int selectedTile,bool b_Move){

        int numRaw = selectedTile / _Column.intValue;
        int numColumn = selectedTile % _Column.intValue;

        string result = "Raw : " + numRaw.ToString() + " : Column : " + numColumn.ToString();

    //-> Move if it is Possible
        ///--> Check Up position
        if(numRaw > 0){
            result += " : Up Ok";
            if(positionList.GetArrayElementAtIndex(selectedTile - _Column.intValue).intValue  == -1){
                result += " : Could move Up";  
                positionList.GetArrayElementAtIndex(selectedTile - _Column.intValue).intValue = positionList.GetArrayElementAtIndex(selectedTile).intValue;
                positionList.GetArrayElementAtIndex(selectedTile).intValue = -1; 
                if(b_Move)MoveTileInSceneView(selectedTile, "Up", myScript);
            }
        }
        else{
            result += " : Up No"; }
    //--> Check Down position
        if (numRaw < _Raw.intValue - 1)
        {
            result += " : Down Ok";
            if (positionList.GetArrayElementAtIndex(selectedTile + _Column.intValue).intValue == -1)
            {
                result += " : Could move Down";
                positionList.GetArrayElementAtIndex(selectedTile + _Column.intValue).intValue = positionList.GetArrayElementAtIndex(selectedTile).intValue;
                positionList.GetArrayElementAtIndex(selectedTile).intValue = -1;
                if (b_Move)MoveTileInSceneView(selectedTile, "Down",myScript);
            }
        }
        else{
            result += " : Down No";}
    //--> Check Right position
        if (numColumn < _Column.intValue - 1)
        {
            result += " : Right Ok";
            if (positionList.GetArrayElementAtIndex(selectedTile + 1).intValue == -1)
            {
                result += " : Could move Right";
                positionList.GetArrayElementAtIndex(selectedTile + 1).intValue = positionList.GetArrayElementAtIndex(selectedTile).intValue;
                positionList.GetArrayElementAtIndex(selectedTile).intValue = -1; 
                if (b_Move)MoveTileInSceneView(selectedTile, "Right", myScript);
            }
        }
        else{
            result += " : Right No";}
    //--> Check Left position
        if (numColumn > 0)
        {
            result += " : Left Ok";
            if (positionList.GetArrayElementAtIndex(selectedTile - 1).intValue == -1)
            {
                result += " : Could move Left";
                positionList.GetArrayElementAtIndex(selectedTile - 1).intValue = positionList.GetArrayElementAtIndex(selectedTile).intValue;
                positionList.GetArrayElementAtIndex(selectedTile).intValue = -1; 
                if (b_Move) MoveTileInSceneView(selectedTile, "Left", myScript);
            }
        }
        else{
            result += " : Left No";}
        
        return result;
    }

//--> Move tile in scene view
    private void MoveTileInSceneView(int oldPosition, string direction, SlidingPuzzle myScript){
        if(oldPosition != -1){
           // Debug.Log("oldPosition : " + oldPosition);
            SerializedObject serializedObject2 = new UnityEditor.SerializedObject(myScript.tilesList[myScript.positionList[oldPosition]].GetComponent<Transform>());
            serializedObject2.Update();
            SerializedProperty m_Position = serializedObject2.FindProperty("m_LocalPosition");

            if (direction == "Down")
                m_Position.vector3Value = new Vector3(m_Position.vector3Value.x, m_Position.vector3Value.y - .25f, 0);
            if (direction == "Up")
                m_Position.vector3Value = new Vector3(m_Position.vector3Value.x, m_Position.vector3Value.y + .25f, 0);
            if (direction == "Left")
                m_Position.vector3Value = new Vector3(m_Position.vector3Value.x - .25f, m_Position.vector3Value.y, 0);
            if (direction == "Right")
                m_Position.vector3Value = new Vector3(m_Position.vector3Value.x + .25f, m_Position.vector3Value.y, 0);
            
            serializedObject2.ApplyModifiedProperties();  
        }

    }

    //--> Section : Customization
    private void displaySelectSpriteSection(SlidingPuzzle myScript,GUIStyle style_Blue)
    {
        EditorGUILayout.BeginVertical(style_Blue);
            EditorGUILayout.HelpBox("Section :Select a tile and change his sprite.", MessageType.Info);
            _helpBox(1);

            EditorGUILayout.BeginVertical(style_Blue);

            EditorGUILayout.LabelField("Current selected Tile : " + currentSelectedSprite.intValue,EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();

            if (tilesList.arraySize > currentSelectedSprite.intValue)
            {

                Transform[] ts = myScript.tilesList[currentSelectedSprite.intValue].GetComponentsInChildren<Transform>();

                spriteTransform = null;
                bool b_Sprite = false;
                for (var k = 0; k < ts.Length; k++)
                {
                    if (ts[k].name.Contains("Sprite"))
                    {
                        spriteTransform = ts[k];
                        b_Sprite = true;
                    }
                }

//-> Display current selected tile parameters
                if(myScript.tilesList[currentSelectedSprite.intValue].transform.GetChild(0).transform.childCount > 0 && b_Sprite){
                    GameObject objSprite = spriteTransform.gameObject;

                    if (objSprite)
                    {
                        EditorGUILayout.BeginHorizontal();
                        Texture2D DisplayTexture = null;

                        if(spriteTransform.GetComponent<SpriteRenderer>().sprite)
                            DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;

                        GUILayout.Label(DisplayTexture, GUILayout.Width(20), GUILayout.Height(20));

                        SerializedObject serializedObject3 = new UnityEditor.SerializedObject(spriteTransform.GetComponent<SpriteRenderer>());
                        SerializedProperty m_Sprite = serializedObject3.FindProperty("m_Sprite");
                        serializedObject3.Update();
                        EditorGUILayout.PropertyField(m_Sprite, new GUIContent(""), GUILayout.Width(200));
                        serializedObject3.ApplyModifiedProperties();

                        EditorGUILayout.EndHorizontal();

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


        EditorGUILayout.LabelField("");

//-> Display all tile and sprite in the Inspector
        int number = 0;
        for (var i = 0; i < _Raw.intValue; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < _Column.intValue; j++)
            {
                if (i == _Raw.intValue - 1 && j == _Column.intValue - 1)
                {
                }
                else if(tilesList.arraySize > number)
                {
                    Transform[] ts = myScript.tilesList[number].GetComponentsInChildren<Transform>();

                    spriteTransform = null;
                    bool b_Sprite = false;
                    for (var k = 0; k < ts.Length; k++)
                    {
                        if (ts[k].name.Contains("Sprite"))
                        {
                            spriteTransform = ts[k];
                            b_Sprite = true;
                        }
                    }

                    if (myScript.tilesList[number].transform.GetChild(0).transform.childCount > 0 && b_Sprite)
                    {
                        GameObject objSprite = spriteTransform.gameObject;

                        if (objSprite)
                        {
                           
                            Texture2D DisplayTexture = null;

                            if (spriteTransform.GetComponent<SpriteRenderer>().sprite)
                                DisplayTexture = (Texture2D)spriteTransform.GetComponent<SpriteRenderer>().sprite.texture;


                            if(currentSelectedSprite.intValue != number)
                                GUI.backgroundColor = _cGray;
                            else
                                GUI.backgroundColor = _cBlue;
                          
                            if (GUILayout.Button(DisplayTexture, GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                            {
                                currentSelectedSprite.intValue = number;
                            }

                        }

                        number++;
                    }
                    else
                    {
                        if (currentSelectedSprite.intValue != number)
                            GUI.backgroundColor = _cGray;
                        else
                            GUI.backgroundColor = _cBlue;
                        if (GUILayout.Button("No Sprite", GUILayout.Width(SquareSize.intValue), GUILayout.Height(SquareSize.intValue)))
                        {
                            currentSelectedSprite.intValue = number;
                        }
                        number++;
                    }
                }
              
            }
            EditorGUILayout.EndHorizontal();
        }

    }

//--> Generate Puzzle
    private void GenerateTile(SlidingPuzzle myScript){
        currentSelectedSprite.intValue = 0;
        Undo.RegisterFullObjectHierarchyUndo(myScript, myScript.name);
        Transform[] ts = myScript.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in ts)
        {
            if (child != null && child.name.Contains("Tile"))
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }

        tilesList.ClearArray();
        positionList.ClearArray();

        for (var i = 0; i < _Raw.intValue; i++)
        {
            for (var j = 0; j < _Column.intValue; j++)
            {
                if (i == _Raw.intValue - 1 && j == _Column.intValue - 1)
                {
                }
                else
                {
                    tilesList.InsertArrayElementAtIndex(0);

                }
                positionList.InsertArrayElementAtIndex(0);
            }
        }

        int number = 0;
        for (var i = 0; i < _Raw.intValue;i++){
            for (var j = 0; j < _Column.intValue; j++)
            {
                //Debug.Log("i : " + i + " j : " + j);
                if (i == _Raw.intValue-1 && j == _Column.intValue-1)
                {
                }
                else{
                    GameObject newTile = Instantiate(myScript.defaultTile, myScript.gameObject.transform);

                    newTile.transform.localPosition = new Vector3(.25f * j, -.25f * i, 0);


                    //Debug.Log("number : " + number);
                    if (number < 10)
                        newTile.name = "Tile_0" + number;
                    else
                        newTile.name = "Tile_" + number;


                    newTile.transform.GetChild(0).name = number.ToString();

                    Undo.RegisterCreatedObjectUndo(newTile, newTile.name);

                    tilesList.GetArrayElementAtIndex(number).objectReferenceValue = newTile;
                    positionList.GetArrayElementAtIndex(number).intValue = number;
                    number++;
                }
            }
        }
        positionList.GetArrayElementAtIndex(number).intValue = -1;
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
                    EditorGUILayout.HelpBox("1-Choose the number of column." +
                                            "\n2-Choose the number of row." +
                                            "\n3-Press button Generate to create the puzzle.", MessageType.Info);
                    break;
                case 1:
                    EditorGUILayout.HelpBox("1-Click a tile in the inspector to access its parameters." +
                                            "\n2-Drag and drop a sprite in the slot next to the tile thumbnail." +
                                            "\n3-Change its scale." +
                                            "\n4-Apply the same scale to all tiles by pressing button ''Apply to All''.", MessageType.Info);
                    break;
                case 2:
                    EditorGUILayout.HelpBox("1-Press button 'Random Mixing' to mix the tiles." +
                                            "\n2-Press button 'Update Scene View' to update the scene view visualization." +
                                            "\n3-Press a tile to move it manually." +
                                            "\n\n4-Press button 'Reset Mixing' to initialized the puzzle.", MessageType.Info);
                    break;
                
            }
        }
    }

}
#endif