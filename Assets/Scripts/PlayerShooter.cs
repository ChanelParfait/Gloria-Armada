using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] GameObject projectile; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Shoot();
        }
    }

    void Shoot(){
        // get the position 4 units in front of the enemy 
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.right * 8;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "EnemyProjectile"){
            // Take Damage? / Die
            //Debug.Log("Die");
            //Destroy(gameObject);
            // increase player score 
        }
    }
}
