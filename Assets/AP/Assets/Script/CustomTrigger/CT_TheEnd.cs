// Description : CT_TheEnd : use in special end trigger to generate a fade in and display a text on screen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CT_TheEnd : MonoBehaviour
{
    public bool SeeInspector = false;
    public GameObject CanvasTheEnd;
    public CanvasGroup canvasGrp;

    public bool b_EndActivated = false;
    public bool b_Once = true;

    public float fadeSpeed = .5f;

    private void Update()
    {
        if(Input.anyKeyDown && b_EndActivated && b_Once){
            b_Once = false;
            F_GoToMain();
        }
    }

    public void activateEndScreen()
    {
        StartCoroutine(I_activateEndScreen());
    }



    IEnumerator I_activateEndScreen()
    {
        CanvasTheEnd.SetActive(true);
        ingameGlobalManager.instance.b_InputIsActivated = false; 
        ingameGlobalManager.instance.b_bodyMovement = false;

        if (canvasGrp)
        {
            canvasGrp.interactable = true;
            canvasGrp.blocksRaycasts = true;
        }


        float t = 0;
        while (t != 1)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t = Mathf.MoveTowards(t, 1, Time.deltaTime * fadeSpeed);
                canvasGrp.alpha = t;
            }
            yield return null;
        }

        t = 0;
        while (t != 2)
        {
            if (!ingameGlobalManager.instance.b_Ingame_Pause)
            {
                t = Mathf.MoveTowards(t, 2, Time.deltaTime);
            }
            yield return null;
        }


        // Player can't activate any interface or action
        ingameGlobalManager.instance.navigationList.Add("TheEnd");
        b_EndActivated = true;
        yield return null;
    }

    public void F_GoToMain()
    {
        Destroy(ingameGlobalManager.instance.currentPlayer);
        ingameGlobalManager.instance.b_InputIsActivated = true; 
        ingameGlobalManager.instance.b_AllowCharacterMovment = true;
        ingameGlobalManager.instance.GetComponent<SaveAndLoadManager>().F_Load_MainMenu_Scene(0);
    }


}
