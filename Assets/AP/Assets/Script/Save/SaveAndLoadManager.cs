// Description : SaveAndLoadManager : Manage load and Save Process. Find this script on ingameGlobalManager in scene view
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using TS.Generics;

public class SaveAndLoadManager : MonoBehaviour {
	[HideInInspector] 
	public LevelManager _levelManager;
	[HideInInspector] 
	public bool 		b_IngameDataHasBeenLoaded = true;

	//-> Variables : spawn point when you go from one level to another
	public string	 	currentSpawnName;

	public bool 		b_saveProcessFinished = true;

	public Text			txt_Debug;
    public bool         showDebugLog = false;

	[Serializable]
	public class DataClass
	{
		public string data;
	}

	void Start(){
		GameObject tmp = GameObject.Find ("Text_Debug");

		if(tmp)
		txt_Debug = tmp.GetComponent<Text> ();
	}

		
//--> Save DATAS Ingame -- Begin --

//--> Loading Process : Load all the information  
	IEnumerator loadProcess () {
		if (txt_Debug)
			txt_Debug.text = "";

		b_IngameDataHasBeenLoaded = false;
//--> Set the _levelManager.listState to false. This state is used to know if all the objects parameters are loaded correctly 
		for (var i = 0; i < ingameGlobalManager.instance._levelManager.listState.Count; i++) {
			ingameGlobalManager.instance._levelManager.listState [i] = false;
		}

		string s_ObjectsDatas = "";

//-> Load InGameManager information.
		s_ObjectsDatas = returnDataDependingHisType ("_InGameManager");

		yield return new WaitUntil(() => ingameGlobalManager.instance.loadInGameManager(s_ObjectsDatas));
        if (showDebugLog)Debug.Log (" 1 : ingameManager Loaded");
		if (txt_Debug)
			txt_Debug.text += " 1 : ingameManager Loaded" + "\n";
//-> Load Player Inventory
		s_ObjectsDatas = returnDataDependingHisType ("_Inventory");

		yield return new WaitUntil(() => ingameGlobalManager.instance.loadPlayerInventoryList(s_ObjectsDatas));
        if (showDebugLog)Debug.Log (" 2 : Player Inventory Loaded : " + s_ObjectsDatas);
		if (txt_Debug)
			txt_Debug.text += "2 : Player Inventory Loaded : " + s_ObjectsDatas + "\n";
//-> Load Player Diary
		s_ObjectsDatas = returnDataDependingHisType ("_Diary");

		yield return new WaitUntil(() => ingameGlobalManager.instance.loadPlayerDiaryList(s_ObjectsDatas));
        if (showDebugLog)Debug.Log (" 3 : Player Diary Loaded");
		if (txt_Debug)
			txt_Debug.text += "3 : Player Diary Loaded : " + s_ObjectsDatas + "\n";
//-> Load Objects Datas in the current scene
		s_ObjectsDatas = returnDataDependingHisType ("_Objs");

		if (s_ObjectsDatas != "") {         // Data exist for objects in LevelManager
			string[] codes = s_ObjectsDatas.Split (':');
			//Debug.Log (s_ObjectsDatas);
			int countTheNumberOfObjectInitialized = 0;

			for (var i = 0; i < codes.Length; i++) {
				//Debug.Log (codes [i]);
				if (_levelManager.listOfGameObjectForSaveSystem [i].activeInHierarchy) {
					_levelManager.listOfGameObjectForSaveSystem [i].GetComponent<SaveData> ().LoadData (codes [i]);

				} else {
					_levelManager.listState [i] = true;
					//Debug.Log (i + " : " + codes [i]);
				}
				countTheNumberOfObjectInitialized++;
			}

			//-> New Object have been added to the scene but the save doesn't contains those data
			//Debug.Log("countTheNumberOfObjectInitialized: " + countTheNumberOfObjectInitialized);
            if(countTheNumberOfObjectInitialized < _levelManager.listOfGameObjectForSaveSystem.Count)
            {
				for (var i = countTheNumberOfObjectInitialized; i < _levelManager.listOfGameObjectForSaveSystem.Count; i++)
				{
					//Debug.Log("i: " + i);
					if (_levelManager.listOfGameObjectForSaveSystem[i].activeInHierarchy)
					{
						_levelManager.listOfGameObjectForSaveSystem[i].GetComponent<SaveData>().LoadData("");
					}
					else
					{
						_levelManager.listState[i] = true;
					}
				}
			}

			yield return new WaitUntil(() => _levelManager.allObjectsInitiliazed() == true);
		}
        else{       // No data exist. Init Object in LevelManager
            for (var i = 0; i < _levelManager.listOfGameObjectForSaveSystem.Count; i++)
            {
                if (_levelManager.listOfGameObjectForSaveSystem[i].activeInHierarchy){
                    _levelManager.listOfGameObjectForSaveSystem[i].GetComponent<SaveData>().LoadData("");}
                else{
                    _levelManager.listState[i] = true;}
            }
            yield return new WaitUntil(() => _levelManager.allObjectsInitiliazed() == true);
        }

        if (showDebugLog)Debug.Log (" 4 : Objects Init Loaded");
		if (txt_Debug)
			txt_Debug.text += "4 : Objects Init Loaded" + "\n";

      

        if (showDebugLog)Debug.Log(" 5 : UI Text Loaded");
        yield return new WaitUntil(() => createListOfAllUITextInTheCurrentScene() == true);


        //yield return new WaitForEndOfFrame();

//-> Load Player Character information.
		s_ObjectsDatas = returnDataDependingHisType ("_Chara");

		yield return new WaitUntil(() => ingameGlobalManager.instance.currentPlayer.GetComponent<savePlayerInformations>().loadChararcterParams(s_ObjectsDatas));
        if (showDebugLog)Debug.Log (" 6 : Player Character Loaded");
		if (txt_Debug)
			txt_Debug.text += "5 : Player Character Loaded" + "\n";

		b_IngameDataHasBeenLoaded = true;
		ingameGlobalManager.instance.b_AllowCharacterMovment = true;
		ingameGlobalManager.instance.b_Ingame_Pause = false;
		ingameGlobalManager.instance.canvasPlayerInfos.deleteAllButtons ();

		//-> Load Flashlight State
        Torch[] allTorch = FindObjectsByType<Torch>(FindObjectsSortMode.None);
		foreach (Torch child in allTorch)
			child.Init();

	}



