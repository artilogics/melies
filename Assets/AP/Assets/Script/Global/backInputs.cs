// Description : backInputs : Actions when when player press back button. Find this script on ingameGlobalManager
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class backInputs : MonoBehaviour {
	public bool 			            SeeInspector = false;

	public float 						backSoundVolume = .5f;
	public mobileInputsFingerMovement 	fingerMovement;
	private bool 						b_doubleTap = false;

	public int 							backButtonDesktop 	= 6;
	public int 							backButtonJoystick 	= 3;

	public int 							pauseButtonDesktop 	= 7;
    public bool                         b_DoubleTapMainMenu_OnMobile = false;

	// Update is called once per frame
	void Update () {
        if (ingameGlobalManager.instance.eventSys == null)
        {
            GameObject tmpEventSys = GameObject.Find("EventSystem");
            if (tmpEventSys)
                ingameGlobalManager.instance.eventSys = tmpEventSys.GetComponent<EventSystem>();
        }

        ingameGlobalManager gManager = ingameGlobalManager.instance;

        if (gManager.b_InputIsActivated)
        {
            if (!gManager.b_DesktopInputs
                && fingerMovement)
            {
                b_doubleTap = fingerMovement.checkDoubleTap();
            }

            //-> Back Inputs in scene name Main Menu
            if (SceneManager.GetActiveScene().buildIndex == 0)
                SceneMainMenu();
            //-> Back Inputs in the other scenes (Game scenes)
            else
                SceneExceptMainMenu();

            b_doubleTap = false;                                                                // Init double Tap

            joystickCheckIfButtonSelected();

            // Reactivate mouse inside Menu if the gamepad is activated
            if(Input.GetAxis("Mouse X") > .5f|| Input.GetAxis("Mouse Y") > .5f){        // Mouse is moving
                if(gManager.navigationList.Count > 0
                    && gManager.navigationList[0] == "MainMenuManager"
                    && gManager.b_Joystick 
                    && gManager.b_DesktopInputs 
                    && Cursor.visible == false){
                    
                    //Debug.Log("Mouse is Moving");
                    Cursor.visible = true;
                    gManager.canvasMainMenu.GetComponent<GraphicRaycaster>().enabled = true;
                }
                   
            }
        }
	}


	public void SceneExceptMainMenu(){
		if (ingameGlobalManager.instance.b_DesktopInputs
			&& ingameGlobalManager.instance.saveAndLoadManager.b_IngameDataHasBeenLoaded) {				// Game datas and parameters are loaded
			if (Input.GetKeyDown (KeyCode.Mouse1) 																				// Mouse right Click
				|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[backButtonDesktop]) 			// Keyboard Back Button	 : Default Escape
				|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[pauseButtonDesktop]) 			// Keyboard Pause Button : Default P
				|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringGamepadButton[backButtonJoystick])) {		// Joystick back button
				ingameGlobalManager gManager = ingameGlobalManager.instance;
				//-> Go back from ingame Multi pages Viewer to game
				if (gManager.navigationList.Count > 0) {
					if (gManager.navigationList [gManager.navigationList.Count - 1] == "MultiPages") {
						GoBackFromInGameMultiPageToGame ();
					} 
					//-> Go back from 3D Viewer to game
					else if (gManager.navigationList [gManager.navigationList.Count - 1] == "Viewer3D") {
						GoBackFrom3DViewerToGame ();
					}
					//-> Go back from ingame inventory 3D Viewer to all the inventory entry page
					else if (gManager.navigationList [gManager.navigationList.Count - 1] == "InventoryViewer3D") {
						GoBackFrom3DViewerToInventoryUI ();
					}
					//-> Exit Focus Mode
                    else if (gManager.navigationList [gManager.navigationList.Count - 1] == "Focus" && gManager._focusCamEffect.b_MovementEnded) {	
						ExitFocusMode ();
					}
					//-> Go back from diary specific ID to all the diary entry page
					else if (gManager.navigationList.Count > 0
						&& gManager.navigationList [gManager.navigationList.Count - 1] == "DiaryMultiPages") {	

						GoBackFromInGameMultiPageToDiaryUI ();
					}
					//-> Go back from diary to game
					else if (gManager.navigationList.Count > 0
						&& gManager.navigationList [gManager.navigationList.Count - 1] == "Diary") {	
						GoBackFromDiaryToGame();
					}
					//-> Go back from Inventory to game
					else if (gManager.navigationList.Count > 0
						&& gManager.navigationList [gManager.navigationList.Count - 1] == "Inventory") {	

						GoBackFromInventoryToGame ();
					}
                    else if (gManager.navigationList.Count > 0
                        && gManager.navigationList[gManager.navigationList.Count - 1] == "Clue")
                    {
                        ingameGlobalManager.instance.canvasPlayerInfos.obj_Grp_InfoPuzzle.GetComponent<AP_ClueButton>().AP_HideClueUI();
                    }
					//--> Case avalaible Only for Escape Button	
					else if (Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[backButtonDesktop]) 	// Keyboard Back Button	 : Default Escape 
						|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[pauseButtonDesktop])  	// Keyboard Pause Button : Default P
						|| Input.GetKeyDown (gManager.inputListOfStringGamepadButton[backButtonJoystick])) {						// Joystick back button					
						if (SceneManager.GetActiveScene ().buildIndex != 0) {								// Game Scenes
							if (gManager.navigationList.Count > 0
								&& gManager.navigationList [gManager.navigationList.Count - 1] == "MainMenuManager") {
								//Debug.Log ("here");
								//-> Hide Main Menu
								HideMainMenu();
							}
						}
					}
					else if (Input.GetKeyDown (KeyCode.Mouse1)) {																	// Mouse Right Click					
						if (SceneManager.GetActiveScene ().buildIndex != 0) {								// Game Scenes
							if (gManager.navigationList.Count > 0
								&& gManager.navigationList [gManager.navigationList.Count - 1] == "MainMenuManager") {
								//-> Hide Main Menu
								HideMainMenu();
							}
						} 
					}
				} 
				//-> Open Main Menu
				else if ((Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[backButtonDesktop]) 	// Keyboard Back Button	 : Default Escape 
					|| Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[pauseButtonDesktop])  	// Keyboard Pause Button : Default P
					|| Input.GetKeyDown (gManager.inputListOfStringGamepadButton[backButtonJoystick])) 	&&						// Joystick back button			 
					     gManager.navigationList.Count == 0 &&
                         !ingameGlobalManager.instance.b_focusModeIsActivated) {
					MainMenuIngame ();
				}
			}


		}


		//--> Mobile Part -- Begin --
		if (!ingameGlobalManager.instance.b_DesktopInputs
			&& fingerMovement) {
			//b_doubleTap = fingerMovement.checkDoubleTap ();										// Check double Tap

			if (ingameGlobalManager.instance.saveAndLoadManager.b_IngameDataHasBeenLoaded) {				// Game datas and parameters are loaded
				if (b_doubleTap) {
					ingameGlobalManager gManager = ingameGlobalManager.instance;
					//-> Go back from ingame Multi pages Viewer to game
					if (gManager.navigationList.Count > 0) {
						if (gManager.navigationList [gManager.navigationList.Count - 1] == "MultiPages") {
                            gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
							GoBackFromInGameMultiPageToGame ();
						} 
						//-> Go back from 3D Viewer to game
						else if (gManager.navigationList [gManager.navigationList.Count - 1] == "Viewer3D") {
                            gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
							GoBackFrom3DViewerToGame ();
						}
						//-> Go back from ingame inventory 3D Viewer to all the inventory entry page
						else if (gManager.navigationList [gManager.navigationList.Count - 1] == "InventoryViewer3D") {
                            gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
							GoBackFrom3DViewerToInventoryUI ();
						}
						//-> Exit Focus Mode
                        else if (gManager.navigationList [gManager.navigationList.Count - 1] == "Focus" && gManager._focusCamEffect.b_MovementEnded) {	
                            gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
                            Debug.Log("Exit Puzzle");
                            // Focus Activate for an object different to a puzzle
                            if (ingameGlobalManager.instance.currentPuzzle != null && 
                                ingameGlobalManager.instance.currentPuzzle.GetComponent<conditionsToAccessThePuzzle>().b_ActivateDoubleTapIcon
                               ||
                                ingameGlobalManager.instance.currentPuzzle == null
                               ) 
							ExitFocusMode ();
						}
						//-> Go back from diary specific ID to all the diary entry page
						else if (gManager.navigationList.Count > 0
							&& gManager.navigationList [gManager.navigationList.Count - 1] == "DiaryMultiPages") {	
                            gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
							GoBackFromInGameMultiPageToDiaryUI ();
						}
						else if (b_doubleTap) {						
							if (SceneManager.GetActiveScene ().buildIndex != 0) {								// Game Scenes
								//-> Go back from diary to game
								if (gManager.navigationList.Count > 0
									&& gManager.navigationList [gManager.navigationList.Count - 1] == "Diary") {	
                                    gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
									GoBackFromDiaryToGame ();
								}
								//-> Go back from Inventory to game
								else if (gManager.navigationList.Count > 0
									&& gManager.navigationList [gManager.navigationList.Count - 1] == "Inventory") {
                                    gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();

									GoBackFromInventoryToGame ();
								} else if (gManager.navigationList.Count > 0
									&& gManager.navigationList [gManager.navigationList.Count - 1] == "MainMenuManager") {
									//-> Hide Main Menu
									HideMainMenu ();
								}
                                else if (gManager.navigationList.Count > 0
                                    && gManager.navigationList[gManager.navigationList.Count - 1] == "Clue")
                                {
                                    ingameGlobalManager.instance.canvasPlayerInfos.obj_Grp_InfoPuzzle.GetComponent<AP_ClueButton>().AP_HideClueUI();
                                }
							} else {																			// Main Menu Scene

							}
						}

                    } else if (b_doubleTap && 
                               gManager.navigationList.Count == 0 &&
                               !ingameGlobalManager.instance.b_focusModeIsActivated &&
                               b_DoubleTapMainMenu_OnMobile) {
						MainMenuIngame ();
					}
				}
			}
		}

		SceneMainMenu ();
              
	}

	public void SceneMainMenu(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		if (Input.GetKeyDown (ingameGlobalManager.instance.inputListOfStringKeyboardButton[backButtonDesktop]) 		// Keyboard Back Button	 : Default Escape 
			|| Input.GetKeyDown (gManager.inputListOfStringGamepadButton[backButtonJoystick])
            || b_doubleTap) {	 				// Joystick back button								
			if (gManager.navigationList.Count > 0 && 
				(gManager.navigationList [gManager.navigationList.Count - 1] == "Credit"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "Options"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "SaveOrLoad"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "AreYouSure")) {
			//-> Go to Main Menu
				MMS_GoToMainMenu();
			} 

			if (gManager.navigationList.Count > 0 && 
				(gManager.navigationList [gManager.navigationList.Count - 1] == "SlotAlreadyExist")) {
				//-> Go to Main Menu
				MMS_GoToNewGame();
			} 


			if (gManager.navigationList.Count > 0 && 
				(gManager.navigationList [gManager.navigationList.Count - 1] == "Inputs"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "Quality"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "MenuAudio"
				|| gManager.navigationList [gManager.navigationList.Count - 1] == "GameOptions")) {
				if (gManager.navigationList [gManager.navigationList.Count - 1] == "Inputs") {
					GameObject inputsManager = GameObject.Find ("InputsManager");
					if(inputsManager)
						inputsManager.GetComponent<MM_MenuInputs>().saveInputs();

				}

				//-> Go to Main Menu
				MMS_GoToOptionsMenu();
			} 
		}
	}




