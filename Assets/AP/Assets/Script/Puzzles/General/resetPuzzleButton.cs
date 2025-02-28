// Description : resetPuzzleButton : script attached to btn_ResetPuzzle in the Hierarchy.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetPuzzleButton : MonoBehaviour
{

    public void F_ButtonResetPuzzle(){
        ingameGlobalManager.instance.currentPuzzle.GetComponent<SaveData>().ResetPuzzle("");
    }

   
}
