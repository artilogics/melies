//Description : UIVariousFunctions : Manage the UI Icons displayed for Interactable Objects, Items ...
// Hierarchy : Canvas_PlayerInfos
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVariousFunctions : MonoBehaviour {

	public List<GameObject> 	objrefList				= new List<GameObject>();		// list of Interactable Object, item ... in scene view 
	public List<bool> 			objVisible				= new List<bool>();				// know if Interactable Object, item ... in scene view are visible in the scene view (no object between camera and object.)
	public List<float> 			objrefListCurrentScale	= new List<float>();			// Current scale of UI Icons
	public List<GameObject> 	gameobjectList 			= new List<GameObject>();		// UI Icon refering to objrefList (same order as objrefList) 


	public GameObject			btn_Forward;											// Mobile UI. Move forward
	public GameObject			btn_Backward;											// Mobile UI. Move backward
	public GameObject			btn_JoystickLook;										// Mobile UI. Joystick look

	public GameObject			btn_BackFromFocus;										// Mobile UI. Back

	public bool					iconAvailable = true;									// know if UI icons need to be displayed
	public bool 				interactiveIconAvailable = true;						// know if UI interactive icons need to be displayed

	public float 				iconScaleSpeed = 4;										// icon scale speed
	public float 				iconCurrentScale = 1;									// icon current scale
	public AnimationCurve		iconCurveSpeed;											// icon scale speed


	// Section Available Action
	public GameObject			btn_LeftClic;
	public GameObject			btn_RightClic;
	public GameObject			btn_FingerSlide;
	public GameObject			btn_FingerDoubleTap;

	public GameObject			btn_JoystickBackBouton;
	public GameObject			btn_JoystickStick;

	public GameObject			btn_RightClic_OnlyMainMenu;
	public GameObject			btn_FingerDoubleTap_OnlyMainMenu;
	public GameObject			btn_BackButton_Joystick_OnlyMainMenu;

	public infoUI				_infoUI;

	public Text					txt3DViewer;
    public bool                 b_rememberLastStatetxt3DViewer = false;
    public GameObject           obj_PuzzleNotAvailable;
    public bool                 b_rememberLastState = false;
   


    public GameObject           obj_ResetPuzzle;
    public bool                 b_rememberLastStateResetPuzzle = false;
    public GameObject           obj_Btn_Mobile_ExitPuzzle;

    public GameObject           Grp_ImageFakeMouse;
    public Image                Image_ObjDetected;
    public Transform            ReticuleJoystick;

    public GameObject obj_PuzzleClue;
    public GameObject obj_Grp_InfoPuzzle;


    void Start()
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        foreach(Transform  child in allTransforms){
            if(child.name == "btn_Clue"){
                obj_PuzzleClue = child.gameObject;
            }
            if (child.name == "Grp_InfoPuzzle")
            {
                obj_Grp_InfoPuzzle = child.gameObject;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate () {
		if (iconAvailable) {
			for (var i = 0; i < objrefList.Count; i++) {
                if (gameobjectList [i] != null && ingameGlobalManager.instance.b_InputIsActivated) {        // object exist and the feedback camera is not used (inputs are disabled)
			//-> Update the UI Icon position
					Vector3 btnPos = Camera.main.WorldToScreenPoint (gameobjectList [i].transform.position);
					objrefList [i].transform.position = btnPos;

			//-> Update the Ui Icon Scale if needed
					if (objrefList [i].transform.localScale.x < 1) {
						objrefListCurrentScale[i] = Mathf.MoveTowards(objrefListCurrentScale[i],1,iconScaleSpeed * Time.deltaTime);
						objrefList [i].transform.localScale = Vector2.MoveTowards (objrefList [i].transform.localScale, new Vector2 (1, 1),iconCurveSpeed.Evaluate(objrefListCurrentScale[i])* iconScaleSpeed * Time.deltaTime);
					}

			//-> Special case
					if (!interactiveIconAvailable && objrefList [i].CompareTag ("InteractObject")			// Deactivate interactive icon when focus is activated
						|| objrefList [i].CompareTag ("Item") && !objVisible [i]							// Deactivate UI Item Button if the item is not visible
                        || objrefList[i].CompareTag("PuzzleIcon") && !objVisible[i]                         // Deactivate UI Puzzle button if puzzle is not visible
                        || objrefList[i].CompareTag("OnlyFocusIcon") && !objVisible[i]
                        ||  objrefList[i].CompareTag("PuzzleIcon") && ingameGlobalManager.instance.b_focusModeIsActivated  // Deactivate UI Puzzle button if focus is activated
                        || objrefList[i].CompareTag("OnlyFocusIcon") && ingameGlobalManager.instance.b_focusModeIsActivated  // Deactivate UI Puzzle button if focus is activated
						|| objrefList [i].CompareTag ("InteractObject") && !objVisible [i]	) {				// Deactivate UI InteractObject Button if the InteractObject is not visible					
						objrefList [i].SetActive (false);
						objrefList [i].transform.localScale = Vector2.zero;
					}
					else 
						objrefList [i].SetActive (true);
				}
			}
		}
	}


// --> Instantiate Icon on screen
	public void AutoInstantiateButton(GameObject obj, GameObject objRef,int index,bool b_showTitleOnUIButton){
		GameObject tmpBtn = Instantiate (objRef,this.transform);
		tmpBtn.transform.localScale = new Vector2 (0, 0);
		tmpBtn.transform.SetSiblingIndex(0);											// Put the gameObject on the top of the child list to prevent but with mobile Inputs
		objrefList.Add (tmpBtn);
		objVisible.Add (false);
		objrefListCurrentScale.Add (0);

		gameobjectList.Add (obj);


		if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList [0])) {		// Type : Item
			tmpBtn.GetComponent<btn_Check> ().SetupButton (obj, index,b_showTitleOnUIButton);
		}
		if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList [1])) {		// Type : Action
		}
		if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList [2])) {		// Type : Info
		}
		if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList [3])) {		// Type : puzzle
            tmpBtn.GetComponent<btn_Puzzle>().currentPuzzle = obj.transform.parent.gameObject;
		}
        if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList[5]))         // Type : focus Only
        {       
            tmpBtn.GetComponent<btn_Puzzle>().currentPuzzle = obj.transform.parent.gameObject;
        }
		if (obj.gameObject.CompareTag(ingameGlobalManager.instance.tagList [4])) {		// Type : Interactive Object
			//Debug.Log("Interctive Object");
			tmpBtn.GetComponent<btn_Check> ().SetupButton (obj, index,b_showTitleOnUIButton);
		}
	}


