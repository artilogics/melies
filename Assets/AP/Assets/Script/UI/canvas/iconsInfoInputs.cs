// Description : iconsInfoInputs : Display dipending the inputs type info for the player during the game. It displays action like action for right click, ACtion to exit a puzzle ...
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iconsInfoInputs : MonoBehaviour {

    public GameObject btn_LeftClic;
    public GameObject btn_RightClic;
    public GameObject btn_FingerSlide;
    public GameObject btn_FingerDoubleTap;

    public GameObject btn_JoystickBackBouton;
    public GameObject btn_JoystickStick;


    //--> Display available actions on screen
    public void displayAvailableActionOnScreen(bool b_LeftClic, bool b_RightClic)
    {
        if (ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick)
        {       // Keyboard Case
            btn_LeftClic.SetActive(b_LeftClic);
            btn_RightClic.SetActive(b_RightClic);

            if (btn_FingerSlide.activeSelf)
            {
                btn_FingerSlide.SetActive(false);
                btn_FingerDoubleTap.SetActive(false);
            }

            if (btn_JoystickBackBouton.activeSelf)
            {
                btn_JoystickBackBouton.SetActive(false);
                btn_JoystickStick.SetActive(false);
            }
        }
        else if (ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.b_Joystick)
        {   // Joystick Case
            if (btn_LeftClic.activeSelf)
            {
                btn_LeftClic.SetActive(false);
                btn_RightClic.SetActive(false);
            }
            if (btn_FingerSlide.activeSelf)
            {
                btn_FingerSlide.SetActive(false);
                btn_FingerDoubleTap.SetActive(false);
            }


            btn_JoystickStick.SetActive(b_LeftClic);
            btn_JoystickBackBouton.SetActive(b_RightClic);
        }
        else
        {                                                                                               
            // Mobile Case
            if (btn_LeftClic.activeSelf)
            {
                btn_LeftClic.SetActive(false);
                btn_RightClic.SetActive(false);
            }

            if (btn_JoystickBackBouton.activeSelf)
            {
                btn_JoystickBackBouton.SetActive(false);
                btn_JoystickStick.SetActive(false);
            }

            btn_FingerSlide.SetActive(b_LeftClic);
            btn_FingerDoubleTap.SetActive(b_RightClic);
        }

    }

    //--> Display available actions on screen
    public void displayAvailableActionOnScreen(bool b_RightClic, string name)
    {
        if (ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick)
        {       // Keyboard Case
            btn_RightClic.SetActive(b_RightClic);
            btn_FingerDoubleTap.SetActive(false);
            btn_JoystickBackBouton.SetActive(false);
        }
        else if (ingameGlobalManager.instance.b_DesktopInputs && ingameGlobalManager.instance.b_Joystick)
        {// Joystick Case
            btn_RightClic.SetActive(false);
            btn_FingerDoubleTap.SetActive(false);
            btn_JoystickBackBouton.SetActive(b_RightClic);
        }
        else
        {                                                                                               
            // Mobile Case
            btn_FingerDoubleTap.SetActive(b_RightClic);
            btn_RightClic.SetActive(false);
            btn_JoystickBackBouton.SetActive(false);
        }

    }
}
