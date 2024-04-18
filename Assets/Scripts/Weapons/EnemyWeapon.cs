using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        SetupAudio();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Fire()
    {
            Debug.Log("Enemy Gun Fire");
            Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
            Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
            audioSource.Play();
    }
}
