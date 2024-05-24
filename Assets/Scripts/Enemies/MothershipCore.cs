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
        if(currentHealth <= (maxHealth / 3) * 1){
            // set active heart to heart 3
            SetActiveHeart(3);
        }
        else if(currentHealth <= (maxHealth / 3) * 2){
            // set active heart to heart 2
            SetActiveHeart(2);
        }

        if(isMoving){
            rb.velocity = new Vector3(playerPlane.internalVelocity.x + playerPlane.localVelocity.x, 0, 0);
        }
        else{
            if(Vector3.Distance(gameObject.transform.position, playerPlane.gameObject.transform.position) < 100){
                isMoving = true;
            }
        }
    }


    private void SetActiveHeart(int index){
        foreach(GameObject heart in hearts){
            // disable all hearts
            heart.SetActive(false);
        }
        hearts[index - 1].SetActive(true);

    }


    protected override void Die(){
        foreach(GameObject heart in hearts){
            Destroy(heart);
        }
        hearts = null;
        onFinalBossDefeated?.Invoke();

    }
}
