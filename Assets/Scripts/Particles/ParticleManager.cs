using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    ParticleSystem ps;
    bool detatched = false;
    // Start is called before the first frame update
    void Start()
    {
        //if the object has no parent, set as detatched from the getgo
        if (transform.parent == null){
            detatched = true;
        }
        ps = GetComponent<ParticleSystem>();
        if (ps != null && detatched) {
            ps.Play();
        }     
    }

    public void Detatch(){
        var em = ps.emission;
        em.enabled = false;
        // on last particle death
    }

    // Update is called once per frame
    void Update()
    {
        if (detatched){
            if (ps != null && !ps.IsAlive()) {
            Destroy(gameObject);
            }     
        }
 
    }
}
