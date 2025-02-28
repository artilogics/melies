// Description : cylinderPuzzle : Manage Cylinder Puzzle type
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cylinderPuzzle : MonoBehaviour {
    public bool             SeeInspector = false;
    public bool             helpBoxEditor = true;

    public int              puzzleType = 0;      // Cylinder or Circle
    public int              puzzleSubType = 0;  // Horizontal/ Vertical or nested/Align

    public int              HowManyCylinderPosition = 2;
    public int              startAngle = 50;
    public int              totalMovement = 100;                            // Rotation from start position to end position in degres 
    public List<int>        CylinderPositionList = new List<int>();
    public List<int>        CylinderSolutionList = new List<int>();

    public List<int>        inGameCylinderPositionList = new List<int>();

    [System.Serializable]
    public class LinkCylinder
    {
        public List<int> _CylinderList;
    }

    [SerializeField]
    public List<LinkCylinder> linkCylinder;

    public bool             b_LinkMode = false;
    public bool             b_SelectCylindersToLink = false;

    public int              selectDefaultTile = 0;
    public List<GameObject> defaultTileList = new List<GameObject>();
    public GameObject       defaultTile;
 
    public int              _Column = 3;
    public int              _NumberOfKey = 10;

    public int              toolbarCurrentValue = 0;
    public int              SquareSize = 80;
    public int              currentSelectedSprite = 0;
    public List<GameObject> tilesList = new List<GameObject>();

    public List<int>        positionList = new List<int>();

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

    public bool             VisualizeSprite = true;

    public Text             txt_result;

    public GameObject       iconPosition;

    public bool             b_popUpDone = false;
    public GameObject       popUpObject;
    public float            popupSpeed = 3;


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
        initCurrentPosition("");                             // init Cylinder position

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
            //txt_result.text = codes[1];
            if (inGameCylinderPositionList.Count == 0)                          // Load saved value in inGameCylinderPositionList
            {
                for (var i = 0; i < CylinderPositionList.Count; i++){       
                    inGameCylinderPositionList.Add(0);
                    number++;
                }
            }

            for (var i = 0; i < inGameCylinderPositionList.Count; i++)
            {
                inGameCylinderPositionList[i] = int.Parse(codes[i+1]);
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
        for (var i = 0; i < inGameCylinderPositionList.Count;i++){
            value += inGameCylinderPositionList[i] + "_";          // Save the current Cylinder or Circle position
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


                moveCylinder(objClicked.transform, int.Parse(objClicked.transform.parent.name));

                for (var i = 0; i < linkCylinder[int.Parse(objClicked.transform.parent.name)]._CylinderList.Count;i++){
                    moveCylinder(tilesList[linkCylinder[int.Parse(objClicked.transform.parent.name)]._CylinderList[i]].transform,
                              linkCylinder[int.Parse(objClicked.transform.parent.name)]._CylinderList[i]); 
                }

                CheckIfPuzzleSolved();
            }
        }
    }

    private void initCurrentPosition(string s_SaveState){
       
            

        if(inGameCylinderPositionList.Count == 0){
            for (var i = 0; i < CylinderPositionList.Count; i++)
            {
                inGameCylinderPositionList.Add(CylinderPositionList[i]);
            }  
        }

        for (var i = 0; i < CylinderPositionList.Count; i++)
        {
            float step = 360 / HowManyCylinderPosition;
            float newAngle = 0f;
            if (s_SaveState == "SaveDoesntExist" || s_SaveState == "")      // Save slot doesn't exist or Reset button is pressed
                newAngle = step * CylinderPositionList[i];
            if (s_SaveState == "SaveExist")                                 // Save slot exist
                newAngle = step * inGameCylinderPositionList[i];

            if (puzzleType == 1)    // Cylinder
                tilesList[i].transform.localEulerAngles = new Vector3(-newAngle, 0, 0);
            if (puzzleType == 2)   // Circle
                tilesList[i].transform.localEulerAngles = new Vector3(90, 0, newAngle);
            

            if (s_SaveState == "SaveDoesntExist" || s_SaveState == "")      // Save slot doesn't exist or Reset button is pressed
                inGameCylinderPositionList[i] = CylinderPositionList[i];
        }

    }


    private void moveCylinder(Transform objPIVOT, int number)
    {
        float step = 360 / HowManyCylinderPosition;

        int position = inGameCylinderPositionList[number];

        position++;
        position = position % HowManyCylinderPosition;

        float newAngle = step * position;


        if (puzzleType == 1)    // Cylinder
            tilesList[number].transform.localEulerAngles = new Vector3(-newAngle, 0, 0);
        if (puzzleType == 2)   // Cercle
            tilesList[number].transform.localEulerAngles = new Vector3(90, 0, newAngle);
        
        inGameCylinderPositionList[number] = position;
    }


   
  
    private void CheckIfPuzzleSolved(){
        bool result = true;

        for (var i = 0; i < inGameCylinderPositionList.Count;i++){
            if (CylinderSolutionList[i] != inGameCylinderPositionList[i]){
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
