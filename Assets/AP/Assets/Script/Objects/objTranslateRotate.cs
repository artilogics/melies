// Description : objTranslateRotate : used to translate or rotate object like door or drawer
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objTranslateRotate : MonoBehaviour {
	public 	bool 			SeeInspector = false;
	public 	bool 			seeInfo = false;

    public bool             b_EditrSection4 = false;                 // do not display section 4 in the custom editor (door case)

	//--> General Part
	public GameObject 		objIcon;
	public int 				movementType = 0; 					// 0 = rotation , 1 = Translation
	public int 				movementAxis = 1; 					// 0 = X , 1 = Y; Z = 2

	public List<bool> 	 	constraintsAxisPosition = new List<bool>{true,true,true};
	public List<bool>  		constraintsAxisRotation = new List<bool>{true,false,true};


	//--> Audio Part
	public AudioClip 		a_Open;
	public float 			a_OpenVolume = 1;
	public AudioClip 		a_Close;
	public float 			a_CloseVolume = 1;
    /*public AudioClip        a_Closed;
    public float            a_ClosedVolume = 1;*/
	private AudioSource 	_audio;								// access audiosource


	//--> Rotation Part
	public Transform 		objPivot;
	public float 			startPosition = 0;
	public float 			endPosition = 0;
	public bool 			b_objStateOpen = false; 			// object is open or close

	public int 				_force = 100;						// hinge joint variables
	public int 				_targetVelocity = 150;
	private HingeJoint 		hinge;
	private JointMotor 		motor;
	private JointLimits 	limits;


	//--> Translate part
	public float 			translationSpeed = 10; 				// between 1 and 2

	private float 			toVel = 2.5f;							// use to add force and move the drawer
    private float 			maxVel = 80.0f;						// use to add force and move the drawer
	private float 			maxForce = 130.0f;					// use to add force and move the drawer
	private float 			gain = 70f;						    // use to add force and move the drawer
	public Transform 		targetPos;							// actual position of the object you want to translate
	public Transform 		targetEndPosition;					// target position
	public Transform 		targetStartPosition;				// start position
	private Rigidbody 		rb;									// Translate object rigidbody
	public GameObject		groupFollow;						// Follow the rigidody gameObject
	private int 			_rbMass	= 40;						// Object mass to prevent bug with the player
	//private float 			target = 0;							// know the position to reach

	public bool 			b_FocusMode_Mobile = true;			// know if focus mode activated on Mobile for this object	
	public bool 			b_FocusMode_Desktop = true;			// know if focus mode activated on desktop for this object	

	//--> Lock Part
	//public bool				b_LockState = false;				// know if the object is lock or unlock				

	public List<EditorMethodsList.MethodsList> methodsList 		// Create a list of Custom Methods that could be edit in the Inspector
	= new List<EditorMethodsList.MethodsList>();

	public CallMethods 		callMethods;						// Access script taht allow to call public function in this script.
	public bool 			moreOptionSection1 = false;			// use in custom editor to show custom method section 5
	public AudioClip 		a_Locked;							// Sound played if object is locked
	public float 			a_LockedVolume = 1;					// volume
	public AudioClip 		a_Unlocked;							// Sound played if object is unlocked
	public float 			a_UnlockedVolume = 1;				// volume


    public GameObject       GrpDoor;                            // use have a correct pivot rotation depending the door open inware or not
    public GameObject       GrpFollow;                          // use have a correct pivot rotation depending the door open inware or not
    public float            pivotOffset = .05f;


	[System.Serializable]
	public class idList{
		public int ID = 0;
		public int uniqueID = 0;
	}

	public List<idList> 		inventoryIDList 		= new List<idList> (){new idList()};		// Lock Section : Check if an Object is in the player inventory using ID

	public List<idList> 		diaryIDList 			= new List<idList> (){new idList()};		// Lock Section : Play a voice over using an ID
	public List<idList> 		feedbackIDList 			= new List<idList> (){new idList()};		// Lock Section : Display a text using an ID
	public bool 				b_VoiceOverActivated 	= false;									// Lock Section : if true voice is played
	public bool 				b_feedbackActivated 	= false;									// Lock Section : if true text is displayed
    public AudioClip            feedbackSound;
    public float                feedbackVolume = 1;

	public List<GameObject> 	activatedObjectList = new List<GameObject>();						// Lock Section : Check if all the object in the list are activated in the scene view

	public bool					b_playVoiceOverOnlyOneTime = false;									// Lock Section : True -> Play only one time the voice over
	public int 					howManyTimeVoiceOverWasPlayed = 0;									// Lock Section : Play only one time the voice over

	public List<idList> 		feedbackIDListUnlock 	= new List<idList> (){new idList()};		// unlock Section : Play a voice over using an ID
	public List<idList> 		diaryIDListUnlock 		= new List<idList> (){new idList()};		// unlock Section : Display a text using an ID
	public bool 				b_feedbackActivatedUnlock = false;									// unlock Section : if true text is displayed
	public bool 				b_VoiceOverActivatedUnlocked = false;								// unlock Section : if true voice is played

	public bool 				b_unlocked = false;													// Play only one time the unlock options

	private VoiceOver_Manager 	voiceOverManager;													// reference to the voice over manager
	public infoUI 				info;																// use to display info on screen


	//-> Focus Camera Section
	public focusCamEffect 		camManager;															// access focusCamEffect component
	[SerializeField]
	private GameObject 			testCameraPosition;													// used to find the camera position


    public bool                 allowDeactivateUIButtonWhenObjTranslateOrRotate = true;
    private audioVariousFunctions audioVarious;


    public conditionsToAccessThePuzzle           puzzle;                                                             // used when the door or drawer use focus and a puzzle is inside the door or drawer

    public bool                 b_FirstTimeStartClose = true;                                        // If save doesn't exist choose if the object is open or close

    public bool                 doorOpen = false;
    public bool                 doorPivotLeft = true;
    public bool                 openInward = true;


    public int                  doorAngle = 90;

    public string               currentDoorState = "Close";

    public bool                 b_mo = false;
    public float                _Timer = 0;
    public bool                 b_release = false;

	// Use this for initialization
	void Start () {
        audioVarious = new audioVariousFunctions();

		GameObject tmpObj = GameObject.Find ("UI_Infos");
		if (tmpObj)info = tmpObj.GetComponent<infoUI> ();

		_audio = GetComponent<AudioSource> ();

		if (checkFocusMode())camManager = GetComponent<focusCamEffect> ();

	//-> Rotation case
		if (movementType == 0) {
			hinge = objPivot.GetComponent<HingeJoint> ();					// init hinge joint
            motor = hinge.motor;
            limits = hinge.limits;
             
            doorInitialization();

			rb = objPivot.GetComponent<Rigidbody> ();
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rb.isKinematic = true;
		}

	//-> Translation Case
		if (movementType == 1) {
			rb = targetPos.GetComponent<Rigidbody> ();
			initAxisConstraints ();
			rb.mass = _rbMass;
		}

		tmpObj = GameObject.Find ("VoiceOver_Manager");
		if (tmpObj) voiceOverManager = tmpObj.GetComponent<VoiceOver_Manager> ();
	}


//--> This function is called to rotate or translate an object	
	public void MoveObject(){
        //-> Object is unlocked
        if (callMethods.Call_A_Method_Only_Boolean(methodsList)     // Check if all the custom methods return true
            && checkIfGameobjectsInTheListAreActivated()				// check if needed object are activated in the scene	
            && checkNeededObjectsInTheInventory()                       // check if needed object is in the inventory    
           ||
            ingameGlobalManager.instance._D                         // Debug Mode Activated                
           ) {

           DeactivateObjectInTheInvenetoryViewer();

			StopAllCoroutines ();
			if (checkFocusMode () && camManager != null) {				// Check if Focus Mode is activated
				
				if (!ingameGlobalManager.instance.b_focusModeIsActivated) {
					ingameGlobalManager.instance.FocusIsActivated (gameObject);

					camManager.MoveCameraToFocusPosition (camManager.targetFocusCamera,true);
				} else {
					
					camManager.MoveCameraToDefaultPosition ();
				}
			}

	//-> Play one time Unlock options custom editor section 6
			if (!b_unlocked) {
	//-> Play Voice Over
				if (voiceOverManager && b_VoiceOverActivatedUnlocked && howManyTimeVoiceOverWasPlayed == 0) {
					//Debug.Log ("Play Voice");
					voiceOverManager.setupNewVoice (
						ingameGlobalManager.instance.currentDiary, 
						ingameGlobalManager.instance.currentLanguage,
						diaryIDListUnlock [0].ID,
						ingameGlobalManager.instance.currentDiary.voiceOverDescription (
							ingameGlobalManager.instance.currentLanguage,
							diaryIDListUnlock [0].ID),
						ingameGlobalManager.instance.currentDiary.r_audioPriority (
							ingameGlobalManager.instance.currentLanguage,
							diaryIDListUnlock [0].ID),
						true);
				}

	//-> Display feedback
				if (info && b_feedbackActivatedUnlock) {
					bool b_Exist = false;
					for (var i = 0; i < info.listRefGameObject.Count; i++) {
						if (gameObject == info.listRefGameObject [i])
							b_Exist = true;
					}
					if(!b_Exist)
						info.playAnimInfo(ingameGlobalManager.instance.currentFeedback.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [feedbackIDListUnlock[0].ID].diaryTitle[0],"Feedback",gameObject);
				}
					
	//-> Play unLock sound Fx
				if (_audio && a_Unlocked) {
                    //Debug.Log ("Here");
                    audioVarious.prepareAudio(_audio, a_UnlockedVolume, .2f, a_Unlocked);   // Prepare Audio before playing sound
					_audio.Play ();
				}
				b_unlocked = true;
			}

	//-> Start Rotate object
			if (movementType == 0)
				StartCoroutine (I_Rotate ());
	//-> Start translate
			if (movementType == 1)
				StartCoroutine (I_Translate ());
		}
	//-> Object is locked
		else {
	//-> Display feedback
			if (info && b_feedbackActivated) {
				bool b_Exist = false;
				for (var i = 0; i < info.listRefGameObject.Count; i++) {
					if (gameObject == info.listRefGameObject [i])
						b_Exist = true;
				}
				if(!b_Exist)
					info.playAnimInfo(ingameGlobalManager.instance.currentFeedback.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [feedbackIDList[0].ID].diaryTitle[0],"Feedback",gameObject);
			}
	//-> Play Voice Over 
			if (voiceOverManager && b_VoiceOverActivated && howManyTimeVoiceOverWasPlayed == 0) {
				if (b_playVoiceOverOnlyOneTime)
					howManyTimeVoiceOverWasPlayed++;
				
				voiceOverManager.setupNewVoice (
					ingameGlobalManager.instance.currentDiary, 
					ingameGlobalManager.instance.currentLanguage,
					diaryIDList[0].ID,
					ingameGlobalManager.instance.currentDiary.voiceOverDescription(
						ingameGlobalManager.instance.currentLanguage,
						diaryIDList[0].ID),
					ingameGlobalManager.instance.currentDiary.r_audioPriority(
						ingameGlobalManager.instance.currentLanguage,
						diaryIDList[0].ID),
					true);
			}

	//-> Play Lock sound Fx
			if (_audio && a_Locked) {
                audioVarious.prepareAudio(_audio, a_LockedVolume, .2f, a_Locked);   // Prepare Audio before playing sound
				_audio.Play ();
			}
		}


	}

    private void deactivateInteractiveUIButtons(){
        if ((!b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs               // Focus camera deactivate for desktop
            || !b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs)            // Focus camera deactivate for mobile
            && allowDeactivateUIButtonWhenObjTranslateOrRotate)
        {                               // Interactive UI Button need to be deactivate when the object is moving        

            ingameGlobalManager.instance.canvasPlayerInfos.deactivateInteractiveIcons();
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateIcons(true);
        }
    }

    private void ChangeDirection(int value)
    {
        if (movementType == 0)
        {
            if (doorOpen && doorPivotLeft)
            {                   // If the door is open at the beginning inverse tagetVelocity
                value *= -1;
            }

            motor.force = _force;
            motor.targetVelocity = value;
            motor.freeSpin = false;
            hinge.motor = motor;
            hinge.useMotor = true;
        }
    }

 
    IEnumerator I_Rotate(){
        //-> Object could rotate
       
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    //-> Deactivate Intercative Ui Buttons if needed
    deactivateInteractiveUIButtons();                   

    //-> Play Audio
        if (b_objStateOpen && startPosition >= 0 
            || !b_objStateOpen && startPosition < 0 ) {
            //target = startPosition;
            if (_audio && a_Open && !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_OpenVolume, 1f, a_Open);   // Prepare Audio before playing sound
            }
        }
        else{
            //target = endPosition;
            if (_audio && a_Close&& !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_CloseVolume, 1f, a_Close);   // Prepare Audio before playing sound
            }
            else if (_audio && a_Open && !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_OpenVolume, 1f, a_Open);   // Prepare Audio before playing sound
            }
        }


        if (_audio && !_audio.isPlaying) {
            _audio.Play ();
        }

        // Deactivate puzzle if needed
        if(ingameGlobalManager.instance.GetComponent<focusCamEffect>().movementState == 2){
            if (puzzle && ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Desktop            // Desktop case 
          ||
                puzzle && !ingameGlobalManager.instance.b_DesktopInputs && !b_FocusMode_Mobile)          // Mobile Case
            {
                puzzle.F_PuzzleInsideDoorOrDrawerExit(null);
            }
        }
       
        //--> The door is currently open
        if (currentDoorState == "Open")
        {
            ChangeDirection(_targetVelocity);     
            float currentLimits = hinge.limits.max;

            if (!doorPivotLeft && currentDoorState == "Open")
            {
                ChangeDirection(-_targetVelocity);
            }

            if (doorOpen && currentLimits == 0)
                currentLimits = hinge.limits.min;

            if (!doorOpen && currentLimits != 0)
                currentLimits = hinge.limits.min;
            

            //Debug.Log("currentLimits : " + currentLimits);

            //while (Mathf.Round(objPivot.localEulerAngles.y) != hinge.limits.max) {
            while (Mathf.Abs(Mathf.Round(hinge.angle * 10000) - currentLimits * 10000) > 20 || !ingameGlobalManager.instance.GetComponent<focusCamEffect>().returnb_MovementEnded())
            {
               // Debug.Log("Rotation process : " + hinge.angle +  " : " + hinge.limits.max);
                // Debug.Log("Here 2 : " + Mathf.Round(hinge.angle * 10000) + " : " + hinge.limits.max * 10000);
                if (!ingameGlobalManager.instance.b_Ingame_Pause && rb.isKinematic){
                   
                    rb.isKinematic = false; 
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                }
                else if (ingameGlobalManager.instance.b_Ingame_Pause && !rb.isKinematic){
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.isKinematic = true;
                }
                    

                //if (groupFollow)
                 //   groupFollow.transform.rotation = rb.transform.rotation;

                //Debug.Log("Go To End Position : " + hinge.angle);
                currentDoorState = "Close";
                yield return null;
            }
        }
        //--> The door is currently close
        else if (currentDoorState == "Close")
        {
            if(doorPivotLeft){
                ChangeDirection(-_targetVelocity);  
            }
            else if(!doorPivotLeft && currentDoorState == "Close"){
                ChangeDirection(_targetVelocity);  
            }


            float currentLimits2 = hinge.limits.min;
           

            if(doorOpen && currentLimits2 != 0)
                currentLimits2 = hinge.limits.max;
                
            if(!doorOpen && currentLimits2 == 0)
                currentLimits2 = hinge.limits.max;

           // Debug.Log("currentLimits2 : " + currentLimits2);
            //while (Mathf.Round(objPivot.localEulerAngles.y) != hinge.limits.min){
            while (Mathf.Abs(Mathf.Round(hinge.angle * 10000) - currentLimits2 * 10000) > 20 || !ingameGlobalManager.instance.GetComponent<focusCamEffect>().returnb_MovementEnded())
            {
               // Debug.Log("Rotation process : " + hinge.angle +  " : " + currentLimits2);
                //Debug.Log("Here 3 : " + Mathf.Round(hinge.angle*10000) + " : " + hinge.limits.min* 10000 + " : " + ingameGlobalManager.instance.GetComponent<focusCamEffect>().returnb_MovementEnded());
                if (!ingameGlobalManager.instance.b_Ingame_Pause && rb.isKinematic){
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                }
                   
                else if (ingameGlobalManager.instance.b_Ingame_Pause && !rb.isKinematic){
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.isKinematic = true;
                }
                   

               // if (groupFollow)
               //     groupFollow.transform.rotation = rb.transform.rotation;

                //Debug.Log("Go To S Position : " + hinge.angle);

                currentDoorState = "Open";
                yield return null;
            }
        }

        //-> Activate UI Icons and buttons
        if ((!b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs                   // Focus camera deactivate for desktop
            || !b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs)
            && allowDeactivateUIButtonWhenObjTranslateOrRotate)
        {                                   // Focus camera deactivate for mobile
            ingameGlobalManager.instance.canvasPlayerInfos.MovementEnded_ActivateIcon();
        }

        //-> Init character and Reticule after a focus(Cam go to the init position). 
        if ((b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs                    // Focus camera activated for desktop
            || b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs)                 // Focus camera activated for Mobile  
            && ingameGlobalManager.instance.GetComponent<focusCamEffect>().movementState == 2)
        {    // Only if Focus Cam Go to the init position

            ingameGlobalManager.instance.currentPlayer.GetComponent<CapsuleCollider>().isTrigger = false;

            ingameGlobalManager.instance.currentPlayer.GetComponent<Rigidbody>().isKinematic = false;
            ingameGlobalManager.instance.currentPlayer.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            ingameGlobalManager.instance.b_AllowCharacterMovment = true;

            ingameGlobalManager.instance.b_focusModeIsActivated = false;
            if (ingameGlobalManager.instance.b_DesktopInputs)
                ingameGlobalManager.instance.reticule.SetActive(true);


        }
        else
        {
            // Activate puzzle if needed
            if (puzzle && ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Desktop            // Desktop case 
                ||
                puzzle && !ingameGlobalManager.instance.b_DesktopInputs && !b_FocusMode_Desktop)          // Mobile Case
            {

                //Debug.Log("Here 1");
                puzzle.F_PuzzleInsideDoorOrDrawer(null);
            }
        }


        if(currentDoorState == "Open")
            b_objStateOpen = true;
        else
            b_objStateOpen = false;


        hinge.useMotor = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        //if (groupFollow)
        //    groupFollow.transform.rotation = rb.transform.rotation;
        //-> Rotation is ended
        //Debug.Log("Rotation ended");

        _Timer = 0;
        b_release = true;

        yield return null;
    }

    private void doorInitialization(){
        //hinge = objPivot.GetComponent<HingeJoint>();                    // init hinge joint
        //JointLimits limits = hinge.limits;

        //--> Door is Open when scene Start
        if(doorOpen){
            currentDoorState = "Open";
            //--> If the door is moving inward
            if (openInward)
            {                               
                float refMin = limits.min;

                // Door pivot on the  Left
                if (doorPivotLeft)
                {                              
                    limits.max = endPosition;
                    _targetVelocity = -_targetVelocity;
                }
                // Door pivot on the right
                else{                                           
                    limits.min = -endPosition;
                    limits.max = refMin;
                }
            }
            //--> The door is opened at the beginning 
            else if (!openInward)
            {                           
                float refMin = limits.min;

                // Door pivot on the  Left
                if (doorPivotLeft){                             
                    limits.min = -endPosition;
                    limits.max = refMin;
                }
                // Door pivot on the right
                else{                                           
                    limits.min = refMin;
                    limits.max = endPosition;
                    _targetVelocity = -_targetVelocity;
                }
            }
        }
        //--> Door is closed when scene Start
        else{
            currentDoorState = "Close";
            //--> If the door is moving inward
            if (openInward)
            {                                
                float refMin = limits.min;

                // Door pivot on the  Left
                if (doorPivotLeft)
                {                              
                    limits.min = -endPosition;
                    limits.max = refMin;

                }
                // Door pivot on the right
                else
                {                                          
                    limits.max      = endPosition; 
                }
            }
            //--> If the door is opened at the beginning inverse tagetVelocity
            else if (!openInward)
            {                           
                float refMin = limits.min;

                // Door pivot on the Left
                if (doorPivotLeft)
                {                             
                    limits.max = endPosition;
                    _targetVelocity = -_targetVelocity;                          
                }
                // Door pivot on the right
                else
                {                                          
                    limits.min = -endPosition;
                    limits.max = refMin;
                    _targetVelocity = -_targetVelocity;
                }
            }
        }

        //--> update modification
        hinge.limits = limits;                             

    }

