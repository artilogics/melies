// Description : canvasVariousFunctions : Various mehods use in canvas
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class canvasVariousFunctions {

	public void MM_pointerEnterSetSelected(MainMenu mainMenuManager, string s_ToFind, GameObject newObj){
		if (mainMenuManager == null) {
			GameObject tmp = GameObject.Find (s_ToFind);
			if (tmp)mainMenuManager = tmp.GetComponent<MainMenu> ();}

		if (mainMenuManager)
			mainMenuManager.F_pointerEnterSetSelected (newObj);
	}

	public void MM_playSound(int soundToPlay){
		ingameGlobalManager.instance.audioMenuClips.playASound (soundToPlay);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
	}

	public void MM_newButtonSelected(GameObject newObj){
		ingameGlobalManager.instance.lastUIButtonSelected = newObj;
	}

	public void MM_addToNavigationList(string navName){
		if (navName == "RemapInput" && 
			ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "RemapInput") {

		} else {
			ingameGlobalManager.instance.navigationList.Add (navName);
		}
	}

	public void MM_removeToNavigationList(){
		ingameGlobalManager.instance.navigationList.RemoveAt (ingameGlobalManager.instance.navigationList.Count-1);
		ingameGlobalManager.instance.navigationButtonList.RemoveAt (ingameGlobalManager.instance.navigationButtonList.Count-1);
	}

	public void MM_addToNavigationButtonList(GameObject newObj){
		ingameGlobalManager.instance.navigationButtonList.Add (newObj);
	}

	public void MM_removeToNavigationButtonList(){
		ingameGlobalManager.instance.navigationList.RemoveAt (ingameGlobalManager.instance.navigationButtonList.Count-1);
	}

	public void MM_GoToOtherPage(Menu_Manager canvasMainMenu, CanvasGroup newCanvas){
		canvasMainMenu.GoToOtherPage (newCanvas);
	}


	public void MM_SetSelectedGameObject(EventSystem eventSys,GameObject newSelectedObj){
		eventSys.SetSelectedGameObject (newSelectedObj);
	}

	public void MM_UpdateInputManager(MM_MenuInputs inputManager){
		inputManager.updateInputPage ();
	}

	public void MM_EraseAndReplaceSaveSlot(LoadAndCreateMenu loadMenuManager){
		loadMenuManager.EraseAndReplaceSaveSlot ();
	}

	public void MM_loadSlotInformation(LoadAndCreateMenu newMenu){
        GameObject tmp = GameObject.Find("txtDebug");
        if (tmp && newMenu)
            tmp.GetComponent<Text>().text = "newMenu";
		newMenu.LoadSlotInformation ();
	}


}
