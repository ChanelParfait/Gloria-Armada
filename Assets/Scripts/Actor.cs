using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // Base Actor Class for Enemies and Player to Inherit From
    [SerializeReference] public int maxHealth;
    public int currentHealth {get; protected set;}

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0){
            Die();
        }
    }

    protected virtual void Die(){
        // Death Function 
        // If any children are particle managers or have particle managers, detach them
        ParticleManager[] pms = GetComponentsInChildren<ParticleManager>();
        foreach (ParticleManager pm in pms)
        {
            pm.transform.SetParent(null);   
            pm.Detatch();
        }
        Destroy(gameObject);
    }
}
