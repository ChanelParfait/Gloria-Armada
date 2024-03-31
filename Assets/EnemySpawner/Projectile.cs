using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 10; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 nextPos = gameObject.transform.position + ; 
        gameObject.transform.position += gameObject.transform.right * Time.deltaTime * speed; 
    }
}
