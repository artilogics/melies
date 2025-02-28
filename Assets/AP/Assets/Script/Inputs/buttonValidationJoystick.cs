// Description : buttonValidationJoystick : Use To invoke onClick() from a button on joystick
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buttonValidationJoystick : MonoBehaviour {

	public bool 			SeeInspector = false;
	private EventSystem 	eventSystem;
	public int 				validationButtonJoystick 	= 4;

	// Use this for initialization
	void Start () {
        eventSystem = gameObject.GetComponent<EventSystem> ();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            ingameGlobalManager.instance.StartCoroutine(ingameGlobalManager.instance.changeLockStateConfined(true));
	}
	
	// Update is called once per frame
	void Update () {
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[validationButtonJoystick]))
            {
                //
                if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject.GetComponent<Button>())
                {
                    Debug.Log("Submit Joystick Button Pressed : " + eventSystem.currentSelectedGameObject.name);
                    eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
                }
                if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject.GetComponent<Toggle>())
                {
                    Debug.Log("Submit Joystick Toggle Pressed : " + eventSystem.currentSelectedGameObject.name);
                    if (eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                    else
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
                }
            }
        }
	}
}
