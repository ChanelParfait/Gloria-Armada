using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    
    // Start is called before the first frame update
    void Start()
    {

        if(!isEnemyWeapon){
            // Find and Retrieve Player Projectile Prefab from Resources Folder
            Object prefab = Resources.Load("Projectiles/Plasma_Player");
            projectile = (GameObject)prefab;
        }
        else{
            // If this weapon belongs to an enemy
            // Find and Retrieve Enemy Projectile Prefab from Resources Folder
            Object prefab = Resources.Load("Projectiles/Plasma_Enemy");
            projectile = (GameObject)prefab;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {
        // base.Fire();
        Debug.Log("Machine Gun Fire");
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
    }
}
