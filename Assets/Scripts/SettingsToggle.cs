using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject playMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (settingsMenu.activeSelf == true)
            {
                settingsMenu.SetActive(false);
                mainMenu.SetActive(true);
            }

            if (playMenu.activeSelf == true)
            {
                playMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
    }

    public void ToggleSettings()
    {      
        if (settingsMenu.activeSelf == true)
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        else
        {
            settingsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
    public void TogglePlay()
    {
        if (playMenu.GetComponent<Canvas>().enabled == true)
        {
            playMenu.GetComponent<Canvas>().enabled = false;
            mainMenu.SetActive(true);
        }

        else
        {
            playMenu.GetComponent<Canvas>().enabled = true;
            mainMenu.SetActive(false);
        }
    }
}
