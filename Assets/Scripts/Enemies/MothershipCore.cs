using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class MothershipCore : Actor
{
    [SerializeField] private GameObject[] hearts;
    private Rigidbody rb;
    private Plane playerPlane; 
    bool isMoving = false;
    public static UnityAction onFinalBossDefeated; 
    public static UnityAction onBossStageChange; 

    private int currentStage = 1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerPlane = GameObject.FindGameObjectWithTag("Player").GetComponent<Plane>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(hearts != null){
            if(CurrentHealth <= maxHealth / 3 * 1 && currentStage < 3){
                // set active heart to heart 3
                SetActiveHeart(3);
                // set current stage
                currentStage = 3;
                onBossStageChange?.Invoke();
            }
            else if(CurrentHealth <= maxHealth / 3 * 2 && currentStage < 2){
                // set active heart to heart 2
                SetActiveHeart(2);
                // set current stage
                currentStage = 2;
                onBossStageChange?.Invoke();

            }

            if(isMoving){
                rb.velocity = new Vector3(playerPlane.internalVelocity.x + playerPlane.localVelocity.x , 0, 0);
            }
            else{
                if(Vector3.Distance(gameObject.transform.position, playerPlane.gameObject.transform.position) < 60){
                    isMoving = true;
                }
            }
        }
        
    }


    private void SetActiveHeart(int index){
        if(hearts.Length > 0 ){
            foreach(GameObject heart in hearts){
                // disable all hearts
                heart.SetActive(false);
            }
            hearts[index - 1].SetActive(true);
        }
        

    }


    protected override void Die(){
        foreach(GameObject heart in hearts){
            Destroy(heart);
        }
        hearts = null;
        onFinalBossDefeated?.Invoke();
        //Debug.Log("Level Finished");

    }
}
