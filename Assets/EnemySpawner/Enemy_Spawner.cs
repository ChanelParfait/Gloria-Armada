using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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


        //SpawnEnemy("Top_Middle", 0); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnEnemies(string spawnPointName, int enemyIndex, int spawnAmount){
        for(int i = 0; i < spawnAmount; i++){
            // Spawn enemy 
            Debug.Log("Spawn"); 
            SpawnEnemy(spawnPointName, enemyIndex); 
            // then wait _ seconds
            yield return new WaitForSeconds(3);
        }
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
            // upon colliding with trigger, retrieve spawn parameters and start spawning coroutine
            SpawnTrigger trigger = col.GetComponent<SpawnTrigger>();
            StartCoroutine(SpawnEnemies(trigger.spawnPointName, trigger.enemyIndex, trigger.spawnAmount));
            col.enabled = false; 
        }
    }
}
