using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ButtonManager : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TMP_InputField playerNameInput;
    public GameObject nameEdit;
    string playerName;

    void Start(){
        nameEdit = GameObject.Find("NameEdit");
        playerNameText = nameEdit.transform.Find("Text Area/Text").GetComponent<TextMeshProUGUI>();
        playerNameInput = nameEdit.GetComponent<TMP_InputField>();
        playerName = PlayerPrefs.GetString("PlayerName", "PLAYER");
        playerNameInput.text = playerName;

    }

     public void QuitGame()
    {
        Application.Quit();
    }

    public void SetName()
    {
        // Remove any characters that aren't letters or numbers
        string inputText = playerNameText.text;
        string sanitizedText = Regex.Replace(inputText, @"[^a-zA-Z0-9]", "");

        // Only get the first 6 characters of the sanitized name
        //Debug.Log(sanitizedText.Length);

        if (sanitizedText.Length <= 1)
        {
            playerName = "PLAYER";
        }
        else if (sanitizedText.Length > 7)
        {
            playerName = sanitizedText.Remove(6);
        }
        else
        {
            playerName = sanitizedText;
        }

        playerNameInput.text = playerName;
        PlayerPrefs.SetString("PlayerName", playerName);
        string name = PlayerPrefs.GetString("PlayerName", "Player");
        Debug.Log("PlayerName: " + name);   
    }
}