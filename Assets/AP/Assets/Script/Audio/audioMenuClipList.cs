// Description : audioMenuClipList : Mange UI sounds
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioMenuClipList : MonoBehaviour {

	public AudioClip 	BackButton;
	public float 		BackButtonVolume = .5f;
	public AudioClip 	openGeneric;
	public float 		openGenericVolume = .5f;
	public AudioClip 	openViewer3D;
	public float 		openViewer3DVolume = .5f;
	public AudioClip 	openMultipages;
	public float 		openMultipagesVolume = .5f;
	public AudioClip 	openDiary;
	public float 		openDiaryVolume = .5f;
	public AudioClip 	openInventory;
	public float 		openInventoryVolume = .5f;
	public AudioSource _source;

	void Start(){
		
	}

	public void playASound(int _number){
		if (_source.isPlaying)
			_source.Stop ();

		AudioClip newClip = null;

		if (_number == 0 ) {
			newClip = BackButton;
			_source.volume = BackButtonVolume;
		}
		if (_number == 1) {
			newClip = openGeneric;
			_source.volume = openGenericVolume;
		}
		if (_number == 2) {
			newClip = openViewer3D;
			_source.volume = openViewer3DVolume;
		}
		if (_number == 3) {
			newClip = openMultipages;
			_source.volume = openMultipagesVolume;
		}
		if (_number == 4) {
			newClip = openDiary;
			_source.volume = openMultipagesVolume;
		}
		if (_number == 5) {
			newClip = openInventory;
			_source.volume = openMultipagesVolume;
		}

		if (newClip) {
			_source.clip = newClip;
			_source.Play ();
		}
	}
}
