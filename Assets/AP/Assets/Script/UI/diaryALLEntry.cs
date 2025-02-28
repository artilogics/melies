// Description : Diary all entry : Manage In Game Diary UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class diaryALLEntry : MonoBehaviour {
	public bool 			SeeInspector = false;
	//--> Section UI : Diary all entry
	public List<GameObject> listEntry = new List<GameObject>();
	public List<Text> 		listText = new List<Text>();
	public GameObject 		Journal_UI_Entry;
	public Transform 		diaryContainer;
	private TextList 		diary;
	public GameObject		lastPage;
	public GameObject		nextPage;
	private int 			currentPage = 0;
	private int 			howManyENtryByPage = 6;

	//--> Section UI : Diary Reader Section
	public Text 			diaryReaderTxt;
	public GameObject		lastPageReader;
	public GameObject		nextPageReader;
	private int 			currentEntryDisplayedOnScreen = 0;
	private int 			currentPageReader = 0;

	//public string 			inputDesktop = "j";
	//public KeyCode 			inputJoystick = KeyCode.JoystickButton19;
	public CanvasGroup 		canvasDiary;
	public CanvasGroup 		canvasGame;
	//public string inputJoystick = "joystick button 1";

	public int 				diaryButtonDesktop 	= 5;
	public int 				diaryButtonJoystick = 1;


	// Use this for initialization
	void Start(){
        howManyENtryByPage = listEntry.Count;
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
		if (Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[diaryButtonDesktop]) && !gManager.b_Joystick  			// Keyboard Diary Button : Default J
			|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringGamepadButton[diaryButtonJoystick]) && gManager.b_Joystick) {		// Joystick Diary Button

			if (gManager.navigationList.Count > 0 && gManager.navigationList [0] == "MainMenuManager") {
				// anything to do
			}
			else if (gManager.navigationList.Count > 0
				&& gManager.navigationList [gManager.navigationList.Count - 1] == "DiaryMultiPages") {

				showDiaryMultiPages ();
			}
		//-> Show Diary
			else if (gManager.navigationList.Count == 0
				|| gManager.navigationList [gManager.navigationList.Count - 1] != "Diary" 
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "Viewer3D"
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "MultiPages"
				&& gManager.navigationList [gManager.navigationList.Count - 1] != "InventoryViewer3D"
				&& gManager.navigationList[gManager.navigationList.Count - 1] != "Clue") {	
               // gManager.canvasPlayerInfos.displayAvailableActionOnScreen(false, true);
                ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);

                if(gManager.navigationList.Count > 0 && gManager.navigationList[gManager.navigationList.Count - 1] == "Inventory")
				    showDiary (false);
                else
                    showDiary(true); 
			}
		//-> Hide Diary (go back to game)
			else if (gManager.navigationList.Count > 0 
				&& gManager.navigationList [gManager.navigationList.Count - 1] == "Diary") {	
			
				HideDiary ();
			}
		}
	}

//--> Show selected Multi page
	public void showDiaryMultiPages(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);
		gManager.canvasMainMenu.GoToOtherPage (canvasDiary);

		//--> Display available actions on screen
		//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false, true);
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
	}

//--> Show Diary
    public void showDiary(bool b_saveObjPuzzleNotAvailableState){
		StartCoroutine (Init());

		DisableAllPuzzleRefPosHand();

		if (ingameGlobalManager.instance.canvasMobileInputs
			&& ingameGlobalManager.instance.canvasMobileInputs.activeSelf) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (false);

		ingameGlobalManager gManager = ingameGlobalManager.instance;
		

        gManager.onlyPauseGame();

        if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Inventory")
			gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		gManager.canvasMainMenu.GoToOtherPage (canvasDiary);
		gManager.navigationList.Add ("Diary");
		gManager.StartCoroutine (gManager.changeLockStateConfined (true));
		gManager.b_AllowCharacterMovment = false;
        if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
			gManager.reticule.SetActive (false);


        ingameGlobalManager.instance._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);
        if (gManager.reticuleJoystickImage && gManager.b_Joystick)
            gManager.reticuleJoystickImage.gameObject.SetActive(true);

        if (b_saveObjPuzzleNotAvailableState)
        {
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjTitle();
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjPuzzleNotAvailable();
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjResetPuzzle();
            ingameGlobalManager.instance.voiceOverManager.deactivateObSubtitle();
        }


		gManager.canvasPlayerInfos.deactivateIcons(false);

		//--> Display available actions on screen
		//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false, false);
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
		ingameGlobalManager.instance.audioMenuClips.playASound (4);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)


        if (ingameGlobalManager.instance.canvasPlayerInfos && !ingameGlobalManager.instance.b_Joystick)
            ingameGlobalManager.instance.canvasPlayerInfos.gameObject.SetActive(false);


       
	}

