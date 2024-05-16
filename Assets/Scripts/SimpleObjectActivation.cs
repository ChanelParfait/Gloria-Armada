using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectActivation : MonoBehaviour
{
    public GameObject target;
    private float timer;
    public float activationTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= activationTime)
        {
            target.SetActive(true);
        }
    }
}
