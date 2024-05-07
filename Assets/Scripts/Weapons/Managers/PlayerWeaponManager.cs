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

    Powerups[] powerups;

    public bool isArmed = true;

    public Rigidbody playerRB;

    public GameObject test1;
    public GameObject test2;

    // Start is called before the first frame update
    void Start()
    {
        //Get RB of parent 
        playerRB = GetComponentInParent<Rigidbody>();

        if(!primaryWeapon && !specialWeapon){
            SetPrimaryWeapon(test1);
            SetSpecialWeapon(test2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // Fire Primary Weapon when pressing Space
        if(Input.GetKey(KeyCode.Space) && isArmed){
            //Debug.Log("Weapon: " + primaryWeapon + "Fired by: " + gameObject.name);
            FirePrimaryWeapon();
        }
        // Fire Special Weapon when pressing E
        if(Input.GetKeyDown(KeyCode.E) && isArmed){
            FireSpecialWeapon();
        }

    }

    // Set and Create Primary Weapon using a provided Game Object
    public void SetPrimaryWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the primary weapon
        primaryWeapon = Instantiate(weaponObj, transform.position, transform.rotation, transform).GetComponent<Weapon>();
        primaryWeapon.isPlayerWeapon = true;
        
    }

    // Set and Create Special Weapon using a provided Game Object
    public void SetSpecialWeapon(GameObject weaponObj){
        // create the provided weapon prefab as a child of the player
        // and store it as the special weapon
        specialWeapon = Instantiate(weaponObj, transform.position, transform.rotation, transform).GetComponent<Weapon>();
        specialWeapon.isPlayerWeapon = true;
    }

    
    public Weapon GetPrimaryWeapon(){
        return primaryWeapon;
    }

    public Weapon GetSpecialWeapon(){
        return specialWeapon;
    }

    

    public virtual void FirePrimaryWeapon(){
        if(primaryWeapon){
            primaryWeapon.Fire(playerRB.velocity);
        }
        
    }

    public virtual void FireSpecialWeapon(){
        if(specialWeapon){
            specialWeapon.Fire(playerRB.velocity);
        }
    }
}
