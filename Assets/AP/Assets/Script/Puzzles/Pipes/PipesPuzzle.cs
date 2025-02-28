// Description : PipesPuzzle : Manage the Pipe puzzle behaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipesPuzzle : MonoBehaviour {
    public bool                         SeeInspector = false;
    public bool                         helpBoxEditor = true;

    public List<Texture2D>              pipeSprite = new List<Texture2D>();         // use for custom editor : Default Sprite

    public int                          pipeType = 0;                               // use for custom editor : Pipes type : No Move, Horizontal, Vertical, T, Elbow 
    public int                          puzzleSubType = 0;                          // use for custom editor : Horizontal/ Vertical or nested/Align
    public int                          HowManyPipesPosition = 2;                   // use for custom editor.
    public bool                         b_SelectPipessToLink = false;               // use for custom editor.
    public int                          selectDefaultTile = 0;
    public List<GameObject>             defaultTileList = new List<GameObject>();
    public GameObject                   defaultTile;
    public int                          _Column = 3;
    public int                          _NumberOfKey = 10;
    public int                          toolbarCurrentValue = 0;
    public int                          SquareSize = 80;
    public int                          currentSelectedSprite = 0;
    public List<GameObject>             tilesList = new List<GameObject>();        // use for custom editor.


    public List<int>                    PipesTypeList = new List<int>();            // Know the type of each pipe (Cross,elbow,line ...)       
    public List<int>                    PipesPositionList = new List<int>();        // Use for save and Load : Know initial pipe rotation : 0 : no rotation / 1 : 90 degres / 2 : 180 degres / 2 : 270 degres 
    public List<int>                    PipesSolutionList = new List<int>();        // Use for save and Load :Know solution pipe rotation

    public int                          startTile = 0;                              // Know the Start pipe position
    public int                          endTile = 0;                                // Know the end pipe position

    public int                          mazeStartTileX = 0;                         // use to find if the puzzle is solved. X position in maze
    public int                          mazeStartTileY = 0;                         // use to find if the puzzle is solved. Y position in maze

    public int                          mazeEndTileX = 0;                           // use to find if the puzzle is solved. X position in maze
    public int                          mazeEndTileY = 0;                           // use to find if the puzzle is solved. Y position in maze

    public List<int>                    inGamePipesPositionList = new List<int>();  // Know in-game the current pipes rotation

    [System.Serializable]
    public class LinkPipes
    {
        public List<int> _PipesList;
    }

    [SerializeField]
    public List<LinkPipes>              linkPipes;                                  // list of linked pipe

    public List<int>                    positionList = new List<int>();

    public bool                         b_PuzzleSolved = false;         // Know if the puzzle is solved
    public LayerMask                    myLayer;                        // Raycast is done only on layer 15 : Puzzle
    public bool                         b_UsePuzzleFocus = true;        // Use the puzzle focus. If false : focus step is bypass
    public focusCamEffect               camManager;                     // access focusCamEffect component
    public conditionsToAccessThePuzzle _conditionsToAccessThePuzzle;    // access conditionsToAccessThePuzzle component
    public actionsWhenPuzzleIsSolved   _actionsWhenPuzzleIsSolved;      // access actionsWhenPuzzleIsSolved component

    private detectPuzzleClick           _detectClick;                   // Access component to manage click in puzzle (mobile, desktop)

    public int                          validationButtonJoystick = 4;   // Joystick button to validate action in the puzzle

    public AudioClip                    a_KeyPressed;                   // Sound when pipe is pressed
    public float                        a_KeyPressedVolume = 1;         
    public AudioClip                    a_Reset;                        // Sound when Reset button is pressed
    public float                        a_ResetVolume = 1;

    private AudioSource                 a_Source;                       // Access audioSource

    public GameObject                   iconPosition;

    public bool b_popUpDone = false;
    public GameObject popUpObject;
    public float popupSpeed = 3;


    [System.Serializable]
    public class idList
    {
        public int ID = 0;          // entry ID in the window tab
        public int uniqueID = 0;    // entry Unique ID
    }

    public bool b_UIFeedback = true;
    public bool b_feedbackActivated = false;                               // Lock Section : if true text is displayed
    public List<idList> feedbackIDList = new List<idList>() { new idList() };      // Lock Section : Check if an Object is in the player inventory using his ID


    // Use this for initialization
    void Start()
    {
        //--> Every Puzzle  ----> BEGIN <----
        camManager = GetComponent<focusCamEffect>();                                   // Access focusCamEffect to zoom and dezoom on puzzle
        _conditionsToAccessThePuzzle = GetComponent<conditionsToAccessThePuzzle>();    // Access the condition to unlock the puzzle
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();        // Access the actions done when the puzzle is solved
        //----> END <----


        //--> Common for all puzzle ----> BEGIN <----
        _detectClick = new detectPuzzleClick();                 // Access Class that allow to detect click (Mouse, Joystick, Mobile) 

        a_Source = GetComponent<AudioSource>();                 
        //----> END <----
    }


    // Update is called once per frame
    void Update()
    {
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (_conditionsToAccessThePuzzle.b_PuzzleIsActivated && !ingameGlobalManager.instance.b_Ingame_Pause)
            {
                if (iconPosition.activeInHierarchy)
                {
                    iconPosition.SetActive(false);
                    if (popUpObject && !b_popUpDone)
                    {
                        StartCoroutine(popUp(popUpObject, popUpObject.transform.localScale));

                        if (popUpObject.GetComponent<Renderer>())
                            popUpObject.GetComponent<Renderer>().enabled = true;

                        Transform[] allChildren = popUpObject.GetComponentsInChildren<Transform>(true);

                        foreach (Transform child in allChildren)
                        {
                            if (child.GetComponent<Renderer>())
                                child.GetComponent<Renderer>().enabled = true;
                        }

                        popUpObject.SetActive(true);
                    }

                    if (b_UIFeedback)
                    {
                        b_UIFeedback = false;
                        displayUIInfo();
                    }

                }

                PuzzleBehaviour();
            }
            if(!iconPosition.activeInHierarchy && ingameGlobalManager.instance.currentPuzzle == null){
                iconPosition.SetActive(true); 
            }
        }
    }

    public IEnumerator popUp(GameObject obj, Vector3 value)
    {
        b_popUpDone = true;
        //float timer = 0;
        obj.transform.localScale = Vector3.zero;

        while (obj.transform.localScale != value)
        {
            obj.transform.localScale = Vector3.MoveTowards(obj.transform.localScale, value, Time.deltaTime * popupSpeed);
            yield return null;
        }
        yield return null;
    }

    public void displayUIInfo()
    {
        //-> Display feedback info
        ingameGlobalManager gManager = ingameGlobalManager.instance;
        if (b_feedbackActivated && gManager.canvasPlayerInfos._infoUI)
        {
            bool b_Exist = false;
            for (var i = 0; i < gManager.canvasPlayerInfos._infoUI.listRefGameObject.Count; i++)
            {
                if (gameObject == gManager.canvasPlayerInfos._infoUI.listRefGameObject[i])
                    b_Exist = true;
            }
            if (!b_Exist)
                gManager.canvasPlayerInfos._infoUI.playAnimInfo(gManager.currentFeedback.diaryList[gManager.currentLanguage]._languageSlot[feedbackIDList[0].ID].diaryTitle[0], "Feedback", gameObject);
        }
    }

