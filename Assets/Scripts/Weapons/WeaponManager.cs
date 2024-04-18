using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager Class for Player and Enemy Weapons
// In Loadout Menu, use weapon manager to add weapons to player
// For enemies, weapons can be manually configured in editor
public class WeaponManager : MonoBehaviour
{
    // primary and special weapon will never be the same weapon
    public Weapon primaryWeapon;
    public Weapon specialWeapon;
    // indicates who this manager belongs to
    // by default the base class belongs to an enemy

    // Start is called before the first frame update
    void Start()
    {

    }
    public virtual void FirePrimaryWeapon(){
        if(primaryWeapon){
            //Debug.Log("Fire Primary Weapon");
            primaryWeapon.Fire();
        }
        
    }

    public virtual void FireSpecialWeapon(){
        if(specialWeapon){
            specialWeapon.Fire();
        }
    }
}
