// Description : conditionsToAccessThePuzzle : This script is use to check if the puzzle could be unlock or not
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conditionsToAccessThePuzzle : MonoBehaviour {
    public bool             SeeInspector = false;
    public bool             onlyFocusMode = false;

    public bool             b_PuzzleIsActivated = false;        // return if the puzzle is currently activated

    private GameObject      iconPuzzleNotAvailable;             // refers to Image in Canvas_PlayerInfos
    private GameObject      iconResetPuzzle;                    // refers to Button in Canvas_PlayerInfos

    private GameObject      iconMobile_ExitPuzzle;
    private GameObject      iconPuzzleClue;                    // refers to Button in Canvas_PlayerInfos
    public AP_Clue         objClueBox;                    // know if the puzzle has a hint

    public bool b_OnlyFocus = false;
    public bool b_Generic = false;


    public List<EditorMethodsList.MethodsList> methodsList      // Create a list of Custom Methods that could be edit in the Inspector
     = new List<EditorMethodsList.MethodsList>();

    public CallMethods      callMethods;                        // Access script taht allow to call public function in this script.

    private actionsWhenPuzzleIsSolved  _actionsWhenPuzzleIsSolved;                    // Access component actionsWhenPuzzleIsSolved


    [System.Serializable]
    public class idList
    {
        public int ID = 0;          // entry ID in the window tab
        public int uniqueID = 0;    // entry Unique ID
    }

    public List<idList>     inventoryIDList = new List<idList>() { new idList() };     // Lock Section : Check if an Object is in the player inventory using his ID

    public bool             b_feedbackActivated = false;                               // Lock Section : if true text is displayed
    public List<idList>     feedbackIDList = new List<idList>() { new idList() };      // Lock Section : Check if an Object is in the player inventory using his ID

    public bool b_ActivateDoubleTapIcon = true;    // True : ACtivated the double tap UI icon if game is played on mobile platform

	private void Start()
	{
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();                                   // Access actionsWhenPuzzleIsSolved component
       // GameObject tmp = GameObject.Find("Canvas_PlayerInfos");        

        GameObject Grp_canvas = GameObject.Find("Grp_Canvas");
        Transform[] allTransform = Grp_canvas.GetComponentsInChildren<Transform>(true);

        UIVariousFunctions canvas_PlayerInfos = null;

        foreach (Transform obj in allTransform)
        {
            if (obj.name == "Canvas_PlayerInfos")
                canvas_PlayerInfos = obj.gameObject.GetComponent<UIVariousFunctions>();   // Access to the UIVariousFunctions script

            if (obj.name == "btn_Clue")
                iconPuzzleClue = obj.gameObject;
        }



        if(canvas_PlayerInfos){
            GameObject tmp2 = canvas_PlayerInfos.obj_PuzzleNotAvailable;
            if(tmp2)        
                iconPuzzleNotAvailable = tmp2;                                              // Access object PuzzleNotAvailable
            tmp2 = canvas_PlayerInfos.obj_ResetPuzzle;
            if (tmp2)
                iconResetPuzzle = tmp2;                                                     // Access object iconResetPuzzle
            tmp2 = canvas_PlayerInfos.obj_Btn_Mobile_ExitPuzzle;
            if (tmp2)
                iconMobile_ExitPuzzle = tmp2;                                                     // Access object iconMobile_ExitPuzzle
        }


        //Find if a ClueBox is attached to the puzzle
        allTransform = GetComponentsInChildren<Transform>(true);
        foreach (Transform obj in allTransform)
        {
            if (obj.name == "ClueBox"){
                objClueBox = obj.gameObject.GetComponent<AP_Clue>();   // Access to the AP_Clue script
                break;
            }
        }
	}


//--> Call from puzzle object to start puzzle initialization. Case : Start Focus Process because the puzzle is not inside a door or a drawer .
    public void F_ActivateFocus(GameObject currentPuzzle){
        if (!b_Generic){
            ingameGlobalManager.instance.navigationList.Add("Focus");           // Use by the script BackInputs.cs
            enterFocusMode(currentPuzzle, true);                                 // Start Focus Mode  
        }
        else{
            checkIfPuzzleIsAvailable();
        }
    }

//--> Call from door or drawer to start puzzle initialization
    public void F_PuzzleInsideDoorOrDrawer(GameObject currentPuzzle)
    {
        enterFocusMode(currentPuzzle,false);                                // Start Focus Mode
    }


//--> Start the focus process
    private void enterFocusMode(GameObject currentPuzzle,bool b_StartFocus)
    {
        if(b_StartFocus){
            currentPuzzle.GetComponent<focusCamEffect>().MoveCameraToFocusPosition(currentPuzzle.GetComponent<focusCamEffect>().targetFocusCamera,b_ActivateDoubleTapIcon);
        }

        StartCoroutine(checkEnterFocusEnded(b_StartFocus));
    }


//--> Check when focus ended. Then Check if the puzzle could be activated
    private IEnumerator checkEnterFocusEnded(bool b_StartFocus)
    {
        if (b_StartFocus){
            focusCamEffect _focus = ingameGlobalManager.instance.GetComponent<focusCamEffect>();

            yield return new WaitUntil(() => _focus.b_MovementEnded == false);
            yield return new WaitUntil(() => _focus.b_MovementEnded == true);
        }

        checkIfPuzzleIsAvailable();

        ingameGlobalManager.instance.currentPuzzle = GetComponent<conditionsToAccessThePuzzle>();


        if(b_PuzzleIsActivated){
            if (!gameObject.GetComponent<focusOnly>() && ingameGlobalManager.instance.b_Joystick){
                ingameGlobalManager.instance._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);
                ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive(true);
            }
                



            if (gameObject.GetComponent<AP_.DragAndDrop>())
                ingameGlobalManager.instance.b_dragAndDropActivated = true;
        }

      


        //b_FocusEnded = true;
        yield return null;
    }


