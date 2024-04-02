using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    [SerializeField] GameObject projectile; 
    [SerializeField] float shootInterval = 5.0f;
    private float timer = 0;
    [SerializeField] AudioSource fireSound;
    // Start is called before the first frame update
    void Start()
    {
        shootInterval = 3;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= shootInterval){
            Shoot();
            timer = 0; 
        }
    }


    void Shoot(){
        // get the position 4 units in front of the enemy 
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.right * 8;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
        fireSound.Play();
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage? / Die
            Debug.Log("Die");
            Destroy(gameObject);
            // increase player score 
        }
    }

}
