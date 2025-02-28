// Decription : updateManually : Debug Log is displayed if object need to be updated manually
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateManually : MonoBehaviour {

	public List<GameObject> listObjs = new List<GameObject>();


	public void Start()
	{
        Debug.Log("INFO : You need to update objects contain in the ''UpdateManually'' List_Objs." +
                  "Don't forget to delete the objects after updating manually the needed Objects");
	}
}
