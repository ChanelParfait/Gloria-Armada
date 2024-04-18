using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponManager : MonoBehaviour
{
    public Weapon[] weapons = {};
    private Weapon ActiveWeapon;

    void Start()
    {
        if(weapons.Length > 0){
            ActiveWeapon = weapons[0];
        }
    }

    // Weapon Manager Class for Enemies
    public void SetActiveWeapon(int weaponNum){
        ActiveWeapon = weapons[weaponNum - 1];
    }

    public void FireActiveWeapon(){
        ActiveWeapon.Fire();
    }
}
