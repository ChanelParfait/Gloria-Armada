using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretMount : MonoBehaviour
{
    public GameObject fire;
    public GameObject parentPlane;

    public bool OnFire = false;

    [SerializeField] float timer;
    [SerializeField] float fireInterval = 1;
     // Start is called before the first frame update
    void Start()
    {
        parentPlane = transform.parent.gameObject;
    }

    public void TurretDied(){
        // If the turret dies, detach the fire particle system
        Instantiate(fire, transform.position, transform.rotation, this.transform.parent);
        fire.GetComponent<ParticleSystem>().Play();
        OnFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
