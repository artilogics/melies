//Description : footstepSystemEditor. Work in association with footstepSystem. Allow to manage player foostep sound depending the surface
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[CustomEditor(typeof(characterMovement))]
public class characterMovementEditor : Editor {
	SerializedProperty	SeeInspector;											// use to draw default Inspector

	SerializedProperty	rbBodyCharacter;
	SerializedProperty 	objCamera;
	SerializedProperty 	addForceObj;
	SerializedProperty	minimum;
	SerializedProperty 	maximum;
	SerializedProperty 	characterSpeed;
	SerializedProperty 	sensibilityMouse;
	SerializedProperty 	sensibilityJoystick;
	SerializedProperty	animationCurveJoystick;
	SerializedProperty	mobileToystickController;
	SerializedProperty	sensibilityMobile;
	SerializedProperty	animationCurveMobile;

	SerializedProperty 			forwardKeyboard;
	SerializedProperty 			backwardKeyboard;
	SerializedProperty 			leftKeyboard;
	SerializedProperty 			rightKeyboard;

    SerializedProperty b_MobileMovement_Stick;
    SerializedProperty b_MobileCamRotation_Stick;

    SerializedProperty mobileSpeedRotation;
    SerializedProperty allowCrouch;
    SerializedProperty JoystickCrouch;
    SerializedProperty KeyboardCrouch;

   

	SerializedProperty 			VerticalAxisBody;
	SerializedProperty 			HorizontalAxisBody;
	SerializedProperty 			JoystickVerticalAxisCam;
	SerializedProperty 			JoystickHorizontalAxisCam;

	SerializedProperty 			mouseInvertYAxisCam;
	SerializedProperty 			joystickInvertYAxisCam;

    SerializedProperty JoystickRun;
    SerializedProperty KeyboardRun;
    SerializedProperty b_AllowRun;
    SerializedProperty speedMultiplier;

    SerializedProperty JoystickJump;
    SerializedProperty KeyboardJump;
    SerializedProperty b_AllowJump;
    SerializedProperty jumpForce;
    SerializedProperty GravityFallSpeed;

    SerializedProperty hitDistanceMin;
    SerializedProperty hitDistanceMax;
    SerializedProperty MaxAngle;
    SerializedProperty moreInfoMaxAngle;

	public List<string> s_inputListJoystickAxis = new List<string> ();
	public List<string> s_inputListJoystickButton = new List<string> ();
	public List<string> s_inputListKeyboardAxis = new List<string> ();
	public List<string> s_inputListKeyboardButton = new List<string> ();

	public List<string>  s_inputListJoystickBool = new List<string> ();
	public List<string> s_inputListKeyboardBool= new List<string> ();



	public GameObject objCanvasInput;


