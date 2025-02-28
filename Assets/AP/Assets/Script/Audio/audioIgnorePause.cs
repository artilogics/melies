// Description : audioIgnorePause : The sounds played with this gameObject can be played even if AudioSource is paused. Use For UI sound Ambiance and Music
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioIgnorePause : MonoBehaviour {
	public bool 		b_ignoreListenerPause = false;
	private AudioSource audioSource;


	// Use this for initialization
	void Awake () {
		audioSource = GetComponent<AudioSource> ();
		audioSource.ignoreListenerPause = b_ignoreListenerPause;	// If True : Sound is played even if the Audiosource is paused.
	}

}
