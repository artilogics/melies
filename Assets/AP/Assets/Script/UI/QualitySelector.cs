// Description : QualitySelector.cs : Allow to change global quality setiings and resolutions screen 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySelector : MonoBehaviour {

	public int 				currentResolution	= 0;
	public Text 			txt_Resolution;										// Use to display the actual resolution on screen
	public List<Button>		l_QualityButtons = new List<Button>();				// list of Quality Buttons
	public List<GameObject>	l_QualityCheckmarks = new List<GameObject>();		// list of Quality Checkmarks
	public int 				LastHeightResolution = 0;
	public int 				LastWidthResolution = 0;

// --> Init text that display the actual resolution
	void Start () {
        Resolution[] resolutions = Screen.resolutions;

		for (int i = 0; i < resolutions.Length; i++) {
			if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
				currentResolution = i;											// Init resolution value that represent the current Resolution settings
			}
		}

        if (txt_Resolution) txt_Resolution.text = resolutions[currentResolution].width + "x" + resolutions[currentResolution].height;       

		InitQualitySettingsOnScreen ();											// Init checkmark that represent the current quality settings
	}

// --> The quality settings Fastest, Fast, good, beautiful, fantastic	
	public void ChooseQualitySettings(int quality) {																																										
		QualitySettings.SetQualityLevel(quality, true);
		InitQualitySettingsOnScreen ();
	}


// --> Init Quality on screen
	public void InitQualitySettingsOnScreen() {
       // Debug.Log("Init");
       // if(txt_Resolution)txt_Resolution.text = Screen.currentResolution.width + "x" + Screen.currentResolution.height;                 
		int  tmpQuality = QualitySettings.GetQualityLevel();
		for (int i = 0; i < l_QualityCheckmarks.Count; i++) {
			if(i == tmpQuality)
				l_QualityCheckmarks [i].SetActive (true);
			else
				l_QualityCheckmarks [i].SetActive (false);
		}
	}


// --> Press button " Next_Resolution" or "Last Resolution"
	public void ChooseResolution(int value) {																		
		Resolution[] resolutions = Screen.resolutions;
		LastHeightResolution = resolutions [currentResolution].height;
		LastWidthResolution = resolutions [currentResolution].width;

		while(LastHeightResolution == resolutions[currentResolution].height || LastWidthResolution == resolutions[currentResolution].width){
			currentResolution += value;
			if (currentResolution < 0) 
				currentResolution = resolutions.Length-1;
			else
				currentResolution = currentResolution % resolutions.Length;

			if(txt_Resolution)txt_Resolution.text = resolutions[currentResolution].width + "x" + resolutions[currentResolution].height;
		}
	}
		
// --> Press button "Validate_Resolution"
	public void ValidateResolution() {																				
		Resolution[] resolutions = Screen.resolutions;
		Screen.SetResolution(resolutions[currentResolution].width,resolutions[currentResolution].height,true);
	}
}
