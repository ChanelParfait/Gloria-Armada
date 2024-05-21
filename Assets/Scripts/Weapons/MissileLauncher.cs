using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Missile launcher fires homing missiles
// four missiles can be shot in a row before incuring a cooldown period. 
public class MissileLauncher : Weapon
{
    // Enemy Weapon Values
    private bool fireLeft = true;

    //Player Weapon Stats 
    private int currentAmmo; 
    private float firerateTimer = 0; 
    private float reloadTimer = 0; 
    private bool isReloading = false;

    // time it takes for the ammo to be replensished by 1
    private const float replenTime = 4f;

    // Start is called before the first frame update
    void Start()
    {
        SetupWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        reloadTimer += Time.deltaTime;
        firerateTimer += Time.deltaTime;

        if(firerateTimer >  weaponStats.fireInterval && !canFire){
            canFire = true;
        }

        if(!isReloading){
            // if out of ammo, the player cannot fire and must reload
            if(currentAmmo == 0){
                StartCoroutine(StartReload());
            }

            // replenish ammo by 1 after refresh time has passed
            if(reloadTimer > replenTime && currentAmmo < weaponStats.maxAmmo){
                IncreaseAmmo();
            }
        }
    }
    private void IncreaseAmmo(){
        // Add one to ammo and reset reload timer
        currentAmmo ++;
        reloadTimer = 0;
        Debug.Log("Ammo: " + currentAmmo);
        OnAmmoChange?.Invoke(currentAmmo);
        
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
        if(canFire && currentAmmo != 0){
            //Debug.Log("Missle Launched");
            base.Fire(velocity);
            // Decrement Ammo
            currentAmmo --;
            OnAmmoChange?.Invoke(currentAmmo);
            canFire = false;
            // Reset Timers
            reloadTimer = 0;
            firerateTimer = 0;
        }
        Debug.Log("Ammo: " + currentAmmo);

    }

    public override void EnemyFire()
    {   
            //Debug.Log("Enemy Missile Fire");
            base.EnemyFire();
            // Decrement Ammo
            //currentAmmo --;
    }

    public override void SetupWeapon(){
        currentAmmo = weaponStats.maxAmmo;

        if(!projectile){
            // Find and Retrieve Missile Prefab from Resources Folder
            projectile = (GameObject)Resources.Load("Projectiles/Missile_Player");
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;

        if(!fireSound){
            fireSound = (AudioClip)Resources.Load("Audio/Rockets/launch");
            if (fireSound){
                audioSource.clip = fireSound;
            }
            
        }
    }

    public override Vector3 GetSpawnPos()
    {   

        if(isPlayerWeapon){
            // projectile spawn position for the player remains the same
            return base.GetSpawnPos();
        }
        else{
            // to get enemy spawn position
            // switch between left and right between spawn positions on enemy model
            if(fireLeft){
                fireLeft = false;
                return gameObject.transform.position + gameObject.transform.forward * 1.32f + gameObject.transform.right * -2.055f;
            }
            else{
                fireLeft = true;    
                return gameObject.transform.position + gameObject.transform.forward * 1.32f + gameObject.transform.right * 2.055f;
            }
        }
    }
}