	private Texture2D MakeTex(int width, int height, Color col) {						// use to change the GUIStyle
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	private Texture2D 		Tex_01;														// 
	private Texture2D 		Tex_02;
	private Texture2D 		Tex_03;
	private Texture2D 		Tex_04;
	private Texture2D 		Tex_05;

	public string selectedTag = "";

	public string newTagName = "";

	void OnEnable () {
		// Setup the SerializedProperties.
		SeeInspector 				= serializedObject.FindProperty ("SeeInspector");

		rbBodyCharacter				= serializedObject.FindProperty ("rbBodyCharacter");
		objCamera					= serializedObject.FindProperty ("objCamera");
		addForceObj					= serializedObject.FindProperty ("addForceObj");
		minimum						= serializedObject.FindProperty ("minimum");
		maximum						= serializedObject.FindProperty ("maximum");
		characterSpeed				= serializedObject.FindProperty ("characterSpeed");
		sensibilityMouse			= serializedObject.FindProperty ("sensibilityMouse");
		sensibilityJoystick			= serializedObject.FindProperty ("sensibilityJoystick");
		animationCurveJoystick		= serializedObject.FindProperty ("animationCurveJoystick");
		mobileToystickController	= serializedObject.FindProperty ("mobileToystickController");
		sensibilityMobile			= serializedObject.FindProperty ("sensibilityMobile");
		animationCurveMobile		= serializedObject.FindProperty ("animationCurveMobile");

		forwardKeyboard				= serializedObject.FindProperty ("forwardKeyboard");
		backwardKeyboard			= serializedObject.FindProperty ("backwardKeyboard");
		leftKeyboard				= serializedObject.FindProperty ("leftKeyboard");
		rightKeyboard				= serializedObject.FindProperty ("rightKeyboard");

        b_MobileMovement_Stick = serializedObject.FindProperty("b_MobileMovement_Stick");
        b_MobileCamRotation_Stick = serializedObject.FindProperty("b_MobileCamRotation_Stick");

        mobileSpeedRotation = serializedObject.FindProperty("mobileSpeedRotation");
        //LeftStickSensibility = serializedObject.FindProperty("LeftStickSensibility");


        allowCrouch = serializedObject.FindProperty("allowCrouch");
        JoystickCrouch = serializedObject.FindProperty("JoystickCrouch");
        KeyboardCrouch = serializedObject.FindProperty("KeyboardCrouch");
   

		VerticalAxisBody			= serializedObject.FindProperty ("VerticalAxisBody");
		HorizontalAxisBody			= serializedObject.FindProperty ("HorizontalAxisBody");
		JoystickVerticalAxisCam		= serializedObject.FindProperty ("JoystickVerticalAxisCam");
		JoystickHorizontalAxisCam	= serializedObject.FindProperty ("JoystickHorizontalAxisCam");

		mouseInvertYAxisCam			= serializedObject.FindProperty ("mouseInvertYAxisCam");
		joystickInvertYAxisCam		= serializedObject.FindProperty ("joystickInvertYAxisCam");

        JoystickRun = serializedObject.FindProperty("JoystickRun");
        KeyboardRun = serializedObject.FindProperty("KeyboardRun");
        b_AllowRun = serializedObject.FindProperty("b_AllowRun");
        speedMultiplier = serializedObject.FindProperty("speedMultiplier");

        JoystickJump = serializedObject.FindProperty("JoystickJump");
        KeyboardJump = serializedObject.FindProperty("KeyboardJump");
        b_AllowJump = serializedObject.FindProperty("b_AllowJump");
        jumpForce = serializedObject.FindProperty("jumpForce");
        GravityFallSpeed = serializedObject.FindProperty("GravityFallSpeed");

        hitDistanceMin = serializedObject.FindProperty("hitDistanceMin");
        hitDistanceMax = serializedObject.FindProperty("hitDistanceMax");
        MaxAngle = serializedObject.FindProperty("MaxAngle");
        moreInfoMaxAngle = serializedObject.FindProperty("moreInfoMaxAngle");

        if (EditorPrefs.GetBool("AP_ProSkin") == true)
        {
            float darkIntiensity = EditorPrefs.GetFloat("AP_DarkIntensity");
            Tex_01 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_02 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
            Tex_03 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .5f));
            Tex_04 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .3f));
            Tex_05 = MakeTex(2, 2, new Color(darkIntiensity, darkIntiensity, darkIntiensity, .4f));
        }
        else
        {
            Tex_01 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_02 = MakeTex(2, 2, new Color(1, .8f, 0.2F, .4f));
            Tex_03 = MakeTex(2, 2, new Color(.3F, .9f, 1, .5f));
            Tex_04 = MakeTex(2, 2, new Color(1, .3f, 1, .3f));
            Tex_05 = MakeTex(2, 2, new Color(1, .5f, 0.3F, .4f));
        }

        GameObject tmp = GameObject.Find ("InputsManager");
		if(tmp){
			objCanvasInput = tmp;
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[0].buttonsList.Count;i++){
				s_inputListJoystickAxis.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [0].buttonsList [i].name);
			}
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[1].buttonsList.Count;i++){
				s_inputListJoystickButton.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [1].buttonsList [i].name);
			}



			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[2].buttonsList.Count;i++){
				s_inputListKeyboardAxis.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [2].buttonsList [i].name);
			}
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().remapButtons[3].buttonsList.Count;i++){
				s_inputListKeyboardButton.Add (tmp.GetComponent<MM_MenuInputs> ().remapButtons [3].buttonsList [i].name);
			}

			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().boolValues[0].buttonsList.Count;i++){
				s_inputListJoystickBool.Add (tmp.GetComponent<MM_MenuInputs> ().boolValues [0].buttonsList [i].name);
			}
			for(var i = 0;i< tmp.GetComponent<MM_MenuInputs>().boolValues[1].buttonsList.Count;i++){
				s_inputListKeyboardBool.Add (tmp.GetComponent<MM_MenuInputs> ().boolValues [1].buttonsList [i].name);
			}






		}
	}


    public override void OnInspectorGUI()
    {
        if (SeeInspector.boolValue)                         // If true Default Inspector is drawn on screen
            DrawDefaultInspector();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("See Inspector :", GUILayout.Width(85));
        EditorGUILayout.PropertyField(SeeInspector, new GUIContent(""), GUILayout.Width(30));
        EditorGUILayout.EndHorizontal();

        GUIStyle style_Yellow_01 = new GUIStyle(); style_Yellow_01.normal.background = Tex_01;
        GUIStyle style_Blue = new GUIStyle(); style_Blue.normal.background = Tex_03;
        GUIStyle style_Purple = new GUIStyle(); style_Purple.normal.background = Tex_04;
        GUIStyle style_Orange = new GUIStyle(); style_Orange.normal.background = Tex_05;
        GUIStyle style_Yellow_Strong = new GUIStyle(); style_Yellow_Strong.normal.background = Tex_02;


        //GUILayout.Label("");
        characterMovement myScript = (characterMovement)target;


        EditorGUILayout.HelpBox("This script allow to setup character movement on desktop and Mobile", MessageType.Info);
        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginVertical(style_Yellow_01);
        EditorGUILayout.HelpBox("The next 4 fields need to be connected", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Character Rigidbody :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(rbBodyCharacter, new GUIContent(""), GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Character Camera :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(objCamera, new GUIContent(""), GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("M Stick controller :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(mobileToystickController, new GUIContent(""), GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add Force Position :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(addForceObj, new GUIContent(""), GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("");

        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Character speed", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Character speed :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(characterSpeed, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speed multiplier (Run) :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(speedMultiplier, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        */
        EditorGUILayout.EndVertical();


   
        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Character Climbing Max Angle", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max angle (Max 80):", GUILayout.Width(160));
        EditorGUILayout.PropertyField(MaxAngle, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ray Minimum Distance:", GUILayout.Width(160));
        EditorGUILayout.PropertyField(hitDistanceMin, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ray Maximum Distance:", GUILayout.Width(160));
        EditorGUILayout.PropertyField(hitDistanceMax, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("More info:", GUILayout.Width(60));
        EditorGUILayout.PropertyField(moreInfoMaxAngle, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        if(moreInfoMaxAngle.boolValue){
            EditorGUILayout.HelpBox("You have to adapt the Ray Minimum Distance and Max Distance depending the Max Angle." +
                                    "\nFor Max Angle:" +
                                    "\n45 choose Min = .15 and Max = .45" +
                                    "\n70 choose Min = .25 and Max = .55" +
                                    "\n80 choose Min = .45 and Max = .85", MessageType.Info);
        }


        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical(style_Orange);
        EditorGUILayout.HelpBox("Camera Options", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Minimum Camera Angle :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(minimum, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Maximum Camera Angle :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(maximum, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("");


        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Desktop Mouse and Keyboard Options", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Mouse sensibility :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(sensibilityMouse, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        if (objCanvasInput)
        {
            //-> Mouse Invert Y Cam Axis
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Invert Y Look: ", GUILayout.Width(160));
            mouseInvertYAxisCam.intValue = EditorGUILayout.Popup(mouseInvertYAxisCam.intValue, s_inputListKeyboardBool.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> forward Keyboard
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Forward : ", GUILayout.Width(160));
            forwardKeyboard.intValue = EditorGUILayout.Popup(forwardKeyboard.intValue, s_inputListKeyboardButton.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> backward Keyboard
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Backward : ", GUILayout.Width(160));
            backwardKeyboard.intValue = EditorGUILayout.Popup(backwardKeyboard.intValue, s_inputListKeyboardButton.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> left Keyboard
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Left : ", GUILayout.Width(160));
            leftKeyboard.intValue = EditorGUILayout.Popup(leftKeyboard.intValue, s_inputListKeyboardButton.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> right Keyboard
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Right : ", GUILayout.Width(160));
            rightKeyboard.intValue = EditorGUILayout.Popup(rightKeyboard.intValue, s_inputListKeyboardButton.ToArray());
            EditorGUILayout.EndHorizontal();


            //-> Crouch
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Crouch: ", GUILayout.Width(160));

            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(allowCrouch, new GUIContent(""), GUILayout.Width(20));
            if(allowCrouch.boolValue)
            KeyboardCrouch.intValue = EditorGUILayout.Popup(KeyboardCrouch.intValue, s_inputListKeyboardButton.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Run
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Run: ", GUILayout.Width(160));

            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(b_AllowRun, new GUIContent(""), GUILayout.Width(20));


            if (b_AllowRun.boolValue)
            {
                EditorGUILayout.LabelField("speed:", GUILayout.Width(40));
                EditorGUILayout.PropertyField(speedMultiplier, new GUIContent(""), GUILayout.Width(20));
                KeyboardRun.intValue = EditorGUILayout.Popup(KeyboardRun.intValue, s_inputListKeyboardButton.ToArray());
            }
               
            EditorGUILayout.EndHorizontal();

             //-> Jump
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Jump: ", GUILayout.Width(160));

            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(b_AllowJump, new GUIContent(""), GUILayout.Width(20));

            if (b_AllowJump.boolValue)
            {
                KeyboardJump.intValue = EditorGUILayout.Popup(KeyboardJump.intValue, s_inputListKeyboardButton.ToArray());
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (b_AllowJump.boolValue)
            {
                GUILayout.Label("", GUILayout.Width(230));
                EditorGUILayout.LabelField("Force:", GUILayout.Width(40));
                EditorGUILayout.PropertyField(jumpForce, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Fall Speed:", GUILayout.Width(70));
                EditorGUILayout.PropertyField(GravityFallSpeed, new GUIContent(""), GUILayout.Width(30));

            }
            EditorGUILayout.EndHorizontal();

        }


        EditorGUILayout.EndVertical();

        //EditorGUILayout.LabelField ("");

        EditorGUILayout.BeginVertical(style_Blue);
        EditorGUILayout.HelpBox("Desktop Joystick Options", MessageType.Info);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Joystick sensibility :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(sensibilityJoystick, new GUIContent(""), GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Joystick sensibility curve :", GUILayout.Width(160));
        EditorGUILayout.PropertyField(animationCurveJoystick, new GUIContent(""), GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        if (objCanvasInput)
        {
            //-> Joystick Input to move  Horizontaly
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Horizontal Body: ", GUILayout.Width(160));
            HorizontalAxisBody.intValue = EditorGUILayout.Popup(HorizontalAxisBody.intValue, s_inputListJoystickAxis.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Joystick Input to move  verticaly
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertical Body : ", GUILayout.Width(160));
            VerticalAxisBody.intValue = EditorGUILayout.Popup(VerticalAxisBody.intValue, s_inputListJoystickAxis.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Joystick Input to move  Horizontaly
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Horizontal Cam : ", GUILayout.Width(160));
            JoystickHorizontalAxisCam.intValue = EditorGUILayout.Popup(JoystickHorizontalAxisCam.intValue, s_inputListJoystickAxis.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Joystick Input to move  verticaly
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Vertical Cam : ", GUILayout.Width(160));
            JoystickVerticalAxisCam.intValue = EditorGUILayout.Popup(JoystickVerticalAxisCam.intValue, s_inputListJoystickAxis.ToArray());
            EditorGUILayout.EndHorizontal();


            //-> Joystick Input Invert Y Cam Axis
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Invert Y Look: ", GUILayout.Width(160));
            joystickInvertYAxisCam.intValue = EditorGUILayout.Popup(joystickInvertYAxisCam.intValue, s_inputListJoystickBool.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Joystick Crouch Button
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Crouch Button: ", GUILayout.Width(160));
            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(allowCrouch, new GUIContent(""), GUILayout.Width(20));
            if (allowCrouch.boolValue)
            JoystickCrouch.intValue = EditorGUILayout.Popup(JoystickCrouch.intValue, s_inputListJoystickButton.ToArray());
            EditorGUILayout.EndHorizontal();

            //-> Run
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Run: ", GUILayout.Width(160));

            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(b_AllowRun, new GUIContent(""), GUILayout.Width(20));

            if (b_AllowRun.boolValue)
            {
                EditorGUILayout.LabelField("speed:", GUILayout.Width(40));
                EditorGUILayout.PropertyField(speedMultiplier, new GUIContent(""), GUILayout.Width(20));
                JoystickRun.intValue = EditorGUILayout.Popup(JoystickRun.intValue, s_inputListJoystickButton.ToArray());
            }

            EditorGUILayout.EndHorizontal();

             //-> Jump
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Jump: ", GUILayout.Width(160));

            EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(b_AllowJump, new GUIContent(""), GUILayout.Width(20));

            if (b_AllowJump.boolValue)
            {
                JoystickJump.intValue = EditorGUILayout.Popup(JoystickJump.intValue, s_inputListJoystickButton.ToArray());
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (b_AllowJump.boolValue)
            {
                GUILayout.Label("", GUILayout.Width(230));
                EditorGUILayout.LabelField("Force:", GUILayout.Width(40));
                EditorGUILayout.PropertyField(jumpForce, new GUIContent(""), GUILayout.Width(20));
                EditorGUILayout.LabelField("Fall Speed:", GUILayout.Width(70));
                EditorGUILayout.PropertyField(GravityFallSpeed, new GUIContent(""), GUILayout.Width(30));

            }
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.EndVertical();

        //EditorGUILayout.LabelField ("");

        /*
        EditorGUILayout.BeginVertical(style_Blue);
      EditorGUILayout.HelpBox("Mobile Joystick Options", MessageType.Info);


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Movement use :", GUILayout.Width(160));


        //-> Left Stick
        string newtitle = "Left Stick";
        if (!b_MobileMovement_Stick.boolValue)
            newtitle = "Left Buttons";

        if (GUILayout.Button(newtitle, GUILayout.Width(200)))
        {
            b_MobileMovement_Stick.boolValue = !b_MobileMovement_Stick.boolValue;

            GameObject Grp_Canvas = GameObject.Find("Grp_Canvas");

            Transform[] allChildren = Grp_Canvas.GetComponentsInChildren<Transform>(true);
            canvasMobileConnect mobileCanvas = null;

            foreach (Transform child in allChildren)
            {
                if (child.name == "mobileCanvas")
                {
                    mobileCanvas = child.GetComponent<canvasMobileConnect>();
                    break;
                }
            }
            Undo.RegisterFullObjectHierarchyUndo(mobileCanvas.grp_LeftButtonsMove, mobileCanvas.grp_LeftButtonsMove.name);
            Undo.RegisterFullObjectHierarchyUndo(mobileCanvas.virtualJoystickLeftStickToMove, mobileCanvas.virtualJoystickLeftStickToMove.name);

            if (b_MobileMovement_Stick.boolValue)
            {
                mobileCanvas.grp_LeftButtonsMove.SetActive(false);
                mobileCanvas.virtualJoystickLeftStickToMove.gameObject.SetActive(true);
            }
            else
            {
                mobileCanvas.grp_LeftButtonsMove.SetActive(true);
                mobileCanvas.virtualJoystickLeftStickToMove.gameObject.SetActive(false);
            }

        }
        EditorGUILayout.EndHorizontal();

     
        
    
        //EditorGUILayout.LabelField("");


        //-> Right Stick
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Camera Rotation use :", GUILayout.Width(160));

        newtitle = "Right Stick";
        if (!b_MobileCamRotation_Stick.boolValue)
            newtitle = "No Stick";


        if (GUILayout.Button(newtitle, GUILayout.Width(200)))
        {
            b_MobileCamRotation_Stick.boolValue = !b_MobileCamRotation_Stick.boolValue;

            GameObject Grp_Canvas = GameObject.Find("Grp_Canvas");

            Transform[] allChildren = Grp_Canvas.GetComponentsInChildren<Transform>(true);
            canvasMobileConnect mobileCanvas = null;

            foreach (Transform child in allChildren)
            {
                if (child.name == "mobileCanvas")
                {
                    mobileCanvas = child.GetComponent<canvasMobileConnect>();
                    break;
                }
            }
            Undo.RegisterFullObjectHierarchyUndo(mobileCanvas.virtualJoystick, mobileCanvas.virtualJoystick.name);
            if (!b_MobileCamRotation_Stick.boolValue)
            {
                mobileCanvas.virtualJoystick.gameObject.SetActive(false);
            }
            else
            {
                mobileCanvas.virtualJoystick.gameObject.SetActive(true);
            }



        }
        EditorGUILayout.EndHorizontal();

        // Right Stick Options 
        if (b_MobileCamRotation_Stick.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mobile stick sensibility :", GUILayout.Width(160));
            EditorGUILayout.PropertyField(sensibilityMobile, new GUIContent(""), GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mobile stick sensibility curve :", GUILayout.Width(160));
            EditorGUILayout.PropertyField(animationCurveMobile, new GUIContent(""), GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();


        }
        // No Stick Options
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("sensibility :", GUILayout.Width(160));
            EditorGUILayout.PropertyField(mobileSpeedRotation, new GUIContent(""), GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
        }


        //-> Crouch Button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Crouch Button: ", GUILayout.Width(160));
        EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
        EditorGUILayout.PropertyField(allowCrouch, new GUIContent(""), GUILayout.Width(20));
        if (allowCrouch.boolValue)
            JoystickCrouch.intValue = EditorGUILayout.Popup(JoystickCrouch.intValue, s_inputListJoystickButton.ToArray());
        EditorGUILayout.EndHorizontal();
        
        //-> Run
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Run: ", GUILayout.Width(160));

        EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
        EditorGUILayout.PropertyField(b_AllowRun, new GUIContent(""), GUILayout.Width(20));

        if (b_AllowRun.boolValue)
        {
            EditorGUILayout.LabelField("speed:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(speedMultiplier, new GUIContent(""), GUILayout.Width(20));
        }

        EditorGUILayout.EndHorizontal();

         //-> Jump
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Jump: ", GUILayout.Width(160));

        EditorGUILayout.LabelField("Allow:", GUILayout.Width(40));
        EditorGUILayout.PropertyField(b_AllowJump, new GUIContent(""), GUILayout.Width(20));

       
        if (b_AllowJump.boolValue)
        {
            EditorGUILayout.LabelField("Force:", GUILayout.Width(40));
            EditorGUILayout.PropertyField(jumpForce, new GUIContent(""), GUILayout.Width(20));
            EditorGUILayout.LabelField("Fall Speed:", GUILayout.Width(70));
            EditorGUILayout.PropertyField(GravityFallSpeed, new GUIContent(""), GUILayout.Width(30));

        }
        EditorGUILayout.EndHorizontal();
    
		EditorGUILayout.EndVertical ();
      */

		EditorGUILayout.LabelField ("");
		EditorGUILayout.LabelField ("");

		serializedObject.ApplyModifiedProperties ();
	}



	void OnSceneGUI( )
	{
	}
}
#endif