//--> Find rotation Axis
	Quaternion CheckAxisRotation(float target){
		Quaternion quat = Quaternion.identity;
		if (!constraintsAxisRotation[0])
			quat = Quaternion.Euler (target,0,  0);
		if (!constraintsAxisRotation[1])
			quat = Quaternion.Euler (0, target, 0);
		if (!constraintsAxisRotation[1])
			quat = Quaternion.Euler (0, 0,target);

		return quat;
	}
		
//--> Find Translation Axis
	float axisPosition(Vector3 _localPosition){
        
		if (!constraintsAxisPosition[0]) {
			return _localPosition.x;
		} else if (!constraintsAxisPosition[1]) {
			return _localPosition.y;
		} else if (!constraintsAxisPosition[2]) {
			return _localPosition.z;
		} else {
			return 0;
		}

	}



//--> Translate Object
	private IEnumerator I_Translate(){
        //ingameGlobalManager.instance.canvasPlayerInfos.interactiveIconAvailable = false;	
	//-> Play Audio
		if (_audio && a_Open && !_audio.isPlaying) {
            audioVarious.prepareAudio(_audio, a_OpenVolume, 1f, a_Open);   // Prepare Audio before playing sound

		}

		if(targetPos == targetEndPosition){
			targetPos = targetStartPosition;
			if (_audio && a_Open && !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_OpenVolume, 1f, a_Open);   // Prepare Audio before playing sound
			}
		}
		else{
			targetPos = targetEndPosition;
			if (_audio && a_Close && !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_CloseVolume, 1f, a_Close);   // Prepare Audio before playing sound
			}
			else if (_audio && a_Open && !_audio.isPlaying) {
                audioVarious.prepareAudio(_audio, a_OpenVolume, 1f, a_Open);   // Prepare Audio before playing sound
			}
		}

		if (_audio && !_audio.isPlaying) {
			_audio.Play();
		}

		rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    //-> Deactivate Intercative Ui Buttons if needed
        deactivateInteractiveUIButtons();       


		int _sign = 1;
		if (axisPosition (targetStartPosition.localPosition) > axisPosition (targetEndPosition.localPosition))
			_sign = -1;	

		bool b_once = true;
        bool b_once2 = true;
       // focusCamEffect inGameManager_Focus = ingameGlobalManager.instance.GetComponent<focusCamEffect>();

	//-> Open the drawer
		if (targetPos == targetEndPosition) {
            //Debug.Log ("trans");

			b_objStateOpen = true;
			

            while (Mathf.Round (axisPosition(rb.transform.localPosition) * 10000)*_sign < Mathf.Round (axisPosition(targetPos.transform.localPosition) * 10000)*_sign) {
				if (!ingameGlobalManager.instance.b_Ingame_Pause) {
                    //Debug.Log("trans");
                    if(rb.isKinematic){
                        rb.isKinematic = false;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    }
					//calculateTranslationForceToAdd ();
                    b_mo = true;
					//if(groupFollow)
					//	groupFollow.transform.localPosition = rb.transform.localPosition;
					if (!b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs							// Focus camera deactivate for desktop
					    || !b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs) {						// Focus camera deactivate for mobile
						if (b_once && Vector3.Distance (rb.transform.position, targetPos.transform.position) < .05f) {	// Activate Ui Icon on screen
							ingameGlobalManager.instance.canvasPlayerInfos.MovementEnded_ActivateIcon ();

							//Debug.Log ("Open");
							b_once = false;
                           
						}
					}

                   
                   

				}
                else{
                    rb.linearVelocity = Vector3.zero;
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.isKinematic = true;

                }
				yield return null;
			}

            // Activate puzzle if needed
            if (b_once2)
            {
                if (puzzle && ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Desktop            // Desktop case 
                    ||
                    puzzle && !ingameGlobalManager.instance.b_DesktopInputs && !b_FocusMode_Mobile)          // Mobile Case
                {
                    b_once2 = false;
                    //Debug.Log("Here 1");
                    puzzle.F_PuzzleInsideDoorOrDrawer(null);
                }
            }
           // 
		}

	//-> Close the drawer
		if (targetPos == targetStartPosition) {
			b_objStateOpen = false;


            // Deactivate puzzle if needed
            if (puzzle && ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Desktop            // Desktop case 
               ||
                puzzle && !ingameGlobalManager.instance.b_DesktopInputs && !b_FocusMode_Mobile)          // Mobile Case
           {
               puzzle.F_PuzzleInsideDoorOrDrawerExit(null);
           }


			while (Mathf.Round (axisPosition(rb.transform.localPosition) * 10000)*_sign > Mathf.Round (axisPosition(targetPos.transform.localPosition) * 10000)*_sign) {
				if (!ingameGlobalManager.instance.b_Ingame_Pause) {
                    if (rb.isKinematic)
                    {
                        rb.isKinematic = false;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    }
                    b_mo = true;
					//calculateTranslationForceToAdd ();
					//if(groupFollow)
					//	groupFollow.transform.localPosition = rb.transform.localPosition;
					if (!b_FocusMode_Desktop && ingameGlobalManager.instance.b_DesktopInputs							// Focus camera deactivate for desktop
					    || !b_FocusMode_Mobile && !ingameGlobalManager.instance.b_DesktopInputs) {						// Focus camera deactivate for mobile
						if (b_once && Vector3.Distance (rb.transform.position, targetPos.transform.position) < .05f) {	// Activate Ui Icon on screen

                            ingameGlobalManager.instance.canvasPlayerInfos.MovementEnded_ActivateIcon ();
							b_once = false;
					
						}
					}
				}
                else
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.isKinematic = true;

                }
				yield return null;
			}
		}

        if(b_once && 
           ingameGlobalManager.instance.navigationList.Count > 0 && 
           ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count-1] == "Focus"){
            //ingameGlobalManager.instance.canvasPlayerInfos.interactiveIconAvailable = false;
            ingameGlobalManager.instance.canvasPlayerInfos.MovementEnded_ActivateIcon();
        }
        /* else if (b_once){
             ingameGlobalManager.instance.canvasPlayerInfos.interactiveIconAvailable = true;
             ingameGlobalManager.instance.canvasPlayerInfos.MovementEnded_ActivateIcon(); 
         }*/

        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		rb.isKinematic = true;
		rb.transform.localPosition = targetPos.localPosition;
		//if(groupFollow)
		//	groupFollow.transform.localPosition = rb.transform.localPosition;
        b_mo = false;
        _Timer = 0;
        b_release = true;

	//-> Translation ended
		yield return null;
	}






