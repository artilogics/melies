// Description : VoiceOver_manager : use to play voice over + subtitle + Info UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceOver_Manager : MonoBehaviour {

	[System.Serializable]
	public class sub
	{
		public List<float> 		startPointsClip = new List<float> ();
		public List<string> 	textSub = new List<string> ();
	}

	[System.Serializable]
	public class slot
	{
		public List<string> 	diaryTitle = new List<string> (){""};
		public List<string> 	diaryText= new List<string> (){""};
		public List<AudioClip> 	diaryAudioClip= new List<AudioClip> (){null};
		public List<sub> 		diarySub= new List<sub> (){new sub()};
		public List<Sprite> 	diarySprite= new List<Sprite> (){null};
	}

	[SerializeField]
	public List<slot> 			currentVoiceOverInfo = new List<slot>();			// Save the infos : this list is use to know which text and audio is needed to play

	private TextList			currentTextList;									// variables used to display text and play sound
	private int					currentID;
	private int 				currentAudioPriority = 0;
	private int 				currentPage;

    public bool                 b_rememberLastStateSubtitle = true;
	public Text					txtSubTitle;										// Text where subtitle is displayed
	public AudioSource 			audiosourceVoiceOver;								// Audiosource use to play the voice over

	private infoUI 				info;												// Info UI reference



	// Use this for initialization
	void Start () {
		GameObject tmpObj = GameObject.Find ("UI_Infos");
		if (tmpObj)info = tmpObj.GetComponent<infoUI> ();
	}


//--> Setup a voice over
	public void setupNewVoice(TextList newtextList, int newLanguage,int ID,List<int> newElements,int newAudioPriority,bool alreadyInDiary){

		//Debug.Log ("currentLanguage : " + ingameGlobalManager.instance.currentLanguage + " : newLanguage : " + newLanguage);
//--> Audio Priority : Voice Over is played if no other sound is played or the new audioclip audio priority is higher from the first one
		if (!audiosourceVoiceOver) {
			GameObject tmp = GameObject.FindGameObjectWithTag ("charaVoiceOverAudioSource");

			if (tmp) {
				audiosourceVoiceOver = tmp.GetComponent<AudioSource> ();
			}
		}


		if (audiosourceVoiceOver) {
			//-> Save the new parameters for the voice over
			if (currentTextList != newtextList)
				currentTextList = newtextList;
			//--> Play a new voice over
			if (!audiosourceVoiceOver.isPlaying || newAudioPriority > currentAudioPriority) {
				//currentLanguage = ingameGlobalManager.instance.currentLanguage;
				//currentElements = newElements; 
				currentID = ID;

				currentAudioPriority = newAudioPriority;

				//-> Play new Voice over
				StopAllCoroutines ();
				StartCoroutine (I_setupNewVoice(alreadyInDiary));
			} 
			//-> Display info UI if the new voice audio priority is lower than the current
			else if(audiosourceVoiceOver.isPlaying && newAudioPriority <= currentAudioPriority && ID != currentID){
				displayNewInfoUI (newLanguage,ID);
			}
		}
	}

	private AudioClip findAudioCLip(int i){
		AudioClip result;

		//-> Multiple voice over allow
		if (currentTextList.multipleVoiceOver) {
			if (currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentID].diaryAudioClip [i]) {
				result = currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentID].diaryAudioClip [i];
			} else if (currentTextList.diaryList [0]._languageSlot [currentID].diaryAudioClip [i]) {
				result = currentTextList.diaryList [0]._languageSlot [currentID].diaryAudioClip [i];
			} else {
				result = null;
			}
		}
		//-> Only Default language voice over allow
		else {
			if (currentTextList.diaryList [0]._languageSlot [currentID].diaryAudioClip [i]) {
				result = currentTextList.diaryList [0]._languageSlot [currentID].diaryAudioClip [i];
			} else {
				result = null;
			}
		}


		return result;
	}

