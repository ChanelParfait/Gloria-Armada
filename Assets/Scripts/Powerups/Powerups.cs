using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType {
    PrimaryDamageUp,
    BulletSpeedUp,
    FirerateUp,
    BulletSizeUp,
    BurnDamage,
    // FreezeShots,
    // ExplodingShots,
    LightningChain,
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

    GameObject BoxMesh;

    public GameObject emptyCratePrefab;

    public GameObject primaryDamageUpPrefab;
    public GameObject bulletSpeedUpPrefab;
    public GameObject fireRateUpPrefab;
    public GameObject bulletSizeUpPrefab;
    public GameObject burnDamagePrefab;
    public GameObject freezeShotsPrefab;
    public GameObject explodingShotsPrefab;
    public GameObject lightningChainPrefab;
    public GameObject specialDamageUpPrefab;
    public GameObject splitShotPrefab;

    
    void Start()
    {
        playerWeaponManager = FindObjectOfType<PlayerWeaponManager>();
        powerupType = GetRandomPowerup();
        // Set the box mesh to the powerup type
        BoxMesh = powerupType switch
        {
            PowerupType.PrimaryDamageUp => primaryDamageUpPrefab,
            PowerupType.BulletSpeedUp => bulletSpeedUpPrefab,
            PowerupType.FirerateUp => fireRateUpPrefab,
            PowerupType.BulletSizeUp => bulletSizeUpPrefab,
            PowerupType.BurnDamage => burnDamagePrefab,
            PowerupType.SplitShot => splitShotPrefab,
            PowerupType.LightningChain => lightningChainPrefab,
            _ => emptyCratePrefab,
        };

        if (BoxMesh == null)
        {
            BoxMesh = emptyCratePrefab;
            return;
        }
        // Instantiate the powerup box mesh as a child object
        Instantiate(BoxMesh, transform.position, Quaternion.identity, transform);

        
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