//--> Calculate the force to move the object frome start position to end position
	private void calculateTranslationForceToAdd(){
		Vector3 dist = targetPos.position - rb.transform.position;

		// calculate a target velocity proportional to distance (clamped to maxVel)
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist * translationSpeed, maxVel * translationSpeed);
		// calculate the velocity error
		Vector3 error = tgtVel - rb.linearVelocity;
		// calculate a force proportional to the error (clamped to maxForce)
		Vector3 force = Vector3.ClampMagnitude(gain * translationSpeed * error, maxForce * translationSpeed);

        //Debug.Log("Force : " + force);
        rb.AddForce(force);
	}


    void LateUpdate()
    {
        if (movementType == 0)
        {
            if (groupFollow != null && !rb.isKinematic || b_release)
            {
                groupFollow.transform.rotation = Quaternion.Lerp(groupFollow.transform.rotation, rb.transform.rotation, Time.deltaTime * 10);
            }
        }

        if (movementType == 1){
            // object follow object with translation
            if (groupFollow != null && !rb.isKinematic || b_release)
            {
                groupFollow.transform.position = Vector3.Lerp(groupFollow.transform.position, rb.transform.position, Time.deltaTime * 10);
            }

           
        }

        if (_Timer < .5f && b_release)
        {
            _Timer += Time.deltaTime;
        }
        else if (_Timer >= .5f && b_release)
        {
            b_release = false;
        }




    }

    private void FixedUpdate()
    {
        if(b_mo){           // calculate translation
            calculateTranslationForceToAdd();}
            
    }


    //--> init rotation and position axis constraints
    private void initAxisConstraints (){
		rb.constraints = RigidbodyConstraints.None;

		if(constraintsAxisRotation[0])	// X Axis Rotation
			rb.constraints  = rb.constraints | RigidbodyConstraints.FreezeRotationX;
		if(constraintsAxisRotation[1])	// Y Axis Rotation
			rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationY;
		if(constraintsAxisRotation[2])	// Z Axis Rotation
			rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationZ;
	}

