// Description : isObjectActivated : Use to save if an object is activated or not in the save system.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isObjectActivated : MonoBehaviour {

	public bool SaveVoiceOverProperties = true;


    [Header("Choose the state of this object when the scene starts and save data doesn't exist.")]
    public bool  firstTimeEnabledObject = false;

    [HideInInspector]
    public List<EditorMethodsList.MethodsList> methodsList      // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList.MethodsList>();

    [HideInInspector]
    public List<EditorMethodsList.MethodsList> methodsListObjDeactivated      // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList.MethodsList>();
    
    [HideInInspector]
    public List<EditorMethodsList.MethodsList> methodsListSaveExtend      // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList.MethodsList>();

    [HideInInspector]
    public List<EditorMethodsList.MethodsList> methodsListSaveExtendLoadProcess      // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList.MethodsList>();

    [HideInInspector]
    public CallMethods callMethods;

    //private bool b_ProcessDone = false;

//--> Use to load object state
	public void saveSystemInitGameObject(string s_ObjectDatas){
		string[] codes = s_ObjectDatas.Split ('_');
		// Load Parameters 
        if(s_ObjectDatas != ""){                                  // Save Exist
            //Debug.Log("Here : " + s_ObjectDatas);
            for (var i = 0; i < codes.Length; i++)
            {
                if (codes[0] == "T")
                {
                    if (gameObject.GetComponent<Renderer>())
                    {         // Items
                       // InitRenderer(gameObject, true);
                        StartCoroutine(CallAllTheMethodsOneByOne(methodsList,0,true));
                    }
                    else
                    {                                                   // Obj activate
                   //     gameObject.SetActive(true);
                        StartCoroutine(CallAllTheMethodsOneByOne(methodsList,1,true));
                    }
                }
                else if (codes[0] == "F")
                {
                    if (gameObject.GetComponent<Renderer>())
                    {          // Items
                        StartCoroutine(CallAllTheMethodsOneByOne(methodsListObjDeactivated,0,false));
                   //     InitRenderer(gameObject, false);

                    }
                    else
                    {                                               // Obj activate
                        StartCoroutine(CallAllTheMethodsOneByOne(methodsListObjDeactivated,1,false));
                    //    gameObject.SetActive(false);
                       
                    }
                }
            } 

           
        }
        else{                                                       // Save doesn't exist   (only for Obj Activate)
             if (!gameObject.GetComponent<Renderer>()){
                if (firstTimeEnabledObject)
                {
                   // gameObject.SetActive(true);                          // Obj activate
                    StartCoroutine(CallAllTheMethodsOneByOne(methodsList,1,true));
                }
                else
                {
                    StartCoroutine(CallAllTheMethodsOneByOne(methodsListObjDeactivated,1,false));
                   // gameObject.SetActive(false);                          // Obj activate

                }
            }
            else{
                if (firstTimeEnabledObject)
                {
                   // InitRenderer(gameObject, true);
                    StartCoroutine(CallAllTheMethodsOneByOne(methodsList,0,true));
                }
                else
                {
                    StartCoroutine(CallAllTheMethodsOneByOne(methodsListObjDeactivated,0,false));
                    //InitRenderer(gameObject, false);
                }
            }
            //Debug.Log(gameObject.name);
        }

        callMethods.Call_A_Method_WithSpecificStringArgument(methodsListSaveExtendLoadProcess, s_ObjectDatas);       // Extend Save Procees
		

		for (var i = 0; i < ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem.Count; i++) {
			if (ingameGlobalManager.instance._levelManager.listOfGameObjectForSaveSystem [i] == gameObject) {
				ingameGlobalManager.instance._levelManager.listState [i] = true;
				break;
			}
		}


		// Update VoiceProperties if needed
        if (codes.Length > 1 && SaveVoiceOverProperties && gameObject.GetComponent<VoiceProperties> ()) {
			if (codes [1] == "T") {
				gameObject.GetComponent<VoiceProperties> ().b_alreadyPlayed = true;
			} else if (codes [1] == "F") {
				gameObject.GetComponent<VoiceProperties> ().b_alreadyPlayed = false;
			}
		}
	}

//--> Enable or disable gameObject renderer
	private void InitRenderer(GameObject obj, bool b_Enabled){
		obj.GetComponent<Renderer> ().enabled = b_Enabled;
		Transform[] Children = obj.GetComponentsInChildren<Transform>(true);
		for (var j = 0; j < Children.Length; j++) {
			if(Children[j].GetComponent<Renderer>()){
				Children[j].GetComponent<Renderer>().enabled = b_Enabled;
			}
            else{
                Children[j].gameObject.SetActive(b_Enabled);
            }
		}
	}


//--> Use to save Object state
	public string ReturnSaveData () {
		string value = r_TrueFalse(gameObject.activeSelf);

		if(gameObject.GetComponent<Renderer>())
			value = r_TrueFalse(gameObject.GetComponent<Renderer>().enabled);

		if (SaveVoiceOverProperties && gameObject.GetComponent<VoiceProperties> ()) {
			value += "_" + r_TrueFalseVoiceAlreadyPlayed(gameObject.GetComponent<VoiceProperties> ().b_alreadyPlayed);
		}

		//Debug.Log ("valuevalue : " + value);

        for (var i = 0; i < methodsListSaveExtend.Count; i++)
        {
            value += "_" + callMethods.Call_A_Method_Only_String(methodsListSaveExtend, "");
        }



		return value;
	}

	private string r_TrueFalse(bool s_Ref){
		if (s_Ref)return "T";
		else return "F";
	}

	private string r_TrueFalseVoiceAlreadyPlayed(bool s_Ref){
		if (s_Ref)return "T";
		else return "F";
	}

    public bool Bool_returnIfObjectIsActivated(){
        return gameObject.activeInHierarchy;
    }

    public void ActivateObject(){
        gameObject.SetActive(true);
    }
    public void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    public bool B_AP_ActivateObject()
    {
        gameObject.SetActive(true);
        return true;
    }
    public bool B_AP_DeactivateObject()
    {
        gameObject.SetActive(false);
        return true;
    }

    public void ActivateRenderer()
    {
        InitRenderer(gameObject, true);
    }
    public void DeactivateRenderer()
    {
        InitRenderer(gameObject, false);
    }

    public bool SwitchObjectEnabledState()
    {
        gameObject.SetActive(!Bool_returnIfObjectIsActivated());
        return true;
    }

    public IEnumerator CallAllTheMethodsOneByOne(List<EditorMethodsList.MethodsList> listOfMethods, int value, bool TorF)
    {
        
        for (var i = 0; i < listOfMethods.Count; i++)
        {
            yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(listOfMethods, i) == true);
        }


        yield return new WaitForEndOfFrame();

        if(value == 0){ // InitRenderer
            InitRenderer(gameObject, TorF);
        }
        else if(value == 1){// Set active
            gameObject.SetActive(TorF);
        } 

        yield return null;
    }


    public string info()
    {

        return "Rotation";
    }


    public void  UseInfo(string s_ObjectDatas)
    {
        string[] codes = s_ObjectDatas.Split('_');
        // Load Parameters 
        if (s_ObjectDatas != "")
        {                                  // Save Exist
            //Debug.Log("Here : " + s_ObjectDatas);
            for (var i = 0; i < codes.Length; i++)
            {
                if (i == 0)
                {
                    Debug.Log("codes[0]: " + "The object is enable or disable in the scene");

                }

                if (i == 1)
                {
                    Debug.Log("Rotation: " + codes[i]);
                }
            }
        }   
    }
}
