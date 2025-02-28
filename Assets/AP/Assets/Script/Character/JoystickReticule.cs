// Description : Move the fake mouse with the joystick
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickReticule : MonoBehaviour {
	public bool 				SeeInspector = false;

	//private float				minimumAxisMovement = .3f;
	//private float 				tmpXAxis 	= 0;			// temporary values
	//private float 				tmpYAxis 	= 0;	

	public float 				sensibilityJoystick = .1f;	// Joystick sensibility

	public Image 				joyReticule;

	public AnimationCurve		animationCurveJoystick;

	public RectTransform 		joyReticule2;

	public int 					horizontalJoystick 	= 1;
	public int 					verticaJloystick 	= 0;

    Vector3 joyInput = Vector3.zero;



	private void Awake()
	{
        if (joyReticule2) joyReticule2.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
        if (ingameGlobalManager.instance.b_InputIsActivated)
    		MoveJoystickReticule ();
        
           
	}


	public void MoveJoystickReticule(){
		float joyVertical = Input.GetAxis (ingameGlobalManager.instance.inputListOfStringGamepadAxis[verticaJloystick]);
		float joyHorizontal = Input.GetAxis (ingameGlobalManager.instance.inputListOfStringGamepadAxis[horizontalJoystick]);


        joyInput = new Vector2(joyHorizontal,-joyVertical);

        if (joyInput.sqrMagnitude > 1.0f)
            joyInput = joyInput.normalized;



        if(joyInput.sqrMagnitude > .005f)
        joyReticule2.position += joyInput * sensibilityJoystick * Time.deltaTime * 5;

        joyReticule2.position = new Vector3(Mathf.Clamp(joyReticule2.position.x, 0, Screen.width * .97f),
                                            Mathf.Clamp(joyReticule2.position.y,Screen.height * 0.07f, Screen.height),
                                            0);
        

        joyReticule.rectTransform.pivot = new Vector2 (joyReticule2.position.x / Screen.width, joyReticule2.position.y / Screen.height);
	}

	public void newPosition(float newPosX, float newPosY){


        joyReticule2.position = new Vector3(newPosX, newPosY,0);
        joyReticule.rectTransform.pivot = new Vector2(joyReticule2.position.x / Screen.width, joyReticule2.position.y / Screen.height);

	}

}