//-> Show Main Menu ingame
	public void MainMenuIngame(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		if (gManager.currentPlayer.GetComponent<characterMovement>().isOnFloor)
        {
			if (ingameGlobalManager.instance.canvasMobileInputs
				&& ingameGlobalManager.instance.canvasMobileInputs.activeSelf) 
				ingameGlobalManager.instance.canvasMobileInputs.SetActive (false);

			ingameGlobalManager.instance.canvasPlayerInfos.gameObject.SetActive (false);
			gManager.audioMenuClips.playASound (0);		
			ingameGlobalManager.instance.onlyPauseGame ();

			GameObject mainMenuManager = GameObject.Find ("MainMenuManager");
			if (mainMenuManager) {
				
				if (mainMenuManager.GetComponent<MainMenu> ().txtValidation)
					mainMenuManager.GetComponent<MainMenu> ().txtValidation.text ="";
			}


			if (SceneManager.GetActiveScene ().buildIndex != 0) {								// Game Scenes
				ingameGlobalManager.instance.navigationList.Add ("MainMenuManager");

				//--> Display available actions on screen
		
				for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
					if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "MainMenu") {
						gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
						break;
					}
				}

				GameObject  tmp = GameObject.Find ("EventSystem");
				EventSystem eventSystem = tmp.GetComponent<EventSystem> ();
				// Keyboard
				if (!ingameGlobalManager.instance.b_Joystick) {
					Cursor.visible = true;
					gManager.b_currentCursorVisibility = Cursor.visible;
				} 
				// Joystick
				else {
					Cursor.visible = false;

					if (tmp && mainMenuManager) {
						//Debug.Log ("here");

						eventSystem.SetSelectedGameObject(null);
						eventSystem.SetSelectedGameObject(mainMenuManager.GetComponent<MainMenu> ().btnResumeMainMenu);

						StandaloneInputModule standInputModule = tmp.GetComponent<StandaloneInputModule> ();

						standInputModule.horizontalAxis = "MenuNagHorizontal";
						standInputModule.verticalAxis 	= "MenuNagVertical";
						standInputModule.submitButton 	= "Submit";
						standInputModule.cancelButton 	= "Cancel";
					}
				}


				gManager.StartCoroutine (gManager.changeLockStateConfined (true));
				if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
					gManager.reticule.SetActive (false);


				if (gManager.reticuleJoystickImage && gManager.reticuleJoystickImage.gameObject.activeSelf) {
					gManager._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);
					gManager.reticuleJoystickImage.gameObject.SetActive(false);

					
				}


				gManager.canvasPlayerInfos.deactivateIcons (false);


				if (eventSystem && ingameGlobalManager.instance.canvasMainMenu.eventSysFirstSelectedGameObject != null) {
					eventSystem.SetSelectedGameObject (ingameGlobalManager.instance.canvasMainMenu.eventSysFirstSelectedGameObject);
				}

				if(!ingameGlobalManager.instance.b_DesktopInputs){
				gManager.currentPlayer.GetComponent<characterMovement>().StopMoving();
				}

			}
		}
	}
		
