using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerupType {
    PrimaryDamageUp,
    BulletSpeedUp,
    FirerateUp,
    BulletSizeUp,
    BurnDamage,
    FreezeShots,
    // ExplodingShots,
    LightningChain,
    SpecialDamageUp,
    SplitShot,
    // APDamage,
    // HomingShots
}

public class Powerups : MonoBehaviour
{
    [SerializeField] private PlayerWeaponManager playerWeaponManager; 

    PowerupManager powerupManager;
    // [SerializeField] private Projectile projectile; 
    [SerializeField] PowerupType powerupType;

    GameObject BoxMesh;

    Image powerupIcon;

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

    
    public Image primaryDamageUpImage;
    public Image bulletSpeedUpImage;
    public Image fireRateUpImage;
    public Image bulletSizeUpImage;
    public Image burnDamageImage;
    public Image freezeShotsImage;
    public Image lightningChainImage;
    public Image specialDamageUpImage;
    public Image splitShotImage;
    
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
            // PowerupType.SpecialDamageUp => specialDamageUpPrefab,
            PowerupType.FreezeShots => freezeShotsPrefab,
            _ => emptyCratePrefab,
        };
        powerupIcon = powerupType switch
        {
            PowerupType.PrimaryDamageUp => primaryDamageUpImage,
            PowerupType.BulletSpeedUp => bulletSpeedUpImage,
            PowerupType.FirerateUp => fireRateUpImage,
            PowerupType.BulletSizeUp => bulletSizeUpImage,
            PowerupType.BurnDamage => burnDamageImage,
            PowerupType.SplitShot => splitShotImage,
            PowerupType.LightningChain => lightningChainImage,
            PowerupType.SpecialDamageUp => specialDamageUpImage,
            PowerupType.FreezeShots => freezeShotsImage,
            _ => emptyImage,
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
        // Get the sprite for the collected powerup from the prefab
        Sprite powerupSprite = GetPowerupSprite(type);
        if (powerupSprite != null && powerupManager != null)
        {
            powerupManager.AddPowerup(powerupSprite);
        }
    }

    private Sprite GetPowerupSprite(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.PrimaryDamageUp:
                return primaryDamageUpPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.BulletSpeedUp:
                return bulletSpeedUpPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.FirerateUp:
                return fireRateUpPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.BulletSizeUp:
                return bulletSizeUpPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.BurnDamage:
                return burnDamagePrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.FreezeShots:
                return freezeShotsPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.LightningChain:
                return lightningChainPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.SpecialDamageUp:
                return specialDamageUpPrefab.GetComponent<SpriteRenderer>().sprite;
            case PowerupType.SplitShot:
                return splitShotPrefab.GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
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
