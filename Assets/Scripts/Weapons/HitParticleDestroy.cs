using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticleDestroy : MonoBehaviour
{
    private float timer;
    public float destroyTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
