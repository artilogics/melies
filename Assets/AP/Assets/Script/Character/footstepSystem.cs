//Description : footstepSystem. Allow to manage player foostep sound depending the surface
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footstepSystem : MonoBehaviour {
	public 	bool 			        SeeInspector = true;
	public  LayerMask 		        myLayerMask;							// Ignore specific Layer when physic raycasting is used	
	public 	Rigidbody		        rb;
	public	AudioSource		        _audio;

	[System.Serializable]
	public class Footsteps{
		public List<AudioClip> footstepSamples;
		public string 			MaterialTag = "";
	}
		
	public List<Footsteps>          listFootstepSystem = new  List<Footsteps>();


	[System.Serializable]
	public class compareCharacterMagnitude{
		public float listTimeBetweenTwoStep = .3f;
		public float listCharacterMangnitude = .4f;
	}


	public List<compareCharacterMagnitude> listCompareMangnitude = new  List<compareCharacterMagnitude>();

	private float                   _Timer = 0;
	private Vector3                 lastPos = new Vector3(0,0,0);
	private Vector3                 bodyVelocity = new Vector3(0,0,0);

	private int                     currentFootstepType =0;
	private int                     currentSample = 0;

    private characterMovement charMovement;

	private bool lastIsOnFloor = false;
	public AudioClip aFall;
	public bool bFirstTimeTouchFloor = true;    // Prevent playing the sound when the scene is loaded.

	// Use this for initialization
	void Start () {
        charMovement = GetComponent<characterMovement>();
	}

	void Update()
	{
		checkEndFallSound();
	}

	// Update is called once per frame
	void FixedUpdate () {
		RaycastHit hit;

		if (Physics.Raycast (rb.transform.position+ Vector3.up*.2f, -Vector3.up, out hit, 10, myLayerMask)) {
            if(charMovement && charMovement.isOnFloor)
			    playFootstep (hit.transform.tag);
            else if(!charMovement)
                playFootstep(hit.transform.tag);
		}
	}

	private void playFootstep(string _tag){
		if (CheckTag (_tag)) {
			for (var i = 0; i < listCompareMangnitude.Count; i++) {
				if (bodyVelocity.magnitude > listCompareMangnitude[i].listCharacterMangnitude) {
					if (!_audio.isPlaying) {
                        float tmpTimeBetweenTwoSteps = listCompareMangnitude[i].listTimeBetweenTwoStep;
                        if (charMovement && charMovement.isRunning)
                            tmpTimeBetweenTwoSteps = listCompareMangnitude[i].listTimeBetweenTwoStep - .2f;

                        if (_Timer == tmpTimeBetweenTwoSteps) {
						
							playSound (ChooseSound ());
							_Timer = 0;
						} else {
                            _Timer = Mathf.MoveTowards (_Timer, tmpTimeBetweenTwoSteps, Time.deltaTime);
						}
					}
				}
			}
		}

		bodyVelocity = (rb.position - lastPos) * 50;
		lastPos = rb.position;
	}


	private bool CheckTag(string _tag){

		for (var i = 0; i < listFootstepSystem.Count; i++) {
			if (_tag == listFootstepSystem[i].MaterialTag) {
				currentFootstepType = i;
				return true;
			}
		}
		return false;
	}

	private int ChooseSound(){
		currentSample ++;
		currentSample = currentSample%listFootstepSystem [currentFootstepType].footstepSamples.Count;

		return currentSample;
	}

	private void playSound(int newSample){

		if (listFootstepSystem [currentFootstepType].footstepSamples [newSample] != null) {
			_audio.clip = listFootstepSystem [currentFootstepType].footstepSamples [newSample];

			int tmpRandomPitch = UnityEngine.Random.Range (-5, 6);
			_audio.pitch = 1 + (tmpRandomPitch * .01f);

			int tmpRandomStartPoint = UnityEngine.Random.Range (0, 9);
			_audio.time = tmpRandomStartPoint * .001f;

			_audio.Play ();
		}
	}

	void checkEndFallSound()
    {
        if(charMovement.isOnFloor &&
            lastIsOnFloor != charMovement.isOnFloor)
        {
            if (!bFirstTimeTouchFloor &&
				ingameGlobalManager.instance.b_InputIsActivated &&
				ingameGlobalManager.instance.b_bodyMovement)
            {
				_audio.clip = aFall;
				_audio.Play();
			}
            else
            {bFirstTimeTouchFloor = false;}
		}
        
		lastIsOnFloor = charMovement.isOnFloor;
	}

}
