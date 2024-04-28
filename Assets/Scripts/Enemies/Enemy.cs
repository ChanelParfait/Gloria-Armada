using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : EnemyBase
{
    
    // Base Class for enemies 
    EnemyWeaponManager weaponManager; 
    [SerializeField] private float fireInterval = 3;

    public float referenceSpeed = 0;
    public int speed = 8;
    public Vector3 moveDir;
    public Vector3 orientation;
    private float timer = 0;

    public GameObject deathExplosion;

    // Start is called before the first frame update
    void Start()
    {   
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
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


    override public void Fire(){
        weaponManager.FireActiveWeapon();
    }

    private void OnTriggerEnter(Collider col){
        // if hit by a player projectile
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage
            //TakeDamage(col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            Debug.Log("Enemy Health:" + currentHealth);
            //Debug.Log("Enemy Damage Taken:" + col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            // Destroy Projectile
            //Destroy(col.gameObject);
        }
    }

    override public void Die(){
        // Trigger Enemy Death Event 
        Debug.Log("Enemy Death");
        Actions.OnEnemyDeath?.Invoke(this);
        // Destroy Self and emit death explosion
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void MoveEnemy(){
        Vector3 referenceMovement = new Vector3(referenceSpeed, 0, 0) * Time.deltaTime;
        Vector3 enemyMovement = moveDir * Time.deltaTime * speed;

        gameObject.transform.position += enemyMovement + referenceMovement; 
    }

}
