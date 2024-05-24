using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType {
    // PrimaryDamageUp,
    // BulletSpeedUp,
    // FirerateUp,
    // BulletSizeUp,
    BurnDamage,
    // FreezeShots,
    // ExplodingShots,
    // LightningChain,
    // SpecialDamageUp,
    SplitShot,
    // APDamage,
    // HomingShots
}

public class Powerups : MonoBehaviour
{
    [SerializeField] private PlayerWeaponManager playerWeaponManager; 
    // [SerializeField] private Projectile projectile; 
    [SerializeField] PowerupType powerupType;

    
    void Start()
    {
        playerWeaponManager = FindObjectOfType<PlayerWeaponManager>();
        powerupType = GetRandomPowerup();
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
        playerWeaponManager.AddPowerup(type);
    }

     private void OnTriggerEnter(Collider col)
    {
        // Check if the colliding object is the player
        if (col.gameObject.CompareTag("Player")) 
        {
            ApplyPowerupEffect(powerupType);
            // Display the name of the obtained powerup item on the screen
            Debug.Log("Obtained Powerup: " + powerupType);
            Destroy(gameObject);

        }
    }
}