//--> Hide Diary (go back to game)
	public void HideDiary(){
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
            //Debug.Log("Here");
			//Cursor.visible = true;
			gManager.b_currentCursorVisibility = Cursor.visible;

            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
            {
                gManager.StartCoroutine(gManager.changeLockStateConfined(true));
                gManager.b_AllowCharacterMovment = false;
                gManager.reticule.SetActive(false);
            }


            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (false);

            if (gManager.reticuleJoystickImage &&
                ingameGlobalManager.instance.currentPuzzle &&
                !ingameGlobalManager.instance.currentPuzzle.GetComponent<focusOnly>() &&
                ingameGlobalManager.instance.b_Joystick) 
            {
                if(!ingameGlobalManager.instance.canvasPlayerInfos.b_rememberLastState)
                    gManager.reticuleJoystickImage.gameObject.SetActive(true);
                else
                    gManager.reticuleJoystickImage.gameObject.SetActive(false);
            }
            //-> Focus in a wardrobe or drawer
            else if (ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                    ingameGlobalManager.instance.b_DesktopInputs &&
                    ingameGlobalManager.instance.b_Joystick)
            {
                gManager._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);
                gManager.reticuleJoystickImage.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log("Here 2");
                gManager.reticuleJoystickImage.gameObject.SetActive(false);
            }

			//--> Display available actions on screen
			//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false, true);
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
		} else {
			Cursor.visible = true;
			gManager.b_currentCursorVisibility = Cursor.visible;
			gManager.StartCoroutine (gManager.changeLockStateLock ());
			gManager.b_AllowCharacterMovment = true;
            if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (true);

			if (gManager.reticuleJoystickImage && gManager.reticuleJoystickImage.gameObject.activeSelf) {
				gManager.reticuleJoystickImage.gameObject.SetActive (true);}

			//--> Display available actions on screen
			//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false, false);
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
		}

        if (!gManager.canvasPlayerInfos.gameObject.activeSelf)
            gManager.canvasPlayerInfos.gameObject.SetActive(true);
		
        gManager.canvasPlayerInfos.activateIcons ();

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)

        //gManager.canvasPlayerInfos.displayAvailableActionOnScreen(false, false);
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
		if (nextPageReader) 
			nextPageReader.SetActive(false);
		if (lastPageReader) 
			lastPageReader.SetActive(false);


        ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();
	}


//--> Wait until the ingameGlbalManager finish his initialization
	IEnumerator Init(){

		yield return new WaitUntil (ingameGlobalManager.instance.r_InitBool);
		diary = ingameGlobalManager.instance.currentDiary;
		GenerateListOfDiaryEntry (currentPage);
	}
		
// --> Display Title in Diary 
	void GenerateListOfDiaryEntry (int Page) {
		//ingameGlobalManager gManager = ingameGlobalManager.instance;

		int firstEntry = 0 + howManyENtryByPage * Page;
		int lastEntry = howManyENtryByPage + howManyENtryByPage * Page;

		// Check if next page entry and last page entry buttons need to be activated
		if (ingameGlobalManager.instance.currentPlayerDiaryList.Count > lastEntry) {
			nextPage.SetActive (true);} 
		else {
			nextPage.SetActive (false);}

		if (Page != 0) {
			lastPage.SetActive (true);} 
		else {
			lastPage.SetActive (false);}


		//-> Display Title	
		for (var i =firstEntry ; i < lastEntry; i++) {
			if (ingameGlobalManager.instance.currentPlayerDiaryList.Count > i) {
				listText[i%howManyENtryByPage].text = diary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [ingameGlobalManager.instance.currentPlayerDiaryList[i]].diaryTitle [0];
				listEntry [i%howManyENtryByPage].SetActive (true);
			} else {
				listEntry [i%howManyENtryByPage].SetActive (false);
			}

		}
	}

//--> Call last 14 entry
	public void F_lastPage(){
		currentPage--;
		GenerateListOfDiaryEntry (currentPage);
	}
//--> Call next 14 entry
	public void F_nextPage(){
		currentPage++;
		GenerateListOfDiaryEntry (currentPage);
	}

//--> Display page for a selected entry
	public void showDiaryEntry(int value){
		//ingameGlobalManager gManager = ingameGlobalManager.instance;
		//Debug.Log("a value: " + value);
		if (diaryReaderTxt) {
			lastPageReader.SetActive(false);

			//currentEntryDisplayedOnScreen = ingameGlobalManager.instance.currentPlayerDiaryList[value] + howManyENtryByPage * currentPage;
			currentEntryDisplayedOnScreen = ingameGlobalManager.instance.currentPlayerDiaryList[value + howManyENtryByPage * currentPage] ;
			currentPageReader = 0;
			if (diary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText.Count > 1)
				nextPageReader.SetActive (true); 
			else 
				nextPageReader.SetActive (false);

			diaryReaderTxt.text = diary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText [currentPageReader];

			ingameGlobalManager.instance.navigationList.Add ("DiaryMultiPages");

			//--> Display available actions on screen
			//ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false, true);
			ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
			ingameGlobalManager.instance.audioMenuClips.playASound (3);                         // Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
		}
	}

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


//--> Change page in the diary Reader
	public void ChangePageInDiaryReader(){
		//ingameGlobalManager gManager = ingameGlobalManager.instance;

		if(diary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText.Count > currentPageReader+1){
			nextPageReader.SetActive (true);} 
		else {
			nextPageReader.SetActive (false);}

		if (currentPageReader != 0) {
			lastPageReader.SetActive (true);} 
		else {
			lastPageReader.SetActive (false);}

		diaryReaderTxt.text = diary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [currentEntryDisplayedOnScreen].diaryText [currentPageReader];
	}


//--> Init all entry 
	public void InitDiaryAllEntry(){
		GenerateListOfDiaryEntry (currentPage);
	}


	void DisableAllPuzzleRefPosHand()
	{
		initRefPositionSprite[] initRefPositionSpriteList = FindObjectsByType<initRefPositionSprite>(FindObjectsSortMode.None);

		foreach (var child in initRefPositionSpriteList)
			child.GetComponent<SpriteRenderer>().enabled = false;
	}

	void EnableAllPuzzleRefPosHand()
	{
		initRefPositionSprite[] initRefPositionSpriteList = FindObjectsByType<initRefPositionSprite>(FindObjectsSortMode.None);

		foreach (var child in initRefPositionSpriteList)
			child.GetComponent<SpriteRenderer>().enabled = true;
	}
}
