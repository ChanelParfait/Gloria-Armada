using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallExplosion : MonoBehaviour
{
    private float timer;
    public float destroyTime = 1.0f;

    public float damage = 1.0f;

    public float impulse = 300.0f;

    [SerializeField] float explosionRadius = 13.0f;

    Collider col;
    // Start is called before the first frame update
    private void Start()
    {
        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Iterate over each collider to apply damage
        foreach (Collider col in colliders)
        {
            float range = (col.gameObject.transform.position - transform.position).magnitude;
            float scaledDamage = damage * Mathf.Clamp01(1 - range / explosionRadius);

            if (col.CompareTag("Player"))
            {
                col.GetComponent<PlayerPlane>()?.TakeDamage(scaledDamage);
                col.GetComponent<Rigidbody>().AddExplosionForce(impulse, transform.position, explosionRadius, 0, ForceMode.Impulse);
            }
            else if (col.CompareTag("Enemy"))
            {
                col.GetComponent<EnemyBase>()?.TakeDamage((int)scaledDamage);
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