//--> Prepare a voice over
	private IEnumerator I_setupNewVoice(bool alreadyInDiary){

		currentVoiceOverInfo = new List<slot> ();
		currentVoiceOverInfo.Add (new slot());

		//Debug.Log ("Setup new Voice : " + ingameGlobalManager.instance.currentLanguage);

		currentVoiceOverInfo [0].diaryTitle[0] = currentTextList.r_Title( ingameGlobalManager.instance.currentLanguage,currentID);


		//--> Add Page + Audio Clip if needed
		currentVoiceOverInfo [0].diaryText.Clear ();
		currentVoiceOverInfo [0].diaryAudioClip.Clear ();
		for (var i = 0; i < currentTextList.diaryList[ ingameGlobalManager.instance.currentLanguage]._languageSlot[currentID].diaryText.Count; i++) {
			currentVoiceOverInfo [0].diaryText.Add (currentTextList.r_Page( ingameGlobalManager.instance.currentLanguage,currentID,i));


			currentVoiceOverInfo [0].diaryAudioClip.Add (findAudioCLip (i));

		}


		//--> Know the number of Page + the number of subtitles for each page for a specific ID
		currentVoiceOverInfo [0].diarySub = new List<sub>();

		//--> Prepare list use to display subtitle and play audio
		for (var i = 0; i < currentTextList.diaryList [ ingameGlobalManager.instance.currentLanguage]._languageSlot [currentID].diaryText.Count; i++) {
			currentVoiceOverInfo [0].diarySub.Add (new sub ());
		}
			

		int numberOfPage = currentTextList.voiceOverDescription (ingameGlobalManager.instance.currentLanguage, currentID) [0];
		//Debug.Log (numberOfPage);
		for (var i = 0; i < numberOfPage; i++) {		// i = 1 because 0 represent the number of page. The next values represent the number subtitles for each page 

			int numberOfSubForSelectedPage = currentTextList.voiceOverDescription (ingameGlobalManager.instance.currentLanguage, currentID) [i + 1];
			//Debug.Log (numberOfSubForSelectedPage);
			for (var j = 0; j < numberOfSubForSelectedPage; j++) {
				//Debug.Log (numberOfPage);
				currentVoiceOverInfo [0].diarySub [i].textSub.Add (currentTextList.diaryList [ ingameGlobalManager.instance.currentLanguage]._languageSlot [currentID].diarySub[i].textSub[j]);
				currentVoiceOverInfo [0].diarySub [i].startPointsClip.Add (currentTextList.diaryList [ 0]._languageSlot [currentID].diarySub[i].startPointsClip[j]);
			}
		}

		StartCoroutine(launchVoiceOver(alreadyInDiary));

		yield return null;
	}

    public bool subIsFinished = false;

//--> Launch Voice Over +  subtitle
	private IEnumerator launchVoiceOver(bool alreadyInDiary){
		if (audiosourceVoiceOver) {
            subIsFinished = false;
           // Debug.Log("Voice Starts");
			if (txtSubTitle)
				txtSubTitle.gameObject.SetActive (true);

			for (var i = 0; i < currentVoiceOverInfo [0].diaryText.Count; i++) {
				if (audiosourceVoiceOver != null && currentVoiceOverInfo [0].diaryAudioClip [i] != null) {
					audiosourceVoiceOver.clip = currentVoiceOverInfo [0].diaryAudioClip [i];
					audiosourceVoiceOver.Play ();
					currentPage = i;
					StartCoroutine (displaySubTitles());


                    yield return new WaitUntil(() => subIsFinished == true);
           
					while (audiosourceVoiceOver != null && (audiosourceVoiceOver.isPlaying || ingameGlobalManager.instance.b_Ingame_Pause)) {	
							yield return null;
					}

					if (txtSubTitle) txtSubTitle.text = "";
				}
			}

			if (txtSubTitle) txtSubTitle.text = "";
			if (txtSubTitle)
				txtSubTitle.gameObject.SetActive (false);
			//Debug.Log ("End");
           // Debug.Log("Voice End");
			if(!alreadyInDiary)
				displayNewInfoUI ( ingameGlobalManager.instance.currentLanguage,currentID);
		}
		yield return null;
	}


//--> Display subtitle on screen
	private IEnumerator displaySubTitles ()
	{
		//Debug.Log ("Subtitles Starts");

		float t = 0;
		float target = 0;

		for (var i = 0; i < currentVoiceOverInfo [0].diarySub [currentPage].startPointsClip.Count; i++) {

			target = currentVoiceOverInfo [0].diarySub [currentPage].startPointsClip [i];
			while (t != target) {	
                if (!ingameGlobalManager.instance.b_Ingame_Pause) {
                    if (!txtSubTitle.gameObject.activeInHierarchy)
                        txtSubTitle.gameObject.SetActive(true);

					t = Mathf.MoveTowards (t, target, Time.deltaTime);
				}
                else{
                    if (txtSubTitle.gameObject.activeInHierarchy)
                        txtSubTitle.gameObject.SetActive(false);
                }
				yield return null;
			}

			if (txtSubTitle && ingameGlobalManager.instance.subtitlesState) 
				txtSubTitle.text = currentVoiceOverInfo [0].diarySub [currentPage].textSub [i];
		}

        //if (txtSubTitle) txtSubTitle.text = "";
        subIsFinished = true;
		yield return null;
       // Debug.Log("Subtitles Ends");
	}


//--> Display info UI if the ID need to be added to the inventory or the Diary
	private void displayNewInfoUI(int language,int ID){
		if (currentTextList.r_Available (language, ID)) {
			info.playAnimInfo (
				currentTextList.r_Title (language, ID),
				"Diary");
		}
	}

    public void StopVoiceOver(){
        StopAllCoroutines();
        if(audiosourceVoiceOver)
        audiosourceVoiceOver.Stop();
    }

//--> Pause Voice Over 
	public void _Pause(){
		/*if (!ingameGlobalManager.instance.b_Ingame_Pause) {
			audiosourceVoiceOver.UnPause ();
		} else {
			audiosourceVoiceOver.Pause ();
		}
		*/
	}

    public void deactivateObSubtitle()
    {
        if (txtSubTitle.gameObject.activeInHierarchy)
            b_rememberLastStateSubtitle = true;
        else
            b_rememberLastStateSubtitle = false;

        txtSubTitle.gameObject.SetActive(false);

    }
    public void activateObSubtitle()
    {
        txtSubTitle.gameObject.SetActive(b_rememberLastStateSubtitle);
        b_rememberLastStateSubtitle = false;
    }
}
