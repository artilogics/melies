// Decription : MainMenu : methods call in the Main page of the Menu.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public int 				currentButtonSelected = 0;
	public GameObject 		btnSave;
	public Text 			txtValidation;
	public TextProperties	txtProp;
	public GameObject 		btnResumeMainMenu;

	public void F_QuitGame () {
		Application.Quit ();
	}

	public void F_currentButtonSelected(int value){
		currentButtonSelected = value;
	}

	public void F_CallFunctionUsingCurrentButtonSelectedValue(){
		if (currentButtonSelected == 0){
			F_MN_SaveGame ();
		}
		if (currentButtonSelected == 1)
			F_MN_LoadGame ();
		if (currentButtonSelected == 2) {

			F_MN_GoToAnotherLevel (0);

		}
	}

	public void F_MN_GoToAnotherLevel (int BuildInSceneIndex){
		Destroy (ingameGlobalManager.instance.currentPlayer);
		ingameGlobalManager.instance.b_AllowCharacterMovment = true;
		ingameGlobalManager.instance.GetComponent<SaveAndLoadManager> ().F_Load_MainMenu_Scene (BuildInSceneIndex);
	}

	//--> Resume Game
	public void F_MN_ResumeGame(){
		ingameGlobalManager.instance.GetComponent<backInputs> ().HideMainMenu ();
	}



	//--> Load Game
	public void F_MN_LoadGame(){
			ingameGlobalManager.instance.saveAndLoadManager.LoadGameFromSlot (ingameGlobalManager.instance.currentSaveSlot);
	}

	//--> Save Game
	public void F_MN_SaveGame(){
		StartCoroutine (I_MN_SaveGame ());
	}

	IEnumerator I_MN_SaveGame(){
		ingameGlobalManager.instance.saveAndLoadManager.F_SaveProcess ();
		yield return new WaitUntil(() => ingameGlobalManager.instance.saveAndLoadManager.b_saveProcessFinished == true);

		//--> Deactivate Loading black screen
		CanvasGroup refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas [i].name == "MainMenu") {
				refCanvas = ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasMainMenu.GoToOtherPage (refCanvas);

		EventSystem eSys = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();

		if (eSys) {
			eSys.SetSelectedGameObject (btnSave);
		}

		if (txtValidation)
			txtValidation.text = txtProp.returnInfoText ();
	}


	public void addToNavigationList(string navName){
		if (navName == "RemapInput" && 
			ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "RemapInput") {

		} else {
			ingameGlobalManager.instance.navigationList.Add (navName);
		}
	}

	public void removeToNavigationList(){
		ingameGlobalManager.instance.navigationList.RemoveAt (ingameGlobalManager.instance.navigationList.Count-1);
		ingameGlobalManager.instance.navigationButtonList.RemoveAt (ingameGlobalManager.instance.navigationButtonList.Count-1);
	}

	public void addToNavigationButtonList(GameObject newObj){
		ingameGlobalManager.instance.navigationButtonList.Add (newObj);
	}

	public void removeToNavigationButtonList(){
		ingameGlobalManager.instance.navigationList.RemoveAt (ingameGlobalManager.instance.navigationButtonList.Count-1);
	}


	public void playSound(int soundToPlay){
		ingameGlobalManager.instance.audioMenuClips.playASound (soundToPlay);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
	}


	public void F_pointerEnterSetSelected(GameObject objRef){
		EventSystem eSys = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		if (eSys && objRef != eSys.currentSelectedGameObject) {
			eSys.SetSelectedGameObject(objRef);
			ingameGlobalManager.instance.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
		}
	}


	public void newButtonSelected(GameObject newObj){
		ingameGlobalManager.instance.lastUIButtonSelected = newObj;
	}

    public void initPlayerPrefs(GameObject newObj)
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

}
