// Description : ingameGlobalManger.cs : Manage the gameobject ingameGlobalManager in scene view. 
// Only one instance of this object can be created in scene. This object brings together elements used by many scripts. 
// Many scripts refer to the information contained in this script.
// From any script you can access this script with : ingameGlobalManager.instance
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Globalization;

public class ingameGlobalManager : MonoBehaviour {
	public bool							SeeInspector = true;                
	public static ingameGlobalManager 	instance = null;                    // Static instance of GameManager which allows it to be accessed by any other script.

	public bool 						b_Ingame_Pause = false;             // Know if the game is paused
	public bool 						b_AllowCharacterMovment = true;     // Know if the character could move
	public bool 						b_DesktopInputs = true;             // Know if the platform is desktop. True = Desktop | False = mobile
	public bool 						b_Joystick = false;                 // True Joystick is used | False = Keyboard and mouse are used

	public datasProjectManager 			dataFolder;                         // Refers to data that contains the global game parameters

	public TextList 					currentDiary;                       // Refers to the current Diary datas
    public TextList 					currentInventory;                   // Refers to the current Inventory datas                   
    public TextList 					currentInfo;                        // Refers to the current Info datas
    public TextList 					currentFeedback;                    // Refers to the current Feedback datas


	public int 							currentLanguage = 0;	            // Refers to the current selected language 

    public List<int> 					currentPlayerDiaryList 		= new List<int> (); // Refers to objects contained in the player's Diary 
    public List<int> 					currentPlayerInventoryList 	= new List<int> (); // Refers to objects contained in the player's inventory 
    public List<bool>                   currentPlayerInventoryObjectVisibleList  = new List<bool>(); // Refers to objects contained in the player's inventory 

    public SaveDataMethods 			    saveDataMethods = new SaveDataMethods();    // Access script SaveDataMethods that contains methods for the save system

	public int                          currentSaveSlot = 0;                // The current slot used to load and save player progression
	public SaveAndLoadManager  			saveAndLoadManager;                 // Script that manage the save system
    public LevelManager 				_levelManager;                      // Refers to the script that handles the objects that need to be backed up

	public bool 						initScene = false;                  // Know if the scene initialisation is finished

	public investigationCam				cameraViewer3D;                     // access script that manage the 3D viewer
    public UIVariousFunctions			canvasPlayerInfos;                  // Access script that manage the canvasPlayerInfos

	public Menu_Manager		 			canvasMainMenu;                     // Access the Menu_Manager
    public Menu_Manager		 			canvasLoadingScreen;                // Access the Menu_Manager
    public ingameMultiPageText		 	Game_ObjectReader;                  // Access the Game_ObjectReader
    public GameObject		 			canvasMobileInputs;                 // Access the canvasMobileInputs
	public GameObject		 			reticule;                           // Access reticule gameObject
	public GameObject		 			reticuleJoystick;                   // Access the fake Joystick cursor
    public Transform		 			reticuleJoystickImage;	            // Access the fake Joystick cursor	
    public JoystickReticule 			_joystickReticule;                  // Access the fake Joystick cursor

	public AudioSource 					audioMenu;                          // Access Audiosource that manage Menu sounds
	public audioMenuClipList 			audioMenuClips;                     // Access the script that manage the Menu Sounds

    public bool 						mouseWaitUnitlFirstMouseMove = true;// Prevent bug with the mouse when CursorLockMode is changed

    public bool 						b_currentCursorState = true;        // Cursor state
	public bool 						b_currentCursorVisibility = true;   // Cursor visibilty


	public bool 						isPaused = false;                   // Check if the application is paused

	// Ui buttons
	public 	List<GameObject> 			objRef			= new List<GameObject>();   // 
	public 	List<string> 				tagList	 		= new List<string> ();

	public bool 						b_focusModeIsActivated = false;
	public GameObject					currentFocusedGameObject;
	public objTranslateRotate 			currentobjTranslateRotate;

	public GameObject 					currentPlayer;
	public int 							currentSceneBuildInIndex = 0;

	public bool 						b_bodyMovement = true;				// Use with Triggers to stop moving the player. Player Camera still move

