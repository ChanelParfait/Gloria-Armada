using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlane_Physical : EnemyPlane
{

    Autopilot ap;
    // Start is called before the first frame update
    protected override void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        currentPerspective = lm.currentPerspective;
        cam = Camera.main;
        camUtils = GameObject.FindObjectOfType<CameraUtils>();
        if(targetObj == null){
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }
        randomOffsetComponent = Random.Range(-0.4f, 0.4f);
        ap = GetComponent<Autopilot>();
        StartCoroutine(Initialize());
    }

    override protected IEnumerator Initialize(){
        yield return StartCoroutine(base.Initialize());
        yield return new WaitForSeconds(2);
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        if(targetObj == null){
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }
        ap.setTargetObject(targetObj);
        targetOffset = base.GetTargetOffset();
        ap.targetOffset = targetOffset;
    }

    protected override void FixedUpdate(){

        radarTimer += Time.deltaTime;
        if (radarTimer > 0.5f){
            //AvoidGround();
            radarTimer = 0;
        }
    }
    


    // Update is called once per frame
    void Update()
    {
        
    }
}
