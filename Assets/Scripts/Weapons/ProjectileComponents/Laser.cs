using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public ProjectileStats projectileStats; 
    private float startTime;
    public GameObject hitParticle;
    // Projectile with no velocity
    // Deals damage over time
    
    public void UpdateStats(ProjectileStats stats) {
        projectileStats = stats;
    }

    protected void OnTriggerStay(Collider col){
        //Debug.Log("Trigger: " + projectileStats.damage / (4 * 24));

        if(col.gameObject.tag == "Player"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / 4); 
        }
        else if(col.gameObject.tag == "Enemy"){
            Debug.Log("Enemy Hit: " + projectileStats.damage / 4);
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / 4);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            } 
        }
    }


    /*protected void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage); 
        }
        else if(col.gameObject.tag == "Enemy"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            } 
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            } 
        }
    }*/
}
