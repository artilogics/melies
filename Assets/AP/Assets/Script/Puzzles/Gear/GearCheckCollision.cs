// Description : GearCheckCollision : Use in gear puzzle to know if a gear is touching an other gear
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearCheckCollision : MonoBehaviour {
    public bool b_CollisionWithOtherGear = false;
    public int counter = 0;

    public void OnTriggerEnter(Collider other)
    {

        if ((other.CompareTag("PuzzleObject") || other.CompareTag("GearFixed")) && gameObject.transform.localPosition != Vector3.zero)
        {
            //Debug.Log(other.name);
            // Debug.Log(other.transform.parent.transform.parent.name + " : " + gameObject.transform.parent.transform.parent.name);
            b_CollisionWithOtherGear = true;
        }

    }

    public void OnTriggerStay(Collider other){
        if((other.CompareTag("PuzzleObject") || other.CompareTag("GearFixed") )&& gameObject.transform.localPosition != Vector3.zero){
            //Debug.Log(other.name);
           // Debug.Log(other.transform.parent.transform.parent.name + " : " + gameObject.transform.parent.transform.parent.name);
            b_CollisionWithOtherGear = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PuzzleObject") 
            || 
            other.CompareTag("GearFixed"))
        {
            b_CollisionWithOtherGear = false;
        }
    }




    public bool returnCheckCollision(){
        return b_CollisionWithOtherGear;
    }
}
