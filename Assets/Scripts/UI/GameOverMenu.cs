using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    public bool timerStart = false;
    private float timer;
    public static bool gameIsPaused = false;
    public GameObject gameOverUI;
    public Canvas gameHUD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timerStart == true)
        {
            timer += Time.deltaTime;
        }

        if(timer > 3.0f)
        {
            Pause();
        }
    }

    void Pause()
    {
        //Stops time and enables Pause UI
        gameOverUI.SetActive(true);
        Time.timeScale = 0;

        //Disables the HUD that includes player's life, and special bar as well as the power-up slot
        gameHUD.enabled = false;

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

            gameOverUI.SetActive(false);
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
}
