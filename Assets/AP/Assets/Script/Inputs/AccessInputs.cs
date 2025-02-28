//Description : Use in AP_Documentation.pdf to eplain how to access inputs via script (Section 15-Inputs)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessInputs : MonoBehaviour {
    
	void F_AccessInputs () {

        ingameGlobalManager gManager = ingameGlobalManager.instance;

        // Access the 1st Gamepad Float
        float newF = gManager.inputListOfFloatGamepadButton[0];
        // Access the 1st Keyboard Float
        newF = gManager.inputListOfFloatKeyboardButton[0];


        // Access the 1st Gamepad boolean
        bool newB = gManager.inputListOfBoolGamepadButton[0];
        // Access the 1st  Keyboard boolean
        newB = gManager.inputListOfBoolKeyboardButton[0];


        // Access the 1st Gamepad Axis
        if(Input.GetKeyDown(gManager.inputListOfStringGamepadAxis[0])){
        }

        // Access the 1st Keyboard Axis
        if (Input.GetKeyDown(gManager.inputListOfStringKeyboardAxis[0])){
        }

        // Access the 1st Gamepad Button
        if (Input.GetKeyDown(gManager.inputListOfStringGamepadButton[0])){
        }

        // Access the 1st Keyboard Button
        if (Input.GetKeyDown(gManager.inputListOfStringKeyboardButton[0])){
        }

	}
}
