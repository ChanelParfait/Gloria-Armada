using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "FiringPatterns/SingleShotPattern")]
public class SingleShotPattern : FiringPattern_SO
{
    public override IEnumerator Fire(GameObject projectile, Transform spawnTransform, WeaponStats weaponStats, MonoBehaviour shooter, Perspective pers)
    {
       if (projectile == null || spawnTransform == null)
        {   
            yield break;
        }

        GameObject clone = GameObject.Instantiate(projectile, spawnTransform.position, spawnTransform.rotation);
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats);
        yield return null;
    }
}