using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class Torch : MonoBehaviour
    {
        public int              torchID = 0;
        //  inputs IDs
        public int              JoystickTorch = 6;
        public int              KeyboardTorch = 9;

        public GameObject       objTorch;
        public bool             bTorchState;

        public bool             bActionAvailable = true;
        public Transform        torchPosOn;
        public Transform        torchPosOff;

        public float            durationOnOff = .5f;

        public AudioSource      aFlashlightMovement;
        public AudioSource      aFlashlightOnOff;

        

       

        void Start()
        {
            // Init Torch if the gameplay scene is loading without using the Menu scene.
            if (Application.isEditor)
            {
                if (bTorchState)
                {
                    objTorch.SetActive(true);
                    transform.position = torchPosOn.position;
                }
                else
                {
                    objTorch.SetActive(false);
                    transform.position = torchPosOff.position;
                }
            }
                
        }

        void Update()
        {
          /*  if (ingameGlobalManager.instance.b_InputIsActivated &&
                ingameGlobalManager.instance.navigationList.Count == 0)
            {*/
                if (Input.GetKeyDown(
                ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickTorch]) &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                ingameGlobalManager.instance.b_Joystick
                ||

                Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardTorch]) &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                !ingameGlobalManager.instance.b_Joystick)
                {
                    if (objTorch/* && bActionAvailable*/)
                    {
                        bTorchState = !bTorchState;
                        StartCoroutine(MoveTorchRoutine());
                    }
                }
           // }
        }


        public IEnumerator MoveTorchRoutine()
        {
           // Debug.Log("Torch On/Off");
            bActionAvailable = false;
            Transform target = torchPosOn;
            if(!bTorchState) target = torchPosOff;

            if (bTorchState) objTorch.SetActive(true);

            if (aFlashlightOnOff && !bTorchState) aFlashlightOnOff.Play();

            if (aFlashlightMovement) aFlashlightMovement.Play();


            var currentPos = transform.position;
            float t = 0;

            while(t < 1)
            {
               // if (!ingameGlobalManager.instance.b_Ingame_Pause)
               // {
                    t += Time.deltaTime / durationOnOff;
                    transform.position = Vector3.Lerp(currentPos, target.position, t);
               // }
                yield return null;
            }

            if (!bTorchState) objTorch.SetActive(false);
            else objTorch.SetActive(true);

            if (aFlashlightOnOff && bTorchState) aFlashlightOnOff.Play();


             bActionAvailable = true;
            yield return null;
        }

        //-> If save exist init the torch using the save state of the current selected slot
        public void Init()
        {
            Debug.Log("init Torch");
            string prefs = ingameGlobalManager.instance.currentSaveSlot + "_" + torchID;

            if (PlayerPrefs.HasKey(prefs))
            {
                if (bool.Parse(PlayerPrefs.GetString(prefs)))
                {
                    bTorchState = true;
                }
                else
                {
                    bTorchState = false;
                }
            }


            if (bTorchState)
            {
                objTorch.SetActive(true);
                transform.position = torchPosOn.position;
            }
            else
            {
                objTorch.SetActive(false);
                transform.position = torchPosOff.position;
            }
        }

        public void SaveTorchState()
        {
            string prefs = ingameGlobalManager.instance.currentSaveSlot + "_" + torchID;
            PlayerPrefs.SetString(prefs, bTorchState.ToString());
        }
    }
}
