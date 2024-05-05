using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectChoiceCam : MonoBehaviour
{
    public Animator camAnim;
    public SceneLoader sceneChanger;
    public GameObject LaunchButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneChanger.LoadPreviousScene();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(LaunchButton.activeSelf == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void SelectPrimary()
    {
        camAnim.SetBool("ChangePrimary", true);
        LaunchButton.SetActive(false);
    }

    public void SelectSpecial()
    {
        camAnim.SetBool("ChangeSpecial", true);
        LaunchButton.SetActive(false);
    }

    public void SelectBody()
    {
        camAnim.SetBool("ChangeBody", true);
        LaunchButton.SetActive(false);
    }

    public void BackToLoadout()
    {
        camAnim.SetBool("ChangePrimary", false);
        camAnim.SetBool("ChangeSpecial", false);
        camAnim.SetBool("ChangeBody", false);
        LaunchButton.SetActive(true);
    }

    public void Launch()
    {
        camAnim.SetBool("Launch", true);
    }
}
