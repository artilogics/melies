//Description : LeverPuzzle : Manage the Lever Puzzle behaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeverPuzzle : MonoBehaviour {
    public bool                         SeeInspector = false;
    public bool                         helpBoxEditor = true;

    public int                          HowManyLeverPosition = 2;
    public int                          startAngle = 50;
    public int                          totalMovement = 100;                            // Rotation from start position to end position in degres 
    public List<int>                    LeverPositionList = new List<int>();
    public List<int>                    LeverSolutionList = new List<int>();

    public List<bool>                   LeverDirectionUpList = new List<bool>();
    public List<bool>                   LeverDirectionUpSolutionList = new List<bool>();

    public List<int>                    inGameLeverPositionList = new List<int>();
    public List<bool>                   inGameLeverDirectionUpList = new List<bool>();

    public List<Renderer>               lightList = new List<Renderer>();

    [System.Serializable]
    public class LinkLever
    {
        public List<int> _leverList;
    }

    [SerializeField]
    public List<LinkLever>              linkLever;

    public bool                         b_LinkMode = false;
    public bool                         b_SelectLeversToLink = false;


    public Color                        Emission_Off = new Color(0, 0, 0);
    public Color                        Emission_On = new Color(1, 1, 1);


    public int                          selectDefaultTile = 0;
    public List<GameObject>             defaultTileList = new List<GameObject>();
    public GameObject                   defaultTile;
  

    public int                          _Column = 3;
    public int                          _NumberOfKey = 10;

    public int                          toolbarCurrentValue = 0;
    public int                          SquareSize = 80;
    public int                          currentSelectedSprite = 0;
    public List<GameObject>             tilesList = new List<GameObject>();


    public string                       resultCode = "";

    public List<int>                    positionList = new List<int>();

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


    public bool                         VisualizeSprite = true;


    public Text                         txt_result;

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


    // Use this for initialization
    void Start()
    {
        //--> Every Puzzle  ----> BEGIN <----
        camManager = GetComponent<focusCamEffect>();
        _conditionsToAccessThePuzzle = GetComponent<conditionsToAccessThePuzzle>();
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();
        //----> END <----


        //--> Common for all puzzle ----> BEGIN <----
        _detectClick = new detectPuzzleClick();                 // Access Class that allow to detect click (Mouse, Joystick, Mobile) 


        for (var i = 0; i < lightList.Count;i++){
            if(lightList[i] != null)
                lightList[i] = lightList[i].GetComponent<Renderer>();      
        }
         


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
            if (!iconPosition.activeInHierarchy && ingameGlobalManager.instance.currentPuzzle == null)
            {
                iconPosition.SetActive(true);
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
        initCurrentPosition("");                             // init Lever position

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
        int number2 = 0;

        //--> Actions to do for this puzzle ----> BEGIN <----
        if (s_ObjectDatas == ""){                               // Save Doesn't exist
            //txt_result.text = "";
            initCurrentPosition("SaveDoesntExist");
        }
        else{                                                   // Save exist
            //txt_result.text = codes[1];
            if (inGameLeverPositionList.Count == 0)                          // Load saved value in inGameLeverPositionList and inGameLeverDirectionUpList
            {
                for (var i = 0; i < LeverPositionList.Count; i++)
                {
                    inGameLeverPositionList.Add(0);
                    inGameLeverDirectionUpList.Add(true);
                    number2+= 2;

                }
            }

            int number = 1;
            for (var i = 0; i < inGameLeverPositionList.Count; i++)
            {
               
                inGameLeverPositionList[i] = int.Parse(codes[number]);

                if (codes[number+1] == "F")
                    inGameLeverDirectionUpList[i] = false;
                else if (codes[number + 1] == "T")
                    inGameLeverDirectionUpList[i] = true;

                number += 2;
            }

            //Debug.Log("Number : " + number2);
            number2++;
            if (codes[number2] == "T")
                b_popUpDone = true;
            else
                b_popUpDone = false; 

            number2++;
            //Debug.Log("Number : " + number);

            if (codes[number2] == "T")
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
        for (var i = 0; i < inGameLeverPositionList.Count; i++)
        {
            value += inGameLeverPositionList[i] + "_";          // Save the current lever position
            value += r_TrueFalse(inGameLeverDirectionUpList[i]) + "_";          // Save the current lever direction
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


                moveLever(objClicked.transform, int.Parse(objClicked.transform.parent.name));

                for (var i = 0; i < linkLever[int.Parse(objClicked.transform.parent.name)]._leverList.Count;i++){
                    moveLever(tilesList[linkLever[int.Parse(objClicked.transform.parent.name)]._leverList[i]].transform,
                              linkLever[int.Parse(objClicked.transform.parent.name)]._leverList[i]); 
                }

                CheckIfPuzzleSolved();
            }
        }
    }

    private void initCurrentPosition(string s_SaveState){
        if(inGameLeverPositionList.Count == 0){
            for (var i = 0; i < LeverPositionList.Count; i++)
            {
                inGameLeverPositionList.Add(LeverPositionList[i]);
                inGameLeverDirectionUpList.Add(LeverDirectionUpList[i]);
            }  
        }

        for (var i = 0; i < LeverPositionList.Count; i++)
        {
            float step = totalMovement / (HowManyLeverPosition-1);

            float newAngle = 0f;
            if (s_SaveState == "SaveDoesntExist" || s_SaveState == "")      // Save slot doesn't exist or Reset button is pressed
                newAngle = startAngle - step * (LeverPositionList[i]);
            if (s_SaveState == "SaveExist")                                 // Save slot exist
                newAngle = startAngle - step * (inGameLeverPositionList[i]);
            

            //Debug.Log(i + " : " + newAngle);
            tilesList[i].transform.localEulerAngles = new Vector3(newAngle, 0, 0);


            if (s_SaveState == "SaveDoesntExist" || s_SaveState == ""){      // Save slot doesn't exist or Reset button is pressed
                inGameLeverPositionList[i] = LeverPositionList[i];
                inGameLeverDirectionUpList[i] = LeverDirectionUpList[i];
            }

            checkSwitchOnLight(i);
        }

    }


    private void moveLever(Transform objPIVOT, int number)
    {
        float step = totalMovement / (HowManyLeverPosition - 1);

        int position = inGameLeverPositionList[number];
        bool directionUp = inGameLeverDirectionUpList[number];

        if (directionUp)
        {

            if (position == 1)
            {
                directionUp = false;
            }

            position--;
        }
        else
        {

            if (position == HowManyLeverPosition - 2)
            {
                directionUp = true;
            }

            position++;
        }

        float newAngle = startAngle - step * position;

        //Debug.Log(position + " : " + newAngle);
        tilesList[number].transform.localEulerAngles = new Vector3(newAngle, 0, 0);


        inGameLeverPositionList[number] = position;
        inGameLeverDirectionUpList[number] = directionUp;

        checkSwitchOnLight(number);

        //--> Check Light
    }


    private void checkSwitchOnLight(int number){
        if (lightList[number] != null){
            if (lightList[number]
           && LeverSolutionList[number] == inGameLeverPositionList[number])
            {
                lightList[number].material.SetColor("_EmissionColor", Emission_On);
            }
            else if (lightList[number])
            {
                lightList[number].material.SetColor("_EmissionColor", Emission_Off);
            } 
        }
    }
  
    private void CheckIfPuzzleSolved(){
        bool result = true;

        for (var i = 0; i < inGameLeverPositionList.Count;i++){
            if (LeverSolutionList[i] != inGameLeverPositionList[i]){
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

}
