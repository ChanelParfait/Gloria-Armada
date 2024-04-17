using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWeaponManager : WeaponManager
{
    public Weapon testWeapon1;
    public Weapon testWeapon2;
    // Start is called before the first frame update
    void Start()
    {
        isEnemyManager = false;
        SetPrimaryWeapon(testWeapon1);
        //SetPrimaryWeapon(testWeapon2);
    }

    // Update is called once per frame
    void Update()
    {
        // Fire Primary Weapon when pressing Q
        if(Input.GetKeyDown(KeyCode.Q)){
            FirePrimaryWeapon();
        }
        // Fire Special Weapon when pressing E
        if(Input.GetKeyDown(KeyCode.E)){
            FireSpecialWeapon();
        }
    }

    public void SetPrimaryWeapon(Weapon weapon){
        // add a weapon component to this object
        Type weaponType = weapon.GetType();
        // store it as the primary weapon
        primaryWeapon = (Weapon)gameObject.AddComponent(weaponType); 
        // use this line to setup the weapon component using a prefab object
        primaryWeapon.SetProjectile(weapon.GetProjectile());
        primaryWeapon.SetFireSound(weapon.GetFireSound());
    }

    public void SetSpecialWeapon(Weapon weapon){
         // add a weapon component to this object
        Type weaponType = weapon.GetType();
        gameObject.AddComponent(weaponType); 
        // store it as the special weapon 
        specialWeapon = (Weapon)gameObject.GetComponent(weaponType);
        // use this line to setup the weapon component using a prefab object
        primaryWeapon.SetProjectile(weapon.GetProjectile());

    }
}
