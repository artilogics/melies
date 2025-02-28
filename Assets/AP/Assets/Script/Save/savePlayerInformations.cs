// Description : savePlayerInformations. You find this script on character gameObject. It allow to save Player character parameters.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class savePlayerInformations : MonoBehaviour {

	public GameObject 	character;						// Connect the character gameobject
	public Transform 	playerCamera;					// Connect the player camera

//--> save the character parameters
	public string saveChararcterParams () {
		CultureInfo cultureInfo = new CultureInfo("en-US");
		System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

		string result = "";
        GameObject _Head = GameObject.FindWithTag("Head");

		if (character && playerCamera) {
			result += character.transform.localPosition.x + "_";					// Player Body X Position
			result += character.transform.localPosition.y + "_";					// Player Body Y Position
			result += character.transform.localPosition.z + "_";					// Player Body Z Position

			result += character.transform.localEulerAngles.x + "_";					// Player Body X Rotation
			result += character.transform.localEulerAngles.y + "_";					// Player Body Y Rotation
			result += character.transform.localEulerAngles.z + "_";					// Player Body Z Rotation

			result += character.GetComponent<Rigidbody> ().isKinematic + "_";		// Collider state
			result += character.GetComponent<CapsuleCollider> ().isTrigger + "_";	// Collider state

            result += _Head.transform.localPosition.x + "_";					// Player Camera X Position
            result += _Head.transform.localPosition.y + "_";					// Player Camera Y Position
            result += _Head.transform.localPosition.z + "_";					// Player Camera Z Position

            result += _Head.transform.localEulerAngles.x + "_";				// Player Camera X Rotation
            result += _Head.transform.localEulerAngles.y + "_";				// Player Camera Y Rotation
            result += _Head.transform.localEulerAngles.z + "_";				// Player Camera Z Rotation



            result += character.GetComponent<characterMovement> ().mouseY + "_";			// Mouse Y Value to prevent bug when the game is loaded (Huge mouse movement)

            if (character.GetComponent<characterMovement>().b_Crouch)
                result += "T";             // Player is crouching
            else
                result += "F";
		}
			
		return result;
	}
	
//--> load the character parameters
	public bool loadChararcterParams (string s_Datas) {
		System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
		GameObject _Head = GameObject.FindWithTag("Head");
		if (s_Datas != "") {
			string[] codes = s_Datas.Split ('_');
           
            if (codes[15] == "T")   // The Character is crouching
            {
                character.GetComponent<characterMovement>().b_Crouch = true;
                character.transform.localScale = new Vector3(character.transform.localScale.x,
                                                             character.GetComponent<characterMovement>().targetScaleCrouch,
                                                             character.transform.localScale.z);
            }


			for (var i = 0; i < codes.Length; i++) {
				character.transform.localPosition = new Vector3 (float.Parse (codes [0]), float.Parse (codes [1]), float.Parse (codes [2]));			// Update body position
				character.transform.localEulerAngles = new Vector3 (float.Parse (codes [3]), float.Parse (codes [4]), float.Parse (codes [5]));			// Update body Rotation
			}

			if (codes [6] == "True") {
                character.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
				character.GetComponent<Rigidbody> ().isKinematic = true;
			} else {
                character.GetComponent<Rigidbody>().isKinematic = false;
                character.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
			}

			if (codes [7] == "True") {
				character.GetComponent<CapsuleCollider> ().isTrigger = true;
			} else {
				character.GetComponent<CapsuleCollider> ().isTrigger = false;
			}

			for (var i = 0; i < codes.Length; i++) {
                _Head.transform.localPosition = new Vector3 (float.Parse (codes [8]), float.Parse (codes [9]), float.Parse (codes [10]));		// Update camera position
                _Head.transform.localEulerAngles = new Vector3 (float.Parse (codes [11]), float.Parse (codes [12]), float.Parse (codes [13]));	// Update camera Rotation
			}

			character.GetComponent<characterMovement> ().mouseY = float.Parse (codes [14]);

            playerCamera.transform.position = _Head.transform.position;        // Update camera position
            playerCamera.transform.eulerAngles = _Head.transform.eulerAngles; // Update camera Rotation

           

		} 
	//->Use the Scene Spawn Position to init Player Position
		else {
			GameObject spawnPosition = GameObject.FindGameObjectWithTag ("spawnPosition");
			if(spawnPosition){
				character.transform.position = spawnPosition.transform.position;					// Update body position
				character.transform.eulerAngles = spawnPosition.transform.eulerAngles;				// Update body Rotation


                playerCamera.transform.position = _Head.transform.position;                         // Update camera position
                playerCamera.transform.eulerAngles = _Head.transform.eulerAngles;                   // Update camera Rotation
			}

		}

        character.GetComponent<characterMovement>().isOnFloor = false;
		return true;
	}


//--> init character parameters when player is teleported to another scene
	public bool initChararcterParamsTeleport (Transform newSpawnPoint) {
        
        GameObject _Head = GameObject.FindWithTag("Head");

        character.transform.position = newSpawnPoint.transform.position;
        character.transform.eulerAngles = newSpawnPoint.transform.eulerAngles;


        _Head.transform.localEulerAngles = new Vector3(0,0,0);

        playerCamera.transform.position = _Head.transform.position;        // Update camera position
        playerCamera.transform.eulerAngles = _Head.transform.eulerAngles; // Update camera Rotation

		


		character.GetComponent<characterMovement> ().mouseY = 0;

		ingameGlobalManager.instance.b_AllowCharacterMovment = true;


		character.GetComponent<Rigidbody> ().isKinematic = false;
        character.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
		character.GetComponent<CapsuleCollider> ().enabled = true;

        character.GetComponent<characterMovement>().isOnFloor = false;

		return true;
	}
}