    public bool createListOfAllUITextInTheCurrentScene(){
        ingameGlobalManager.instance.UITextList.Clear();
        ingameGlobalManager.instance.UITextList.Clear();

        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject go in allObjects)
        {
            Canvas[] Children = go.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas child in Children)
            {
                Text[] allUIText = child.GetComponentsInChildren<Text>(true);

                foreach (Text childUI in allUIText)
                {
                    if(childUI.GetComponent<TextProperties>())
                        ingameGlobalManager.instance.UITextList.Add(childUI.GetComponent<TextProperties>());
                }                
            }
        }


        return true;
    }


    //--> Call in the MainMenu.cs to start save process
	public void F_SaveProcess(){
		StartCoroutine(	SaveProcess(true));
	}

	//--> Save All the informations ingame
	IEnumerator SaveProcess(bool b_Chara){
		b_saveProcessFinished = false;
		yield return new WaitUntil(() => F_Save_InGameManager ("_InGameManager") == true);
		yield return new WaitUntil(() => F_Save_Objs ("_Objs") == true);
		yield return new WaitUntil(() => F_Save_Inventory ("_Inventory") == true);
		yield return new WaitUntil(() => F_Save_Diary ("_Diary") == true);
		if(b_Chara)
		yield return new WaitUntil(() => F_Save_Chara ("_Chara") == true);
		b_saveProcessFinished = true;
		yield return null;

		//-> Save Flashlight State
		Torch[] allTorch = FindObjectsByType<Torch>(FindObjectsSortMode.None);
		foreach (Torch child in allTorch)
			child.SaveTorchState();


        if (showDebugLog)Debug.Log ("Save Done");
	}


	//--> Save Objects Datas in the current scene
	bool F_Save_Objs(string savePartName){
		string s_ObjectsDatas = "";

		//-> Save each gameObject info in the LevelManager gameObject
		for (var i = 0; i < _levelManager.listOfGameObjectForSaveSystem.Count; i++) {
			if (i > 0)s_ObjectsDatas += ":";

			s_ObjectsDatas += _levelManager.listOfGameObjectForSaveSystem [i].GetComponent<SaveData> ().R_SaveData ();
		}

		savedataDependingHisType (savePartName, s_ObjectsDatas);							// Save Data (PlayerPrefs or .dat)
        if (showDebugLog)Debug.Log("F_Save_Objs : " + s_ObjectsDatas);
		return true;
	}


	//--> Save current Player Inventory
	bool F_Save_Inventory(string savePartName){
		string s_ObjectsDatas = "";

        s_ObjectsDatas += ingameGlobalManager.instance.currentPlayerInventoryList.Count.ToString() + ":";

        // Save Object available in the Inventory.
		for (var i = 0; i < ingameGlobalManager.instance.currentPlayerInventoryList.Count; i++) {
			if (i < ingameGlobalManager.instance.currentPlayerInventoryList.Count - 1) {
				s_ObjectsDatas += ingameGlobalManager.instance.currentPlayerInventoryList [i].ToString () + ":";} 
			else {
                s_ObjectsDatas += ingameGlobalManager.instance.currentPlayerInventoryList [i].ToString () + ":";}
		}


        // Save if the object in Inventory need to be displayed in the Inventory Viewer
        for (var i = 0; i < ingameGlobalManager.instance.currentPlayerInventoryList.Count; i++)
        {
            //if (i < ingameGlobalManager.instance.currentPlayerInventoryList.Count - 1)
            //{
                s_ObjectsDatas += r_TrueFalse(ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList[i]);
            //}

            if (i < ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList.Count - 1)
            {
                s_ObjectsDatas += ":";
            }

            //;
           /* else
            {
                s_ObjectsDatas += r_TrueFalse(ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList[i]);
            }*/
        }


		savedataDependingHisType (savePartName, s_ObjectsDatas);							// Save Data (PlayerPrefs or .dat)
		return true;
	}

	//--> Save current Player diary
	bool F_Save_Diary(string savePartName){
		string s_ObjectsDatas = "";

		for (var i = 0; i < ingameGlobalManager.instance.currentPlayerDiaryList.Count; i++) {
			if (i < ingameGlobalManager.instance.currentPlayerDiaryList.Count - 1)
				s_ObjectsDatas += ingameGlobalManager.instance.currentPlayerDiaryList [i].ToString() + ":";
			else
				s_ObjectsDatas += ingameGlobalManager.instance.currentPlayerDiaryList [i].ToString();
		}

		savedataDependingHisType (savePartName, s_ObjectsDatas);							// Save Data (PlayerPrefs or .dat)
		return true;
	}

	//--> Save current Character data
	bool F_Save_Chara(string savePartName){
		string s_ObjectsDatas = "";

		s_ObjectsDatas += ingameGlobalManager.instance.currentPlayer.GetComponent<savePlayerInformations>().saveChararcterParams ();

		savedataDependingHisType (savePartName, s_ObjectsDatas);							// Save Data (PlayerPrefs or .dat)
		return true;
	}

	//--> Save InGameManager Datas in the current scene
	bool F_Save_InGameManager(string savePartName){
		string s_ObjectsDatas = "";

		s_ObjectsDatas += ingameGlobalManager.instance.saveInGameManagerParams();

		savedataDependingHisType (savePartName, s_ObjectsDatas);							// Save Data (PlayerPrefs or .dat)
		return true;
	}


