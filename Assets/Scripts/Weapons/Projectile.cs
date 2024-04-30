using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct ProjectileStats{
    public float speed; 
    public int damage;
    public int AP;
    public float lifetime;
    public Vector3 size; 
    public float range; 
}

public class Projectile : MonoBehaviour
{
    // Base Class for All Projectile Objects
    public ProjectileStats projectileStats; 
    public Rigidbody projectileRB;
    float startTime;
    public GameObject hitParticle;


    public void Launch(ProjectileStats stats) {
        startTime = Time.time;
        projectileStats = stats;
        projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddRelativeForce(new Vector3(0, 0, stats.speed), ForceMode.VelocityChange); 
    }

    public void Launch(ProjectileStats stats, Vector3 cameraVelocity) {
        startTime = Time.time;
        projectileStats = stats;

        projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddRelativeForce(new Vector3(0, 0, stats.speed * 2), ForceMode.VelocityChange);
        projectileRB.AddForce(cameraVelocity, ForceMode.VelocityChange); 
    }
    
    public void SetStats(ProjectileStats stats){
        projectileStats = stats;
    }

    void FixedUpdate() {
        if (Time.time > startTime + projectileStats.lifetime) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage);
            Destroy(gameObject);
        }
        else if(col.gameObject.tag == "Enemy"){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage);
            Destroy(gameObject);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            }
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            MissileController missile;
            if (missile = GetComponent<MissileController>()){
                Debug.Log("Missile Hit Terrain");
                missile.Detonate();
            }
            Destroy(gameObject);
        }
    }
}
