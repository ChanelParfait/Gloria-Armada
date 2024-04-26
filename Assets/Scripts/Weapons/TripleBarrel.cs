using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleBarrel : Weapon
{
    private float timer = 0; 
    private int counter = 1;

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

    public override void Fire()
    {
        if(canFire){
            base.Fire();
            counter++;
            base.Fire();
            counter++;
            base.Fire();
            timer = 0;
            canFire = false;
        }
    } 

    public override void EnemyFire()
    {
        //Debug.Log("Enemy Machine Gun Fire");
        base.Fire();
    }

    public override void SetupWeapon(){
        weaponStats.fireInterval = 1f;
        weaponStats.projectileStats.damage = 1;
        weaponStats.projectileStats.speed = 6;

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

    private void PlayerUpdate(){
        // Increase timer
        timer += Time.deltaTime; 
        if(!canFire && timer >= weaponStats.fireInterval){
            canFire = true;
        }
    }

    public override Vector3 GetSpawnPos(){
        switch(counter){
            case 1:
                return gameObject.transform.position + gameObject.transform.forward * 8 + gameObject.transform.up * 3;
            case 2:
                return base.GetSpawnPos();
            case 3:
                return gameObject.transform.position + gameObject.transform.forward * 8 + gameObject.transform.up * -3;
            default:
                return base.GetSpawnPos();
        }
    }

    public override Quaternion GetSpawnRotation(){
        return base.GetSpawnRotation();
    }

    public override void PlaySound(){
        if(counter == 1){
            base.PlaySound();
        }
    }
}