//------> BEGIN <------ Next 6 methods are always needed in a puzzle script 

 
//--> Reset Puzzle when button iconResetPuzzle in Canvas_PlayerInfos is pressed
    public void F_ResetPuzzle(){
        if (a_Source && a_Reset)
        {
            a_Source.clip = a_Reset;
            a_Source.volume = a_ResetVolume;
            a_Source.Play();
        }
        initCurrentPosition("Reset");                             // init Pipes position

    }
   

//--> Actions when puzzle is solved
    private void puzzleSolved(){
        //-> Actions done for all type of puzzle
        if(!b_PuzzleSolved || ingameGlobalManager.instance._P)
            _actionsWhenPuzzleIsSolved.F_PuzzleSolved();                   // Call script actionsWhenPuzzleIsSolved. Do actions when the puzzle is solved the first time.
        else
            _actionsWhenPuzzleIsSolved.b_actionsWhenPuzzleIsSolved = true; // Use when focus is called. The variable b_actionsWhenPuzzleIsSolved in script puzzleSolved equal True

        b_PuzzleSolved = true;                                  
    }

//--> Use to load object state. Initialize the puzzle  (T = True or F = False)
    public void saveSystemInitGameObject(string s_ObjectDatas)
    {
        string[] codes = s_ObjectDatas.Split('_');              // Split data in an array.
        int number = 0;

        //--> Actions to do for this puzzle ----> BEGIN <----
        if (s_ObjectDatas == ""){                               // Save Doesn't exist
            initCurrentPosition("SaveDoesntExist");
        }
        else{                                                   // Save exist
            if (inGamePipesPositionList.Count == 0)             // Load saved value in inGamePipesPositionList
            {
                for (var i = 0; i < PipesPositionList.Count; i++){       
                    if (i == startTile || i == endTile)
                        inGamePipesPositionList.Add(PipesSolutionList[i]);
                    else
                        inGamePipesPositionList.Add(PipesPositionList[i]);

                    number++;
                }
            }

            for (var i = 0; i < inGamePipesPositionList.Count; i++)
            {
                inGamePipesPositionList[i] = int.Parse(codes[i+1]);
            }

            //Debug.Log("Number : " + number);
            number++;
            if (codes[number] == "T")
                b_popUpDone = true;
            else
                b_popUpDone = false; 

            number++;
            //Debug.Log("Number : " + number);

            if (codes[number] == "T")
                b_UIFeedback = true;
            else
                b_UIFeedback = false; 

            initCurrentPosition("SaveExist");
        }
        //----> END <----



        //--> Actions to do for all puzzle ----> BEGIN <----
        if (codes[0] == "T")
        {                                                       // Element 0 : Check if the puzzle is solved                           
            b_PuzzleSolved = true;
            puzzleSolved();
        }

        if(!b_PuzzleSolved)
            _actionsWhenPuzzleIsSolved.initSolvedSection();    // Init Popup object in script actionsWhenPuzzleIsSolved.cs
        //----> END <----

        LoadingProcessDone();                                   // Return that the object is loaded correctly
    }

