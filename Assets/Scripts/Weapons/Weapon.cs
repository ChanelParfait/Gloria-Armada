using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponStats{
    public string WeaponName; 
    // how frequently the weapon can fire
    public float fireInterval;
    // time taken to reload the weapon
    public float reloadTime; 
    // total ammo that can be stored at a time
    public int maxAmmo;
    public ProjectileStats projectileStats;

}
public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    [SerializeReference] protected GameObject projectile; 
    [SerializeReference] protected AudioClip fireSound;
    protected WeaponStats weaponStats;
    public WeaponCategories weaponCategory;  
    public bool isPlayerWeapon = false;
    public bool canFire = true;
    protected AudioSource audioSource;
    public Vector3 spawnPosition;

    // Start is called before the first frame  update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SetupWeapon(){
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

    public virtual void Fire(Vector3 velocity){
        Debug.Log("Fire Base Weapon");
        // Get spawn position and spawn projectile object
        GameObject clone = Instantiate(projectile, GetSpawnPos(), gameObject.transform.rotation); 
        
        //GameObject clone = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation); 
        // set stats of projectile
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats, velocity); 
        audioSource.Play();
    }

    
    public virtual void EnemyFire()
    {
        // Get spawn position and spawn projectile object
        GameObject clone = Instantiate(projectile, GetSpawnPos(), gameObject.transform.rotation); 
        
        //GameObject clone = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation); 
        // set stats of projectile
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats); 
        audioSource.Play();
    }

    public virtual Vector3 GetSpawnPos()
    {
        // return the default spawn position
        return gameObject.transform.position + gameObject.transform.forward * 8;
    }

    public GameObject GetProjectile(){
        return projectile; 
    }

    public void SetProjectile(GameObject obj){
        projectile = obj;
    }


    public void SetFireSound(AudioClip clip){
        fireSound = clip;
    }
    
    public AudioClip GetFireSound(){
        return fireSound;
    }
}
