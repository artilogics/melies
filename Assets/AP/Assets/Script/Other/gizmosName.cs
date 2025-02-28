//Description : gizmosName.cs : Use to display the gameObject name on scene view
using UnityEngine;
using System.Collections;

public class gizmosName : MonoBehaviour {

	public float _Size = .25F; 

	void OnDrawGizmos() {
		Gizmos.color = new Color(1,.092F,.016F,.5F);
		Gizmos.DrawSphere(transform.position, _Size);
	}

}
