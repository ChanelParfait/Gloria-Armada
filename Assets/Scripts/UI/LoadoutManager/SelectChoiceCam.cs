using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectChoiceCam : MonoBehaviour
{
    public Animator camAnim;
    public GameObject LaunchButton;


    void OnEnable()
    {
        LaunchButton.GetComponent<Button>().onClick.AddListener(Launch);
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
