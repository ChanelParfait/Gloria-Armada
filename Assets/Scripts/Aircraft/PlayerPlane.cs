using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPlane : PlaneBase
{
    public AudioSource damageSound;
    public Animator damageCircle;
    public static event UnityAction<PlayerPlane> OnPlayerDamage;
    public static event UnityAction OnPlayerDeath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(int damage){
        damageCircle.SetTrigger("DamageTaken");
        damageSound.Play();
        OnPlayerDamage?.Invoke(this);
        base.TakeDamage(damage);
    }

    protected override void Die(){   
        OnPlayerDeath?.Invoke();
        base.Die();
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
            int damage = (int)Mathf.Lerp(1,maxHealth, dot);
            TakeDamage(damage);
        }
    }
    
}
