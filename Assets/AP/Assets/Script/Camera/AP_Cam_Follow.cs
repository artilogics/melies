// Description : Cam_Follow.cs : use on camera to follow the player character
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour {
    
	public Transform 	target;
	public float 		rotationDamping = 15;		

	
    void LateUpdate()
    {
        if(target != null){
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping); 
        }
      
    }
}
