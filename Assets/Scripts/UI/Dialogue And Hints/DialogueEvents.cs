using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Event", menuName = "Dialogue/Dialogue Event")]
public class DialogueEvents : ScriptableObject
{

    [System.Serializable]
    public enum EventOption {
        SpawnEnemy,
        ChangeOrientation,
        AddWaypoints,
        EndScene,
        InvertControls,
    }

    public EventOption eventOption;

    public GameObject enemyPrefab;

    public GameObject[] waypoints;
    //Spawn enemy
    public void SpawnEnemy()
    {
        LevelManager lm = FindObjectOfType<LevelManager>();
        lm.spawnOverTime = true;
        
    }

    public void InvertControls()
    {
        PlayerPrefs.SetInt("InvertPitch", 1);
    }

    //Change Orientation
    public void ChangeOrientation()
    {
        //Find the levelManager
        LevelManager lm = FindObjectOfType<LevelManager>();
        //Change the orientation of the levelManager
        Perspective currentPerspective = lm.currentPerspective;
        if (currentPerspective == Perspective.Top_Down)
        {
            lm.UpdatePerspective(Perspective.Side_On);
        }
        else
        {
            Debug.Log("Current Perspective = " + currentPerspective.ToString() + " Changing to Top Down");
            lm.UpdatePerspective(Perspective.Top_Down);
        }
    }

    public void DoEvent()
    {
        switch (eventOption)
        {
            case EventOption.SpawnEnemy:
                SpawnEnemy();
                break;
            case EventOption.ChangeOrientation:
                ChangeOrientation();
                break;
            case EventOption.EndScene:
                LevelManager lm = FindObjectOfType<LevelManager>();
                lm.YouWin();
                break;
            case EventOption.InvertControls:
                InvertControls();
                break;
        }
    }
}
