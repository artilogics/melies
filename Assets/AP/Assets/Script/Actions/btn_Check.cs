// Description : btn_Check : use to manage the canvas Icons. (Check,Focus...) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class btn_Check : MonoBehaviour {

	public GameObject 				objRef;
	public int 						index;

	public Text 					descriptionTxt;
	private variousVoiceOverMethods variousVoiceOverMethods;
	private bool 					b_showTitle = false;

	// Use this for initialization
	void Start () {
		//rend = GetComponent<Renderer> ();

	}

	public void SetupButton(GameObject newRef,int newIndex,bool b_showTitleOnUIButton){
		//Debug.Log ("here newRef : " + newRef+ ", newIndex: " + newIndex + ", b_showTitleOnUIButton: " + b_showTitleOnUIButton);
		b_showTitle = b_showTitleOnUIButton;
		objRef = newRef;
		index = newIndex;
		if (b_showTitleOnUIButton) {
            if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties>() && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentInventory)
				descriptionTxt.text = ingameGlobalManager.instance.currentInventory.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
            else if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties>() && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentDiary)
				descriptionTxt.text = ingameGlobalManager.instance.currentDiary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
            else if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties>() && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentFeedback)
				descriptionTxt.text = ingameGlobalManager.instance.currentFeedback.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
        } else if (descriptionTxt){
			descriptionTxt.text = "";
		}
	}
	
//--> Use for GamObjects with the tag Item : Display 3D viewer or the multi page viewer
	public void VisualizeGameobject () {
		ingameGlobalManager gManager = ingameGlobalManager.instance;

	//-> Check if a voice need to be played
		if (objRef.GetComponent<VoiceProperties> ()	
			&& objRef.GetComponent<VoiceProperties> ().textList == gManager.currentDiary) { 
			variousVoiceOverMethods = new variousVoiceOverMethods();
			variousVoiceOverMethods.voiceOver (objRef);
		}



		gManager.GetComponent<mobileInputsFingerMovement> ().initDoubleTap();
	//-> Display 3D Viewer
		if (gManager.cameraViewer3D) {
			//-> Type : Object 3D Viewer
			//Debug.Log(objRef.name);
			if (objRef.GetComponent<TextProperties>().textList == gManager.currentInventory) { 
				if (gManager.currentInventory.diaryList [0]._languageSlot [index].itemType == 0) {

				//-> Add the object in the inventory if needed
					addEntryToTheInventory ();

				//-> Activated the 3D viewer Camera
					gManager.cameraViewer3D.gameObject.SetActive (true);

				//-> If a no prefab is saved for this object in w_Inventory, the gameObject selected in scene view is used. 
					GameObject tmpObj = objRef;
				//-> If a prefab is saved for this object in w_Inventory, the prefab is displayed on screen. 
					if (gManager.currentInventory.diaryList [0]._languageSlot [index].refGameObject != null)	
						objRef = gManager.currentInventory.diaryList [0]._languageSlot [index].refGameObject;	

				//-> Create the visualization in the 3D viewer
					gManager.cameraViewer3D.newObjectToInvestigate (
						objRef,
						gManager.currentInventory.diaryList [0]._languageSlot [index].prefabSizeInViewer,
						gManager.currentInventory.diaryList [0]._languageSlot [index].prefabRotationInViewer,
                        true,
                        false
					);

					if (gManager.canvasPlayerInfos.txt3DViewer) {
                        if (!gManager.canvasPlayerInfos.txt3DViewer.gameObject.activeSelf)
                            gManager.canvasPlayerInfos.txt3DViewer.gameObject.SetActive(true);
						gManager.canvasPlayerInfos.txt3DViewer.text = gManager.currentInventory.diaryList [gManager.currentLanguage]._languageSlot [index].diaryTitle [0];
					}
						
				//-> Deactivate the gameObject and his children selected in scene view. 
					tmpObj.GetComponent<Renderer> ().enabled = false;
					Transform[] children = tmpObj.GetComponentsInChildren<Transform> ();

					foreach(Transform child in children ){
						if(child.gameObject.GetComponent<Renderer> ())
						    child.gameObject.GetComponent<Renderer> ().enabled = false;
                        else
                            child.gameObject.SetActive(false);
					}
						
					if (gManager.canvasPlayerInfos) {
				//-> Deactivate intractions icons on screen
						gManager.canvasPlayerInfos.deactivateIcons (false);

				//-> Deactivate Mobile Inputs
						if (gManager.canvasMobileInputs && gManager.canvasMobileInputs.activeSelf)
							gManager.canvasMobileInputs.SetActive (false);
				//-> Deactivate reticule
                        if (gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
							gManager.reticule.SetActive (false);

				//-> Change Lock state to Confined
						gManager.StartCoroutine (gManager.changeLockStateConfined (false));

				//-> Update the navigation List
						gManager.navigationList.Add ("Viewer3D");

				//--> Display available UI actions on screen
						//gManager.canvasPlayerInfos.displayAvailableActionOnScreen (true, true);
                        gManager.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(true, true);
						gManager.audioMenuClips.playASound (2);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)
					}
				}
			}
		//-> Type Multi Page
			else {						
				if (gManager.canvasMainMenu) {
					CanvasGroup Game_ObjectReader = null;
				//-> open the reader interface
					for (var i = 0; i < gManager.canvasMainMenu.List_GroupCanvas.Count; i++) {
						if (gManager.canvasMainMenu.List_GroupCanvas [i].name == "Game_ObjectReader") {
							Game_ObjectReader = gManager.canvasMainMenu.List_GroupCanvas [i];
							break;
						}
						
					}
					if (Game_ObjectReader) {
					//-> Add the object in the diary if needed
						TextList currentTextList = gManager.currentInventory;
						if (objRef.GetComponent<TextProperties> ().textList == gManager.currentDiary) {
							addEntryToTheDiary ();
							currentTextList = gManager.currentDiary;
						}
							
                        ingameGlobalManager.instance._joystickReticule.newPosition(Screen.width / 2, Screen.height / 2);

                        if (gManager.reticuleJoystickImage && gManager.b_Joystick)
                            gManager.reticuleJoystickImage.gameObject.SetActive(true);

					//-> Deactivate charcter movement
						gManager.b_AllowCharacterMovment = false;
					//-> Show diary entry in the scene view
						gManager.Game_ObjectReader.showDiaryEntry(index,currentTextList);
						gManager.canvasMainMenu.GoToOtherPage (Game_ObjectReader);
					//-> Enable to move the mouse on screen to select an item when focus is activated
						gManager.StartCoroutine( gManager.changeLockStateConfined (true));			
					}

					//-> Deactivate the gameObject and his children selected in scene view. 
					objRef.GetComponent<Renderer>().enabled = false;
					Transform[] children = objRef.GetComponentsInChildren<Transform> ();

					foreach(Transform child in children ){
						if(child.gameObject.GetComponent<Renderer> ())
							child.gameObject.GetComponent<Renderer> ().enabled = false;
					}


					if (gManager.canvasPlayerInfos) {
					//-> deactivate UI Icons
						gManager.canvasPlayerInfos.deactivateIcons (false);

					//-> Deactivate Mobile Inputs
						if(gManager.canvasMobileInputs && gManager.canvasMobileInputs.activeSelf)
							gManager.canvasMobileInputs.SetActive (false);
					
					//-> Deactivate the reticule
                        if(gManager.reticule && gManager.reticule.activeSelf && gManager.b_DesktopInputs)
							gManager.reticule.SetActive (false);
					}

				//-> Update the navigation List
					gManager.navigationList.Add("MultiPages");

				//--> Display available UI actions on screen
					//gManager.canvasPlayerInfos.displayAvailableActionOnScreen (false, true);
                    gManager.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
					gManager.audioMenuClips.playASound (3);							// Play sound (Hierarchy : ingameGlobalManager -> audioMenu)

				}
			}

		} else {
			Debug.Log ("Info : You need to connect cameraShowObject to the ingameGlobalManager");
		}
		}


