using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public struct WeaponStats{
    public string WeaponName; 
    public int FireRate; 
    public int MaxAmmo;
    public ProjectileStats projectileStats;

}
public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    [SerializeReference] protected GameObject projectile; 
    [SerializeReference] protected AudioClip fireSound;
    
    public WeaponCategories weaponCategory;  
    public bool canFire = true;
    protected WeaponStats weaponStats;
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

    public virtual void Fire(){
        Debug.Log("Fire Base Weapon");
    }

    
    public virtual void EnemyFire()
    {
        Debug.Log("Enemy Gun Fire");
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
