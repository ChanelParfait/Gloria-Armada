using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] bool detatched = false;
    // Start is called before the first frame update
    void Start()
    {

        ps = GetComponent<ParticleSystem>();
        

        if (ps == null){
            Debug.Log("No Particle System Attached to Particle Manager");
            return;
        }
        //if the object has no parent, set as detatched from the getgo
        if (transform.parent == null){
            detatched = true;
            ps.transform.SetParent(null);
        }
        if (detatched) {
            ps.Play();
        }     
    }

    public void Detach(){
        detatched = true;

        if (ps == null){
            Debug.Log("Died: Particle manager has no particle system attached on death");
            TryGetComponent<ParticleSystem>(out ParticleSystem ps);
            Debug.Log("Tried to get component" + ps);
            if (ps == null){
                return;
            }
            
        }
        // on last particle death
        if (ps != null){
            ps.Stop();
            if (!ps.IsAlive()) {
                Destroy(gameObject);
            }
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
