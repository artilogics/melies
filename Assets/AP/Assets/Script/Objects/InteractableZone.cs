// Description : InteractableZone : Use to display feedback regarding an object when the player press interaction button
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableZone : MonoBehaviour
{
    public bool SeeInspector = false;
    public int VoiceOverID = -1;
    public string PlayerTag = "Player";
    public Sprite UI_Icon;

    private bool isInZone = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInZone && Input.GetKeyDown(KeyCode.E) && !ingameGlobalManager.instance.b_Ingame_Pause)
        {
            ShowFeedback();
        }
    }

    public void ShowFeedback()
    {
        string textToShow = "";
        
        if (GetComponent<TextProperties>())
        {
            textToShow = GetComponent<TextProperties>().returnInfoText();
        }
        else
        {
            textToShow = "Missing TextProperties Component";
        }

        // Display Feedback
        if (ingameGlobalManager.instance.canvasPlayerInfos && ingameGlobalManager.instance.canvasPlayerInfos._infoUI)
        {
            ingameGlobalManager.instance.canvasPlayerInfos._infoUI.playAnimInfo(textToShow, "Feedback");
        }

        // Play VoiceOver if ID is valid
        if (VoiceOverID != -1 && ingameGlobalManager.instance.voiceOverManager)
        {
             // Logic to play voice over if needed later, keeping it simple for now as per minimal requirement
             // ingameGlobalManager.instance.voiceOverManager.playVoiceOver(VoiceOverID);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isInZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isInZone = false;
        }
    }
}
