using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public bool settingsOpen = false;
    public bool levelSelectOpen = false;
    public GameObject pauseMenuUI;
    public Canvas gameHUD;
    public GameObject plane;
    public GameObject gameOverUI;

    public GameObject settingsMenu;
    public GameObject levelSelectMenu;

    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] lvlObjs = scene.GetRootGameObjects();
        //iterate over lvl objects to find settingsCanvas
        foreach (GameObject obj in lvlObjs)
        {
            if (obj.name == "SettingsCanvas")
            {
                settingsMenu = obj;
            }
            if (obj.name == "LevelSelectCanvas")
            {
                levelSelectMenu = obj;
            }
        }
        EnsureSettingsButtonListener();
        EnsureLevelSelectBackButtonListener();
    }

    void EnsureSettingsButtonListener()
    {
        Button[] buttons = settingsMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "Button")
            {
                Button btn = button;
                PauseMenu pauseMenu = GetComponent<PauseMenu>();
                Debug.Log("Checking for listeners on SettingsBackButton");
                Debug.Log("SettingsBackTarget: " + btn.onClick.GetPersistentTarget(0));
                if (btn.onClick.GetPersistentTarget(0) != pauseMenu)
                {
                    Debug.Log("Adding listener to SettingsBackButton");
                    btn.onClick.AddListener(pauseMenu.CloseSettings);
                }
            }
        }
    }

    void EnsureLevelSelectBackButtonListener(){
        Button[] buttons = levelSelectMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "BackButton")
            {
                Button btn = button;
                PauseMenu pauseMenu = GetComponent<PauseMenu>();
                Debug.Log("Checking for listeners on LevelSelectBackButton");
                Debug.Log("LevelSelectBackTarget: " + btn.onClick.GetPersistentTarget(0));
                if (btn.onClick.GetPersistentTarget(0) != pauseMenu)
                {
                    Debug.Log("Adding listener to LevelSelectBackButton");
                    btn.onClick.AddListener(pauseMenu.CloseLevelSelect);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (plane != null)
        {
            //Checks if the game is paused or not when ESC is pressed, then pause or unpause
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (settingsOpen)
                {
                    CloseSettings();
                    return;
                }
                else if (levelSelectOpen)
                {
                    CloseLevelSelect();
                    return;
                }
                else if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        else
        {
            //gameOverUI.GetComponent<GameOverMenu>().timerStart = true;
        }
    }

    void Pause()
    {
        //Stops time and enables Pause UI
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;

        //Disables the HUD that includes player's life, and special bar as well as the power-up slot
        gameHUD.enabled = false;

        //To be used in the Resume button to make sure the game is paused when the player resumes the game
        gameIsPaused = true;
        Debug.Log("Game is paused - showingCursor");
        //Locks the cursor and makes it invisible
        Cursor.lockState = CursorLockMode.None;
        if (Cursor.visible == false)
        {
            Cursor.visible = true;
        }
    }

    //This entire resume function practically does the opposite of the Pause function
    public void Resume()
    {
        if (gameIsPaused)
        {

            pauseMenuUI.SetActive(false);
            settingsMenu.SetActive(false);
            levelSelectMenu.SetActive(false);
            Time.timeScale = 1;
            gameHUD.enabled = true;
            gameIsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (Cursor.visible == true)
            {
                Cursor.visible = false;
            }
        }
    }

    public void RestartLevel()
    {
        //Restart current scene, and makes sure time isn't stopped when the scene loads
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToTitle()
    {
        //Restart current scene, and makes sure time isn't stopped when the scene loads
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
        //Opens the settings menu
        settingsOpen = true;
        pauseMenuUI.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        //Closes the settings menu
        settingsOpen = false;
        pauseMenuUI.SetActive(true);
        settingsMenu.SetActive(false);
    }
    

    public void OpenLevelSelect()
    {
        //Opens the level select menu
        
        levelSelectOpen = true;
        pauseMenuUI.SetActive(false);
        levelSelectMenu.GetComponent<Canvas>().enabled = true;  
        levelSelectMenu.GetComponent<LevelSelection>().UpdateMaxLevelComplete();
    }

    public void CloseLevelSelect()
    {
        //Closes the level select menu
        levelSelectOpen = false;
        pauseMenuUI.SetActive(true);
        levelSelectMenu.GetComponent<Canvas>().enabled = false;
    }
}
