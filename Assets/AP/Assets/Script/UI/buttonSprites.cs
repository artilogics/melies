using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonSprites : 
	MonoBehaviour, 	
	IDragHandler,
	IPointerUpHandler,
	IPointerDownHandler,
	IPointerEnterHandler,
	IPointerExitHandler{

	public Sprite 			buttonOver;
	public Sprite 			buttonSelected;
	public Sprite 			buttonClick;
	public Sprite 			buttonDefault;


	public AudioClip 		clickDown;
	public AudioClip 		audioEnter;


	private buttonSprites	_buttonSprites;
	private Image 			btn;

	void Start(){
		_buttonSprites  = gameObject.GetComponent<buttonSprites> ();
		btn = gameObject.GetComponent<Image> ();
	}


	public virtual void OnPointerDown(PointerEventData ped){
		if (_buttonSprites) {
			if(_buttonSprites.buttonOver)btn.sprite = _buttonSprites.buttonOver;
			if (_buttonSprites.clickDown) {
				if (ingameGlobalManager.instance.audioMenu)
					ingameGlobalManager.instance.audioMenu.clip = _buttonSprites.clickDown;
					ingameGlobalManager.instance.audioMenu.Play ();
			}
		}
	}

	public virtual void OnPointerUp(PointerEventData ped){
		if (_buttonSprites) {
			if(_buttonSprites.buttonDefault)btn.sprite = _buttonSprites.buttonDefault;
		}
	}

	public virtual void OnDrag(PointerEventData ped){

	}

	public virtual void OnPointerEnter(PointerEventData ped){
		//Debug.Log ("Enter");
		if (_buttonSprites) {
			if(_buttonSprites.buttonOver)btn.sprite = _buttonSprites.buttonOver;
		}
	}

	public virtual void OnPointerExit(PointerEventData ped){
		//Debug.Log ("Exit");
		if (_buttonSprites) {
			if(_buttonSprites.buttonDefault)btn.sprite = _buttonSprites.buttonDefault;
		}
	}


	public void Action(){
		Debug.Log ("Action");
	}
}
