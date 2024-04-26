using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLoadout : MonoBehaviour
{
    public Animator loadoutAnim;
    public GameObject primary, special;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPrimaryMenu()
    {
        loadoutAnim.SetBool("Changing", true);
        if (primary.activeSelf == false)
        {
            primary.SetActive(true);
        }
        if (special.activeSelf == true)
        {
            special.SetActive(false);
        }
    }

    public void OpenSpecialMenu()
    {
        loadoutAnim.SetBool("Changing", true);
        if (special.activeSelf == false)
        {
            special.SetActive(true);
        }
        if (primary.activeSelf == true)
        {
            primary.SetActive(false);
        }
    }

    public void SaveChanges()
    {
        loadoutAnim.SetBool("Changing", false);
    }
}
