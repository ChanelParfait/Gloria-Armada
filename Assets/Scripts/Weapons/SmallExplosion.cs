using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallExplosion : MonoBehaviour
{
    private float timer;
    public float destroyTime = 1.0f;

    public float damage = 1.0f;

    public float impulse = 300.0f;

    [SerializeField] private bool dealDamageSeparately = false;  

    [SerializeField] float explosionRadius = 13.0f;

    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    Collider col;
    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioClip != null)
        {
            audioSource.clip = audioClip;

            audioSource.volume = Random.Range(0.8f, 1.0f);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            audioSource.Play();
        }




        // Only do the damage assessment if the effect does damage (optimize)
        if (damage <= 0){
            return;
        }
        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Iterate over each collider to apply damage
        foreach (Collider col in colliders)
        {
            float range = (col.gameObject.transform.position - transform.position).magnitude;
            float scaledDamage = damage * Mathf.Clamp01(1 - range / explosionRadius);

            if (col.CompareTag("Player") && !dealDamageSeparately)
            {
                col.GetComponent<PlayerPlane>().TakeDamage(scaledDamage);
                col.GetComponent<Rigidbody>().AddExplosionForce(impulse, transform.position, explosionRadius, 0, ForceMode.Impulse);
            }
            else if (col.CompareTag("Enemy"))
            {
                col.GetComponent<EnemyBase>()?.TakeDamage(scaledDamage * 2);
            }
            else if (col.CompareTag("EnemyWreckage")){
                col.GetComponent<Rigidbody>().AddExplosionForce(impulse/10, transform.position, explosionRadius, 0, ForceMode.Impulse);
            }
        }

        if (dealDamageSeparately){
            SeparatedDamageCollider();
        }
    }

    void SeparatedDamageCollider(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius/2);

        // Iterate over each collider to apply damage
        foreach (Collider col in colliders)
        {
            float range = (col.gameObject.transform.position - transform.position).magnitude;
            float scaledDamage = damage * Mathf.Clamp01(1 - range / explosionRadius);

            if (col.CompareTag("Player"))
            {
                col.GetComponent<PlayerPlane>().TakeDamage(Mathf.Min(scaledDamage, 1.0f));
                col.GetComponent<Rigidbody>().AddExplosionForce(impulse, transform.position, explosionRadius, 0, ForceMode.Impulse);
            }
        }
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
