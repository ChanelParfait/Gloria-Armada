using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    }

    // Update is called once per frame
    void Update()
    { 
        gameObject.transform.position += gameObject.transform.forward * Time.deltaTime * projectileStats.speed; 
    }

    public void SetStats(ProjectileStats stats){
        projectileStats = stats;
    }

}