//--> Check if focus mode available
	bool checkFocusMode(){
		bool result = false;

		if (ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Desktop)
			result = true;
		if (!ingameGlobalManager.instance.b_DesktopInputs && b_FocusMode_Mobile)
			result = true;

		return result;
	}

//--> Check if the player has the needed object in his inventory
	bool checkNeededObjectsInTheInventory(){
		bool result = true; 

		if (ingameGlobalManager.instance.currentPlayerInventoryList.Count == 0
			&& inventoryIDList.Count > 0) {
			result = false; 
		}

		for (var i = 0; i < inventoryIDList.Count; i++) {
			for (var j = 0; j < ingameGlobalManager.instance.currentPlayerInventoryList.Count; j++) {
				if (ingameGlobalManager.instance.currentPlayerInventoryList[j] == inventoryIDList [i].ID) {
					result = true;
					break;
				}
				result = false; 
			}
		}

		//Debug.Log (result);
		return result;
	}

    private void DeactivateObjectInTheInvenetoryViewer()
    {
        for (var i = 0; i < inventoryIDList.Count; i++)
        {
            for (var j = 0; j < ingameGlobalManager.instance.currentPlayerInventoryList.Count; j++)
            {
                if (ingameGlobalManager.instance.currentPlayerInventoryList[j] == inventoryIDList[i].ID)
                {
                    ingameGlobalManager.instance.currentPlayerInventoryObjectVisibleList[j] = false;
                }
            }
        }
    }

