using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Burn : MonoBehaviour
{
    //damage per tick
    public float burnDamage = 0.5f;
    [SerializeField]
    float burnDuration = 5f;
    [SerializeField]
    float burnTime = 1f;
    public bool isBurning;
 
    public GameObject projectileObj;
    Projectile projectile;


    // Start is called before the first frame update
    void Start()
    {
        projectileObj = gameObject;
        projectile = projectileObj.GetComponent<Projectile>();
    }


    public void ApplyBurn(EnemyPlane enemy)
    {
        if (!isBurning)
        {
            isBurning = true;
            StartCoroutine(BurnDamageOverTime(enemy));
        }
    }

    //Triggers tick damage on the enemy for 5 seconds or until dead
    private IEnumerator BurnDamageOverTime(EnemyPlane enemy)
    {
        float elapsed;
        if (enemy != null)
            {
            for (elapsed = 0f; elapsed < burnDuration; elapsed += burnTime)
            {
                enemy.maxHealth -= burnDamage;
                Debug.Log("Burn damage applied. Current health: " + enemy.maxHealth);
                yield return new WaitForSeconds(burnTime);
            }
        }
        isBurning = false;
    }
}