// Save Data using PlayerPrefs or .dat
	public void savedataDependingHisType(string savePartName,string s_ObjectsDatas){
		if (ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0){		// PlayerPrefs are used
			PlayerPrefs.SetString (pathOfTheSavePart(savePartName), s_ObjectsDatas);}
		else {																				// .dat
			saveDAT(s_ObjectsDatas,savePartName);}
	}

//-- End --


//--> Save And Load Slots information -- Begin --

//--> Save Slot Information
	public void F_Save_SlotInformation(int slotNumber,string s_DateAndTime,bool b_createNewData){
		string s_refDatas = F_Load_SlotInformation();
		string s_ObjectsDatas = "";

		if (s_refDatas != "") {									// Save Already Exist
			string[] codes = s_refDatas.Split ('_');
			for (var i = 0; i < codes.Length; i++) {

				if(i!=slotNumber)
					s_ObjectsDatas += codes[i];
				else
					s_ObjectsDatas += s_DateAndTime;

				if(i < codes.Length-1)
					s_ObjectsDatas += "_" ;
			}

		} else {												// First time save for this slot
		}


		if (ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0){		// PlayerPrefs are used
			PlayerPrefs.SetString ("Slot", s_ObjectsDatas);}
		else {																				// .dat
			saveDAT(s_ObjectsDatas,"Slot");}


		LoadGameFromSlot (slotNumber);
	}


