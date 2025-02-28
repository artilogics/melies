// Description : ToolMenu : Correspond to the Menu Tools -> AP -> ... in the Unity Menu bar
#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour {

    /*
	[MenuItem("Tools/AP/Other/Disconnect Prefab",false,0)]
	static void A_DisconnectPrefab()
	{
		if(Selection.activeGameObject != null)
		PrefabUtility.DisconnectPrefabInstance (Selection.activeGameObject);
	}*/

	/*[MenuItem("Tools/AP/Update/Update All",false,0)]
	static void updateAll()
	{
		updateIDs ();

		saveLevelInfos ();

		builinScene ();
	}
*/
// --> Update TextList ID for all the gameObjects that use ItemTextProperties component
	[MenuItem("Tools/AP/Update/Text Manager/Update ID",false,1)]
	static void updateIDs()
	{
        windowMethods _wMethods = new windowMethods();
        _wMethods.updateIDs();


		/*int numberTotal = 0;
		int number = 0;
		GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

		foreach (GameObject go in allObjects) {
			Transform[] Children = go.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in Children) {
				if(child.GetComponent<TextProperties>()){
					Find_UniqueId_In_TextProperties(child);
					number++;
				}
				if(child.GetComponent<VoiceProperties>()){
					Find_UniqueId_In_VoiceProperties (child);
					number++;
				}
				if(child.GetComponent<objTranslateRotate>()){
					Find_UniqueId_In_objRotateTranslate (child);
					number++;
				}
				numberTotal++;
			}
		}
			
		GameObject tmpObj = GameObject.Find ("UpdateManually");

		if (tmpObj) {
			Selection.activeGameObject = tmpObj;
		}

		Debug.Log("Info : " + number +" ID have been updated. There are " + numberTotal + " gameObjects in the scene. " +
			"\n!!! Don't forget to do this operation on each project Scene !!!");
			*/
	}

