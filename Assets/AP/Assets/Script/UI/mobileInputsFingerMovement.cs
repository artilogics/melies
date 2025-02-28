//Description : mobileInputsFingerMovement. Various function to check finger gesture
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mobileInputsFingerMovement : MonoBehaviour {
	public GraphicRaycaster 	m_Raycaster;							// Use to check the finger touch a UI button		
	EventSystem 				m_EventSystem;
	public List<GraphicRaycaster> 	listRaycaster = new List<GraphicRaycaster>();							// Use to check the finger touch a UI button		


//-> Double Tab variables
	[Header ("Double Tab")]
	public float 		DoubleTapDuration = 0.8f; 
	private float[] 	arrTimer = new float[10]; 
	public string[] 	ignoreTag	= new string[]{"IgnoreDoubleTap"};

//--> Swip variables
	[Header ("Swip")]
	public int 			swipSensibility = 150;
	private Vector2 	startPos;
	private Vector2[] 	arrStartPos = new Vector2[10]; 


	void Start()
	{
		m_EventSystem = GetComponent<EventSystem>();
	}

//--> Detect Double Tab on screen 
	public bool checkDoubleTap(){
		bool result = false;
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                //Debug.Log (checkIfFingerTouchAUIButton (Input.GetTouch(i).position));
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {

                    if (arrTimer[i] < DoubleTapDuration                                     // Debug.Log ("Double tap");
                        && !checkIfFingerTouchAUIButton(Input.GetTouch(i).position))
                    {       // Finger is not touching UI object with tag in ignoreTag array		
                        Debug.Log("Double Tap");
                        result = true;
                        arrTimer[i] = DoubleTapDuration + .2f;

                    }
                    else
                    {
                        arrTimer[i] = 0;
                    }
                }
            }

            for (int i = 0; i < arrTimer.Length; ++i)
            {
                if (arrTimer[i] < DoubleTapDuration + .2f)
                {
                    arrTimer[i] += Time.deltaTime;
                }
            }
        }
		return result;
	}





//--> return if there is a swip on mobile screen
	public string checkSwip(bool b_CheckUp,bool b_CheckDown,bool b_CheckLeft,bool b_CheckRight){

		string result = "";
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.touches[i];

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        arrStartPos[i] = touch.position;
                        break;

                    case TouchPhase.Ended:
                        float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, arrStartPos[i].y, 0)).magnitude;

                        if (swipeDistVertical > swipSensibility)
                        {
                            float swipeValue = Mathf.Sign(touch.position.y - arrStartPos[i].y);
                            if (swipeValue > 0)
                            {
                                if (b_CheckUp) result += "Swipe Up";
                            }
                            else if (swipeValue < 0)
                            {
                                if (b_CheckDown) result += "Swipe Down";
                            }
                        }

                        float swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(arrStartPos[i].x, 0, 0)).magnitude;

                        if (swipeDistHorizontal > swipSensibility)
                        {
                            if ((b_CheckUp || b_CheckDown) && result != "")
                            {
                                result += " : ";
                            }

                            float swipeValue = Mathf.Sign(touch.position.x - arrStartPos[i].x);
                            if (swipeValue > 0)
                            {
                                if (b_CheckLeft) result += "Swipe Right";
                            }
                            else if (swipeValue < 0)
                            {
                                if (b_CheckRight) result += "Swipe Left";
                            }
                        }
                        break;
                }
            }
        }
		return result;
	}



public bool checkIfFingerTouchAUIButton(Vector2 newPos){
		bool b_result = false;

		for (var i = 0; i < listRaycaster.Count; i++) {
			if (listRaycaster[i] != null) {
				//Set up the new Pointer Event
				PointerEventData m_PointerEventData = new PointerEventData (m_EventSystem);
				//Set the Pointer Event Position to that of the mouse position
				m_PointerEventData.position = newPos;


				//Create a list of Raycast Results
				List<RaycastResult> results = new List<RaycastResult> ();
				//Raycast using the Graphics Raycaster and mouse click position
				listRaycaster[i].Raycast (m_PointerEventData, results);

				//Debug.Log (results.Count);

				//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
				foreach (RaycastResult result in results) {
					Debug.Log(result.gameObject.tag);
					if (result.gameObject.CompareTag (ignoreTag [0])) {			// Type : Item
						b_result = true;

						break;
					}

				}

			}
		}
		//Debug.Log (b_result);
		return b_result;
	}

	public void initDoubleTap(){
		for (int i = 0; i < arrTimer.Length; ++i) {		
			arrTimer [i] = DoubleTapDuration + .2f;
		}
	}
}