//--> Use to save Object state
    public string ReturnSaveData()
    {
        //-> Common for all puzzle ----> BEGIN <----
        string value = "";

        value += r_TrueFalse(b_PuzzleSolved) + "_";     // b_PuzzleSolved : Save if the puzzle is solved or not
        //----> END <----


        //-> Specific for this puzzle ----> BEGIN <----
        for (var i = 0; i < inGamePipesPositionList.Count;i++){
            value += inGamePipesPositionList[i] + "_";          // Save the current Pipes or Cercle position
        }

        value += r_TrueFalse(b_popUpDone) + "_"; 

        value += r_TrueFalse(b_UIFeedback) + "_"; 

        //----> END <---- 


        return value;
    }

//-> Send info to the levelManager (Object is correctly loaded)
    private void LoadingProcessDone()
    {
        for (var i = 0; i < ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem.Count; i++)
        {
            if (ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem[i] == gameObject)
            {
                ingameGlobalManager.instance._levelManager.listState[i] = true;
                break;
            }
        }
    }

//--> Convert bool to T or F string
    private string r_TrueFalse(bool s_Ref)
    {
        if (s_Ref) return "T";
        else return "F";
    }


//------> END <------


//--> The Puzzle Behaviour
    private void PuzzleBehaviour()
    {
        Vector3 cursorPosition = Input.mousePosition;

        if (!b_PuzzleSolved)
        {
            Transform objClicked = _detectClick.F_detectPuzzleClick(myLayer, ingameGlobalManager.instance,validationButtonJoystick);

            if (objClicked != null && objClicked.transform.name == "btnPuzzleReset") // Player press button reset 
            {                                                    
                F_ResetPuzzle(); 
            }
            else if(objClicked != null){                                                     // Player press validation button on a puzzle object

                if (a_Source && a_KeyPressed)
                {
                    a_Source.clip = a_KeyPressed;
                    a_Source.volume = a_KeyPressedVolume;
                    a_Source.Play();
                }

                movePipes(objClicked.transform, int.Parse(objClicked.transform.parent.name));                       // Rotate the pipe

                for (var i = 0; i < linkPipes[int.Parse(objClicked.transform.parent.name)]._PipesList.Count;i++){   // Rotate linked pipe if needed
                    movePipes(tilesList[linkPipes[int.Parse(objClicked.transform.parent.name)]._PipesList[i]].transform,
                              linkPipes[int.Parse(objClicked.transform.parent.name)]._PipesList[i]); 
                }

                CheckIfPuzzleSolved();
            }
        }
    }

