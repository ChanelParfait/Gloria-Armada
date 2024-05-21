using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    ParticleSystem ps;
    [SerializeField] bool detatched = false;
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

    public void Detach(){
        ps.Stop();
        // on last particle death
        detatched = true;
        if (!ps.IsAlive()) {
            Destroy(gameObject);
        }
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
