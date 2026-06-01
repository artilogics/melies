// Description : Grp_Audio : Create only one instance of this group even if a new scene is loaded with the same gameObject
// Static instance of GameManager which allows it to be accessed by any other script.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Grp_Audio : MonoBehaviour {

	public static Grp_Audio 	instance = null;

	public AudioMixer masterMixer;

	private string[] arrAudio = new string[]{ "masterVol", "musicVol", "ambianceVol", "voiceVol", "fxVol" };

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			instance = this;
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}

		if (masterMixer != null)
			StartCoroutine(ApplyVolumesNextFrame());
	}

	void Start(){
		DontDestroyOnLoad (gameObject);
	}

	// El AudioMixer en Windows ignora SetFloat() en el primer frame.
	// Esperamos un frame para garantizar que el mixer esté listo.
	private IEnumerator ApplyVolumesNextFrame()
	{
		yield return null;

		if (PlayerPrefs.HasKey("GameVolumes"))
		{
			string[] codes = PlayerPrefs.GetString("GameVolumes").Split('_');
			if (codes.Length >= arrAudio.Length)
			{
				for (int i = 0; i < arrAudio.Length; i++)
					masterMixer.SetFloat(arrAudio[i], float.Parse(codes[i], System.Globalization.CultureInfo.InvariantCulture));
			}
		}
		else
		{
			// Primera vez en este PC: todos los canales al máximo (0 dB)
			foreach (var param in arrAudio)
				masterMixer.SetFloat(param, 0f);
		}
	}

}
