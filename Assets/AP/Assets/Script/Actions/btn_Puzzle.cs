// Description : btn_Puzzle : use to manage the canvas puzzle Icons
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btn_Puzzle : MonoBehaviour {

    public GameObject currentPuzzle;

	// Use this for initialization
	void Start () {
		
	}
	
    public void focusPuzzle(){
        //Debug.Log("puzzle");
        if(!ingameGlobalManager.instance.b_focusModeIsActivated)        // Focus not activated
            currentPuzzle.GetComponent<conditionsToAccessThePuzzle>().F_ActivateFocus(currentPuzzle);
      

    }
}
