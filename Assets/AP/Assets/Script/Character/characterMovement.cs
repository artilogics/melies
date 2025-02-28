// Description : MouseController : use to controller where the character is look to.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class characterMovement : MonoBehaviour {
	public 	bool 				SeeInspector = true;

	public Rigidbody			rbBodyCharacter;			// Reference to the character body
	public Transform 			tangentStartPosition;
	public Transform			objCamera;					// Reference to the camera
	public GameObject 			addForceObj;				// Position where forces is add to the character
    public Transform            refHead;                    // use for focus camera in inGameGloabalManager in the Hierarchy

	private string 				s_mouseAxisX 		= "Mouse X";				// Default Mouse Inputs
	private string 				s_mouseAxisY 		= "Mouse Y";
	public int 					forwardKeyboard 	= 0;
	public int 					backwardKeyboard 	= 1;
	public int 					leftKeyboard 		= 2;
	public int 					rightKeyboard 		= 3;

	public int 					VerticalAxisBody 	= 0;
	public int 					HorizontalAxisBody 	= 1;

	public int 					JoystickVerticalAxisCam 	= 2;
	public int 					JoystickHorizontalAxisCam 	= 3;

	public int 					mouseInvertYAxisCam 	= 0;
	public int 					joystickInvertYAxisCam 	= 0;

	public int 					mouseInGameSensibilty 	= 0;			// Sensibility chosen by the player in Game
	public int 					gamepadLookInGameSensibilty 	= 0;	// Sensibility chosen by the player in Game


	public float 				currentDesktop_X_Axis = 0;
	public float 				currentDesktop_Y_Axis = 0;
	public float				speedKeybordMovement = 2;


    //Crouch
    public int                  JoystickCrouch = 2;
    public int                  KeyboardCrouch = 2;

	public float 				minimum 	= -60f;			// Limit camera Y movement
	public float 				maximum		= 60f;

	public float 				characterSpeed = 2;			// Character speed when moving left right or forward backward


	public float 				sensibilityMouse 	= 2;	// Mouse sensibility
	public AnimationCurve		animationCurveMouse;
	public float 				sensibilityJoystick = 2;	// Joystick sensibility
	public AnimationCurve		animationCurveJoystick;

	private float				minimumAxisMovement = .2f;

	public float 				mouseY 		= 0;			// current X camera Rotation

	private float 				tmpXAxis 	= 0;			// temporary values
	private float 				tmpYAxis 	= 0;	

	public VirtualController	mobileToystickController;
	public float 				sensibilityMobile = 2;
	public AnimationCurve		animationCurveMobile;

	private bool 				b_MoveForward = false;		// Use on Mobile
	private bool 				b_MoveBackward = false;
	private bool 				b_MoveLeft = false;		// Use on Mobile
	private bool 				b_MoveRight = false;

    // Mobile Move Camera Second system
    public bool                 b_MobileCamRotation_Stick = true;
    public float                mobileSpeedRotation = 1f;
    private Vector2[]           arrStartPos = new Vector2[10]; 
    // Mobile Move Player Second system
    public bool                 b_MobileMovement_Stick = true;
    public VirtualController    mobileLeftJoystickToMove;
    public float                LeftStickSensibility = 5;

	private float				smoothStart = 0;
	public AnimationCurve		animationCurveMobileSmoothMove;

	private float 				mouseYLastValue = 0;		// Use to prevent bug on mac with the cursor lock state
	private bool 				b_Once = true;

	public LayerMask 			myLayerMask;


    private float               joyHorizontal = 0;
    float                       joyVertical = 0;

    private float               axisHorizontal = 0;
    private float               AxisVertical = 0;

    private float               XAxis = 0;
    private float               YAxis = 0;

    private float               mouseVertical = 0;

    float                       mouseInputX = 0;
   //float mouseInputY = 0;

    //float xRot;
    float                       yRot;
    float                       mouseHorizontal = 0;


    public float                BrakeForce = 35f;
    public float                Coeff = .15f;
    public float                MaxSpeed = 1f;

    public scPreventClimbing    preventClimbing;

    // Crouch
    public bool                 allowCrouch = false;
    public bool                 b_Crouch = false;
    public float                targetScaleCrouch = .5f;
    private float               refScaleCrouch = 1f;
    public float                crouchSpeed = 3f;
    public float                heightCheck = 2.05f;
    public LayerMask            layerCheckCrouch;

    // Run
    public int                  JoystickRun = 7;
    public int                  KeyboardRun = 10;
    public bool                 bMobileRun = false;
    public float                speedMultiplier = 3;
    private float               currentSpeedMultiplier = 1;
    public bool                 b_AllowRun = false;
    public bool                 isRunning = false;

    // Jump
    public bool                 b_AllowJump = true;
    public int                  JoystickJump = 8;
    public int                  KeyboardJump = 11;
    public float                jumpForce = 3;
    public float                jumpSpeed = 10;
    public bool                 b_IsJumping = false;
    public float                fallCurve;
    public AnimationCurve       animFallCurve;
    public float                GravityFallSpeed = 30;
    public float                heightRoof = .45f;
    public float                minimumJump = .6f;

    public float                gravityScale = 1.0f;
    private static float        globalGravity = -9.81f;
    public float                MaxAngle = 70;
    private float               currentAngle = 0;
    private Vector3             circlePos = Vector3.zero;
    public bool                 moreInfoMaxAngle = true;

    //-> Variables use to check if the character is touching the floor
    public bool                 isOnFloor = true;
    private float               hitDistance = .35f;
    public float                hitDistanceMin = .45f;
    public float                hitDistanceMax = .75f;
    public LayerMask            myLayer;
    public Vector3              rayPosition = Vector3.zero;

    public PhysicsMaterial       pMove;
    public PhysicsMaterial       pStop;
    public PhysicsMaterial       pIce;
    private CapsuleCollider     charCol;

    // Use to know if the player touching something. Use to if the character is grounded
    public LayerMask            myLayer02;
    public float                overlapSize = .5f;
    public float                overlapPos = .334f;
    public bool                 b_Overlap = false;

    //Layers 12 and 17. Use to know if the character is touching a door or a drawer
    public bool b_TouchLayer12_17 = false;

    private void Start()
    {
        refScaleCrouch = gameObject.transform.localScale.y;     // Save the character standing size
        charCol = GetComponent<CapsuleCollider>();
    }


    public void charaGeneralMovementController () {
		// Left right forward backward

		if (ingameGlobalManager.instance.b_bodyMovement) {
            if (!b_MobileMovement_Stick || ingameGlobalManager.instance.b_DesktopInputs)
                bodyMovement();
            else{           // Mobile system 2: Left joystick
               
                AP_Mobile_bodyMovement_LeftStick();
            }   
               
		}
	}

    Vector3 joyInput = Vector3.zero;


    void Update()
    {
        joyVertical = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[JoystickVerticalAxisCam]);
        joyHorizontal = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[JoystickHorizontalAxisCam]);


        joyInput = new Vector3(joyVertical * returnInvertJoystickAxis(), joyHorizontal,0);

        if (joyInput.sqrMagnitude > 1.0f)
            joyInput = joyInput.normalized;


        mouseHorizontal = Input.GetAxis(s_mouseAxisX);
        mouseVertical = Input.GetAxis(s_mouseAxisY);

        axisHorizontal = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[HorizontalAxisBody]);
        AxisVertical = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[VerticalAxisBody]);

        XAxis = returnDesktopXAxis();
        YAxis = returnDesktopYAxis();

        mouseInputX = Input.GetAxis("Mouse X");

        if (b_MoveRight || b_MoveLeft || b_MoveBackward || b_MoveForward)
        {
            smoothStart = Mathf.MoveTowards(smoothStart, 1, Time.deltaTime * 2);
        }

       
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (ingameGlobalManager.instance.saveAndLoadManager.b_IngameDataHasBeenLoaded
                && ingameGlobalManager.instance.b_AllowCharacterMovment
                && !ingameGlobalManager.instance.b_Ingame_Pause)
            {

                //-> Desktop Case
                if (ingameGlobalManager.instance.b_DesktopInputs)
                {
                    bodyRotation();
                    cameraRotation();
                }
                //-> Mobile case
                else
                {
                    bodyRotationMobile();
                    CamRotationMobile();
                }

                //-> Jump
                if (b_AllowJump)
                {
                    Debug.DrawRay(refHead.transform.position + Vector3.up * .1f, Vector3.up * heightRoof, Color.blue);
                    if ((Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        ingameGlobalManager.instance.b_Joystick

                    ||

                        Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        !ingameGlobalManager.instance.b_Joystick)

                        && !b_IsJumping && isOnFloor)
                    {
                        StartCoroutine(Jump());
                    }
                }

                //-> Crouch
                if (allowCrouch)
                {
                    if (Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickCrouch]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        ingameGlobalManager.instance.b_Joystick

                    ||

                        Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardCrouch]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        !ingameGlobalManager.instance.b_Joystick)
                    {
                        
                        if (b_Crouch && AP_CheckIfPlayerCanStopCrouching() || !b_Crouch){
                            b_Crouch = !b_Crouch;
                            //Debug.Log("Crouch");
                        }
                           
                    }
                }

                #region Run
                //-> Run
                 if(b_AllowRun ){
                    //Joystick
                    if ((Input.GetKey(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickRun]) &&
                       ingameGlobalManager.instance.b_DesktopInputs &&
                       ingameGlobalManager.instance.b_Joystick

                   ||
                   // Keyboard
                       Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardRun]) &&
                       ingameGlobalManager.instance.b_DesktopInputs &&
                       !ingameGlobalManager.instance.b_Joystick

                       ||
                       // Mobile
                       bMobileRun &&
                       !ingameGlobalManager.instance.b_DesktopInputs)

                        && !b_Crouch)
                    {
                         isRunning = true;
                         currentSpeedMultiplier = speedMultiplier;
                     }
                     else{
                         isRunning = false;
                         currentSpeedMultiplier = 1;
                     }
                 }
                #endregion
            }
        }
    }





    private void FixedUpdate()
    {
        AP_OverlapSphere();
        Ap_isOnFloor();
        AP_ApplyGravity();

        // Crouch: Check if the character scale need to be updated
        if(allowCrouch){
            if (b_Crouch && gameObject.transform.localScale.y != targetScaleCrouch)
            {
                gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale,
                                                                      new Vector3(gameObject.transform.localScale.x, targetScaleCrouch, gameObject.transform.localScale.z),
                                                                      Time.deltaTime * crouchSpeed);
            }
            else if (!b_Crouch && gameObject.transform.localScale.y != refScaleCrouch)
            {
                gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale,
                                                                      new Vector3(gameObject.transform.localScale.x, refScaleCrouch, gameObject.transform.localScale.z),
                                                                      Time.deltaTime * crouchSpeed);
            }
        }
    }


    //--> Desktop Case : Body rotation
    private void bodyRotation()
    {
        #region
        if (ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove)
        {

            if (!ingameGlobalManager.instance.b_Joystick && mouseHorizontal != 0)
            {
                tmpXAxis = mouseInputX * 1.1f;
                tmpXAxis *= sensibilityMouse * ingameGlobalManager.instance.inputListOfFloatKeyboardButton[mouseInGameSensibilty];
            }
            else
            {
                tmpXAxis = 0;
            }

            objCamera.transform.Rotate(0, tmpXAxis, 0);

        }
        else if (ingameGlobalManager.instance.b_Joystick)
        {     // Caculate body and camera X rotation at the same time

            objCamera.localEulerAngles += joyInput * animationCurveJoystick.Evaluate(joyInput.magnitude) * ingameGlobalManager.instance.inputListOfFloatGamepadButton[gamepadLookInGameSensibilty] * 100f * Time.deltaTime;

            objCamera.localEulerAngles = new Vector3(ClampAngle(objCamera.localEulerAngles.x, minimum, maximum),
                                                     objCamera.localEulerAngles.y,
                                                     0);
        }
        #endregion
    }

    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }




    //--> Desktop case : camera rotation X Axis
    private void cameraRotation()
    {
        #region
        if (ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove)
        {
            // Mouse Case
            if (!ingameGlobalManager.instance.b_Joystick && mouseVertical != 0)
            {
                tmpYAxis = mouseVertical;

                tmpYAxis = Mathf.Clamp(tmpYAxis, -3f, 3f);
                tmpYAxis *= 1.5f;

                mouseY -= tmpYAxis * sensibilityMouse * returnInvertMouseAxis() * ingameGlobalManager.instance.inputListOfFloatKeyboardButton[mouseInGameSensibilty];
            }

            mouseY = Mathf.Clamp(mouseY, minimum, maximum);

            objCamera.localEulerAngles = new Vector3(
                mouseY,
                objCamera.localEulerAngles.y,
                0);

        }
        // Joystick Case
        else if (ingameGlobalManager.instance.b_Joystick)
        {
            // camera X rotation is calculated in section bodyRotation() for gamepad inputs

        }
        //-> Prevent Mac bug with the cursor lockstate
        else if (!ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove && mouseYLastValue != Input.GetAxis(s_mouseAxisY) && b_Once)
        {
            StartCoroutine(waitToInitMouseMovement());
        }

        mouseYLastValue = Input.GetAxis(s_mouseAxisY);
        #endregion
    }

