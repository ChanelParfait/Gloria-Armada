using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TutorialSequence : MonoBehaviour
{
    PlaySpaceBoundary playSpaceBoundary;
    Enemy_Spawner enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        playSpaceBoundary = GetComponentInChildren<PlaySpaceBoundary>();
        playSpaceBoundary.enforceBoundary = false;

        enemySpawner = GetComponentInChildren<Enemy_Spawner>();
        enemySpawner.isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
