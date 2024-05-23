using System.Collections;
using UnityEngine;

public class Burn : MonoBehaviour
{
    // Damage per tick
    public float burnDamage = 0.5f;
    public float burnDuration = 5f;
    public float burnTime = 1f;
    public bool isBurning;

    public GameObject projectileObj;
    Projectile projectile;

    PowerupManager powerupManager;
    EnemyPlane enemy;  // Store the enemy reference

    void Start()
    {
        projectileObj = gameObject;
        projectile = projectileObj.GetComponent<Projectile>();
        powerupManager = FindObjectOfType<PowerupManager>();
    }

    void OnDestroy()
    {
        if (isBurning  && enemy != null)
        {
            isBurning = true;
            Debug.Log("Raahhh");
            StartCoroutine(powerupManager.BurnDamageOverTime(enemy, burnDamage, burnDuration, burnTime));
        }
    }

    public void ApplyBurn(EnemyPlane enemy)
    {
        this.enemy = enemy;  // Store the enemy reference
        if (!isBurning && powerupManager != null)
        {
            isBurning = true;
            StartCoroutine(powerupManager.BurnDamageOverTime(enemy, burnDamage, burnDuration, burnTime));
        }
    }
}
