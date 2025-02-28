// Description : LoadAndCreateMenu : Use by UI button to call function that allow to save and load player informations.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class LoadAndCreateMenu : MonoBehaviour {
	public List<Text> 			ListOfUIText = new List<Text>();		// List of UI Text to display save Name	
	public Menu_Manager 		menuManager;							// Connect to the gameObject Canvas_MainMenu
	private int 				currentSelectedSlot = 0;				// The current selected save slot			


//--> Load Slot Information (return Date if save slot exist / Empty if svae slot doesn't exist)
	public void LoadSlotInformation(){
		string s_ObjectsDatas = ingameGlobalManager.instance.gameObject.GetComponent<SaveAndLoadManager> ().F_Load_SlotInformation ();



		string[] codes  = s_ObjectsDatas.Split('_');

		if (s_ObjectsDatas == "") {
			for (var i = 0; i < ListOfUIText.Count; i++) {
				ListOfUIText [i].text = ListOfUIText [i].GetComponent<TextProperties> ().returnInfoText ();}
		} else {
			for (var i = 0; i < ListOfUIText.Count; i++) {
				if (codes [i] == "Empty") {
					ListOfUIText [i].text = ListOfUIText [i].GetComponent<TextProperties> ().returnInfoText ();
				} else {
					ListOfUIText [i].text = codes[i];}
			}
		}

	}

//--> Replace the save slot with a new one
	public void EraseAndReplaceSaveSlot(){
		currentSelectedSlot = ingameGlobalManager.instance.currentSaveSlot;
		ingameGlobalManager.instance.gameObject.GetComponent<SaveAndLoadManager> ().EraseAndReplaceSaveSlot (currentSelectedSlot);	
	}

//--> Save Slot Information (return Date if save slot exist / Empty if save slot doesn't exist)
	public void SaveSlotInformation(int slotNumber){
		currentSelectedSlot = slotNumber;
		if (ListOfUIText [slotNumber]) {

			ingameGlobalManager.instance.currentSaveSlot = slotNumber;
			if (ListOfUIText [slotNumber].text != ListOfUIText [slotNumber].GetComponent<TextProperties> ().returnInfoText () && menuManager) {
				CanvasGroup refCanvas = new CanvasGroup ();
				for (var i = 0; i < menuManager.List_GroupCanvas.Count; i++) {
					if (menuManager.List_GroupCanvas [i].name == "SlotAlreadyExist") {
						refCanvas = menuManager.List_GroupCanvas [i];
						break;
					}
				}

				menuManager.GoToOtherPage (refCanvas);
				ingameGlobalManager.instance.navigationList.Add ("SlotAlreadyExist");
			} 
			else {
				ingameGlobalManager.instance.gameObject.GetComponent<SaveAndLoadManager> ().F_Save_SlotInformation (slotNumber, DateTime.UtcNow.ToString (),true);				
				ListOfUIText [slotNumber].text = DateTime.UtcNow.ToString ();
			}
		}
	}
		
//--> Load Game
	public void LoadGame(int slotNumber){
		currentSelectedSlot = slotNumber;
		if (ListOfUIText [slotNumber].text != ListOfUIText [slotNumber].GetComponent<TextProperties> ().returnInfoText ()) {
			//Debug.Log ("Load Game");
			ingameGlobalManager.instance.currentSaveSlot = slotNumber;

			ingameGlobalManager.instance.saveAndLoadManager.LoadGameFromSlot (slotNumber);
		}
	}

}
