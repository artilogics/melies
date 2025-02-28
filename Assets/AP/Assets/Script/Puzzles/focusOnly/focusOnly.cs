// Description : focusOnly : Use to have a create a focus on a target
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class focusOnly : MonoBehaviour {
    public bool                         SeeInspector = false;
    public bool                         helpBoxEditor = true;
       
    public bool                         b_PuzzleSolved = false;                     // Know if the puzzle is solved
    public LayerMask                    myLayer;                                    // Raycast is done only on layer 15 : Puzzle
    public focusCamEffect               camManager;                                 // access focusCamEffect component
    public conditionsToAccessThePuzzle  _conditionsToAccessThePuzzle;               // access conditionsToAccessThePuzzle component
    public actionsWhenPuzzleIsSolved    _actionsWhenPuzzleIsSolved;                 // access actionsWhenPuzzleIsSolved component

    public bool                         b_VoiceOverActivated = false;               // If true a voice over is played
    public bool                         alreadyPlayed = false;                      // Play the voice only one time


    [System.Serializable]
    public class idList
    {
        public int ID = 0;              // entry ID in the window tab
        public int uniqueID = 0;        // entry Unique ID
    }

    public bool                         b_UIFeedback = true;
    public bool                         b_feedbackActivated = false;                               // If true text is displayed
    public List<idList>                 feedbackIDList = new List<idList>() { new idList() };      // Check if an Object is in the player inventory using his ID
   
    public List<idList>                 diaryIDList = new List<idList>() { new idList() };         // Play a voice over using an ID
    private VoiceOver_Manager           voiceOverManager;                                          // reference to the voice over manager



    public GameObject iconPosition;

	// Use this for initialization
	void Start () {
        camManager      = GetComponent<focusCamEffect>();
        _conditionsToAccessThePuzzle    = GetComponent<conditionsToAccessThePuzzle>();
        _actionsWhenPuzzleIsSolved = GetComponent<actionsWhenPuzzleIsSolved>();

        GameObject tmpObj = GameObject.Find("VoiceOver_Manager");
        tmpObj = GameObject.Find("VoiceOver_Manager");
        if (tmpObj) voiceOverManager = tmpObj.GetComponent<VoiceOver_Manager>();
	}

    // Update is called once per frame
    void Update()
    {
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (_conditionsToAccessThePuzzle.b_PuzzleIsActivated && !ingameGlobalManager.instance.b_Ingame_Pause)
            {
                if (iconPosition.activeInHierarchy && ingameGlobalManager.instance.currentPuzzle == _conditionsToAccessThePuzzle)
                {
                    iconPosition.SetActive(false);
                    displayUIInfo(ingameGlobalManager.instance.currentFeedback.diaryList[ingameGlobalManager.instance.currentLanguage]._languageSlot[feedbackIDList[0].ID].diaryTitle[0]);
                
                    if (voiceOverManager && b_VoiceOverActivated && !alreadyPlayed )
                    {
                        alreadyPlayed = true;
                       // Debug.Log("Play Voice");
                        voiceOverManager.setupNewVoice(
                            ingameGlobalManager.instance.currentDiary,
                            ingameGlobalManager.instance.currentLanguage,
                            diaryIDList[0].ID,
                            ingameGlobalManager.instance.currentDiary.voiceOverDescription(
                                ingameGlobalManager.instance.currentLanguage,
                                diaryIDList[0].ID),
                            ingameGlobalManager.instance.currentDiary.r_audioPriority(
                                ingameGlobalManager.instance.currentLanguage,
                                diaryIDList[0].ID),
                            true);
                    }
                }

                PuzzleBehaviour();
            }
            if (!iconPosition.activeInHierarchy && ingameGlobalManager.instance.currentPuzzle == null)
            {
                iconPosition.SetActive(true);
                displayUIInfo("");
            }
        }
       
	}

 
    public void displayUIInfo(string newText)
    {
        //-> Display feedback info
        ingameGlobalManager gManager = ingameGlobalManager.instance;
        if (b_feedbackActivated && gManager.canvasPlayerInfos._infoUI)
        {
            if(!gManager.canvasPlayerInfos.txt3DViewer.gameObject.activeSelf)
                gManager.canvasPlayerInfos.txt3DViewer.gameObject.SetActive(true);
            gManager.canvasPlayerInfos.txt3DViewer.text = newText;
        }

    }


//------> BEGIN <------ Next 6 methods are always needed in a puzzle script 

//--> Reset Puzzle when button iconResetPuzzle in Canvas_PlayerInfos is pressed
    public void F_ResetPuzzle(){
    }


//--> Actions when puzzle is solved
    private void puzzleSolved(){                                 
    }

//--> Use to load object state. Initialize the puzzle
    public void saveSystemInitGameObject(string s_ObjectDatas)
    {
        LoadingProcessDone();                       // Return that the object is loaded correctly
    }

//--> Use to save Object state
    public string ReturnSaveData()
    {
        string value = "";         
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
    }


    public void activateAnObject(GameObject obj)
    {
        if(obj)obj.SetActive(true);
    }
}
