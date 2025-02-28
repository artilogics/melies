//  Description : LevelManager. You find this script on each scene except MainMenu scene (Hierarchy Tab : Managers -> LevelManager)
// Use to initialized gameObjects in a specific scene
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;

public class LevelManager : MonoBehaviour {

	public List<GameObject> listOfGameObjectForSaveSystem = new  List<GameObject>();	// List of gameObjects that need to be initialized when game is loaded
	public List<bool> 		listState = new  List<bool>();								// Know if each object is initialized after loading process

//--> return if all the gameobject have been initialized correctly
	public bool allObjectsInitiliazed(){
		bool result = true;
		for (var i = 0; i < listState.Count; i++) {
			if (listState [i] == false){
				result = false;
				break;
			}
		}

		return result;
	}
}


