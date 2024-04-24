using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private float maxHealth = 6; 
    private float currentHealth = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "EnemyProjectile"){
            // Take Damage? / Die
            currentHealth -= col.gameObject.GetComponent<Projectile>().projectileStats.damage;
            if(currentHealth <= 0){
                //isDead = true; 
            }
        }
    }

    
}
