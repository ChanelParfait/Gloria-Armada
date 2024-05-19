using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternShot : Weapon
{
    private float firerateTimer = 0; 

    //Player Weapon Stats 
    private int currentAmmo; 
    private float reloadTimer = 0; 
    private bool isReloading = false;

    Transform spawnTransform;

    public List<FiringPattern_SO> firingPatterns = new List<FiringPattern_SO>();
    private FiringPattern_SO defaultPattern;

    // Start is called before the first frame update

    void Awake(){
        defaultPattern = ScriptableObject.CreateInstance<SingleShotPattern>();
        spawnTransform = gameObject.transform;
    }
    void Start()
    {
        SetupWeapon();
        canFire = true;
        LevelManager lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (lm != null){
            currentPerspective = lm.currentPerspective;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerWeapon){
            PlayerUpdate();
        }
    }

    public override void Fire(Vector3 velocity)
    {       
        firerateTimer += Time.deltaTime;
        if(firerateTimer >  weaponStats.fireInterval && !canFire){
            canFire = true;
        }
        // If the weapon doesn't have a maximum ammo, fire as normal
        if (weaponStats.maxAmmo == 0){
            if(canFire){
                base.Fire(velocity);
                firerateTimer = 0;
                canFire = false;
            }
        }
        else{
            reloadTimer += Time.deltaTime;
            // If the weapon has a maximum ammo, check if the player has ammo
            if(currentAmmo > 0 && canFire){
                base.Fire(velocity);
                firerateTimer = 0;
                canFire = false;
                currentAmmo --;
            }
            else if(currentAmmo == 0 && !isReloading){
                //Debug.Log("Out of Ammo");
                StartCoroutine(StartReload());
            }
        }
    } 

    public override void EnemyFire()
    {
        spawnTransform.SetPositionAndRotation(GetSpawnPos(), GetSpawnRotation());
        if (firingPatterns == null || firingPatterns.Count == 0)
        {
            // Use the default single shot pattern if no patterns are in the list
            StartCoroutine(defaultPattern.Fire(projectile, spawnTransform, weaponStats, this, currentPerspective));        }
        else
        { 
            //Pick a random pattern from the list
            int randomPattern = Random.Range(0, firingPatterns.Count);
            StartCoroutine(firingPatterns[randomPattern].Fire(projectile, spawnTransform, weaponStats, this, currentPerspective));
        }
        PlaySound();
    }

    public override Vector3 GetSpawnPos()
    {
        // return the default spawn position
        return gameObject.transform.position + gameObject.transform.forward * 4;
    }

    private IEnumerator StartReload(){
        // start the reloading process
        isReloading = true; 
        yield return new WaitForSeconds(weaponStats.reloadTime);
        FinishReload();
    }

    private void FinishReload(){
        // returns the weapon to max ammo
        //Debug.Log("Full Reload Complete");
        currentAmmo = weaponStats.maxAmmo; 
        isReloading = false;
        OnAmmoChange?.Invoke(currentAmmo);
    }

    public override void SetupWeapon(){
        //weaponStats.fireInterval = 0.5f;
        //weaponStats.projectileStats.damage = 1;
        //weaponStats.projectileStats.speed = 10;


        if(!projectile){
            Object prefab = Resources.Load("Projectiles/Plasma_Player");
            projectile = (GameObject)prefab;
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;

        if(!fireSound){
           Object audioPrefab = Resources.Load("Audio/Enemy_Plasma");
            fireSound = (AudioClip)audioPrefab;
        }
    }

    private void PlayerUpdate(){
        // Increase timer
        firerateTimer += Time.deltaTime; 
        if(!canFire && firerateTimer >= weaponStats.fireInterval){
            canFire = true;
        }
    }
}
