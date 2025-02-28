// Description : EditorMethodsList.cs. Class to define a method to load
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorMethodsList : MonoBehaviour {

	[System.Serializable]
	public class MethodsList{
		public GameObject 		obj;					// gameobject where you find function to call
		public MonoBehaviour	scriptRef;				// MonoBehaviour call
		public int 				indexScript = 0;		// which script to use	
		public int				indexMethod = 0;		// which method to call
		public string 			methodInfoName;			// the name of the method
		public int 				intValue;				// integer argument
		public float 			floatValue;				// float argument
		public string 			stringValue;			// string argument
		public GameObject 		objValue;				// gameObject argument
		public AudioClip 		audioValue;				// audioClip argument
	}
}
