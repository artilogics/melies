// Description : ItemDetector : Use on gameobject Detector (inside the chararcter) to detect interactable objects, item objects in the scene
// Hierarchy : Character -> Main Camera -> Detector
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDecector : MonoBehaviour {
	public  LayerMask 			myLayerMask;								// Ignore specific Layer when physic raycasting is used	
    public LayerMask myLayerMask2;                               // Ignore specific Layer when physic raycasting is used 
    public LayerMask myLayerMask3;                               // Ignore specific Layer when physic raycasting is used 

	private UIVariousFunctions 	canvas;										// Access gameobject canvas_PlayerInfos
	private	GameObject 			startPoint;									// Access camera Position
    public GameObject          ObjectInLayer13;


	void Start() {
		startPoint = gameObject.transform.parent.gameObject;				// Init starting Point
		canvas = ingameGlobalManager.instance.canvasPlayerInfos;			// Access gameobject canvas_PlayerInfos
	}


	void Update() {
		
		if(canvas == null)canvas = ingameGlobalManager.instance.canvasPlayerInfos;			// Access gameobject canvas_PlayerInfos
		if (startPoint && canvas) {
          	for (var i = 0; i < canvas.gameobjectList.Count; i++) {
				RaycastHit hit;
                //-> Check if there is an object between the camera and the targeted object. Display UI Icon Only if there is no object between camera and target object
                bool raycastObjectInLayer13 = false;

//--> Check Item Icon and Puzzle Icon Object
                if (Physics.Linecast(startPoint.transform.position, canvas.gameobjectList[i].transform.position, out hit, myLayerMask))
                {
                    //if(hit.transform.gameObject.layer == 13)
                    if (canvas.objrefList[i].CompareTag("Item") || 
                        canvas.objrefList[i].CompareTag("PuzzleIcon") || 
                        canvas.objrefList[i].CompareTag("OnlyFocusIcon"))
                    {
                        if (hit.transform.gameObject != canvas.gameobjectList[i])
                        {
                            //Debug.Log(canvas.gameobjectList[i].name + " not visible : " + hit.transform.name);
                            canvas.objVisible[i] = false;
                        }
                        else
                        {
                            //Debug.Log(canvas.gameobjectList[i].name + " is visible");
                            canvas.objVisible[i] = true;
                        }
                    }
                    if (hit.transform.gameObject.CompareTag("PlayerRayIsBlocked")){
                        raycastObjectInLayer13 = true;
                        ObjectInLayer13 = hit.transform.gameObject;
                    }

                }


//--> Check InteractObject
                    if (Physics.Linecast(startPoint.transform.position, canvas.gameobjectList[i].transform.position, out hit, myLayerMask2))
                    {
                    //Debug.Log(canvas.gameobjectList[i].name + " is visible");

                    if (canvas.objrefList[i].CompareTag("InteractObject"))
                    {
                        if (hit.transform.gameObject != canvas.gameobjectList[i])
                        {
                                //Debug.Log(canvas.gameobjectList[i].name + " not visible : " + hit.transform.name);
                                canvas.objVisible[i] = false;
                        }
                        else if(ObjectInLayer13
                               && ObjectInLayer13.GetComponent<_OrderInLayer>()
                                && canvas.gameobjectList[i].GetComponent<_OrderInLayer>()
                                && ObjectInLayer13.GetComponent<_OrderInLayer>().intOrderInLayer 
                                == canvas.gameobjectList[i].GetComponent<_OrderInLayer>().intOrderInLayer
                                //&& raycastObjectInLayer13
                                || 

                                ObjectInLayer13 == null
                               
                                || 
                               
                                !raycastObjectInLayer13
                               )
                        {
                                //Debug.Log(canvas.gameobjectList[i].name + " is visible");
                                canvas.objVisible[i] = true;
                        }
                        else{
                            canvas.objVisible[i] = false;

                        }
                    }

                    Debug.DrawLine(startPoint.transform.position, canvas.gameobjectList[i].transform.position, Color.red);

                    }

                if (Physics.Linecast(startPoint.transform.position, canvas.gameobjectList[i].transform.position, out hit, myLayerMask3))
                {
                    canvas.objVisible[i] = false;

                    Debug.DrawLine(startPoint.transform.position, canvas.gameobjectList[i].transform.position, Color.yellow);
                }

			}
            ObjectInLayer13 = null;
		}
	}


//--> Add UI button if there is an interactable on screen
	void OnTriggerStay(Collider other) {
		if (ingameGlobalManager.instance.saveAndLoadManager.b_IngameDataHasBeenLoaded) {			// Deactivate the item detector during the loading process
			for (var i = 0; i < ingameGlobalManager.instance.tagList.Count; i++) {
				//-> Check gameObject tag
				if (other.gameObject.CompareTag (ingameGlobalManager.instance.tagList [i])) {
					if (canvas) {
						//--> Case viewer 3D and Multi pages
						if(ingameGlobalManager.instance.tagList [i] == "Item")
						Viewer3D_and_MultiPages (other, i);	

						//--> Case interctive objects : Door, drawer, chest ...
						if(ingameGlobalManager.instance.tagList [i] == "InteractObject")
						interctiveObject (other, i);	

                        //--> Case Puzzles
                        if(ingameGlobalManager.instance.tagList [i] == "PuzzleIcon")
                            puzzleObject (other, i);    

                        //--> Case FocusOnly
                        if(ingameGlobalManager.instance.tagList[i] == "OnlyFocusIcon"  )
                            puzzleObject (other, i);  

					}
				}
			}
		}
	}

//--> Create a UI Icon to display the viewer 3D or the multi pages text viewer
	private void Viewer3D_and_MultiPages(Collider other,int i){
		//-> Check if the gameObject has a component TextProperties
		if (other.gameObject.GetComponent<TextProperties> ()) {
			//-> Check if the button already exist
			bool b_Exist = false; 
			for (var j = 0; j < canvas.gameobjectList.Count; j++) {
				if (canvas.gameobjectList [j] == other.gameObject){
					b_Exist = true;
					break;
				}
			}

			if (other.gameObject.GetComponent<Renderer> ().enabled && !b_Exist) {
				//Debug.Log ("Here");
				//-> Create a new Ui Button
				canvas.AutoInstantiateButton (
					other.gameObject, 
					ingameGlobalManager.instance.objRef [i],
					other.gameObject.GetComponent<TextProperties> ().managerID,
					other.gameObject.GetComponent<TextProperties> ().b_UIButtonShowTitle);
			}
		}
	}

//--> Create a UI Icon for an Interactive object (door, drawer...)
	private void interctiveObject(Collider other,int i){
		
		bool b_Exist = false; 
		for (var j = 0; j < canvas.gameobjectList.Count; j++) {
			if (canvas.gameobjectList [j] == other.gameObject){
				b_Exist = true;
				break;
			}
		}

        GameObject tmpObj = other.gameObject;
        while (!tmpObj.GetComponent<objTranslateRotate>())          // Find the parent with the script objTranslateRotate attached to it
        { tmpObj = tmpObj.transform.parent.gameObject; }

       
			
        if (tmpObj.transform
            && tmpObj.transform.GetComponent<objTranslateRotate> () 
			&& !b_Exist) {
            //Debug.Log(tmpObj.name);
			if (!ingameGlobalManager.instance.b_focusModeIsActivated) {
				
                if (other.gameObject.GetComponent<TextProperties> ()) {
					//Debug.Log ("Here la");
					canvas.AutoInstantiateButton (
                        other.gameObject, 
						ingameGlobalManager.instance.objRef [i],
                        other.gameObject.GetComponent<TextProperties> ().managerID,
                        other.gameObject.GetComponent<TextProperties> ().b_UIButtonShowTitle);
				} else {
					
					canvas.AutoInstantiateButton (
                        other.gameObject, 
						ingameGlobalManager.instance.objRef [i],
						-1,
						false);
				}
				
			} 


		}
	}



    //--> Create a puzzle Icon to display the viewer 3D or the multi pages text viewer
    private void puzzleObject(Collider other, int i)
    {
        //-> Check if the gameObject has a component TextProperties
        //if (other.gameObject.GetComponent<TextProperties>())
        //{
            //-> Check if the button already exist
            bool b_Exist = false;
            for (var j = 0; j < canvas.gameobjectList.Count; j++)
            {
                if (canvas.gameobjectList[j] == other.gameObject)
                {
                    b_Exist = true;
                    break;
                }
            }

            if (!b_Exist)
            {
                //Debug.Log ("Here");
                //-> Create a new Ui Button
                canvas.AutoInstantiateButton(
                    other.gameObject,
                    ingameGlobalManager.instance.objRef[i],
                    0,
                    false);
            }
        //}
    }



//--> Delete UI button if there is no interactable on screen
	void OnTriggerExit(Collider other) {
		for (var i = 0; i < ingameGlobalManager.instance.tagList.Count; i++) {
			if (other.gameObject.tag == ingameGlobalManager.instance.tagList [i]) {
				if (canvas) {
					canvas.AutoDestroyButton (other.gameObject);
				}
			}
		}
	}


}
