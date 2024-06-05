using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : Actor
{
    public float shieldRefreshTime;
    [SerializeField] private GameObject[] shieldGameObjects;
    [SerializeField] private Collider coreCollider;


    [SerializeField] private Collider shieldCollider;
    private int currentStage = 1;


    private void OnEnable(){
        MothershipCore.onFinalBossDefeated += DestroySelf;
    }

    private void OnDisable(){
        MothershipCore.onFinalBossDefeated -= DestroySelf;
    }

    // Start is called before the first frame update
     protected override void Start()
    {
        base.Start();
        shieldCollider = GetComponent<Collider>();
        coreCollider = GameObject.FindGameObjectWithTag("EnemyBoss").GetComponent<Collider>();
        coreCollider.enabled = false;
        SetActiveShield(1);


    }

    // Update is called once per frame
    void Update()
    {
        if(shieldGameObjects != null){
            if(CurrentHealth <= maxHealth / 3 * 1 && currentStage < 3){
                // set active shield to most damaged model
                SetActiveShield(3);
                // set current stage
                currentStage = 3;
                
            }
            else if(CurrentHealth <= maxHealth / 3 * 2 && currentStage < 2){
                // set active shield to slightly damaged model
                SetActiveShield(2);
                // set current stage
                currentStage = 2;

            }
        }
    }

    protected override void Die()
    {   
        Debug.Log("Shield Off");
        // when shield reaches zero health 
        DisableShield();
        StartCoroutine(RefreshShield());
    }

    private IEnumerator RefreshShield(){
        // wait for set amount of time
        yield return new WaitForSeconds(shieldRefreshTime);
        Debug.Log("Shield Refreshed");

        // reset health to full and revert changes
        CurrentHealth = maxHealth;
        currentStage = 1;
        isAlive = true;

        // reenable shield
        SetActiveShield(1);
    }

    private void DisableShield(){
        // disable all models
         if(shieldGameObjects.Length > 0 ){
            foreach(GameObject shield in shieldGameObjects){
                shield.SetActive(false);
            }
        }
        // turn off shield collider
        shieldCollider.enabled = false;
        // enable core collider 
        coreCollider.enabled = true;
    }

    private void SetActiveShield(int index){
        DisableShield();
        if(index <= shieldGameObjects.Length){
            shieldGameObjects[index - 1].SetActive(true);
            shieldCollider.enabled = true;
            coreCollider.enabled = false;
        }
    }

    private void DestroySelf(){
         Destroy(gameObject);
    }



}
