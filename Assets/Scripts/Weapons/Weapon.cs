using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct WeaponStats{
    public string WeaponName;
    public int Damage;
    public int FireRate; 
    public int MaxAmmo;
    public int Range; 
    
}
public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    [SerializeReference] protected GameObject projectile; 
    [SerializeReference] protected AudioClip fireSound;

    public WeaponCategories weaponCategory;  
    public bool canFire = true;
    protected AudioSource audioSource;

    // Start is called before the first frame  update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Fire(){
        Debug.Log("Fire Base Weapon");
    }

    public GameObject GetProjectile(){
        return projectile; 
    }

    public void SetProjectile(GameObject obj){
        projectile = obj;
    }
    
    public void SetupAudio(){
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

    public void SetFireSound(AudioClip clip){
        fireSound = clip;
    }
    
    public AudioClip GetFireSound(){
        return fireSound;
    }
}