//--> Init the puzzle positions
    private void initCurrentPosition(string s_SaveState){
        if(s_SaveState == "Reset"){
            inGamePipesPositionList.Clear();

            for (var k = 0; k < PipesPositionList.Count; k++)
            {
                 if(k == startTile || k == endTile){
                    inGamePipesPositionList.Add(PipesSolutionList[k]);
                }
                else
                    inGamePipesPositionList.Add(PipesPositionList[k]);
            }
        }

        if(inGamePipesPositionList.Count == 0){
            for (var i = 0; i < PipesPositionList.Count; i++)
            {
                if(i == startTile || i == endTile){
                    inGamePipesPositionList.Add(PipesSolutionList[i]);
                }
                    
                else
                    inGamePipesPositionList.Add(PipesPositionList[i]);
            }  
        }

        for (var i = 0; i < PipesPositionList.Count; i++)
        {
            float step = 90;
            float newAngle = 0f;

            newAngle = step * inGamePipesPositionList[i];

            tilesList[i].transform.localEulerAngles = new Vector3(0, 0, -newAngle);
        }
    }

    //--> Move (rotate ) specific pipe
    private void movePipes(Transform objPIVOT, int number)
    {
        if(number != startTile && number != endTile){
            float step = 90;

            int position = inGamePipesPositionList[number];

            position++;
            position = position % 4;

            float newAngle = step * position;

            tilesList[number].transform.localEulerAngles = new Vector3(0, 0, -newAngle);

       
        inGamePipesPositionList[number] = position;

        }
    }

 //--> Check if puzzle is solved 
    private void CheckIfPuzzleSolved(){
        if(createPuzzleArray()){
            puzzleSolved();
        }
        if (ingameGlobalManager.instance._P)        // Debug Mode
            puzzleSolved();
    }


//--> Create an array that represent the puzzle
    private bool createPuzzleArray()
    {
        int raw = Mathf.RoundToInt(_NumberOfKey / _Column);
        int[,] arrPuzzle = new int[_Column * 3 + 2, raw * 3 + 2];
        bool[,] lastPos = new bool[_Column * 3 + 2, raw * 3 + 2];
        int[] pipeStartPosition = new int[2] { mazeStartTileX, mazeStartTileY };
        int[] pipeEndPosition = new int[2] { mazeEndTileX, mazeEndTileY };


        //-> Create the puzzle array

        // Upper border
        for (var m = 0; m <= _Column * 3 + 1; m++)
            arrPuzzle[m, 0] = 2;

        int currentRaw = 1;
        int currentColumn = 0;

        for (var f = 0; f < raw; f++)                   // How many raw
        {
            int number = 0;
            int currentSel = 0 + f * _Column;
            for (var k = 0; k < 3; k++)                 // Subdivision in a pipe
            {
                //Border Left
                arrPuzzle[0, currentRaw] = 2;

                number = 0;
                currentSel = 0 + f * _Column;

                for (var i = 0; i < _Column * 3; i++)
                {
                    // Line Up
                    if (k == 0 && number == 0 ||
                        k == 0 && number == 2 ||
                        k == 0 && number == 1 && b_returnValidExitPosition(currentSel)[0] == false)           // Check Up
                    {
                        arrPuzzle[currentColumn + 1, currentRaw] = 2;
                    }
                    // Line Mid
                    else if (k == 1 && number == 0 && b_returnValidExitPosition(currentSel)[3] == false ||    // Check Right
                             k == 1 && number == 2 && b_returnValidExitPosition(currentSel)[1] == false)      // Check Right
                    {
                        arrPuzzle[currentColumn + 1, currentRaw] = 2;
                    }
                    // Line Down
                    else if (k == 2 && number == 0 ||
                             k == 2 && number == 2 ||
                             k == 2 && number == 1 && b_returnValidExitPosition(currentSel)[2] == false)      // Check Down
                    {
                        arrPuzzle[currentColumn + 1, currentRaw] = 2;
                    }
                    else
                    {
                        arrPuzzle[currentColumn + 1, currentRaw] = 1;
                    }

                    number++;
                    number %= 3;
                    if (number == 0)
                        currentSel++;

                    currentColumn++;
                    currentColumn %= _Column * 3;

                }

                // Border Right
                arrPuzzle[_Column * 3 + 1, currentRaw] = 2;
                currentRaw++;
            }

        }

        //-> Bottom border
        for (var m = 0; m <= _Column * 3 + 1; m++)
        {
            arrPuzzle[m, raw * 3 + 1] = 2;
        }


        for (int i = 0; i < _Column * 3 + 2; i++)
        {         // Column
            for (int j = 0; j < raw * 3 + 2; j++)       // Raw
            {
                lastPos[i, j] = false;          // init array
            }
        }

        //-> Check if the puzzle is solved
        return b_checkIfPuzzleCouldbeSolved(arrPuzzle, lastPos, pipeStartPosition[0], pipeStartPosition[1], pipeEndPosition);

    }



    private bool b_checkIfPuzzleCouldbeSolved(int[,] arrPuzzle, bool[,] lastPos, int X_pipeStart, int Y_PipeStart, int[] _pipeEndPosition)
    {
        int raw = Mathf.RoundToInt(_NumberOfKey / _Column);

        if (X_pipeStart == _pipeEndPosition[0] && Y_PipeStart == _pipeEndPosition[1])                                                               // Puzzle is complete
            return true;

        if (arrPuzzle[X_pipeStart, Y_PipeStart] == 2 || lastPos[X_pipeStart, Y_PipeStart])                                                          // no more movement available
            return false;

        lastPos[X_pipeStart, Y_PipeStart] = true;


        if (Y_PipeStart != 0 && b_checkIfPuzzleCouldbeSolved(arrPuzzle, lastPos, X_pipeStart, Y_PipeStart - 1, _pipeEndPosition))                   // Check Up
            return true;

        if (Y_PipeStart != raw * 3 + 2 - 1 && b_checkIfPuzzleCouldbeSolved(arrPuzzle, lastPos, X_pipeStart, Y_PipeStart + 1, _pipeEndPosition))     // Check Down
            return true;

        if (X_pipeStart != 0 && b_checkIfPuzzleCouldbeSolved(arrPuzzle, lastPos, X_pipeStart - 1, Y_PipeStart, _pipeEndPosition))                   // Check Left
            return true;

        if (X_pipeStart != _Column * 3 + 2 - 1 && b_checkIfPuzzleCouldbeSolved(arrPuzzle, lastPos, X_pipeStart + 1, Y_PipeStart, _pipeEndPosition))  // Check Right
            return true;

        return false;
    }


