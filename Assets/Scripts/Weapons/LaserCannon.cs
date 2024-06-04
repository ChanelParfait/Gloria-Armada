using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class LaserCannon : Weapon
{
    //Remaining charge of weapon 
    public GameObject warningLaser;
    private float currentCharge;  
    private bool isReloading = false;
    private bool isFiring = false;
    private float[] damageLevels = {1, 1.5f, 2};
    private float holdTimer = 0;

    public float delayTime = 1;
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
        if(currentCharge <= 0 && isPlayerWeapon){
            StopFiring();
        }

        if (isFiring){
            UpdateDamage();
            float adjustedFireTime = weaponStats.fireTime / weaponStats.maxAmmo * currentCharge;
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

    public override void EnemyFire()
    {
        StartCoroutine(FireWarning());        
    }

    private void DelayedEnemyFire()
    {
        if(currentCharge > 0){
            Debug.Log("Enemy Laser Fire " + projectile);
            // Fire Laser 
            activeProjectile = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation(), transform); 
            laser = activeProjectile.GetComponent<Laser>(); 
            laser.UpdateStats(weaponStats.projectileStats, 4);
            // loop sound while firing
            PlaySound();
            currentCharge --;
            StartCoroutine(HoldEnemyFire(weaponStats.fireTime));
        }
        else{
            Debug.Log("Out of Charge");
        }
        
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
    }

    private IEnumerator FireWarning(){
        // instantiate warning laser
        GameObject line = Instantiate(warningLaser, GetSpawnPos(), GetSpawnRotation(), transform); 
        // wait for delay time
        yield return new WaitForSeconds(delayTime);
        // destroy warning laser
        Destroy(line);
        // fire weapon  
        DelayedEnemyFire();
    }


    public override void SetupWeapon(){
        currentCharge = weaponStats.maxAmmo;
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
            laser.UpdateStats(weaponStats.projectileStats, damageLevels.Length * 2);
        }
    }

    public override void StopFiring(){
        // destroy active projectile
        Debug.Log("Stop Firing ");

        if(activeProjectile){
            Destroy(activeProjectile);
            activeProjectile = null;
            laser = null;
        }
        audioSource.Stop();
        isFiring = false;
    }

    private IEnumerator HoldEnemyFire(float waitTime){
        Debug.Log("Hold for: " + waitTime);

        yield return new WaitForSeconds(waitTime);
        StopFiring();
    }

}
