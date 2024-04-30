using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour
{
    public GameObject enemy;

    Autopilot ap;

    [SerializeField]LayerMask collisionMask;

    FieldOfView fov;

    [SerializeField] float armingDelay = 0.50f;
    bool isArmed = false;

    // Start is called before the first frame update
    void Start()
    {
        ap = GetComponent<Autopilot>();
        fov = GetComponent<FieldOfView>();
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
