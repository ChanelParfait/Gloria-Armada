using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    // Base Class for enemies 
    [SerializeField] GameObject projectile; 
    [SerializeField] float shootInterval = 5.0f;
    private float timer = 0; 
    // Start is called before the first frame update
    void Start()
    {
        shootInterval = 5;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        Debug.Log(shootInterval);
        if(timer >= shootInterval){
            Debug.Log("shoot");
            Shoot();
            timer = 0; 
        }
    }


    void Shoot(){
        // get the position 4 units in front of the enemy 
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.right * 4;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation);
        // provide a reference to self  
    }

}
