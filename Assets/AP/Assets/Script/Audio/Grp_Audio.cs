// Description : Grp_Audio : Create only one instance of this group even if a new scene is loaded with the same gameObject
// Static instance of GameManager which allows it to be accessed by any other script.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grp_Audio : MonoBehaviour {

	public static Grp_Audio 	instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.


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
	}

	void Start(){
		DontDestroyOnLoad (gameObject);
	}
	

}
