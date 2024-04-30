using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaneBase : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
    
        //OnPlayerDamage?.Invoke();
        if(currentHealth <= 0){
            Die();
        }
    }

    protected virtual void Die(){
        // Death Function 
        Destroy(gameObject);
    }
}
