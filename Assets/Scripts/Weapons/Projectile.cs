using System;
using System.Collections;
using System.Collections.Generic;
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
    //float lifetime = 10;

    public GameObject hitParticle;


    // Start is called before the first frame update
    void Start()
    {

    }
    public void Launch(ProjectileStats stats) {
        startTime = Time.time;
        projectileStats = stats;

        
        projectileRB = GetComponent<Rigidbody>();
        //Debug.Log(stats.speed);
        projectileRB.AddRelativeForce(new Vector3(0, 0, stats.speed), ForceMode.VelocityChange); 
    }
    public void Launch(ProjectileStats stats, Vector3 cameraVelocity) {
        startTime = Time.time;
        projectileStats = stats;

        projectileRB = GetComponent<Rigidbody>();
        projectileRB.AddRelativeForce(new Vector3(0, 0, stats.speed * 2), ForceMode.VelocityChange);
        projectileRB.AddForce(cameraVelocity, ForceMode.VelocityChange); 
    }


    void FixedUpdate() {
        if (Time.time > startTime + projectileStats.lifetime) {
            Destroy(gameObject);
        }
    }

    public void SetStats(ProjectileStats stats){
        projectileStats = stats;
    }

    private void OnTriggerEnter(Collider col){
        //Debug.Log("Projectile Triggered");

        if(col.gameObject.tag == "Player"){
            col.GetComponent<PlayerLife>().TakeDamage(projectileStats.damage);
            Destroy(gameObject);

        }
        else if(col.gameObject.tag == "Enemy"){
            col.GetComponent<EnemyBase>().TakeDamage(projectileStats.damage);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);

        }

    }

}