	// Navigation (know the current player action)
	public List<string> 				navigationList = new List<string>();


	public GameObject 					lastUIButtonSelected;
	public List<GameObject> 			navigationButtonList = new List<GameObject>();


	public List<string> 				inputListOfStringGamepadAxis = new List<string> ();
	public List<KeyCode> 				inputListOfStringGamepadButton = new List<KeyCode> ();
	public List<string> 				inputListOfStringKeyboardAxis = new List<string> ();
	public List<KeyCode> 				inputListOfStringKeyboardButton = new List<KeyCode> ();

	public List<float> 					inputListOfFloatGamepadButton = new List<float> ();
	public List<bool> 					inputListOfBoolGamepadButton = new List<bool> ();
	public List<float> 					inputListOfFloatKeyboardButton = new List<float> ();
	public List<bool> 					inputListOfBoolKeyboardButton = new List<bool> ();

	public EventSystem 					eventSys;

	public VoiceOver_Manager 			voiceOverManager;
	public bool 						subtitlesState = true;			

	public mobileInputsFingerMovement	mobileInputFinger;
    public conditionsToAccessThePuzzle  currentPuzzle;
    public bool                         b_dragAndDropActivated = false;


    public bool                         b_InputIsActivated = true;

    public GameObject testText;

    public List<TextProperties> UITextList = new List<TextProperties>();

    public backInputs _backInputs;

    public bool showDebugLog = false;

    public bool _D = false;              // Use for Debug Mode
    public bool _P = false;              // Use for Debug Mode

