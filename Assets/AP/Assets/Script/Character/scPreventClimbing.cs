using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scPreventClimbing : MonoBehaviour {


    public bool b_preventClimbing = false;

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("DoNotClimb"))
            b_preventClimbing = true;
    }

    void OnTriggerExit(Collider other)
    {
        if(b_preventClimbing && other.CompareTag("DoNotClimb"))
            b_preventClimbing = false;
    }
}