//--> Load Slot Information
	public string F_Load_SlotInformation(){
		string s_ObjectsDatas = returnDataDependingHisType ("Slot");

		if (s_ObjectsDatas == "") {																// Create Slot Save Data file the first time
			s_ObjectsDatas += "Empty_Empty_Empty";
			if (ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0){		// PlayerPrefs are used
				PlayerPrefs.SetString ("Slot", s_ObjectsDatas);}
			else {																				// .dat
				saveDAT(s_ObjectsDatas,"Slot");}
		} 
		return s_ObjectsDatas;
	}



//--> Load Game
	public void LoadGameFromSlot(int slotNumber){
		string s_ObjectsDatas = "";
		int buildinIndexToLoad = 0;
		if(ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0)		// PlayerPrefs are used
			s_ObjectsDatas = PlayerPrefs.GetString (pathOfTheSavePart("_InGameManager"));
		else {																			// .dat
			s_ObjectsDatas = LoadDAT ("_InGameManager");}

		if (s_ObjectsDatas == "") {														// datas doesn't exist
			buildinIndexToLoad = ingameGlobalManager.instance.dataFolder.firstSceneBuildInIndex;
		} 
		else {
			string[] codes = s_ObjectsDatas.Split (',');

			//Debug.Log (codes [11]); // BuildIn Index : Current scene to load for the selected slot
			buildinIndexToLoad = Int32.Parse( codes [11]);
		}

        if(ingameGlobalManager.instance.voiceOverManager)ingameGlobalManager.instance.voiceOverManager.StopVoiceOver();

//-> Display Loading black screen
		CanvasGroup refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "LoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
					break;
				}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);

//-> Load the needed scene
		StartCoroutine (LoadYourAsyncScene (buildinIndexToLoad));
	}


