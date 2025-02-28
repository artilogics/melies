//Description : gizmosCube.cs : Use to display a Cube on gameObject 
using UnityEngine;
using System.Collections;

public class gizmosCube : MonoBehaviour {

	public Color GizmoColor = new Color(0,.9f,1f,.5f);	

	public int meshType = 0;

	public Vector3 customScale = new Vector3 (1, 1, 1);
	public Vector3 customPosition = new Vector3 (0, 0, 0);

	void OnDrawGizmos()
	{
		Gizmos.color = GizmoColor;																						

		Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);		// Allow the gizmo to fit the position, rotation and scale of the gameObject
		Gizmos.matrix = cubeTransform;

		F_MeshType ();


	}



	void F_MeshType(){
		switch (meshType) {
			case 0:
			Gizmos.DrawCube(customPosition, customScale);
			Gizmos.DrawWireCube(customPosition, customScale);
				break;
			case 1:
			Gizmos.DrawMesh(gameObject.GetComponent<MeshFilter>().sharedMesh,new Vector3(0,gameObject.transform.localScale.y,0), Quaternion.identity,customScale);
				//Gizmos.DrawWireMesh(Vector3.zero, Vector3.one);
				break;
		}
	}
}
