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

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        playerControl.enabled = false;
        gameHUD.enabled = false;
        PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolume.enabled = true;
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        if (Cursor.visible == false)
        {
            Cursor.visible = true;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
