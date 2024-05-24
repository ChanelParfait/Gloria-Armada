using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{

    [SerializeField] Slider slider;
    [SerializeField] AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("Saved_SFX_Volume", 0.5f));
        RefreshSlider(PlayerPrefs.GetFloat("Saved_SFX_Volume", 0.5f));
        
    }

    public void SetVolume(float volume){
        //RefreshSlider(volume);  
        Debug.Log("Volume: " + volume);
        if (volume == 0) volume = 0.0001f;
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Saved_SFX_Volume", volume);
    }

    public void RefreshSlider(float volume){
        slider.value = volume;
    }

    public void SetVolumeFromSlider(){
        SetVolume(slider.value);
    }

}
