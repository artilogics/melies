// Description : DragAndDrop : Use in puzzle to drag and drop object (mobile, keyboard and desktop)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

namespace AP_
{
    public class DragAndDrop : MonoBehaviour
    {

        public bool SeeInspector = false;
        public int validationButtonJoystick = 4;                   // Reference to the input
        public int inventoryButtonJoystick = 0;
        public int diaryButtonJoystick = 1;
        public int inventoryButtonKeyboard = 4;
        public int diaryButtonKeyboard = 5;
        public bool b_ValidationButtonPressed = false;              // Prevent bug. Only one click available

        public bool puzzleObjectIsDetected = false;                 // Know if an object from the puzzle is detected

        public List<SpriteRenderer> listOfSelectedPuzzlePosition = new List<SpriteRenderer>();      // list of the PuzzleRefPosition. The position where an object could be drop


        public Transform ReticuleJoystick;                                // Joystick fake Mouse              

        public Color DefaultColor = Color.white;
        public Color SelectedColor = Color.red;
        public Image ImageObjDetected;
        public GameObject Grp_ImageFakeMouse;

        public LayerMask myLayer;                                        // Layer : Default. Use to raycast objects that needed to move. Object with Tag : Puzzle Object 
        public LayerMask myLayer2;                                       // Layer : PuzzleFeedbackCam. Use to detect the PuzzleRefPosition. Tag : PuzzleRefPosition
        public bool puzzleObjectDetectionIsActivated = false;       // Allow object detection if True    

        public GameObject currentSelectedGameObject;                      // The current selected object
        public GameObject lastSelectedGameObject;                         // Remember the last select object

        public GameObject currentSelectedPuzzlePosition;                  // The current selected position
        public GameObject lastSelectedPuzzlePosition;                     // Remember the last select position
        private int fingerNum = 0;                                  // Mobile : know which finger is pressed on screen

        public AudioSource a_TakeObject;                                   // Play audio when object is taken or released
        public Vector3 currentTargetObjectPosition = Vector3.zero;

        public float distanceFromTheCamera = .45f;                   // Tweak the distance between focus cam and puzzle Objects

        public GameObject refDragAndDropEulerAngle;                       // When an object is drag on screen, the object use refDragAndDropEulerAngle eulerAngle


        public float raycastDistance = 100;

        public List<GameObject> listOfHandsObj = new List<GameObject>();
        public List<GameObject> listOfGearsLogicsObj = new List<GameObject>();

        void Start()
        {
            initDragAndDrop();

            foreach (Transform child in transform)
            {
                if (child.name == "a_TakeObject")
                    a_TakeObject = child.GetComponent<AudioSource>();
            }
        }

        //--> Script Initialisation
        private void initDragAndDrop()
        {
            GameObject Grp_canvas = GameObject.Find("Grp_Canvas");
            Transform[] allTransform = Grp_canvas.GetComponentsInChildren<Transform>(true);

            UIVariousFunctions canvas_PlayerInfos = null;

            foreach (Transform obj in allTransform)
            {
                if (obj.name == "Canvas_PlayerInfos")
                    canvas_PlayerInfos = obj.gameObject.GetComponent<UIVariousFunctions>();   // Access to the UIVariousFunctions script
            }



            if (canvas_PlayerInfos)
            {
                if (canvas_PlayerInfos.ReticuleJoystick)
                    ReticuleJoystick = canvas_PlayerInfos.ReticuleJoystick;         // Access joystick reticule

                if (canvas_PlayerInfos.Grp_ImageFakeMouse)
                    Grp_ImageFakeMouse = canvas_PlayerInfos.Grp_ImageFakeMouse;     // Access joystick fake mouse 

                if (canvas_PlayerInfos.Image_ObjDetected)
                    ImageObjDetected = canvas_PlayerInfos.Image_ObjDetected;        //  Access Joystick fake mouse 
            }

        }


