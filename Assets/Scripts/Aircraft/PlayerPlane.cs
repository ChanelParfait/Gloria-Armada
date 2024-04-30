using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPlane : Actor
{
    public AudioSource audioSource;
    public static event UnityAction<PlayerPlane> OnPlayerDamage;
    public static event UnityAction OnPlayerDeath;

    [SerializeField] GameObject deathObj;
    // Start is called before the first frame update
    override protected void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }


    public override void TakeDamage(int damage){
        audioSource.Play();
        base.TakeDamage(damage);
        //Debug.Log("Current Health: " + currentHealth);
        OnPlayerDamage?.Invoke(this);
    }

    protected override void Die(){   
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
        base.Die();
        //TODO: This goes before the base.Die() call
        OnPlayerDeath?.Invoke();
        
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
            
            //Reduce health by a minimum of 1health, max of MaxLife based on dot
            int damage = (int)Mathf.Lerp(1,maxHealth, dot);

            TakeDamage(damage);
        }
    }
    
}
