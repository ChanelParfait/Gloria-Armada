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
    
    public void Launch(ProjectileStats stats) {
        startTime = Time.time;
        projectileStats = stats;
    }

    void FixedUpdate() {
        
        if (Time.time > startTime + projectileStats.lifetime) {
            Die();
        }
        // deal damage over time
    }

    protected void Die(){
        if (hitParticle){
            Instantiate(hitParticle, transform.position, Quaternion.identity);
        }
        ParticleManager[] pms = GetComponentsInChildren<ParticleManager>();
        foreach (ParticleManager pm in pms)
        {
            pm.transform.SetParent(null);   
            pm.Detach();
        }
        Destroy(gameObject);
    }

    protected void OnTriggerEnter(Collider col){
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
    }
}