// --> Update Id for a specific gameObject
	static void Find_UniqueId_In_TextProperties(Transform child){
        windowMethods _wMethods = new windowMethods();
        _wMethods.Find_UniqueId_In_TextProperties(child);

        /*
		if (child.GetComponent<TextProperties> ().textList != null) {
			
			int HowManyEntry = child.GetComponent<TextProperties> ().textList.diaryList [0]._languageSlot.Count;



			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<TextProperties> ().textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<TextProperties> ().uniqueID) {
					//if (child.name == "DebugText")
					//Debug.Log (child.name + " : " + HowManyEntry);


					Undo.RegisterFullObjectHierarchyUndo (child, child.name);

					SerializedObject serializedObject2 = new UnityEditor.SerializedObject (child.GetComponent<TextProperties> ());
					SerializedProperty m_managerID = serializedObject2.FindProperty ("managerID");
	
					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();

					break;
				}
			}
		} else {
			GameObject tmpObj = GameObject.Find ("UpdateManually");

			if (tmpObj) {
				updateManually updateMan = tmpObj.GetComponent<updateManually> ();
				updateMan.listObjs.Add (child.gameObject);
			} else {
				GameObject newObj = new GameObject ();
				newObj.name = "UpdateManually";
				newObj.AddComponent<updateManually> ();
				newObj.GetComponent<updateManually> ().listObjs.Add (child.gameObject);
			}


			//Debug.Log (child.parent.name);
		}
		*/
	}

	// --> Update Id for a specific gameObject
	static void Find_UniqueId_In_VoiceProperties(Transform child){

        windowMethods _wMethods = new windowMethods();
        _wMethods.Find_UniqueId_In_VoiceProperties(child);

		/*if (child.GetComponent<VoiceProperties>().textList != null) {

			int HowManyEntry = child.GetComponent<VoiceProperties>().textList.diaryList [0]._languageSlot.Count;



			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<VoiceProperties>().textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<VoiceProperties>().uniqueID) {
					Undo.RegisterFullObjectHierarchyUndo (child, child.name);

					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<VoiceProperties>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty ("managerID");

					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();

					break;
				}
			}
		}
		else {
			GameObject tmpObj = GameObject.Find ("UpdateManually");

			if (tmpObj) {
				updateManually updateMan = tmpObj.GetComponent<updateManually> ();
				updateMan.listObjs.Add (child.gameObject);
			} else {
				GameObject newObj = new GameObject ();
				newObj.name = "UpdateManually";
				newObj.AddComponent<updateManually> ();
				newObj.GetComponent<updateManually> ().listObjs.Add (child.gameObject);
			}


			//Debug.Log (child.parent.name);
		}
		*/
	}

	static TextList loadTextList(string s_Load){
        windowMethods _wMethods = new windowMethods();
        return _wMethods.loadTextList(s_Load);
        /*
		TextList _textList; 

		string objectPath2 = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _datasProjectManager = AssetDatabase.LoadAssetAtPath (objectPath2, typeof(UnityEngine.Object)) as datasProjectManager;

		string objectPath = "Assets/AP/Assets/Resources/" + _datasProjectManager.currentDatasProjectFolder + "/TextList/" + s_Load + ".asset";
		_textList = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as TextList;



		return _textList;
*/

	}

	// --> Update Id for a specific gameObject
	static void Find_UniqueId_In_objRotateTranslate(Transform child){
        windowMethods _wMethods = new windowMethods();
        _wMethods.Find_UniqueId_In_objRotateTranslate(child);

        /*
		TextList _textList;

	//-> Update : Player Object needed in the Inventory 
        _textList = loadTextList("wItem");

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<objTranslateRotate>().inventoryIDList.Count > 0
					&& _textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<objTranslateRotate>().inventoryIDList[0].uniqueID ) {
					//Debug.Log ("Here : " + i);

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<objTranslateRotate>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty("inventoryIDList").GetArrayElementAtIndex(0).FindPropertyRelative ("ID");

					serializedObject2.Update ();
						m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();
					break;
				}
			}
		}
		else {
			GameObject tmpObj = GameObject.Find ("UpdateManually");

			if (tmpObj) {
				updateManually updateMan = tmpObj.GetComponent<updateManually> ();
				updateMan.listObjs.Add (child.gameObject);
			} else {
				GameObject newObj = new GameObject ();
				newObj.name = "UpdateManually";
				newObj.AddComponent<updateManually> ();
				newObj.GetComponent<updateManually> ().listObjs.Add (child.gameObject);
			}


			//Debug.Log (child.parent.name);
		}

	//-> Update : Voice lock and Unlock
        _textList = loadTextList("wTextnVoices");

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<objTranslateRotate>().diaryIDList.Count > 0
					&& _textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<objTranslateRotate>().diaryIDList[0].uniqueID ) {

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<objTranslateRotate>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty("diaryIDList").GetArrayElementAtIndex(0).FindPropertyRelative ("ID");

					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();
					break;
				}
			}
			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<objTranslateRotate>().diaryIDListUnlock.Count > 0
					&& _textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<objTranslateRotate>().diaryIDListUnlock[0].uniqueID ) {

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<objTranslateRotate>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty("diaryIDListUnlock").GetArrayElementAtIndex(0).FindPropertyRelative ("ID");

					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();
					break;
				}
			}
		}
		else {
			GameObject tmpObj = GameObject.Find ("UpdateManually");

			if (tmpObj) {
				updateManually updateMan = tmpObj.GetComponent<updateManually> ();
				updateMan.listObjs.Add (child.gameObject);
			} else {
				GameObject newObj = new GameObject ();
				newObj.name = "UpdateManually";
				newObj.AddComponent<updateManually> ();
				newObj.GetComponent<updateManually> ().listObjs.Add (child.gameObject);
			}


			//Debug.Log (child.parent.name);
		}

	//-> Update : Feedback lock and Unlock
        _textList = loadTextList("wFeedback");

		if (_textList != null) {
			int HowManyEntry = _textList.diaryList [0]._languageSlot.Count;

			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<objTranslateRotate>().feedbackIDList.Count > 0
					&& _textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<objTranslateRotate>().feedbackIDList[0].uniqueID ) {

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<objTranslateRotate>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty("feedbackIDList").GetArrayElementAtIndex(0).FindPropertyRelative ("ID");

					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();
					break;
				}
			}
			for (var i = 0; i < HowManyEntry; i++) {
				if (child.GetComponent<objTranslateRotate>().feedbackIDListUnlock.Count > 0
					&& _textList.diaryList [0]._languageSlot [i].uniqueItemID == child.GetComponent<objTranslateRotate>().feedbackIDListUnlock[0].uniqueID ) {

					Undo.RegisterFullObjectHierarchyUndo (child, child.name);
					SerializedObject serializedObject2 			= new UnityEditor.SerializedObject (child.GetComponent<objTranslateRotate>());
					SerializedProperty m_managerID 				= serializedObject2.FindProperty("feedbackIDListUnlock").GetArrayElementAtIndex(0).FindPropertyRelative ("ID");

					serializedObject2.Update ();
					m_managerID.intValue = i;
					serializedObject2.ApplyModifiedProperties ();
					break;
				}
			}
		}
		else {
			GameObject tmpObj = GameObject.Find ("UpdateManually");

			if (tmpObj) {
				updateManually updateMan = tmpObj.GetComponent<updateManually> ();
				updateMan.listObjs.Add (child.gameObject);
			} else {
				GameObject newObj = new GameObject ();
				newObj.name = "UpdateManually";
				newObj.AddComponent<updateManually> ();
				newObj.GetComponent<updateManually> ().listObjs.Add (child.gameObject);
			}


			//Debug.Log (child.parent.name);
		}
*/
	}


	[MenuItem("Tools/AP/Update/Save System/Create a list of objects to save in the current scene",false,2)]
	static void saveLevelInfos()
	{
        windowMethods _wMethods = new windowMethods();
        _wMethods.saveLevelInfos();

        /*
		List<GameObject> listGameObject = new List<GameObject> ();
		List<string> listString = new List<string> ();

		int numberTotal = 0;
		int number = 0;
		GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

		foreach (GameObject go in allObjects) {
			Transform[] Children = go.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in Children) {
				if(child.GetComponent<SaveData>()){
					//Find_UniqueId_In_The_TextList (child);
					listGameObject.Add(child.gameObject);
					listString.Add (child.GetComponent<SaveData> ().R_SaveData ());

					number++;
				}
				numberTotal++;
			}
		}
		GameObject tmp = GameObject.Find ("LevelManager");
		string tmpString = "";
		if (tmp) {
			Undo.RegisterFullObjectHierarchyUndo (tmp, tmp.name);

			LevelManager levelManager = tmp.GetComponent<LevelManager> ();

			levelManager.listOfGameObjectForSaveSystem.Clear ();
			levelManager.listState.Clear ();

			for (var i = 0; i < listGameObject.Count; i++) {
				levelManager.listOfGameObjectForSaveSystem.Add(listGameObject[i]);
				levelManager.listState.Add(false);
				tmpString += listString [i];
			}

		} else {
			//Debug.Log ("Info : You need a LevelManager in your scene to be allowed to save data for this level");
			if (EditorUtility.DisplayDialog ("Info : This action is not possible."
				, "You need an object LevelManager in your scene to record data for this level. LevelManager need to have LevelManager.cs attached to it."
				, "Continue")) {}

		}
*/
	}

