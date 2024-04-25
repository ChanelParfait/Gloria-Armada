using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
    
    var ps = GetComponentInChildren<ParticleSystem>();
    if (ps != null) {
        ps.Play();
    }
        
    }

    // Update is called once per frame
    void Update()
    {
    if (ps != null && !ps.IsAlive()) {
        Destroy(gameObject);
    }      
    }
}
