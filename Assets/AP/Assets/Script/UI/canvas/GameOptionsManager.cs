// Description : GameOptionsManager : Use when the Otpions Menu is displayed on screen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsManager : MonoBehaviour {

	public TextList 	data;
	public Text 		txtCurrentLanguage;
	public Toggle 		subtitleStateToggle;	

	// Use this for initialization
	void Start () {
		
	}

    //-> Call after the inGameGlobalManger initialisation
    public void initData(){
       data = ingameGlobalManager.instance.currentInfo;
        if (PlayerPrefs.HasKey("currentLanguage"))
        {
            int value = PlayerPrefs.GetInt("currentLanguage");
            ingameGlobalManager.instance.currentLanguage = value;
            txtCurrentLanguage.text = data.listOfLanguage[value];
        }
        else
        {
            txtCurrentLanguage.text = data.listOfLanguage[ingameGlobalManager.instance.currentLanguage];
        }

        if (PlayerPrefs.HasKey("subtitleState"))
        {
            bool subState = bool.Parse(PlayerPrefs.GetString("subtitleState"));
            ingameGlobalManager.instance.subtitlesState = subState;
            if (subtitleStateToggle)
                subtitleStateToggle.isOn = subState;
        }
        else
        {
            ingameGlobalManager.instance.subtitlesState = true;
            if (subtitleStateToggle)
                subtitleStateToggle.isOn = true;
        }
    }

	public void nextLanguage(){
		int value = ingameGlobalManager.instance.currentLanguage + 1;
		value = value % data.listOfLanguage.Count;
		ingameGlobalManager.instance.currentLanguage = value;
		txtCurrentLanguage.text = data.listOfLanguage [value];
		PlayerPrefs.SetInt ("currentLanguage", value);

		ingameGlobalManager.instance.updateUITexts ();
	}

	public void lastLanguage(){
		int value = ingameGlobalManager.instance.currentLanguage - 1;

		if(value == -1)
			value = data.listOfLanguage.Count -1;

		ingameGlobalManager.instance.currentLanguage = value;
		txtCurrentLanguage.text = data.listOfLanguage [value];
		PlayerPrefs.SetInt ("currentLanguage", value);

		ingameGlobalManager.instance.updateUITexts ();
	}

    //-> Use to display or not subtitle on screen
	public void subtitleState(bool b_state){
		PlayerPrefs.SetString ("subtitleState", b_state.ToString());
		ingameGlobalManager.instance.subtitlesState = b_state;

	}

}
