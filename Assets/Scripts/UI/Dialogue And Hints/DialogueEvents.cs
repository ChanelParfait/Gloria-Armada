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
    }

    public EventOption eventOption;

    public GameObject enemyPrefab;

    public GameObject[] waypoints;
    //Spawn enemy
    public void SpawnEnemy()
    {
        GameObject formationPoint = GameObject.Find("FormationPoint");
        //Instantiate the enemy 
        GameObject spawnedEnemy = Instantiate(enemyPrefab, formationPoint.transform.position, Quaternion.LookRotation(Vector3.right, Vector3.up));
        EnemyPlane e;
        if ((e = spawnedEnemy.GetComponent<EnemyPlane>()) != null)
        {
            e.referenceSpeed = formationPoint.GetComponent<Rigidbody>().velocity.x;
        }
        
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
            case EventOption.EndScene:
                LevelManager lm = FindObjectOfType<LevelManager>();
                lm.YouWin();
                break;
        }
    }
}