//--> Prevent Mac bug with the cursor lockstate
	private IEnumerator waitToInitMouseMovement(){
		b_Once = false;
		//Debug.Log ("waitToInitMouseMovement");
		yield return new WaitForEndOfFrame ();
		ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove = true;
		b_Once = true;
	}





    // --> Desktop case : Move the character left right forward backward
    void bodyMovement(){
       // Debug.Log ("Vertical : " + AxisVertical);

		addForceObj.transform.localEulerAngles 
		= new Vector3 (addForceObj.transform.localEulerAngles.x,
			objCamera.transform.localEulerAngles.y,
			addForceObj.transform.localEulerAngles.z);

        Vector3 Direction = new Vector3 (0,0, 0);

		if (ingameGlobalManager.instance.b_Joystick) {
			// --> Left and Right Movement	Joystick
			if (axisHorizontal > minimumAxisMovement) {
				Direction += FindTangentX () * axisHorizontal;
			} else if (axisHorizontal < -minimumAxisMovement) {
				Direction -= FindTangentX () * -axisHorizontal;
			}
		} 
		// --> Left and Right Movement Keyboard
        else if (ingameGlobalManager.instance.b_DesktopInputs){
			Direction += FindTangentX () * XAxis;
		}

		if (ingameGlobalManager.instance.b_Joystick) {
            // --> Forward backward movement Joystick

            if (b_TouchLayer12_17 && !isOnFloor)
            {

            }
            else
            {
                if (AxisVertical > minimumAxisMovement)
                {
                    Direction += FindTangentZ() * -AxisVertical;
                }
                else if (AxisVertical < -minimumAxisMovement)
                {
                    Direction -= FindTangentZ() * AxisVertical;
                }

            }


        }
		// --> Forward backward movement Keyboard
        /*else if(ingameGlobalManager.instance.b_DesktopInputs){

    			Direction += FindTangentZ () * YAxis;
		}*/
        else if (ingameGlobalManager.instance.b_DesktopInputs)
        {
            if (b_TouchLayer12_17 && !isOnFloor)
            {

            }
            else
                Direction += FindTangentZ() * YAxis;
        }


        if (b_MoveRight) {
			Direction += FindTangentX () * animationCurveMobileSmoothMove.Evaluate(smoothStart);
		} else if (b_MoveLeft) {
			Direction -= FindTangentX () * animationCurveMobileSmoothMove.Evaluate(smoothStart);
		}


    	if (b_MoveForward) {
    	   Direction += FindTangentZ () * animationCurveMobileSmoothMove.Evaluate(smoothStart);
        } else if (b_MoveBackward) {
    		Direction -= FindTangentZ () * animationCurveMobileSmoothMove.Evaluate(smoothStart);
   		}
 

        if (preventClimbing.b_preventClimbing){
            Direction.y = 0;
        }

        //Debug.Log("smoothStart: " + smoothStart);
          
        if (isOnFloor)
        {
            if(currentAngle >= 180 - MaxAngle )
                rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);            // move the character

            Vector3 opposite = rbBodyCharacter.transform.InverseTransformDirection(-rbBodyCharacter.linearVelocity);                          // Opposite force to stop the character
           
            rbBodyCharacter.AddRelativeForce(opposite * BrakeForce * Coeff, ForceMode.Force);  
        }
        else{
                rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);            // move the character 
        }


            
        if (rbBodyCharacter.linearVelocity.magnitude > MaxSpeed)
            rbBodyCharacter.linearVelocity = rbBodyCharacter.linearVelocity.normalized * MaxSpeed;
        
	}
	 


	private float returnDesktopXAxis(){
		float result = currentDesktop_X_Axis;
		bool b_PressKey = false;
		if (Input.GetKey (ingameGlobalManager.instance.inputListOfStringKeyboardButton[leftKeyboard])) {
			if (result > 0)
				result = 0;
			result = Mathf.MoveTowards (result, -1, Time.deltaTime * speedKeybordMovement);
			b_PressKey = true;
		}
		if (Input.GetKey (ingameGlobalManager.instance.inputListOfStringKeyboardButton[rightKeyboard])) {
			if (result < 0)
				result = 0;
			result = Mathf.MoveTowards (result, 1, Time.deltaTime * speedKeybordMovement);
			b_PressKey = true;
		}

		if (!b_PressKey) {
			result = Mathf.MoveTowards (result, 0, Time.deltaTime * speedKeybordMovement * 2);
		}

		currentDesktop_X_Axis = result;
		return result;
	}

	private float returnDesktopYAxis(){
		float result = currentDesktop_Y_Axis;
		bool b_PressKey = false;
		if (Input.GetKey (ingameGlobalManager.instance.inputListOfStringKeyboardButton[backwardKeyboard])) {
			if (result > 0)
				result = 0;
			result = Mathf.MoveTowards (result, -1, Time.deltaTime * speedKeybordMovement);
			b_PressKey = true;
		}
			if (Input.GetKey (ingameGlobalManager.instance.inputListOfStringKeyboardButton[forwardKeyboard])) {
			if (result < 0)
				result = 0;
			result = Mathf.MoveTowards (result, 1, Time.deltaTime * speedKeybordMovement);
			b_PressKey = true;
		}

		if (!b_PressKey) {
			result = Mathf.MoveTowards (result, 0, Time.deltaTime * speedKeybordMovement * 2);
		}

		currentDesktop_Y_Axis = result;
		return result;
	}




