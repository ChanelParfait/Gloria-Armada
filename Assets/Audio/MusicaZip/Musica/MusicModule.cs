using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicModule : MonoBehaviour
{
    AudioSource mainSpeaker, loopSpeaker;
    private void Start() {
    	AudioSource[] source = GetComponents<AudioSource>();
    	if(source == null) return;
    	if(source[0].loop == true) {
    	    loopSpeaker = source[0];
    	    mainSpeaker = source[1];
    	}
    	else {
    	    loopSpeaker = source[1];
    	    mainSpeaker = source[0];
    	}
    	
    	mainSpeaker.Play();
    	double transitionPoint = AudioSettings.dspTime + mainSpeaker.clip.length;
    	//mainSpeaker.SetScheduledEndTime(transitionPoint);
    	//loopSpeaker.PlayScheduled(transitionPoint);
    	loopSpeaker.PlayDelayed(mainSpeaker.clip.length);
    }
}
