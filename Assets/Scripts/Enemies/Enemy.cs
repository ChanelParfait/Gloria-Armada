using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    EnemyWeaponManager weaponManager; 
    [SerializeField] private float fireInterval = 3;
    [SerializeField] private int totalHealth = 3;
    [SerializeField] private int speed = 3;
    public Vector3 moveDir;
    public int scoreValue = 10; 
    private int currentHealth;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {   
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        currentHealth = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveDir != null){
            MoveEnemy();
        }

        timer += Time.deltaTime; 
        if(timer >= fireInterval){
            timer = 0; 
            Fire();
        }
    }


    public void Fire(){
        weaponManager.FireActiveWeapon();
    }

    private void OnTriggerEnter(Collider col){
        // if hit by a player projectile
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage
            TakeDamage(col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            Debug.Log("Enemy Health:" + currentHealth);
            // Destroy Projectile
            Destroy(col.gameObject);
        }
    }

    void TakeDamage(int damage){
        currentHealth -= damage;
        if(currentHealth <= 0){
            Die();
        }
    }

    void Die(){
        // Trigger Enemy Death Event 
        Debug.Log("Enemy Death");
        Actions.OnEnemyDeath?.Invoke(this);
        // Destroy Self
        Destroy(gameObject);
    }

    private void MoveEnemy(){
        gameObject.transform.position += moveDir * Time.deltaTime * speed; 
    }

}
