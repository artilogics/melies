// Description : datasWindowReadyToUse : ScriptableObject : Save data for the window w_ObjCreator
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class datasWindowReadyToUse : ScriptableObject 
{
	public bool 		    helpBoxEditor = false;							// Show helpbox in the window tab
	//public string 		currentDatasProjectFolder = "Default";			// The folder where are the datas for the game (inventory, infos, feedback,diary...)
    public int 			    currentTypeSelected = 0;
    public List<Texture2D>  listOfTexture2DDoor = new List<Texture2D>();
    public List<Texture2D>  listOfTexture2DDrawer = new List<Texture2D>();
    public List<Texture2D>  listOfTexture2DWardrobe = new List<Texture2D>();
    public List<Texture2D>  listOfTexture2DActionTrigger = new List<Texture2D>();

    public int              currentItemSubType = 0;
    public int              currentDoorSubType = 0;
    public int              currentDrawerSubType = 0;
    public int              currentWardrobeSubType = 0;
    public int              currentActionTriggerSubType = 0;

    public List<GameObject> listOfPuzzles = new List<GameObject>();

    public List<GameObject> listOfObjsDoor = new List<GameObject>();
    public List<GameObject> listOfObjsDrawer = new List<GameObject>();
    public List<GameObject> listOfObjsWardrobe = new List<GameObject>();
    public List<GameObject> listOfObjsActionTrigger = new List<GameObject>();

    public List<GameObject> listOfObjsCustomAction = new List<GameObject>();

    public GameObject       clueSystem;


    public bool             b_TextProperties = true;
    public bool             b_VoiceProperties = true;
    public bool             b_Collider = true;
    public bool             b_SaveSystem = true;


    public bool             b_ActivatedTheFirstTime = true;
}
	
