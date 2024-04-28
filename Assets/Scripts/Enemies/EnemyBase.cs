using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int scoreValue = 10; 
    [SerializeField] private int totalHealth = 3;
    public int currentHealth {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = totalHealth;
    }

    public virtual void Fire(){

    }

    public void TakeDamage(int damage){
        currentHealth -= damage;
        if(currentHealth <= 0){
            Die();
        }
    }

    public virtual void Die(){
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
