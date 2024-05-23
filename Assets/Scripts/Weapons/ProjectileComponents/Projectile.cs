using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct ProjectileStats{
    public float speed; 
    public float damage;
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
    private float startTime;
    public GameObject hitParticle;

    //public bool destroyOnHit = true;


    [SerializeField] AudioClip launchSound;
    [SerializeField] AudioClip flybySound;
    [SerializeField] AudioClip engineSound;
    [SerializeField] AudioClip impactSound;

    AudioSource audioSource;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        projectileRB = GetComponent<Rigidbody>();
        if (projectileRB == null){
            projectileRB = gameObject.AddComponent<Rigidbody>();
            projectileRB.interpolation = RigidbodyInterpolation.Interpolate;
        }
        Collider col = GetComponent<Collider>();
        if (col == null){
            col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
        }
        if (engineSound){
            audioSource.clip = engineSound;
            audioSource.time = UnityEngine.Random.Range(0, audioSource.clip.length);
            audioSource.volume = UnityEngine.Random.Range(1.3f, 1.5f);
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.loop = true;
            audioSource.Play();
        }
    }

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

    public ProjectileStats GetStats(){
        return projectileStats;
    }

    void FixedUpdate() {
        if (Time.time > startTime + projectileStats.lifetime) {
            Die();
        }
    }

    protected void OnCollisionEnter(Collision col){
        
        if(col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            Die();
        }
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
            Die();
        }
        else if(col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("EnemyBoss")){
            col.GetComponent<Actor>().TakeDamage(projectileStats.damage);
            if (hitParticle)
            {
                Instantiate(hitParticle, transform.position, Quaternion.identity);
            } 
            Die();
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            MissileController missile;
            if (missile = GetComponent<MissileController>()){
                missile.Detonate();
            } 
            Die();
        }
    }
}