//--> Use for GamObjects with the InteractObject Item : Drawer, door
	public void ObjTranslateRotate(){
		ingameGlobalManager.instance.GetComponent<mobileInputsFingerMovement> ().initDoubleTap();

        GameObject tmpObj = objRef;
        while (!tmpObj.GetComponent<objTranslateRotate>())          // Find the parent with the script objTranslateRotate attached to it
        {tmpObj = tmpObj.transform.parent.gameObject;}


        if (tmpObj.transform.GetComponent<objTranslateRotate>())
        {
            if (tmpObj.transform.GetComponent<objTranslateRotate>().b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs
                || tmpObj.transform.GetComponent<objTranslateRotate>().b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs)
            {
                ingameGlobalManager.instance.currentobjTranslateRotate = tmpObj.transform.GetComponent<objTranslateRotate>();
            }
            tmpObj.transform.GetComponent<objTranslateRotate>().MoveObject();
        }
	}
		

//--> Add an object in the current player Inventory
	private void addEntryToTheInventory(){
		ingameGlobalManager gManager = ingameGlobalManager.instance;
		//-> Add the entry in the diary if needed 
		int managerID = index;
		if(gManager.currentInventory.diaryList [0]._languageSlot [index].showInInventory){
			bool result = true;
			for (var i = 0; i < ingameGlobalManager.instance.currentPlayerInventoryList.Count; i++) {
				if (index == gManager.currentPlayerInventoryList [i]) {
					result = false;
					break;
				}
			}
			if (result) {
				//--> Display available UI actions on screen
				gManager.canvasPlayerInfos._infoUI.playAnimInfo(gManager.currentInventory.diaryList [gManager.currentLanguage]._languageSlot [index].diaryTitle[0],"Inventory");
				gManager.currentPlayerInventoryList.Add (managerID);
                gManager.currentPlayerInventoryObjectVisibleList.Add(true);     // Object Visible in the Inventory Window
			}
		}
		//Debug.Log("New Item");
	}

//--> Add an object in the current player Diary
	private void addEntryToTheDiary(){

		ingameGlobalManager gManager = ingameGlobalManager.instance;

		//-> Add the entry in the diary if needed 
		int managerID = index;
		if(gManager.currentDiary.diaryList [0]._languageSlot [index].showInInventory){
			bool result = true;
			for (var i = 0; i < ingameGlobalManager.instance.currentPlayerDiaryList.Count; i++) {
				if (index == gManager.currentPlayerDiaryList [i]) {
					result = false;
					break;
				}
			}
			if (result) {
				//--> Display available UI actions on screen
				gManager.canvasPlayerInfos._infoUI.playAnimInfo(gManager.currentDiary.diaryList [gManager.currentLanguage]._languageSlot [index].diaryTitle[0],"Diary");
				gManager.currentPlayerDiaryList.Add (managerID);
			}
		}
		//Debug.Log("New page in the diary");
	}



//--> Update Text when language is change in Menu Options
	public void updateText(){
		if (b_showTitle) {
			if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentInventory)
				descriptionTxt.text = ingameGlobalManager.instance.currentInventory.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
			else if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentDiary)
				descriptionTxt.text = ingameGlobalManager.instance.currentDiary.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
			else if (descriptionTxt && index != -1 && objRef.GetComponent<TextProperties> ().textList == ingameGlobalManager.instance.currentFeedback)
				descriptionTxt.text = ingameGlobalManager.instance.currentFeedback.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [index].diaryTitle [0];
		} 
	}

}
