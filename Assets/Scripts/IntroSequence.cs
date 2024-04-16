using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    public bool burst = false;
    public float time;
    public GameObject thruster;
    public float power = 0.0f;
    public float powerMulti = 0.0f;
    public float burstScale = 1.0f;

    public AudioSource engineSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thruster.transform.localScale = new Vector3(power, power, power);
        power = power * burstScale;

        if (time <= 7.0f)
        {
            time += Time.deltaTime;
        }
        else if (power > -5.0f)
        {
            StartEngine();
        }
        else 
        {
            gameObject.GetComponentInParent<Animator>().SetBool("Launch", true);
            if (burst == false)
            {
                power = -50.0f;
                engineSound.pitch = 3f;
                burst = true;
            }

            if (power < -10.0f)
            {
                power += Time.deltaTime * 75.0f;
            }
            else
            {
                power = -10.0f;
            }

            if (engineSound.volume >= 0f)
            {
                engineSound.volume -= Time.deltaTime * 0.15f;
            }
        }
    }
    void StartEngine()
    {
        if (Input.GetKey(KeyCode.W))
        {
            power -= Time.deltaTime * 0.1f;
            engineSound.pitch += Time.deltaTime * 0.05f + powerMulti;
            if (power < -0.5f)
            {
                powerMulti += Time.deltaTime * 0.001f;
                power = power - powerMulti;
            }
        }
    }
}

