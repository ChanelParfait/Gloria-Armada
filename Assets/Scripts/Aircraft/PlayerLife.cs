using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static event Action OnPlayerDamage;
    public static event Action OnPlayerHeal;

    [SerializeField] private GameObject gameOverScreen;

    public float life, maxLife;
    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
            Debug.Log("Damage Taken");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Heal(1);
            Debug.Log("Life Recovered");
        }*/

        if (life == 0)
        {
            if (gameOverScreen.activeSelf == false)
            {
                Time.timeScale = 0;
                gameOverScreen.SetActive(true);
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyProjectile")
        {
            // TakeDamage(other.GetComponent<Projectile>().projectileStats.damage);
            // //Debug.Log("Damage Taken");
            // Debug.Log("Damage: " + other.GetComponent<Projectile>().projectileStats.damage);
            // Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            //Get the normal of the collision
            Vector3 normal = col.contacts[0].normal;
            //Get dot product of the normal and the velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            float dot = Vector3.Dot(rb.velocity.normalized, normal);
            
            Debug.Log(dot);

            dot = Mathf.Clamp01(dot * 5);
            
            //Reduce health by a minimum of 1healh, max of MaxLife based on dot
            float damage = Mathf.Lerp(1, maxLife, dot);
            TakeDamage(damage);
        }
    }
}
