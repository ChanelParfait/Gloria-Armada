using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Team {Player, Enemy};
public class ProximityFuse : MonoBehaviour
{
    //NOTE: This script should be implemented via the prefab as it relies on a sphere collider

    [SerializeField] Transform target;
    float rangeToTarget = 0.0f;
    float lastRangeToTarget = 1000.0f;
    
    [SerializeField] GameObject detonationEffect;

    [SerializeField] Team team;

    [SerializeField] float radius = 13.0f;

    [SerializeField] float armingDelay = 1.0f;
    public bool isArmed = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Arm());
    }

    IEnumerator Arm(){
        yield return new WaitForSeconds(armingDelay);
        isArmed = true;
    }

    void OnTriggerEnter(Collider col){

    }

    // Update is called once per frame
    void Update()
    {
        //SphereCollisionCheck for target
        if (!target){
            //Get all colliders within proxy sensor radius
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider col in colliders){
                if (team == Team.Player && col.CompareTag("Enemy")){
                   target = col.transform;
                }
                else if (team == Team.Enemy && col.CompareTag("Player")){
                    target = col.transform;
                }
            }
        }
        //If target is found, look for increasing range (we have passed target)
        else{
            //Debug.Log(name + " is tracking " + target.name + " at " + rangeToTarget + " last: " + lastRangeToTarget);
            rangeToTarget = (target.position - transform.position).magnitude;
            if (rangeToTarget > lastRangeToTarget && isArmed){
                Detonate();
            }
            lastRangeToTarget = rangeToTarget; 
        }
        
    }

    void Detonate(){
        //StopAllCoroutines();
        if(detonationEffect)
        {
            Instantiate(detonationEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