        //--> Call from other scrit that need to use drag and drop
        public void F_DragAndDrop(List<SpriteRenderer> puzzleListOfSelectedPuzzlePosition)
        {
            if (listOfSelectedPuzzlePosition.Count == 0)                            // Once : Init listOfSelectedPuzzlePosition                                  
                listOfSelectedPuzzlePosition = puzzleListOfSelectedPuzzlePosition;


            if (ingameGlobalManager.instance.b_InputIsActivated)                    // Check if input is activated
            {
                bool b_Wait = false;
                if (puzzleObjectDetectionIsActivated)                               // Puzzle detection is activated
                {
                    //-> Object stop to follow the mouse
                    if (ingameGlobalManager.instance.b_Joystick && ingameGlobalManager.instance.b_DesktopInputs)
                    {    // Joystick Case
                        if ((Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[validationButtonJoystick]) && b_ValidationButtonPressed )
                            ||
                            Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[inventoryButtonJoystick])
                            ||
                            Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[diaryButtonJoystick])
                            )
                        {
                            b_ValidationButtonPressed = false;
                            StartCoroutine(WaitBeforeDeselect(true));
                            //DeselectObject(true);
                            b_Wait = true;
                        }
                    }


                    if (!b_Wait)
                    {
                        bool b_PuzzleRefPosition = false;

                        //-> Raycast object in the Scene View. 
                        if (ReticuleJoystick && ingameGlobalManager.instance.b_Joystick && ingameGlobalManager.instance.b_DesktopInputs)    // Joystick Case
                            CheckObjectWithTag_Joystick(b_PuzzleRefPosition);
                        else if (!ingameGlobalManager.instance.b_Joystick && ingameGlobalManager.instance.b_DesktopInputs)                  // Keyboard Case
                            CheckObjectWithTag_Keyboard(b_PuzzleRefPosition, b_Wait);
                        else
                            CheckObjectWithTag_Mobile(b_PuzzleRefPosition, b_Wait);                                                         // Mobile Case


                        //-> Update the reticule color (Joystick case)
                        updateReticuleColor();
                    }
                }

