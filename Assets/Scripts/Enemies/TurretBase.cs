using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : EnemyBase
{
    [SerializeField] private float fireInterval = 1;
    public TurretMount turretMount;

    public GameObject deathExplosion;

    protected float randFireTime;
    protected float timer = 0;
    



    // Start is called before the first frame update
    protected override void Start()
    {
        if (transform.parent != null){
            if (transform.parent.GetComponent<TurretMount>() != null){
                turretMount = transform.parent.GetComponent<TurretMount>();
            }
        }
        weaponManager = gameObject.GetComponentInChildren<EnemyWeaponManager>();
        TryGetComponent<Rigidbody>(out rb);
        if (rb == null)
        {
            rb = transform.root.GetComponent<Rigidbody>();
        }

        randFireTime = Random.Range(1f, 2.0f);
        timer = fireInterval - 1; 
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= fireInterval * randFireTime){
            randFireTime = Random.Range(0.5f, 2.0f);
            timer = 0; 
            Fire();
        }
    }

    protected override void Die(){
        //Alert turretMount that this turret has died
        if (turretMount != null){
            turretMount.TurretDied();
        }
        // Destroy Self and emit death explosion
        Instantiate(deathExplosion, transform.position, Quaternion.identity);

        base.Die();
    }
}
