using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : Actor
{
    public float shieldRefreshTime;
    [SerializeField] private GameObject[] shieldGameObjects;
    [SerializeField] private Collider coreCollider;


    [SerializeField] private Collider shieldCollider;

    // Start is called before the first frame update
     protected override void Start()
    {
        base.Start();
        shieldCollider = GetComponent<Collider>();
        coreCollider = GameObject.FindGameObjectWithTag("EnemyBoss").GetComponent<Collider>();
        coreCollider.enabled = false;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Die()
    {   
        Debug.Log("Shield Off");
        // when shield reaches zero health 
            // turn off shield collider temporarily 
            shieldCollider.enabled = false;
            coreCollider.enabled = true;

            // enable core collider 
            // switch to damaged model
            shieldGameObjects[0].SetActive(false);
            shieldGameObjects[1].SetActive(true);


            StartCoroutine(RefreshShield());
    }

    private IEnumerator RefreshShield(){
        // wait for set amount of time
        yield return new WaitForSeconds(shieldRefreshTime);
        Debug.Log("Shield Refreshed");

        // reset health to full and revert changes
        CurrentHealth = maxHealth;
        isAlive = true;
        Debug.Log("Shield Health: " + CurrentHealth);

        shieldCollider.enabled = true;
        coreCollider.enabled = false;

        shieldGameObjects[1].SetActive(false);
        shieldGameObjects[0].SetActive(true);
    }


}
