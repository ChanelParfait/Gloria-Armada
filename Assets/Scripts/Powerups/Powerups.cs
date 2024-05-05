using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PowerupManager powerupManager; 
    [SerializeField] private PlayerWeaponManager playerWeaponManager; 
    [SerializeField] private Projectile projectile; 



    public enum PowerupType {
        DamageUp,
        BulletSpeedUp,
        FirerateUp,
        BulletSizeUp
        
    }

    public PowerupType powerupType;

    
    void Start()
    {
        playerWeaponManager = FindObjectOfType<PlayerWeaponManager>();
        powerupManager = FindObjectOfType<PowerupManager>();
        ApplyPowerupEffect(powerupType);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


     private PowerupType GetRandomPowerup()
    {
        return (PowerupType)Random.Range(0, System.Enum.GetValues(typeof(PowerupType)).Length);
    }

     private void ApplyPowerupEffect(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.DamageUp:
                ProjectileStats currentStats = playerWeaponManager.GetPrimaryWeapon().GetProjectileStats();
                currentStats.damage *= 2; // Double bullet speed             
                Debug.Log("Damage up");
                break;
            case PowerupType.BulletSpeedUp:
                currentStats = playerWeaponManager.GetPrimaryWeapon().GetProjectileStats();
                currentStats.speed *= 1.25f; // Double bullet speed             
                //playerWeaponManager.GetPrimaryWeapon().weaponStats.projectileStats(currentStats);   
                Debug.Log("Bullet Speed Up");
                break;
        }
    }

     private void OnTriggerEnter(Collider col)
    {
        // Check if the colliding object is the player
        if (col.gameObject.CompareTag("Player")) 
        {

            //Apply the Powerup Effect
            Powerups powerupInstance = col.GetComponent<Powerups>();
            if (powerupInstance != null)
            {
                powerupInstance.ApplyPowerupEffect(powerupType);
            }

            // Display the name of the obtained powerup item on the screen
            // Debug.Log("Collision Detected");
            Destroy(gameObject);

        }
    }
}