// Load scene async
	IEnumerator LoadYourAsyncScene(int index)
	{
        initDebugCanvas();    // Debug Canvas

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

		if(ingameGlobalManager.instance.canvasPlayerInfos)
			yield return new WaitUntil(() => ingameGlobalManager.instance.canvasPlayerInfos.deleteAllButtons() == true);
        if (showDebugLog)Debug.Log ("Init UI Buttons (interactive, item)");
		if (txt_Debug)
			txt_Debug.text += "Init UI Buttons (interactive, item)" + "\n";

		//Wait until the last operation fully loads to return anything
		while (!asyncLoad.isDone){yield return null;}
        if (showDebugLog)Debug.Log ("Scene is loaded");
		if (txt_Debug)
			txt_Debug.text += "Scene is loaded" + "\n";

		yield return new WaitUntil(() => ingameGlobalManager.instance.initIngameGlobalManageVariables());
        if (showDebugLog)Debug.Log ("ingameGlobalManager OK");
		if (txt_Debug)
			txt_Debug.text += "ingameGlobalManager OK" + "\n";

		StartCoroutine( loadProcess ());
		yield return new WaitUntil(() => b_IngameDataHasBeenLoaded == true);

        if (showDebugLog)Debug.Log ("Done OK");
		if (txt_Debug)
			txt_Debug.text += "Done OK" + "\n";

		ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove = false;

//--> Deactivate Loading black screen
		CanvasGroup refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "NoLoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);
		if(AudioListener.pause == true)
			AudioListener.pause = false;
		
		ingameGlobalManager.instance.navigationList.Clear ();

	}

    //--> Call by TriggerManager.cs to start process to load a new scene 
	public void F_GoToAnotherLevel (string spawnPointName,int BuildInSceneIndex){
		StartCoroutine(I_GoToAnotherLevel (spawnPointName, BuildInSceneIndex));
	}
		
	//--> Go to another Level (use teleporter)
	IEnumerator I_GoToAnotherLevel(string spawnPointName,int BuildInSceneIndex){
        initDebugCanvas();    // Debug Canvas

		ingameGlobalManager.instance.b_AllowCharacterMovment = false;
        ingameGlobalManager.instance.currentPlayer.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
		ingameGlobalManager.instance.currentPlayer.GetComponent<Rigidbody> ().isKinematic = true;
		ingameGlobalManager.instance.currentPlayer.GetComponent<CapsuleCollider> ().enabled = false;

		//-> Display Loading black screen
		CanvasGroup refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "LoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);

		yield return new WaitForEndOfFrame ();

		StartCoroutine( SaveProcess(false));

		yield return new WaitUntil(() => b_saveProcessFinished == true);


        if (showDebugLog)Debug.Log ("Save Ok");
		if (txt_Debug)
			txt_Debug.text += "Save Ok" + "\n";
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(BuildInSceneIndex,LoadSceneMode.Single);
        if (showDebugLog)Debug.Log ("Start loading");
		if (txt_Debug)
			txt_Debug.text += "Start loading" + "\n";
		yield return new WaitUntil(() => ingameGlobalManager.instance.canvasPlayerInfos.deleteAllButtons() == true);
        if (showDebugLog)Debug.Log ("Init UI Buttons (interactive, item)");
		if (txt_Debug)
			txt_Debug.text += "Init UI Buttons (interactive, item)" + "\n";
		//Wait until the last operation fully loads to return anything
		while (!asyncLoad.isDone){yield return null;}
        if (showDebugLog)Debug.Log ("Scene is loaded");
		if (txt_Debug)
			txt_Debug.text += "Scene is loaded" + "\n";
		yield return new WaitUntil(() => ingameGlobalManager.instance.initIngameGlobalManageVariables());
        if (showDebugLog)Debug.Log ("ingameGlobalManager OK");
		if (txt_Debug)
			txt_Debug.text += "ingameGlobalManager OK" + "\n";

		StartCoroutine( loadProcess ());
		yield return new WaitUntil(() => b_IngameDataHasBeenLoaded == true);
        if (showDebugLog)Debug.Log ("Load Data OK");
		if (txt_Debug)
			txt_Debug.text += "Load Data OK" + "\n";

		ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove = false;


		GameObject newSpawnPoint = GameObject.Find (spawnPointName);

		if (newSpawnPoint) {
			yield return new WaitUntil(() => ingameGlobalManager.instance.currentPlayer.GetComponent<savePlayerInformations>().initChararcterParamsTeleport(newSpawnPoint.transform));
            if (showDebugLog)Debug.Log (" 5 : Player Character Position Initialized");
		}



		//-> Display Loading black screen
		refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "NoLoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);
	}



	public void F_Load_MainMenu_Scene (int BuildInSceneIndex){
		StartCoroutine(I_Load_MainMenu_Scene (BuildInSceneIndex));
	}



