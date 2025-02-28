// Description : CanvasD : Use in canvas debug Mode : Unlock everything and complete all puzzles
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasD : MonoBehaviour {
    public bool SeeInspector = false;

    public KeyCode obj = KeyCode.F;
    public KeyCode puzzle = KeyCode.G;
    public Text     txtFeedback;


    public static CanvasD instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

    public bool b_MobileButtons;
    public GameObject btn_Puzzle;
    public GameObject btn_Objects;

    void Awake()
    {
        if (instance == null)           //Check if instance already exists
            instance = this;            //if not, set instance to this

        else if (instance != this)      //If instance already exists and it's not this:
            Destroy(gameObject);        //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(obj))
            debugObjects();

        if (Input.GetKeyDown(puzzle))
            debugPuzzle();

        if(txtFeedback){
            string tmpString = "";

            if (ingameGlobalManager.instance._P)
                tmpString += "Puzzle Automatically Solved || ";

            if (ingameGlobalManager.instance._D)
                tmpString += "Everything is unlocked";

            txtFeedback.text = tmpString;
        }
	}

    public void debugPuzzle(){
        if (ingameGlobalManager.instance._P)
            ingameGlobalManager.instance._P = false;
        else
            ingameGlobalManager.instance._P = true;
    }

    public void debugObjects()
    {
        if (ingameGlobalManager.instance._D)
            ingameGlobalManager.instance._D = false;
        else
            ingameGlobalManager.instance._D = true;
    }
}
