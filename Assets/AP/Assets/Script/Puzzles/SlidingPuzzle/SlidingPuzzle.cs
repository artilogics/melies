//Description : Sliding Puzzle. Manage the sliding puzzle behaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingPuzzle : MonoBehaviour {
    public bool                         SeeInspector = false;           // variable for the custom editor
    public bool                         helpBoxEditor = true;           
    public GameObject                   defaultTile;
    public int                          _Raw = 3;
    public int                          _Column = 3;
    public int                          toolbarCurrentValue = 0;
    public int                          SquareSize = 80;
    public int                          currentSelectedSprite = 0;
    public int                          randomNumber = 1000;


    public List<GameObject>             tilesList = new List<GameObject>(); // list of tiles 
    public List<int>                    positionList = new List<int>();     // During the game know the position of each Tile
    public List<int>                    refPositionList = new List<int>();  // Know the default position for each tile

    public bool                         b_PuzzleSolved = false;         // Know if the puzzle is solved
    public LayerMask                    myLayer;                        // Raycast is done only on layer 15 : Puzzle
    public bool                         b_UsePuzzleFocus = true;        // Use the puzzle focus. If false : focus step is bypass
    public focusCamEffect               camManager;                     // access focusCamEffect component
    public conditionsToAccessThePuzzle _conditionsToAccessThePuzzle;    // access conditionsToAccessThePuzzle component
    public actionsWhenPuzzleIsSolved   _actionsWhenPuzzleIsSolved;      // access actionsWhenPuzzleIsSolved component


    private detectPuzzleClick           _detectClick;                   // Access the script that detect click (mobile, gaepad and keyboard)

    public int                          validationButtonJoystick = 4;   // The button use by the gamepad to validate an action in this puzzle


    public AudioClip                    a_TileMove;                     // Audio played when a tile is moved
    public float                        a_TileMoveVolume = 1;           // a_TileMove volume
    public AudioClip                    a_Reset;                        // Sound when Reset button is pressed
    public float                        a_ResetVolume = 1;
    private bool                        b_PlaySound = true;             // prevent bug with a_Reset sound when the puzzle starts
    private AudioSource                 a_Source;                       // use to access the audio source

    public GameObject iconPosition;

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
        camManager = GetComponent<focusCamEffect>();                                    // Access focusCamEffect to zoom and dezoom on puzzle
        _conditionsToAccessThePuzzle = GetComponent<conditionsToAccessThePuzzle>();     // Access the condition to unlock the puzzle
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();         // Access the actions done when the puzzle is solved
        //----> END <----


        //--> Common for all puzzle ----> BEGIN <----
        _detectClick = new detectPuzzleClick();                 // Access Class that allow to detect click (Mouse, Joystick, Mobile) 

        for (var i = 0; i < positionList.Count; i++)            // Save the default Mix. Use when puzzle is reset.
            refPositionList.Insert(0, 0);
       
        for (var i = 0; i < refPositionList.Count; i++)         // init the positionList
            refPositionList[i] = positionList[i];
        
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
        if (a_Source && a_Reset && b_PlaySound)
        {
            a_Source.clip = a_Reset;
            a_Source.volume = a_ResetVolume;
            a_Source.Play();
        }

        b_PlaySound = true;

        InitMixPosition();
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
        if (s_ObjectDatas == "")
        {                               // Save Doesn't exist
            b_PlaySound = false;
            F_ResetPuzzle();
            //Debug.Log("Puzzle Init");
        }
        else
        {                                                   // Save exist
            //Debug.Log("Puzzle Init Exist");
            for (var i = 0; i < positionList.Count; i++)
            {
                positionList[i] = int.Parse(codes[i + 1]);      // load Each tile position in PostionList List
                number++;
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

            InitPositionAfterLoading();
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

        value += r_TrueFalse(b_PuzzleSolved) + "_";         // b_PuzzleSolved : Save if the puzzle is solved or not
        //----> END <----


        //-> Specific for this puzzle ----> BEGIN <----
        for (var i = 0; i < positionList.Count;i++){       // Save Each tile position
            value +=   positionList[i] + "_";
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

            if(objClicked != null){                                                     // Player press validation button on a puzzle object
                MoveTile(objClicked.transform, int.Parse(objClicked.transform.name)); 
               
            }
        }
    }

   
//--> Update the tile array list
    private void MoveTile(Transform obj, int selectedObj)
    {
        int selectedTile = 0;
        for (var i = 0; i < positionList.Count;i++){
            if(positionList[i] == selectedObj){
                selectedTile = i;
                break;
            }
        }

        int numRaw = selectedTile / _Column;
        int numColumn = selectedTile % _Column;

        string result = "Raw : " + numRaw.ToString() + " : Column : " + numColumn.ToString();


        //-> Move if it is Possible
        ///--> Check Up position
        if (numRaw > 0)
        {
            if (positionList[selectedTile - _Column] == -1)
            {
                result += " : Could move Up";
                positionList[selectedTile - _Column] = positionList[selectedTile];
                positionList[selectedTile] = -1;
                MoveTileInSceneView(obj, "Up");
            }
        }
       
        //--> Check Down position
        if (numRaw < _Raw - 1)
        {
            result += " : Down Ok";
            if (positionList[selectedTile + _Column] == -1)
            {
                result += " : Could move Down";
                positionList[selectedTile + _Column] = positionList[selectedTile];
                positionList[selectedTile] = -1;
                MoveTileInSceneView(obj, "Down");
            }
        }
        //--> Check Right position
        if (numColumn < _Column - 1)
        {
            result += " : Right Ok";
            if (positionList[selectedTile + 1] == -1)
            {
                result += " : Could move Right";
                positionList[selectedTile + 1] = positionList[selectedTile];
                positionList[selectedTile] = -1;
                MoveTileInSceneView(obj, "Right");
            }
        }

        //--> Check Left position
        if (numColumn > 0)
        {
            result += " : Left Ok";
            if (positionList[selectedTile - 1] == -1)
            {
                result += " : Could move Left";
                positionList[selectedTile - 1] = positionList[selectedTile];
                positionList[selectedTile] = -1;
                MoveTileInSceneView(obj, "Left");
            }
        }

    }


//--> Move a selected tile in the scene view
    private void MoveTileInSceneView(Transform obj, string direction)
    {
        if(a_Source && a_TileMove){
            a_Source.clip = a_TileMove;
            a_Source.volume = a_TileMoveVolume;
            a_Source.Play();
        }

            if (direction == "Down")
            obj.parent.transform.localPosition = new Vector3(obj.parent.transform.localPosition.x, obj.parent.transform.localPosition.y - .25f, 0);
            if (direction == "Up")
            obj.parent.transform.localPosition = new Vector3(obj.parent.transform.localPosition.x, obj.parent.transform.localPosition.y + .25f, 0);
            if (direction == "Left")
            obj.parent.transform.localPosition = new Vector3(obj.parent.transform.localPosition.x - .25f, obj.parent.transform.localPosition.y, 0);
            if (direction == "Right")
            obj.parent.transform.localPosition = new Vector3(obj.parent.transform.localPosition.x + .25f, obj.parent.transform.localPosition.y, 0);

        CheckIfactionsWhenPuzzleIsSolved();
    }


//--> Check if the the puzzle is solved after having moving a tile
    private void CheckIfactionsWhenPuzzleIsSolved(){
        bool b_solved = true;
        for (var i = 0; i < positionList.Count-1; i++){
            if(i !=  positionList[i]){
                b_solved = false;
            }
        }

        if(b_solved){
            //Debug.Log("Puzzle is Solved");
            puzzleSolved();
        }

        if (ingameGlobalManager.instance._P)        // Debug Mode
        {
            puzzleSolved();
        }
    }

//--> Init the tiles position in scene view after pressing button reset the puzzle
    private void InitMixPosition()
    {
        for (var i = 0; i < refPositionList.Count; i++)
        {
            positionList[i] = refPositionList[i];
        }

        for (var i = 0; i < refPositionList.Count; i++)
        {
            if (refPositionList[i] != -1)
            {
                int numRaw = i / _Column;
                int numColumn = i % _Column;

                tilesList[refPositionList[i]].transform.localPosition = new Vector3(.25f * numColumn, -.25f * numRaw, 0);
            }
        }
    }


//--> Init the tiles position in scene view after a save slot is loading
    private void InitPositionAfterLoading()
    {
        for (var i = 0; i < positionList.Count; i++)
        {
            if (positionList[i] != -1)
            {
                int numRaw = i / _Column;
                int numColumn = i % _Column;

                tilesList[positionList[i]].transform.localPosition = new Vector3(.25f * numColumn, -.25f * numRaw, 0);
            }
        }
    }
}
