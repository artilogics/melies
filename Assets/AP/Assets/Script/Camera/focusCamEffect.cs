// Description : CameraManager : Use to make a focus on a gameobject
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class focusCamEffect : MonoBehaviour {
	public 	bool 				SeeInspector = false;
	private GameObject 			Head;
	public 	Transform 			targetFocusCamera;
	private Vector3 			oldPosition;
	private Quaternion 			oldQuaternion;

	private GameObject 			character;
	private Rigidbody 			rbCharacter;

	//private AnimationCurve 		animCurve;
    private float                speed = 1.5f ;


	public bool 				b_Child = true;								// if child send message to InGameManager -> script focusCamEffect.cs

    public bool                 b_MovementEnded = true;
    public int                  movementState = 0;   // 0 : Init 1 : Move to focus Position 2 : Move to init Position

    public AudioSource          _audio;
    public AudioClip            a_FocusIn;
    public AudioClip            a_FocusOut;
    public float                volume_FocusIn = 1;
    public float                volume_FocusOut = 1;

    public Transform objRef;


	void Start(){
		Init ();
	}


	public void Init(){
		Head = GameObject.FindGameObjectWithTag ("Head");
		character 		= GameObject.FindGameObjectWithTag ("Player");
		if(character)rbCharacter 	= character.GetComponent<Rigidbody> ();
        if (character && character.GetComponent<characterMovement>().refHead)
            objRef = character.GetComponent<characterMovement>().refHead;

		//createAnimCurve ();
	}



  





// --> Activate Focus Camera Position
	public void MoveCameraToFocusPosition(Transform trans,bool b_ActivateDoubleTapIcon){
		if (b_Child) {
            ingameGlobalManager.instance.GetComponent<focusCamEffect> ().MoveCameraToFocusPosition (trans,b_ActivateDoubleTapIcon);
		} else {
            rbCharacter.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rbCharacter.isKinematic = true;

			ingameGlobalManager.instance.b_AllowCharacterMovment = false;
			ingameGlobalManager.instance.currentPlayer.GetComponent<CapsuleCollider> ().isTrigger = true;

			targetFocusCamera = trans;

			StopAllCoroutines ();
            StartCoroutine (I_MoveCameraToFocusPosition(b_ActivateDoubleTapIcon));
		}
	}


   
    IEnumerator I_MoveCameraToFocusPosition(bool b_ActivateDoubleTapIcon){

        if(a_FocusIn && _audio){
            _audio.clip = a_FocusIn;
            _audio.volume = volume_FocusIn;
            _audio.Play();
        }


        ingameGlobalManager.instance.b_focusModeIsActivated = true;
        b_MovementEnded = false;
        movementState = 1;
		if(ingameGlobalManager.instance.b_DesktopInputs)
			ingameGlobalManager.instance.reticule.SetActive (false);
		
		//ingameGlobalManager.instance.canvasPlayerInfos.deactivateInteractiveIcons();
		ingameGlobalManager.instance.canvasPlayerInfos.deactivateIcons(false);

		//--> Display available actions on screen
        if(!b_ActivateDoubleTapIcon && !ingameGlobalManager.instance.b_DesktopInputs)   // Mobile case : Do not activate double tap icon when a puzzle starts
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
        else 
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
		oldPosition = Head.transform.position;
		oldQuaternion = Head.transform.rotation;
	
		var t = 0f;

		bool b_once = false;

		if (ingameGlobalManager.instance.canvasMobileInputs
			&& ingameGlobalManager.instance.canvasMobileInputs.activeSelf) 
			ingameGlobalManager.instance.canvasMobileInputs.SetActive (false);



        objRef.rotation = Head.transform.rotation;


        while(t < 1){
			if (!ingameGlobalManager.instance.b_Ingame_Pause) {
				//t += Time.deltaTime *.25f;
                t = Mathf.MoveTowards(t, 1, Time.deltaTime * speed);

                Head.transform.position = new Vector3(Mathf.Lerp(objRef.position.x, targetFocusCamera.position.x, t),
                                                      Mathf.Lerp(objRef.position.y, targetFocusCamera.position.y, t),
                                                      Mathf.Lerp(objRef.position.z, targetFocusCamera.position.z, t));

                Head.transform.rotation = Quaternion.Lerp(objRef.rotation, targetFocusCamera.rotation, t);


				if (t > .5f) {
					if (!b_once) {
						StartCoroutine (I_FocusStart_ActivateIcon ());
						b_once = true;


					}
				}
			}
            yield return null;
		}

        b_MovementEnded = true;
        //Debug.Log ("Focus Ok " + gameObject.name);
        yield return new WaitForEndOfFrame();

        //-> Deactivate the cursor and change lock state to confine (Focus Only case)
        if (!ingameGlobalManager.instance.b_Joystick &&
            ingameGlobalManager.instance.b_DesktopInputs &&
            ingameGlobalManager.instance.currentPuzzle &&
            ingameGlobalManager.instance.currentPuzzle.gameObject.GetComponent<focusOnly>())
        {
            ingameGlobalManager.instance.StartCoroutine(ingameGlobalManager.instance.changeLockStateConfined(false));
        }
        //-> Activate the cursor and change lock state to confine.
        else
        {
            ingameGlobalManager.instance.StartCoroutine(ingameGlobalManager.instance.changeLockStateConfined(true));            
        }
       
		yield return null;
	}

    IEnumerator I_FocusStart_ActivateIcon()
    {
        var t = 0f;

        while (t < .1f)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t += Time.deltaTime;
            }
            yield return null;
        }

        ingameGlobalManager.instance.canvasPlayerInfos.interactiveIconAvailable = false;
        ingameGlobalManager.instance.canvasPlayerInfos.activateIcons();												// Activate Item UI Icon


           
	}

	IEnumerator I_FocusEnded_ActivateIcon(){
		var t = 0f;
		while(t < .1f)
		{
			if (!ingameGlobalManager.instance.b_Ingame_Pause) {
				t += Time.deltaTime;
			}
			yield return null;
		}


		ingameGlobalManager.instance.canvasPlayerInfos.activateIcons ();
	}


