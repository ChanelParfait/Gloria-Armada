using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWreckage : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //Find a random rigidbody in the children
        rb = GetComponentInChildren<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < 0.1f){
            StartCoroutine(Despawn());
        }
        
    }

    IEnumerator Despawn(){
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
