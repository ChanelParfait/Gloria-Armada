using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChoiceCam : MonoBehaviour
{
    public Animator camAnim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPrimary()
    {
        camAnim.SetBool("ChangePrimary", true);
    }

    public void SelectSpecial()
    {
        camAnim.SetBool("ChangeSpecial", true);
    }

    public void SelectBody()
    {
        camAnim.SetBool("ChangeBody", true);
    }

    public void BackToLoadout()
    {
        camAnim.SetBool("ChangePrimary", false);
        camAnim.SetBool("ChangeSpecial", false);
        camAnim.SetBool("ChangeBody", false);
    }
}
