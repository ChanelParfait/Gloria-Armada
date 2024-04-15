using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Missile launcher fires homing missiles
// four missiles can be shot in a row before incuring a cooldown period. 
public class MissileLauncher : Weapon
{
    private const int maxAmmo = 4; 
    private int currentAmmo = maxAmmo; 
    private float cooldownTimer = 0; 
    private float reloadTimer = 0; 
    private bool canFire = true;
    private bool reloading = false;
    // a short delay time between the player being able to fire
    // to prevent spamming a weapon
    private const float cooldownTime = 0.25f;
    // time it takes for the ammo to be replensished by 1
    private const float refreshTime = 1.5f;
    // time it takes for ammo to fully replensish after being depleted 
    private const float reloadTime = 5;



    // Start is called before the first frame update
    void Start()
    {
        // Find and Retrieve Missile Prefab from Resources Folder
        Object prefab = Resources.Load("Projectiles/Missile");
        projectile = (GameObject)prefab;
    }

    // Update is called once per frame
    void Update()
    {
        

        reloadTimer += Time.deltaTime;
        cooldownTimer += Time.deltaTime;



        if(!reloading){
            // if out of ammo, the player cannot fire and must reload
            if(currentAmmo == 0){
                canFire = false;
                reloading = true; 
                reloadTimer = 0;
            }

            if(cooldownTimer > cooldownTime && !canFire){
                canFire = true;
            }
            
            // replensish ammo after refresh time has passed
            if(reloadTimer > refreshTime && currentAmmo < maxAmmo){
                Debug.Log("Ammo Replenished");

                currentAmmo ++;
                reloadTimer = 0;
                Debug.Log(currentAmmo);

            }
        }
        else{
            if(reloadTimer > reloadTime){
                Debug.Log("Full Reload Complete");
                // reload
                currentAmmo = maxAmmo; 
                reloading = false;
                canFire = true;
                reloadTimer = 0;
            }
        }
    }

    public override void Fire()
    {
        // base.Fire();
        if(canFire){
            Debug.Log("Missle Launched");
            Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
            Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
            canFire = false;
            currentAmmo --;
            reloadTimer = 0;
            cooldownTimer = 0;
        }
        Debug.Log(currentAmmo);

    }
}
