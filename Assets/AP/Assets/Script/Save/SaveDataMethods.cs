// Description : CallMethods.cs : use to call function
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[System.Serializable]
public class SaveDataMethods {
	private string s_SplitEntry = ":";
	private string s_SplitValues = "_";

	public void parseString(string playerPrefsName){


		string allCodes = PlayerPrefs.GetString(playerPrefsName);

		// this splits the string using comma as a parameter.
		// every index on the array will now have a code in it.
		// note that this method requires you to use a single ' instead of "
		string[] codes  = allCodes.Split(new string[] { s_SplitEntry }, StringSplitOptions.None);


		foreach(string current in codes)
		{
			string[] entries  = current.Split(new string[] { s_SplitValues }, StringSplitOptions.None);
			Debug.Log("ID : " + current  + " / " + "Number of values :" + entries.Length);
		}
	}

	public string[] returnArray(string playerPrefsName){
		string allCodes = PlayerPrefs.GetString(playerPrefsName);

		// this splits the string using comma as a parameter.
		// every index on the array will now have a code in it.
		// note that this method requires you to use a single ' instead of "
		string[] codes  = allCodes.Split(new string[] { s_SplitEntry }, StringSplitOptions.None);
		return codes;
	}

}