    public focusCamEffect _focusCamEffect;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);  

		loadDatas ();

		if (b_DesktopInputs &&  SceneManager.GetActiveScene ().buildIndex != 0) 		// Desktop case. Only if we are not in the main menu Scene
			StartCoroutine( changeLockStateLock ());

        _backInputs = gameObject.GetComponent<backInputs>();
	}

	void Start(){
		DontDestroyOnLoad (gameObject);

        _focusCamEffect =  gameObject.GetComponent<focusCamEffect>();

		// Reticule
		GameObject tmpObj = GameObject.Find("Reticule");
		if (tmpObj) {
			
			if (!b_DesktopInputs) {				// Deactivate Reticule if Mobile input are used
				tmpObj.SetActive(false);
				reticule = null;
			} else {
				tmpObj.SetActive(true);
				reticule = tmpObj;
			}
		}

		initIngameGlobalManageVariables ();
        saveAndLoadManager.createListOfAllUITextInTheCurrentScene();


		tmpObj = GameObject.Find("ReticuleJoystick");
		if (tmpObj) {
			reticuleJoystick = tmpObj;

			reticuleJoystickImage = reticuleJoystick.transform.GetChild (0);
			_joystickReticule = reticuleJoystick.GetComponent<JoystickReticule> ();
		}


		// canvasMobileInputs
		GameObject tmpMobileCanvas = GameObject.Find("mobileCanvas");
		if (tmpMobileCanvas) {
			canvasMobileInputs = tmpMobileCanvas.GetComponent<canvasMobileConnect>().canvas_Mobile;
			if (b_DesktopInputs) {canvasMobileInputs.SetActive(false);}  // Deactivate Mobile canvas if Desktop input are used
			else {canvasMobileInputs.SetActive(true);}
		}


		StartCoroutine("loadingData");


        //-> Objects in the LevelManager are Initialized when the scene start (Only on Editor Mode).
        if (Application.isEditor)
            saveAndLoadManager.initLevelManagerObjectsUnityEditorOnly();

	}


	bool loadDatas(){
		bool result = false;
		currentDiary 		= Resources.Load(dataFolder.currentDatasProjectFolder + "/TextList/wTextnVoices") as TextList;
		currentInventory 	= Resources.Load(dataFolder.currentDatasProjectFolder + "/TextList/wItem") as TextList;
		currentInfo 		= Resources.Load(dataFolder.currentDatasProjectFolder + "/TextList/wUI") as TextList;
		currentFeedback 	= Resources.Load(dataFolder.currentDatasProjectFolder + "/TextList/wFeedback") as TextList;

		result = true;
		return result;
	}



	void OnApplicationFocus(bool hasFocus)
	{
        if (!Application.isEditor)
        {
            if (b_Ingame_Pause)
                onlyUnPauseGame();
            else
                onlyPauseGame();
            
            if (!testText) testText = GameObject.Find("txt_Timer");
            if (testText)testText.GetComponent<Text>().text += "OnApplicationFocus_ " + hasFocus;

            StartCoroutine(changeLockStateConfined(b_currentCursorVisibility));

            if (b_DesktopInputs && SceneManager.GetActiveScene().buildIndex != 0)
            {       // Desktop case. Only if we are not in the main menu Scene
                if (!b_currentCursorState)
                    StartCoroutine(changeLockStateLock());
                else
                    StartCoroutine(changeLockStateConfined(b_currentCursorVisibility));
            }
        }
	}

    public IEnumerator changeLockStateLock()
    {
        if (b_DesktopInputs) {    // True for Desktop | False for Mobile
            mouseWaitUnitlFirstMouseMove = false;
            yield return new WaitForEndOfFrame();
            //if (Application.platform != RuntimePlatform.WindowsEditor)
            Cursor.lockState = CursorLockMode.None;
            yield return new WaitForEndOfFrame();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (reticuleJoystickImage && reticuleJoystickImage.gameObject.activeSelf)
            {
                reticuleJoystickImage.gameObject.SetActive(false);
            }

            b_currentCursorVisibility = Cursor.visible;
            b_currentCursorState = true;
        }
	}

	public IEnumerator changeLockStateConfined(bool b_showCursor){
        if (b_DesktopInputs)    // True for Desktop | False for Mobile
        {
            yield return new WaitForEndOfFrame();
            //if (Application.platform != RuntimePlatform.WindowsEditor)
            Cursor.lockState = CursorLockMode.None;
            yield return new WaitForEndOfFrame();
            Cursor.lockState = CursorLockMode.Confined;
           
            if (b_Joystick){
                if (reticuleJoystick){
                    if (b_showCursor){
                       // reticuleJoystickImage.gameObject.SetActive(false);
                       // yield return new WaitForEndOfFrame();
                        _joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);
                        yield return new WaitForEndOfFrame();
                        if(ingameGlobalManager.instance.navigationList.Count > 0 
                           && ingameGlobalManager.instance.navigationList[0] == "MainMenuManager"){
                            reticuleJoystickImage.gameObject.SetActive(false);
                        }
                        else{
                            if(ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                               ingameGlobalManager.instance.b_DesktopInputs &&
                               ingameGlobalManager.instance.b_Joystick)
                              ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive(true);

                        
                        }
                    }
                    else{
                        reticuleJoystickImage.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (b_showCursor)
                    Cursor.visible = true;
                else
                    Cursor.visible = false;

                b_currentCursorVisibility = Cursor.visible;

                mouseWaitUnitlFirstMouseMove = true;
                b_currentCursorState = false;
            }
            //Debug.Log("Confined");
        }
	}


	void OnApplicationPause(bool pauseStatus)
	{
        if(!Application.isEditor){
            if (!b_Ingame_Pause)
                onlyPauseGame();
            else 
                onlyUnPauseGame();
            
            isPaused = pauseStatus; 
        }
        if (!testText) testText = GameObject.Find("txt_Timer");
        if (testText)testText.GetComponent<Text>().text += "OnApplicationPause_ " + pauseStatus;

	}

	public void PauseGame(){
		if (!b_Ingame_Pause){
			b_Ingame_Pause = true;
			AudioListener.pause = true;
		}
		else{
			b_Ingame_Pause = false;
			AudioListener.pause = false;
		}

	//-> Pause the Voice Over
		GameObject objVoicOverManager = GameObject.Find ("VoiceOver_Manager");
		if (objVoicOverManager)objVoicOverManager.GetComponent<VoiceOver_Manager> ()._Pause ();
	}

    public void onlyPauseGame()
    {

        b_Ingame_Pause = true;
        AudioListener.pause = true;
     

        //-> Pause the Voice Over
        //GameObject objVoicOverManager = GameObject.Find("VoiceOver_Manager");
        //if (objVoicOverManager) objVoicOverManager.GetComponent<VoiceOver_Manager>()._Pause();
    }

    public void onlyUnPauseGame()
    {
        b_Ingame_Pause = false;
        AudioListener.pause = false;


        //-> Pause the Voice Over
        //GameObject objVoicOverManager = GameObject.Find("VoiceOver_Manager");
        //if (objVoicOverManager) objVoicOverManager.GetComponent<VoiceOver_Manager>()._Pause();
    }




	public IEnumerator loadingData(){
//--> Load Datas
		GameObject tmpObj = GameObject.Find("LevelManager");
		if (tmpObj) {
			_levelManager = tmpObj.GetComponent<LevelManager>();
			saveAndLoadManager._levelManager = _levelManager;
		}



//--> Init UI Text (language)
		updateUITexts();

		//Debug.Log("UI Ok" );

//--> Init ALl Gameobject in the scene

		int numberTotal = 0;
		int number = 0;
		GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

		foreach (GameObject go in allObjects) {
			Transform[] Children = go.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in Children) {
				if(child.GetComponent<TextProperties>()){
					//Find_UniqueId_In_The_TextList (child);
					number++;
				}
				numberTotal++;
			}
		}

        if (showDebugLog)Debug.Log("GameObjects Ok " +  numberTotal);

		initScene = true;

		return null;
	}

	public bool r_InitBool(){
		return initScene;
	}






