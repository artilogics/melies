// Desciption : destroyObj : Destroy the object that contains this script on Awake.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyObj : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		Destroy (this.gameObject);
	}
}
