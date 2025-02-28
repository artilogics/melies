// Decription : TextProperties : Choose and update the Text ID. Update the text in game too 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextProperties : MonoBehaviour {
	public bool			SeeInspector = false;
	public int 			managerID 	= 0;
	public int 			uniqueID 	= 0;
	public TextList		textList;
	public int 			editorType = 0; // 0 Diary, 1 Inventory, 2 Info, 3 Feedback

	public int t_Language = 0;
	public int t_ID = 0;

	private Text		txt;

	public bool 		language_AutoUpdate = true;
	public bool 		b_UIButtonShowTitle = false;

    public bool         b_UpdateID = false;


	public void Start(){
		if(language_AutoUpdate)
			updateInfoText ();
	}


	public void updateInfoText(){
		if (language_AutoUpdate) {
			if (txt == null)
				txt = gameObject.GetComponent<Text> ();

            if (txt != null){
               //Debug.Log("Update : " + ingameGlobalManager.instance.currentLanguage);
               txt.text = r_TextList().r_Title(ingameGlobalManager.instance.currentLanguage, managerID);
            }
		}
	}

	public string returnInfoText(){
		if (txt == null)
			txt = gameObject.GetComponent<Text> ();

		return r_TextList().r_Title (ingameGlobalManager.instance.currentLanguage, managerID);
	}


	public TextList r_TextList(){
		if (editorType == 0)
			return ingameGlobalManager.instance.currentDiary;
		else if (editorType == 1)
			return ingameGlobalManager.instance.currentInventory;
		else if (editorType == 2)
			return ingameGlobalManager.instance.currentInfo;
		else if (editorType == 3)
			return ingameGlobalManager.instance.currentFeedback;
		else
			return null;
		
	}

}
