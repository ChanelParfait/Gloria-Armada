using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    GameObject enemy;
    Autopilot ap;
    Plane planeSelf;
    Rigidbody rb;

    float burnTime = 1.0f;

    Perspective pers = Perspective.Side_On;
    // Start is called before the first frame update
    void Start()
    {
        planeSelf = GetComponent<Plane>();
        ap = GetComponent<Autopilot>();
        rb = GetComponent<Rigidbody>();

        if (pers == Perspective.Side_On)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY  | RigidbodyConstraints.FreezePositionZ;
        }

        rb.angularDrag = 0.01f;
        //Find the player by tag
        enemy = GameObject.FindGameObjectWithTag("Player");
        ap.onAxes = true;
        // Wait for a short time before homing in on player
        StartCoroutine(Wait());
        //When the coroutine is finished, set the missile to home in on the player
        
    }
    
    // Coroutine for missile to wait before homing in on player
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f);
        ap.setTargetObject(enemy);
        planeSelf.SetThrottle(1.0f);
        StartCoroutine(BurnOut());
    }

    // Coroutine for setting thrust to 0 after a certain amount of time
    IEnumerator BurnOut()
    {
        yield return new WaitForSeconds(burnTime);
        planeSelf.SetThrottle(0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
