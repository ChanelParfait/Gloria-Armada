using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerWeaponManager : MonoBehaviour
{
    // Player has 2 main weapons
    // Currently Serializable for testing so they can be viewed in the editor
    [SerializeField] private Weapon primaryWeapon;

    [SerializeField] private Weapon specialWeapon;

    // Test Variables
    public GameObject testWeapon1;
    public GameObject testWeapon2;


    // Start is called before the first frame update
    void Start()
    {
        // Used currently to test / show functionality
        // Will later be changed to Set Weapons through Loadout Menu
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

    // Set and Create Primary Weapon using a provided Game Object
    public void SetPrimaryWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the primary weapon
        primaryWeapon = Instantiate(weaponObj, Vector3.zero, Quaternion.identity, transform).GetComponent<Weapon>();
        primaryWeapon.isPlayerWeapon = true;
        
    }

    // Set and Create Special Weapon using a provided Game Object
    public void SetSpecialWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the special weapon
        specialWeapon = Instantiate(weaponObj, Vector3.zero, Quaternion.identity, transform).GetComponent<Weapon>();
        specialWeapon.isPlayerWeapon = true;
    }

    public virtual void FirePrimaryWeapon(){
        if(primaryWeapon){
            primaryWeapon.Fire();
        }
        
    }

    public virtual void FireSpecialWeapon(){
        if(specialWeapon){
            specialWeapon.Fire();
        }
    }
}
