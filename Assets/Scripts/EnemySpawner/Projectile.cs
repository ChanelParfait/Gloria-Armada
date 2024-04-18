using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileStats{
    public float speed; 
    public int damage;
}
public class Projectile : MonoBehaviour
{
    // Base Class for All Projectile Objects
    public ProjectileStats projectileStats; 

    // Start is called before the first frame update
    void Start()
    {
        // /projectileStats.speed = 5;
        //projectileStats.damage = 1;
    }

    // Update is called once per frame
    void Update()
    { 
        gameObject.transform.position += gameObject.transform.forward * Time.deltaTime * projectileStats.speed; 
    }

    public void SetStats(ProjectileStats stats){
        //Debug.Log(stats);
        projectileStats = stats;
    }
}
