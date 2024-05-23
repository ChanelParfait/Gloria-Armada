using System.Collections;
using UnityEngine;

public class EnemyPlane_Physical : EnemyPlane
{
    private Autopilot ap;
    // Start is called before the first frame update
    protected override void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        ap = GetComponent<Autopilot>();
        currentPerspective = lm.currentPerspective;
        if (targetObj == null)
        {
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }

        //Find and set camera
        cam = Camera.main;
        camUtils = GameObject.FindObjectOfType<CameraUtils>();

        randomOffsetComponent = Random.Range(-0.4f, 0.4f);

        _ = StartCoroutine(Initialize());
    }

    protected override void UpdatePerspective(int _pers)
    {
        return;
    }

    override protected IEnumerator Initialize()
    {
        yield return StartCoroutine(base.Initialize());
        yield return new WaitForSeconds(2);
        ap.setAPState(Autopilot.AutopilotState.targetFormation);
        if (targetObj == null)
        {
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }
        ap.setTargetObject(targetObj);
        targetOffset = base.GetTargetOffset();
        ap.targetOffset = targetOffset;
    }

    protected override void FixedUpdate()
    {
        radarTimer += Time.deltaTime;
        if (radarTimer > 0.5f)
        {
            //AvoidGround();
            radarTimer = 0;
        }
    }



    // Update is called once per frame
    private void Update()
    {

    }
}
