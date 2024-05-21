using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleInvertInput : MonoBehaviour
{
    public Image image;

    public Sprite onSprite;
    public Sprite offSprite;

    public Button _button;

    // Start is called before the first frame update
    void Start()
    {
        //Get a reference to the current state of PlayerPrefs InvertPitch and set the sprite accordingly
        if (PlayerPrefs.GetInt("InvertPitch") == 1)
        {
            image.sprite = onSprite;
        }
        else
        {
            image.sprite = offSprite;
        }
        
    }

    public void ToggleInvert(){
        //Toggle the InvertPitch value in PlayerPrefs
        if (PlayerPrefs.GetInt("InvertPitch") == 1)
        {
            PlayerPrefs.SetInt("InvertPitch", 0);
            image.sprite = offSprite;
        }
        else
        {
            PlayerPrefs.SetInt("InvertPitch", 1);
            image.sprite = onSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
