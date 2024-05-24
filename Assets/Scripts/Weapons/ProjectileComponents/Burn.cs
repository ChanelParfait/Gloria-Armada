using System.Collections;
using UnityEngine;

public class Burn : MonoBehaviour
{
    // Damage per tick
    public float dmg = 0.5f;
    public float duration = 5f;
    public float tickRate = 1f;
    public bool isBurning;

    public GameObject projectileObj;

    void Start()
    {
        projectileObj = gameObject;
    }

    public void ApplyBurn(EnemyBase enemy)
    {
        if (enemy != null){
            enemy.SetOnFire(burnDamage: dmg, burnDuration: duration, burnTime: tickRate); 
        }    
    }
}