//--> Update UI Text using the current selected language
	public void updateUITexts(){
		List<GameObject> listCanvas = new List<GameObject> ();


		if (ingameGlobalManager.instance.canvasMainMenu != null)
			listCanvas.Add (ingameGlobalManager.instance.canvasMainMenu.gameObject);
		
		if (ingameGlobalManager.instance.canvasPlayerInfos != null)
			listCanvas.Add (ingameGlobalManager.instance.canvasPlayerInfos.gameObject);
		
		if (ingameGlobalManager.instance.canvasMobileInputs != null)
			listCanvas.Add (ingameGlobalManager.instance.canvasMobileInputs.gameObject);
		
		if (ingameGlobalManager.instance.canvasLoadingScreen != null)
			listCanvas.Add (ingameGlobalManager.instance.canvasLoadingScreen.gameObject);


		for (var i = 0; i < listCanvas.Count; i++) {
			GameObject newCanvas = listCanvas[i];

			if (newCanvas) {
				//Debug.Log (listCanvas [i]);
				TextProperties[] allObjects = newCanvas.GetComponentsInChildren<TextProperties> (true);

				foreach (TextProperties go in allObjects) {
					TextProperties[] Children = go.GetComponentsInChildren<TextProperties> (true);
					foreach (TextProperties child in Children) {
						child.updateInfoText ();
					}

				}

				if (i == 1) {		// Case : canvas_PlayerInfos Ui Check button
					btn_Check[] allCheckObjs = newCanvas.GetComponentsInChildren<btn_Check> (true);

					foreach (btn_Check go in allCheckObjs) {
						btn_Check[] Children = go.GetComponentsInChildren<btn_Check> (true);
						foreach (btn_Check child in Children) {
							child.updateText ();
						}

					}
				}
			}
		}



        foreach (TextProperties obj in UITextList)
        {
            obj.updateInfoText();
        }

	}

	public void FocusIsActivated(GameObject obj){
		ingameGlobalManager.instance.b_focusModeIsActivated = true;
		currentFocusedGameObject = obj;
		navigationList.Add ("Focus");
	}





	//--> save the ingameGlobalManager parameters
	public string saveInGameManagerParams () {
		CultureInfo cultureInfo = new CultureInfo("en-US");
		System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

		string separator = ",";
		string result = "";

		result += b_Ingame_Pause + separator;											// 0
		result += b_AllowCharacterMovment + separator;									// 1
		result += b_DesktopInputs + separator;											// 2

		result += currentLanguage + separator;											// 3

		result += currentSaveSlot + separator;											// 4

		result += b_currentCursorState + separator;										// 5
		result += b_currentCursorVisibility + separator;								// 6

		result += isPaused + separator;													// 7
			
		result += b_focusModeIsActivated + separator;									// 8
		if(currentFocusedGameObject == null)
			result += "null"+ separator; 												// 9 : GameObject		
		else
			result += currentFocusedGameObject.name + separator; 		
			

		if(currentobjTranslateRotate == null)
			result += "null"+ separator; 												// 10 : objTranslateRotate		
		else
			result += currentobjTranslateRotate.name + separator;			 	


		result += SceneManager.GetActiveScene ().buildIndex  + separator;				// 11


		result += gameObject.GetComponent<focusCamEffect> ().saveFocusModeInformations ();	// 12

		for (var i = 0; i < navigationList.Count; i++) {								// 20+ 
			if (i == 0)result += separator;

			if(i < navigationList.Count-1)
				result += navigationList [i] + separator;
			else
				result += navigationList [i];
		}

			//result += character.transform.localPosition.x + separator;		// Player Body X Position


		return result;
	}

	//--> load the ingameGlobalManager parameters
	public bool loadInGameManager (string s_Datas) {
		System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

		if (showDebugLog)Debug.Log ("loadInGameManager : " + s_Datas);
		if (s_Datas != "") {
			string[] codes = s_Datas.Split (',');


			/*if (codes [0] == "True")
				b_Ingame_Pause = true;
			else
				b_Ingame_Pause = false;
			*/
			if (codes [1] == "True")
				b_AllowCharacterMovment = true;
			else
				b_AllowCharacterMovment = false;
			
			if (codes [2] == "True")
				b_DesktopInputs = true;
			else
				b_DesktopInputs = false;


			currentLanguage = Int32.Parse (codes [3]);	

			currentSaveSlot = Int32.Parse (codes [4]);	

			if (codes [5] == "True")
				b_currentCursorState = true;
			else
				b_currentCursorState = false;

			

			if (codes [6] == "True")
				b_currentCursorVisibility = true;
			else
				b_currentCursorVisibility = false;

			if (codes [7] == "True")
				isPaused = true;
			else
				isPaused = false;

			if (codes [8] == "True")
				b_focusModeIsActivated = true;
			else
				b_focusModeIsActivated = false;


			if (codes [9] != "null") {
				//Debug.Log (codes [9]);
				currentFocusedGameObject = GameObject.Find (codes [9]); 		// GameObject		
			}



			if (codes [10] != "null") {
				currentobjTranslateRotate = GameObject.Find (codes [10]).GetComponent<objTranslateRotate> ();				// objTranslateRotate 			
			}


			currentSceneBuildInIndex = Int32.Parse (codes [11]);

			int tmpIndex = 11;

			gameObject.GetComponent<focusCamEffect> ().loadFocusModeInformations (new List<string> {
				codes [tmpIndex + 2],
				codes [tmpIndex + 3],
				codes [tmpIndex + 4],
				codes [tmpIndex + 5],
				codes [tmpIndex + 6],
				codes [tmpIndex + 7],
				codes [tmpIndex + 8]
			});


			int _navLength = codes.Length - tmpIndex - 9;
            if (showDebugLog)Debug.Log ("codes.Length :" + codes.Length + " : " + "_navLength : " + _navLength);
			if (_navLength > 0) {
				navigationList.Clear ();
				// Update the navigation List. Where is the player. In Focus Focus, looking the inventory ...
				for (var i = 0; i < _navLength; i++) {													
					navigationList.Add (codes [tmpIndex + 9 + i]);
				}
			}
		}
			
		if (b_DesktopInputs) {
			//if(b_currentCursorState)
				StartCoroutine (changeLockStateLock ());
			//else
			//	StartCoroutine (changeLockStateConfined (b_currentCursorVisibility));

		}
		return true ;
	}



	public bool loadPlayerInventoryList (string s_Datas) {
		if (s_Datas != "") {
           

			currentPlayerInventoryList.Clear ();
            currentPlayerInventoryObjectVisibleList.Clear();
			string[] codes = s_Datas.Split (':');

            int HowManyEntry = Int32.Parse(codes[0]);

            //Debug.Log("s_Datas : " + s_Datas + " : HowManyEntry : " + HowManyEntry);

            for (var i = 1; i < HowManyEntry+1; i++) {
				currentPlayerInventoryList.Add(Int32.Parse(codes[i]));
			}


            for (var i = HowManyEntry; i < HowManyEntry*2; i++)
            {
                if(codes[i+1] == "T")
                    currentPlayerInventoryObjectVisibleList.Add(true);
                else
                    currentPlayerInventoryObjectVisibleList.Add(false); 
            }
		}

		return true ;
	}

	public bool loadPlayerDiaryList (string s_Datas) {

		if (s_Datas != "") {
            
			currentPlayerDiaryList.Clear ();
			string[] codes = s_Datas.Split (':');

			for (var i = 0; i < codes.Length; i++) {
				currentPlayerDiaryList.Add(Int32.Parse(codes[i]));
			}
		}

		return true ;
	}



    IEnumerator InitCanvasMenu()
    {
        canvasPlayerInfos.gameObject.SetActive(false);
        yield return null;
        canvasPlayerInfos.gameObject.SetActive(true);
    }

	public bool initIngameGlobalManageVariables(){
        //Debug.Log("here Init");


	// LevelManager
		GameObject tmpObj = GameObject.Find("LevelManager");
		if (tmpObj) {
			_levelManager = tmpObj.GetComponent<LevelManager> ();
			saveAndLoadManager._levelManager = _levelManager;}
			
	// cameraViewer3D
		tmpObj = GameObject.Find("CamShowObject");
		if (tmpObj) {
			cameraViewer3D = tmpObj.GetComponent<investigationCam> ();}

	// canvasPlayerInfos
		tmpObj = GameObject.Find("Canvas_PlayerInfos");
		if (tmpObj) {
			canvasPlayerInfos = tmpObj.GetComponent<UIVariousFunctions> ();

            // Reticule
            tmpObj = GameObject.Find("Reticule");
            if (tmpObj)
            {
                if (!b_DesktopInputs)
                {               // Deactivate Reticule if Mobile input are used
                    tmpObj.SetActive(false);
                    reticule = null;
                }
                else
                {
                    tmpObj.SetActive(true);
                    reticule = tmpObj;
                }
            }

            tmpObj = GameObject.Find("ReticuleJoystick");
            if (tmpObj)
            {
                reticuleJoystick = tmpObj;
                reticuleJoystickImage = reticuleJoystick.transform.GetChild(0);
                _joystickReticule = reticuleJoystick.GetComponent<JoystickReticule>();

                if (!b_DesktopInputs && reticuleJoystickImage && reticuleJoystickImage.gameObject.activeSelf)
                {
                    reticuleJoystickImage.gameObject.SetActive(false);
                }
            }

            StartCoroutine(InitCanvasMenu());
		}



       



	// canvasMainMenu
		tmpObj = GameObject.Find("Canvas_MainMenu");
		if (tmpObj) {
			canvasMainMenu = tmpObj.GetComponent<Menu_Manager> ();
		}

		switchKeyboardJoystick ();

	// canvasLoadingScreen
		tmpObj = GameObject.Find("Canvas_LoadingScreen");
		if (tmpObj) {
			canvasLoadingScreen = tmpObj.GetComponent<Menu_Manager> ();}

	// Game_ObjectReader
		tmpObj = GameObject.Find("Game_ObjectReaderConnect");
		if (tmpObj) {
			Game_ObjectReader = tmpObj.GetComponent<ObjectReaderConnect>().Game_ObjectReaderConnect.GetComponent<ingameMultiPageText> ();}

	// canvasMobileInputs
		GameObject tmpMobileCanvas = GameObject.Find("mobileCanvas");
		if (tmpMobileCanvas) {
			canvasMobileInputs = tmpMobileCanvas.GetComponent<canvasMobileConnect>().canvas_Mobile;
			if (b_DesktopInputs) {canvasMobileInputs.SetActive(false);} // Deactivate Mobile canvas if Desktop input are used
			else {canvasMobileInputs.SetActive(true);}
		}

		lastUIButtonSelected = null;
		navigationButtonList.Clear ();

	// Find the event System
		GameObject tmpEventSys = GameObject.Find ("EventSystem");
		if (tmpEventSys) {
			eventSys = tmpEventSys.GetComponent<EventSystem> ();
		}

	// InitInputs 

		initInputsValues ();


	// inputListOfStringGamepad


	// audioMenu
		tmpObj = GameObject.Find("audioMenu");
		if (tmpObj) {
			audioMenu = tmpObj.GetComponent<AudioSource> ();}

	// audioMenuClips
		tmpObj = GameObject.Find("audioMenu");
		if (tmpObj) {
			audioMenuClips = tmpObj.GetComponent<audioMenuClipList> ();}


	// voiceOverManager
		tmpObj = GameObject.Find ("VoiceOver_Manager");
		if (tmpObj) voiceOverManager = tmpObj.GetComponent<VoiceOver_Manager> ();


	





		currentPlayerDiaryList.Clear ();
		currentPlayerInventoryList.Clear ();
        currentPlayerInventoryObjectVisibleList.Clear();

	// Connect script mobileInputsFingerMovement to canvasMobileInputs
		if (GetComponent<mobileInputsFingerMovement> () && !b_DesktopInputs) {
			//Debug.Log ("Here Ok");
			if(canvasMobileInputs)GetComponent<mobileInputsFingerMovement> ().m_Raycaster = canvasMobileInputs.GetComponent<GraphicRaycaster> ();
			if(canvasPlayerInfos)GetComponent<mobileInputsFingerMovement> ().listRaycaster[0] = canvasPlayerInfos.GetComponent<GraphicRaycaster> ();
			if(canvasMobileInputs)GetComponent<mobileInputsFingerMovement> ().listRaycaster[1] = canvasMobileInputs.GetComponent<GraphicRaycaster> ();
			if(canvasMainMenu)GetComponent<mobileInputsFingerMovement> ().listRaycaster[2] = canvasMainMenu.GetComponent<GraphicRaycaster> ();
		}

        // currentPlayer
        b_bodyMovement = true;

		tmpObj = GameObject.Find("Character");
		if (tmpObj) {
			currentPlayer = tmpObj;}


		if (tmpMobileCanvas && tmpMobileCanvas.GetComponent<canvasMobileConnect> ())
			tmpMobileCanvas.GetComponent<canvasMobileConnect>().initializedCanvasMobile ();

		gameObject.GetComponent<focusCamEffect> ().Init ();



        // language Choice
        tmpObj = GameObject.Find("OptionsManager");
        if (tmpObj)
        {
            tmpObj.GetComponent<GameOptionsManager>().initData();
        }



		return true;
	}

	public KeyCode FindTheKeyCodeUpdate(string s_Convert){																

		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
		if (s_Convert == key.ToString())
			{return key;}
		}
		return KeyCode.None;
	}


	public void switchKeyboardJoystick(){
		
		if (PlayerPrefs.GetInt ("InputsType") == 0) { 	// Desktop
			b_Joystick = false;}
		else {											// Joystick
			b_Joystick = true;}


		if (canvasPlayerInfos) {
		//Debug.Log ("here visible : " + b_Joystick);
			// Joystick
			if (b_Joystick) {
				if (SceneManager.GetActiveScene ().buildIndex != 0) {
					canvasPlayerInfos.gameObject.GetComponent<GraphicRaycaster> ().enabled = false;
					if (reticuleJoystick && Cursor.visible) {															// If needed
						reticuleJoystick.GetComponent<JoystickReticule> ().joyReticule2.gameObject.SetActive (true);	// enable Fake Mouse
						Cursor.visible = false;																			// disable cursor
					}
				} 
				else {

				}
			}
			// Desktop
			else if (!b_Joystick) {
				
				if (SceneManager.GetActiveScene ().buildIndex != 0) {
                    if (b_DesktopInputs)
                    {
                        canvasPlayerInfos.gameObject.GetComponent<GraphicRaycaster>().enabled = true;

                        if (reticuleJoystick && reticuleJoystick.GetComponent<JoystickReticule>().joyReticule2.gameObject.activeSelf)
                        {// If needed
                            reticuleJoystick.GetComponent<JoystickReticule>().joyReticule2.gameObject.SetActive(false); // disable Fake Mouse
                            Cursor.visible = true;                                                                  // enable cursor
                        }
                    }
						
				} 
				else {

				}
			}

		}

		if (canvasMainMenu) {
            // Joystick
            if (b_DesktopInputs)
            {
                if (b_Joystick && SceneManager.GetActiveScene().buildIndex != 0)
                {
                    canvasMainMenu.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
                }
                // Desktop
                else if (!b_Joystick && SceneManager.GetActiveScene().buildIndex != 0)
                {
                    canvasMainMenu.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
                }
            }
		}
	}




	public void initInputsValues (){
	
		GameObject tmpInputs = GameObject.Find ("InputsManager");
		if (tmpInputs) {
			string inputsRawList = tmpInputs.GetComponent<MM_MenuInputs> ().loadInputs ();

			//Debug.Log (inputsRawList);

			// Parse
			string[] InputList = inputsRawList.Split ('_');


			inputListOfStringGamepadAxis.Clear ();
			inputListOfStringGamepadButton.Clear ();
			inputListOfStringKeyboardAxis.Clear ();
			inputListOfStringKeyboardButton.Clear ();

			string[] InputListJoystickAxis = InputList [0].Split (':');
			string[] InputListJoystickButton = InputList [1].Split (':');
			string[] InputListKeyboardAxis = InputList [2].Split (':');
			string[] InputListKeyboardButton = InputList [3].Split (':');

			//Debug.Log (InputList [0]);
			//Debug.Log (InputList [1]);

			foreach (string _input in InputListJoystickAxis) {
				inputListOfStringGamepadAxis.Add (_input);
			}

			foreach (string _input in InputListJoystickButton) {
				inputListOfStringGamepadButton.Add (FindTheKeyCodeUpdate (_input));
			}

			foreach (string _input in InputListKeyboardAxis) {
				inputListOfStringKeyboardAxis.Add (_input);
			}

			foreach (string _input in InputListKeyboardButton) {
				inputListOfStringKeyboardButton.Add (FindTheKeyCodeUpdate (_input));
			}


		//-> Init Input Float
			string inputsRawListFloat = tmpInputs.GetComponent<MM_MenuInputs> ().loadInputsFloatsValue ();

			//Debug.Log (inputsRawListFloat);

			// Parse
			string[] InputListFloat = inputsRawListFloat.Split ('_');

			inputListOfFloatGamepadButton.Clear ();
			inputListOfFloatKeyboardButton.Clear ();


			string[] InputListJoystickFloat = InputListFloat [0].Split (':');
			//string[] InputListJoystickBool = InputList [1].Split (':');
			string[] InputListKeyboardFloat = InputListFloat [1].Split (':');
			//string[] InputListKeyboardBool = InputList [3].Split (':');


			foreach (string _input in InputListJoystickFloat) {
				inputListOfFloatGamepadButton.Add (float.Parse(_input));
			}

			foreach (string _input in InputListKeyboardFloat) {
				inputListOfFloatKeyboardButton.Add (float.Parse(_input));
			}
			

		//-> Init Input Bool
		string inputsRawListBool = tmpInputs.GetComponent<MM_MenuInputs> ().loadInputsBoolsValue ();

		//Debug.Log (inputsRawListBool);

		// Parse
		string[] InputListBool = inputsRawListBool.Split ('_');

			inputListOfBoolGamepadButton.Clear ();
			inputListOfBoolKeyboardButton.Clear ();


		string[] InputListJoystickBool = InputListBool [0].Split (':');

		string[] InputListKeyboardBool = InputListBool [1].Split (':');


		foreach (string _input in InputListJoystickBool) {
			inputListOfBoolGamepadButton.Add (bool.Parse(_input));
		}

		foreach (string _input in InputListKeyboardBool) {
			inputListOfBoolKeyboardButton.Add (bool.Parse(_input));
		}

		}
	}


    public void deactivateBodyMovement(){
        b_bodyMovement = false;
    }
    public void activateBodyMovement()
    {
        b_bodyMovement = true;
    }

    //--> Convert bool to T or F string
    private string r_TrueFalse(bool s_Ref)
    {
        if (s_Ref) return "T";
        else return "F";
    }

}
