// Desciption : deactivateOnStart : Deactivate the object that contains this script on Awake.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivateOnStart : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		gameObject.SetActive(false);
	}
	

}