//--> Check if all the object in activatedObjectList list are activated in scene view
	bool checkIfGameobjectsInTheListAreActivated(){
		bool result = true; 

		for (var i = 0; i < activatedObjectList.Count; i++) {
			if (activatedObjectList [i] != null) {
				if(!activatedObjectList [i].activeSelf)
					result = false; 
			}
		}
		return result;
	}

//--> Use to load object state
	public void saveSystemInitGameObject(string s_ObjectDatas){
		string[] codes  = s_ObjectDatas.Split('_');

       
        //-> Case Rotate object
        if (movementType == 0)
        {
            // Load Parameters 
            if (s_ObjectDatas != "") {                  // Save already exist
                for (var i = 0; i < codes.Length; i++)
                {
                    if (i == 0)
                    {                           // b_objStateOpen : Open or close
                        Debug.Log(gameObject.name + " : " + codes[i]);
                        if (codes[i] == "T")
                        {
                            b_objStateOpen = true;
                            InitPosition(true);
                        }
                        else
                        {
                            b_objStateOpen = false;
                            InitPosition(false);
                        }

                  
                    }
                    if (i == 1)
                    {                           // b_unlocked : Object is unlocked
                        if (codes[i] == "T") b_unlocked = true;
                        else b_unlocked = false;
                    }
                   

                }
            }
      
		}

		//-> Case translate
		if (movementType == 1) {
            // Load Parameters 
            if (s_ObjectDatas != "")
            {                  // Save already exist
                for (var i = 0; i < codes.Length; i++)
                {
                    if (i == 0)
                    {                           // b_objStateOpen : Open or close
                                                //Debug.Log("i = 0 : " + codes [i]);
                        if (codes[i] == "T")
                        {
                            b_objStateOpen = true;
                            InitPosition(true);
                        }
                        else
                        {
                            b_objStateOpen = false;
                            InitPosition(false);
                        }
                    }
                    if (i == 1){                           // b_unlocked : Object is unlocked
                     //Debug.Log("i = 1 : " + codes [i]);
                        if (codes[i] == "T")
                        {
                            b_unlocked = true;
                        }
                        else
                        {
                            b_unlocked = false;
                        }
                    }
                }
            }
            else
            {       // Save doesn't exist
                if (!b_FirstTimeStartClose)
                {
                    b_objStateOpen = true;
                    InitPosition(true);
                }
                else
                {
                    b_objStateOpen = false;
                    InitPosition(false);
                }

          
            }
		}


		for (var i = 0; i < ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem.Count; i++) {
			if (ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem [i] == gameObject) {
				ingameGlobalManager.instance._levelManager.listState [i] = true;
				break;
			}
		}
	}

