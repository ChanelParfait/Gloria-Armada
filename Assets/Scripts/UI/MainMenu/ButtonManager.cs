using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Text playerNameText;
    public InputField playerNameInput;
    public GameObject nameEdit;
    string playerName;

    void Start()
    {
        nameEdit = GameObject.Find("NameEdit");
        if (nameEdit == null)
        {
            Debug.LogError("NameEdit object not found");
            return;
        }

        playerNameInput = nameEdit.GetComponent<InputField>();
        if (playerNameInput == null)
        {
            Debug.LogError("PlayerNameInput component not found");
            return;
        }

        playerName = PlayerPrefs.GetString("PlayerName", "PLAYER");
        playerNameInput.text = playerName;

        playerNameInput.onValueChanged.AddListener(delegate { OnInputValueChanged(); });
        playerNameInput.onEndEdit.AddListener(delegate { OnInputEndEdit(); });

        Debug.Log("Start method completed");
    }

    void OnInputValueChanged()
    {
        Debug.Log("Input value changed: " + playerNameInput.text);
    }

    void OnInputEndEdit()
    {
        Debug.Log("Input end edit: " + playerNameInput.text);
    }

    public void SetName()
    {
        Debug.Log("SetName method called");
        // Remove any characters that aren't letters or numbers
        string inputText = playerNameInput.text; // Access the input field's text
        Debug.Log("Input Text: " + inputText);
        string sanitizedText = Regex.Replace(inputText, @"[^a-zA-Z0-9]", "");

        // Only get the first 6 characters of the sanitized name
        if (sanitizedText.Length <= 1)
        {
            Debug.Log("Name too short");
            playerName = "PLAYA";
        }
        else if (sanitizedText.Length > 6)
        {
            Debug.Log("Trimming name");
            playerName = sanitizedText.Substring(0, 6); // Use Substring instead of Remove
        }
        else
        {
            playerName = sanitizedText;
        }

        playerNameInput.text = playerName;
        PlayerPrefs.SetString("PlayerName", playerName);
        string name = PlayerPrefs.GetString("PlayerName", "NULL");
        Debug.Log("PlayerPrefsName: " + name);
        Debug.Log("PlayerName: " + playerName);
    }
}