//--> Create a list of scenes used in the Save System
	[MenuItem("Tools/AP/Update/Save System/Create a list of build in scenes",false,2)]
	static void builinScene()
	{
        windowMethods _wMethods = new windowMethods();
        _wMethods.builinScene(false, false);

        /*
		string objectPath = "Assets/AP/Assets/Datas/ProjectManagerDatas.asset";
		datasProjectManager _ProjectManagerDatas = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as datasProjectManager;

		if (_ProjectManagerDatas) {
			Undo.RegisterFullObjectHierarchyUndo (_ProjectManagerDatas, _ProjectManagerDatas.name);


			_ProjectManagerDatas.buildinList.Clear ();
			for (var i = 0; i < EditorBuildSettings.scenes.Length; i++) {
				string scenName = EditorBuildSettings.scenes [i].path;
				scenName = scenName.Replace (".unity", "");

				for(var j = scenName.Length-1;j > 1;j--){					
					char tmp = scenName[j];

					if(tmp == '/'){
						scenName = scenName.Substring (j+1, scenName.Length - 1 - j);
						break;
					}
				}

				_ProjectManagerDatas.buildinList.Add (scenName);

			}
		}
		else {
			if (EditorUtility.DisplayDialog ("Info : This action is not possible."
				, "You need an object datasProjectManager in your Project Tab : Assets/AP/Assets/Datas/ProjectManagerDatas.asset."
				, "Continue")) {}

		}
*/
	}

//--> create Text List
	/*[MenuItem("Assets/Create/Text List")]
	public static TextList  Create()
	{
		TextList asset = ScriptableObject.CreateInstance<TextList>();

		AssetDatabase.CreateAsset(asset, "Assets/TestList.asset");
		AssetDatabase.SaveAssets();
		return asset;
	}

//--> Create datas use to know the object that needed to be update/Saved when scene start or closed
	[MenuItem("Assets/Create/Level Save System")]
	public static LevelSaveSystem  F_LevelSaveSystem()
	{
		LevelSaveSystem asset = ScriptableObject.CreateInstance<LevelSaveSystem>();

		AssetDatabase.CreateAsset(asset, "Assets/AP/Assets/Datas/Level/" + SceneManager.GetActiveScene ().name + ".asset");
		AssetDatabase.SaveAssets();
		return asset;
	}
    */
