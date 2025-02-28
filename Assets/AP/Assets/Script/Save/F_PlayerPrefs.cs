// Descritption : F_PlayerPrefs : Set input Type : Find on gameObject InputManager
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_PlayerPrefs : MonoBehaviour {
	public void SetInputType(int inputType) {
		PlayerPrefs.SetInt ("InputsType", inputType);
		ingameGlobalManager.instance.switchKeyboardJoystick ();
	}

   /* private void Start()
    {
		if (!PlayerPrefs.HasKey("InputsType"))
        {
			PlayerPrefs.SetInt("InputsType", 1);
			ingameGlobalManager.instance.switchKeyboardJoystick();
		}		
	}*/
}
