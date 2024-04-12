using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        // Find and Retrieve Missile Prefab from Resources Folder
        Object prefab = Resources.Load("Projectiles/Missile");
        projectile = (GameObject)prefab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Fire()
    {
        // base.Fire();
        Debug.Log("Missle Launched");
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
    }
}
