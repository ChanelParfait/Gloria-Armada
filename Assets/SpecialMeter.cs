using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialMeter : MonoBehaviour
{
    public Image meter;
    public float meterLevel;
    private float meterCooldown = 3;

    public Color uncharged, charged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        meterCooldown += Time.deltaTime;
        if (meterLevel < 0)
        {
            meterLevel = 0;
        }

        if (meterLevel < 1)
        {
            if (meterCooldown >= 3)
            {
                meterLevel += 0.1f * Time.deltaTime;
            }
        }
        else
        {
            meterLevel = 1;
        }

        if (meterLevel < 0.25f)
        {
            meter.color = uncharged;
        }
        else
        {
            meter.color = charged;
        }

        meter.fillAmount = meterLevel;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(meterLevel >= 0.25f)
            {
                meterLevel -= 0.25f;
                meterCooldown = 0;
            }
        }
    }
}
