// Description : CallMethods.cs : use to call function
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

[System.Serializable]
public class CallMethods {

// --> Call a list of methods 
	public void Call_A_Method (List<EditorMethodsList.MethodsList> methodsList){
		for (var i = 0; i < methodsList.Count; i++) {

			if (methodsList [i].obj != null && methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).GetParameters ().Length > 0) {

				ParameterInfo pInfo = methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).GetParameters ().First ();


				if (CheckValueType (pInfo) == 1) {																							// --> int value																					
					methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ methodsList [i].intValue });		// call method with an integer argument
				}

				if (CheckValueType (pInfo) == 2) {																							// --> GameObject value
					methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ methodsList [i].objValue });		// call method with an GameObject argument
				}
				if (CheckValueType (pInfo) == 3) {																							// --> string value
					methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ methodsList [i].stringValue });	// call method with an string argument
				}
				if (CheckValueType (pInfo) == 4) {																							// --> float value
					methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ methodsList [i].floatValue });	// call method with an float argument
				}
				if (CheckValueType (pInfo) == 5) {																							// --> float value
					methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ methodsList [i].audioValue });	// call method with an AudioCLip argument
				}

			} 
			else if (methodsList [i].obj != null) 
			{
				methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
					methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ });
			}
		}
	}

    // --> Call a list of methods 
    public void Call_A_Specific_Method(List<EditorMethodsList.MethodsList> methodsList,int value)
    {
        for (var i = 0; i < methodsList.Count; i++)
        {
            if (i == value)
            {
                if (methodsList[i].obj != null && methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().Length > 0)
                {

                    ParameterInfo pInfo = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().First();


                    if (CheckValueType(pInfo) == 1)
                    {                                                                                           // --> int value                                                                                    
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].intValue });     // call method with an integer argument
                    }

                    if (CheckValueType(pInfo) == 2)
                    {                                                                                           // --> GameObject value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].objValue });     // call method with an GameObject argument
                    }
                    if (CheckValueType(pInfo) == 3)
                    {                                                                                           // --> string value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].stringValue });  // call method with an string argument
                    }
                    if (CheckValueType(pInfo) == 4)
                    {                                                                                           // --> float value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].floatValue });   // call method with an float argument
                    }
                    if (CheckValueType(pInfo) == 5)
                    {                                                                                           // --> float value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].audioValue });   // call method with an AudioCLip argument
                    }

                }
                else if (methodsList[i].obj != null)
                {
                    methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                        methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                }
            }
        }
    }

    // --> Call a list of methods only boolean
    public bool Call_One_Bool_Method(List<EditorMethodsList.MethodsList> methodsList,int value)
    {
        bool result = true;
        //for (var i = 0; i < methodsList.Count; i++)
        //{
        if (methodsList[value].obj != null)
            {
            Type info = methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).ReturnType;

                if (info == typeof(System.Boolean))
                {
                result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                    methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { });
                }
                else{
                    Debug.Log(methodsList[value].methodInfoName + " is not a boolean Method");
                }
            }
            //if (result == false) break;
        //}
        return result;
    } 

// --> Call a list of methods only boolean
	public bool Call_A_Method_Only_Boolean (List<EditorMethodsList.MethodsList> methodsList){
		bool result = true;
		for (var i = 0; i < methodsList.Count; i++) {
			if (methodsList [i].obj != null) {
				Type info = methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).ReturnType;

				if(info == typeof(System.Boolean)){
					result = (bool)methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ });	
				}
			}
            if (result == false) break;
		}
		return result;
	}

// --> Call a list of methods only string
	public string Call_A_Method_Only_String (List<EditorMethodsList.MethodsList> methodsList,string defaultResult){
		string result = defaultResult;
		for (var i = 0; i < methodsList.Count; i++) {
			if (methodsList [i].obj != null && methodsList [i].scriptRef != null) {
				Type info = methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).ReturnType;

				if(info == typeof(System.String)){
					result = (string)methodsList [i].scriptRef.GetType ().GetMethod (methodsList [i].methodInfoName).Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ });	
				}
			}
		}
		return result;
	}

// --> Call a method named ReturnSaveData when game is saving objects data
	public string Call_A_Method_Only_String_SaveData (List<EditorMethodsList.MethodsList> methodsList,string defaultResult){
		string result = defaultResult;
		for (var i = 0; i < methodsList.Count; i++) {
			if (methodsList [i].obj != null && methodsList [i].scriptRef != null) {
				//Debug.Log(methodsList [i].obj.name);
				Type info = methodsList [i].scriptRef.GetType ().GetMethod ("ReturnSaveData").ReturnType;

				if(info == typeof(System.String)){
					result = (string)methodsList [i].scriptRef.GetType ().GetMethod ("ReturnSaveData").Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ });	
				}
			}
		}
		return result;
	}

	// --> Call a method named F_ObjectLoadData when game is saving objects data
	public void Call_A_Method_ObjectLoadData (List<EditorMethodsList.MethodsList> methodsList,string s_Value){
		for (var i = 0; i < methodsList.Count; i++) {

			if (methodsList [i].obj != null) {

				Type info = methodsList [i].scriptRef.GetType ().GetMethod ("saveSystemInitGameObject").ReturnType;
																								
				methodsList [i].scriptRef.GetType ().GetMethod ("saveSystemInitGameObject").Invoke (
						methodsList [i].obj.GetComponent (methodsList [i].scriptRef.GetType ()), new object[]{ s_Value });	// call method with an string argument
                
			} 
		}
	}

    // --> Call a method named F_ResetPuzzle when game is saving objects data
    public void Call_A_Method_F_ResetPuzzle(List<EditorMethodsList.MethodsList> methodsList, string s_Value)
    {
        for (var i = 0; i < methodsList.Count; i++)
        {

            if (methodsList[i].obj != null)
            {

                Type info = methodsList[i].scriptRef.GetType().GetMethod("F_ResetPuzzle").ReturnType;

                methodsList[i].scriptRef.GetType().GetMethod("F_ResetPuzzle").Invoke(
                        methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] {}); // call method with an string argument

            }
        }
    }

    // --> Call a method named F_ResetPuzzle when game is saving objects data
    public void Call_A_Method_WithSpecificStringArgument(List<EditorMethodsList.MethodsList> methodsList, string s_Value)
    {
        for (var i = 0; i < methodsList.Count; i++)
        {

            if (methodsList[i].obj != null)
            {

                Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                    methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { s_Value}); // call method with an string argument

            }
        }
    }


// --> Check the argument type of a method
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
