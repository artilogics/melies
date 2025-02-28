// Description : OptionMenuManager : Use in Various Menu to access different gameoject in the Hierarchy during game 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionMenuManager : MonoBehaviour {
	public string s_canvasMainMenu 		= "Canvas_MainMenu";
	public string s_eventSystem 		= "EventSystem";
	public string s_mainMenuManager 	= "MainMenuManager";
	public string s_InputManager 		= "InputsManager";
	public string s_loadMenuManager		= "loadMenuManager";
	public string s_newGameMenuManager	= "newGameMenuManager";




	private Menu_Manager 		menu_Manager;
	private EventSystem 		eventSys;
	private MainMenu 			mainMenuManager;
	private MM_MenuInputs 		inputsManager;
	private LoadAndCreateMenu	loadMenuManager;
	private LoadAndCreateMenu	newGameMenuManager;

    private canvasVariousFunctions canvasVarious;



	// Use this for initialization
	void Start () {
		canvasVarious = new canvasVariousFunctions ();
		GameObject tmp = GameObject.Find (s_canvasMainMenu);
		if (tmp)menu_Manager = tmp.GetComponent<Menu_Manager> ();

		tmp = GameObject.Find (s_eventSystem);
		if (tmp)eventSys = tmp.GetComponent<EventSystem> ();

		tmp = GameObject.Find (s_mainMenuManager);
		if (tmp)mainMenuManager = tmp.GetComponent<MainMenu> ();

		tmp = GameObject.Find (s_InputManager);
		if (tmp)inputsManager = tmp.GetComponent<MM_MenuInputs> ();

		tmp = GameObject.Find (s_loadMenuManager);
		if (tmp)loadMenuManager = tmp.GetComponent<LoadAndCreateMenu> ();

		tmp = GameObject.Find (s_newGameMenuManager);
		if (tmp)newGameMenuManager = tmp.GetComponent<LoadAndCreateMenu> ();


	}

	public void OMM_pointerEnterSetSelected(GameObject newObj){
		canvasVarious.MM_pointerEnterSetSelected (mainMenuManager, s_mainMenuManager, newObj);
	}

	public void OMM_playSound(int soundToPlay){
		canvasVarious.MM_playSound(soundToPlay);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
	}
	
	public void OMM_newButtonSelected(GameObject newObj){
		ingameGlobalManager.instance.lastUIButtonSelected = newObj;
	}

	public void OMM_addToNavigationList(string navName){
		canvasVarious.MM_addToNavigationList (navName);
	}

	public void OMM_removeToNavigationList(){
		canvasVarious.MM_removeToNavigationList ();
	}

	public void OMM_addToNavigationButtonList(GameObject newObj){
		canvasVarious.MM_addToNavigationButtonList(newObj);
	}

	public void OMM_removeToNavigationButtonList(){
		canvasVarious.MM_removeToNavigationButtonList ();
	}

	public void OMM_GoToOtherPage(CanvasGroup newCanvas){
		if(menu_Manager != null)
			canvasVarious.MM_GoToOtherPage (menu_Manager,newCanvas);
	}

	public void OMM_SetSelectedGameObject(GameObject newSelectedObj){
		if(eventSys != null)
			canvasVarious.MM_SetSelectedGameObject (eventSys,newSelectedObj);
	}

	public void OMM_SetSelectedGameObjectToNull(){
		if(eventSys != null)
			canvasVarious.MM_SetSelectedGameObject (eventSys,null);
	}

	public void OMM_UpdateInputManager(){
		if(inputsManager != null)
			canvasVarious.MM_UpdateInputManager(inputsManager);
	}


	public void OMM_EraseAndReplaceSaveSlot(){
		if (loadMenuManager != null)
			canvasVarious.MM_EraseAndReplaceSaveSlot (loadMenuManager);
	}


	public void OMM_loadSlotInformationFromNewGameMenu(){
        if (newGameMenuManager != null){

            canvasVarious.MM_loadSlotInformation(newGameMenuManager);
        }
			
	}

	public void OMM_loadSlotInformationFromLoadGameMenu(){
		if (loadMenuManager != null)
			canvasVarious.MM_loadSlotInformation (loadMenuManager);
	}



}
