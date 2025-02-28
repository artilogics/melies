// Description : reticuleGraphicRaycaster : Find this script on Canvas_PlayerInfos in the hierarchy. Use to detact mouse on keyboard and gamepad 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class reticuleGraphicRaycaster : MonoBehaviour
{
	public bool 			        SeeInspector = false;

	//--> Reticule
	public List<GraphicRaycaster> 	m_Raycaster = new List<GraphicRaycaster>();
	PointerEventData 				m_PointerEventData;
	EventSystem 					m_EventSystem;
	public UIVariousFunctions 	    uiVariousFunctions;

	public Transform 			    _pointer;

	public Color 				    DefaultColor = Color.white;
	public Color 				    SelectedColor = Color.red;
	public Image				    imageReticule;			
	public GameObject			    imageFakeMouse;

	public int 					    validationButtonJoystick 	= 4;

	public int 					    editorInfoCase = 0;

    private detectPuzzleClick       _detectClick;
    public LayerMask                myLayer;                                    // Raycast is done only on layer 15 : Puzzle


	void Start()
	{
        uiVariousFunctions  = gameObject.GetComponent<UIVariousFunctions>();
		m_EventSystem       = GetComponent<EventSystem>();
        _detectClick = new detectPuzzleClick();
	}


	void Update()
	{
        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            bool b_Color = false;


            //-> Raycast UI Objects
            if (uiVariousFunctions && uiVariousFunctions.iconAvailable          // Prevent bug : It is not possible to click if icon are not available on screen
                || uiVariousFunctions && ingameGlobalManager.instance.navigationList.Count > 0) // Gamepad : Allow to click on UI button         
            {
              
                
                if (imageFakeMouse && imageFakeMouse.activeSelf || !imageFakeMouse)
                {

                   
                    //Set up the new Pointer Event
                    m_PointerEventData = new PointerEventData(m_EventSystem);
                    //Set the Pointer Event Position to that of the mouse position
                    if (_pointer)
                    {
                        m_PointerEventData.position = new Vector2(_pointer.position.x, _pointer.position.y);
                    }
                    else
                        m_PointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);


                   

                    for (var i = 0; i < m_Raycaster.Count; i++)
                    {
                        //Create a list of Raycast Results
                        List<RaycastResult> results = new List<RaycastResult>();

                       
                        //Raycast using the Graphics Raycaster and mouse click position
                        m_Raycaster[i].Raycast(m_PointerEventData, results);

                        if (results.Count > 0)
                        {
                            foreach (RaycastResult result in results)
                            {
                                foreach (string _tag in ingameGlobalManager.instance.tagList)
                                {
                                    if (result.gameObject.CompareTag(_tag))
                                    {
                                        b_Color = true;
                                    }
                                }
                                if (result.gameObject.GetComponent<Button>() )
                                {
                                    b_Color = true;
                                }
                            }
                        }

                        //Check if the left Mouse button is clicked
                        if (ingameGlobalManager.instance.b_DesktopInputs)
                        {
                            if (Input.GetKeyDown(KeyCode.Mouse0)                                                                                    // Keyboard case
                                && !ingameGlobalManager.instance.b_Joystick
                                /*&& !ingameGlobalManager.instance.b_focusModeIsActivated*/
                               || Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[validationButtonJoystick])       // Joystick Case
                                && ingameGlobalManager.instance.b_Joystick
                               /* && !ingameGlobalManager.instance.b_focusModeIsActivated*/)
                            {

                                //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                                bool onlyOne = true;
                                foreach (RaycastResult result in results)
                                {

                                    if (onlyOne)
                                    {
                                        //Debug.Log("Hit : " + result.gameObject.name);

                                        if (result.gameObject.CompareTag(ingameGlobalManager.instance.tagList[0]))
                                        {       // Type : Item
                                                //Debug.Log("Item Object");
                                            result.gameObject.GetComponent<btn_Check>().VisualizeGameobject();
                                        }
                                        else if (result.gameObject.CompareTag(ingameGlobalManager.instance.tagList[4]))
                                        {       // Type : Interactive Object
                                                //Debug.Log("Interctive Object");
                                            result.gameObject.GetComponent<btn_Check>().ObjTranslateRotate();
                                        }
                                        else if (result.gameObject.CompareTag(ingameGlobalManager.instance.tagList[3]))
                                        {       // Type : Puzzle Object
                                                //Debug.Log("Puzzle Object");
                                            result.gameObject.GetComponent<btn_Puzzle>().focusPuzzle();
                                        }
                                        else if (result.gameObject.CompareTag(ingameGlobalManager.instance.tagList[5]))
                                        {       // Type : Focus Only
                                            //Debug.Log("Focus Only");
                                            result.gameObject.GetComponent<btn_Puzzle>().focusPuzzle();
                                        }
                                        else if (result.gameObject.GetComponent<Button>())
                                        {
                                            // Type : UI Buttons
                                            result.gameObject.GetComponent<Button>().onClick.Invoke();
                                        }
                                        onlyOne = false;
                                    }
                                }
                            }
                        }

                        if (ingameGlobalManager.instance.b_Joystick 
                            && ingameGlobalManager.instance.b_DesktopInputs
                            && _detectClick.F_detectPuzzleObject(myLayer, ingameGlobalManager.instance, validationButtonJoystick)
                            && ingameGlobalManager.instance.currentPuzzle != null 
                            && !ingameGlobalManager.instance.currentPuzzle.GetComponent<actionsWhenPuzzleIsSolved>().b_actionsWhenPuzzleIsSolved)
                            b_Color = true;

                       // Debug.Log("COlor");



                        if (b_Color && imageReticule && imageReticule.color == DefaultColor

                            ||

                            ingameGlobalManager.instance.b_dragAndDropActivated &&
                            b_Color && imageReticule && (imageReticule.color == DefaultColor || imageReticule.color == ingameGlobalManager.instance.currentPuzzle.GetComponent<AP_.DragAndDrop>().DefaultColor) && 
                            !ingameGlobalManager.instance.currentPuzzle.GetComponent<AP_.DragAndDrop>().puzzleObjectIsDetected 
                           )
                        {
                            imageReticule.color = SelectedColor;
                            //Debug.Log("COlor 1");
                        }
                        else if (!b_Color && imageReticule && imageReticule.color != DefaultColor && !ingameGlobalManager.instance.currentPuzzle

                                 ||

                                 !b_Color && imageReticule && imageReticule.color != DefaultColor && !ingameGlobalManager.instance.currentPuzzle.GetComponent<AP_.DragAndDrop>()

                                 ||

                                 !b_Color && imageReticule && imageReticule.color != DefaultColor && 
                                 ingameGlobalManager.instance.currentPuzzle.GetComponent<AP_.DragAndDrop>() && 
                                 (ingameGlobalManager.instance. navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "Inventory" || 
                                 ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "Diary" ||
                                  ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "DiaryMultiPages"||
                                  ingameGlobalManager.instance.navigationList[ingameGlobalManager.instance.navigationList.Count - 1] == "Clue")

                                


                                 )
                        {
                            imageReticule.color = DefaultColor;
                            //Debug.Log("COlor 2");
                        }
                    }
                }
            }
            else if (imageReticule && 
                     imageReticule.color != DefaultColor)
            {
                imageReticule.color = DefaultColor;
            }
        }
	}



  
}