// --> Destroy Icon On screen 
	public void AutoDestroyButton(GameObject obj){
		
		for (var i = 0; i < objrefList.Count; i++) {
			if (obj == gameobjectList [i]) {
				//Debug.Log ("Destroy");
				Destroy (objrefList [i]);
				objrefList.RemoveAt (i);
				objVisible.RemoveAt (i);
				objrefListCurrentScale.RemoveAt (i);
				gameobjectList.RemoveAt (i);
			}
		}
	}

	public bool deleteAllButtons(){
		
		for (var i = 0; i < objrefList.Count; i++) {
			//Debug.Log ("Destroy All");
			Destroy (objrefList [i]);
			objrefList.RemoveAt (i);
			objVisible.RemoveAt (i);
			objrefListCurrentScale.RemoveAt (i);
			gameobjectList.RemoveAt (i);
		}
		return true;
	}

// --> Activate Mobile UI Inputs on screen
	public void ActivateMobileMovement(GameObject obj){
		if(btn_Forward)btn_Forward.SetActive (true);
		if(btn_Backward)btn_Backward.SetActive (true);
		if(btn_JoystickLook)btn_JoystickLook.SetActive (true);
	}


// --> Deactivate Mobile UI Inputs on screen
	public void DeactivateMobileMovement(GameObject obj){
		if(btn_Forward)btn_Forward.SetActive (false);
		if(btn_Backward)btn_Backward.SetActive (false);
		if(btn_JoystickLook)btn_JoystickLook.SetActive (false);

		if(btn_BackFromFocus)btn_BackFromFocus.SetActive (true);
	}

// --> Activate Mobile Focus
	public void ActivateMobileFocus(){
		if(btn_BackFromFocus)btn_BackFromFocus.SetActive (true);
	}

// --> Deactivate Mobile Focus
	public void DeactivateMobileFocus(){
		if(btn_BackFromFocus)btn_BackFromFocus.SetActive (false);
	}
		
//--> Deactivate all the UI Icon
	public void deactivateIcons(bool allowCharacterMovement){
		iconAvailable = false;
		for (var i = 0; i < objrefList.Count; i++) {
			objrefList[i].SetActive(false);
			objrefListCurrentScale[i] = 0;
			objrefList [i].transform.localScale = new Vector2 (0, 0);
		}
		if(!allowCharacterMovement)
			ingameGlobalManager.instance.b_AllowCharacterMovment = false;
	}

