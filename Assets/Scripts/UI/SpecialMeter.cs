using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpecialMeter : MonoBehaviour
{
    public Image meter, meterOuter;
    public float meterLevel;
    //private float meterCooldown = 3;

    public Color uncharged, charged;

    private float maxAmmo;


    // Start is called before the first frame update
    void Start()
    {
        //Find the meter from GameCanvas/PlayerHUD/Special/SpecialInner
        meter = GameObject.Find("GameCanvas/PlayerHUD/Special/Special_Inner").GetComponent<Image>();
        meterOuter = GameObject.Find("GameCanvas/PlayerHUD/Special").GetComponent<Image>();
        meterLevel = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        if (meter){
            meter.fillAmount = meterLevel;
            if(meterLevel < 1)
                {
                    meter.color = new Color(1, 1, 1, 1);
                    meterOuter.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    meter.color -= new Color(0, 0, 0, Time.deltaTime);
                    meterOuter.color -= new Color(0, 0, 0, Time.deltaTime);
                }
        }
    
    }

    private void OnEnable(){
        // Update Score on enemy death 
        Weapon.OnAmmoChange += UpdateMeter;
        
    }

    private void OnDisable(){
        // if gameobject is disabled remove all listeners
        Weapon.OnAmmoChange -= UpdateMeter;

    }


    private void UpdateMeter(float ammo){
        // get max ammo from first ammo update
        if(maxAmmo == 0){
            maxAmmo = ammo;
        }
        //Debug.Log("Level: " + ammo);
        //Debug.Log("Meter Level: " + meterLevel);
        meterLevel = ammo * (1 / maxAmmo);
        

    }
}
