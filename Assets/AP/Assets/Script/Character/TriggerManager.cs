// Description : TriggerManager : When player enter a trigger something is done (Play voiceover,do something...)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;


public class TriggerManager : MonoBehaviour {
	public bool 				SeeInspector = false;
	public int 					TriggerType = 0;												// The trigger type
	public bool 				alreadyPlayed = false;											// know if This trigger have been already played
	public bool 				playOnce = false;
	private int 				intPlayOnce = 0;
	public bool 				playOnlyIfNoOtherVoiceOverIsPlayed = false;						// know if the trigger is activate when a voice over is already played
	public bool 				b_DisabledPlayerMovement = false;
	public float 				DisabledMovementTimer = 5;


    public List<EditorMethodsList.MethodsList> methodsList      // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList.MethodsList>();

    public CallMethods callMethods;                     // Access script taht allow to call public function in this script.


//-> Voice Variables
	private VoiceOver_Manager 	voiceOverManager;												// reference to the voice over manager

//-> Load Level Variables
	public int BuildInSceneIndex = 0;	// The index of the scene you want to load
	public string spawnPointName = "Spawn_0";


   // public Text testTxt;


	// Use this for initialization
	void Start () {
		GameObject tmpObj = GameObject.Find ("UI_Infos");
		//if (tmpObj) info = tmpObj.GetComponent<infoUI> ();

		tmpObj = GameObject.Find ("VoiceOver_Manager");
		if (tmpObj) voiceOverManager = tmpObj.GetComponent<VoiceOver_Manager> ();
	}
	

//--> Player Enter this Trigger
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			//Debug.Log ("Character enter inside a trigger");
			if (!alreadyPlayed && playOnce && intPlayOnce == 0){
				intPlayOnce++;
				alreadyPlayed = true;
				triggerAction ();
			} else if(!playOnce) {
				triggerAction ();
			}
		}
	}



//--> What to do for each trigger type
	private void triggerAction(){
		switch (TriggerType) {
	//-> Play a voice
		case 0:									
			voiceOver ();
			if (b_DisabledPlayerMovement)
				StartCoroutine (DisabledMovementDuringWSeconds (DisabledMovementTimer));
			break;
	//-> Go to another level
		case 1:									
			ingameGlobalManager.instance.saveAndLoadManager.F_GoToAnotherLevel (spawnPointName, BuildInSceneIndex);
			break;
    //-> Custom Method
        case 2:
            //Custom Method
            if (b_DisabledPlayerMovement)
                StartCoroutine(DisabledMovementDuringWSeconds(DisabledMovementTimer));

            callAllTheMethods();
            break;
        case 3 :
           //Do nothing;
           //StartCoroutine(DisabledMovementDuringWSeconds(0));
           break;
		}
       
	}



	IEnumerator DisabledMovementDuringWSeconds(float duration){

        ingameGlobalManager.instance.b_bodyMovement = false;

        if(duration == -1f){}
        else{

            float t = 0;
            while (t != duration)
            {
                if (!ingameGlobalManager.instance.b_Ingame_Pause)
                {
                    t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                    //Debug.Log ("duration : " + t);
                    // if (testTxt) testTxt.text = "Timer : " + t;
                }
                yield return null;
            }
            //if (testTxt) testTxt.text = "Timer : Ended";
            ingameGlobalManager.instance.b_bodyMovement = true;
        }
		
		yield return null;
	}

//--> Case 0 : Play Voice
//--> Trigger Type : Voice Over
	private void voiceOver(){
		if (playOnlyIfNoOtherVoiceOverIsPlayed) {
			if (!voiceOverManager.audiosourceVoiceOver) {
				voiceOverManager.audiosourceVoiceOver = ingameGlobalManager.instance.currentPlayer.GetComponent<Character> ().voiceOverAudioSource;

			}


			if (voiceOverManager && !voiceOverManager.audiosourceVoiceOver.isPlaying)
				newVoiceOver ();					
		} else {
			if (voiceOverManager)
				newVoiceOver ();
		}
	}

//--> Launch a new Voice Over
	private void newVoiceOver(){
		TextProperties textProperties = gameObject.GetComponent<TextProperties> ();

	//-> Add the entry in the diary if needed 
		int managerID = textProperties.managerID;
			bool alreadyInDiary = false;
		if( textProperties.textList.r_Available(0,managerID)){
			
			for (var i = 0; i < ingameGlobalManager.instance.currentPlayerDiaryList.Count; i++) {
				if (textProperties.managerID == ingameGlobalManager.instance.currentPlayerDiaryList [i]) {
					alreadyInDiary = true;
					break;
				}
			}
			if(!alreadyInDiary)
				ingameGlobalManager.instance.currentPlayerDiaryList.Add (managerID);
		}

		if (textProperties) {
			if (voiceOverManager) {
				voiceOverManager.setupNewVoice (
					textProperties.r_TextList(), 
					textProperties.t_Language,
					textProperties.managerID,
					textProperties.r_TextList().voiceOverDescription (textProperties.t_Language,textProperties.managerID),
					textProperties.r_TextList().r_audioPriority(textProperties.t_Language,textProperties.managerID),
					alreadyInDiary);
			}
		}
	}

//--> Launch a new Voice Over with its ID
    public void newVoiceOver_WithID(int newID)
    {
        StartCoroutine(I_newVoiceOver_WithID(newID));
    }

    public IEnumerator I_newVoiceOver_WithID(int newID)
    {
        TextProperties textProperties = gameObject.GetComponent<TextProperties>();

        //-> Add the entry in the diary if needed 
        //int managerID = textProperties.managerID;
        bool alreadyInDiary = false;

        if (textProperties)
        {
            if (voiceOverManager)
            {
                voiceOverManager.setupNewVoice(
                    textProperties.r_TextList(),
                    textProperties.t_Language,
                    textProperties.managerID,
                    textProperties.r_TextList().voiceOverDescription(textProperties.t_Language, textProperties.managerID),
                    textProperties.r_TextList().r_audioPriority(textProperties.t_Language, textProperties.managerID),
                    alreadyInDiary);
            }
        }
        yield return null;
    }





//--> Call All the methods
    private void callAllTheMethods(){
        callMethods.Call_A_Method(methodsList);

    }



//--> Use to load object state
	public void saveSystemInitGameObject(string s_ObjectDatas){

        if(s_ObjectDatas == ""){
            GetComponent<BoxCollider>().enabled = true;
        }

		string[] codes  = s_ObjectDatas.Split('_');

		for (var i = 0; i < codes.Length; i++){
			if (i == 0) {	
                if (codes [0] == "T"){
                    alreadyPlayed = true;
                    gameObject.SetActive(false);
                    GetComponent<BoxCollider>().enabled = false;
                }
                else{
                    alreadyPlayed = false;
                    GetComponent<BoxCollider>().enabled = true;
                }
					
			}
		}

		for (var i = 0; i < ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem.Count; i++) {
			if (ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem [i] == gameObject) {
				ingameGlobalManager.instance._levelManager.listState [i] = true;
				break;
			}
		}
	}

//--> Use to save Object state
	public string ReturnSaveData () {
		string value = r_TrueFalse(alreadyPlayed);
		return value;
	}

	private string r_TrueFalse(bool s_Ref){
		if (s_Ref)return "T";
		else return "F";
	}

}
