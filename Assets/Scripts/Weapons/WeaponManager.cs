using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager Class for Player and Enemy Weapons
// In Loadout Menu, add selected weapon components to player
// Then the weapon manager will find, organise and manage them 
public class WeaponManager : MonoBehaviour
{
    public Weapon primaryWeapon;
    public Weapon specialWeapon;

    // Start is called before the first frame update
    void Start()
    {
        // Get weapon component and organise them accordingly
        Weapon[] Weapons  = GetComponents<Weapon>();
        foreach (Weapon weapon in Weapons){
            if (weapon.weaponCategory == WeaponCategories.Primary){
                SetPrimaryWeapon(weapon);
            }
            else if(weapon.weaponCategory == WeaponCategories.Special){
                SetSpecialWeapon(weapon);
            }
        }

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

    private void SetPrimaryWeapon(Weapon weapon){
        primaryWeapon = weapon;
    }

    private void SetSpecialWeapon(Weapon weapon){
        specialWeapon = weapon;
    }

    public void FirePrimaryWeapon(){
        if(primaryWeapon){
            primaryWeapon.Fire();
        }
    }

    public void FireSpecialWeapon(){
        if(specialWeapon){
            specialWeapon.Fire();
        }
    }
}
