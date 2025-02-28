//Description : manipulateSave : Collection of methods use in SaveDataEditor
#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class manipulateSave {

	public void updateObjectsSaveList()
	{
		List<GameObject> listGameObject = new List<GameObject> ();
		List<string> listString = new List<string> ();

		int numberTotal = 0;
		int number = 0;
		GameObject[] allObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();

		foreach (GameObject go in allObjects) {
			Transform[] Children = go.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in Children) {
				if(child.GetComponent<SaveData>()){
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
			LevelManager levelManager = tmp.GetComponent<LevelManager> ();
			levelManager.listOfGameObjectForSaveSystem.Clear ();
			levelManager.listState.Clear ();
			for (var i = 0; i < listGameObject.Count; i++) {
				levelManager.listOfGameObjectForSaveSystem.Add(listGameObject[i]);
				tmpString += listString [i];
				levelManager.listState.Add (false);
			}
		} else {
			if (EditorUtility.DisplayDialog ("Info : This action is not possible."
				, "You need a object LevelManager in your scene to record data for this level. LevelManager need to have LevelManager.cs attached to it."
				, "Continue")) {}

		}
	}
}
#endif