using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (settingsMenu.activeSelf == true)
            {
                settingsMenu.SetActive(false);
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
}
