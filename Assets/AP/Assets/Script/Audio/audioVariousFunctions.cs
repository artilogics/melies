// Description : audioVariousFunctions : methods to  Prepare a new audio clip before playing it
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class audioVariousFunctions {
    //--> Prepare a new audio clip before playing it
    public void prepareAudio(AudioSource refAudioSource, float newVolume, float newSpacialBlend, AudioClip newClip)
    {
        refAudioSource.volume = newVolume;
        refAudioSource.spatialBlend = newSpacialBlend;
        refAudioSource.clip = newClip;
    }
}
