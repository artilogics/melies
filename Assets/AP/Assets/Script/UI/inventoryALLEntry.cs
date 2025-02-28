// Description : invenory all entry : Manage In Game inventory UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventoryALLEntry : MonoBehaviour {
	public bool 			SeeInspector = false;

	//--> Section UI : inventory all entry
	public List<Image> listEntry = new List<Image>();
	public List<Text> 		listText = new List<Text>();
	//public GameObject 		Journal_UI_Entry;
	//public Transform 		inventoryContainer;
	private TextList 		inventory;
	public GameObject		lastPage;
	public GameObject		nextPage;
	private int 			currentPage = 0;
	private int 			howManyENtryByPage = 9;

    public List<int> entryCurrentlyVisibleInInventory = new List<int>();        // Create a list of object that are visible in the Inventory. OBject already used are not visible.


	//--> Section UI : inventory Reader Section
	public Text 			inventoryTitleTxt;
	public Text 			inventoryDescriptionTxt;
	private int 			currentEntryDisplayedOnScreen = 0;


	public RectTransform 	borderObjectSelected;

	public GameObject 		showSelectedObject;
	public investigationCam _investigationCam;

	public int 				inventoryButtonDesktop 	= 4;
	public int 				inventoryButtonJoystick = 0;

	public CanvasGroup canvasInventory;
	public CanvasGroup canvasGame;

	public GameObject blackScreen;

	// Use this for initialization
	void Start(){
		//Init ();
		StartCoroutine (Init());
	}

	void Update(){
        //--> Manage Inputs
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            manageDesktopInputs();
        }
	}

	//--> Manage Inputs
	public void manageDesktopInputs(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		if (Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[inventoryButtonDesktop]) && !gManager.b_Joystick  			// Keyboard Inventory Button : Default I 
			|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringGamepadButton[inventoryButtonJoystick]) && gManager.b_Joystick) {		// Joystick Inventory Button
		
		//-> Main Menu is activated
			if (gManager.navigationList.Count > 0 && gManager.navigationList [0] == "MainMenuManager") {
				// anything to do
			}
		//-> Hide 3D Viewer
			else if (gManager.navigationList.Count > 0
				&& gManager.navigationList [gManager.navigationList.Count - 1] == "InventoryViewer3D"
                     && (!Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[inventoryButtonJoystick]) && gManager.b_Joystick
                    ||
                         !Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[inventoryButtonDesktop]) && !gManager.b_Joystick)) {

				Hide3DViewer ();
			}
		//-> Show Diary
			else if (gManager.navigationList.Count == 0
				|| gManager.navigationList [gManager.navigationList.Count - 1] != "Inventory" 
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "Viewer3D"
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "MultiPages"
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "DiaryMultiPages"
                     && gManager.navigationList[gManager.navigationList.Count - 1] != "InventoryViewer3D"
                     && gManager.navigationList[gManager.navigationList.Count - 1] != "Clue") {	

                if(gManager.navigationList.Count > 0 && gManager.navigationList[gManager.navigationList.Count - 1] == "Diary"
                  /*||
                   gManager.navigationList.Count > 0 && gManager.navigationList[gManager.navigationList.Count - 1] == "InventoryViewer3D"*/)
				    showInventory (false);
                else
                    showInventory(true);  

			}
		//-> Hide Diary (go back to game)
			else if (gManager.navigationList.Count > 0 
				&& gManager.navigationList [gManager.navigationList.Count - 1] == "Inventory") {	

				HideInventory ();
			}
		}
	}

	//--> Hide Viewer 3D
	public void Hide3DViewer(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);
		if(_investigationCam.gameObject)_investigationCam.clearInvestigateView();
		if(blackScreen)blackScreen.SetActive (false);

		Cursor.visible = true;
		ingameGlobalManager.instance.b_currentCursorVisibility = Cursor.visible;
		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);


	}

	//--> Show Iventory
    public void showInventory(bool b_saveObjPuzzleNotAvailableState){
		StartCoroutine (Init());

		DisableAllPuzzleRefPosHand();

		if (ingameGlobalManager.instance.canvasMobileInputs
			&& ingameGlobalManager.instance.canvasMobileInputs.activeSelf) 
				ingameGlobalManager.instance.canvasMobileInputs.SetActive (false);
        
		ingameGlobalManager gManager = ingameGlobalManager.instance;

        gManager.onlyPauseGame();

        if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Diary")	
			gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		gManager.canvasMainMenu.GoToOtherPage (canvasInventory);
		gManager.navigationList.Add ("Inventory");

		gManager.StartCoroutine (gManager.changeLockStateConfined (true));
		gManager.b_AllowCharacterMovment = false;
        if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
			gManager.reticule.SetActive (false);

        ingameGlobalManager.instance._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);

        if (gManager.reticuleJoystickImage && gManager.b_Joystick)
			gManager.reticuleJoystickImage.gameObject.SetActive (true);

        if(b_saveObjPuzzleNotAvailableState){
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjTitle();
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjPuzzleNotAvailable();
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjResetPuzzle();
            ingameGlobalManager.instance.voiceOverManager.deactivateObSubtitle();
        }

    

		gManager.canvasPlayerInfos.deactivateIcons(false);

        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
		gManager.audioMenuClips.playASound (5);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)



        //if(ingameGlobalManager.instance.canvasPlayerInfos && !ingameGlobalManager.instance.b_Joystick)
          //  ingameGlobalManager.instance.canvasPlayerInfos.gameObject.SetActive(false);

       
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
	}

	//--> Hide Inventory (go back to game)
	public void HideInventory(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		EnableAllPuzzleRefPosHand();


		gManager.onlyUnPauseGame();

        if (ingameGlobalManager.instance.canvasPlayerInfos)
            ingameGlobalManager.instance.canvasPlayerInfos.gameObject.SetActive(true);

		if (ingameGlobalManager.instance.canvasMobileInputs
			&& !ingameGlobalManager.instance.canvasMobileInputs.activeSelf
			&& !ingameGlobalManager.instance.b_DesktopInputs) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);

		gManager.canvasMainMenu.GoToOtherPage (canvasGame);
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Focus") {
			//Cursor.visible = true;
			gManager.b_currentCursorVisibility = Cursor.visible;

            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs) {
				gManager.StartCoroutine (gManager.changeLockStateConfined (true));
				gManager.b_AllowCharacterMovment = false;
				gManager.reticule.SetActive (false);
			}


            if (gManager.reticuleJoystickImage && 
                ingameGlobalManager.instance.currentPuzzle &&
                !ingameGlobalManager.instance.currentPuzzle.GetComponent<focusOnly>() &&
                ingameGlobalManager.instance.b_Joystick) {

                if (!ingameGlobalManager.instance.canvasPlayerInfos.b_rememberLastState)
                    gManager.reticuleJoystickImage.gameObject.SetActive(true);
                else
                    gManager.reticuleJoystickImage.gameObject.SetActive(false);
            }
            //-> Focus in a wardrobe or drawer
            else if(ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                    ingameGlobalManager.instance.b_DesktopInputs &&
                    ingameGlobalManager.instance.b_Joystick){
                gManager.reticuleJoystickImage.gameObject.SetActive(true);
            }
            else{
                //Debug.Log("Here 2");
                gManager.reticuleJoystickImage.gameObject.SetActive(false);
            }

			//--> Display available actions on screen
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
		} else {
			Cursor.visible = true;
			gManager.b_currentCursorVisibility = Cursor.visible;
			gManager.StartCoroutine (gManager.changeLockStateLock ());
			gManager.b_AllowCharacterMovment = true;
            if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (true);

			if (gManager.reticuleJoystickImage && 
                gManager.reticuleJoystickImage.gameObject.activeSelf) {
				gManager.reticuleJoystickImage.gameObject.SetActive (false);
            }

			//--> Display available actions on screen
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
		}

        if(!gManager.canvasPlayerInfos.gameObject.activeSelf)
            gManager.canvasPlayerInfos.gameObject.SetActive(true);
		
        gManager.canvasPlayerInfos.activateIcons ();
		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)

        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);


        ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();


        for (var i = 0; i < listEntry.Count; i++)
            listEntry[i].gameObject.SetActive(false);

        clearInventoryVisualization();
	}




	//--> Wait until the ingameGlbalManager finish his initialization
	IEnumerator Init(){
		yield return new WaitUntil (ingameGlobalManager.instance.r_InitBool);
		inventory = ingameGlobalManager.instance.currentInventory;
		GenerateListOfinventoryEntry (currentPage);

		if(borderObjectSelected)
			borderObjectSelected.gameObject.SetActive(false);
	}



