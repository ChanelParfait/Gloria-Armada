using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    public bool isEnemyWeapon = false; 
    [SerializeReference] protected GameObject projectile; 
    public WeaponCategories weaponCategory;  
    public bool canFire = true;
    protected AudioSource audioSource;
    [SerializeReference] protected AudioClip fireSound;

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