//--> Deactivate Interactive UI Icon when focus mode is activated
	public void deactivateInteractiveIcons(){
		interactiveIconAvailable = false;
		for (var i = 0; i < objrefList.Count; i++) {
			if (objrefList [i].CompareTag ("InteractObject")) {
				objrefListCurrentScale[i] = 0;
				objrefList [i].transform.localScale = new Vector2 (0, 0);
				objrefList [i].SetActive (false);
			}
		}
	}

//--> activate Interactive Object when focus mode is activated
	public void activateInteractiveIcons(){
		interactiveIconAvailable = true;
	}

//--> Activate UI Icon
	public void activateIcons(){
		StartCoroutine (I_activateIcons ());
	}

	IEnumerator I_activateIcons(){
		yield return new WaitForEndOfFrame ();
		for (var i = 0; i < objrefList.Count; i++) {
			if (gameobjectList [i] != null) {
                if(Camera.main){
                    Vector3 btnPos = Camera.main.WorldToScreenPoint(gameobjectList[i].transform.position);
                    objrefList[i].transform.position = btnPos;

                    if (!interactiveIconAvailable && 
                        objrefList[i].CompareTag("InteractObject")
                       )
                        objrefList[i].SetActive(false);
                    else
                        objrefList[i].SetActive(true);  
                }
				
			}
		}

//--> Activate or deactivate inventory Object in the scene
		for (var i = 0; i < objrefList.Count; i++) {
			if (gameobjectList [i].GetComponent<TextProperties> () && gameobjectList [i].GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentInventory) {
				if (gameobjectList [i].GetComponent<Renderer> ()
				   && !gameobjectList [i].GetComponent<Renderer> ().enabled) {

					List<int> currentInventory = ingameGlobalManager.instance.currentPlayerInventoryList;
					bool result = true;

					//-> Case player currentInventory > 0
					if (currentInventory.Count > 0) {
						for (var j = 0; j < currentInventory.Count; j++) {
					

							if (gameobjectList [i] && gameobjectList [i].GetComponent<TextProperties> ().managerID
								  == currentInventory [j] ) {
								result = false;
							}
						}


						//-> Object is not in the inventory. ACtivate the gameObject in the scene
							if (gameobjectList [i] && result) {
								gameobjectList [i].GetComponent<Renderer> ().enabled = true;
								F_rendererState (gameobjectList [i], true);


						//-> Object is in the inventory. Keep the gameObject deactivated in the scene
							} else {
								if (gameobjectList [i]) {
									GameObject tmpObj = gameobjectList [i];
									//gameobjectList [i].SetActive (false);
									gameobjectList [i].GetComponent<Renderer> ().enabled = false;
									F_rendererState (gameobjectList [i], false);

									AutoDestroyButton (tmpObj);
								}
							}
						
					} 
		//-> Case Player currentInventory = 0
				else {
						//-> Object is not in the inventory. ACtivate the gameObject in the scene
						gameobjectList [i].GetComponent<Renderer> ().enabled = true;
						F_rendererState (gameobjectList [i], true);
					}
				}
			}
		}

//--> Activate or deactivate Diary Object in the scene
		for (var i = 0; i < objrefList.Count; i++) {
			if (gameobjectList [i].GetComponent<TextProperties> () && gameobjectList [i].GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentDiary) {
				if (gameobjectList [i].GetComponent<Renderer> ()
					&& !gameobjectList [i].GetComponent<Renderer> ().enabled) {
					List<int> currentDiary = ingameGlobalManager.instance.currentPlayerDiaryList;
					bool result = true;

					//-> Case player currentDiary > 0
					if (currentDiary.Count > 0) {
						for (var j = 0; j < currentDiary.Count; j++) {
							//if (gameobjectList [i].GetComponent<TextProperties> ()) {
							if (gameobjectList [i].GetComponent<TextProperties> ().managerID
								== currentDiary [j]) {
								result = false;
							}
							//}
							//-> Object is not in the inventory. ACtivate the gameObject in the scene
							if (result) {
								gameobjectList [i].GetComponent<Renderer> ().enabled = true;
								//-> Object is in the inventory. Keep the gameObject deactivated in the scene
							} else {
								GameObject tmpObj = gameobjectList [i];
								//gameobjectList [i].SetActive (false);
                                gameobjectList[i].GetComponent<Renderer>().enabled = false;
								//gameobjectList [i].transform.GetComponent<Collider> ().enabled = false;
								AutoDestroyButton (tmpObj);
							}
						}
					} 
					//-> Case Player currentDiary = 0
					else {
						//-> Object is not in the inventory. ACtivate the gameObject in the scene
						gameobjectList [i].GetComponent<Renderer> ().enabled = true;
					}
				}
			}
		}

		iconAvailable = true;
		yield return null;
	}


