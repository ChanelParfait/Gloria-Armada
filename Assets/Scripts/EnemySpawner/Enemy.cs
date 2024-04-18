using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct EnemyStats{
    public int MaxHealth; 
}
public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    EnemyWeaponManager weaponManager; 
    [SerializeField] private float shootInterval;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {   
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
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
        weaponManager.FireActiveWeapon();
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