// --> Display Title in inventory 
	void GenerateListOfinventoryEntry (int Page) {
		clearInventoryVisualization ();
        entryCurrentlyVisibleInInventory.Clear();
		int firstEntry = 0 + howManyENtryByPage * Page;
		int lastEntry = howManyENtryByPage + howManyENtryByPage * Page;


        //Create the list of Object visible in the Inventory
        for (var i = 0; i < ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList.Count; i++)
        {
            if(ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList[i] == true){
                entryCurrentlyVisibleInInventory.Add(ingameGlobalManager.instance.currentPlayerInventoryList[i]);
            }
        }

        // Check if next page entry and last page entry buttons need to be activated
        if (entryCurrentlyVisibleInInventory.Count > lastEntry) {
			nextPage.SetActive (true);} 
		else {
			nextPage.SetActive (false);}

		if (Page != 0) {
			lastPage.SetActive (true);} 
		else {
			lastPage.SetActive (false);}


		//-> Display Title	
		for (var i =firstEntry ; i < lastEntry; i++) {
            if (entryCurrentlyVisibleInInventory.Count > i) {
                if (inventory.diaryList [0]._languageSlot [entryCurrentlyVisibleInInventory [i]].diarySprite [0]) {
                    Sprite newSprite = inventory.diaryList [0]._languageSlot [entryCurrentlyVisibleInInventory [i]].diarySprite [0];

                    Vector2 newSize = new Vector2(1,1);

                    if(newSprite.rect.width >= newSprite.rect.height){
                        if(newSprite.rect.width > 210){
                            float multiplier = Mathf.Abs((newSprite.rect.width / 210)-2);
                            newSize = new Vector2(newSprite.rect.width * multiplier, newSprite.rect.height * multiplier);
                        }
                        else{
                            newSize = new Vector2(newSprite.rect.width, newSprite.rect.height);
                        }
                    }
                    else{
                        if (newSprite.rect.height > 210)
                        {
                            float multiplier = Mathf.Abs( (newSprite.rect.height / 210)-2);
                            //Debug.Log("multiplier : " + multiplier);
                            newSize = new Vector2(newSprite.rect.width * multiplier, newSprite.rect.height * multiplier);
                        } 
                        else
                        {
                            newSize = new Vector2(newSprite.rect.width, newSprite.rect.height);
                        }
                    }

                    listEntry[i % howManyENtryByPage].GetComponent<RectTransform>().sizeDelta = newSize;
					listEntry [i % howManyENtryByPage].sprite = newSprite;
				} else {
					listEntry [i % howManyENtryByPage].sprite = null;
				}
			

				listEntry [i%howManyENtryByPage].gameObject.SetActive (true);
			} else {
				listEntry [i%howManyENtryByPage].gameObject.SetActive (false);
			}

		}
	}

