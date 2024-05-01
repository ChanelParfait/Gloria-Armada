using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : Actor
{
    
    // Events
    public static UnityAction<EnemyBase> OnEnemyDeath;
    protected EnemyWeaponManager weaponManager; 


    // How much the score increases on enemy death
    public int scoreValue = 10; 

    // Start is called before the first frame update
    override protected void Start()
    {
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        base.Start();
    }

    protected void Setup(){
        
    }

    public virtual void Fire(){
        // Fire a Weapon
        weaponManager.FireActiveWeapon();
    }

    protected override void Die(){   
        OnEnemyDeath?.Invoke(this);
        base.Die();
    }

}
