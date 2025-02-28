//Descriptioin : investigationCam : Manage Object movement in 3D viewer 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class investigationCam : MonoBehaviour {
	public bool 				SeeInspector = false;
	private bool 				turnTableIsActivated = false;
	public Transform 			turntable;
	public float 				speed = 15;
	//public float 				learpSpeed = 5;
	private float 				xDeg;
	private float	 			yDeg;
	private Quaternion 			fromRotation;
	private Quaternion 			toRotation;
	public int 					intTest = 0;

	private float				minimumAxisMovement = .3f;

	public float 				speedJoystick = 2;
	public AnimationCurve		animationCurveJoystick;

	private int 				MouseRotateObject = 0;

	private string 				MouseX = "Mouse X";
	private string 				MouseY = "Mouse Y";



	public float 				mobileSpeedRotation = .3f;
	private Vector2[] 			arrStartPos = new Vector2[10]; 

	private GameObject 			currentGameObject;

	public int 					horizontalJoystick 	= 1;
	public int 					verticaJloystick 	= 0;

    public Transform            lightProbeRef;
	GameObject inventoryCanvas;


	void Start()
    {
		var inventory = FindFirstObjectByType<inventoryALLEntry>();

		if (inventory)
			inventoryCanvas = inventory.canvasInventory.gameObject;
	}

    void Update (){
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (ingameGlobalManager.instance.b_DesktopInputs)
            {
                //--> Mouse case
                mouseRotation();

                //--> Joystick case
                joystickRotation();
            }
            else
            {
                //--> Touch case
                touchRotation();
            }
        }
	}

//--> Mouse case
	public void mouseRotation(){
		if (Input.GetMouseButton (MouseRotateObject) && turnTableIsActivated) {
            
			float RotationOnXAxis = Input.GetAxis (MouseX) * speed;
			float RotationOnYAxis = Input.GetAxis (MouseY) * speed;

			turntable.transform.Rotate(Vector3.up*-RotationOnXAxis, Space.World);
			turntable.transform.Rotate(Vector3.right*RotationOnYAxis, Space.World);
		}
	}

//--> Joystick case
	public void joystickRotation(){
		if (turnTableIsActivated) {
			float joyVertical = Input.GetAxis (ingameGlobalManager.instance.inputListOfStringGamepadAxis [verticaJloystick]);
			float joyHorizontal = Input.GetAxis (ingameGlobalManager.instance.inputListOfStringGamepadAxis [horizontalJoystick]);

			if (joyHorizontal > minimumAxisMovement || joyHorizontal < -minimumAxisMovement) {
				int dir = -1;
				if (joyHorizontal < 0)
					dir = 1;
				//Debug.Log (Input.GetAxis ("Horizontal Joystick"));
				turntable.transform.Rotate (Vector3.up * dir * animationCurveJoystick.Evaluate (Mathf.Abs (joyHorizontal)) * speedJoystick * Time.deltaTime, Space.World);
			}

			if (joyVertical > minimumAxisMovement || joyVertical < -minimumAxisMovement) {
				int dir = -1;
				if (joyVertical < 0)
					dir = 1;
				turntable.transform.Rotate (Vector3.right * -dir * animationCurveJoystick.Evaluate (Mathf.Abs (joyVertical)) * speedJoystick * -1 * Time.deltaTime, Space.World);
			}
		}
	}



//--> Show a new Object in the 3D viewer
    public void newObjectToInvestigate(GameObject refObject,float newScale,Vector3 newEulerAngle,bool b_ObjPuzzleNotAvailable,bool b_ObjSubtitle){
		turnTableIsActivated = true;
		turntable.rotation = Quaternion.identity;
		/*if (ingameGlobalManager.instance.b_Joystick 
			&& ingameGlobalManager.instance.b_DesktopInputs
			&& ingameGlobalManager.instance._joystickReticule != null) {
			ingameGlobalManager.instance._joystickReticule.mouseX = .5f;
			ingameGlobalManager.instance._joystickReticule.mouseY = .5f;
		}*/

		GameObject newObj = Instantiate (refObject, turntable);
		currentGameObject = newObj;
		newObj.transform.localScale = new Vector3 (newScale, newScale, newScale);
		newObj.transform.localEulerAngles = newEulerAngle;
		newObj.transform.localPosition = Vector3.zero;
		newObj.layer = 8;	// Layer 8 : use to display object on 3D Viewer
        if(lightProbeRef)
            newObj.GetComponent<Renderer>().probeAnchor = lightProbeRef;

		Transform[] children = newObj.GetComponentsInChildren<Transform> ();

		foreach(Transform child in children ){
			child.gameObject.layer = 8;
		}

        //ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjTitle();
        if(b_ObjPuzzleNotAvailable)
            ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjPuzzleNotAvailable();
        
       // ingameGlobalManager.instance.canvasPlayerInfos.deactivateObjResetPuzzle();

        if (b_ObjSubtitle)
            ingameGlobalManager.instance.voiceOverManager.deactivateObSubtitle();
       

	}

	//--> Use to close/ disable 3D viewer on screen
	public void clearInvestigateView(){
		//gameObject.SetActive (false);
		if(currentGameObject != null)
			Destroy(currentGameObject.gameObject);

		turntable.rotation = Quaternion.identity;

		turnTableIsActivated = false;

		inventoryCanvas.SetActive(true);
	}


//--> Touch case
	public void touchRotation(){
		if (turnTableIsActivated) {
			for (int i = 0; i < Input.touchCount; ++i) {
				Vector2 touchDeltaPosition = Input.GetTouch (i).deltaPosition;
				//Debug.Log (returnDirection (touchDeltaPosition));

				if (Input.GetTouch (i).phase == TouchPhase.Began) {
					arrStartPos [i] = Input.GetTouch (i).position;
				}

				if (Input.GetTouch (i).phase == TouchPhase.Moved) {
					float swipe = (new Vector3 (Input.GetTouch (i).position.x, Input.GetTouch (i).position.y, 0) - new Vector3 (arrStartPos [i].x, arrStartPos [i].y, 0)).magnitude;

					if (swipe > 50) {
						turntable.transform.Rotate (Vector3.up * -touchDeltaPosition.x * mobileSpeedRotation * Time.deltaTime, Space.World);
						turntable.transform.Rotate (Vector3.right * touchDeltaPosition.y * mobileSpeedRotation * Time.deltaTime, Space.World);
					}
				}
			}
		}
	}


//--> Return touch movement direction
	string returnDirection(Vector2 touchDeltaPosition){
		string _direction = "";
		if (touchDeltaPosition.x > 0)_direction += "Right /";
		if (touchDeltaPosition.x < 0)_direction += "Left /";
		if (touchDeltaPosition.y > 0)_direction += "Up";
		if (touchDeltaPosition.y < 0)_direction += "Down";

		return _direction;
	}


}