//-> Go back from 3D Viewer to game
	private void GoBackFrom3DViewerToGame(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);
		gManager.canvasPlayerInfos.hideViewer ();

		//--> Display available actions on screen
		if (!gManager.b_focusModeIsActivated) {
			//gManager.canvasPlayerInfos.displayAvailableActionOnScreen (false, false);
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
			
		} else {
			//gManager.canvasPlayerInfos.displayAvailableActionOnScreen (false, true);
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
			if (gManager.b_Joystick && gManager.reticuleJoystickImage && !gManager.reticuleJoystickImage.gameObject.activeSelf) {
                gManager._joystickReticule.newPosition (Screen.width / 2, Screen.height / 2);
				gManager.reticuleJoystickImage.gameObject.SetActive (true);
			}
		}

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
		if (gManager.canvasPlayerInfos.txt3DViewer) {
			gManager.canvasPlayerInfos.txt3DViewer.text = "";}

        //ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();

        gManager.onlyUnPauseGame();
	}
		
	private void GoBackFromInGameMultiPageToGame (){

		ingameGlobalManager gManager = ingameGlobalManager.instance;

		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);
		gManager.canvasPlayerInfos.hideMultiPage ();

		//--> Display available actions on screen
		if (!gManager.b_focusModeIsActivated)
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
		else {
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
			if (gManager.b_Joystick && gManager.reticuleJoystickImage && !gManager.reticuleJoystickImage.gameObject.activeSelf) {
                gManager._joystickReticule.newPosition (Screen.width / 2, Screen.height / 2);
				gManager.reticuleJoystickImage.gameObject.SetActive (true);
			}
		}

        ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();

        //if (gManager.reticuleJoystickImage && gManager.b_Joystick)
          //  gManager.reticuleJoystickImage.gameObject.SetActive(false);

        if (gManager.reticuleJoystickImage &&
              ingameGlobalManager.instance.currentPuzzle &&
              !ingameGlobalManager.instance.currentPuzzle.GetComponent<focusOnly>() &&
              ingameGlobalManager.instance.b_Joystick)
        {
            if (!ingameGlobalManager.instance.canvasPlayerInfos.b_rememberLastState)
                gManager.reticuleJoystickImage.gameObject.SetActive(true);
            else
                gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }
        //-> Focus in a wardrobe or drawer
        else if (ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                ingameGlobalManager.instance.b_Joystick)
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(true);
        }
        else
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
        gManager.onlyUnPauseGame();
	}


	private void GoBackFrom3DViewerToInventoryUI (){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		GameObject objInvestigationCam = GameObject.Find ("CamShowObject");

		if (objInvestigationCam) {
			objInvestigationCam.GetComponent<investigationCam> ().clearInvestigateView ();
		}

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Inventory") {
				GameObject obj = gManager.canvasMainMenu.List_GroupCanvas [i].gameObject;
				Transform[] allTransform = obj.GetComponentsInChildren<Transform> (true);


				for (var j = 0; j < allTransform.Length; j++) {
					if (allTransform [j].name == "BlackScreen") {
						allTransform [j].gameObject.SetActive (false);
						break;
					}

                   
				}
			}
		}

        GameObject inventoryObj = GameObject.FindWithTag("inventoryEntry");
        if(inventoryObj){
            foreach (Image objs in inventoryObj.GetComponent<inventoryALLEntry>().listEntry)
            {
                objs.gameObject.transform.parent.gameObject.SetActive(true);
            }
        }


		//--> Display available actions on screen
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);


		// Desktop
		if (!gManager.b_Joystick) {
			Cursor.visible = true;
			gManager.b_currentCursorVisibility = Cursor.visible;
		} 
		// Joystick
		else {
			if (gManager.reticuleJoystickImage && !gManager.reticuleJoystickImage.gameObject.activeSelf) {
                gManager._joystickReticule.newPosition (Screen.width / 2, Screen.height / 2);
				gManager.reticuleJoystickImage.gameObject.SetActive (true);
			}
		}


      /*  if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Focus") {
            //Cursor.visible = true;
            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
            {
                gManager.b_currentCursorVisibility = Cursor.visible;
                gManager.StartCoroutine(gManager.changeLockStateConfined(true));
            }

            
            
            gManager.b_AllowCharacterMovment = false;
            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
                gManager.reticule.SetActive (false);
        } else {
            gManager.StartCoroutine (gManager.changeLockStateLock ());
            gManager.b_AllowCharacterMovment = true;
            if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
                gManager.reticule.SetActive (true);
        }*/





		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
		//if (gManager.canvasPlayerInfos.txt3DViewer) {
		//	gManager.canvasPlayerInfos.txt3DViewer.text = "";}
	}

	public void ExitFocusMode (){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

        if (gManager.currentPuzzle && gManager.currentPuzzle.GetComponent<AP_.DragAndDrop>())     // Deactivate Hand Icons for puzzles using drag and drop script
            gManager.currentPuzzle.GetComponent<AP_.DragAndDrop>().initAllSpriteWhenPuzzleIsSolved();


		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);
        if(gManager.currentobjTranslateRotate != null)                          // Actually focus in drawer or door is activated
		    gManager.currentobjTranslateRotate.MoveObject ();
        else{                                                                   // Actually focus in puzzle is activated
            gManager.gameObject.GetComponent<focusCamEffect>().MoveCameraToDefaultPosition();
            if(gManager.currentPuzzle)
            gManager.currentPuzzle.F_PuzzleInsideDoorOrDrawerExit(null);
           
        }

        gManager.currentPuzzle = null;
		gManager.currentFocusedGameObject = null;
        gManager.b_dragAndDropActivated = false;

		//--> Display available actions on screen
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);

	}

	private void GoBackFromInGameMultiPageToDiaryUI (){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Diary_All_Entry") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);


		diaryALLEntry diaryAllEntryManager = GameObject.Find ("diaryAllEntryManager").GetComponent<diaryALLEntry> ();

		if (diaryAllEntryManager && diaryAllEntryManager.nextPageReader) 
			diaryAllEntryManager.nextPageReader.SetActive(false);
		if (diaryAllEntryManager && diaryAllEntryManager.lastPageReader) 
			diaryAllEntryManager.lastPageReader.SetActive(false);
	}

	private void GoBackFromDiaryToGame(){
		if (ingameGlobalManager.instance.canvasMobileInputs
			&& !ingameGlobalManager.instance.canvasMobileInputs.activeSelf
			&& !ingameGlobalManager.instance.b_DesktopInputs) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);

		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Game") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}

		if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Focus") {
            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
            {
                gManager.b_currentCursorVisibility = Cursor.visible;
                gManager.StartCoroutine(gManager.changeLockStateConfined(true));
                gManager.b_AllowCharacterMovment = false;
            }


            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (false);
		} else {
			gManager.StartCoroutine (gManager.changeLockStateLock ());
			gManager.b_AllowCharacterMovment = true;
            if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (true);
		}

        if (!gManager.canvasPlayerInfos.gameObject.activeSelf)
            gManager.canvasPlayerInfos.gameObject.SetActive(true);

		gManager.canvasPlayerInfos.activateIcons ();

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
        gManager.onlyUnPauseGame();


        if (gManager.reticuleJoystickImage &&
              ingameGlobalManager.instance.currentPuzzle &&
              !ingameGlobalManager.instance.currentPuzzle.GetComponent<focusOnly>() &&
              ingameGlobalManager.instance.b_Joystick)
        {
            if (!ingameGlobalManager.instance.canvasPlayerInfos.b_rememberLastState)
                gManager.reticuleJoystickImage.gameObject.SetActive(true);
            else
                gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }
        //-> Focus in a wardrobe or drawer
        else if (ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                ingameGlobalManager.instance.b_Joystick)
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(true);
        }
        else
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }



        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
       
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();


		EnableAllPuzzleRefPosHand();

	}

	private void GoBackFromInventoryToGame (){
		if (ingameGlobalManager.instance.canvasMobileInputs
			&& !ingameGlobalManager.instance.canvasMobileInputs.activeSelf
			&& !ingameGlobalManager.instance.b_DesktopInputs) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);

		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Game") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}

		if (gManager.navigationList.Count > 0 && gManager.navigationList [gManager.navigationList.Count - 1] == "Focus") {
			//Cursor.visible = true;
            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
            {
                gManager.b_currentCursorVisibility = Cursor.visible;
                gManager.StartCoroutine(gManager.changeLockStateConfined(true));
            }

			
			
            gManager.b_AllowCharacterMovment = false;
            if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (false);
		} else {
			gManager.StartCoroutine (gManager.changeLockStateLock ());
			gManager.b_AllowCharacterMovment = true;
            if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
				gManager.reticule.SetActive (true);
		}






        if (!gManager.canvasPlayerInfos.gameObject.activeSelf)
            gManager.canvasPlayerInfos.gameObject.SetActive(true);

		gManager.canvasPlayerInfos.activateIcons ();

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
        gManager.onlyUnPauseGame();
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);


        if (gManager.reticuleJoystickImage &&
               ingameGlobalManager.instance.currentPuzzle &&
               !ingameGlobalManager.instance.currentPuzzle.GetComponent<focusOnly>() &&
               ingameGlobalManager.instance.b_Joystick)
        {
            if (!ingameGlobalManager.instance.canvasPlayerInfos.b_rememberLastState)
                gManager.reticuleJoystickImage.gameObject.SetActive(true);
            else
                gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }
        //-> Focus in a wardrobe or drawer
        else if (ingameGlobalManager.instance.currentobjTranslateRotate != null &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                ingameGlobalManager.instance.b_Joystick)
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(true);
        }
        else
        {
            gManager.reticuleJoystickImage.gameObject.SetActive(false);
        }

        ingameGlobalManager.instance.canvasPlayerInfos.activateObjTitle();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjPuzzleNotAvailable();
        ingameGlobalManager.instance.canvasPlayerInfos.activateObjResetPuzzle();
        ingameGlobalManager.instance.voiceOverManager.activateObSubtitle();



		EnableAllPuzzleRefPosHand();


	}



	public void HideMainMenu(){
		if (ingameGlobalManager.instance.canvasMobileInputs
			&& !ingameGlobalManager.instance.canvasMobileInputs.activeSelf
			&& !ingameGlobalManager.instance.b_DesktopInputs) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);

		GameObject mainMenuManager = GameObject.Find ("MainMenuManager");
		if (mainMenuManager) {
			if (mainMenuManager.GetComponent<MainMenu> ().txtValidation)
				mainMenuManager.GetComponent<MainMenu> ().txtValidation.text ="";
		}

		ingameGlobalManager.instance.canvasPlayerInfos.gameObject.SetActive (true);


		//--> Display available actions on screen
		ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen (false,"MainMenu");
       

		ingameGlobalManager gManager = ingameGlobalManager.instance;
		gManager.StartCoroutine (gManager.changeLockStateLock ());
		gManager.b_AllowCharacterMovment = true;
        if (gManager.reticule && !gManager.reticule.activeSelf && gManager.b_DesktopInputs)
			gManager.reticule.SetActive (true);


		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

        // Deactivate mouse cursor if gamepad is selected 
        if(gManager.b_Joystick 
            && gManager.b_DesktopInputs
            && Cursor.visible){
            Cursor.visible = false;
            gManager.canvasMainMenu.GetComponent<GraphicRaycaster>().enabled = false;  
        }
       

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Game") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}

        if (!gManager.canvasPlayerInfos.gameObject.activeSelf)
            gManager.canvasPlayerInfos.gameObject.SetActive(true);
		gManager.canvasPlayerInfos.activateIcons ();

		gManager.audioMenuClips.playASound (0);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)

     
		ingameGlobalManager.instance.eventSys.GetComponent<EventSystem> ().SetSelectedGameObject(null);

        ingameGlobalManager.instance.onlyUnPauseGame ();
	}