//--> Return valid exit position for a specific pipe depending his rotation and type
    private bool[] b_returnValidExitPosition(int selectedPosition)
    {
        bool[] arrExistPosition = new bool[4] { false, false, false, false };

        if (PipesTypeList[selectedPosition] == 0)           // no position
        {
        }

        if (PipesTypeList[selectedPosition]== 1)           // Vertical
        {
            if (inGamePipesPositionList[selectedPosition] == 0)
                arrExistPosition = new bool[4] { true, false, true, false };
            if (inGamePipesPositionList[selectedPosition] == 1)
                arrExistPosition = new bool[4] { false, true, false, true };
            if (inGamePipesPositionList[selectedPosition] == 2)
                arrExistPosition = new bool[4] { true, false, true, false };
            if (inGamePipesPositionList[selectedPosition] == 3)
                arrExistPosition = new bool[4] { false, true, false, true };
        }

        if (PipesTypeList[selectedPosition] == 2)           // T
        {
            if (inGamePipesPositionList[selectedPosition] == 0)
                arrExistPosition = new bool[4] { true, true, true, false };
            if (inGamePipesPositionList[selectedPosition] == 1)
                arrExistPosition = new bool[4] { false, true, true, true };
            if (inGamePipesPositionList[selectedPosition] == 2)
                arrExistPosition = new bool[4] { true, false, true, true };
            if (inGamePipesPositionList[selectedPosition] == 3)
                arrExistPosition = new bool[4] { true, true, false, true };
        }


        if (PipesTypeList[selectedPosition] == 3)           // elbow
        {
            if (inGamePipesPositionList[selectedPosition] == 0)
                arrExistPosition = new bool[4] { true, true, false, false };
            if (inGamePipesPositionList[selectedPosition] == 1)
                arrExistPosition = new bool[4] { false, true, true, false };
            if (inGamePipesPositionList[selectedPosition] == 2)
                arrExistPosition = new bool[4] { false, false, true, true };
            if (inGamePipesPositionList[selectedPosition] == 3)
                arrExistPosition = new bool[4] { true, false, false, true };
        }

        if (PipesTypeList[selectedPosition] == 4)           // Vertical
        {
            arrExistPosition = new bool[4] { true, true, true, true };
        }

        return arrExistPosition;
    }
}