//--> Hide Viewer and activate UI Icons
	public void hideViewer(){
		if (ingameGlobalManager.instance.cameraViewer3D) {
			if (ingameGlobalManager.instance.navigationList.Count > 0) {
				if (ingameGlobalManager.instance.navigationList [ingameGlobalManager.instance.navigationList.Count - 1] == "Focus") {
					if(!ingameGlobalManager.instance.b_Joystick){									// Keyboard
						Cursor.visible = true;
						ingameGlobalManager.instance.b_currentCursorVisibility = Cursor.visible;
					}
					else{																			// Joystick
						if (ingameGlobalManager.instance.reticuleJoystickImage) {
                                ingameGlobalManager.instance._joystickReticule.newPosition (Screen.width / 2, Screen.height / 2);
								ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive (true);
							}
						}
				} 
			}

			ingameGlobalManager.instance.cameraViewer3D.clearInvestigateView ();
			activateIcons ();
		}
		if(!ingameGlobalManager.instance.b_focusModeIsActivated)		// If focus mode is not activated
			InitCanvasPlayerInfos ();

	}

//--> Hide Multi page viewer and activate UI Icons
	public void hideMultiPage() {
		if (ingameGlobalManager.instance.canvasMainMenu) {
			//CanvasGroup Game_ObjectReader = null;
			for (var i = 0; i < ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas.Count; i++) {
				if (ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas [i].name == "Game") {
					ingameGlobalManager.instance.canvasMainMenu.GoToOtherPage (ingameGlobalManager.instance.canvasMainMenu.List_GroupCanvas [i]);
					activateIcons ();
					break;
				}
			}
		}
		if(!ingameGlobalManager.instance.b_focusModeIsActivated)		// If focus mode is not activated
			InitCanvasPlayerInfos ();


	}

//--> Init canvas Canvas_PlayerInfos
	private void InitCanvasPlayerInfos(){
		ingameGlobalManager.instance.b_AllowCharacterMovment = true;

		if (!ingameGlobalManager.instance.canvasMobileInputs.activeSelf
		    && !ingameGlobalManager.instance.b_DesktopInputs) {
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);
		}
        else if (!ingameGlobalManager.instance.reticule.activeSelf && ingameGlobalManager.instance.b_DesktopInputs) {
			ingameGlobalManager.instance.StartCoroutine( ingameGlobalManager.instance.changeLockStateLock ());

			ingameGlobalManager.instance.reticule.SetActive (true);
		}
	}

	public void MovementEnded_ActivateIcon(){
		StartCoroutine (I_MovementEnded_ActivateIcon ());
	}

	public IEnumerator I_MovementEnded_ActivateIcon(){
		var t = 0f;
        if (ingameGlobalManager.instance.navigationList.Count > 0 && ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "Focus"){
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateInteractiveIcons();
           // Debug.Log("Focus");
        }
           
        else{
            ingameGlobalManager.instance.canvasPlayerInfos.activateInteractiveIcons();
         //Debug.Log("End Focus");
        }

        while(t < .1f)
		{
			if (!ingameGlobalManager.instance.b_Ingame_Pause) {
				t += Time.deltaTime;
			}
			yield return null;
		}


		activateIcons ();

	}


