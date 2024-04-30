using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDetonation : MonoBehaviour
{
    [SerializeField] float selfDetTime = 3.0f;

    [SerializeField] GameObject detonationEffect;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDetTime);
        Detonate();
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