// --> Next Sections are used for mobile virtual buttons
	public void CamRotationMobile(){
        if(b_MobileCamRotation_Stick){      // using right stick to move the player
            float virtualJoyVertical = mobileToystickController.inputVector.z;

            mouseY -= animationCurveMobile.Evaluate(Mathf.Abs(virtualJoyVertical)) * virtualJoyVertical * sensibilityMobile;
            mouseY = Mathf.Clamp(mouseY, minimum, maximum);

            // Rotate Camera
            objCamera.localEulerAngles = new Vector3(
                mouseY,
                objCamera.localEulerAngles.y,
                objCamera.localEulerAngles.z);
  
        }
        else{ // moving using all screen
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(i).deltaPosition;

                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    arrStartPos[i] = Input.GetTouch(i).position;
                }


                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    float swipe = (new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, 0) - new Vector3(arrStartPos[i].x, arrStartPos[i].y, 0)).magnitude;

                    Vector2 newPos = Input.GetTouch(i).position;

                    if (swipe > 50 && !ingameGlobalManager.instance.mobileInputFinger.checkIfFingerTouchAUIButton(newPos))
                    {
                        // Rotate camera Horizontaly
                        objCamera.transform.Rotate(Vector3.up * touchDeltaPosition.x * mobileSpeedRotation * Time.deltaTime, Space.World);

                        mouseY -= touchDeltaPosition.y * mobileSpeedRotation * Time.deltaTime;
                        mouseY = Mathf.Clamp(mouseY, minimum, maximum);

                        // Rotate Camera Verticaly
                        objCamera.localEulerAngles = new Vector3(
                            mouseY,
                            objCamera.localEulerAngles.y,
                            objCamera.localEulerAngles.z);
                    }
                }
            }
        }
	}


	private void bodyRotationMobile(){
		float virtualJoyVertical = mobileToystickController.inputVector.x;

		tmpXAxis = animationCurveMobile.Evaluate( Mathf.Abs( virtualJoyVertical)) * virtualJoyVertical;
		tmpXAxis *= sensibilityMobile;

		// Rotate the character using his rigidbody
		Quaternion deltaRotation = Quaternion.Euler(new Vector3(0,tmpXAxis,0));
		rbBodyCharacter.MoveRotation(rbBodyCharacter.rotation * deltaRotation );
	}


	// --> First System Move the camera using the vitual joystick
	public void pointerDrag(){
		Vector2 pos;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
			mobileToystickController.backgroundImage.rectTransform,
			mobileToystickController.eventData.position,
			mobileToystickController.eventData.pressEventCamera,
			out pos)){

			pos.x = pos.x / mobileToystickController.backgroundImage.rectTransform.sizeDelta.x;
			pos.y = pos.y / mobileToystickController.backgroundImage.rectTransform.sizeDelta.y;

			mobileToystickController.inputVector = new Vector3 (
				pos.x * 2,
				0,
				pos.y * 2);

			if (mobileToystickController.inputVector.magnitude > 1){
				mobileToystickController.inputVector = mobileToystickController.inputVector.normalized;
			}


			// Move joystick Image
			mobileToystickController.virtualCenter.rectTransform.anchoredPosition = 
				new Vector2(mobileToystickController.inputVector.x * (mobileToystickController.backgroundImage.rectTransform.sizeDelta.x/3),
					mobileToystickController.inputVector.z * (mobileToystickController.backgroundImage.rectTransform.sizeDelta.y/3));
		}
	}

	public void pointerUp(){
		if (mobileToystickController.inputVector != Vector3.zero){
			mobileToystickController.virtualCenter.rectTransform.anchoredPosition = Vector2.zero;
		}
		mobileToystickController.inputVector = Vector3.zero;
	}


    // --> Second System: Move the player using the vitual Left joystick
    public void pointerDrag_MoveWithLeftStick()
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mobileLeftJoystickToMove.backgroundImage.rectTransform,
            mobileLeftJoystickToMove.eventData.position,
            mobileLeftJoystickToMove.eventData.pressEventCamera,
            out pos))
        {

            pos.x = pos.x / mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.x;
            pos.y = pos.y / mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.y;

            mobileLeftJoystickToMove.inputVector = new Vector3(
                pos.x * 2,
                0,
                pos.y * 2);

            if (mobileLeftJoystickToMove.inputVector.magnitude > 1)
            {
                mobileLeftJoystickToMove.inputVector = mobileLeftJoystickToMove.inputVector.normalized;
            }

            // Move joystick Image
            mobileLeftJoystickToMove.virtualCenter.rectTransform.anchoredPosition =
                                        new Vector2(mobileLeftJoystickToMove.inputVector.x * (mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.x / 3),
                                                    mobileLeftJoystickToMove.inputVector.z * (mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }

    public void pointerUp_MoveWithLeftStick()
    {
      //  Debug.Log("Here");
        if (mobileLeftJoystickToMove.inputVector != Vector3.zero)
        {
            mobileLeftJoystickToMove.virtualCenter.rectTransform.anchoredPosition = Vector2.zero;
        }
        mobileLeftJoystickToMove.inputVector = Vector3.zero;
    }

    // --> Desktop case : Move the character left right forward backward
    void AP_Mobile_bodyMovement_LeftStick(/*Vector2 mobileLeftJoystickToMove*/)
    {
        if(mobileLeftJoystickToMove){

            //Debug.Log("here: " + mobileLeftJoystickToMove.inputVector.z);
            if (mobileLeftJoystickToMove.inputVector.magnitude > 1)
            {
                mobileLeftJoystickToMove.inputVector = mobileLeftJoystickToMove.inputVector.normalized;
            }


            addForceObj.transform.localEulerAngles
            = new Vector3(addForceObj.transform.localEulerAngles.x,
                objCamera.transform.localEulerAngles.y,
                addForceObj.transform.localEulerAngles.z);

            Vector3 Direction = new Vector3(0, 0, 0);


            // --> Left and Right Movement  Joystick
            if (mobileLeftJoystickToMove.inputVector.x > minimumAxisMovement)
            {
                Direction += FindTangentX() * mobileLeftJoystickToMove.inputVector.x;
            }
            else if (mobileLeftJoystickToMove.inputVector.x < -minimumAxisMovement)
            {
                Direction -= FindTangentX() * -mobileLeftJoystickToMove.inputVector.x;
            }



            // --> Forward backward movement Joystick
            if (mobileLeftJoystickToMove.inputVector.z > minimumAxisMovement)
            {
                Direction += FindTangentZ() * mobileLeftJoystickToMove.inputVector.z;
            }
            else if (mobileLeftJoystickToMove.inputVector.z < -minimumAxisMovement)
            {
                Direction -= FindTangentZ() * -mobileLeftJoystickToMove.inputVector.z;
            }

            if (b_MoveForward)
            {
                Direction += FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            }
            else if (b_MoveBackward)
            {
                Direction -= FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            }

            if (preventClimbing.b_preventClimbing)
            {
                Direction.y = 0;
            }

            //Debug.Log("Here 2" +  Direction);

            rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);            // move the character

            Vector3 opposite = rbBodyCharacter.transform.InverseTransformDirection(-rbBodyCharacter.linearVelocity);                          // Opposite force to stop the character
            rbBodyCharacter.AddRelativeForce(opposite * BrakeForce * Coeff, ForceMode.Force);


            if (rbBodyCharacter.linearVelocity.magnitude > MaxSpeed)
                rbBodyCharacter.linearVelocity = rbBodyCharacter.linearVelocity.normalized * MaxSpeed;


        }

    }



	// Use for Mobile : Player move forward
	public void MoveForward(){
		b_MoveForward 	= true;
		b_MoveBackward 	= false;
		b_MoveLeft		= false;
		b_MoveRight 	= false;
	}
	// Use for Mobile : Player move backward
	public void MoveBackward(){
		b_MoveBackward 	= true;
		b_MoveForward 	= false;
		b_MoveLeft		= false;
		b_MoveRight 	= false;
	}
	// Use for Mobile : Player Stop moving when button is released
	public void StopMoving(){
		b_MoveBackward 	= false;
		b_MoveForward 	= false;
		b_MoveLeft		= false;
		b_MoveRight 	= false;
		smoothStart = 0;
	}

	// Use for Mobile : Player move Left
	public void MoveLeft(){
		b_MoveBackward 	= false;
		b_MoveForward 	= false;
		b_MoveLeft		= true;
		b_MoveRight 	= false;
	}
	// Use for Mobile : Player move Right
	public void MoveRight(){
		b_MoveBackward 	= false;
		b_MoveForward 	= false;
		b_MoveLeft		= false;
		b_MoveRight 	= true;
	}

    // Use for Mobile : Player is crouching
    public void AP_Crouch()
    {
        if (allowCrouch)
            b_Crouch = !b_Crouch;
    }
		


	Vector3 FindTangentZ(){
		Vector3 newVector = Vector3.zero;
		Vector3 tangente = Vector3.zero;
		RaycastHit hit2;
		if (Physics.Raycast (tangentStartPosition.position, -Vector3.up, out hit2, 10, myLayerMask)) {						
			hit2.normal.Normalize ();

			//Debug.DrawRay(hit2.point, hit2.normal , Color.white);
			tangente = Vector3.Cross( hit2.normal, -addForceObj.transform.right );

			if( tangente.magnitude == 0 )
			{tangente = Vector3.Cross( hit2.normal, Vector3.up );}
			Debug.DrawRay(hit2.point, tangente , Color.yellow);
		}
		//Debug.Log (tangente);
		return tangente;
	}

	Vector3 FindTangentX(){
		Vector3 newVector = Vector3.zero;
		Vector3 tangente = Vector3.zero;
		RaycastHit hit2;
		if (Physics.Raycast (tangentStartPosition.position, -Vector3.up, out hit2, 10, myLayerMask)) {						
			hit2.normal.Normalize ();

			Vector3 myDirection = Vector3.Cross(addForceObj.transform.right, hit2.normal);

			Debug.DrawRay(hit2.point, hit2.normal , Color.white);
			tangente = Vector3.Cross( hit2.normal, myDirection );
		
			if( tangente.magnitude == 0 )
				tangente = Vector3.Cross( hit2.normal, Vector3.up );

			Debug.DrawRay(hit2.point, tangente , Color.red);
		}
		//Debug.Log (tangente);
		return tangente;
	}


	int returnInvertJoystickAxis(){
		int result = 1;
		if(ingameGlobalManager.instance.inputListOfBoolGamepadButton[joystickInvertYAxisCam] == true)
			result = -1;
		return result;
	}

	int returnInvertMouseAxis(){
		int result = 1;
		if(ingameGlobalManager.instance.inputListOfBoolKeyboardButton[mouseInvertYAxisCam] == true)
			result = -1;
		return result;
	}

    // Stop moving the character is pause is activated
    public void charaStopMoving(){
        if(!ingameGlobalManager.instance.b_AllowCharacterMovment && rbBodyCharacter.linearVelocity != Vector3.zero){
            rbBodyCharacter.linearVelocity = Vector3.zero;
        }
    }

    #region Apply Gravity
    private void AP_ApplyGravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * .1f, -Vector3.up, out hit, 100.0f))
        {
            if (isOnFloor)
            {
                currentAngle = Vector3.SignedAngle(hit.normal, -Vector3.up, Vector3.up);
                gravityScale = 1 - (180 - currentAngle) / 80;
                circlePos = hit.point;
            }
            else
            {
                //gravityScale = 1;
            }
        }


        if (b_TouchLayer12_17 && isOnFloor)  // Character is touching door or drawer
        {
            charCol.material = pIce;
            gravityScale = 0;

            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (currentAngle < 180 - MaxAngle || !isOnFloor)
        {
             charCol.material = pIce;
            fallCurve = Mathf.MoveTowards(fallCurve, 1, Time.deltaTime);
            gravityScale = Mathf.MoveTowards(gravityScale, 30, animFallCurve.Evaluate(fallCurve) * GravityFallSpeed * Time.deltaTime);

            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (YAxis == 0 && XAxis == 0 && ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick ||                                                                    // Keyboard 
                 AxisVertical == 0 && axisHorizontal == 0 && ingameGlobalManager.instance.b_Joystick ||     // Joystick
                 mobileLeftJoystickToMove &&  mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x == 0 && !ingameGlobalManager.instance.b_DesktopInputs && b_MobileMovement_Stick|| // Mobile Left Stick
                 !b_MoveForward && !b_MoveBackward  && !b_MoveLeft && !b_MoveRight && !ingameGlobalManager.instance.b_DesktopInputs && !b_MobileMovement_Stick                               // Mobile Left Buttons
                )
        {
            charCol.material = pStop;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }
        else if (YAxis == 0 && XAxis != 0 && ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick ||                                                                    // Keyboard  // Character move only Horizontaly
                 AxisVertical == 0 && axisHorizontal != 0 && ingameGlobalManager.instance.b_Joystick ||     //Joystick
                 mobileLeftJoystickToMove && mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x != 0 && !ingameGlobalManager.instance.b_DesktopInputs && b_MobileMovement_Stick|| // Mobile Left Stick     
                 !b_MoveForward && !b_MoveBackward && (b_MoveLeft || b_MoveRight) && !ingameGlobalManager.instance.b_DesktopInputs && !b_MobileMovement_Stick                               // Mobile Left Buttons
                )
        {
            charCol.material = pMove;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }
        else
        {
            charCol.material = pMove;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (rbBodyCharacter.linearVelocity.sqrMagnitude * 10000 < 2 && YAxis == 0 && XAxis == 0 && ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && !ingameGlobalManager.instance.b_Joystick ||     // Keyboard 
            rbBodyCharacter.linearVelocity.sqrMagnitude * 10000 < 2 && AxisVertical == 0 && axisHorizontal == 0 && ingameGlobalManager.instance.b_Joystick && isOnFloor && !b_TouchLayer12_17 ||     // Joystick 
            mobileLeftJoystickToMove && rbBodyCharacter.linearVelocity.sqrMagnitude * 10000 < 2 && mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x == 0 && !ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && b_MobileMovement_Stick ||// Mobile Left Stick
            rbBodyCharacter.linearVelocity.sqrMagnitude * 10000 < 2 && !b_MoveForward && !b_MoveBackward && !b_MoveLeft && !b_MoveRight && !ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && !b_MobileMovement_Stick // Mobile Left Buttons
            )
        {
            rbBodyCharacter.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;

            gravityScale = 0;
        }

        if (b_IsJumping)
        {
            charCol.material = pIce;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }

        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rbBodyCharacter.AddForce(gravity, ForceMode.Acceleration);
    }
    #endregion

    #region isOnFloor: Check if the character is touching the floor
    //-> Check if the character is touching the floor
    public void Ap_isOnFloor()
    {
        float offset = .6f * (180 - currentAngle) / 80;

        if (isOnFloor)
            hitDistance = hitDistanceMax + offset;
        else
            hitDistance = hitDistanceMin + offset;

        if (Physics.Raycast(transform.position + Vector3.up * .1f, -Vector3.up, hitDistance, myLayer))
        {
            if (b_Overlap)
                isOnFloor = true;
            rayPosition = transform.position + Vector3.up * .1f;
        }
        else
        {
            if (b_Overlap)
                isOnFloor = false;
            rayPosition = transform.position;
        }
    }
    #endregion

    #region OverlapSphere: Check if the character is touching something. If not we consider the character is not touching the floor.
    // Check if the character is touching something. If not we consider the character is not touching the floor.
    
    private void AP_OverlapSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position+ Vector3.up * overlapPos, overlapSize, myLayer02);

        if (hits.Length > 0)
        {
            b_Overlap = true;
        }
        else
        {
            b_Overlap = false;
            isOnFloor = false;
        }
    }
    #endregion

    #region Check If Player Can Stop Crouching
    //-> Call to know if the height above the character is enough to leave the crouch mode
    private bool AP_CheckIfPlayerCanStopCrouching()
    {
        Debug.DrawRay(transform.position + Vector3.up * .1f, Vector3.up * heightCheck, Color.yellow);
        if (Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.up, heightCheck, layerCheckCrouch))
            return false;
        else
            return true;
    }
    #endregion

    #region Check if the player is on collision with a door or a drawer
    //-> Check if the player is on collision with a door or a drawer
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {b_TouchLayer12_17 = true;}
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {b_TouchLayer12_17 = true;}

        if (collision.gameObject.layer == 18)   // Specific objsFloor: Prevent Player to be stuck. If the player is on collision with this layer: It means that the player touch the floor.
        { /*Debug.Log("Border"); */isOnFloor = true; }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {b_TouchLayer12_17 = false;}
    }
    #endregion


    private void OnDrawGizmos()
    {
        #region 
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(circlePos, .1f);

        if (rayPosition == transform.position)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.blue;

        Gizmos.DrawSphere(rayPosition, .1f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position+Vector3.up * overlapPos, overlapSize);
        #endregion
    }


  

    public IEnumerator Jump()
    {
        //Debug.Log("Jump Here Start");
        fallCurve = 0;
        float t = 0;
        b_IsJumping = true;
        while (t != jumpForce)
        {
            if (!Input.GetKey(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        ingameGlobalManager.instance.b_Joystick && t > jumpForce * minimumJump

                    ||

                        !Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        !ingameGlobalManager.instance.b_Joystick && t > jumpForce * minimumJump

                || AP_CheckIfPlayerIsTouchingRoof())
            {
                //Stop Jump
                t = jumpForce;
            }
            else
            {
                rbBodyCharacter.AddForceAtPosition(Vector3.up * t * Time.deltaTime * 50, addForceObj.transform.position, ForceMode.Impulse);            // move the character

                t = Mathf.MoveTowards(t, jumpForce, Time.deltaTime * jumpSpeed);
            }

            yield return null;
        }
        b_IsJumping = false;
        fallCurve = 0;
        //Debug.Log("Jump Here End");
        yield return null;
    }

    #region Check If Player touch the roof (jump check)
    //-> Call to know if the height above the character is enough to leave the crouch mode
    private bool AP_CheckIfPlayerIsTouchingRoof()
    {
        if (Physics.Raycast(refHead.transform.position + Vector3.up * .1f, Vector3.up, heightRoof, layerCheckCrouch))
            return true;
        else
            return false;
    }
    #endregion
}
