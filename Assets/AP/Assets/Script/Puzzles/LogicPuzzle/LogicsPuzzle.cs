//Description : LogicsPuzzle : Manage the Logics Puzzle behaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicsPuzzle : MonoBehaviour {
    public bool                         SeeInspector = false;
    public bool                         helpBoxEditor = true;

    public int                          LogicType = 0;                                  // Logics type 

    public int                          currentColor = 0;
    public List<Color>                  colorList = new List<Color>(){Color.white};

   
    public int                          HowManyLogics = 8;
    public List<int>                    LogicsTypeList = new List<int>();
    public List<int>                    AxisTypeList = new List<int>();

    public List<bool>                   LogicsUseOrFakeList = new List<bool>();
    public List<bool>                   LogicsInitPositionWhenStart = new List<bool>();
    public List<bool>                   LogicsAvailableWhenStart = new List<bool>();
    public List<bool>                   AxisAvailableWhenStart = new List<bool>();

    public List<int>                    LogicsPositionList = new List<int>();
 
    public List<int>                    inGameLogicsPositionList = new List<int>();
    public List<bool>                   inGameAxis = new List<bool>();

    public int                          selectDefaultTile = 0;
    public List<GameObject>             defaultTileList = new List<GameObject>();
    public GameObject                   defaultTile;

    public int                          _Column = 3;
    public int                          _NumberOfKey = 10;

    public int                          toolbarCurrentValue = 0;
    public int                          SquareSize = 80;
    public int                          currentSelectedSprite = 0;
    public List<GameObject>             pivotLogicList = new List<GameObject>();
    public List<GameObject>             LogicList = new List<GameObject>();
    private List<Logic>                 pLogicList = new List<Logic>();
    private List<LogicCheckCollision>   pLogicOnTriggerList = new List<LogicCheckCollision>();

    public bool                         b_PuzzleSolved = false;         // Know if the puzzle is solved
    public LayerMask                    myLayer;                        // Raycast is done only on layer 15 : Puzzle
    public bool                         b_UsePuzzleFocus = true;        // Use the puzzle focus. If false : focus step is bypass
    public focusCamEffect               camManager;                     // access focusCamEffect component
    public conditionsToAccessThePuzzle _conditionsToAccessThePuzzle;    // access conditionsToAccessThePuzzle component
    public actionsWhenPuzzleIsSolved   _actionsWhenPuzzleIsSolved;      // access actionsWhenPuzzleIsSolved component

    private detectPuzzleClick           _detectClick;

    public int                          validationButtonJoystick = 4;

    public AudioClip                    a_KeyPressed;
    public float                        a_KeyPressedVolume = 1;
    public AudioClip                    a_Reset;
    public float                        a_ResetVolume = 1;

    private AudioSource                 a_Source;

    private AP_.DragAndDrop                 dragAndDrop;
    private List<SpriteRenderer>        listOfSelectedPuzzlePosition = new List<SpriteRenderer>();

    public GameObject                   iconPosition;

    public bool                         b_popUpDone = false;
    public GameObject                   popUpObject;
    public float                        popupSpeed = 3;


    [System.Serializable]
    public class idList
    {
        public int ID = 0;          // entry ID in the window tab
        public int uniqueID = 0;    // entry Unique ID
    }

    public bool                         b_UIFeedback = true;
    public bool                         b_feedbackActivated = false;                               // Lock Section : if true text is displayed
    public List<idList>                 feedbackIDList = new List<idList>() { new idList() };      // Lock Section : Check if an Object is in the player inventory using his ID
    bool once = false;

    // Use this for initialization
    void Start()
    {
        dragAndDrop = GetComponent<AP_.DragAndDrop>();

        for (var i = 0; i < pivotLogicList.Count;i++){
            listOfSelectedPuzzlePosition.Add(pivotLogicList[i].transform.parent.GetComponent<SpriteRenderer>()); 
            pLogicList.Add(LogicList[i].GetComponent<Logic>());
            pLogicOnTriggerList.Add(LogicList[i].transform.GetChild(0).GetComponent<LogicCheckCollision>());
            inGameLogicsPositionList.Add(-1);
            inGameAxis.Add(false);

            if (!LogicsInitPositionWhenStart[i]){
                LogicList[i].transform.GetChild(0).tag = "Untagged";
                pivotLogicList[i].transform.parent.GetComponent<Collider>().enabled = false;
                LogicList[i].transform.GetChild(0).GetComponent<Collider>().enabled = false; 
            }
                
        }

        InitListOfHandsInDragAndDropScript();

        //--> Every Puzzle  ----> BEGIN <----
        camManager = GetComponent<focusCamEffect>();
        _conditionsToAccessThePuzzle = GetComponent<conditionsToAccessThePuzzle>();
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();
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
                once = true;
                if(listOfSelectedPuzzlePosition.Count >0 && !b_PuzzleSolved){
                    dragAndDrop.F_DragAndDrop(listOfSelectedPuzzlePosition);
                }
               
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

                    if(b_UIFeedback){
                        b_UIFeedback = false;
                        displayUIInfo();
                    }

                }

                PuzzleBehaviour();
            }
            if (!iconPosition.activeInHierarchy && ingameGlobalManager.instance.currentPuzzle == null)
            {
                //dragAndDrop.DeselectObject(false);
                StartCoroutine(dragAndDrop.WaitBeforeDeselect(false));
                iconPosition.SetActive(true);
            }

            // default color for joystick fake mouse when a menu is displayed on screen
            if (once && 
                ingameGlobalManager.instance.navigationList.Count > 0 && 
                ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] != "Focus")
            {
                once = false;
                dragAndDrop.defaultColor();
            }
        }
    }


    //--> WHen the puzzle start the first time, an object popup
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

    //--> WHen the puzzle start the first time, an UI Info is displayed on screen
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
        initCurrentPosition("");                             // init Logics position

    }
   

