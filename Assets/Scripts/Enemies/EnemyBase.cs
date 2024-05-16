using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class EnemyBase : Actor
{
    
    // Events
    public static UnityAction<EnemyBase> OnEnemyDeath;
    protected EnemyWeaponManager weaponManager; 

    [SerializeField] protected Rigidbody rb;

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
        if (weaponManager != null)
        {
            weaponManager.FireActiveWeapon();
        }
        
    }

    protected override void Die(){ 
        Debug.Log("Enemy Died: " + name + "UniqueID: " + GetInstanceID());  
        OnEnemyDeath?.Invoke(this);
        base.Die();
    }

}
