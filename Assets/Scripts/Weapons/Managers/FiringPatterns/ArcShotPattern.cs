using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FiringPatterns/ArcShotPattern")]
public class ArcShotPattern : FiringPattern_SO
{
    public int numberOfShots = 5;
    public float arc = 90f;
    public float delayBetweenShots = 0.2f;

    public bool descending = false;

    public override IEnumerator Fire(GameObject projectile, Transform spawnTransform, WeaponStats weaponStats, MonoBehaviour shooter)
    {
        if (projectile == null || spawnTransform == null)
        {
            Debug.LogError("DescendingShotPattern: Missing reference(s)");
            yield break;
        }

        float direction = descending ? 1 : -1;

        float angleStep = arc / (numberOfShots - 1) * direction;
        float startAngle = -arc / 2 * direction;

        for (int i = 0; i < numberOfShots; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 parentRotation = shooter.transform.parent.transform.rotation.eulerAngles;
            Quaternion rotation = spawnTransform.rotation * Quaternion.Euler(angle, 0, 0);
            GameObject clone = GameObject.Instantiate(projectile, spawnTransform.position, rotation);
            clone.GetComponent<Rigidbody>().MoveRotation(rotation);
            clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats, shooter.transform.parent.GetComponent<Rigidbody>().velocity);
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }
}