//--> Go to Main Nenu scene
	IEnumerator I_Load_MainMenu_Scene(int BuildInSceneIndex){
        initDebugCanvas();    // Debug Canvas
       
		//-> Display Loading black screen
		CanvasGroup refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "LoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);

		yield return new WaitForEndOfFrame ();

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(BuildInSceneIndex,LoadSceneMode.Single);
        if (showDebugLog)Debug.Log ("Start loading");
		//Wait until the last operation fully loads to return anything
		while (!asyncLoad.isDone){yield return null;}
        if (showDebugLog)Debug.Log ("Scene is loaded");


		if (BuildInSceneIndex == 0) {
			Cursor.visible = true;
		}


		//-> Display Loading black screen
		refCanvas = new CanvasGroup ();									
		for (var i = 0; i < ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas.Count; i++) {
			if (ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i].name == "NoLoadingScreen") {
				refCanvas = ingameGlobalManager.instance.canvasLoadingScreen.List_GroupCanvas [i];
				break;
			}
		}
		ingameGlobalManager.instance.canvasLoadingScreen.GoToOtherPage (refCanvas);
		ingameGlobalManager.instance.PauseGame();

		ingameGlobalManager.instance.navigationList.Clear ();

        if (showDebugLog) Debug.Log(" 5 : UI Text Loaded");
        yield return new WaitUntil(() => createListOfAllUITextInTheCurrentScene() == true);

        if (showDebugLog)Debug.Log(" 6 : init InGameGlobalManager");
        yield return new WaitUntil(() => ingameGlobalManager.instance.initIngameGlobalManageVariables() == true);


	}

//--> Init Data When game start (editor Mode Only)
    public void initLevelManagerObjectsUnityEditorOnly()
    {
        StartCoroutine(I_initLevelManagerObjectsUNityEditorOnly());
    }
    IEnumerator I_initLevelManagerObjectsUNityEditorOnly()
    {
        yield return new WaitForEndOfFrame();
        if(_levelManager){
            for (var i = 0; i < _levelManager.listOfGameObjectForSaveSystem.Count; i++)
            {
                if (_levelManager.listOfGameObjectForSaveSystem[i].activeInHierarchy)
                {
                    _levelManager.listOfGameObjectForSaveSystem[i].GetComponent<SaveData>().LoadData("");
                }
                else
                {
                    _levelManager.listState[i] = true;
                }
            }
            yield return new WaitUntil(() => _levelManager.allObjectsInitiliazed() == true); 
        }
        yield return null;
    }

    // Init Debug Canvas
    private void initDebugCanvas()
    {
        ingameGlobalManager.instance._D = false;
        ingameGlobalManager.instance._P = false;
    }


//--> Replace existing slot
	public void EraseAndReplaceSaveSlot(int currentSelectedSlot){
		currentSelectedSlot = ingameGlobalManager.instance.currentSaveSlot;

//--> Delete all save Parts
	//-> PlayerPrefs Case
		if (ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0) {		
			for (var i = 0; i < ingameGlobalManager.instance.dataFolder.buildinList.Count; i++) {
				string sceneName = ingameGlobalManager.instance.dataFolder.buildinList [i];

				if (PlayerPrefs.HasKey(currentSelectedSlot + "_" + sceneName + "_Objs"))
					PlayerPrefs.DeleteKey (currentSelectedSlot + "_" + sceneName + "_Objs");
			}

			string[] saveParts = new string[]{ "_Chara", "_Diary", "_InGameManager", "_Inventory" };					

			for (var i = 0; i < saveParts.Length; i++) {
				if (PlayerPrefs.HasKey(currentSelectedSlot + saveParts [i]))
					PlayerPrefs.DeleteKey (currentSelectedSlot + saveParts [i]);
			}
		} 
	//-> .dat Case
		else { 																			
			for (var i = 0; i < ingameGlobalManager.instance.dataFolder.buildinList.Count; i++) {
				string sceneName = ingameGlobalManager.instance.dataFolder.buildinList [i];

				if (File.Exists (Application.persistentDataPath + "/" + currentSelectedSlot + "_" + sceneName + "_Objs" + ".dat"))
					File.Delete (Application.persistentDataPath + "/" + currentSelectedSlot + "_" + sceneName + "_Objs" + ".dat");
			}

			string[] saveParts = new string[]{ "_Chara", "_Diary", "_InGameManager", "_Inventory" };					

			for (var i = 0; i < saveParts.Length; i++) {
				if (File.Exists (Application.persistentDataPath + "/" + currentSelectedSlot + saveParts [i] + ".dat"))
					File.Delete (Application.persistentDataPath + "/" + currentSelectedSlot + saveParts [i] + ".dat");
			}
		}

		F_Save_SlotInformation (currentSelectedSlot, DateTime.UtcNow.ToString (),true);	
	}




	string returnDataDependingHisType(string s_Name){
		string s_ObjectsDatas = "";
		if(ingameGlobalManager.instance.dataFolder.int_CurrentDatasSaveSystem == 0)		// PlayerPrefs are used
			s_ObjectsDatas = PlayerPrefs.GetString (pathOfTheSavePart(s_Name));
		else {																			// .dat
			s_ObjectsDatas = LoadDAT (s_Name);}

		return s_ObjectsDatas;
	}




