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
    public Rigidbody projectileRB;

    // Start is called before the first frame update
    void Start()
    {
        // Set Default Stats
        //projectileStats.speed = 8;
        //projectileStats.damage = 2;
    }

    public void Launch(Rigidbody planeRB, ProjectileStats stats) {
        
        projectileRB = GetComponent<Rigidbody>();


        projectileRB.AddRelativeForce(new Vector3(0, 0, stats.speed), ForceMode.VelocityChange);
        projectileRB.AddForce(planeRB.velocity, ForceMode.VelocityChange); 
    }

    // Update is called once per frame
    void Update()
    { 
        //gameObject.transform.position += gameObject.transform.forward * Time.deltaTime * projectileStats.speed; 


    }

    public void SetStats(ProjectileStats stats){
        //Debug.Log(stats);
        projectileStats = stats;
    }

}
