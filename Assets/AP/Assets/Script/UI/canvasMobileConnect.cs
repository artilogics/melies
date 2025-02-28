//Decription : canvasMobileConnect : Use for Mobile Inputs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;

public class canvasMobileConnect : MonoBehaviour {
	public GameObject           canvas_Mobile;
	public characterMovement    currentCharacter;
    public VirtualController    virtualJoystick;                    // System 1: Right stick camera
    public GameObject           grp_LeftButtonsMove;                // System 1: Left buttons Forward/Backward/Left/Right
    public VirtualController    virtualJoystickLeftStickToMove;     // System 2: Left Stick Forward/Backward/Left/Right
    private inventoryALLEntry _inventoryALLEntry;

	public Torch Flashlight;
	public int flashlightID = 0;

	public void Start()
    {
		Torch[] allTorch = FindObjectsByType<Torch>(FindObjectsSortMode.None);
		foreach (Torch child in allTorch)
			if (flashlightID == child.torchID) Flashlight = child;
	}

	//--> Display Main Menu
	public void displayMainMenu () {
		ingameGlobalManager.instance.GetComponent<backInputs> ().MainMenuIngame ();
	}

    //--> Display Inventory
    public void displayInventory()
    {
        if(_inventoryALLEntry == null)
            _inventoryALLEntry = GameObject.Find("inventoryAllEntryManager").GetComponent<inventoryALLEntry>();
        
        _inventoryALLEntry.showInventory(true);
    }

	public void initializedCanvasMobile(){

		if (ingameGlobalManager.instance.currentPlayer.GetComponent<characterMovement> ()) {
			currentCharacter = ingameGlobalManager.instance.currentPlayer.GetComponent<characterMovement> ();


			currentCharacter.mobileToystickController = virtualJoystick;
            currentCharacter.mobileLeftJoystickToMove = virtualJoystickLeftStickToMove;


			currentCharacter.StopMoving ();
			currentCharacter.pointerUp ();
		}
	}


    // --> System 1: Move the camera using the vitual joystick
	public void F_pointerDrag(){
		currentCharacter.pointerDrag ();
	}

	public void F_pointerUp(){
		currentCharacter.pointerUp ();
	}


	// Use for Mobile : Player move forward
	public void F_MoveForward(){
		currentCharacter.MoveForward ();
	}
	// Use for Mobile : Player move backward
	public void F_MoveBackward(){
		currentCharacter.MoveBackward ();
	}

	// Use for Mobile : Player move Left
	public void F_MoveLeft(){
		currentCharacter.MoveLeft ();
	}
	// Use for Mobile : Player move right
	public void F_MoveRight(){
		currentCharacter.MoveRight ();
	}
	// Use for Mobile : Player Stop moving when button is released
	public void F_StopMoving(){
		currentCharacter.StopMoving ();
	}
    //  Use for Mobile : Player is crouching
    public void AP_F_Crouch()
    {
        currentCharacter.AP_Crouch();
    }


    // Second System
    // --> Move the camera using the vitual joystick
    public void F_pointerDrag_MoveWithLeftStick()
    {
        currentCharacter.pointerDrag_MoveWithLeftStick();
    }

    public void F_pointerUp_MoveWithLeftStick()
    {
        currentCharacter.pointerUp_MoveWithLeftStick();
    }
	//  Use for Mobile : Player switch On/Off Flashlight
	public void AP_F_Flashlight()
	{
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
			if (Flashlight.objTorch && Flashlight.bActionAvailable)
			{
				Flashlight.bTorchState = !Flashlight.bTorchState;
				StartCoroutine(Flashlight.MoveTorchRoutine());
			}
		}
    }

	//  Use for Mobile : Start/Stop player Run
	public void AP_F_Run()
	{
		if (ingameGlobalManager.instance.b_InputIsActivated && currentCharacter)
		{
			currentCharacter.bMobileRun = !currentCharacter.bMobileRun;
		}
	}

	//  Use for Mobile : Player Jump
	public void AP_F_Jump()
	{
		if (ingameGlobalManager.instance.b_InputIsActivated)
		{
			characterMovement characterMovement = ingameGlobalManager.instance.currentPlayer.GetComponent<characterMovement>();

			if (characterMovement &&
                characterMovement.b_AllowJump &&
                !characterMovement.b_IsJumping &&
                characterMovement.isOnFloor)
			{
				StartCoroutine(characterMovement.Jump());
			}
		}
	}
}
