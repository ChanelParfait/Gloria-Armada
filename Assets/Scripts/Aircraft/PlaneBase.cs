using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaneBase : MonoBehaviour
{
    [SerializeReference] public int maxHealth;
    public int currentHealth {get; protected set;}

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(maxHealth);
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
