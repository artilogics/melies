//MeshCombinerEditor : use with MeshComnier.cs
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof(Meshcombinervtwo))]
public class MeshcombinervtwoEditor : Editor {
	SerializedProperty		SeeInspector;
	SerializedProperty		list_Tags;
	SerializedProperty		CombineDone;

   // SerializedProperty      moreOptions;

    SerializedProperty      f_ScaleInLightmap;
    SerializedProperty      b_StitchSeams;
    SerializedProperty      _HardAngle;
    SerializedProperty      _PackMargin;
    SerializedProperty      _AngleError;
    SerializedProperty      _AreaError;

    SerializedProperty      keepShadowMode;
    SerializedProperty      _vertexCountMax;

    SerializedProperty currentMatSelected;
    SerializedProperty b_CombineAvailable;
    SerializedProperty b_CalculateVertices;




	private bool 			b_Combine = false;
	private bool 			b_Uncombine = false;
	private bool			b_AddTag = false;
	private bool			b_DeleteTag = false;
	private int				deleteNum = 0;



	//Quaternion oldRot = Quaternion.identity;						// Save the original position and rotation of obj
	//Vector3 oldPos =  new Vector3(0,0,0);

	private Texture2D MakeTex(int width, int height, Color col) {
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	// Use this for initialization
	void OnEnable () {
		SeeInspector 	= serializedObject.FindProperty ("SeeInspector");
		list_Tags		= serializedObject.FindProperty ("list_Tags");
		CombineDone		= serializedObject.FindProperty ("CombineDone");

       // moreOptions     = serializedObject.FindProperty("moreOptions");

        f_ScaleInLightmap= serializedObject.FindProperty("f_ScaleInLightmap");
        b_StitchSeams   = serializedObject.FindProperty("b_StitchSeams");

        _HardAngle      = serializedObject.FindProperty("_HardAngle");
        _PackMargin     = serializedObject.FindProperty("_PackMargin");
        _AngleError     = serializedObject.FindProperty("_AngleError");
        _AreaError      = serializedObject.FindProperty("_AreaError");

        keepShadowMode  = serializedObject.FindProperty("keepShadowMode");

        _vertexCountMax = serializedObject.FindProperty("_vertexCountMax");
        currentMatSelected = serializedObject.FindProperty("currentMatSelected");
        b_CombineAvailable = serializedObject.FindProperty("b_CombineAvailable");
        b_CalculateVertices = serializedObject.FindProperty("b_CalculateVertices");


	}

	public override void OnInspectorGUI(){
		Meshcombinervtwo myScript = (Meshcombinervtwo)target;
		GUIStyle style = new GUIStyle();

		style.normal.background = MakeTex(2, 2, new Color(.5f, .8f, 0, .3f));

		serializedObject.Update ();

		EditorGUILayout.LabelField("");

		if(SeeInspector.boolValue)
			DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("See Variables : ",GUILayout.Width(90));
			EditorGUILayout.PropertyField (SeeInspector, new GUIContent (""));
		EditorGUILayout.EndHorizontal();	

		EditorGUILayout.LabelField("");

		EditorGUILayout.HelpBox("\n" +"Combine Meshes : " +
			"\n" +
			"\n1 - GameObject inside this gameObject are combine." +
			"\n" +
			"\nAll the gameObjects with the same material are combine in a single mesh." +
			"\n" +
			"\nCombining Process could take time if there are a lots of gameObjects to combine." +
			"\n" +
			"\n2 - Choose the ScaleInLightmap then Press button Combine to start the process" +
			"\n",MessageType.Info);


		EditorGUILayout.BeginVertical(style);
        EditorGUILayout.LabelField("");
       
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scale In Lightmap : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(f_ScaleInLightmap, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        /*
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show more options : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(moreOptions, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        if(moreOptions.boolValue){
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hard Angle : ", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_HardAngle, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pack Margin : ", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_PackMargin, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angle Error : ", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_AngleError, new GUIContent(""));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Area Error : ", GUILayout.Width(120));
            EditorGUILayout.PropertyField(_AreaError, new GUIContent(""));
            EditorGUILayout.EndHorizontal();
        }*/

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Stitch Seams : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_StitchSeams, new GUIContent(""));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Keep Shadow Mode : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(keepShadowMode, new GUIContent(""));
        EditorGUILayout.EndHorizontal();


		if(!CombineDone.boolValue){
			if(GUILayout.Button("Combine"))
			{
                currentMatSelected.intValue = -1;
				b_Combine = true;
			}
		}
		else{
			if(GUILayout.Button("Reset")){
                currentMatSelected.intValue = -1;
				b_Uncombine = true;
			}
		}
       

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show Vertices : ", GUILayout.Width(120));
        EditorGUILayout.PropertyField(b_CalculateVertices, new GUIContent(""));
        EditorGUILayout.EndHorizontal();
        if(b_CalculateVertices.boolValue)
        calculatesVertices(myScript);

		//EditorGUILayout.LabelField("");
		EditorGUILayout.EndVertical();
		//

		EditorGUILayout.HelpBox("3 - After the combining process gameObjects are created for each material inside this folder.",MessageType.Info);
		EditorGUILayout.HelpBox("INFO 1 : Mesh renderer are disabled for the gameObjects that have been used in the combining process." +
		"\nINFO 2 : After combinig process, colliders stay activated.",MessageType.Info);


		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("");
		style.normal.background = MakeTex(2, 2, new Color(.5f,.8f,0,.3f));
		EditorGUILayout.BeginVertical(style);
		EditorGUILayout.HelpBox("(Optional) : Exclude gameObjects with specific TAG",MessageType.Info);
			EditorGUILayout.LabelField("");
			
			if(GUILayout.Button("Add new tag"))
			{
				b_AddTag = true;
			}
			EditorGUILayout.LabelField("");

			EditorGUILayout.LabelField("Exclude gameObjects with these Tags :");

			for(int i = 0; i < myScript.list_Tags.Count; i++){
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.PropertyField (list_Tags.GetArrayElementAtIndex(i), new GUIContent (""));
				if(GUILayout.Button("-",GUILayout.Width(20)))
				{
					b_DeleteTag = true;
					deleteNum  = i;
					break;
				}
				EditorGUILayout.EndHorizontal();
			}
		EditorGUILayout.EndVertical();


		serializedObject.ApplyModifiedProperties ();

		if(b_Combine){
			Undo.RegisterCompleteObjectUndo(myScript,"MeshCombiner" + myScript.gameObject.name);
			
			Component[] ChildrenMesh = myScript.GetComponentsInChildren(typeof(MeshRenderer), true);

            int vertexCount = 0;
            foreach (MeshRenderer child in ChildrenMesh)
            {
                if (child.GetComponent<MeshFilter>() && child.GetComponent<MeshFilter>().sharedMesh != null)
                    vertexCount += child.GetComponent<MeshFilter>().sharedMesh.vertexCount;
            }

            Debug.Log("vertexCount : " + vertexCount);
            if(_vertexCountMax.intValue < vertexCount){
                if (EditorUtility.DisplayDialog("INFO : Action not available",
                                                "Select fewer objects at once."+
                                                "\nThe combiner can't combine more than " + _vertexCountMax.intValue + " vertices for a Material type at the same time.",
                                                "Continue"))
                {

                }
            }
            else{
                myScript.CombineDone = true;
                myScript.list_Materials.Clear();

                foreach (MeshRenderer child in ChildrenMesh)
                {               // Find all the different materials
                    myScript.list_Materials.Add(child.sharedMaterial);
                }

                for (int i = 0; i < myScript.list_Materials.Count; i++)
                {               // remove materials using multiple time
                    for (int k = 0; k < myScript.list_Materials.Count; k++)
                    {
                        if (k != i && myScript.list_Materials[i] == myScript.list_Materials[k])
                        {
                            myScript.list_Materials[k] = null;
                        }
                    }
                }

                List<Material> Tmp_list_Materials = new List<Material>();               // List of materials

                for (int i = 0; i < myScript.list_Materials.Count; i++)
                {               // Update Materials List
                    if (myScript.list_Materials[i])
                    {
                        Tmp_list_Materials.Add(myScript.list_Materials[i]);
                    }
                }

                myScript.list_Materials.Clear();

                for (int i = 0; i < Tmp_list_Materials.Count; i++)
                {               // Update Materials List
                    myScript.list_Materials.Add(Tmp_list_Materials[i]);
                }

                myScript.list_CreatedObjects.Clear();
                myScript.list_CombineObjects.Clear();

                Quaternion oldRot = myScript.transform.rotation;                        // Save the original position and rotation of obj
                Vector3 oldPos = myScript.transform.position;

                myScript.transform.rotation = Quaternion.identity;
                myScript.transform.position = new Vector3(0, 0, 0);

                for (int i = 0; i < myScript.list_Materials.Count; i++)
                {
                    CombineMeshes(myScript.list_Materials[i]);
                }

                myScript.transform.rotation = oldRot;
                myScript.transform.position = oldPos; 
               
            }

            b_Combine = false;

			

		}

		if(b_Uncombine){
			Undo.RegisterCompleteObjectUndo(myScript,"MeshCombiner" + myScript.gameObject.name);
			myScript.CombineDone = false;

			for(int i = 0; i < myScript.list_CombineObjects.Count; i++){	
				if(myScript.list_CombineObjects[i] != null){
					SerializedObject serializedObject3 = new UnityEditor.SerializedObject(myScript.list_CombineObjects[i].gameObject.GetComponents<Renderer>());
					serializedObject3.Update ();
					SerializedProperty tmpSer2 = serializedObject3.FindProperty("m_Enabled");
					tmpSer2.boolValue  = true;
					serializedObject3.ApplyModifiedProperties ();
				}
			}

			for(int i = 0; i < myScript.list_CreatedObjects.Count; i++){	
				if(myScript.list_CreatedObjects[i] != null){
					Undo.DestroyObjectImmediate(myScript.list_CreatedObjects[i]);
				}
			}

			myScript.list_CreatedObjects.Clear();
			myScript.list_CombineObjects.Clear();
			b_Uncombine = false;
		}

		if(b_AddTag){
			Undo.RegisterCompleteObjectUndo(myScript,"Save" + myScript.gameObject.name);
			myScript.list_Tags.Add("Your Tag");
			b_AddTag = false;
		}

		if(b_DeleteTag){
			Undo.RegisterCompleteObjectUndo(myScript,"Delete" + myScript.gameObject.name);
			myScript.list_Tags.RemoveAt(deleteNum);
			b_DeleteTag = false;
		}
	}


	public void CombineMeshes (Material mat) {											// -> Combine all the maesh with a specif material.
		
		Meshcombinervtwo myScript = (Meshcombinervtwo)target;

		GameObject newGameObject = new GameObject();

		newGameObject.AddComponent<MeshFilter>();
		newGameObject.AddComponent<MeshRenderer>();

		newGameObject.GetComponent<Renderer>().sharedMaterial = null;

		newGameObject.name = "Combine_" + mat.name;
		Undo.RegisterCreatedObjectUndo(newGameObject,"CombineMat" + mat.name);

		myScript.list_CreatedObjects.Add(newGameObject);

		bool OneMesh = false;												// This variable is used to know if there is at least one mesh to combine

		newGameObject.transform.rotation = Quaternion.identity;						// Init position to zero

		newGameObject.transform.SetParent(myScript.transform);
		newGameObject.transform.localPosition = new Vector3(0,0,0);								// Init position to Vector3(0,0,0)
		newGameObject.isStatic = true;

		MeshFilter[] filters = myScript.gameObject.GetComponentsInChildren<MeshFilter>();	// Find all the children with MeshFilter component

		Mesh finalMesh = new Mesh();										// Create the new mesh

		CombineInstance[] combiners = new CombineInstance[filters.Length];	// Struct used to describe meshes to be combined using Mesh.CombineMeshes.

        UnityEngine.Rendering.ShadowCastingMode currentShadowMode = UnityEngine.Rendering.ShadowCastingMode.On;

		for(int i = 0; i < filters.Length; i++){							// Check all the children
			if(filters[i].transform ==  myScript.gameObject.transform)						// Do not select the parent himself
				continue;
			if(filters[i].gameObject.GetComponent<Renderer>() ==  null)		// Check if there is Renderer component
				continue;
			bool checkTag = false;								
			for(int j = 0; j < myScript.list_Tags.Count; j++){						// Check tag to know if you need to ignore this gameobject 
				if(filters[i].gameObject.tag ==  myScript.list_Tags[j]){
					checkTag = true;
				}
			}

			if(mat == filters[i].gameObject.GetComponent<Renderer>().sharedMaterial && !checkTag	// Add this gameObject to the combiner
				&& filters[i].gameObject.GetComponent<Renderer>().enabled){
				combiners[i].subMeshIndex = 0;
				combiners[i].mesh = filters[i].sharedMesh;

				combiners[i].transform = filters[i].transform.localToWorldMatrix;

				myScript.list_CombineObjects.Add(filters[i].gameObject);

				SerializedObject serializedObject3 = new UnityEditor.SerializedObject(filters[i].gameObject.GetComponents<Renderer>());
				serializedObject3.Update ();
				SerializedProperty tmpSer2 = serializedObject3.FindProperty("m_Enabled");
				tmpSer2.boolValue  = false;
				serializedObject3.ApplyModifiedProperties ();


                currentShadowMode = filters[i].GetComponent<Renderer>().shadowCastingMode;

				OneMesh = true;
			}

		}

		finalMesh.CombineMeshes(combiners);						// Combine the new mesh
		newGameObject.GetComponent<MeshFilter>().sharedMesh = finalMesh;		// Create the new Mesh Filter
		newGameObject.GetComponent<Renderer>().material = mat;				// ADd the good material

	
        // newGameObject.GetComponent<MeshRenderer>().
        Change_Scaleinlightmap(newGameObject,OneMesh,finalMesh,myScript);

        if(keepShadowMode.boolValue == true)                    // use the shadow mode find find on the last combine object
            UpdateCasShadowMode(newGameObject,currentShadowMode);
	}


    private void Change_Scaleinlightmap(GameObject go,bool OneMesh,Mesh finalMesh,Meshcombinervtwo myScript)
    {
        //Check if he got a renderer
        if (go.GetComponent<MeshRenderer>() != null)
        {
          

            //Find the property and modify them
                                                   
            SerializedObject serializedObject2 = new UnityEditor.SerializedObject(go.GetComponent<Renderer>());
            SerializedProperty m_nScaleInLightmap = serializedObject2.FindProperty("m_ScaleInLightmap");
            SerializedProperty m_StitchLightmapSeams = serializedObject2.FindProperty("m_StitchLightmapSeams");
            serializedObject2.Update();

            m_nScaleInLightmap.floatValue = f_ScaleInLightmap.floatValue;
            m_StitchLightmapSeams.boolValue = b_StitchSeams.boolValue;

            serializedObject2.ApplyModifiedProperties();

          
        }


        if (!OneMesh)
        {                                           // If there is nothing to combine delete the object
            if (myScript.gameObject.GetComponent<MeshFilter>()) myScript.gameObject.GetComponent<MeshFilter>().sharedMesh = null;
        }
        else
        {
            UnwrapParam param = new UnwrapParam();              // enable lightmap


            UnwrapParam.SetDefaults(out param);
            param.hardAngle =   _HardAngle.floatValue;
            param.packMargin =  _PackMargin.floatValue;
            param.angleError =  _AngleError.floatValue;
            param.areaError =   _AreaError.floatValue;


            Unwrapping.GenerateSecondaryUVSet(finalMesh, param);

           
        }

    }

    private void UpdateCasShadowMode(GameObject go,UnityEngine.Rendering.ShadowCastingMode currentShadowMmode){
        go.GetComponent<Renderer>().shadowCastingMode = currentShadowMmode;
    }

    /*
    public void Test(){
        Meshcombinervtwo myScript = (Meshcombinervtwo)target;

        Debug.Log("Ok " + myScript.gameObject.name);
    }
    */




    private void calculatesVertices(Meshcombinervtwo myScript){
        myScript.list_MaterialsForVerticesCheck.Clear();
        Component[] ChildrenMesh = myScript.GetComponentsInChildren(typeof(MeshRenderer), true);



        foreach (MeshRenderer child in ChildrenMesh)
        {               // Find all the different materials
            myScript.list_MaterialsForVerticesCheck.Add(child.sharedMaterial);
        }

        for (int i = 0; i < myScript.list_MaterialsForVerticesCheck.Count; i++)
        {               // remove materials using multiple time
            for (int k = 0; k < myScript.list_MaterialsForVerticesCheck.Count; k++)
            {
                if (k != i && myScript.list_MaterialsForVerticesCheck[i] == myScript.list_MaterialsForVerticesCheck[k])
                {
                    myScript.list_MaterialsForVerticesCheck[k] = null;
                }
            }
        }

        List<Material> Tmp_list_Materials_2 = new List<Material>();               // List of materials


        for (int i = 0; i < myScript.list_MaterialsForVerticesCheck.Count; i++)
        {               // Update Materials List
            if (myScript.list_MaterialsForVerticesCheck[i])
            {
                Tmp_list_Materials_2.Add(myScript.list_MaterialsForVerticesCheck[i]);
            }
        }

        List<int> Tmp_list_Vertices_2 = new List<int>();               // List of materials

        for (int i = 0; i < Tmp_list_Materials_2.Count; i++)
        {
            Tmp_list_Vertices_2.Add(0);
            foreach (MeshRenderer child in ChildrenMesh)
            {              
                if(child.GetComponent<Renderer>().sharedMaterial == Tmp_list_Materials_2[i] && 
                   child.gameObject.name != "Combine_" + Tmp_list_Materials_2[i].name)
                {
                    if(child.GetComponent<MeshFilter>()&& child.GetComponent<MeshFilter>().sharedMesh !=null)
                    Tmp_list_Vertices_2[i] += child.GetComponent<MeshFilter>().sharedMesh.vertexCount;
            }
        }
        }



        // Display list of material and the number of vertices
        for (int i = 0; i < Tmp_list_Materials_2.Count; i++)
        {               // Update Materials List
            //myScript.list_MaterialsForVerticesCheck.Add(Tmp_list_Materials[i]);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("", GUILayout.Width(20)))
            {
                if(currentMatSelected.intValue == i)
                    currentMatSelected.intValue = -1;
                else
                    currentMatSelected.intValue = i;
            }

            EditorGUILayout.LabelField("Mat : " + Tmp_list_Materials_2[i].name, GUILayout.Width(120));

            EditorGUILayout.LabelField(" Vertices : " + Tmp_list_Vertices_2[i], GUILayout.Width(120));

            if (Tmp_list_Vertices_2[i] > _vertexCountMax.intValue)
                EditorGUILayout.LabelField("Warning !!!", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

           
            if (currentMatSelected.intValue == i){
                EditorGUILayout.BeginVertical();
                foreach (MeshRenderer child in ChildrenMesh)
                {               // Find all the different materials
                    if (child.GetComponent<Renderer>().sharedMaterial == Tmp_list_Materials_2[i] &&
                        child.gameObject.name != "Combine_" + Tmp_list_Materials_2[i].name){
                       
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(60));
                        if (GUILayout.Button("", GUILayout.Width(20)))
                        {
                            Selection.activeGameObject = child.gameObject;
                        }
                        EditorGUILayout.LabelField(child.name + " : " + child.GetComponent<MeshFilter>().sharedMesh.vertexCount);
                        EditorGUILayout.EndHorizontal();
                    }
                        
                }
                EditorGUILayout.EndVertical();
            }
        }

        bool AlloWCombine = true;
        for (int i = 0; i < Tmp_list_Vertices_2.Count; i++)
        {
            if(Tmp_list_Vertices_2[i] > _vertexCountMax.intValue ){
                AlloWCombine = false;
                break;
            }
        }

        if (AlloWCombine)
            b_CombineAvailable.boolValue = true;
        else
            b_CombineAvailable.boolValue = false;
    }
}
#endif