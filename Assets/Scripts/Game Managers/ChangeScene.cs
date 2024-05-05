using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool changeScene = false;
    public Animator cutSceneEndTransition;

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                LoadNextScene();
            }
        }*/

        if (cutSceneEndTransition)
        {
            if (changeScene == true)
            {
                LoadNextScene();
            }
        }
    }

    public void EventAtAnimEnd()
    {
        changeScene = true;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
