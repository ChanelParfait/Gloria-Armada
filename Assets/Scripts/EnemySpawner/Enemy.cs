using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    //[SerializeField] GameObject projectile; 
    WeaponManager weaponManager; 
    [SerializeField] float shootInterval = 5.0f;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {   
        weaponManager = gameObject.GetComponent<WeaponManager>();
        shootInterval = 3;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= shootInterval){
            timer = 0; 
            Fire();
        }
    }


    public void Fire(){
        weaponManager.FirePrimaryWeapon();
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage? / Die
            //Debug.Log("Die");
            Destroy(gameObject);
            // increase player score 
        }
    }

}