                //--> Object reach the current target position
                if (currentSelectedGameObject && b_ValidationButtonPressed)
                {
                    currentSelectedGameObject.transform.position = Vector3.MoveTowards(currentSelectedGameObject.transform.position, currentTargetObjectPosition, 4 * Time.deltaTime);
                    //currentSelectedGameObject.transform.localPosition = new Vector3(currentSelectedGameObject.transform.localPosition.x, currentSelectedGameObject.transform.localPosition.y, 0);
                    currentSelectedGameObject.transform.eulerAngles = refDragAndDropEulerAngle.transform.eulerAngles;
                }

            }

        }


        //--> Update the reticule color (Joystick case)
        private void updateReticuleColor()
        {
            //-> Raycast UI Objects
            if (puzzleObjectIsDetected)
            {
                ImageObjDetected.color = SelectedColor;
                //Debug.Log("Here");
            }
            else if (!puzzleObjectIsDetected && ImageObjDetected && ImageObjDetected.color != DefaultColor)
            {
                ImageObjDetected.color = DefaultColor;
            }

        }

        public IEnumerator WaitBeforeDeselect(bool b_PlaySound)
        {
            if (currentSelectedGameObject && currentSelectedPuzzlePosition)
                currentSelectedGameObject.transform.position = currentSelectedPuzzlePosition.transform.GetChild(0).position;

            GameObject refObj = currentSelectedPuzzlePosition;

            if(currentSelectedPuzzlePosition){
                for (var i = 0;i< 3;i++)
                    yield return new WaitForFixedUpdate();
                
                //yield return new WaitForSeconds(.1f); 
            }

            DeselectObject(b_PlaySound,refObj);
        }


        //--> Deselect the current selected object
        public void DeselectObject(bool b_PlaySound,GameObject refAxis)
        {
            StartCoroutine(DeselectRoutine(b_PlaySound, refAxis));
           
        }


        public bool isDeselected = true;
        IEnumerator DeselectRoutine(bool b_PlaySound, GameObject refAxis)
        {
            b_ValidationButtonPressed = false;
            isDeselected = false;
            // if(currentSelectedGameObject)
            //Debug.Log("WTF 1 " + currentSelectedGameObject.GetComponent<GearCheckCollision>().returnCheckCollision());

            //-> Selected object go to his inital position
            if (!refAxis
                ||
                currentSelectedGameObject &&
                currentSelectedGameObject.GetComponent<GearCheckCollision>() &&
                currentSelectedGameObject.GetComponent<GearCheckCollision>().returnCheckCollision())
            {
                // -> Case : Gear Puzzle
                if (currentSelectedGameObject &&
                   currentSelectedGameObject.GetComponent<GearCheckCollision>())
                {
                    currentSelectedGameObject.GetComponent<GearCheckCollision>().b_CollisionWithOtherGear = false;
                   // Debug.Log("Here 0");
                    yield return new WaitUntil(() => !currentSelectedGameObject.GetComponent<GearCheckCollision>().b_CollisionWithOtherGear);
                   
                }

                if (currentSelectedGameObject)
                {                 // init position to the currentSelectedGameObject position    
                    currentSelectedGameObject.transform.position = currentSelectedGameObject.transform.parent.position;
                    currentSelectedGameObject.transform.rotation = currentSelectedGameObject.transform.parent.transform.rotation;
                    //Debug.Log("Here 1a");
                    yield return new WaitUntil(() => currentSelectedGameObject.transform.position == currentSelectedGameObject.transform.parent.position);
                    // Debug.Log("Here 1b");
                     yield return new WaitUntil(() => currentSelectedGameObject.transform.rotation == currentSelectedGameObject.transform.parent.transform.rotation);

                    //

                }
                /*  else if (lastSelectedGameObject)                 // init position to the lastSelectedGameObject position
                  {
                      lastSelectedGameObject.transform.position = currentSelectedGameObject.transform.parent.position;
                      //lastSelectedGameObject.transform.eulerAngles = lastSelectedGameObject.transform.parent.transform.eulerAngles;
                      lastSelectedGameObject = null;
                       //Debug.Log("Here 2");
                  }*/
                // Debug.Log("Here 2");
            }
            //-> The selected go to the  currentSelectedGameObject or lastSelectedGameObject
            else
            {
                if (currentSelectedGameObject)
                {
                    currentSelectedGameObject.transform.position = refAxis.transform.GetChild(0).position;
                    //Debug.Log("Here 3");
                    yield return new WaitUntil(() => currentSelectedGameObject.transform.position == refAxis.transform.GetChild(0).position);
                    //currentSelectedGameObject.transform.eulerAngles = currentSelectedGameObject.transform.eulerAngles;
                    // Debug.Log("Here 3");
                }
                /*   else if (lastSelectedGameObject)
                   {
                       lastSelectedGameObject.transform.position = lastSelectedPuzzlePosition.transform.GetChild(0).position;
                      // lastSelectedGameObject.transform.eulerAngles = lastSelectedGameObject.transform.eulerAngles;
                       lastSelectedGameObject = null;
                       Debug.Log("Here 4");
                   }*/

            }


            if (currentSelectedGameObject && currentSelectedGameObject.transform.GetComponent<MeshCollider>())
            {
                currentSelectedGameObject.transform.GetComponent<MeshCollider>().convex = true;
                currentSelectedGameObject.transform.GetComponent<MeshCollider>().isTrigger = true;
                currentSelectedGameObject.transform.GetComponent<Rigidbody>().isKinematic = false;
               // Debug.Log("Here 4a");
                yield return new WaitUntil(() => currentSelectedGameObject.transform.GetComponent<MeshCollider>().convex);
               // Debug.Log("Here 4b");
                yield return new WaitUntil(() => currentSelectedGameObject.transform.GetComponent<MeshCollider>().isTrigger);
                //Debug.Log("Here 4c");
                yield return new WaitUntil(() => !currentSelectedGameObject.transform.GetComponent<Rigidbody>().isKinematic);
            }

            currentSelectedGameObject = null;

            if (a_TakeObject && b_PlaySound) a_TakeObject.Play();              // Play a sound

           // yield return new WaitForSeconds(1);
            currentSelectedGameObject = null;
            //b_Wait = true;
            isDeselected = true;
            yield return null;
        }


        //--> Joystick Case : Detect object to drag
        private void CheckObjectWithTag_Joystick(bool b_PuzzleRefPosition)
        {
            if (ReticuleJoystick && isDeselected)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(ReticuleJoystick.position.x, ReticuleJoystick.position.y, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastDistance, myLayer))
                {
                    if (!b_ValidationButtonPressed)
                    {
                        if (GearsLogicsInTheList((hit.transform.gameObject)) && 
                            hit.transform.gameObject.CompareTag("PuzzleObject"))
                        {
                            if (Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[validationButtonJoystick])
                                && !b_ValidationButtonPressed)
                            {
                                //Debug.Log("Joystick Validation is pressed");
                                b_ValidationButtonPressed = true;
                                if (a_TakeObject) a_TakeObject.Play();
                            }

                            puzzleObjectIsDetected = true;

                            //-> An object is selected. 
                            if (b_ValidationButtonPressed && hit.transform.gameObject.CompareTag("PuzzleObject") && !hit.transform.GetComponent<Rigidbody>().isKinematic)
                            {
                                if (currentSelectedGameObject != hit.transform.gameObject)
                                    currentSelectedGameObject = hit.transform.gameObject;

                                if (hit.transform.GetComponent<MeshCollider>())
                                {
                                    hit.transform.GetComponent<MeshCollider>().isTrigger = false;
                                    hit.transform.GetComponent<MeshCollider>().convex = false;
                                    hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                                }
                            }
                        }
                        else if (puzzleObjectIsDetected)
                        {
                            puzzleObjectIsDetected = false;
                            if (currentSelectedGameObject != null)
                            {
                                lastSelectedGameObject = currentSelectedGameObject;
                               // currentSelectedGameObject = null;
                            }
                        }
                    }

                }

                Vector3 temp = ReticuleJoystick.position;
                temp.z = distanceFromTheCamera;
                //-> Follow object
                if (b_ValidationButtonPressed)
                {
                    currentTargetObjectPosition = Camera.main.ScreenToWorldPoint(temp);
                }

                //-> Check if Puzzle ref position is detected
                RaycastHit hit2;
                if (Physics.Raycast(ray, out hit2, raycastDistance, myLayer2))
                {
                    if (hit2.transform.gameObject.CompareTag("PuzzleRefPosition"))
                    {
                        if (hit2.transform.gameObject.GetComponent<SpriteRenderer>() && HandInTheList(hit2.transform.gameObject))
                        {
                            SpriteRenderer _currentHit = hit2.transform.gameObject.GetComponent<SpriteRenderer>();

                            if (_currentHit.color.a == 0 && currentSelectedGameObject != null)
                            {
                                _currentHit.color = new Color(_currentHit.color.r, _currentHit.color.g, _currentHit.color.b, 0.5f);
                                currentSelectedPuzzlePosition = hit2.transform.gameObject;
                            }
                            b_PuzzleRefPosition = true;
                        }
                    }
                }
            }

            saveTheLastSelectedPuzzlePosition(b_PuzzleRefPosition);
            initSelectedPuzzlePositionSPriteColor();
        }


        //--> Keyboard Case : Detect object to drag
        private void CheckObjectWithTag_Keyboard(bool b_PuzzleRefPosition, bool b_Wait)
        {
            if (ReticuleJoystick)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, raycastDistance, myLayer))
                {
                    if (GearsLogicsInTheList((hit.transform.gameObject)) && 
                        hit.transform.gameObject.CompareTag("PuzzleObject") && 
                        !b_ValidationButtonPressed)
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            //Debug.Log("Joystick Validation is pressed");
                            b_ValidationButtonPressed = true;
                            if (a_TakeObject) a_TakeObject.Play();
                        }

                        puzzleObjectIsDetected = true;

                        //-> An object is selected. 
                        if (b_ValidationButtonPressed && hit.transform.gameObject.CompareTag("PuzzleObject"))
                        {
                            if (currentSelectedGameObject != hit.transform.gameObject && !hit.transform.GetComponent<Rigidbody>().isKinematic)
                            {
                                currentTargetObjectPosition = hit.transform.gameObject.transform.position + new Vector3(0, .1f, 0);
                                currentSelectedGameObject = hit.transform.gameObject;


                                if (hit.transform.GetComponent<MeshCollider>())
                                {
                                    hit.transform.GetComponent<MeshCollider>().isTrigger = false;
                                    hit.transform.GetComponent<MeshCollider>().convex = false;
                                    hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                                }
                            }
                        }
                    }
                }


                Vector3 temp = Input.mousePosition;
                temp.z = distanceFromTheCamera;

                if (b_ValidationButtonPressed)
                {
                    currentTargetObjectPosition = Camera.main.ScreenToWorldPoint(temp);
                }

                if ((Input.GetKeyUp(KeyCode.Mouse0) && b_ValidationButtonPressed == true)
                    ||
                    Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[inventoryButtonKeyboard])
                    ||
                    Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[diaryButtonKeyboard])
                    )
                {
                   // Debug.Log("Deselect");
                    b_ValidationButtonPressed = false;

                    StartCoroutine(WaitBeforeDeselect(true));
                    //DeselectObject(true);
                }


                //-> Check if Puzzle ref position is detected
                RaycastHit hit2;
                if (Physics.Raycast(ray, out hit2, raycastDistance, myLayer2))
                {
                    if (hit2.transform.gameObject.CompareTag("PuzzleRefPosition"))
                    {
                        if (hit2.transform.gameObject.GetComponent<SpriteRenderer>() && HandInTheList(hit2.transform.gameObject))
                        {
                            SpriteRenderer _currentHit = hit2.transform.gameObject.GetComponent<SpriteRenderer>();

                            if (_currentHit.color.a == 0/* && currentSelectedGameObject != null*/)
                            {
                                //Debug.Log("Ray");
                                _currentHit.color = new Color(_currentHit.color.r, _currentHit.color.g, _currentHit.color.b, 0.5f);
                                currentSelectedPuzzlePosition = hit2.transform.gameObject;
                            }
                            b_PuzzleRefPosition = true;
                        }
                    }
                    else
                    {
                        b_PuzzleRefPosition = false;
                    }
                }
            }

            saveTheLastSelectedPuzzlePosition(b_PuzzleRefPosition);
            initSelectedPuzzlePositionSPriteColor();
        }

        //--> Mobile Case : Detect object to drag 
        private void CheckObjectWithTag_Mobile(bool b_PuzzleRefPosition, bool b_Wait)
        {
            if (ReticuleJoystick)
            {
                // Debug.Log("Touch Count :" + Input.touchCount);

                //prevent bug
                if (Input.touchCount == 0 && b_ValidationButtonPressed)
                {
                    fingerNum = 0;
                    b_ValidationButtonPressed = false;
                    //DeselectObject(true);
                    StartCoroutine(WaitBeforeDeselect(true));
                }


                for (int i = 0; i < Input.touchCount; ++i)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, raycastDistance, myLayer))
                        {
                            if (GearsLogicsInTheList((hit.transform.gameObject)) &&
                            hit.transform.gameObject.CompareTag("PuzzleObject") && 
                            !b_ValidationButtonPressed)
                            {
                                b_ValidationButtonPressed = true;

                                fingerNum = i;
                                if (a_TakeObject) a_TakeObject.Play();

                                puzzleObjectIsDetected = true;

                                //-> An object is selected. 
                                if (b_ValidationButtonPressed && 
                                hit.transform.gameObject.CompareTag("PuzzleObject") && 
                                !hit.transform.GetComponent<Rigidbody>().isKinematic)
                                {
                                    if (currentSelectedGameObject != hit.transform.gameObject)
                                        currentSelectedGameObject = hit.transform.gameObject;

                                    if (hit.transform.GetComponent<MeshCollider>())
                                    {
                                        hit.transform.GetComponent<MeshCollider>().isTrigger = false;
                                        hit.transform.GetComponent<MeshCollider>().convex = false;
                                        hit.transform.GetComponent<Rigidbody>().isKinematic = true;
                                    }
                                }
                            }
                        }
                    }


                    if (fingerNum == i)
                    {
                        Vector3 temp = Input.GetTouch(i).position;
                        temp.z = distanceFromTheCamera;
                        //-> Follow object
                        if (b_ValidationButtonPressed)
                        {
                            currentTargetObjectPosition = Camera.main.ScreenToWorldPoint(temp);
                        }

                        //-> Follow object
                        if (Input.GetTouch(i).phase == TouchPhase.Moved && b_ValidationButtonPressed)
                        {
                            Ray ray2 = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            RaycastHit hit3;
                            if (Physics.Raycast(ray2, out hit3, raycastDistance, myLayer))
                            {
                                currentSelectedGameObject.transform.position = hit3.point;
                            }
                        }

                        if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            fingerNum = 0;
                            b_ValidationButtonPressed = false;
                            StartCoroutine(WaitBeforeDeselect(true));
                            //DeselectObject(true);
                        }



                        //if (fingerNum == i)
                        //{
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                        RaycastHit hit2;
                        if (Physics.Raycast(ray, out hit2, raycastDistance, myLayer2))
                        {

                            if (hit2.transform.gameObject.CompareTag("PuzzleRefPosition"))
                            {

                                if (hit2.transform.gameObject.GetComponent<SpriteRenderer>() && HandInTheList(hit2.transform.gameObject))
                                {

                                    SpriteRenderer _currentHit = hit2.transform.gameObject.GetComponent<SpriteRenderer>();

                                    if (_currentHit.color.a == 0 && currentSelectedGameObject != null)
                                    {
                                        //Debug.Log("Ray");
                                        _currentHit.color = new Color(_currentHit.color.r, _currentHit.color.g, _currentHit.color.b, 0.5f);
                                        currentSelectedPuzzlePosition = hit2.transform.gameObject;
                                    }
                                    b_PuzzleRefPosition = true;
                                }
                            }
                            else
                            {
                                b_PuzzleRefPosition = false;
                            }
                        }
                    }
                }
            }



            saveTheLastSelectedPuzzlePosition(b_PuzzleRefPosition);
            initSelectedPuzzlePositionSPriteColor();
        }

        //-> Save the last Selectected Puzzle Position
        private void saveTheLastSelectedPuzzlePosition(bool b_PuzzleRefPosition)
        {
            if (!b_PuzzleRefPosition)
            {
                lastSelectedPuzzlePosition = currentSelectedPuzzlePosition;
                currentSelectedPuzzlePosition = null;
            }
        }

        //-> Init the sprite alpha for the last Selected Puzzle position
        public void initSelectedPuzzlePositionSPriteColor()
        {

            if (currentSelectedPuzzlePosition == null && lastSelectedPuzzlePosition)
            {
                lastSelectedPuzzlePosition.GetComponent<SpriteRenderer>().color =
                    new Color(lastSelectedPuzzlePosition.GetComponent<SpriteRenderer>().color.r,
                              lastSelectedPuzzlePosition.GetComponent<SpriteRenderer>().color.g,
                              lastSelectedPuzzlePosition.GetComponent<SpriteRenderer>().color.b, 0f);
            }

            foreach (SpriteRenderer rend in listOfSelectedPuzzlePosition)
            {
                if (currentSelectedPuzzlePosition != null &&
                   rend.gameObject != currentSelectedPuzzlePosition)
                {
                    rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
                }
            }
        }

        // init all the sprite when puzzle is solved
        public void initAllSpriteWhenPuzzleIsSolved()
        {
            foreach (SpriteRenderer rend in listOfSelectedPuzzlePosition)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
            }
        }




        private Vector3 returnNewPosition(Vector3 pos)
        {
            Vector3 result = new Vector3(pos.x, .1f, pos.z);
            return result;
        }


        public GameObject returnCurrentSelectedObject()
        {
            return currentSelectedGameObject;
        }

        public void defaultColor() { ImageObjDetected.color = DefaultColor; }

        public void InitListOfHands(List<GameObject> listFromThePuzzle)
        {
            foreach (GameObject obj in listFromThePuzzle)
                listOfHandsObj.Add(obj);
        }


        bool HandInTheList(GameObject objToTest)
        {
            foreach (GameObject obj in listOfHandsObj)
            {
                if (obj == objToTest)
                    return true;
            }
            return false;
        }

        public void InitListOfGearsLogics(List<GameObject> listFromThePuzzle)
        {
            foreach (GameObject obj in listFromThePuzzle)
                listOfGearsLogicsObj.Add(obj);
        }


        bool GearsLogicsInTheList(GameObject objToTest)
        {
            foreach (GameObject obj in listOfGearsLogicsObj)
            {
                if (obj == objToTest)
                    return true;
            }
            return false;
        }

        public void InitHandsWhenThePlayerLeavesPuzzleFocusMode()
        {
            #region
            foreach (GameObject obj in listOfHandsObj)
            {
                SpriteRenderer objRenderer = obj.GetComponent<SpriteRenderer>();
                objRenderer.color = new Color(objRenderer.color.r, objRenderer.color.g, objRenderer.color.b, 0f);
            }
            #endregion
        }
    }
}
