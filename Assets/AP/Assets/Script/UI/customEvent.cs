using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class customEvent : 
		MonoBehaviour,
		IDragHandler,
		IPointerUpHandler,
		IPointerDownHandler,
		IPointerEnterHandler,
		IPointerExitHandler{

	public buttonSprites _buttonSprites;
	public Image btn;

	void Start(){
		_buttonSprites  = gameObject.GetComponent<buttonSprites> ();
		btn = gameObject.GetComponent<Image> ();
	}


	public virtual void OnPointerDown(PointerEventData ped){
		if (_buttonSprites) {
			btn.sprite = _buttonSprites.buttonOver;
		}
	}

	public virtual void OnPointerUp(PointerEventData ped){
		if (_buttonSprites) {
			btn.sprite = _buttonSprites.buttonDefault;
		}
	}

	public virtual void OnDrag(PointerEventData ped){

	}

	public virtual void OnPointerEnter(PointerEventData ped){
		Debug.Log ("Enter");
		if (_buttonSprites) {
			btn.sprite = _buttonSprites.buttonOver;
		}
	}

	public virtual void OnPointerExit(PointerEventData ped){
		Debug.Log ("Exit");
		if (_buttonSprites) {
			btn.sprite = _buttonSprites.buttonDefault;
		}
	}
}
