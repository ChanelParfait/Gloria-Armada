using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public JetControl playerControl;
    public Canvas gameHUD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if the game is paused or not when ESC is pressed, then pause or unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        //Stops time and enables Pause UI
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;

        //Disables the player's ability to control their ship
        playerControl.enabled = false;

        //Disables the HUD that includes player's life, and special bar as well as the power-up slot
        gameHUD.enabled = false;

        //Blurs the scene while paused
        PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolume.enabled = true;

        //To be used in the Resume button to make sure the game is paused when the player resumes the game
        gameIsPaused = true;

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
            Time.timeScale = 1;
            playerControl.enabled = true;
            gameHUD.enabled = true;
            PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
            ppVolume.enabled = false;
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
}