//--> Call last 14 entry
	public void F_lastPage(){
		currentPage--;
		//clearInventoryVisualization ();
		GenerateListOfinventoryEntry (currentPage);
	}
//--> Call next 14 entry
	public void F_nextPage(){
		currentPage++;
		//clearInventoryVisualization ();
		GenerateListOfinventoryEntry (currentPage);
	}

//--> Display page for a selected entry
	public void showinventoryEntry(int value){
		currentEntryDisplayedOnScreen = value + howManyENtryByPage * currentPage;
        currentEntryDisplayedOnScreen = entryCurrentlyVisibleInInventory[currentEntryDisplayedOnScreen];
		if (!borderObjectSelected.gameObject.activeSelf || borderObjectSelected.position != listEntry [value].gameObject.GetComponent<RectTransform> ().position) {
			//Debug.Log ("Show 2D : " + ingameGlobalManager.instance.currentPlayerInventoryList [currentEntryDisplayedOnScreen]);

			if (inventoryTitleTxt) {
				//Debug.Log ("here inventory : " + currentEntryDisplayedOnScreen);
				inventoryTitleTxt.text = inventory.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryTitle [0];
			}

			if (inventoryDescriptionTxt) {
				inventoryDescriptionTxt.text = inventory.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText [0];
			}


			if (borderObjectSelected.gameObject != null) {
				if (borderObjectSelected && !borderObjectSelected.gameObject.activeSelf)
					borderObjectSelected.gameObject.SetActive (true);
				borderObjectSelected.position = listEntry [value].gameObject.GetComponent<RectTransform> ().position;
			}

			if (showSelectedObject
			    && inventory.diaryList [0]._languageSlot [currentEntryDisplayedOnScreen].refGameObject != null) {
				showSelectedObject.gameObject.SetActive (true);
			} else {
				showSelectedObject.gameObject.SetActive (false);
			}

		} else {

			if (ingameGlobalManager.instance.b_Joystick) {
				if (ingameGlobalManager.instance.reticuleJoystickImage && ingameGlobalManager.instance.reticuleJoystickImage.gameObject.activeSelf) {
					ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive (false);
				}
			}

            if(ingameGlobalManager.instance.b_DesktopInputs){
              
                if (blackScreen) blackScreen.SetActive(true);
                showItem();  
            }
			
		}

	}


	public void clearInventoryVisualization(){
		if (showSelectedObject) {
			showSelectedObject.gameObject.SetActive(false);}

		if(borderObjectSelected)
			borderObjectSelected.gameObject.SetActive(false);

		if (inventoryTitleTxt) {
			inventoryTitleTxt.text = "";}

		if (inventoryDescriptionTxt) {
			inventoryDescriptionTxt.text = "";}
	}



