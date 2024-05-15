using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;

public class LaserCannon : Weapon
{
    //Remaining charge of weapon 
    private float currentCharge; 
    //private float firerateTimer = 0; 
    private float reloadTimer = 0; 
    private bool isReloading = false;

    private int[] damageLevels = {1, 2, 3, 4};
    private int holdCounter = 0;
    // reference to projectile 
    GameObject activeProjectile;


    // Start is called before the first frame update
    void Start()
    {
        SetupWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        reloadTimer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.E)){
            holdCounter ++;
            currentCharge = 0;
            OnAmmoChange?.Invoke(currentCharge);
        }
        if(Input.GetKeyUp(KeyCode.E)){
            // stop firing
            if(activeProjectile){
                Destroy(activeProjectile);
                activeProjectile = null;
            }
            // start reloading
            StartCoroutine(StartReload());
        }

        if(isReloading){
            float adjustedReloadTime = weaponStats.reloadTime - (weaponStats.reloadTime / weaponStats.maxAmmo * currentCharge);
            currentCharge = Mathf.Lerp(currentCharge, weaponStats.maxAmmo, Time.deltaTime / adjustedReloadTime);
            OnAmmoChange?.Invoke(currentCharge);
        }

    }

    private IEnumerator StartReload(){
        // start the reloading process
        isReloading = true; 
        // find reload time based on current charge left
        float adjustedReloadTime = weaponStats.reloadTime - (weaponStats.reloadTime / weaponStats.maxAmmo * currentCharge);
        yield return new WaitForSeconds(adjustedReloadTime);
        FinishReload();
    }

    private void FinishReload(){
        // returns the weapon to full charge
        Debug.Log("Full Reload Complete");
        currentCharge = weaponStats.maxAmmo; 
        isReloading = false;
        //OnAmmoChange?.Invoke(currentCharge);
    }

    public override void Fire(Vector3 velocity)
    {
        if(currentCharge != 0){
            Debug.Log("Fire Cannon");
            activeProjectile = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation(), transform); 
            activeProjectile.GetComponent<Laser>().Launch(weaponStats.projectileStats); 
            // loop sound while firing
            PlaySound();
            // Decrement Charge
            
            OnAmmoChange?.Invoke(currentCharge);
            reloadTimer = 0;
        }
        Debug.Log("Charge: " + currentCharge);

    }


    public override void SetupWeapon(){
        currentCharge = weaponStats.maxAmmo;
        OnAmmoChange?.Invoke(currentCharge);
        weaponStats.projectileStats.damage = damageLevels[0];

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

}
