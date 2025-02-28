// Description : TextList : Use to save game datas 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextList : ScriptableObject 
{
	[System.Serializable]
	public class apSubtitle
	{
		public List<bool> 		showMore;
		public List<float> 		startPointsClip;
		public List<float> 		firstLetter;
		public List<int> 		lastLetter;
		public List<bool> 		bypasstextLayout;
		public List<string> 	textSub;
	}

	[System.Serializable]
	public class languageSlot
	{
		public List<string> 	diaryTitle;
		public List<bool> 		diaryTextDisplayState;
		public List<string> 	diaryText;
		public List<AudioClip> 	diaryAudioClip;
		public List<apSubtitle> diarySub;
		public List<Sprite> 	diarySprite;

		public GameObject 		refGameObject = null;
		public bool 			showInInventory = false;
		public float		 	prefabSizeInViewer = 1;
		public Vector3		 	prefabRotationInViewer = new Vector3(1,1,1);
		public int				uniqueItemID = 0;
		public int 				itemType = 0;
		public int 				audioPriority = 0;

	}



	[System.Serializable]
	public class Diary
	{
		public List<languageSlot> 	_languageSlot;
	}

	[SerializeField]
	public List<Diary> 			diaryList;


	public bool 				helpBoxEditor 				= true;
	public int 					idEditorSize				= 20;
	public int 					titleEditorSize 			= 240;
	public int 					textEditorSize 				= 300;
	public bool					showMoreOptions 			= true;
	public bool					showMoveUpDown 				= true;
	public bool					showNameOptions 			= false;
	public List<string> 		listOfLanguage 				= new List<string>();
	public int 					currentLanguage 			= 0;
	public int 					fisrtTextDisplayedInEditor 	= 0;
	public int 					howManyEntryDisplayed 		= 20;
	public int 					selectedID 					= 0;
	public bool 				b_EditSubtitle 				= false;
	public int 					currentAudioSubtitleEdit 	= 0;

	public int 					editorType = 0; // 0 Diary, 1 Inventory, 2 Info, 3 Feedback
	public bool 				showDefaultLanguage = false;
	public bool 				multipleVoiceOver = false;		// activate Multi language voic over

    public bool                 b_ShowEye = false;

    public bool                 b_UI_DisplayTextArea = false;   // Allows to display a text area in the w_UI Editor


// --> return if a selected ID will be available in diary or inventory
	public bool r_Available(int language,int ID){
		return diaryList [language]._languageSlot [ID].showInInventory;
	}

// --> return the Title of a selected ID 
	public string r_Title(int language,int ID){
		if (diaryList.Count >= language) {
			return diaryList [language]._languageSlot [ID].diaryTitle[0];
		}
		else
			return "Text doesn't exist in this language";
	}

// --> return the page of a selected ID 
	public string r_Page(int language,int ID,int pageNumber){
		return diaryList [language]._languageSlot [ID].diaryText[pageNumber];
	}

// --> return the Sprite of a selected ID 
	public Sprite r_Sprite(int language,int ID){
		return diaryList [language]._languageSlot [ID].diarySprite[0];
	}


// --> return the audio of a selected ID 
	public AudioClip r_Audio(int language,int ID,int audioNumber){
		return diaryList [language]._languageSlot [ID].diaryAudioClip[audioNumber];
	}

// --> return the audio priority of a selected ID 
	public int r_audioPriority(int language,int ID){
		return diaryList [language]._languageSlot [ID].audioPriority;
	}


//--> Know the number of Page + the number of subtitles for each page for a specific ID
	public List<int> voiceOverDescription(int language,int ID){
		List<int> tmpList = new List<int>();

		// -->How many : Page
		tmpList.Add(diaryList [language]._languageSlot [ID].diaryText.Count);

		//--> Know the number of subtitles for each page 
		for(var i = 0;i<diaryList [language]._languageSlot [ID].diarySub.Count;i++){
			tmpList.Add (diaryList [language]._languageSlot [ID].diarySub [i].startPointsClip.Count);
		}

		return tmpList;
	}
}