//--> Save Datas in a .dat file
	private void saveDAT(string s_ObjectsDatas,string savePartName){
		// Old version
		//Debug.Log (Application.persistentDataPath + "/" + ingameGlobalManager.instance.currentSaveSlot + "_" + SceneManager.GetActiveScene ().name + "_Objs" + ".dat");
		/*BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/" + pathOfTheSavePart(savePartName) + ".dat");

		ObjectsDatasInScene data = new ObjectsDatasInScene ();
		data.s_ObjectsDatas = s_ObjectsDatas;
		//Debug.Log ("In : " + data.s_ObjectsDatas);

		bf.Serialize (file, data);
		file.Close ();*/

        // JSON
		DataClass dataObject = new DataClass();
		dataObject.data = s_ObjectsDatas;

		string json = JsonUtility.ToJson(dataObject);
		File.WriteAllText(Application.persistentDataPath + "/" + pathOfTheSavePart(savePartName) + ".dat", json);
	}

//--> Load datas from a .dat file
	string LoadDAT(string s_NameOfTheSavePart){
		if (File.Exists (Application.persistentDataPath + "/" + pathOfTheSavePart(s_NameOfTheSavePart) + ".dat")) {
            // Old version
            /*BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + pathOfTheSavePart(s_NameOfTheSavePart) + ".dat",FileMode.Open);

			ObjectsDatasInScene data = (ObjectsDatasInScene)bf.Deserialize(file);
			//Debug.Log ("Out : " + data.s_ObjectsDatas);
			string result = data.s_ObjectsDatas;
			file.Close();
            */

            // JSON
			string result;
			string dat = File.ReadAllText(Application.persistentDataPath + "/" + pathOfTheSavePart(s_NameOfTheSavePart) + ".dat");
			DataClass dataObject = JsonUtility.FromJson<DataClass>(dat);
			result = dataObject.data;


			return result;
		}
		else
			return "";
	}


//--> Choose the path for save file
	string pathOfTheSavePart(string s_Name){
		//-> Objects in a specific scene
		if (s_Name == "_Objs")
			return ingameGlobalManager.instance.currentSaveSlot + "_" + SceneManager.GetActiveScene ().name + s_Name;
		//-> Inventory, diary, character
		else if (s_Name == "Slot")
			return s_Name;
		//-> Inventory, diary, character
		else
			return 	ingameGlobalManager.instance.currentSaveSlot +  s_Name;

	}


    //--> Convert bool to T or F string
    private string r_TrueFalse(bool s_Ref)
    {
        if (s_Ref) return "T";
        else return "F";
    }




}


[System.Serializable]
class ObjectsDatasInScene{
	public string s_ObjectsDatas;
}
