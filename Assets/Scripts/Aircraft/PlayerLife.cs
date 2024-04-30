using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static event Action OnPlayerDamage;
    public static event Action OnPlayerHeal;

    public AudioSource damageSound;
    public Animator damageCircle;
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] GameObject deathObj;

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

        if (life <= 0)
        {
            OnDeath();
            if (gameOverScreen.activeSelf == false)
            {
                Time.timeScale = 0;
                gameOverScreen.SetActive(true);
            }
        }
    }

    void OnDeath(){
        if (deathObj == null)
        {
            Debug.LogError("Death Object not set in PlayerLife script");
            return;
        }
        GameObject deadObj = Instantiate(deathObj, transform.position, transform.rotation);
        foreach (Rigidbody rb in deadObj.GetComponentsInChildren<Rigidbody>())
        {
            //Add force to the rigid body
            rb.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
            // Using the offset of the child from the parent, apply the appropriate velocity from the angular velocity
            rb.AddTorque(GetComponent<Rigidbody>().angularVelocity, ForceMode.VelocityChange);
        }
        //Destroy the player
        Destroy(gameObject);
    }

    // Update is called once per frame
    public void TakeDamage(float amount)
    {
        damageCircle.SetTrigger("DamageTaken");
        damageSound.Play();
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
