using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyPlane : EnemyBase
{
    [SerializeField] private float fireInterval = 1;
    [SerializeField] private int speed = 8;
    public float referenceSpeed = 0;
    public Vector3 moveDir;
    public Vector3 orientation;
    private float timer = 0;
    public GameObject deathExplosion;

    [SerializeField] private GameObject deathObj;

    
    // Start is called before the first frame update
    protected override void Start()
    {   
        base.Start();
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(moveDir != null){
            MoveEnemy();
        }

        timer += Time.deltaTime; 
        if(timer >= Random.Range(fireInterval, 3f)){
            timer = 0; 
            Fire();
        }
    }

    private void OnTriggerEnter(Collider col){
        // if hit by a player projectile
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage
            TakeDamage(col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            //Debug.Log("Enemy Health:" + currentHealth);
            //Debug.Log("Enemy Damage Taken:" + col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            // Destroy Projectile
            Destroy(col.gameObject);
        }
    }

    public override void TakeDamage(float damage){
        base.TakeDamage(damage);
    }

    protected override void Die(){
        // Destroy Self and emit death explosion
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        if (deathObj != null)
        {
            GameObject deadObj = Instantiate(deathObj, transform.position, transform.rotation);
            foreach (Rigidbody rb in deadObj.GetComponentsInChildren<Rigidbody>())
            {
                //Add force to the rigid body
                rb.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                // Using the offset of the child from the parent, apply the appropriate velocity from the angular velocity
                rb.AddTorque(GetComponent<Rigidbody>().angularVelocity, ForceMode.VelocityChange);
            }
        }

        base.Die();
    }

    protected virtual void MoveEnemy(){
        Vector3 referenceMovement = new Vector3(referenceSpeed, 0, 0) * Time.deltaTime;
        Vector3 enemyMovement = moveDir * Time.deltaTime * speed;
        gameObject.transform.position += enemyMovement + referenceMovement; 
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            //Get the normal of the collision
            Vector3 normal = col.contacts[0].normal;
            //Get dot product of the normal and the velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            float dot = Vector3.Dot(rb.velocity.normalized, normal);
            
            //Debug.Log(dot);

            dot = Mathf.Clamp01(dot * 5);
            
            //Reduce health by a minimum of 1health, max of MaxLife based on dot
            int damage = (int)Mathf.Lerp(1,maxHealth, dot);

            TakeDamage(damage);
        }
    }

}
