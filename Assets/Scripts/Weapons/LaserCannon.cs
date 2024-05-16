using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;

public class LaserCannon : Weapon
{
    //Remaining charge of weapon 
    private float currentCharge; 
    public float fireTime;
    //private float firerateTimer = 0; 
    private float reloadTimer = 0; 
    private bool isReloading = false;
    private bool isFiring = false;
    private int[] damageLevels = {1, 2, 4, 8};
    private float holdTimer = 0;
    // reference to projectile 
    GameObject activeProjectile;
    Laser laser;


    // Start is called before the first frame update
    void Start()
    {
        SetupWeapon();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        reloadTimer += Time.deltaTime;
        if(currentCharge <= 0){
            StopFiring();
        }

        if (isFiring){
            UpdateDamage();
            float adjustedFireTime = fireTime / weaponStats.maxAmmo * currentCharge;
            currentCharge = Mathf.Lerp(currentCharge, 0, Time.deltaTime / adjustedFireTime);
            OnAmmoChange?.Invoke(currentCharge);
        } 
        if(isReloading){
            float adjustedReloadTime = weaponStats.reloadTime - (weaponStats.reloadTime / weaponStats.maxAmmo * currentCharge);
            currentCharge = Mathf.Lerp(currentCharge, weaponStats.maxAmmo, Time.deltaTime / adjustedReloadTime);
            OnAmmoChange?.Invoke(currentCharge);
            if(currentCharge >= weaponStats.maxAmmo){
                isReloading = false;
                currentCharge = weaponStats.maxAmmo;
                Debug.Log("Full Reload Complete");
            }
        }

    }

    public override void Fire(Vector3 velocity)
    {
        if(currentCharge != 0){
            //Debug.Log("Fire Cannon");
            activeProjectile = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation(), transform); 
            laser = activeProjectile.GetComponent<Laser>(); 
            laser.UpdateStats(weaponStats.projectileStats, damageLevels.Length);
            // loop sound while firing
            PlaySound();
            isFiring = true;
            isReloading = false;
        }
        //Debug.Log("Charge: " + currentCharge);

    }

    
    // Function runs when holding down the weapon key 
    // Intended for special weapons with hold features
    public override void Hold(){
        // Increase hold counter
        holdTimer += Time.deltaTime;
        //currentCharge = 0;
        OnAmmoChange?.Invoke(currentCharge);
    }

    // Function for when the weapon key is released
    // Intended for special weapons with hold features
    public override void Release(){
        StopFiring();
        // start reloading
        //StartCoroutine(StartReload());
        isReloading = true; 
        holdTimer = 0;
        reloadTimer = 0;
    }


    public override void SetupWeapon(){
        currentCharge = weaponStats.maxAmmo;
        OnAmmoChange?.Invoke(currentCharge);
        // start at lowest damage level
        weaponStats.projectileStats.damage = damageLevels[0];

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

    private void UpdateDamage(){
        if(holdTimer <= 1){
            weaponStats.projectileStats.damage = damageLevels[0];   
        }
        else if(holdTimer <= 2){
            weaponStats.projectileStats.damage = damageLevels[1];
        }
        else if(holdTimer <= 3){
            weaponStats.projectileStats.damage = damageLevels[2];
        }
        else if(holdTimer <= 4){
            weaponStats.projectileStats.damage = damageLevels[3];
        }
        if(laser){
            laser.UpdateStats(weaponStats.projectileStats, damageLevels.Length);
        }
    }

    private void StopFiring(){
        // destroy active projectile
        if(activeProjectile){
            Destroy(activeProjectile);
            activeProjectile = null;
            laser = null;
        }
        isFiring = false;
    }

}
