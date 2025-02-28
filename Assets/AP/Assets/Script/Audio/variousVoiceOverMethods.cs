// Description : variousVoiceOverMethods : methods to launch voice over
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class variousVoiceOverMethods {

	//--> Case 0 : Play Voice
	//--> Trigger Type : Voice Over
	public void voiceOver(GameObject newObj){
		//if (playOnlyIfNoOtherVoiceOverIsPlayed) {
		if (!ingameGlobalManager.instance.voiceOverManager.audiosourceVoiceOver) {
			ingameGlobalManager.instance.voiceOverManager.audiosourceVoiceOver = ingameGlobalManager.instance.currentPlayer.GetComponent<Character> ().voiceOverAudioSource;

			}


		if (ingameGlobalManager.instance.voiceOverManager && !ingameGlobalManager.instance.voiceOverManager.audiosourceVoiceOver.isPlaying){
			newVoiceOver (newObj);					
		} else {
			if (ingameGlobalManager.instance.voiceOverManager)
				newVoiceOver (newObj);
		}
	}


	//--> Launch a new Voice Over
	public void newVoiceOver(GameObject newObj){
		VoiceProperties voiceProperties = newObj.GetComponent<VoiceProperties> ();

		//-> Add the entry in the diary if needed 
		int managerID = voiceProperties.managerID;
		bool alreadyInDiary = false;
		if( voiceProperties.textList.r_Available(0,managerID)){
			
			for (var i = 0; i < ingameGlobalManager.instance.currentPlayerDiaryList.Count; i++) {
				if (voiceProperties.managerID == ingameGlobalManager.instance.currentPlayerDiaryList [i]) {
					alreadyInDiary = true;
					break;
				}
			}
			if(!alreadyInDiary)
				ingameGlobalManager.instance.currentPlayerDiaryList.Add (managerID);
		}

		if (voiceProperties) {
			if (ingameGlobalManager.instance.voiceOverManager) {
				if (!voiceProperties.b_alreadyPlayed && voiceProperties.b_PlayOnce
					|| !voiceProperties.b_PlayOnce) {
					voiceProperties.b_alreadyPlayed = true;


					ingameGlobalManager.instance.voiceOverManager.setupNewVoice (
						voiceProperties.r_TextList (), 
						ingameGlobalManager.instance.currentLanguage,
						voiceProperties.managerID,
						voiceProperties.r_TextList ().voiceOverDescription (ingameGlobalManager.instance.currentLanguage, voiceProperties.managerID),
						voiceProperties.r_TextList ().r_audioPriority (ingameGlobalManager.instance.currentLanguage, voiceProperties.managerID),
						alreadyInDiary);
				}
			}
		}
	}


}
