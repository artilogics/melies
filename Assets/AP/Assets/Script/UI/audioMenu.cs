// Description : audioMenu : Use in Audio Menu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class audioMenu : MonoBehaviour {

	public AudioMixer masterMixer;

	public Slider 	slider_Master;
	public Slider 	slider_Music;
	public Slider 	slider_Ambiance;
	public Slider 	slider_Voice;
	public Slider 	slider_Fx;


	private string[] arrAudio = new string[]{ "masterVol", "musicVol", "ambianceVol", "voiceVol", "fxVol" };

	// Se ejecuta al inicio de la escena aunque el GameObject esté inactivo.
	// Aplica los volúmenes guardados al AudioMixer antes de que suene nada.
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void ForceInitOnSceneLoad()
	{
		var menus = Resources.FindObjectsOfTypeAll<audioMenu>();
		foreach (var menu in menus)
		{
			if (menu != null && menu.masterMixer != null)
			{
				menu.StartCoroutine(menu.ApplyVolumes());
				return;
			}
		}
	}

	void Start () {
        StartCoroutine(ApplyVolumes());
	}

    public IEnumerator ApplyVolumes() {
        yield return null;

        if (masterMixer == null) yield break;

        if (PlayerPrefs.HasKey("GameVolumes"))
        {
            loadVolumes();
        }
        else {
            foreach (var param in arrAudio)
                masterMixer.SetFloat(param, 0f);
        }

        updateAudioMenu();
        saveVolumes();
    }
	

	public void updateAudioMenu () {
		for(var i = 0;i< arrAudio.Length;i++){
			float value = 0;
			bool result = masterMixer.GetFloat (arrAudio[i], out value);
			//Debug.Log (value);
			if(i == 0 && slider_Master && result)slider_Master.value 		= value;
			if(i == 1 && slider_Music && result)slider_Music.value 			= value;
			if(i == 2 && slider_Ambiance && result)slider_Ambiance.value 	= value;
			if(i == 3 && slider_Voice && result)slider_Voice.value 			= value;
			if(i == 4 && slider_Fx && result)slider_Fx.value 				= value;	
		}
	}


	public void setMasterVolume(float value){
		masterMixer.SetFloat ("masterVol", value);
		saveVolumes ();
	}
	public void setMusicVolume(float value){
		masterMixer.SetFloat ("musicVol", value);
		saveVolumes ();
	}
	public void setAmbianceVolume(float value){
		masterMixer.SetFloat ("ambianceVol", value);
		saveVolumes ();
	}
	public void setVoiceVolume(float value){
		masterMixer.SetFloat ("voiceVol", value);
		saveVolumes ();
	}
	public void setFxVolume(float value){
		masterMixer.SetFloat ("fxVol", value);
		saveVolumes ();
	}


	public void clearMixer(){
		for (var i = 0; i < arrAudio.Length; i++) {
			masterMixer.ClearFloat (arrAudio [i]);
            masterMixer.SetFloat(arrAudio[i], 0);
			slider_Master.value = 0;
			slider_Music.value = 0;
			slider_Ambiance.value = 0;
			slider_Voice.value = 0;
			slider_Fx.value = 0;
		}
		saveVolumes ();
	}


   

	private void saveVolumes(){
		string result = "";
		result += slider_Master.value + "_" + slider_Music.value + "_" + slider_Ambiance.value  + "_" +  slider_Voice.value + "_" + slider_Fx.value;
		PlayerPrefs.SetString ("GameVolumes", result);
	}

	private void loadVolumes(){
		if (PlayerPrefs.HasKey ("GameVolumes")) {
			//Debug.Log ("volume : " + PlayerPrefs.GetString ("GameVolumes"));
			string[] codes = PlayerPrefs.GetString ("GameVolumes").Split ('_');

			masterMixer.SetFloat ("masterVol", 	float.Parse(codes[0]));
			masterMixer.SetFloat ("musicVol", 	float.Parse(codes[1]));
			masterMixer.SetFloat ("ambianceVol",float.Parse(codes[2]));
			masterMixer.SetFloat ("voiceVol", 	float.Parse(codes[3]));
			masterMixer.SetFloat ("fxVol", 		float.Parse(codes[4]));
		}
	}


}
