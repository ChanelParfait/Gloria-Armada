using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectChoice : MonoBehaviour
{
    Image myButtonImage;
    public Sprite[] myButtonSprite;

    void Start()
    {
        
    }

    public void SelectItem1()
    {
        Debug.Log("Equipped Item 1");
        myButtonImage = GetComponent<Image>();
        myButtonImage.color = new Color(1, 1, 1, 1);
        myButtonImage.sprite = myButtonSprite[0];
    }

    public void SelectItem2()
    {
        Debug.Log("Equipped Item 2");
        myButtonImage = GetComponent<Image>();
        myButtonImage.color = new Color(1, 1, 1, 1);
        myButtonImage.sprite = myButtonSprite[1];
    }

    public void SelectItem3()
    {
        Debug.Log("Equipped Item 3");
        myButtonImage = GetComponent<Image>();
        myButtonImage.color = new Color(1, 1, 1, 1);
        myButtonImage.sprite = myButtonSprite[2];
    }
}
