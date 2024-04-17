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
        SetupAudio();
        // if projectile is null
        if(!projectile){
            // Find and Retrieve Player Projectile Prefab from Resources Folder
            Object prefab = Resources.Load("Projectiles/Plasma_Player");
            projectile = (GameObject)prefab;
            Object audioPrefab = Resources.Load("Audio/Enemy_Plasma");
            fireSound = (AudioClip)audioPrefab;
            SetupAudio();
        }
        
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
        // base.Fire();
        if(canFire){
            Debug.Log("Machine Gun Fire");
            Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
            Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
            timer = 0;
            canFire = false;
            audioSource.Play(); 

        }
    }
}