// --> Go back to Init Camera Position
	public void MoveCameraToDefaultPosition(){
        
		if (b_Child) {
			ingameGlobalManager.instance.GetComponent<focusCamEffect> ().MoveCameraToDefaultPosition ();
		} else {
			
			StopAllCoroutines ();
			StartCoroutine ("I_MoveCameraToDefaultPosition");
		}
	}

	IEnumerator I_MoveCameraToDefaultPosition(){

        if (a_FocusOut && _audio)
        {
            _audio.clip = a_FocusIn;
            _audio.volume = volume_FocusOut;
            _audio.Play();
        }


        movementState = 2;
        b_MovementEnded = false;
		ingameGlobalManager.instance.StartCoroutine( ingameGlobalManager.instance.changeLockStateLock ());
		ingameGlobalManager.instance.canvasPlayerInfos.deactivateIcons(false);
		ingameGlobalManager.instance.canvasPlayerInfos.activateInteractiveIcons ();



		var t = 0f;



        while(t < 1)
		{
			if (!ingameGlobalManager.instance.b_Ingame_Pause) {
                //t += Time.deltaTime * .25f;
                t = Mathf.MoveTowards(t, 1, Time.deltaTime * speed);



                Head.transform.position = new Vector3(Mathf.Lerp(targetFocusCamera.position.x, objRef.position.x, t),
                                                      Mathf.Lerp(targetFocusCamera.position.y, objRef.position.y, t),
                                                      Mathf.Lerp(targetFocusCamera.position.z, objRef.position.z, t));

                Head.transform.rotation = Quaternion.Lerp(targetFocusCamera.rotation, objRef.rotation, t);



			}
            yield return null;
			//Debug.Log (t);
		}

		if (ingameGlobalManager.instance.canvasMobileInputs
			&& !ingameGlobalManager.instance.b_DesktopInputs) 
				ingameGlobalManager.instance.canvasMobileInputs.SetActive (true);
			
	//-> Reactivate player character if the gameobject movement is a translation or end of puzzle focus. For Rotation player activation is in Ienumerator I_Rotate()
		if (ingameGlobalManager.instance.currentobjTranslateRotate && ingameGlobalManager.instance.currentobjTranslateRotate.movementType == 1
            || ingameGlobalManager.instance.currentobjTranslateRotate == null && ingameGlobalManager.instance.currentPuzzle == null) {  
			ingameGlobalManager.instance.currentPlayer.GetComponent<CapsuleCollider> ().isTrigger = false;

			rbCharacter.isKinematic = false;
            rbCharacter.collisionDetectionMode = CollisionDetectionMode.Continuous;
			ingameGlobalManager.instance.b_AllowCharacterMovment = true;

			//ingameGlobalManager.instance.b_focusModeIsActivated = false;
			if (ingameGlobalManager.instance.b_DesktopInputs)
				ingameGlobalManager.instance.reticule.SetActive (true);
		}
        //Debug.Log ("Ended");
        b_MovementEnded = true;

        ingameGlobalManager.instance.currentobjTranslateRotate = null;
        ingameGlobalManager.instance.b_focusModeIsActivated = false;
		StartCoroutine (I_FocusEnded_ActivateIcon ());
		yield return null;
	}
		


//--> create a custom animation Curve
	/*private void createAnimCurve ()	{
		//->Set the curve
		Keyframe[] ks 		= new Keyframe[3];

		ks [0] 				= new Keyframe (0, 0);
		ks [0].outTangent 	= 0;         		

		ks [1] 				= new Keyframe (.902f, .632f);
		ks [1].inTangent 	= 1.5708f;   
		ks [1].outTangent 	= 1.5708f;     

		ks [2] 				= new Keyframe (1, 1);
		ks [2].outTangent 	= 1.5708f;        

		//->Create curve
		animCurve = new AnimationCurve (ks);
	}*/
		

	public void loadFocusModeInformations(List<string> s_Datas){
		//string[] codes = s_Datas.Split (',');
		//Debug.Log(s_Datas);


		oldPosition = new Vector3 (float.Parse(s_Datas [0]),float.Parse(s_Datas [1]), float.Parse(s_Datas [2]));
		oldQuaternion = new Quaternion (float.Parse(s_Datas [3]),float.Parse(s_Datas [4]), float.Parse(s_Datas [5]), float.Parse(s_Datas [6]));


	}

	public string saveFocusModeInformations(){
		string result = "";
		string separator = ",";

		if(targetFocusCamera != null)
			result	+= targetFocusCamera.name + separator;
		else
			result	+= "null" + separator;	

		result += oldPosition.x + separator;		// X Position
		result += oldPosition.y + separator;		// Y Position
		result += oldPosition.z + separator;		// Z Position

		result += oldQuaternion.x + separator;	// X Rotation
		result += oldQuaternion.y + separator;	// Y Rotation
		result += oldQuaternion.z + separator;	// Z Rotation
		result += oldQuaternion.w;			// W Rotation

		return result;
	}

    public bool returnb_MovementEnded(){
       // Debug.Log("b_MovementEnded : " + b_MovementEnded);
        return b_MovementEnded;
    }
}
