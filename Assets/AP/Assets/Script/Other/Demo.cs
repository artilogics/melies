using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Demo : MonoBehaviour {

    public datasProjectManager datasPM;
    public Text UIText;

	// Use this for initialization
	void Start () {

        if(datasPM.int_CurrentDatasProjectFolder == 0){
            SceneManager.LoadScene(0, LoadSceneMode.Single);  
        }
        else{
            UIText.text = "The project needs some relevant parameters to works. " +
                "\nPlease ready the documentation section :" +
                "\n\n1-Configuring the project" +
                "\n\n<SIZE=14>Project Tab -> Assets -> AP -> Documentation -> AP_Documentation.pdf</SIZE>";
        }


	}
	
	
}
