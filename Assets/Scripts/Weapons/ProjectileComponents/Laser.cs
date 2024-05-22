using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public ProjectileStats projectileStats; 
    public GameObject hitParticle;
    float scalingValue;

    Transform beam;
    // Projectile with no velocity
    // Deals damage over time

    void Start(){
        beam = transform.GetChild(0);
    }
    
    public void UpdateStats(ProjectileStats stats, float newScalingValue) {
        projectileStats = stats;
        scalingValue = newScalingValue;
    }

    protected void OnTriggerStay(Collider col){
        if(col.gameObject.tag == "Player"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / scalingValue); 
        }
        else if(col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("EnemyBoss")){
            //Debug.Log("Enemy Hit: " + projectileStats.damage / scalingValue);
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage / scalingValue);
            Vector3 hitLocation = col.ClosestPoint(transform.position);
            if (hitParticle)
            {
                Instantiate(hitParticle, hitLocation, Quaternion.identity);
            } 
            //scale the laser object so that it stretches to the target
            beam.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (hitLocation - transform.position).magnitude);
        }
    }
}
