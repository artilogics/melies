// Description : CT_TheBeginning : use in special end trigger to generate a fade out and display a text on screen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CT_TheBeginning : MonoBehaviour
{
    public bool SeeInspector = false;
    public GameObject CanvasTheBeginning;
    public CanvasGroup canvasGrp;

    public float blackScreenDuration = 4;

    public void activateBeginScreen()
    {
        StartCoroutine(I_activateBeginScreen());
    }

    IEnumerator I_activateBeginScreen()
    {
        //Debug.Log("here");
        CanvasTheBeginning.SetActive(true);
        ingameGlobalManager.instance.b_InputIsActivated = false; 
        ingameGlobalManager.instance.b_bodyMovement = false;

        if (!ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.canvasMobileInputs.SetActive(false);

        if(ingameGlobalManager.instance.reticule && ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.reticule.SetActive(false);

        if (canvasGrp)
        {
            canvasGrp.interactable = true;
            canvasGrp.blocksRaycasts = true;
        }

        float t = 0;

        while (t != blackScreenDuration)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t = Mathf.MoveTowards(t, blackScreenDuration, Time.deltaTime);
            }
            yield return null;
        }

        t = 0;
        while (t != 1)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t = Mathf.MoveTowards(t, 1, Time.deltaTime);

                canvasGrp.alpha = 1-t;
            }
            yield return null;
        }

        if (canvasGrp)
        {
            canvasGrp.interactable = false;
            canvasGrp.blocksRaycasts = false;
        }

        ingameGlobalManager.instance.b_InputIsActivated = true; 
        ingameGlobalManager.instance.b_bodyMovement = true;

        if (ingameGlobalManager.instance.reticule && ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.reticule.SetActive(true);

        if (!ingameGlobalManager.instance.b_DesktopInputs)
            ingameGlobalManager.instance.canvasMobileInputs.SetActive(true);

        yield return null;
    }

  


}
