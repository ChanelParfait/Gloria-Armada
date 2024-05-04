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
    }

    public EventOption eventOption;

    public GameObject enemyPrefab;

    public GameObject[] waypoints;
    //Spawn enemy
    public void SpawnEnemy()
    {
        //Instantiate the enemy 
        Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
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

    public void Action()
    {
        switch (eventOption)
        {
            case EventOption.SpawnEnemy:
                SpawnEnemy();
                break;
            case EventOption.ChangeOrientation:
                ChangeOrientation();
                break;
        }
    }
}
