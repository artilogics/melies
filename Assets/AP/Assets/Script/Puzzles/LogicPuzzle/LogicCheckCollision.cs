// Decription : LogicCheckCollision : Script attached to logics Object to check collision 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicCheckCollision : MonoBehaviour {

    public void OnTriggerStay(Collider other){
        if(other.CompareTag("PuzzleObject")){
            Debug.Log(other.name + " : " + gameObject.name);
        }

    }
}
