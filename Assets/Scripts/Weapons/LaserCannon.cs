using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LaserCannon : Weapon
{
    //Player Weapon Stats 
    private int currentAmmo; 
    //private float firerateTimer = 0; 
    private float reloadTimer = 0; 
    private bool isReloading = false;


    // Start is called before the first frame update
    void Start()
    {
        SetupWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        reloadTimer += Time.deltaTime;
        //firerateTimer += Time.deltaTime;

        /*if(firerateTimer >  weaponStats.fireInterval && !canFire){
            canFire = true;
        }*/

        if(!isReloading){
            // if out of ammo, the player cannot fire and must reload
            if(currentAmmo == 0){
                StartCoroutine(StartReload());
            }
        }
    }

    private IEnumerator StartReload(){
        // start the reloading process
        isReloading = true; 
        yield return new WaitForSeconds(weaponStats.reloadTime);
        FinishReload();
    }

    private void FinishReload(){
        // returns the weapon to max ammo
        Debug.Log("Full Reload Complete");
        currentAmmo = weaponStats.maxAmmo; 
        isReloading = false;
        OnAmmoChange?.Invoke(currentAmmo);
    }

    public override void Fire(Vector3 velocity)
    {
        if(currentAmmo != 0){
            Debug.Log("Fire Cannon");
            //Debug.Log("Missle Launched");
            base.Fire(velocity);
            // Decrement Ammo
            currentAmmo --;
            OnAmmoChange?.Invoke(currentAmmo);
            // Reset Timers
            reloadTimer = 0;
        }
        Debug.Log("Ammo: " + currentAmmo);

    }


    public override void SetupWeapon(){
        currentAmmo = weaponStats.maxAmmo;
        OnAmmoChange?.Invoke(currentAmmo);

        if(!projectile){
            // Find and Retrieve Missile Prefab from Resources Folder
            projectile = (GameObject)Resources.Load("Projectiles/Missile_Player");
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;

        if(!fireSound){
            fireSound = (AudioClip)Resources.Load("Audio/Rocket_Sound");
            audioSource.clip = fireSound;
        }
    }

    public override Vector3 GetSpawnPos()
    {   
        return base.GetSpawnPos() + gameObject.transform.forward * weaponStats.projectileStats.size.z; 
    }
}
