//Description : ingameMultiPageText : Display the diary reader ingame. This text viewer is different from the diary viewer
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ingameMultiPageText : MonoBehaviour {
	//--> Section UI : Diary Reader Section
	public Text 			diaryReaderTxt;
	public GameObject		lastPageReader;
	public GameObject		nextPageReader;
	private int 			currentEntryDisplayedOnScreen = 0;
	private int 			currentPageReader = 0;

	private TextList 		_currentTextList;


	//--> Call last Page in the diary Reader
	public void F_lastPageReader(){
		currentPageReader--;
		ChangePageInDiaryReader ();
	}
	//--> Call next page in the diary Reader
	public void F_nextPageReader(){
		currentPageReader++;
		ChangePageInDiaryReader ();
	}

	//--> Display page for a selected entry
	public void showDiaryEntry(int value,TextList currentTextList ){
		//Debug.Log (currentTextList.name + " : " + value);
		_currentTextList = currentTextList;

        ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.deactivateObSubtitle();

		if (diaryReaderTxt) {
			currentEntryDisplayedOnScreen = value;

			int offset = 0;																// diary case
			if (_currentTextList == ingameGlobalManager.instance.currentInventory)		// inventory case
				offset = 1;

			if(currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText.Count > 1+offset)
				nextPageReader.SetActive (true); 
			else 
				nextPageReader.SetActive (false);

			lastPageReader.SetActive (false);

			//Debug.Log (currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText.Count);
			if (currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText.Count > 1// Inventory Case
			    && currentTextList == ingameGlobalManager.instance.currentInventory) {
				currentPageReader = 1;
				diaryReaderTxt.text = currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText [currentPageReader];
			} else if (currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText.Count > 0// Diary Case
			         && currentTextList == ingameGlobalManager.instance.currentDiary) {
				currentPageReader = 0;
				diaryReaderTxt.text = currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [value].diaryText [currentPageReader];
			}
			else {
				diaryReaderTxt.text = "PAGE NUMBER = 0. You need to create at least one page for this entry.";
				Debug.Log ("INFO : Item has no page");
			}
		}
	}


	//--> Change page in the diary Reader
	public void ChangePageInDiaryReader(){
		int offset = 0;																// diary case
		if (_currentTextList == ingameGlobalManager.instance.currentInventory)		// inventory case
			offset = 1;

		//Debug.Log (offset +  " : " + _currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText.Count);
		if(_currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText.Count > currentPageReader+1){
			nextPageReader.SetActive (true);} 
		else {
			nextPageReader.SetActive (false);}

		if (currentPageReader > offset) {
			lastPageReader.SetActive (true);} 
		else {
			lastPageReader.SetActive (false);}

		diaryReaderTxt.text = _currentTextList.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText [currentPageReader];
	}


}
