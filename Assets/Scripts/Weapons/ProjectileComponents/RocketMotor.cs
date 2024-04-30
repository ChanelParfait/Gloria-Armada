using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMotor : MonoBehaviour
{
    [SerializeField] float waitTime = 0.2f;
    [SerializeField] float burnTime = 1.0f;

    Plane planeSelf;

    // Start is called before the first frame update
    void Start()
    {
        planeSelf = GetComponent<Plane>();
        if (planeSelf){
            StartCoroutine(Wait());
        }
        else {
            Debug.LogError(name + ": RocketMotor - No Plane component found on object");
        }    
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        planeSelf.SetThrottle(1.0f);
        StartCoroutine(BurnOut());
    }

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
