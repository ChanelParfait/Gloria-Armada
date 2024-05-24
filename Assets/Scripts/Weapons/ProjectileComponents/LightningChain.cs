using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChain : MonoBehaviour
{
    [SerializeField] private float chainRange = 15f;
    [SerializeField] private float chainDamage = 0.5f;

    // Arc damage is applied on collision with an enemy
    [SerializeField] private float arcDamage = 1f;
    [SerializeField] private float chainTickRate = 0.5f;

    [SerializeField] private float impactFlashRadius = 45f; // 3x the normal radius
    [SerializeField] private float impactFlashDamage = 2f;
    [SerializeField] private GameObject chainEffectPrefab;
    private Transform currentTarget;

    void Awake()
    {
        if (chainEffectPrefab == null)
        {
            chainEffectPrefab = Resources.Load<GameObject>("ElectricityArc");
        }
    }

    void Start()
    {
        if (chainEffectPrefab == null)
        {
            Debug.LogError("No chain effect in Resources 'ElectricityArc'");
            return;
        }

        // Create a chain effect instance
        GameObject chainEffectInstance = Instantiate(chainEffectPrefab, transform.position, Quaternion.identity, transform);
        chainEffectInstance.GetComponent<ElectricityArc>().SetShooter(transform);
        StartCoroutine(FindSparkTarget(chainEffectInstance));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(chainDamage);
                ApplyImpactFlashDamage(other.transform.position);

                // Destroy the projectile
                Destroy(gameObject);
            }
        }
    }

    private void ApplyImpactFlashDamage(Vector3 impactPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(impactPosition, impactFlashRadius);
        List<EnemyBase> enemiesInRange = new List<EnemyBase>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        if (enemiesInRange.Count > 0)
        {
            // Randomly select one enemy from the list (or apply your own logic to select the target)
            EnemyBase targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];
            targetEnemy.TakeDamage(impactFlashDamage);

            // Create an arc effect for the selected enemy
            CreateArcEffect(impactPosition, targetEnemy.transform);
        }
    }

    private void CreateArcEffect(Vector3 startPoint, Transform target)
    {
        GameObject arcEffect = Instantiate(chainEffectPrefab, startPoint, Quaternion.identity);
        arcEffect.GetComponent<ElectricityArc>().SetShooter(transform);
        arcEffect.GetComponent<ElectricityArc>().SetTarget(target);
        arcEffect.transform.SetParent(null); // Detach from any parent to manage independently
        Destroy(arcEffect, 1.0f); // Destroy after some time
    }

    private IEnumerator FindSparkTarget(GameObject chainEffectInstance)
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, chainRange);
            bool foundTarget = false;

            foreach (Collider collider in colliders)
            {
                if (collider == null) continue;

                if (collider.gameObject.CompareTag("Enemy"))
                {
                    currentTarget = collider.transform;
                    chainEffectInstance.GetComponent<ElectricityArc>().SetTarget(currentTarget);
                    ApplyArcDamage(collider.GetComponent<EnemyBase>());
                    foundTarget = true;
                    break;
                }
            }

            if (!foundTarget)
            {
                chainEffectInstance.GetComponent<ElectricityArc>().SetTarget(null);
                currentTarget = null;
            }

            yield return new WaitForSeconds(chainTickRate);
        }
    }

    public void ApplyArcDamage(EnemyBase enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(arcDamage);
        }
    }

    void Update()
    {
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > chainRange)
            {
                chainEffectPrefab.GetComponent<ElectricityArc>().SetTarget(null);
                currentTarget = null;
            }
        }
    }
}
