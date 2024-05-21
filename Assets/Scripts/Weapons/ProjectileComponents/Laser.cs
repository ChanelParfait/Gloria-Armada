using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public ProjectileStats projectileStats; 
    public GameObject hitParticle;
    float scalingValue;
    // Projectile with no velocity
    // Deals damage over time
    
    public void UpdateStats(ProjectileStats stats, float newScalingValue) {
        projectileStats = stats;
        scalingValue = newScalingValue;
    }

    protected void OnTriggerStay(Collider col){
        if(col.gameObject.tag == "Player"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / scalingValue); 
        }
        else if(col.gameObject.tag == "Enemy"){
            //Debug.Log("Enemy Hit: " + projectileStats.damage / scalingValue);
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / scalingValue);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            } 
        }
    }
}
