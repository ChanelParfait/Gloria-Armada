using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    private string jsonFilePath;

    [System.Serializable]
    public class HighScoreEntry
    {
        public string level;
        public string name;
        public int score;
        public float time;

        bool Equals(HighScoreEntry other) => (level == other.level && name == other.name && score == other.score && time == other.time);
    }

    [System.Serializable]
    public class HighScoreList
    {
        public List<HighScoreEntry> highScores;
    }

    public HighScoreList highScoreList;

    public TextMeshProUGUI[] scoreNames;
    public TextMeshProUGUI[] scoreValues;

    public TextMeshProUGUI[] timeNames;
    public TextMeshProUGUI[] timeValues;


    public TextMeshProUGUI currentScoreValue;
    public TextMeshProUGUI currentTimeValue;

    HighScoreEntry currentHighScoreEntry;

    void Start()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        CopyDefaultFileIfNotExists();
        LoadHighScores();
    }

    void CopyDefaultFileIfNotExists()
    {
        if (!File.Exists(jsonFilePath))
        {
            TextAsset defaultJson = Resources.Load<TextAsset>("default_highscores");
            if (defaultJson != null)
            {
                File.WriteAllText(jsonFilePath, defaultJson.text);
            }
            else
            {
                Debug.LogError("Default high score file not found in Resources!");
            }
        }
    }
    
    void LoadHighScores()
    {
        if (File.Exists(jsonFilePath))
        {
            Debug.Log("Loading high scores from: " + jsonFilePath);
            string json = File.ReadAllText(jsonFilePath);
            highScoreList = JsonUtility.FromJson<HighScoreList>(json);
        }
        else
        {
            Debug.LogError("High score file not found!");
            // Initialize with some default high scores if file is not found
            highScoreList = new HighScoreList
            {
                highScores = new List<HighScoreEntry>()
                {
                    new HighScoreEntry { level = "S4_Level_1", name = "Player1", score = 10000, time = 659f },
                    new HighScoreEntry { level = "S4_Level_1", name = "Player2", score = 9000, time = 588f },
                    new HighScoreEntry { level = "S4_Level_1", name = "Player3", score = 8000, time = 587f },
                    new HighScoreEntry { level = "S4_Level_1", name = "Player4", score = 7000, time = 516f },
                    new HighScoreEntry { level = "S4_Level_1", name = "Player5", score = 6000, time = 500f }
                }
            };
        }

        DisplayHighScores();
        if (currentHighScoreEntry != null)
        {
            DisplayCurrentScore(currentHighScoreEntry);
        }
    }
    
    void Update(){
        //If canvas is active, 
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.isActiveAndEnabled){
            //Debug.Log("Canvas enabled");
            Cursor.lockState = CursorLockMode.None;
            if (Cursor.visible == false)
            {
                Cursor.visible = true;
            }
        }
    }

    public void SaveHighScores()
    {
        string json = JsonUtility.ToJson(highScoreList, true);
        File.WriteAllText(jsonFilePath, json);
    }

    public List<HighScoreEntry> GetBestScores(string level)
    {
        highScoreList.highScores.Sort((x, y) => y.score.CompareTo(x.score));
        return highScoreList.highScores.FindAll(entry => entry.level == level);
    }

    public List<HighScoreEntry> GetBestTimes(string level)
    {
        highScoreList.highScores.Sort((x, y) => x.time.CompareTo(y.time));
        return highScoreList.highScores.FindAll(entry => entry.level == level);
    }

    public void AddHighScoreEntry(HighScoreEntry newEntry)
    {
        currentHighScoreEntry = newEntry;
        highScoreList.highScores.Add(newEntry);
        SaveHighScores();
        DisplayHighScores();

    }

    void DisplayCurrentScore(HighScoreEntry currentEntry)
    {
        currentScoreValue.text = currentEntry.score.ToString();
        currentTimeValue.text = System.TimeSpan.FromSeconds((double)currentEntry.time).ToString(@"mm\:ss");
    }

    void DisplayHighScores()
    {
        if (currentHighScoreEntry != null)
        {
            DisplayCurrentScore(currentHighScoreEntry);
        }

        string level = SceneManager.GetActiveScene().name;
        var bestScores = GetBestScores(level);
        var bestTimes = GetBestTimes(level);

        for (int i = 0 ; i < Mathf.Min(bestScores.Count, scoreNames.Length, scoreValues.Length); i++)
        {
            scoreNames[i].text = bestScores[i].name;
            scoreValues[i].text = bestScores[i].score.ToString();
            bool isCurrent = bestScores[i].Equals(currentHighScoreEntry);
            scoreNames[i].color = isCurrent ? Color.yellow : Color.white;
        }

        for (int i = 0 ; i < Mathf.Min(bestTimes.Count, timeNames.Length, timeValues.Length); i++)
        {
            timeNames[i].text = bestTimes[i].name;
            timeValues[i].text = System.TimeSpan.FromSeconds((double)bestTimes[i].time).ToString(@"mm\:ss");
            bool isCurrent = bestTimes[i].Equals(currentHighScoreEntry);
            timeNames[i].color = isCurrent ? Color.yellow : Color.white;
        }
    }
}
