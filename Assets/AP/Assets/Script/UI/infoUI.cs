// Description : infoUI : This script is used to display a UI box that contains infos on screen 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class infoUI : MonoBehaviour {
	public bool 				SeeInspector = false;

	public AnimationCurve 		animationCurve;				// use when info Ui move down
	public float 				t = 0;

	private float 				movementSpeed = 6; 			// info Ui movement speed
	public float 				stayDuration = 5;			// how long the info UI stay on screen

	public Image 				infoSprite;					// Image where it is possible to display a sprite
	public Sprite 				diarySprite;				// ref sprite for diary info
	public Sprite 				inventorySprite;			// ref sprite for inventory info
	public Sprite 				feedbackSprite;				// ref sprite for feedback info

	public RectTransform 		refPrefabInfo;				// ref prefab info Ui that is instantiate

	public List<RectTransform> 	listUi_Info;				// Lists to mangage info Ui on screen
	public List<GameObject> 	listRefGameObject;			// Lists of gameobject who call the UI Info
	public List<float> 			listPosY;
	public List<float> 			listTimer;
	public List<bool> 			listBack;

	public List<Sprite> 		listSprite;
	public List<string> 		listTitle;

	public int 					creationCounter = 0;		// The number of info ui
	public float 				timer = 0;					// time between to ui info creation
	public bool 				b_inCreation = false;		// use to know if is possible to create a new info Ui on screen

	public string 				txt_Inventory = "New Item in the inventory :";
	public string 				txt_Diary = "New document in the diary :";
	public int					inventoryID = 0;
	public int					diaryID = 0;

	public 	AudioClip			s_Inventory;
	public float 				s_InventoryVolume = 1f;
	public 	AudioClip 			s_Diary;
	public float 				s_DiaryVolume = 1f;
	public 	AudioClip 			s_feedback;
	public float 				s_feedbackVolume = 1f;
	private AudioSource 		_audio;



	void Start(){
		_audio = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!ingameGlobalManager.instance.b_Ingame_Pause) {
		//--> Test Creation Info UI
		/*	if (Input.GetKeyDown ("v")) {
				playAnimInfo ("Title",diarySprite);	
			}
		*/		
		//--> instantiate New UI Info
			if (!b_inCreation && creationCounter > 0) {
				instantiateNewUIInfo ();
			} 

		//--> Check if creation is allowed
			if (b_inCreation) {
				timer = Mathf.MoveTowards (timer, .5f, Time.deltaTime);
				if (timer == .5f)b_inCreation = false;
			}

            if (listUi_Info.Count > 0 &&
                !listUi_Info[0].gameObject.activeSelf)
            {
                foreach (RectTransform obj in listUi_Info)
                {
                    obj.gameObject.SetActive(true);
                }
            }



		//--> Move needed info UI
			manage_UI_Info_Movement ();
		}
        else if(ingameGlobalManager.instance.b_Ingame_Pause &&
                listUi_Info.Count > 0 &&
                listUi_Info[0].gameObject.activeSelf){
            foreach(RectTransform obj in listUi_Info){
                obj.gameObject.SetActive(false);
            } 

        }
	
	}

//--> call this function to create a new Info UI
	public void playAnimInfo(string Title,Sprite newSprite){
		listSprite.Add (newSprite);
		listTitle.Add (Title);
		creationCounter++;
	}


//--> call this function to create a new Info UI
	public void playAnimInfo(string Title,string newSpriteType){
        //Debug.Log(newSpriteType);


		if (newSpriteType == "Diary") {
			listSprite.Add (diarySprite);
            if (ingameGlobalManager.instance.currentInfo
               && inventoryID < ingameGlobalManager.instance.currentInfo.diaryList[ingameGlobalManager.instance.currentLanguage]._languageSlot.Count)
            {
                listTitle.Add(ingameGlobalManager.instance.currentInfo.diaryList[ingameGlobalManager.instance.currentLanguage]._languageSlot[diaryID].diaryTitle[0] + "\n" + Title);
            }
		} else if (newSpriteType == "Inventory") {
			listSprite.Add (inventorySprite);
			if (ingameGlobalManager.instance.currentInfo
			   && inventoryID < ingameGlobalManager.instance.currentInfo.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot.Count) {
				listTitle.Add (ingameGlobalManager.instance.currentInfo.diaryList [ingameGlobalManager.instance.currentLanguage]._languageSlot [inventoryID].diaryTitle [0] + "\n" + Title);
			} else {
				listTitle.Add (Title);
			}
		}
		else if (newSpriteType == "Feedback") {
			listSprite.Add (feedbackSprite);
			listTitle.Add (Title);
		}
		else
			listSprite.Add (null);

		listRefGameObject.Add (null);

		creationCounter++;
	}

