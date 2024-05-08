using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Homing : MonoBehaviour
{
    public GameObject enemy;

    Autopilot ap;
    Rigidbody rb;

    [SerializeField]LayerMask collisionMask;

    FieldOfView fov;

    [SerializeField] float armingDelay = 0.50f;
    bool isArmed = false;

    Perspective pers = Perspective.Side_On;
  

    // Start is called before the first frame update
    void Start()
    {
        LevelManager lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        if (lm){
            pers = lm.currentPerspective;
        }
        ap = GetComponent<Autopilot>();
        fov = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody>();
        

        if (pers == Perspective.Side_On)
        {
            Vector3 pos = this.gameObject.transform.position;
            gameObject.transform.position.Set(pos.x, pos.y, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY  | RigidbodyConstraints.FreezePositionZ;
        }
        else if (pers == Perspective.Top_Down)
        {
            Vector3 pos = this.gameObject.transform.position;
            gameObject.transform.position.Set(pos.x, 0, pos.z);
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
        StartCoroutine(Arm());
    }

    IEnumerator Arm()
    {
        yield return new WaitForSeconds(armingDelay);
        isArmed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy == null && isArmed)
        {
            if (fov.visibleTargets.Count > 0)
            {
                // Check if the gameObject still exists
                if (fov.visibleTargets[0] != null)
                {
                    enemy = fov.visibleTargets[0].gameObject;
                    ap.setTargetObject(enemy);
                    ap.setAPState(Autopilot.AutopilotState.pointAt);
                }
            }
        }
    }
}
