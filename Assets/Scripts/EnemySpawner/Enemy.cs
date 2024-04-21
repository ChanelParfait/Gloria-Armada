using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    //[SerializeField] GameObject projectile; 
    WeaponManager weaponManager; 
    [SerializeField] float shootInterval = 5.0f;
    private float timer = 0;
    public float health = 3.0f;
    public bool hit = false;
    public float incomingDamage = 1.0f; 
    // Start is called before the first frame update
    void Start()
    {   
        weaponManager = gameObject.GetComponent<WeaponManager>();
        shootInterval = 3;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= shootInterval){
            timer = 0; 
            weaponManager.FirePrimaryWeapon();
        }
    }


    void Shoot(){
        // get the position 4 units in front of the enemy 
        //Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
        //Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
        //fireSound.Play();
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage? / Die
            //Debug.Log("Die");
            hit = true;
            health -= incomingDamage; 
            if(health <= 0){
                Destroy(gameObject);
            }
            hit = false;
            
            // increase player score 
        }
    }

}
