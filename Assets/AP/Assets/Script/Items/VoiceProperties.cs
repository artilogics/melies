// Description : VoiceProperties : Use to choose and update voice over ID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceProperties : MonoBehaviour {
	public bool			SeeInspector = false;
	public int 			managerID 	= 0;
	public int 			uniqueID 	= 0;
	public TextList		textList;
	public int 			editorType = 0; // 0 Diary, 1 Inventory, 2 Info, 3 Feedback

	public int t_Language = 0;
	public int t_ID = 0;


	private Text		txt;

	public bool 		language_AutoUpdate = true;

	public bool 		b_PlayOnce = true;
	public bool 		b_alreadyPlayed = false;

	public void Start(){
		//if(language_AutoUpdate)
		//	updateInfoText ();
	}


	public void updateInfoText(){
			if (txt == null)
				txt = gameObject.GetComponent<Text> ();

			if (txt != null)
				txt.text = r_TextList ().r_Title (ingameGlobalManager.instance.currentLanguage, managerID);
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