//--> Init all entry 
	public void InitinventoryAllEntry(){
		GenerateListOfinventoryEntry (currentPage);
	}

	//public List<GameObject> objInventaryList = new List<GameObject>();
//--> Show item : 3D turntable
	public void showItem(){
		if (_investigationCam) {

			//foreach (GameObject obj in objInventaryList)
			//	obj.SetActive(false);
			canvasInventory.gameObject.SetActive(false);

            foreach (Image objs in listEntry)
            {
                objs.gameObject.transform.parent.gameObject.SetActive(false);
            }
			
			if (!_investigationCam.gameObject.activeSelf) {
				_investigationCam.gameObject.SetActive (true);
			}

			//Debug.Log ("Show 3D : " + currentEntryDisplayedOnScreen);
			if (inventory.diaryList [0]._languageSlot [currentEntryDisplayedOnScreen].refGameObject != null) {
				//Debug.Log ("Ok 3D");

				_investigationCam.gameObject.SetActive (true);

				_investigationCam.newObjectToInvestigate (
					inventory.diaryList [0]._languageSlot [currentEntryDisplayedOnScreen].refGameObject,
					inventory.diaryList [0]._languageSlot [currentEntryDisplayedOnScreen].prefabSizeInViewer,
					inventory.diaryList [0]._languageSlot [currentEntryDisplayedOnScreen].prefabRotationInViewer,
                    false,
                    false
				);

				ingameGlobalManager.instance.navigationList.Add ("InventoryViewer3D");


				//--> Display available actions on screen
				//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (true, true);

                ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(true, true);

				Cursor.visible = false;
				ingameGlobalManager.instance.b_currentCursorVisibility = Cursor.visible;

				if (ingameGlobalManager.instance.b_Joystick)
				{
					if (ingameGlobalManager.instance.reticuleJoystickImage && ingameGlobalManager.instance.reticuleJoystickImage.gameObject.activeSelf)
					{
						ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive(false);
					}
				}

				ingameGlobalManager.instance.audioMenuClips.playASound (2);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
				/*if (ingameGlobalManager.instance.canvasPlayerInfos.txt3DViewer) {
					ingameGlobalManager.instance.canvasPlayerInfos.txt3DViewer.text = ingameGlobalManager.instance.currentInventory.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryTitle [0];
				}*/
			} 
		}
	}

	public void hideViewer(){
		if (_investigationCam) {
				_investigationCam.clearInvestigateView ();
			//if (ingameGlobalManager.instance.canvasPlayerInfos.txt3DViewer) {
			//	ingameGlobalManager.instance.canvasPlayerInfos.txt3DViewer.text = "";}
			
		}
		
	}

	void DisableAllPuzzleRefPosHand()
    {
		initRefPositionSprite[] initRefPositionSpriteList = FindObjectsByType<initRefPositionSprite>(FindObjectsSortMode.None);

		foreach (var child in initRefPositionSpriteList)
			child.GetComponent<SpriteRenderer>().enabled = false;

		//Debug.Log("Disable Sprite");
	}

	public void EnableAllPuzzleRefPosHand()
	{
		initRefPositionSprite[] initRefPositionSpriteList = FindObjectsByType<initRefPositionSprite>(FindObjectsSortMode.None);

		foreach (var child in initRefPositionSpriteList)
			child.GetComponent<SpriteRenderer>().enabled = true;

		//Debug.Log("Enable Sprite");
	}

}
