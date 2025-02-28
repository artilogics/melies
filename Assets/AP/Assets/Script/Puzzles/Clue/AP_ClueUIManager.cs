using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AP_ClueUIManager : MonoBehaviour
{
    public Text obj_Txt_Clue;
    public Text obj_Faketxt;
    public Image obj_Sprite_Clue;
    public GameObject previousClue;
    public GameObject nextClue;
    public Text obj_Txt_HowManyClues;
    public Image obj_padLock;

    public Sprite spriteLock;
    public Sprite spriteNoneAvailable;

    public AudioClip a_Validate;
    public AudioClip a_Wrong;

    public void AP_Btn_PreviousClue()
    {
        ingameGlobalManager.instance.currentPuzzle.GetComponent<conditionsToAccessThePuzzle>().objClueBox.AP_PreviousClue();         // Display the first clue;
        ingameGlobalManager.instance.audioMenu.clip = a_Validate;
        ingameGlobalManager.instance.audioMenu.Play();
    }

    public void AP_Btn_NextClue()
    {
        ingameGlobalManager.instance.currentPuzzle.GetComponent<conditionsToAccessThePuzzle>().objClueBox.AP_NextClue();         // Display the first clue;
        ingameGlobalManager.instance.audioMenu.clip = a_Validate;
        ingameGlobalManager.instance.audioMenu.Play();
    }

    public void AP_Btn_ShowAds()
    {
        if(obj_padLock.sprite == spriteLock){
            Debug.Log("Show Ads");
            ingameGlobalManager.instance.audioMenu.clip = a_Validate;
            ingameGlobalManager.instance.audioMenu.Play();
            StartCoroutine(unlockClue());
            // Here Call the method that starts your Ads
        }
        else{
            ingameGlobalManager.instance.audioMenu.clip = a_Wrong;
            ingameGlobalManager.instance.audioMenu.Play();
            Debug.Log("Previous Clue must be unlock first");   
        }
    }

    IEnumerator unlockClue(){
        yield return new WaitForSeconds(1);
        ingameGlobalManager.instance.currentPuzzle.GetComponent<conditionsToAccessThePuzzle>().objClueBox.AP_UnlockClue();
        yield return null;
    }
}
