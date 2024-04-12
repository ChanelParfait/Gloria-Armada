using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    public bool isEnemyWeapon = false; 
    public  GameObject projectile; 
    // Base Weapon Class 
    public WeaponCategories weaponCategory;  
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Fire(){
            Debug.Log("Fire Base Weapon");
            
    }
}
