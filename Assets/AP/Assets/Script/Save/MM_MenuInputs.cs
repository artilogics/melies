// Description : MM_MenuInputs : Manager the input remap manager Menu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MM_MenuInputs : MonoBehaviour {
	public bool 			SeeInspector = false;

	public GameObject 				btnKeyboard;
	public GameObject			 	btnJoystick;
	public GameObject 				selectedInputType;
	public GameObject 				keyboardPanel;
	public GameObject 				joystickPanel;

	private EventSystem 			eventSystem;
	private StandaloneInputModule	standInputModule;
	public MainMenu					mainMenu;

	[System.Serializable]
	public class RemapButtons
	{
		public List<GameObject> 	buttonsList 	= new List<GameObject>();
		public List<string> 		inputNameList 	= new List<string>();
		public List<bool> 			stateList 		= new List<bool>();
		public List<bool> 			b_Axis 			= new List<bool>();
		public List<string> 		defaultNamePC	= new List<string>();
		public List<string> 		defaultNameMac	= new List<string>();

	}

	[SerializeField]
	public List<RemapButtons> 		remapButtons;


	[System.Serializable]
	public class BoolValues
	{
		public List<GameObject> 	buttonsList 	= new List<GameObject>();
		public List<bool> 			b_Values		= new List<bool>();
	}

	[SerializeField]
	public List<BoolValues> 		boolValues;

	[System.Serializable]
	public class FloatValues
	{
		public List<GameObject> 	buttonsList 	= new List<GameObject>();
		public List<float> 			b_Values 		= new List<float>();
	}

	[SerializeField]
	public List<FloatValues> 		floatValues;

	public bool 					b_PC			= true;

	public int 						currentlySelected = -1;
	private int 					lastSelected = -1;



	public List<KeyCode> 			keyCodeList = new List<KeyCode>();
	public List<KeyCode> 			keyCodeNotAvailableList = new List<KeyCode>();
	public List<string> 			AxisList = new List<string>();


	public Text 					txt_PlayerInfo;
	private TextProperties 			txt_PlayerInfoTextProp;

    public GameObject K_ref_Button;
    public GameObject K_ref_Slider;
    public GameObject K_ref_Toggle;

    public GameObject J_ref_Button;
    public GameObject J_ref_Slider;
    public GameObject J_ref_Toggle;

	public void Start(){
		GameObject tmp = GameObject.Find ("EventSystem");

		if (tmp) {
			eventSystem = tmp.GetComponent<EventSystem> ();
			standInputModule = tmp.GetComponent<StandaloneInputModule> ();
		}
			
		if (txt_PlayerInfo)
			txt_PlayerInfoTextProp = txt_PlayerInfo.GetComponent<TextProperties> ();


		if (!PlayerPrefs.HasKey ("InputList")) {
			for (var i = 0; i < 4; i++) {
				defaultInputs (i);
			}
		} else {
			defaultInputsAlreadyExist ();
		}
	}



	public void Update(){
		bool b_Selected = false;
		int inputRemapType = 1;
		if (ingameGlobalManager.instance.b_Joystick)
			inputRemapType = 0;


		//-> Know the current selected button (Input Menu)
		for (var j = 0; j < 4; j++) {
			for (var i = 0; i < remapButtons [j].buttonsList.Count; i++) {
				if (remapButtons [j].buttonsList [i] != null && eventSystem.currentSelectedGameObject == remapButtons [j].buttonsList [i]) {
					b_Selected = true;
					currentlySelected = i;
					inputRemapType = j;
				} else {
					//Debug.Log ("T");
				}
			}
		}
		//-> If No button is selected
		if (!b_Selected)
			currentlySelected = -1;

		//-> If remap activated and the selected button change : Disable remap
		for (var j = 0; j < 4; j++) {
			if (lastSelected != currentlySelected) {
				if (lastSelected != -1 && remapButtons [j].buttonsList.Count > lastSelected && remapButtons [j].buttonsList [lastSelected] != null) {
					DisableRemap (j);
				}
			}
		}



		for (var i = 0; i < AxisList.Count; i++) {
		//-> Remap Joystick Input Axis
			if(Mathf.Abs( Input.GetAxisRaw(AxisList[i])) == 1
				&& standInputModule.horizontalAxis == "NoMoveInMenu"
				&& standInputModule.verticalAxis 	== "NoMoveInMenu"
				&& remapButtons [0].stateList.Count > currentlySelected
				&& remapButtons [0].stateList [currentlySelected] == true
				&& remapButtons [0].b_Axis [currentlySelected] == true){


				Debug.Log ("Axis : " + AxisList [i]);

				newInputForJoystick(0,AxisList[i]);
				break;
			}
		//-> Remap Keyboard Input Axis
			if(Mathf.Abs( Input.GetAxisRaw(AxisList[i])) == 1
				&& standInputModule.horizontalAxis == "NoMoveInMenu"
				&& standInputModule.verticalAxis 	== "NoMoveInMenu"
				&& remapButtons [2].stateList.Count > currentlySelected
				&& remapButtons [2].stateList [currentlySelected] == true
				&& remapButtons [2].b_Axis [currentlySelected] == true){


				Debug.Log ("Axis : " + AxisList [i]);

				newInputForJoystick(2,AxisList[i]);
				break;
			}

		}



		//-> Remap a Button
		if (Input.anyKeyDown) {
			
			bool goodKey = false;
			string newInput = FindTheKeyCodeUpdate().ToString();


			for (var i = 0; i < keyCodeList.Count; i++) {
				if (Input.GetKeyDown (keyCodeList [i])) {
					
					if (currentlySelected != -1
					   && (keyCodeList [i] == KeyCode.Return || keyCodeList [i] == KeyCode.Mouse0)
						&& remapButtons [inputRemapType].stateList.Count > currentlySelected
					   && remapButtons [inputRemapType].stateList [currentlySelected] == true) {

					} else if (currentlySelected != -1) {
						Debug.Log (newInput);
						goodKey = true;
						Debug.Log ("Button is pressed");
						newInput = keyCodeList [i].ToString ();
						break;
					}
				}
			}




		

				//-> It is possible to change the Input
				if (goodKey
				   && currentlySelected != -1
					&& remapButtons [inputRemapType].stateList.Count > currentlySelected
					&& remapButtons [inputRemapType].buttonsList [currentlySelected] != null
					&& remapButtons [inputRemapType].stateList [currentlySelected] == false) {

					enableInputRemap (inputRemapType);
				}

		//-> New Input for Joystick
				else if (inputRemapType == 1 
				&& goodKey
				&& currentlySelected != -1 
				&& remapButtons [inputRemapType].stateList.Count > currentlySelected
				&& remapButtons[inputRemapType].buttonsList [currentlySelected] != null 
				&& remapButtons[inputRemapType].stateList [currentlySelected] == true) {

				if (remapButtons [inputRemapType].b_Axis [currentlySelected] == false)
					newInputForJoystick (inputRemapType, newInput);
				else {
					if(Input.GetKeyDown(KeyCode.Escape)){
						DisableRemap (inputRemapType);
					}
					else{
						StopCoroutine("infoPlayer");
						StartCoroutine( infoPlayer (txt_PlayerInfoTextProp.returnInfoText()));		// Text : Input Not Possible
					}
				}
					
			}
		//-> Display info to say why it is not possible to use this key
			else if (inputRemapType == 1 
				&& !goodKey
				&& currentlySelected != -1 
				&& remapButtons [inputRemapType].stateList.Count > currentlySelected
				&& remapButtons[inputRemapType].buttonsList [currentlySelected] != null 
				&& remapButtons[inputRemapType].stateList [currentlySelected] == true) {

				if(Input.GetKeyDown(KeyCode.Escape)){
					DisableRemap (inputRemapType);
				}
				else{
					StopCoroutine("infoPlayer");
					StartCoroutine( infoPlayer (txt_PlayerInfoTextProp.returnInfoText()));		// Text : Input Not Possible
				}
			}
		//-> New Input for Desktop
			else if (inputRemapType == 3 
				&& !goodKey
				&& currentlySelected != -1 
				&& remapButtons [inputRemapType].stateList.Count > currentlySelected
				&& remapButtons[inputRemapType].buttonsList [currentlySelected] != null 
				&& remapButtons[inputRemapType].stateList [currentlySelected] == true) {

				bool b_KeyNotAvailable = false;
				for (var i = 0; i < keyCodeNotAvailableList.Count; i++) {
					if (keyCodeNotAvailableList [i].ToString() == newInput) {
						b_KeyNotAvailable = true;
						break;
					}
						
				}

				if(!b_KeyNotAvailable)
					newInputForDesktop(inputRemapType,newInput);
			}
		//-> Display info to say why it is not possible to use this key
			else if (inputRemapType == 3 
				&& goodKey
				&& currentlySelected != -1 
				&& remapButtons [inputRemapType].stateList.Count > currentlySelected
				&& remapButtons[inputRemapType].buttonsList [currentlySelected] != null 
				&& remapButtons[inputRemapType].stateList [currentlySelected] == true) {

				StopCoroutine("infoPlayer");
				StartCoroutine( infoPlayer (txt_PlayerInfoTextProp.returnInfoText()));		// Text : Input Not Possible
			}
		

		}

		lastSelected = currentlySelected;





	}

	public void DisableRemap(int inputRemapType){
		remapButtons[inputRemapType].stateList [lastSelected] = false;
		changeStandInputModuleState("MenuNagHorizontal", "MenuNagVertical", "Submit", "Cancel");

		remapButtons[inputRemapType].buttonsList [lastSelected].transform.GetChild (0).GetComponent<Text> ().text = remapButtons[inputRemapType].inputNameList[lastSelected];

		if(ingameGlobalManager.instance.navigationList.Count > 0
           && ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "RemapInput"){
            ingameGlobalManager.instance.navigationList.RemoveAt(ingameGlobalManager.instance.navigationList.Count - 1);
            //Debug.Log("Function DisableRemap");
        }
			
		
	}


	public void changeStandInputModuleState(string horizontalAxis,string verticalAxis,string submitButton,string cancelButton){
		standInputModule.horizontalAxis = horizontalAxis;
		standInputModule.verticalAxis 	= verticalAxis;
		standInputModule.submitButton 	= submitButton;
		standInputModule.cancelButton 	= cancelButton;
	}


	public void enableInputRemap(int inputRemapType){
		remapButtons[inputRemapType].stateList [currentlySelected] = true;
		changeStandInputModuleState("NoMoveInMenu", "NoMoveInMenu", "NoMoveInMenu", "NoMoveInMenu");

		remapButtons [inputRemapType].buttonsList [currentlySelected].transform.GetChild (0).GetComponent<Text> ().text = 
		remapButtons [inputRemapType].buttonsList [currentlySelected].transform.GetChild (0).GetComponent<TextProperties> ().returnInfoText ();

		Debug.Log ("Nagigation disabled");
	}

	public void newInputForJoystick(int inputRemapType,string newInput){
		//-> Input is not alredy use
		if (!checkIfInputAlreadyUsed (inputRemapType, newInput)) {		
			remapButtons [inputRemapType].stateList [currentlySelected] = false;

			remapButtons [inputRemapType].buttonsList [currentlySelected].transform.GetChild (0).GetComponent<Text> ().text = newInput;
			remapButtons [inputRemapType].inputNameList [currentlySelected] = newInput;
			Debug.Log ("Nagigation enabled");
			StartCoroutine (reactivateNavigation ());
		}
		else {
			StopCoroutine("infoPlayer");
			StartCoroutine( infoPlayer (txt_PlayerInfoTextProp.returnInfoText()));		// Text : Input Not Possible
		}
	}

	//-> Wait before reactivate Ui Navigation. Prevent bug when directions are remap
	IEnumerator reactivateNavigation(){
		//Input.ResetInputAxes ();
		Debug.Log ("reactivate");
		yield return new WaitForSeconds (.25f);
		changeStandInputModuleState("MenuNagHorizontal", "MenuNagVertical", "Submit", "Cancel");

        if(ingameGlobalManager.instance.navigationList.Count > 0
           && ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "RemapInput"){
            //Debug.Log("Function reactivateNavigation");
            ingameGlobalManager.instance.navigationList.RemoveAt(ingameGlobalManager.instance.navigationList.Count - 1);
        }
			

		Input.ResetInputAxes ();


		saveInputs ();
	}

	public void newInputForDesktop(int inputRemapType,string newInput){
		//-> Input is not alredy use
		if (!checkIfInputAlreadyUsed (inputRemapType, newInput)) {		
			remapButtons [inputRemapType].stateList [currentlySelected] = false;

			remapButtons [inputRemapType].buttonsList [currentlySelected].transform.GetChild (0).GetComponent<Text> ().text = newInput;
			remapButtons [inputRemapType].inputNameList [currentlySelected] = newInput;
			Debug.Log ("Nagigation enabled");

			StartCoroutine (reactivateNavigation ());
		} else {
			StopCoroutine("infoPlayer");
			StartCoroutine( infoPlayer (txt_PlayerInfoTextProp.returnInfoText()));		// Text : Input Not Possible
		}

	}


	public bool checkIfInputAlreadyUsed(int inputRemapType,string newInput){
		bool result = false;
		for (var i = 0; i < remapButtons[inputRemapType].inputNameList.Count; i++) {
			if (remapButtons [inputRemapType].inputNameList [i] == newInput){
				result = true;
			}
		}

		if (remapButtons [inputRemapType].inputNameList [currentlySelected] == newInput){
			result = false;
		}

		return result;
	}

	// --> find the input (keycode)
	public KeyCode FindTheKeyCodeUpdate(){																

		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(key))
			{return key;}
		}
		return KeyCode.None;
	}

	IEnumerator infoPlayer(string newtext){
		if (txt_PlayerInfo)
			txt_PlayerInfo.text = newtext;

		yield return new  WaitForSeconds (4);
		txt_PlayerInfo.text = "";
	}

	public void updateInputPage () {
		if(!PlayerPrefs.HasKey("InputsType"))
			PlayerPrefs.SetInt ("InputsType", 0);


		// Keyboard
		if(PlayerPrefs.GetInt ("InputsType") == 0){
			selectedInputType.transform.position = btnKeyboard.transform.position;
			if (!keyboardPanel.activeSelf)
				keyboardPanel.SetActive (true);
			if (joystickPanel.activeSelf)
				joystickPanel.SetActive (false);


			eventSystem.SetSelectedGameObject (btnKeyboard);
			//mainMenu.addToNavigationButtonList(btnKeyboard);
		}
		// Joystick
		else{
			selectedInputType.transform.position = btnJoystick.transform.position;
			if (keyboardPanel.activeSelf)
				keyboardPanel.SetActive (false);
			if (!joystickPanel.activeSelf)
				joystickPanel.SetActive (true);

			eventSystem.SetSelectedGameObject (btnJoystick);
			//mainMenu.addToNavigationButtonList(btnJoystick);
		}

	}

	public void defaultInputs(int value){
        if(ingameGlobalManager.instance.b_DesktopInputs){
            //Remap 
            //    0 : Axis Joystick     1: Button Joystick      2 Axis Keyboard     3: Button Keyboard
            //Bool  0: Joystick     1: Keyboard
            //float  0: Joystick     1: Keyboard

            //Debug.Log("Reset Mapping : " + value + " : " + remapButtons[value].inputNameList.Count);

            int whichInputs = 0;            // Joystick Axis and button values
            if (value == 3) whichInputs = 2;// Keyboard Axis and button  values 


            //-> Init Axis
            for (var i = 0; i < remapButtons[whichInputs].inputNameList.Count; i++)
            {
                if (b_PC)
                    remapButtons[whichInputs].inputNameList[i] = remapButtons[whichInputs].defaultNamePC[i];
                else
                    remapButtons[whichInputs].inputNameList[i] = remapButtons[whichInputs].defaultNameMac[i];
            }


            for (var i = 0; i < remapButtons[whichInputs].inputNameList.Count; i++)
            {
                remapButtons[whichInputs].buttonsList[i].transform.GetChild(0).GetComponent<Text>().text = remapButtons[whichInputs].inputNameList[i];
            } 


            //-> Init Buttons
            for (var i = 0; i < remapButtons[whichInputs+1].inputNameList.Count; i++)
            {
                if (b_PC)
                    remapButtons[whichInputs+ 1].inputNameList[i] = remapButtons[whichInputs+ 1].defaultNamePC[i];
                else
                    remapButtons[whichInputs+ 1].inputNameList[i] = remapButtons[whichInputs+ 1].defaultNameMac[i];
            }


            for (var i = 0; i < remapButtons[whichInputs+ 1].inputNameList.Count; i++)
            {
                remapButtons[whichInputs+ 1].buttonsList[i].transform.GetChild(0).GetComponent<Text>().text = remapButtons[whichInputs+ 1].inputNameList[i];
            } 


            whichInputs = 0;            // Joystick float and bool values
            if (value == 3) whichInputs = 1;// Keyboard float and bool values 

        
            //-> Init float value
            for (var i = 0; i < floatValues[whichInputs].buttonsList.Count; i++)
            {
                floatValues[whichInputs].buttonsList[i].GetComponent<Slider>().value = floatValues[whichInputs].b_Values[i];
            } 

            //-> Init bool value
        for (var i = 0; i < boolValues[whichInputs].buttonsList.Count; i++)
            {
                boolValues[whichInputs].buttonsList[i].GetComponent<Toggle>().isOn = boolValues[whichInputs].b_Values[i];
            }  
        }
	}

	public void defaultInputsAlreadyExist(){
		string inputsRawList = loadInputs ();
		// Parse
		string[] InputList = inputsRawList.Split ('_');

		string[] InputListJoystickAxis 		= InputList[0].Split (':');
		string[] InputListJoystickButton 	= InputList[1].Split (':');
		string[] InputListKeyboardAxis 		= InputList[2].Split (':');
		string[] InputListKeyboardButton 	= InputList[3].Split (':');


		for(var i = 0;i<InputListJoystickAxis.Length;i++){
			if (remapButtons [0].inputNameList.Count > i) {
				remapButtons [0].inputNameList [i] = InputListJoystickAxis [i];
				remapButtons [0].buttonsList [i].transform.GetChild (0).GetComponent<Text> ().text =
				InputListJoystickAxis [i];
			}
		}
		for(var i = 0;i<InputListJoystickButton.Length;i++){
			if (remapButtons [1].inputNameList.Count > i) {
				remapButtons [1].inputNameList [i] = InputListJoystickButton [i];
				remapButtons [1].buttonsList [i].transform.GetChild (0).GetComponent<Text> ().text =
				InputListJoystickButton [i];
			}
		}
		for(var i = 0;i<InputListKeyboardAxis.Length;i++){
			if (remapButtons [2].inputNameList.Count > i) {
				remapButtons [2].inputNameList [i] = InputListKeyboardAxis [i];
				remapButtons [2].buttonsList [i].transform.GetChild (0).GetComponent<Text> ().text =
				InputListKeyboardAxis [i];
			}
		}
		for(var i = 0;i<InputListKeyboardButton.Length;i++){
			if (remapButtons [3].inputNameList.Count > i) {
				remapButtons [3].inputNameList [i] = InputListKeyboardButton [i];
				remapButtons [3].buttonsList [i].transform.GetChild (0).GetComponent<Text> ().text =
				InputListKeyboardButton [i];
			}
		}


		//-> Init Input Float
		string inputsRawListFloat = loadInputsFloatsValue ();


		// Parse
		string[] InputListFloat = inputsRawListFloat.Split ('_');


		string[] InputListJoystickFloat = InputListFloat [0].Split (':');
		string[] InputListKeyboardFloat = InputListFloat [1].Split (':');

		for(var i = 0;i < InputListJoystickFloat.Length;i++){
			if(floatValues [0].buttonsList.Count > i)
				floatValues [0].buttonsList [i].GetComponent<Slider> ().value = float.Parse(InputListJoystickFloat[i]);
		}
		for(var i = 0;i < InputListKeyboardFloat.Length;i++){
			if(floatValues [1].buttonsList.Count > i)
				floatValues [1].buttonsList [i].GetComponent<Slider> ().value = float.Parse(InputListKeyboardFloat[i]);
		}




		//-> Init Input Bool
		string inputsRawListBool = loadInputsBoolsValue ();

		// Parse
		string[] InputListBool = inputsRawListBool.Split ('_');



		string[] InputListJoystickBool = InputListBool [0].Split (':');

		string[] InputListKeyboardBool = InputListBool [1].Split (':');

		for(var i = 0;i < InputListJoystickBool.Length;i++){
			if(boolValues [0].buttonsList.Count > i)
				boolValues [0].buttonsList [i].GetComponent<Toggle> ().isOn = bool.Parse(InputListJoystickBool[i]);
		}
		for(var i = 0;i < InputListKeyboardBool.Length;i++){
			if(boolValues [1].buttonsList.Count > i)
				boolValues [1].buttonsList [i].GetComponent<Toggle> ().isOn = bool.Parse(InputListKeyboardBool[i]);
		}


	}

	public string returnDefaultInputs(){
		string stringList = "";



		for (var j = 0; j < 4; j++) {
			for (var i = 0; i < remapButtons[j].inputNameList.Count; i++) {
				if(b_PC)
					remapButtons [j].inputNameList [i] = remapButtons [j].defaultNamePC [i];
				else
					remapButtons [j].inputNameList [i] = remapButtons [j].defaultNameMac [i];
			}


			if (remapButtons [j].inputNameList.Count > 0) {
				for (var i = 0; i < remapButtons [j].inputNameList.Count; i++) {
					stringList += remapButtons [j].inputNameList [i];

					if (i == remapButtons [j].inputNameList.Count - 1 && j != 3) {
						stringList += "_";
					} else {
						if (j == 3 && i == remapButtons [j].inputNameList.Count - 1) {

						} else
							stringList += ":";
					} 
				}
			}
			else {
					stringList += "NoInput_";
			}
			
		}
		return stringList;
	}

	public void saveInputs(){
		string stringList = "";
		for (var j = 0; j < 4; j++) {
			if (remapButtons [j].inputNameList.Count > 0) {
				for (var i = 0; i < remapButtons [j].inputNameList.Count; i++) {
					stringList += remapButtons [j].inputNameList [i];
			

					if (i == remapButtons [j].inputNameList.Count - 1 && j != 3) {
						stringList += "_";
					} else {
						if (j == 3 && i == remapButtons [j].inputNameList.Count - 1) {

						} else
							stringList += ":";
					} 
				}
			} else {
				stringList += "NoInput_";
			}
		}

		Debug.Log (stringList);

		PlayerPrefs.SetString ("InputList", stringList);

	//-> Save Inputs Float Value
		string floatList = "";

		if (floatValues [0].buttonsList.Count == 0) {
			floatList += "0";							// no float value for joystick
		}

		//-> Joystick float Values
		for (var i = 0; i < floatValues [0].buttonsList.Count; i++) {
			if (floatValues [0].buttonsList [i].GetComponent<Slider> ())
				floatList += floatValues [0].buttonsList [i].GetComponent<Slider> ().value.ToString ();
			else
				Debug.Log ("Slider is not setup in the input Menu");

			if (i != floatValues [0].buttonsList.Count - 1)
				floatList += ":";
		}

		if (floatValues [1].buttonsList.Count > 0) {
			floatList += "_";
		} else {
			floatList += "_0";							// no float value for Keyboard
		}

		//-> Desktop float Values
		for (var i = 0; i < floatValues [1].buttonsList.Count; i++) {
			if (floatValues [0].buttonsList [i].GetComponent<Slider> ())
				floatList += floatValues [1].buttonsList [i].GetComponent<Slider>().value.ToString();
			else
				Debug.Log ("Slider is not setup in the input Menu");

			if (i != floatValues [1].buttonsList.Count - 1)
				floatList += ":";
		}
			
		PlayerPrefs.SetString ("InputList_Float", floatList);


	//-> Save Inputs Bool Value
		string boolList = "";
		if (boolValues [0].buttonsList.Count == 0) {
			boolList += "False";							// no bool value for joystick
		}
		//-> Joystick float Values
		for (var i = 0; i < boolValues [0].buttonsList.Count; i++) {
			if (boolValues [0].buttonsList [i].GetComponent<Toggle> ())
				boolList += boolValues [0].buttonsList [i].GetComponent<Toggle> ().isOn.ToString ();
			else {
				boolList += "False";
				Debug.Log ("Toggle is not setup in the input Menu");
			}

			if (i != boolValues [0].buttonsList.Count - 1)
				boolList += ":";
		}



		if (boolValues [1].buttonsList.Count > 0) {
			boolList += "_";
		} else {
			boolList += "_False";							// no bool value for Keyboard
		}


		//-> Desktop float Values
		for (var i = 0; i < boolValues [1].buttonsList.Count; i++) {
			if (boolValues [0].buttonsList [i].GetComponent<Toggle> ())
				boolList += boolValues [1].buttonsList [i].GetComponent<Toggle> ().isOn.ToString ();
			else {
				boolList += "False";
				Debug.Log ("Toggle is not setup in the input Menu");
			}

			if (i != boolValues [1].buttonsList.Count - 1)
				boolList += ":";
		}

		PlayerPrefs.SetString ("InputList_Bool", boolList);






		Debug.Log ("InputList : " + PlayerPrefs.GetString ("InputList"));
		Debug.Log ("InputList_Float : " + PlayerPrefs.GetString ("InputList_Float"));
		Debug.Log ("InputList_Bool : " + PlayerPrefs.GetString ("InputList_Bool"));

		// Update inputs in the ingameGlobalManager
		ingameGlobalManager.instance.initInputsValues ();

	}


	public string loadInputs(){
		string newString = "";
		if (!PlayerPrefs.HasKey ("InputList")) {
			newString = returnDefaultInputs ();
		} else {
			newString = PlayerPrefs.GetString ("InputList");
		}

		return  newString;
	}

	public string loadInputsFloatsValue(){
		string newString = "";
		if (!PlayerPrefs.HasKey ("InputList_Float")) {
			newString = returnDefaultInputsFloatValue ();
		} else {
			newString = PlayerPrefs.GetString ("InputList_Float");
		}

		return  newString;
	}

	public string loadInputsBoolsValue(){
		string newString = "";
		if (!PlayerPrefs.HasKey ("InputList_Bool")) {
			newString = returnDefaultInputsBoolValue ();
		} else {
			newString = PlayerPrefs.GetString ("InputList_Bool");
		}

		return  newString;
	}

	//-> return Inputs Float Values
	public string returnDefaultInputsFloatValue(){

		string result = "";

		if (floatValues [0].buttonsList.Count == 0) {
			result += "0";							// no float value for joystick
		}
		//-> Joystick float Values
		for (var i = 0; i < floatValues [0].buttonsList.Count; i++) {
			result += floatValues [0].b_Values [i].ToString();

			if (i != floatValues [0].buttonsList.Count - 1)
				result += ":";
		}

		if (floatValues [1].buttonsList.Count > 0) {
			result += "_";
		} else {
			result += "_0";
		}

		//-> Desktop float Values
		for (var i = 0; i < floatValues [1].buttonsList.Count; i++) {
			result += floatValues [1].b_Values [i].ToString();


			if (i != floatValues [1].buttonsList.Count - 1)
				result += ":";
		}

		return result;
	}

	//-> return Inputs Bool Values
	public string returnDefaultInputsBoolValue(){

		string result = "";
		if (boolValues [0].buttonsList.Count == 0) {
			result += "False";							// no float value for joystick
		}
		//-> Joystick float Values
		for (var i = 0; i < boolValues [0].buttonsList.Count; i++) {
			result += boolValues [0].b_Values [i].ToString();

			if (i != boolValues [0].buttonsList.Count - 1)
				result += ":";
		}

		if (boolValues [1].buttonsList.Count > 0) {
			result += "_";
		} else {
			result += "_False";
		}

		//-> Desktop float Values
		for (var i = 0; i < boolValues [1].buttonsList.Count; i++) {
			result += boolValues [1].b_Values [i].ToString();


			if (i != boolValues [1].buttonsList.Count - 1)
				result += ":";
		}

		return result;
	}


	public void backToMenuOptions(){
		ingameGlobalManager.instance.GetComponent<backInputs> ().MMS_GoToOptionsMenu ();

	}


	public void MM_pointerEnterSetSelected(GameObject newObj){
		if (mainMenu == null) {
			GameObject tmp = GameObject.Find ("mainMenu");
			if (tmp)mainMenu = tmp.GetComponent<MainMenu> ();}
		
		if (mainMenu)
			mainMenu.F_pointerEnterSetSelected (newObj);
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
        //Debug.Log("Function MM_removeToNavigationList");
	}

	public void MM_addToNavigationButtonList(GameObject newObj){
		ingameGlobalManager.instance.navigationButtonList.Add (newObj);
	}

	public void MM_removeToNavigationButtonList(){
		ingameGlobalManager.instance.navigationList.RemoveAt (ingameGlobalManager.instance.navigationButtonList.Count-1);
        //Debug.Log("Function MM_removeToNavigationButtonList");
	}


    public void updateMainMenuTopRightUIInfo(){
        if(ingameGlobalManager.instance.canvasPlayerInfos)ingameGlobalManager.instance.canvasPlayerInfos.displayAvailableActionOnScreen(true, "");
    }
}
