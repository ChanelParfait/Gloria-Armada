using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    private float timer = 0; 

    // Start is called before the first frame update
    void Start()
    {
        SetupWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerWeapon){
            PlayerUpdate();
        }
    }

    public override void Fire(Vector3 velocity)
    {
        if(canFire){
            base.Fire(velocity);
            timer = 0;
            canFire = false;
        }
    } 

    /*public override void EnemyFire()
    {
        //Debug.Log("Enemy Machine Gun Fire");
        base.Fire();
    }*/

    public override void SetupWeapon(){
        //weaponStats.fireInterval = 0.5f;
        //weaponStats.projectileStats.damage = 1;
        //weaponStats.projectileStats.speed = 10;


        if(!projectile){
            Object prefab = Resources.Load("Projectiles/Plasma_Player");
            projectile = (GameObject)prefab;
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;

        if(!fireSound){
           Object audioPrefab = Resources.Load("Audio/Enemy_Plasma");
            fireSound = (AudioClip)audioPrefab;
        }
    }

    private void PlayerUpdate(){
        // Increase timer
        timer += Time.deltaTime; 
        if(!canFire && timer >= weaponStats.fireInterval){
            canFire = true;
        }
    }
}
