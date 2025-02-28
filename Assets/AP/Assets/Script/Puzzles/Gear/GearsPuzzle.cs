// Description : GearsPuzzle : Manage Gear Puzzle
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearsPuzzle : MonoBehaviour {
    public bool                         SeeInspector = false;
    public bool                         helpBoxEditor = true;


    public List<Texture2D>              GearSprite = new List<Texture2D>();

    public int                          GearType = 0;      // Gears type : No Move, Horizontal, Vertical, T, Elbow 
    public int                          puzzleSubType = 0;  // Horizontal/ Vertical or nested/Align

    public int                          HowManyGearsPosition = 2;
    public List<int>                    GearsTypeList = new List<int>();
    public List<int>                    AxisTypeList = new List<int>();

    public List<bool>                   AxisRotationRight = new List<bool>();

    public List<bool>                   GearsUseOrFakeList = new List<bool>();
    public List<bool>                   GearsInitPositionWhenStart = new List<bool>();
    public List<bool>                   GearsAvailableWhenStart = new List<bool>();
    public List<bool>                   AxisAvailableWhenStart = new List<bool>();

    public List<int>                    GearsPositionList = new List<int>();
   
    public List<int>                    GearsSolutionList = new List<int>();

    public List<int>                    inGameGearsPositionList = new List<int>();
    public List<bool>                   inGameAxis = new List<bool>();

    public int                          selectDefaultTile = 0;
    public List<GameObject>             defaultTileList = new List<GameObject>();
    public GameObject                   defaultTile;

    public int                          _Column = 3;
    public int                          _NumberOfKey = 10;
            
    public int                          toolbarCurrentValue = 0;
    public int                          SquareSize = 80;
    public int                          currentSelectedSprite = 0;
    public List<GameObject>             pivotGearList = new List<GameObject>();
    public List<GameObject>             GearList = new List<GameObject>();
    private List<Gear>                  pGearList = new List<Gear>();
    private List<GearCheckCollision>    pGearOnTriggerList = new List<GearCheckCollision>();

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

    public bool b_UIFeedback = true;
    public bool b_feedbackActivated = false;                               // Lock Section : if true text is displayed
    public List<idList> feedbackIDList = new List<idList>() { new idList() };      // Lock Section : Check if an Object is in the player inventory using his ID

    public GameObject currentSelectedObject;
    bool once = false;

    // Use this for initialization
    void Start()
    {
        dragAndDrop = GetComponent<AP_.DragAndDrop>();

        for (var i = 0; i < pivotGearList.Count;i++){
            listOfSelectedPuzzlePosition.Add(pivotGearList[i].transform.parent.GetComponent<SpriteRenderer>()); 
            pGearList.Add(GearList[i].GetComponent<Gear>());
            pGearOnTriggerList.Add(GearList[i].transform.GetChild(0).GetComponent<GearCheckCollision>());
            inGameGearsPositionList.Add(-1);
            inGameAxis.Add(false);

            if (!GearsInitPositionWhenStart[i])
            {
                GearList[i].transform.GetChild(0).tag = "GearFixed";
                pivotGearList[i].transform.parent.tag = "Untagged";
                GearList[i].transform.tag = "Untagged";
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
                if(listOfSelectedPuzzlePosition.Count >0 && !b_PuzzleSolved)
                    dragAndDrop.F_DragAndDrop(listOfSelectedPuzzlePosition);
               
                if (iconPosition.activeInHierarchy){
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

                if(dragAndDrop.returnCurrentSelectedObject() != null){
                    currentSelectedObject = dragAndDrop.returnCurrentSelectedObject();
                }

               /* if(currentSelectedObject != null && !dragAndDrop.b_ValidationButtonPressed){
                    if(currentSelectedObject.GetComponent<GearCheckCollision>().returnCheckCollision() == true){
                        currentSelectedObject.transform.position = currentSelectedObject.transform.parent.position;
                        currentSelectedObject.transform.eulerAngles = currentSelectedObject.transform.parent.transform.eulerAngles;
                    } 
                }
                 */   
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

    public IEnumerator popUp(GameObject obj, Vector3 value)
    {
        b_popUpDone = true;
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
        initCurrentPosition("");                             // init Gears position

    }


//--> Actions when puzzle is solved
    private void puzzleSolved(){

        dragAndDrop.initAllSpriteWhenPuzzleIsSolved();                      // Init puzzle SPrites

        if(!b_PuzzleSolved)
            StartCoroutine(I_gearsRotations());

        //-> Actions done for all type of puzzle
        if(!b_PuzzleSolved || ingameGlobalManager.instance._P)
            _actionsWhenPuzzleIsSolved.F_PuzzleSolved();                   // Call script actionsWhenPuzzleIsSolved. Do actions when the puzzle is solved the first time.
        else
            _actionsWhenPuzzleIsSolved.b_actionsWhenPuzzleIsSolved = true; // Use when focus is called. The variable b_actionsWhenPuzzleIsSolved in script puzzleSolved equal True

        b_PuzzleSolved = true;                                  
    }


    public List<bool> gearAssociatedToAxisWhenGameSolvedNeedToRotate = new List<bool>();

    private IEnumerator I_gearsRotations(){
        float timer = 0;

        //Debug.Log("Rotation");

        for (var i = 0; i < GearsPositionList.Count; i++)
        {
            bool b_State = false;
            for (var j = 0; j < GearsPositionList.Count; j++)
            {
                if (pivotGearList[j].transform.position == GearList[i].transform.GetChild(0).transform.position
                    && !GearsUseOrFakeList[j])
                {
                    gearAssociatedToAxisWhenGameSolvedNeedToRotate.Add(true);
                    b_State = true;
                }

            }
            if(!b_State)
                gearAssociatedToAxisWhenGameSolvedNeedToRotate.Add(false);
        }

        while(timer != 2){
            timer = Mathf.MoveTowards(timer, 2, Time.deltaTime);
           
            for (var i = 0; i < pivotGearList.Count; i++)
            {
                float dir = 1;
                if (AxisRotationRight[i])
                    dir = -1;

                if(!GearsUseOrFakeList[i])
                    pivotGearList[i].transform.Rotate(Vector3.forward * dir * Time.deltaTime * 100, Space.Self);

                if (gearAssociatedToAxisWhenGameSolvedNeedToRotate[i])
                    GearList[i].transform.GetChild(0).transform.Rotate(Vector3.forward * dir * Time.deltaTime * 100, Space.Self);

            } 
            yield return null;
        }
       

        yield return null;
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
            if (inGameGearsPositionList.Count == 0)                          // Load saved value in inGameGearsPositionList
            {
                inGameGearsPositionList.Add(-1);
                number++;
            }

            for (var i = 0; i < inGameGearsPositionList.Count; i++)
            {
                inGameGearsPositionList[i] = int.Parse(codes[i+1]);
                number++;
            }
            for (var i = 0; i < inGameGearsPositionList.Count; i++)
            {
                if(codes[i + inGameGearsPositionList.Count +  1] == "T")
                    inGameAxis[i] = true;
                else
                    inGameAxis[i] = false;
                number++;
            }

           // Debug.Log("Number : " + number);
            number++;
            if (codes[number] == "T")
                b_popUpDone = true;
            else
                b_popUpDone = false;

            number++;


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
        for (var i = 0; i < inGameGearsPositionList.Count;i++){
            value += inGameGearsPositionList[i] + "_";          // Save the current Gears or Cercle position
        }
        for (var i = 0; i < inGameAxis.Count; i++)
        {
            value += r_TrueFalse(inGameAxis[i]) + "_";          // Save the current Gears or Cercle position
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
        if(inGameGearsPositionList.Count == 0){
            for (var i = 0; i < GearsPositionList.Count; i++)
            {
                inGameGearsPositionList.Add(GearsPositionList[i]);
            }  
        }

        for (var i = 0; i < GearList.Count; i++)
        {
            //Debug.Log("Start");
            if(s_SaveState == "" || s_SaveState ==  "SaveDoesntExist"){
                if (GearsInitPositionWhenStart[i]){
                    GearList[i].transform.GetChild(0).transform.localPosition = Vector3.zero;
                    GearList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                    inGameAxis[i] = !GearsInitPositionWhenStart[i];
                }
                else{
                    GearList[i].transform.GetChild(0).transform.position = pivotGearList[i].transform.position;
                    GearList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                    inGameAxis[i] = !GearsInitPositionWhenStart[i];
                } 
            }
            else{
                if (!inGameAxis[i]){
                    GearList[i].transform.GetChild(0).transform.localPosition = Vector3.zero;}
                else{
                    GearList[i].transform.GetChild(0).transform.position = pivotGearList[i].transform.position;
                    GearList[i].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                    //GearList[i].transform.GetChild(0).transform.eulerAngles = pivotGearList[i].transform.eulerAngles;
                } 
            }    
        }
    }



  
  
    private void CheckIfPuzzleSolved(){
        bool result = true;

        for (var i = 0; i < pivotGearList.Count; i++)
        {
            for (var j = 0; j < pGearList.Count; j++)
            {
                // Check if gear is already place on Axis
                if (GearList[j].transform.GetChild(0).transform.position == pivotGearList[i].transform.position)
                {
                    if (pGearList[j].i_AxisType == AxisTypeList[i] &&
                        inGameGearsPositionList[i] != j)    // Check if gear already on this axis. New Gear on Axis position. Move Old gear on Init poisition
                    {
                        if(inGameGearsPositionList[i]!= -1){
                            GearList[inGameGearsPositionList[i]].transform.GetChild(0).transform.localPosition = Vector3.zero;
                            GearList[inGameGearsPositionList[i]].transform.GetChild(0).transform.localEulerAngles = Vector3.zero;
                            inGameGearsPositionList[i] = j;
                            //Debug.Log("Here 0");
                            break;
                        }
                    }
                }
            }
            inGameGearsPositionList[i] = -1;                // No gear on this axis
        }

        for (var i = 0; i < pivotGearList.Count; i++)
        {
            for (var j = 0; j < pGearList.Count; j++)
            {
                // Check if the pipe Axis is compatible with the gear
                if (GearList[j].transform.GetChild(0).transform.position == pivotGearList[i].transform.position)
                {
                    if (pGearList[j].i_AxisType != AxisTypeList[i]){                                // Not compatible
                        GearList[j].transform.GetChild(0).transform.localPosition = Vector3.zero;
                    }

                    if (pGearList[j].i_AxisType == AxisTypeList[i])                                 // Compatible
                    {
                        inGameGearsPositionList[i] = j;
                        GearList[j].transform.GetChild(0).transform.eulerAngles = pivotGearList[i].transform.eulerAngles;
                        //InitGearMeshColliderAndRigidbody(GearList[j].transform.GetChild(0));
                        //Debug.Log("Here 1");
                    }
                }
            }
        }


        // Check if the gear is the needed gear on a specific axis
        for (var i = 0; i < pivotGearList.Count; i++)
        {
            inGameAxis[i] = false;
            for (var j = 0; j < pGearList.Count; j++)
            {
                if (GearList[j].transform.GetChild(0).transform.position == pivotGearList[i].transform.position)
                {
                  
                    if (pGearList[j].i_AxisType == AxisTypeList[i] &&
                        pGearList[j].i_GearType == GearsTypeList[i]) 
                    {
                        inGameAxis[i] = true;
                        //Debug.Log("Here 2");
                        break;
                    }
                }
            }
        }




        // Check if all axis + Gear are well associated
        for (var i = 0; i < pivotGearList.Count; i++)
        {
            if(!inGameAxis[i] && !GearsUseOrFakeList[i]){
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

    void InitGearMeshColliderAndRigidbody(GameObject refGear){
        refGear.GetComponent<MeshCollider>().convex = true;
        refGear.GetComponent<MeshCollider>().isTrigger = true;
        refGear.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void InitListOfHandsInDragAndDropScript()
    {
        #region

        // Use to list a list of hand objects. Use to verify if an object is a part of ths puzzle
        List<GameObject> listFromThePuzzle = new List<GameObject>();


        foreach (GameObject obj in pivotGearList)
            listFromThePuzzle.Add(obj.transform.parent.gameObject);

        dragAndDrop.InitListOfHands(listFromThePuzzle);

        // Use to list a list of Gear objects. Use to verify if an object is a part of ths puzzle
        List<GameObject> listFromThePuzzle_02 = new List<GameObject>();
        foreach (GameObject obj in GearList)
            listFromThePuzzle_02.Add(obj.transform.GetChild(0).gameObject);

        dragAndDrop.InitListOfGearsLogics(listFromThePuzzle_02);

        #endregion
    }
}