//--> Create Data : Manage the global Project Preference
    /*
	[MenuItem("Assets/Create/Project Data")]
	public static datasProjectManager  F_DataProjectManager()
	{
		datasProjectManager asset = ScriptableObject.CreateInstance<datasProjectManager>();

		AssetDatabase.CreateAsset(asset, "Assets/AP/Assets/Datas/ProjectManagerDatas.asset");
		AssetDatabase.SaveAssets();
		return asset;
	}
*/

    //--> Create Data : Manage the global Project Preference
/*
    [MenuItem("Assets/Create/Window ReadyToUse Data")]
    public static datasWindowReadyToUse  F_DataProjectManager()
    {
        datasWindowReadyToUse asset = ScriptableObject.CreateInstance<datasWindowReadyToUse>();

        AssetDatabase.CreateAsset(asset, "Assets/AP/Assets/Datas/windowReadyToUseDatas.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
*/

//--> Desktop : Mobile Options/Mobile
	[MenuItem("Tools/AP/Switch Platform /To Mobile")]
	static void  switchToMobile()
	{
		switchPlatform (false, true,"Mobile",false);
	}

//--> Desktop : Mobile Options/Desktop
	[MenuItem("Tools/AP/Switch Platform /To Desktop")]
	static void  switchToDesktop()
	{
		switchPlatform (true, false,"Desktop",true);
	}

	static void switchPlatform(bool b_ReticuleState,bool b_Canvas_MobileState,string whichSwitch,bool inputState){
		string feedback = "";
		bool b_objectFind = false;

		GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

		//-> Find the reticule
		for (var i = 0; i < allObjects.Length; i++) {
			Transform[] Children = allObjects[i].GetComponentsInChildren<Transform>(true);
			for (var j = 0; j < Children.Length; j++) {
				if(Children[j].name == "Reticule"){
					Undo.RegisterFullObjectHierarchyUndo (Children[j], Children[j].name);
					Children[j].gameObject.SetActive (b_ReticuleState);
					feedback += "Reticule Ok, ";
					b_objectFind = true;
					break;
				}
			}
		}


	
		if(!b_objectFind)feedback += "Reticule not found, ";
		b_objectFind = false;

		//-> Find the canvas_Mobile
		for (var i = 0; i < allObjects.Length; i++) {
			Transform[] Children = allObjects[i].GetComponentsInChildren<Transform>(true);
			for (var j = 0; j < Children.Length; j++) {
				if(Children[j].name == "Canvas_Mobile"){
					Undo.RegisterFullObjectHierarchyUndo (Children[j], Children[j].name);
					Children[j].gameObject.SetActive (b_Canvas_MobileState);
					feedback += "canvas Mobile Ok, ";
					b_objectFind = true;
					break;
				}
			}
		}

		if(!b_objectFind)feedback += "canvas Mobile not found, ";
		b_objectFind = false;


		GameObject _inGameGlobalManager = GameObject.Find ("ingameGlobalManager");

		if (_inGameGlobalManager) {
			Undo.RegisterFullObjectHierarchyUndo (_inGameGlobalManager, _inGameGlobalManager.name);
			_inGameGlobalManager.GetComponent<ingameGlobalManager> ().b_DesktopInputs = inputState;
			feedback += "ingameGlobalManager Ok ";
			b_objectFind = true;
		}

		if(!b_objectFind)feedback += "ingameGlobalManager not found ";
		b_objectFind = false;

		if(whichSwitch == "Mobile")
			switchCanvasMenuToMobile ();
		else
			switchCanvasMenuToDesktop();


        PlayerPrefs.DeleteAll();
		Debug.Log ("INFO : Switch to " + whichSwitch + " finished" + " : " + feedback);

	}

	static void switchCanvasMenuToDesktop (){
		GameObject canvas_MainMenu = GameObject.Find ("Canvas_MainMenu");
		if (canvas_MainMenu) {

			SerializedObject serializedObject2 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>());
			serializedObject2.Update ();
			SerializedProperty m_b_DesktopOrMobile = serializedObject2.FindProperty ("b_DesktopOrMobile");
			m_b_DesktopOrMobile.boolValue = false;

			serializedObject2.ApplyModifiedProperties ();


			for (int m = 0; m < canvas_MainMenu.GetComponent<Menu_Manager>().List_GroupCanvas.Count; m++) {
				for (int i = 0; i < canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject.Count; i++) {


					if (!canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].Desktop) {
						if (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList) {
							SerializedObject serializedObject3 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList);
							serializedObject3.Update ();
							SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
							tmpSer2.boolValue = false;
							serializedObject3.ApplyModifiedProperties ();
						}
					} 
					else {
						if (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList) {
							SerializedObject serializedObject3 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList);
							serializedObject3.Update ();
							SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
							tmpSer2.boolValue = true;
							serializedObject3.ApplyModifiedProperties ();
						}
					}
				}
			}
			GameObject eventSystem = GameObject.Find ("EventSystem");
			if(eventSystem)Selection.activeGameObject = eventSystem;

			Button[] allUIButtons = canvas_MainMenu.GetComponentsInChildren<Button>(true);

			foreach (Button _button in allUIButtons) {
				Undo.RegisterFullObjectHierarchyUndo (_button.gameObject, _button.name);


				SerializedObject serializedObject3 = new UnityEditor.SerializedObject (_button.gameObject.GetComponent<Button>());
				serializedObject3.Update ();
				SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_Transition");
				tmpSer2.enumValueIndex = 2;															//_button.transition =  Selectable.Transition.SpriteSwap;
				serializedObject3.ApplyModifiedProperties ();
			}
		}
	}


	static void switchCanvasMenuToMobile (){
		GameObject canvas_MainMenu = GameObject.Find ("Canvas_MainMenu");
		if (canvas_MainMenu) {

			SerializedObject serializedObject2 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>());
			serializedObject2.Update ();
			SerializedProperty m_b_DesktopOrMobile = serializedObject2.FindProperty ("b_DesktopOrMobile");
			m_b_DesktopOrMobile.boolValue = true;

			serializedObject2.ApplyModifiedProperties ();


			for (int m = 0; m < canvas_MainMenu.GetComponent<Menu_Manager>().List_GroupCanvas.Count; m++) {
				for (int i = 0; i < canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject.Count; i++) {


					if (!canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].Desktop) {
						if (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList) {
							SerializedObject serializedObject3 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList);
							serializedObject3.Update ();
							SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
							tmpSer2.boolValue = true;
							serializedObject3.ApplyModifiedProperties ();
						}
					} 
					else {
						if (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList) {
							SerializedObject serializedObject3 = new UnityEditor.SerializedObject (canvas_MainMenu.GetComponent<Menu_Manager>().list_gameObjectByPage[m].listOfMenuGameobject[i].objList);
							serializedObject3.Update ();
							SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_IsActive");
							tmpSer2.boolValue = false;
							serializedObject3.ApplyModifiedProperties ();
						}
					}
				}
			}



			GameObject eventSystem = GameObject.Find ("EventSystem");
			if(eventSystem)Selection.activeGameObject = eventSystem;

			Button[] allUIButtons = canvas_MainMenu.GetComponentsInChildren<Button>(true);

			foreach (Button _button in allUIButtons) {
				Undo.RegisterFullObjectHierarchyUndo (_button.gameObject, _button.name);

				SerializedObject serializedObject3 = new UnityEditor.SerializedObject (_button.gameObject.GetComponent<Button>());
				serializedObject3.Update ();
				SerializedProperty tmpSer2 = serializedObject3.FindProperty ("m_Transition");
				tmpSer2.enumValueIndex = 0;														//_button.transition =  Selectable.Transition.none;
				serializedObject3.ApplyModifiedProperties ();
			}

		}
	}

    [MenuItem("Tools/AP/Datas Managers/All Data windows", false, 0)]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Item));
        EditorWindow.GetWindow(typeof(w_UI));
        EditorWindow.GetWindow(typeof(w_Feedback));
        EditorWindow.GetWindow(typeof(w_TextnVoice));
    }

//--> custom AP Button from the Hierarchy
	[MenuItem ("GameObject/UI/AP Creator/Button")]
	public static void  createCustomButton()
	{


		string objectPath = "Assets/AP/Assets/Prefab/UI/ButtonDefault.prefab";
		GameObject refButton = AssetDatabase.LoadAssetAtPath (objectPath, typeof(UnityEngine.Object)) as GameObject;

		if (Selection.activeGameObject == null) {
			if (EditorUtility.DisplayDialog ("Info : This action is not possible."
				, "You need to select a canvas menu in the Hierarchy"
				, "Continue")) {}
		}
		else{
			
			GameObject newButton = Instantiate (refButton, Selection.activeGameObject.transform);
			Undo.RegisterCreatedObjectUndo (newButton, newButton.name);
			Selection.activeGameObject = newButton;
		}

	}

	//--> delete all playerPrefs
    [MenuItem ("Tools/AP/Other/Delete All PlayerPrefs")]
	public static void  deleteAllPlayerPrefs()
	{
		PlayerPrefs.DeleteAll ();

	}
}
#endif