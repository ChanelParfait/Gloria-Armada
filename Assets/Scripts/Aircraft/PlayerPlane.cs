using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPlane : PlaneBase
{
    public static event UnityAction OnPlayerDamage;
    public static event UnityAction OnPlayerDeath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(int damage){
        OnPlayerDamage?.Invoke();
        base.TakeDamage(damage);
    }

    protected override void Die(){   
        OnPlayerDeath?.Invoke();
        base.Die();
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "EnemyProjectile"){
            // Take Damage? / Die
            currentHealth -= col.gameObject.GetComponent<Projectile>().projectileStats.damage;
            if(currentHealth <= 0){
                //isDead = true; 
            }
        }
    }

    
}