//--> call this function to create a new Info UI
	public void playAnimInfo(string Title,string newSpriteType,GameObject refGameObject){
		if (newSpriteType == "Diary") {
			listSprite.Add (diarySprite);
			listTitle.Add (txt_Diary + "\n" +  Title);
		} else if (newSpriteType == "Inventory") {
			listSprite.Add (inventorySprite);
			listTitle.Add (txt_Inventory + "\n" +  Title);
		}
		else if (newSpriteType == "Feedback") {
			listSprite.Add (feedbackSprite);
			listTitle.Add (Title);
		}
		else
			listSprite.Add (null);

		listRefGameObject.Add (refGameObject);

		creationCounter++;
	}

//--> instantiate New UI Info
	public void instantiateNewUIInfo(){
		t = 0;
		RectTransform newRect = Instantiate (refPrefabInfo, gameObject.transform);
		newRect.name = "UI_Infos";

		for (var i = 0; i < listUi_Info.Count; i++) {
			listPosY [i] += 1.2f;
		}

		Image[] newImage = newRect.gameObject.GetComponentsInChildren<Image> ();

	//-> Choose sound to play
		foreach (Image im in newImage) {

            if(im.name == "Logo"){
                if (im.gameObject != newRect.gameObject)
                    im.sprite = listSprite[0];
                if (listSprite[0] == diarySprite)
                {                   //-> Diary Case
                    if (s_Diary)
                    {
                        _audio.volume = s_DiaryVolume;
                        _audio.clip = s_Diary;
                        _audio.Play();
                    }
                }
                else if (listSprite[0] == inventorySprite)
                {       //-> inventor case
                    if (s_Inventory)
                    {
                        _audio.volume = s_InventoryVolume;
                        _audio.clip = s_Inventory;
                        _audio.Play();
                    }
                }
                else if (listSprite[0] == feedbackSprite)
                {           //-> feedback case
                    if (s_feedback)
                    {
                        _audio.volume = s_feedbackVolume;
                        _audio.clip = s_feedback;
                        _audio.Play();
                    }
                } 
            }
			
		}
		listSprite.RemoveAt (0);

		Text[] newText = newRect.gameObject.GetComponentsInChildren<Text> ();

		foreach (Text txt in newText) {
			if(txt.gameObject != newRect.gameObject)
			txt.text = listTitle [0];
		}
		listTitle.RemoveAt (0);


		listUi_Info.Add (newRect);
		listPosY.Add (1.2f);
		listTimer.Add (0);
		listBack.Add (false);


		creationCounter--;
		timer = 0;
		b_inCreation = true;
	}

//--> Display Info sprite		
	private void displaySprite(int value){
		int ran = UnityEngine.Random.Range (0, 2);

		value = ran;

		if (infoSprite) {
			if(value == 0)infoSprite.sprite = diarySprite;
			if(value == 1)infoSprite.sprite = inventorySprite;
		}
	}

//--> Move needed info UI
	private void manage_UI_Info_Movement(){
		for (var i = 0; i < listUi_Info.Count; i++) {
//--> Step 1 : UI Info Move down 
			if (!listBack [i]) {
				t = Mathf.MoveTowards (t, 1, Time.deltaTime);
				listUi_Info [i].pivot = new Vector3 (
					0.5f,
					Mathf.MoveTowards (listUi_Info [i].pivot.y, listPosY [i], animationCurve.Evaluate(t) * movementSpeed * Time.deltaTime),
					0); 
			}

//--> Step 2 : UI Info stay on screen 
			if (listTimer [i] != stayDuration) { listTimer [i] = Mathf.MoveTowards (listTimer [i], stayDuration, Time.deltaTime);
			} else { listBack [i] = true;}


//--> Step 3 : UI Info Move up and destroyed at the end 
			if (listBack [i] && listUi_Info [i] != null) {
				// Move Up
				if (listUi_Info [i]) {
					listUi_Info [i].pivot = new Vector3 (
						0.5f,
						Mathf.MoveTowards (listUi_Info [i].pivot.y, 0, movementSpeed * Time.deltaTime),
						0); 
				}

				// init lists
				if (listUi_Info [i].pivot.y == 0) {
					GameObject tmp = listUi_Info [i].gameObject;
					listUi_Info.RemoveAt (0);
					listRefGameObject.RemoveAt (0);
					listPosY.RemoveAt (0);
					listTimer.RemoveAt (0);
					listBack.RemoveAt (0);

					Destroy (tmp);
					break;
				}
			}
		}
	}


	//--> Pause Voice Over 
	public void _Pause(){
		if (!ingameGlobalManager.instance.b_Ingame_Pause) {
			_audio.UnPause ();
		} else {
			_audio.Pause ();
		}

	}
}
