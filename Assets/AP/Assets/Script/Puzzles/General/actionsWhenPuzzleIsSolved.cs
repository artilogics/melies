// Description : actionsWhenPuzzleIsSolved : use inpuzzle to do actions when the puzzle is solved
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionsWhenPuzzleIsSolved : MonoBehaviour {
    public bool                     SeeInspector = false;
    public bool                     onlyFocusMode = false;

    public GameObject               playerCamera;
    public GameObject               feedbackCamera;
    private GameObject              iconResetPuzzle;
    private GameObject              iconMobile_ExitPuzzle;


    public bool                     b_actionsWhenPuzzleIsSolved = false;

    public objTranslateRotate       objTranslationOrRotation;


    [System.Serializable]
    public class ListOfEvent
    {
        public float                duration = 0;
        public GameObject           feedbackCamera;
        public GameObject           objChangeScale;
        public objTranslateRotate   objTranslationOrRotation;
        public AnimationCurve       animCurve = new AnimationCurve();
        public Vector3              objScale = new Vector3(0,0,0);
    }

    public List<ListOfEvent>        listOfEvent = new List<ListOfEvent>() { new ListOfEvent() };    // List of Event when the puzzle is solved

    public List<EditorMethodsList.MethodsList> methodsList                                          // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList.MethodsList>();

    public CallMethods              callMethods;                                                    // Access script taht allow to call public function in this script.

    public float                    popupSpeed = 1;

    public AudioClip                a_puzzleSolved;
    public float                    a_puzzleSolvedVolume = .25f;
    private AudioSource             a_Source;

    public GameObject               objectActivatedWhenPuzzleIsSolved;                              // If the gameobject is activated, the puzzle is solved


	private void Start()
	{ 
        StartCoroutine(InitRoutine());
    }

  

	public void F_PuzzleSolved(){
        StartCoroutine(I_PuzzleSolved());
    }

    private IEnumerator I_PuzzleSolved()
    {

        if (a_Source && a_puzzleSolved)
        {
            a_Source.clip = a_puzzleSolved;
            a_Source.volume = a_puzzleSolvedVolume;
            a_Source.Play();
        }


        // Activate this object when the puzzle is solved. This object can't use to know if a door, drawer or wardrobe can't be open 
        if (objectActivatedWhenPuzzleIsSolved)
            objectActivatedWhenPuzzleIsSolved.SetActive(true);


        //--> Display available actions on screen
        ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);
        b_actionsWhenPuzzleIsSolved = true;
        Cursor.visible = false;
        if (iconResetPuzzle) iconResetPuzzle.SetActive(false);
        if (iconMobile_ExitPuzzle) iconMobile_ExitPuzzle.SetActive(false);
        GameObject tmp = GameObject.Find("Canvas_PlayerInfos");
        if (tmp){
            GameObject tmp2 = tmp.GetComponent<UIVariousFunctions>().obj_PuzzleClue;
            if (tmp2) tmp2.SetActive(false);
        }


        ingameGlobalManager.instance.b_InputIsActivated = false; // Deactivate inputs


        bool b_FakeReticuleIsActivated = false;
        if(ingameGlobalManager.instance.reticuleJoystickImage.gameObject.activeSelf){
            ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive(false);
            b_FakeReticuleIsActivated = true;
        }

        //AudioListener audioListener = playerCamera.GetComponent<AudioListener>();
        //audioListener.enabled = false;

        bool b_FeedbackCamera = false; // Check if a feedback camera has been activated

        for (var i = 0; i < listOfEvent.Count;i++){

            //-> Open a door or a drawer
            if (listOfEvent[i].objTranslationOrRotation)
                listOfEvent[i].objTranslationOrRotation.MoveObject();

            //-> Display feedback camera
            if (listOfEvent[i].feedbackCamera)
            {
                if(i == 0 ||  b_FeedbackCamera == false && playerCamera.activeSelf){
                    playerCamera.GetComponent<Camera>().enabled = false;
                    if(playerCamera.transform.GetChild(0))
                        playerCamera.transform.GetChild(0).gameObject.SetActive(false);
                    
                    if(playerCamera.transform.GetChild(1).CompareTag("a_Listener")){
                        playerCamera.transform.GetChild(1).transform.position = listOfEvent[i].feedbackCamera.transform.position;
                        playerCamera.transform.GetChild(1).transform.eulerAngles = listOfEvent[i].feedbackCamera.transform.eulerAngles;
                    }
                        
                }
                else if(listOfEvent[i - 1].feedbackCamera)
                    listOfEvent[i-1].feedbackCamera.SetActive(false);  
                
                listOfEvent[i].feedbackCamera.SetActive(true);
                b_FeedbackCamera = true;
            } 

            //-> Popup object
            if (listOfEvent[i].objChangeScale)
            {
                StartCoroutine(popupObject(listOfEvent[i].objChangeScale, i)) ;
            } 

            //-> Custom Method
            if (methodsList[i].obj != null)
            {
                callMethods.Call_A_Specific_Method(methodsList,i);

            } 

            yield return new WaitForSeconds(listOfEvent[i].duration);
        }

       
        if(listOfEvent.Count>0 && listOfEvent[listOfEvent.Count - 1].feedbackCamera)
            listOfEvent[listOfEvent.Count-1].feedbackCamera.SetActive(false);


        //audioListener.enabled = true;
        playerCamera.GetComponent<Camera>().enabled = true;
        if (playerCamera.transform.GetChild(0))
        playerCamera.transform.GetChild(0).gameObject.SetActive(true);

        if (playerCamera.transform.GetChild(1).CompareTag("a_Listener"))
        {
            playerCamera.transform.GetChild(1).transform.localPosition = Vector3.zero;
            playerCamera.transform.GetChild(1).transform.localEulerAngles = Vector3.zero;
        }

        Cursor.visible = true;
        ingameGlobalManager.instance.b_InputIsActivated = true; // Activate inputs

        //--> Display available actions on screen
        //if (ingameGlobalManager.instance.b_DesktopInputs)   // Desktop
            ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, true);
        //else        // Mobile
       //     ingameGlobalManager.instance.canvasMainMenu.GetComponent<iconsInfoInputs>().displayAvailableActionOnScreen(false, false);



        if (b_FakeReticuleIsActivated)
        {
            ingameGlobalManager.instance.reticuleJoystickImage.gameObject.SetActive(true);
            b_FakeReticuleIsActivated = false;
        }

        // Dezoom

        ingameGlobalManager.instance._backInputs.ExitFocusMode();

    }


    public IEnumerator popupObject(GameObject obj,int value){
        float timer = 0;
       // Debug.Log("Here : " + obj.name +  " : " + value);
        if (obj.GetComponent<Renderer>())
            obj.GetComponent<Renderer>().enabled = true;

        Transform[] allChildren = obj.GetComponentsInChildren<Transform>(true);

        foreach(Transform child in allChildren){
            if (child.GetComponent<Renderer>())
                child.GetComponent<Renderer>().enabled = true;
        }

        while(timer != 1){
            timer = Mathf.MoveTowards(timer,1,Time.deltaTime * popupSpeed);


            obj.transform.localScale = Vector3.MoveTowards(obj.transform.localScale, listOfEvent[value].objScale, listOfEvent[value].animCurve.Evaluate(timer));
            yield return null;
        }

        yield return null;
    }


    public bool returnactionsWhenPuzzleIsSolved()
    {
        return b_actionsWhenPuzzleIsSolved;
    }
 
    //-> Init Solved Section
    public void initSolvedSection(){
        //-> Init Popup objects
        for (var i = 0; i < listOfEvent.Count; i++)
        {
            if (listOfEvent[i].objChangeScale)
            {
                listOfEvent[i].objScale = listOfEvent[i].objChangeScale.transform.localScale;
                listOfEvent[i].objChangeScale.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }

    IEnumerator InitRoutine()
    {
        yield return new WaitUntil(() => ingameGlobalManager.instance);

        a_Source = GetComponent<AudioSource>();
        playerCamera = Camera.main.gameObject;
       // GameObject tmp = GameObject.Find("Canvas_PlayerInfos");

        iconResetPuzzle = ingameGlobalManager.instance.canvasPlayerInfos.obj_ResetPuzzle;
        iconMobile_ExitPuzzle = ingameGlobalManager.instance.canvasPlayerInfos.obj_Btn_Mobile_ExitPuzzle;
        /*   if (tmp)
           {
               GameObject tmp2 = tmp.GetComponent<UIVariousFunctions>().obj_ResetPuzzle;
               if (tmp2)
                   iconResetPuzzle = tmp2;

               tmp2 = tmp.GetComponent<UIVariousFunctions>().obj_Btn_Mobile_ExitPuzzle;
               if (tmp2)
                   iconMobile_ExitPuzzle = tmp2;                                                     // Access object iconMobile_ExitPuzzle


           }*/

        yield return null;
    }
}