//--> Actions when puzzle is solved
    private void puzzleSolved(){

        dragAndDrop.initAllSpriteWhenPuzzleIsSolved();                      // Init puzzle SPrites

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
            if (inGameLogicsPositionList.Count == 0)                          // Load saved value in inGameLogicsPositionList
            {
                inGameLogicsPositionList.Add(-1);
                number++;
            }

            for (var i = 0; i < inGameLogicsPositionList.Count; i++)
            {
                inGameLogicsPositionList[i] = int.Parse(codes[i+1]);
                number++;
            }
           // Debug.Log("Number : " + number);
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
        for (var i = 0; i < inGameLogicsPositionList.Count;i++){
            value += inGameLogicsPositionList[i] + "_";          // Save the current Logics or Cercle position
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
            }
            CheckIfPuzzleSolved();
        }
    }

    private void initCurrentPosition(string s_SaveState){
        //Debug.Log("Save Exist 2");
        if(inGameLogicsPositionList.Count == 0){
            for (var i = 0; i < LogicsPositionList.Count; i++)
            {
                inGameLogicsPositionList.Add(LogicsPositionList[i]);
            }  
        }

        for (var i = 0; i < LogicList.Count; i++)
        {
            //Debug.Log("Start");
            if (s_SaveState == "" || s_SaveState == "SaveDoesntExist")
            {
                if (LogicsInitPositionWhenStart[i])
                {
                    LogicList[i].transform.GetChild(0).transform.localPosition = Vector3.zero;
                    LogicList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                }
                else
                {
                    LogicList[i].transform.GetChild(0).transform.position = pivotLogicList[i].transform.position;
                    LogicList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                }
            }
            else
            {
                
                for (var j = 0; j < LogicList.Count; j++)
                {
                   // Debug.Log("Save Exist");
                    bool b_Find = false;

                    if (inGameLogicsPositionList[j] == i)
                    {
                        b_Find = true;
                        LogicList[i].transform.GetChild(0).transform.position = pivotLogicList[j].transform.position;
                       // LogicList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                        LogicList[i].transform.GetChild(0).transform.eulerAngles = pivotLogicList[j].transform.eulerAngles;
                        //Debug.Log(LogicList[i].name);
                        break;
                    }
                   

                    if (!b_Find)
                    {
                        if (LogicsInitPositionWhenStart[i]){
                            LogicList[i].transform.GetChild(0).transform.localPosition = Vector3.zero;
                            LogicList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;  
                        }
                       
                    }
                }
               
            }    
        }
    }



  
  
    private void CheckIfPuzzleSolved(){
        bool result = true;

        for (var i = 0; i < pivotLogicList.Count; i++)
        {
            for (var j = 0; j < pLogicList.Count; j++)
            {
                // Check if Logic is already place on Axis
                if (LogicList[j].transform.GetChild(0).transform.position == pivotLogicList[i].transform.position)
                {
                    if (pLogicList[j].i_AxisType == AxisTypeList[i] &&
                        inGameLogicsPositionList[i] != j)    // Check if Logic already on this axis. New Logic on Axis position. Move Old Logic on Init position
                    {
                        if(inGameLogicsPositionList[i]!= -1){
                            LogicList[inGameLogicsPositionList[i]].transform.GetChild(0).transform.localPosition = Vector3.zero;
                            LogicList[inGameLogicsPositionList[i]].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                            inGameLogicsPositionList[i] = j;
                            //Debug.Log("Here 0a");
                            break;
                        }
                    }
                }
            }
            inGameLogicsPositionList[i] = -1;                // No Logic on this axis
        }

        for (var i = 0; i < pivotLogicList.Count; i++)
        {
            for (var j = 0; j < pLogicList.Count; j++)
            {
                // Check if the pipe Axis is compatible with the Logic
                if (LogicList[j].transform.GetChild(0).transform.position == pivotLogicList[i].transform.position)
                {
                    if (pLogicList[j].i_AxisType != AxisTypeList[i]){                                // Not compatible
                        LogicList[j].transform.GetChild(0).transform.localPosition = Vector3.zero;
                       // Debug.Log("Here 1a");
                    }

                    if (pLogicList[j].i_AxisType == AxisTypeList[i])                                 // Compatible
                    {
                        inGameLogicsPositionList[i] = j;
                        LogicList[j].transform.GetChild(0).transform.eulerAngles = pivotLogicList[i].transform.eulerAngles;
                        //Debug.Log("Here 2a");
                    }
                }
            }
        }


        // Check if the Logic is the needed Logic on a specific axis
        for (var i = 0; i < pivotLogicList.Count; i++)
        {
            inGameAxis[i] = false;
            for (var j = 0; j < pLogicList.Count; j++)
            {
                if (LogicList[j].transform.GetChild(0).transform.position == pivotLogicList[i].transform.position)
                {
                  
                    if (pLogicList[j].i_AxisType == AxisTypeList[i] &&
                        pLogicList[j].i_LogicType == LogicsTypeList[i]) 
                    {
                        inGameAxis[i] = true;
                        break;
                    }
                }
            }
        }




        // Check if all axis + Logic are well associated
        for (var i = 0; i < pivotLogicList.Count; i++)
        {
            if(!inGameAxis[i] && !LogicsUseOrFakeList[i]){
                result = false;
                break;
            }
        }


        if(result){
            puzzleSolved();
        }

        if (ingameGlobalManager.instance._P)        // Debug Mode
            puzzleSolved();
    }

    public void InitListOfHandsInDragAndDropScript()
    {
        #region
        // Use to list a list of hand objects. Use to verify if an object is a part of ths puzzle
        List<GameObject> listFromThePuzzle = new List<GameObject>();
        foreach (GameObject obj in pivotLogicList)
            listFromThePuzzle.Add(obj.transform.parent.gameObject);

        dragAndDrop.InitListOfHands(listFromThePuzzle);


        // Use to list a list of Logic objects. Use to verify if an object is a part of ths puzzle
        List<GameObject> listFromThePuzzle_02 = new List<GameObject>();
        foreach (GameObject obj in LogicList)
            listFromThePuzzle_02.Add(obj.transform.GetChild(0).gameObject);

        dragAndDrop.InitListOfGearsLogics(listFromThePuzzle_02);
        #endregion
    }
}
