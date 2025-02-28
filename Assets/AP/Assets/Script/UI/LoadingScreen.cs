// Description : LoadingScreen : Use to have only one Loading in the Hierarchy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

	public static LoadingScreen 	instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
	public Menu_Manager				menuManager;


	void Awake()
	{
		if (instance == null)			//Check if instance already exists
			instance = this;			//if not, set instance to this

		else if (instance != this)		//If instance already exists and it's not this:
			Destroy(gameObject);  		//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
	}


	void Start(){
		DontDestroyOnLoad (gameObject);
	}
}
