// MeshCombiner.cs Description : Combine meshes for a selected group

using UnityEngine;
using System.Collections.Generic;

public class  Meshcombinervtwo : MonoBehaviour {
	public bool 			SeeInspector 	= false;							// use to draw default Inspector
	public List<Material> 	list_Materials 	= new List<Material>();				// List of materials
	public List<string> 	list_Tags 		= new List<string>();				// List of tags


    public List<Material>   list_MaterialsForVerticesCheck = new List<Material>();                // List of materials
        

	public List<GameObject> list_CombineObjects 	= new List<GameObject>();
	public List<GameObject> list_CreatedObjects 	= new List<GameObject>();

	public bool				CombineDone = false;


    public bool             moreOptions = false;

    public float            f_ScaleInLightmap = 1;
    public bool             b_StitchSeams = false;


    public float            _HardAngle = 88;
    public float            _PackMargin = .04f;
    public float            _AngleError = .08f;
    public float            _AreaError = .15f;


    public bool             keepShadowMode = true;

    public int              _vertexCountMax = 65000;

    public int              currentMatSelected = -1;

    public bool             b_CombineAvailable = false;
    public bool             b_CalculateVertices = false;
}

