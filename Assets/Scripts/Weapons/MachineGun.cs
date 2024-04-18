using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    private float timer = 0; 
    private const float cooldownTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

        SetupWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(!canFire && timer >= cooldownTime){
            canFire = true;
        }
    }

    public override void Fire()
    {
        if(canFire){
            //Debug.Log("Machine Gun Fire");
            Debug.Log(weaponStats.projectileStats.damage);
            spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
            GameObject clone = Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
            clone.GetComponent<Projectile>().SetStats(weaponStats.projectileStats); 

            timer = 0;
            canFire = false;
            audioSource.clip = fireSound;
            audioSource.Play();
        }
    }

    public override void EnemyFire()
    {
        //Debug.Log("Enemy Machine Gun Fire");
        spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
        GameObject clone = Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
        clone.GetComponent<Projectile>().SetStats(weaponStats.projectileStats);
        audioSource.Play();
    }

    public override void SetupWeapon(){
        weaponStats.projectileStats.damage = 1;
        weaponStats.projectileStats.speed = 5;


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
}