//--> Use to save Object state
	public string ReturnSaveData () {
		
		string value = "";

		//-> Case Rotate object
		if (movementType == 0) {
			value += r_TrueFalse(b_objStateOpen) + "_";		// b_unlocked
			value += r_TrueFalse(b_unlocked);				// b_objStateOpen
		}

		//-> Case translate
		if (movementType == 1) {
			value += r_TrueFalse(b_objStateOpen) + "_";		// b_unlocked
			value += r_TrueFalse(b_unlocked);				// b_objStateOpen
			//Debug.Log(value);
		}

		return value;
	}



//--> init object position 
	private void InitPosition(bool b_State){
		if (b_State) {							// Open position
            //-> rotation
			if (movementType == 0) {
                float tmpEndPosition = 0;

                if (openInward && doorPivotLeft)
                    tmpEndPosition = -endPosition;
                if(!openInward && doorPivotLeft)
                    tmpEndPosition = endPosition;

                if (openInward && !doorPivotLeft)
                    tmpEndPosition = endPosition;
                if (!openInward && !doorPivotLeft)
                    tmpEndPosition = -endPosition;

				if (movementAxis == 0)
                    objPivot.localEulerAngles = new Vector3 (tmpEndPosition, objPivot.localEulerAngles.y, objPivot.localEulerAngles.z);
				if (movementAxis == 1)
                    objPivot.localEulerAngles = new Vector3 (objPivot.localEulerAngles.x, tmpEndPosition, objPivot.localEulerAngles.z);
				if (movementAxis == 2)
                    objPivot.localEulerAngles = new Vector3 (objPivot.localEulerAngles.x, objPivot.localEulerAngles.y, tmpEndPosition);

				if (groupFollow) {
					groupFollow.transform.localEulerAngles = objPivot.localEulerAngles;
				}

                currentDoorState = "Open";
			}
            //-> translation
			if (movementType == 1) {
				targetPos.localPosition = targetEndPosition.localPosition;
				targetPos = targetEndPosition;
				if (groupFollow) 
					groupFollow.transform.localPosition = targetPos.localPosition;
			}
		} else if (!b_State) {					// Close position
            //-> rotation
			if (movementType == 0) {

				if (movementAxis == 0) {
					objPivot.localEulerAngles = new Vector3 (startPosition, objPivot.localEulerAngles.y, objPivot.localEulerAngles.z);
				}
				if (movementAxis == 1) {
					objPivot.localEulerAngles = new Vector3 (objPivot.localEulerAngles.x, startPosition, objPivot.localEulerAngles.z);
				}
				if (movementAxis == 2) {
					objPivot.localEulerAngles = new Vector3 (objPivot.localEulerAngles.x, objPivot.localEulerAngles.y, startPosition);
				}

				if (groupFollow) {
					groupFollow.transform.localEulerAngles = objPivot.localEulerAngles;
				}

                currentDoorState = "Close";
			}
            //-> translation
			if (movementType == 1) {
               
				targetPos.localPosition = targetStartPosition.localPosition;
				targetPos = targetStartPosition;
				if (groupFollow) 
					groupFollow.transform.localPosition = targetPos.localPosition;
			}

		}

	}

//--> Convert bool to T or F string
	private string r_TrueFalse(bool s_Ref){
		if (s_Ref)return "T";
		else return "F";
	}
}