//--> Display available actions on screen
	public void displayAvailableActionOnScreen(bool b_LeftClic,bool b_RightClic){
		if (ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick) {		// Keyboard Case
			btn_LeftClic.SetActive (b_LeftClic);
			btn_RightClic.SetActive (b_RightClic);

			if (btn_FingerSlide.activeSelf) {
				btn_FingerSlide.SetActive (false);
				btn_FingerDoubleTap.SetActive (false);
			}

			if (btn_JoystickBackBouton.activeSelf) {
				btn_JoystickBackBouton.SetActive (false);
				btn_JoystickStick.SetActive (false);
			}
		} 
		else if (ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.b_Joystick) {	// Joystick Case
			if (btn_LeftClic.activeSelf) {
				btn_LeftClic.SetActive (false);
				btn_RightClic.SetActive (false);
			}
			if (btn_FingerSlide.activeSelf) {
				btn_FingerSlide.SetActive (false);
				btn_FingerDoubleTap.SetActive (false);
			}


			btn_JoystickStick.SetActive (b_LeftClic);
			btn_JoystickBackBouton.SetActive (b_RightClic);
		} 
		else {																								// Mobile Case
			if (btn_LeftClic.activeSelf) {
				btn_LeftClic.SetActive (false);
				btn_RightClic.SetActive (false);
			}

			if (btn_JoystickBackBouton.activeSelf) {
				btn_JoystickBackBouton.SetActive (false);
				btn_JoystickStick.SetActive (false);
			}

			btn_FingerSlide.SetActive (b_LeftClic);
			btn_FingerDoubleTap.SetActive (b_RightClic);
		}

	}

	//--> Display available actions on screen
	public void displayAvailableActionOnScreen(bool b_RightClic,string name){
		if (ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick) {		// Keyboard Case
            if(btn_RightClic_OnlyMainMenu)btn_RightClic_OnlyMainMenu.SetActive (b_RightClic);
            if (btn_FingerDoubleTap_OnlyMainMenu)btn_FingerDoubleTap_OnlyMainMenu.SetActive (false);
            if (btn_BackButton_Joystick_OnlyMainMenu)btn_BackButton_Joystick_OnlyMainMenu.SetActive (false);
		} 
		else if (ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.b_Joystick) {// Joystick Case
            if (btn_RightClic_OnlyMainMenu)btn_RightClic_OnlyMainMenu.SetActive (false);
            if (btn_FingerDoubleTap_OnlyMainMenu)btn_FingerDoubleTap_OnlyMainMenu.SetActive (false);
            if (btn_BackButton_Joystick_OnlyMainMenu)btn_BackButton_Joystick_OnlyMainMenu.SetActive (b_RightClic);
		} 
		else {																								// Mobile Case
            if (btn_FingerDoubleTap_OnlyMainMenu)btn_FingerDoubleTap_OnlyMainMenu.SetActive (b_RightClic);
            if (btn_RightClic_OnlyMainMenu)btn_RightClic_OnlyMainMenu.SetActive (false);
            if (btn_BackButton_Joystick_OnlyMainMenu)btn_BackButton_Joystick_OnlyMainMenu.SetActive (false);
		}

	}
	//


	public void F_rendererState(GameObject obj,bool b_state){
		Transform[] children = obj.GetComponentsInChildren<Transform> ();

		foreach(Transform child in children ){
			if (child.gameObject.GetComponent<Renderer> ()) {
				//Debug.Log (child.name);
				child.gameObject.GetComponent<Renderer> ().enabled = b_state;

			}
		}
	}


    public void MobileExitPuzzle(){
      ingameGlobalManager.instance._backInputs.ExitFocusMode();
    }


    public void deactivateObjPuzzleNotAvailable(){
        if(obj_PuzzleNotAvailable.activeInHierarchy)
            b_rememberLastState = true;
        else
            b_rememberLastState = false;
        
        obj_PuzzleNotAvailable.SetActive(false);

    }
    public void activateObjPuzzleNotAvailable()
    {
        obj_PuzzleNotAvailable.SetActive(b_rememberLastState);
        b_rememberLastState = false;
    }

    public void deactivateObjResetPuzzle()
    {
        if (obj_ResetPuzzle.activeInHierarchy)
            b_rememberLastStateResetPuzzle = true;
        else
            b_rememberLastStateResetPuzzle = false;

        obj_ResetPuzzle.SetActive(false);

    }
    public void activateObjResetPuzzle()
    {
        obj_ResetPuzzle.SetActive(b_rememberLastStateResetPuzzle);
        b_rememberLastStateResetPuzzle = false;
    }


    public void deactivateObjTitle()
    {
        if (txt3DViewer.gameObject.activeInHierarchy)
            b_rememberLastStatetxt3DViewer = true;
        else
            b_rememberLastStatetxt3DViewer = false;

        txt3DViewer.gameObject.SetActive(false);

    }
    public void activateObjTitle()
    {
        txt3DViewer.gameObject.SetActive(b_rememberLastStatetxt3DViewer);
        b_rememberLastStatetxt3DViewer = false;
    }

   
}
