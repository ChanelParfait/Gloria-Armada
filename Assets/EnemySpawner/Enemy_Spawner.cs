using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies; 

    [SerializeField] private Camera mainCamera; 
    [SerializeField] Dictionary<string, GameObject> spawnPoints = new Dictionary<string, GameObject>();


    
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in gameObject.transform){
            Debug.Log(child.gameObject.name);
            //Debug.Log(child.gameObject);

            spawnPoints.Add(child.gameObject.name, child.gameObject);
        }


        SpawnEnemy("Top_Middle", 0); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies(string spawnPointName, int enemyIndex, int spawnAmount){

    }

    private void SpawnEnemy(string spawnPointName, int enemyIndex){
        // given a spawn position name, find the corresponding spawn point gameobject
        GameObject spawnPoint;
        if (spawnPoints.TryGetValue(spawnPointName, out spawnPoint)){
            // check if given enemy index is valid 
            if(enemyIndex < enemies.Length){
                GameObject enemy = enemies[enemyIndex]; 
                //get movement direction 
                // use spawn point to get the spawn position and rotation 
                Instantiate(enemy, spawnPoint.transform.position, spawnPoint.transform.localRotation);
            }  
        }
    }

    private void OnTriggerEnter(Collider col){
        Debug.Log("Trigger");

        if(col.tag == "SpawnTrigger"){
            SpawnTrigger trigger = col.GetComponent<SpawnTrigger>();
            SpawnEnemy(trigger.spawnPointName, trigger.enemyIndex); 
            col.enabled = false; 
        }
    }
}