//--> Exit the puzzle
    public void F_PuzzleInsideDoorOrDrawerExit(GameObject currentPuzzle)
    {
        if (iconPuzzleNotAvailable)
            iconPuzzleNotAvailable.SetActive(false);
        
        if (iconResetPuzzle)
            iconResetPuzzle.SetActive(false);
        if (iconPuzzleClue && objClueBox != null)
            iconPuzzleClue.SetActive(false);
        


        if (iconMobile_ExitPuzzle && !ingameGlobalManager.instance.b_DesktopInputs)
            iconMobile_ExitPuzzle.SetActive(false);

        b_PuzzleIsActivated = false;
    }

//--> Check if the puzzle is locked or not
    public void checkIfPuzzleIsAvailable(){
        if (checkIfNeededObjectAreInTheInventory()                    // Objects needed are available in the inventory
                && callMethods.Call_A_Method_Only_Boolean(methodsList)     // all the custom method return true
                && iconResetPuzzle
                && !_actionsWhenPuzzleIsSolved.returnactionsWhenPuzzleIsSolved()
                 ||
                 ingameGlobalManager.instance._D)                           // Debug Mode Activated
        {
            if (!b_OnlyFocus && !b_Generic){            // It is a puzzle
                iconResetPuzzle.SetActive(true);

                if(iconPuzzleClue && objClueBox != null)
                   iconPuzzleClue.SetActive(true);
            }
                

            DeactivateObjectInTheInvenetoryViewer();
            b_PuzzleIsActivated = true;



        }
        else if ((!checkIfNeededObjectAreInTheInventory()                      // Objects needed are not available in the inventory
             || !callMethods.Call_A_Method_Only_Boolean(methodsList) )    // A custom method return false     
            && iconPuzzleNotAvailable 
            && ingameGlobalManager.instance.b_focusModeIsActivated)
        {
            iconPuzzleNotAvailable.SetActive(true);                       // Activate an image to say that the puzzle is not available
            b_PuzzleIsActivated = false;                                   


            //-> Display feedback info
            ingameGlobalManager gManager = ingameGlobalManager.instance;
            if(b_feedbackActivated && gManager.canvasPlayerInfos._infoUI){
                bool b_Exist = false;
                for (var i = 0; i < gManager.canvasPlayerInfos._infoUI.listRefGameObject.Count; i++)
                {
                    if (gameObject == gManager.canvasPlayerInfos._infoUI.listRefGameObject[i])
                        b_Exist = true;
                }
                if (!b_Exist)
                    gManager.canvasPlayerInfos._infoUI.playAnimInfo(gManager.currentFeedback.diaryList[gManager.currentLanguage]._languageSlot[feedbackIDList[0].ID].diaryTitle[0], "Feedback",gameObject);  
            }
        }

        if (!b_OnlyFocus && !b_Generic)
        {
            if (iconMobile_ExitPuzzle && !ingameGlobalManager.instance.b_DesktopInputs)
                iconMobile_ExitPuzzle.SetActive(true);
        }
         
        if (b_Generic)
        {
            //Debug.Log("Here");
            
            GetComponent<AP_Generic>().AP_Actions();
        }

    }



//--> Check if some objects are in the player inventory
    private bool checkIfNeededObjectAreInTheInventory(){
        //bool result = true;

        if (ingameGlobalManager.instance.currentPlayerInventoryList.Count == 0
            && inventoryIDList.Count > 0)
        {
            return false;
        }

        int counter = 0;
        for (var i = 0; i < inventoryIDList.Count; i++)
        {
            for (var j = 0; j < ingameGlobalManager.instance.currentPlayerInventoryList.Count; j++)
            {
                if (ingameGlobalManager.instance.currentPlayerInventoryList[j] == inventoryIDList[i].ID)
                {
                    counter++;
                    //result = true;
                    //break;
                }
                //result = false;
            }
        }

        if(counter == inventoryIDList.Count)
            return true;
        else
            return false;
    }

    private void DeactivateObjectInTheInvenetoryViewer(){
        for (var i = 0; i < inventoryIDList.Count; i++)
        {
            for (var j = 0; j < ingameGlobalManager.instance.currentPlayerInventoryList.Count; j++)
            {
                if (ingameGlobalManager.instance.currentPlayerInventoryList[j] == inventoryIDList[i].ID)
                {
                    ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList[j] = false;
                }
            }
        }
    }

}
