// Description : PlayASound : methods to play and stop a sound
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayASound : MonoBehaviour {

    private AudioSource _source;

    public void Start()
    {
        _source = GetComponent<AudioSource>(); 
    }

    public void AP_PlayASound(){
        _source.Play();
    }

    public void AP_StopASound()
    {
        _source.Stop();
    }

    public void AP_AudioFadeOut(){
        if(gameObject.activeInHierarchy)
            StartCoroutine(I_audioFadeOut());
    }

    private IEnumerator I_audioFadeOut()
    {
        //Debug.Log ("Subtitles");


        float target = 0;

            while (_source.volume != target)
            {
                if (!ingameGlobalManager.instance.b_Ingame_Pause)
                {
                _source.volume = Mathf.MoveTowards(_source.volume, target, Time.deltaTime);
                }
                yield return null;
            }

        //if (txtSubTitle) txtSubTitle.text = "";
        yield return null;
    }

    public bool Bool_AP_PlayASound()
    {
        _source.Play();
        return true;
    }
}
