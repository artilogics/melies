// Description : EditorMethods.cs : Display gameObject, methods and argument in a list. Only methods with 0 or 1 argument are allowed. Only public void is allowed.
#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;


public class EditorMethods {
// --> Add a method to the list
	public void AddMethodsToList(SerializedProperty methodsList){
		int index = 0;
		if (methodsList.arraySize > 0)
			index = methodsList.arraySize;

		methodsList.InsertArrayElementAtIndex (index);

		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("obj").objectReferenceValue = null;
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("scriptRef").objectReferenceValue = null;

		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("indexScript").intValue = 0;
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("indexMethod").intValue = 0;
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("methodInfoName").stringValue = "";
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("intValue").intValue = 0;
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("floatValue").floatValue = 0;
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("stringValue").stringValue = "";
		methodsList.GetArrayElementAtIndex(index).FindPropertyRelative("objValue").objectReferenceValue = null;

	}

// Remove a method from the list
	public void RemoveMethodsToList(int methodToRemove, SerializedProperty methodsList){
		methodsList.DeleteArrayElementAtIndex (methodToRemove);
	}


// Display gameObject, methods and argument. Only methods with 0 or 1 argument are allowed. Only public void is allowed.
	public void DisplayMethodsOnEditor(List<EditorMethodsList.MethodsList> myScript, SerializedProperty methodsList,GUIStyle style_01){
		for (var j = 0; j < methodsList.arraySize; j++) {

			EditorGUILayout.BeginVertical (style_01);
			EditorGUILayout.BeginHorizontal ();

			if (GUILayout.Button ("-", GUILayout.Width (20))) {																// Remove a method from the list
				RemoveMethodsToList (j,methodsList);
				break;
			}

			EditorGUILayout.LabelField (j + " :", GUILayout.Width (15));


			SerializedProperty objRef 				= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("obj");	// all the list property
			SerializedProperty scriptRefRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef");
			SerializedProperty indexScriptRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript");
			SerializedProperty indexMethodRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod");
			SerializedProperty methodInfoNameRef 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName");
			SerializedProperty _intValueref		 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("intValue");
			SerializedProperty floatValue		 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("floatValue");
			SerializedProperty stringValue		 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("stringValue");
			SerializedProperty objValue		 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("objValue");
			SerializedProperty audioValue		 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("audioValue");


			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.PropertyField(objRef, new GUIContent (""), GUILayout.Width (100));								// --> Display gameobject on the Inspector
				
			if (EditorGUI.EndChangeCheck ()) {
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef").objectReferenceValue = null;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName").stringValue = "";
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("intValue").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("floatValue").floatValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("stringValue").stringValue = "";
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("objValue").objectReferenceValue = null;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("audioValue").objectReferenceValue = null;
			}

		
			if (myScript [j].obj) {

				var comp3 = myScript [j].obj.GetComponents<MonoBehaviour> ();												// find all the MonoBehaviour in a gameObject											

				string[] options3 = new string[comp3.Length];											
			
				for (var i = 0; i < comp3.Length; i++) {
					options3 [i] = comp3 [i].ToString ();
				}

				if (options3.Length > 0) {
					EditorGUI.BeginChangeCheck ();
					indexScriptRef.intValue = EditorGUILayout.Popup (indexScriptRef.intValue, options3, GUILayout.Width (150));					// --> Display all scripts


					if (EditorGUI.EndChangeCheck ()) {
						methodsList.GetArrayElementAtIndex (methodsList.arraySize - 1).FindPropertyRelative ("indexMethod").intValue = 0;
					}




					var method2s = new List<MethodInfo> ();																	// find all the methods in the script
					var mbs2 = myScript [j].obj.GetComponents<MonoBehaviour> ();

					string[] ignoreMethods2 = new string[] { "Start", "Update" };											// Ignore these functions

					foreach (var mb in mbs2) {																				// Ingnore some other functions
						method2s.AddRange (mb.GetType ().GetMethods ()
							.Where (x => !ignoreMethods2.Any (n => n == x.Name))
							.Where (x => x.GetParameters ().Length < 2)
							.Where (x => x.DeclaringType == comp3 [indexScriptRef.intValue].GetType ()));

					}

					string[] options4 = new string[method2s.Count];

					EditorGUILayout.EndHorizontal ();

					if (options4.Length > 0) {

						//EditorGUILayout.BeginVertical ();
						for (var i = 0; i < method2s.Count; i++) {
							options4 [i] = method2s [i].Name;
						}
							
						EditorGUILayout.BeginHorizontal ();

						EditorGUILayout.LabelField ("", GUILayout.Width (140));	
						indexMethodRef.intValue = EditorGUILayout.Popup (indexMethodRef.intValue, options4, GUILayout.Width (120));				// --> Display all methods



						if (method2s [indexMethodRef.intValue].GetParameters ().Length > 0) {								// Display arguments
							ParameterInfo pInfo = method2s [indexMethodRef.intValue].GetParameters ().First ();

							if (CheckValueType (pInfo) == 1) {																// --> int value																					
								EditorGUILayout.PropertyField (_intValueref, new GUIContent (""), GUILayout.Width (100));
							}
							if (CheckValueType (pInfo) == 2) {																// --> GameObject value
								EditorGUILayout.PropertyField (objValue, new GUIContent (""), GUILayout.Width (120));
							}
							if (CheckValueType (pInfo) == 3) {																// --> string value
								EditorGUILayout.PropertyField (stringValue, new GUIContent (""), GUILayout.Width (100));
							}
							if (CheckValueType (pInfo) == 4) {																// --> float value
								EditorGUILayout.PropertyField (floatValue, new GUIContent (""), GUILayout.Width (100));
							}
							if (CheckValueType (pInfo) == 5) {																// --> audioClip value
								EditorGUILayout.PropertyField (audioValue, new GUIContent (""), GUILayout.Width (120));
							}

						}
						EditorGUILayout.EndHorizontal ();
						scriptRefRef.objectReferenceValue = (MonoBehaviour)comp3 [indexScriptRef.intValue];
						methodInfoNameRef.stringValue = method2s [indexMethodRef.intValue].Name;

						//EditorGUILayout.EndVertical ();
					} else {
						//EditorGUILayout.EndHorizontal ();
					}
				} else {
					EditorGUILayout.LabelField ("It is not possible to use this gameObject");								// exception
				}
			} else {
				EditorGUILayout.EndHorizontal ();
			}
				



			//EditorGUILayout.LabelField ("");	



			EditorGUILayout.EndVertical ();
		}
	}


    // Display gameObject, methods and argument. Only methods with 0 or 1 argument are allowed. Only public void is allowed.
    public void DisplaySelectedMethodsOnEditor(List<EditorMethodsList.MethodsList> myScript, SerializedProperty methodsList, GUIStyle style_01,int selectMethods)
    {
        for (var j = 0; j < methodsList.arraySize; j++)
        {
            if (j == selectMethods)
            {



              //  SerializedProperty objRef = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("obj");    // all the list property
                SerializedProperty scriptRefRef = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef");
                SerializedProperty indexScriptRef = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript");
                SerializedProperty indexMethodRef = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod");
                SerializedProperty methodInfoNameRef = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName");
                SerializedProperty _intValueref = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("intValue");
                SerializedProperty floatValue = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("floatValue");
                SerializedProperty stringValue = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("stringValue");
                SerializedProperty objValue = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("objValue");
                SerializedProperty audioValue = methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("audioValue");


                EditorGUI.BeginChangeCheck();

              //  EditorGUILayout.PropertyField(objRef, new GUIContent(""), GUILayout.Width(100));                              // --> Display gameobject on the Inspector

                if (EditorGUI.EndChangeCheck())
                {
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef").objectReferenceValue = null;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript").intValue = 0;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod").intValue = 0;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName").stringValue = "";
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("intValue").intValue = 0;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("floatValue").floatValue = 0;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("stringValue").stringValue = "";
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("objValue").objectReferenceValue = null;
                    methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("audioValue").objectReferenceValue = null;
                }


                if (myScript[j].obj)
                {
                    EditorGUILayout.BeginVertical();
                    var comp3 = myScript[j].obj.GetComponents<MonoBehaviour>();                                               // find all the MonoBehaviour in a gameObject                                           

                    string[] options3 = new string[comp3.Length];

                    for (var i = 0; i < comp3.Length; i++)
                    {
                        options3[i] = comp3[i].ToString();
                    }

                    if (options3.Length > 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        indexScriptRef.intValue = EditorGUILayout.Popup(indexScriptRef.intValue, options3);                 // --> Display all scripts


                        if (EditorGUI.EndChangeCheck())
                        {
                            methodsList.GetArrayElementAtIndex(methodsList.arraySize - 1).FindPropertyRelative("indexMethod").intValue = 0;
                        }




                        var method2s = new List<MethodInfo>();                                                                 // find all the methods in the script
                        var mbs2 = myScript[j].obj.GetComponents<MonoBehaviour>();

                        string[] ignoreMethods2 = new string[] { "Start", "Update" };                                           // Ignore these functions

                        foreach (var mb in mbs2)
                        {                                                                              // Ingnore some other functions
                            method2s.AddRange(mb.GetType().GetMethods()
                                .Where(x => !ignoreMethods2.Any(n => n == x.Name))
                                .Where(x => x.GetParameters().Length < 2)
                                .Where(x => x.DeclaringType == comp3[indexScriptRef.intValue].GetType()));

                        }

                        string[] options4 = new string[method2s.Count];

                      //  EditorGUILayout.EndHorizontal();

                        if (options4.Length > 0)
                        {

                            //EditorGUILayout.BeginVertical ();
                            for (var i = 0; i < method2s.Count; i++)
                            {
                                options4[i] = method2s[i].Name;
                            }

                           // EditorGUILayout.BeginHorizontal();

                           // EditorGUILayout.LabelField("", GUILayout.Width(140));
                            indexMethodRef.intValue = EditorGUILayout.Popup(indexMethodRef.intValue, options4);             // --> Display all methods



                            if (method2s[indexMethodRef.intValue].GetParameters().Length > 0)
                            {                               // Display arguments
                                ParameterInfo pInfo = method2s[indexMethodRef.intValue].GetParameters().First();

                                if (CheckValueType(pInfo) == 1)
                                {                                                              // --> int value                                                                                    
                                    EditorGUILayout.PropertyField(_intValueref, new GUIContent(""), GUILayout.Width(100));
                                }
                                if (CheckValueType(pInfo) == 2)
                                {                                                              // --> GameObject value
                                    EditorGUILayout.PropertyField(objValue, new GUIContent(""), GUILayout.Width(120));
                                }
                                if (CheckValueType(pInfo) == 3)
                                {                                                              // --> string value
                                    EditorGUILayout.PropertyField(stringValue, new GUIContent(""), GUILayout.Width(100));
                                }
                                if (CheckValueType(pInfo) == 4)
                                {                                                              // --> float value
                                    EditorGUILayout.PropertyField(floatValue, new GUIContent(""), GUILayout.Width(100));
                                }
                                if (CheckValueType(pInfo) == 5)
                                {                                                              // --> audioClip value
                                    EditorGUILayout.PropertyField(audioValue, new GUIContent(""), GUILayout.Width(120));
                                }

                            }
                           // EditorGUILayout.EndHorizontal();
                            scriptRefRef.objectReferenceValue = (MonoBehaviour)comp3[indexScriptRef.intValue];
                            methodInfoNameRef.stringValue = method2s[indexMethodRef.intValue].Name;

                            //EditorGUILayout.EndVertical ();
                        }
                        else
                        {
                            //EditorGUILayout.EndHorizontal ();
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("It is not possible to use this gameObject");                               // exception
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                   // EditorGUILayout.EndHorizontal();
                }




                //EditorGUILayout.LabelField ("");  



              // EditorGUILayout.EndVertical();
            }
        }
    }

	// Display gameObject, methods and argument. Only methods with 0 or 1 argument are allowed. Only public void is allowed.
	public void DisplayMethodsOnEditorSaveSection(List<EditorMethodsList.MethodsList> myScript, SerializedProperty methodsList,GUIStyle style_01){
		for (var j = 0; j < methodsList.arraySize; j++) {

			EditorGUILayout.BeginVertical (style_01);
			EditorGUILayout.HelpBox ("This script is used to record information about this gameObject. This information will be used in the Save/Load system." +
				"\n\nSelect a script from the list that contains methods :" +
				"\n-ReturnSaveData ()" +
				"\n-saveSystemInitGameObject(string s_ObjectDatas)", MessageType.Info); 


			EditorGUILayout.BeginHorizontal ();



			SerializedProperty objRef 				= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("obj");	// all the list property
			SerializedProperty scriptRefRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef");
			SerializedProperty indexScriptRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript");
			SerializedProperty indexMethodRef 		= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod");
			SerializedProperty methodInfoNameRef 	= methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName");
			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.LabelField (objRef.objectReferenceValue.name + " : " , GUILayout.Width (100));
			//EditorGUILayout.PropertyField(objRef, new GUIContent (""), GUILayout.Width (100));								// --> Display gameobject on the Inspector

			if (EditorGUI.EndChangeCheck ()) {
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("scriptRef").objectReferenceValue = null;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexScript").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("indexMethod").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("methodInfoName").stringValue = "";
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("intValue").intValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("floatValue").floatValue = 0;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("stringValue").stringValue = "";
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("objValue").objectReferenceValue = null;
				methodsList.GetArrayElementAtIndex(j).FindPropertyRelative("audioValue").objectReferenceValue = null;
			}


			if (myScript [j].obj) {

				var comp3 = myScript [j].obj.GetComponents<MonoBehaviour> ();												// find all the MonoBehaviour in a gameObject											

				string[] options3 = new string[comp3.Length+1];											


				for (var i = 0; i < comp3.Length; i++) {
					options3 [i] = comp3 [i].ToString ();
				}
				options3 [comp3.Length] = "Select";

				/*if (first) {
					indexScriptRef.intValue = comp3.Length;
				}*/

				if (options3.Length > 0) {
					EditorGUI.BeginChangeCheck ();
					indexScriptRef.intValue = EditorGUILayout.Popup (indexScriptRef.intValue, options3, GUILayout.Width (150));					// --> Display all scripts



					if (EditorGUI.EndChangeCheck ()) {
						methodsList.GetArrayElementAtIndex (methodsList.arraySize - 1).FindPropertyRelative ("indexMethod").intValue = 0;
						methodsList.GetArrayElementAtIndex(methodsList.arraySize - 1).FindPropertyRelative("scriptRef").objectReferenceValue = null;
					}



					var method2s = new List<MethodInfo> ();																	// find all the methods in the script
					var mbs2 = myScript [j].obj.GetComponents<MonoBehaviour> ();

					string[] ignoreMethods2 = new string[] { "Start", "Update" };											// Ignore these functions


					foreach (var mb in mbs2) {																				// Ingnore some other functions
						if (indexScriptRef.intValue < options3.Length-1) {
							method2s.AddRange (mb.GetType ().GetMethods ()
							.Where (x => !ignoreMethods2.Any (n => n == x.Name))
							.Where (x => x.GetParameters ().Length < 2)
							.Where (x => x.DeclaringType == comp3 [indexScriptRef.intValue].GetType ()));
						}
					}

					string[] options4 = new string[method2s.Count];

					EditorGUILayout.EndHorizontal ();

						if (options4.Length > 0) {

							//EditorGUILayout.BeginVertical ();
							for (var i = 0; i < method2s.Count; i++) {
								options4 [i] = method2s [i].Name;
							}

					



							scriptRefRef.objectReferenceValue = (MonoBehaviour)comp3 [indexScriptRef.intValue];
							methodInfoNameRef.stringValue = method2s [indexMethodRef.intValue].Name;

						//EditorGUILayout.EndVertical ();
					} else {
						//EditorGUILayout.EndHorizontal ();
					}
				} else {
					EditorGUILayout.LabelField ("It is not possible to use this gameObject");								// exception
				}
			} else {
				EditorGUILayout.EndHorizontal ();
			}




			//EditorGUILayout.LabelField ("");	



			EditorGUILayout.EndVertical ();
		
		}
	}

	// Display gameObject, methods and argument. Only methods with 0 or 1 argument are allowed. Only public void is allowed.
	public int find_iSObjectActivatedScript(List<EditorMethodsList.MethodsList> myScript, SerializedProperty methodsList,GUIStyle style_01){
		for (var j = 0; j < methodsList.arraySize; j++) {
				if (myScript [j].obj) {
					var comp3 = myScript [j].obj.GetComponents<MonoBehaviour> ();												// find all the MonoBehaviour in a gameObject											

					for (var i = 0; i < comp3.Length; i++) {
						if (comp3 [i].ToString ().Contains ("isObjectActivated")) {
						return i;
					}
				}
			}
		}
		return -1;
	}


// --> use to know the type of the argument in a method
	public int CheckValueType(ParameterInfo param){
		if(param.ParameterType == typeof(Int32))
			return 1;
		if(param.ParameterType == typeof(GameObject))
			return 2;
		if (param.ParameterType == typeof(String)) 
			return 3;
		if (param.ParameterType == typeof(float)) 
			return 4;
		if (param.ParameterType == typeof(AudioClip)) 
			return 5;
		return 0;
	}




}
#endif