//--> Section Main Menu Scene -- Begin --

	public void MMS_GoToMainMenu(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		gManager.audioMenuClips.playASound (0);		

		// Game Scenes
			gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

			if (gManager.canvasMainMenu == null) {
				GameObject canvasMainMenu = GameObject.Find ("Canvas_MainMenu");
				gManager.canvasMainMenu = canvasMainMenu.GetComponent<Menu_Manager> ();
			}

			for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
				if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "MainMenu") {
					gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
					break;
				}
			}
		UI_NavigationUIButton ();

	}

	public void MMS_GoToOptionsMenu(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		gManager.audioMenuClips.playASound (0);		


		GameObject tmp = GameObject.Find ("Inputs");
		if(tmp)
			tmp.GetComponent<MM_MenuInputs>().saveInputs ();

		// Game Scenes
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		if (gManager.canvasMainMenu == null) {
			GameObject canvasMainMenu = GameObject.Find ("Canvas_MainMenu");
			gManager.canvasMainMenu = canvasMainMenu.GetComponent<Menu_Manager> ();
		}

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "OptionsMenu") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}
		UI_NavigationUIButton ();

	}



	public void UI_NavigationUIButton(){
		StartCoroutine (UI_Nav());
	}

	IEnumerator UI_Nav(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		yield return new WaitForEndOfFrame ();
		GameObject  tmp = GameObject.Find ("EventSystem");
		EventSystem eventSystem = tmp.GetComponent<EventSystem> ();
		if (eventSystem) {		
			gManager.lastUIButtonSelected = gManager.navigationButtonList [gManager.navigationButtonList.Count - 1];
			eventSystem.SetSelectedGameObject (gManager.lastUIButtonSelected);

		} 
		gManager.navigationButtonList.RemoveAt (gManager.navigationButtonList.Count - 1);
	}


	public void MMS_GoToNewGame(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;

		gManager.audioMenuClips.playASound (0);		

		// Game Scenes
		gManager.navigationList.RemoveAt (gManager.navigationList.Count - 1);

		if (gManager.canvasMainMenu == null) {
			GameObject canvasMainMenu = GameObject.Find ("Canvas_MainMenu");
			gManager.canvasMainMenu = canvasMainMenu.GetComponent<Menu_Manager> ();
		}

		for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
			if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "NewGameMenu") {
				gManager.canvasMainMenu.GoToOtherPage (gManager.canvasMainMenu.List_GroupCanvas [i]);
				break;
			}
		}

		UI_NavigationUIButton ();
	}

	public void joystickCheckIfButtonSelected (){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
        if (!gManager.b_focusModeIsActivated)
        {
			if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
			{
				if (gManager.navigationList.Count > 0 &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "Clue" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "InventoryViewer3D" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "Inventory" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "Diary" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "DiaryMultiPages" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "MultiPages" &&
					gManager.navigationList[gManager.navigationList.Count - 1] != "Viewer3D")
				{                         // The player is not in the Clue menu
					if (gManager.b_Ingame_Pause || SceneManager.GetActiveScene().buildIndex == 0)
					{       // Game is paused or player are in scene 0 (Scene Main Menu)
						GameObject tmp = GameObject.Find("EventSystem");

						if (tmp)
						{
							EventSystem eventSystem = tmp.GetComponent<EventSystem>();
							if (eventSystem.currentSelectedGameObject == null && gManager.lastUIButtonSelected != null)
							{
								eventSystem.SetSelectedGameObject(gManager.lastUIButtonSelected);
							}
							else if (eventSystem.currentSelectedGameObject == null && gManager.canvasMainMenu.eventSysFirstSelectedGameObject)
							{
								eventSystem.SetSelectedGameObject(gManager.canvasMainMenu.eventSysFirstSelectedGameObject);
							}
						}
					}

					//Debug.Log("Here");
				}
			}
		}	
	}


	public void EnableAllPuzzleRefPosHand()
	{
		initRefPositionSprite[] initRefPositionSpriteList = FindObjectsByType<initRefPositionSprite>(FindObjectsSortMode.None);

		foreach (var child in initRefPositionSpriteList)
			child.GetComponent<SpriteRenderer>().enabled = true;

		//Debug.Log("Enable Sprite");
	}




}
