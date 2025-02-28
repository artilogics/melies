// Description : Menu_Manager.cs : Use in association with Menu_ManagerEditor.cs . Use to navigate on menu page. open and close pages
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Menu_Manager : MonoBehaviour {

	private IEnumerator 		Coroutine;
	public bool					b_Coutine = false;
	public List<CanvasGroup> 	List_GroupCanvas = new List<CanvasGroup>();
	public List<bool> 			b_MoreOptions = new List<bool>();
	public int					CurrentPage = 0;
	public bool 				b_DesktopOrMobile = true;

	public GameObject 			eventSysFirstSelectedGameObject;

	[System.Serializable]
	public class _ListOfMenuGameobject{			
		public GameObject objList;												//	gameobject you want to activate or deactivate in a specific menu page
		public bool Desktop = true;												//	if true the gameObject is activate in desktop mode
		public bool Mobile = true;												//	if true the gameObject is activate in Mobile mode


		public _ListOfMenuGameobject(){}
	}

	public List<_ListOfMenuGameobject> 	listOfMenuGameobject = new List<_ListOfMenuGameobject>();				// list of gameObject for each menu page. You could activate or deactivate them for mobile or desktop


	[System.Serializable]
	public class _List_gameObjectsByPage{			
		public List<_ListOfMenuGameobject> 	listOfMenuGameobject = new List<_ListOfMenuGameobject>();

		public _List_gameObjectsByPage(){}
	}

	public List<_List_gameObjectsByPage> 	list_gameObjectByPage = new List<_List_gameObjectsByPage>();	// list of gameObject for each menu page. You could activate or deactivate them for mobile or desktop


	// Use this for initialization
	void Start () {
		for (int i = 0; i < List_GroupCanvas.Count; i++) {
			if (List_GroupCanvas [i].gameObject.activeSelf) {												// Activate the current page
				CurrentPage = i;
				break;
			}
		}
	}
		

// --> Close G_canvas_01 and open G_canvas_02
	public IEnumerator MoveToPosition(CanvasGroup G_canvas_01, CanvasGroup G_canvas_02)
	{
		b_Coutine = true;
		G_canvas_01.interactable = false;
		G_canvas_01.blocksRaycasts = false;

		G_canvas_01.alpha = 0;

		G_canvas_01.gameObject.SetActive(false);

		G_canvas_02.gameObject.SetActive(true);
		G_canvas_02.blocksRaycasts = true;

		G_canvas_02.alpha = 1;
		G_canvas_02.interactable = true;

		b_Coutine = false;
		yield return null;
	}

// --> Button on Menu call this function to go to a new page
	public void GoToOtherPage(CanvasGroup newCanvas){
		if (!b_Coutine) {
			Coroutine = MoveToPosition (List_GroupCanvas [CurrentPage], newCanvas);

			for (int i = 0; i < List_GroupCanvas.Count; i++) {
				if (List_GroupCanvas [i] == newCanvas) {
					CurrentPage = i;
					break;
				}
			}
			StartCoroutine (Coroutine);
		}
	}

// --> Button on Menu call this function to go to a new page with his number on list
	public void GoToOtherPageWithHisNumber(int newCanvasNumber){
		if (!b_Coutine) {
			Coroutine = MoveToPosition (List_GroupCanvas [CurrentPage], List_GroupCanvas [newCanvasNumber]);

			for (int i = 0; i < List_GroupCanvas.Count; i++) {
				if (List_GroupCanvas [i] == List_GroupCanvas [newCanvasNumber]) {
					CurrentPage = i;
					break;
				}
			}
			StartCoroutine (Coroutine);
		}
	}


}


