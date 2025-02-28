using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Clue : MonoBehaviour {
    public bool SeeInspector = false;

    [System.Serializable]
    public class clueParams
    {
        public List<string> txt_Clue = new List<string>();
        public List<Sprite> spriteClue = new List<Sprite>();    // entry Unique ID
        public bool b_Lock = false; 
    }

    public List<clueParams> clueList = new List<clueParams>(1) { new clueParams() };
    public int currentClue = 0; 

    //public string sClue = "Default Clue";

    /* [System.Serializable]
     public class idList
     {
         public int ID = 0;          // entry ID in the window tab
         public int uniqueID = 0;    // entry Unique ID
     }

     public List<idList> feedbackIDList = new List<idList>() { new idList() };      // Lock Section : Check if an Object is in the player inventory using his ID
 */

    /*public  void displayClue () {
        //Debug.Log(sClue);
        //-> Display feedback info
        ingameGlobalManager gManager = ingameGlobalManager.instance;
        if (gManager.canvasPlayerInfos._infoUI)
        {
            bool b_Exist = false;
            for (var i = 0; i < gManager.canvasPlayerInfos._infoUI.listRefGameObject.Count; i++)
            {
                if (gameObject == gManager.canvasPlayerInfos._infoUI.listRefGameObject[i])
                    b_Exist = true;
            }
            if (!b_Exist)
                gManager.canvasPlayerInfos._infoUI.playAnimInfo(gManager.currentFeedback.diaryList[gManager.currentLanguage]._languageSlot[feedbackIDList[0].ID].diaryTitle[0], "Feedback", gameObject);
        }
	}*/

    public void displayClueWithItsNumber(int clueNumber)
    {
        AP_ClueUIManager clueUIManager = GameObject.Find("ClueManager").GetComponent<AP_ClueUIManager>();

        if(clueUIManager){
            if(clueList.Count == 1){
                clueUIManager.previousClue.gameObject.SetActive(false); 
                clueUIManager.nextClue.gameObject.SetActive(false); 
                clueUIManager.obj_Txt_HowManyClues.gameObject.SetActive(false); 
            }


            if(clueList[clueNumber].b_Lock){
                clueUIManager.obj_padLock.gameObject.SetActive(true);
                clueUIManager.obj_Txt_Clue.gameObject.SetActive(false);
                clueUIManager.obj_Sprite_Clue.gameObject.SetActive(false);
                clueUIManager.obj_Faketxt.gameObject.SetActive(false);

                if (clueNumber != 0 && clueList[clueNumber - 1].b_Lock)
                    clueUIManager.obj_padLock.sprite = clueUIManager.spriteNoneAvailable;
                else
                    clueUIManager.obj_padLock.sprite = clueUIManager.spriteLock;
            }
            else{
                clueUIManager.obj_padLock.gameObject.SetActive(false);  
                clueUIManager.obj_Txt_Clue.gameObject.SetActive(true);



                int sLanguage = ingameGlobalManager.instance.currentLanguage;
                if (clueList[clueNumber].txt_Clue.Count - 1 < sLanguage)
                    sLanguage = 0;

                clueUIManager.obj_Txt_Clue.text = clueList[clueNumber].txt_Clue[sLanguage];




                if (clueList[clueNumber].spriteClue[sLanguage] != null){
                    clueUIManager.obj_Sprite_Clue.gameObject.SetActive(true);
                    clueUIManager.obj_Faketxt.gameObject.SetActive(false); 
                    clueUIManager.obj_Sprite_Clue.sprite = clueList[clueNumber].spriteClue[sLanguage];
                }
                else{
                    clueUIManager.obj_Sprite_Clue.gameObject.SetActive(false);
                    clueUIManager.obj_Faketxt.gameObject.SetActive(true);
                }
                    






            }
            clueUIManager.obj_Txt_HowManyClues.text = (clueNumber + 1).ToString() + "/" + clueList.Count;  
        }
    }


    public void AP_InitClue(string s_ObjectDatas){
        Debug.Log("Load Clue: " + s_ObjectDatas);

        string[] codes = s_ObjectDatas.Split('_');              // Split data in an array.
       

        //--> Actions to do for this puzzle ----> BEGIN <----
        if (s_ObjectDatas == "")
        {                               // Save Doesn't exist
           
        }
        else
        {                                                   // Save exist
            int startValue = 2;
            if (!GetComponent<VoiceProperties>())
                startValue = 1;
            
            Debug.Log("startValue: " + startValue);

            for (var i = 0; i < clueList.Count; i++)
            {
                if (codes[i+startValue] == "T")
                    clueList[i].b_Lock = true;
                else
                    clueList[i].b_Lock = false;
            }

          
        }
    }

    public string AP_SaveData()
    {
        Debug.Log("Save Clue: ");
        string valuesToSave = "";

        for (var i = 0; i < clueList.Count;i++){
            valuesToSave += r_TrueFalse(clueList[i].b_Lock);
            valuesToSave += "_";
        }


        return valuesToSave;
    }


    public void AP_PreviousClue()
    {
        currentClue--;
        if(currentClue < 0) currentClue =  clueList.Count - 1;
        displayClueWithItsNumber(currentClue);
    }


    public void AP_NextClue()
    {
        currentClue++;
        currentClue %= clueList.Count;
        displayClueWithItsNumber(currentClue);
    }

    public void AP_UnlockClue(){
        clueList[currentClue].b_Lock = false;       // Clue is available
        displayClueWithItsNumber(currentClue);
    }

    //--> Convert bool to T or F string
    private string r_TrueFalse(bool s_Ref)
    {
        if (s_Ref) return "T";
        else return "F";
    }

}
