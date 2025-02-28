// Descition : detectPuzzleClick : Detect if the player click (mouse, gamepad and Mobile) on screen when a puzzle is activated
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectPuzzleClick {
    public Transform F_detectPuzzleClick(LayerMask myLayer,ingameGlobalManager _ingameManager, int validationButtonJoystick){
        //-> Joystick Case
        if (_ingameManager.canvasPlayerInfos.ReticuleJoystick && _ingameManager.b_Joystick && _ingameManager.b_DesktopInputs)  
            return joystickCheckClick(myLayer, _ingameManager.canvasPlayerInfos.ReticuleJoystick, _ingameManager, validationButtonJoystick);
        //-> Keyboard Case
        else if (!_ingameManager.b_Joystick && _ingameManager.b_DesktopInputs)    
            return keyboardCheckClick(myLayer);
        //-> Mobile case
        else
            return MobileCheckClick(myLayer);
    }




    //--> Check mouse click
    public Transform keyboardCheckClick(LayerMask myLayer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, myLayer))
        {
            if (hit.transform.gameObject.CompareTag("PuzzleObject"))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    return hit.transform;
            }
        }
      
        return null;  
    }


    //--> Check Joystick Click
    public Transform joystickCheckClick(LayerMask myLayer,Transform ReticuleJoystick, ingameGlobalManager _ingameManager,int validationButtonJoystick)
    {
       
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(ReticuleJoystick.position.x, ReticuleJoystick.position.y, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, myLayer))
        {
            if (hit.transform.gameObject.CompareTag("PuzzleObject"))
            {
                if (Input.GetKeyDown(_ingameManager.inputListOfStringGamepadButton[validationButtonJoystick]))
                {
                    return hit.transform;
                }
            }
        }

        return null;
    }
 


    //--> Check mouse click
    public Transform MobileCheckClick(LayerMask myLayer)
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, myLayer))
                {
                    if (hit.transform.gameObject.CompareTag("PuzzleObject"))
                    {
                        return hit.transform;
                    }
                }
            }
        }

        return null;
    }








    //--> Detect if if a puzzle object is under the fake joystick reticule
    public bool F_detectPuzzleObject(LayerMask myLayer, ingameGlobalManager _ingameManager, int validationButtonJoystick)
    {
        //-> Joystick Case
        if (_ingameManager.canvasPlayerInfos &&
            _ingameManager.canvasPlayerInfos.ReticuleJoystick && 
            _ingameManager.b_Joystick && 
            _ingameManager.b_DesktopInputs &&
            _ingameManager.navigationList.Count > 0 &&
            _ingameManager.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "Focus")
            return joystickCheckPuzzleObject(myLayer, _ingameManager.canvasPlayerInfos.ReticuleJoystick, _ingameManager, validationButtonJoystick);

        return false;
    }


    public bool joystickCheckPuzzleObject(LayerMask myLayer,Transform ReticuleJoystick, ingameGlobalManager _ingameManager,int validationButtonJoystick)
    {
        if(Camera.main && ReticuleJoystick){
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(ReticuleJoystick.position.x, ReticuleJoystick.position.y, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, myLayer))
            {
                if (hit.transform.gameObject.CompareTag("PuzzleObject"))
                {
                    return true;
                }
            }
        }


        return false;
    }
     
}
