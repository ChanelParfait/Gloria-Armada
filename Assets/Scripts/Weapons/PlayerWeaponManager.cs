using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] private Weapon primaryWeapon;

    [SerializeField] private Weapon specialWeapon;
    public GameObject testWeapon1;
    public GameObject testWeapon2;
    // Start is called before the first frame update
    void Start()
    {
        SetPrimaryWeapon(testWeapon1);
        SetSpecialWeapon(testWeapon2);
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

    // Set Primary Weapon using a provided Weapon 
    /*public void SetPrimaryWeapon(Weapon weapon){
        // add a weapon component of this weapon type to the player
        Type weaponType = weapon.GetType();
        // store it as the primary weapon
        primaryWeapon = (Weapon)gameObject.AddComponent(weaponType); 
        primaryWeapon.weaponCategory = WeaponCategories.Primary;
        // use this line to setup the weapon component using a prefab object
        primaryWeapon.SetProjectile(weapon.GetProjectile());
        primaryWeapon.SetFireSound(weapon.GetFireSound());
    }*/

    // Set Primary Weapon using a provided Game Object
    public void SetPrimaryWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the primary weapon
        primaryWeapon = Instantiate(weaponObj, Vector3.zero, Quaternion.identity, transform).GetComponent<Weapon>();
        
    }

    // Set Special Weapon using a provided Weapon
    /*public void SetSpecialWeapon(Weapon weapon){
         // add a weapon component to this object
        Type weaponType = weapon.GetType();
        gameObject.AddComponent(weaponType); 
        // store it as the special weapon 
        specialWeapon = (Weapon)gameObject.GetComponent(weaponType);
        specialWeapon.weaponCategory = WeaponCategories.Special;
        // use this line to setup the weapon component using a prefab object
        specialWeapon.SetProjectile(weapon.GetProjectile());
        specialWeapon.SetFireSound(weapon.GetFireSound());
    }*/

    // Set Special Weapon using a provided Game Object
    public void SetSpecialWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the special weapon
        specialWeapon = Instantiate(weaponObj, Vector3.zero, Quaternion.identity, transform).GetComponent<Weapon>();
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
