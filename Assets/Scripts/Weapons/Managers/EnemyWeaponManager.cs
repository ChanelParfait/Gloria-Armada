using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponManager : MonoBehaviour
{
    // Enemy can have many weapons, stored in a list
    public Weapon[] weapons = {};
    // Active weapon represents the weapon that can be fired
    public Weapon ActiveWeapon { get; private set; }

    void Start()
    {
        // Set Default Active Weapon
        if(weapons.Length > 0){
            ActiveWeapon = weapons[0];
            Debug.Log("Active Weapon: " + ActiveWeapon);


        }
    }

    // Weapon Manager Class for Enemies
    public void SetActiveWeapon(int weaponNum){
        // Set Active Weapon given a number representing its place in the list
        // i.e give 1 to get the 1st weapon in the list
        if(weapons.Length <= weaponNum){
            if(weapons[weaponNum - 1]){
                ActiveWeapon = weapons[weaponNum - 1];
            }
        }
    }

    public void FireActiveWeapon(){
        Debug.Log("Active Weapon: " + ActiveWeapon);

        ActiveWeapon.EnemyFire();
    }


    // Stop Firing Function Currently only used for Laser Cannon
    public void StopFiring(float delayTime){
        StartCoroutine(Wait(delayTime));
    }

    private IEnumerator Wait(float waitTime){
        yield return new WaitForSeconds(waitTime);
        ActiveWeapon.StopFiring();
    }
}
