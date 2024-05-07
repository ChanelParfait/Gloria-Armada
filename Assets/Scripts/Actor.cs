using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // Base Actor Class for Enemies and Player to Inherit From
    [SerializeReference] public int maxHealth;
    public float currentHealth {get; protected set;}

    protected bool isAlive = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isAlive){
            currentHealth -= damage;
            if(currentHealth <= 0){
                isAlive = false;
                Die();
            }
        }
    }

    protected virtual void Die(){
        // Death Function 
        // If any children are particle managers or have particle managers, detach them
        ParticleManager[] pms = GetComponentsInChildren<ParticleManager>();
        foreach (ParticleManager pm in pms)
        {
            pm.transform.SetParent(null);   
            pm.Detach();
        }
        Destroy(gameObject);
    }
}
