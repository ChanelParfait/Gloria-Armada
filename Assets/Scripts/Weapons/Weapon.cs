using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    public bool isEnemyWeapon = false; 
    public  GameObject projectile; 
    public WeaponCategories weaponCategory;  
    public bool canFire = true;
    public AudioSource audioSource;
    public AudioClip fireSound;

    // Start is called before the first frame update
    void Start()
    {
        SetupAudio(fireSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Fire(){
        if(canFire){
            Debug.Log("Fire Base Weapon");
            Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
            Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
            audioSource.Play();
        }   
    }

    public void SetupAudio(AudioClip clip){
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
    }


}
