using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static event Action OnPlayerDamage;
    public static event Action OnPlayerHeal;

    public float life, maxLife;
    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
            Debug.Log("Damage Taken");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Heal(1);
            Debug.Log("Life Recovered");
        }
    }

    // Update is called once per frame
    public void TakeDamage(float amount)
    {
        if (life > 0)
        {
            life -= amount;
            OnPlayerDamage?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (life < maxLife)
        {
            life += amount;
            OnPlayerHeal?.Invoke();
        }
    }
}
