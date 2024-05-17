using System.Collections;
using UnityEngine;

public abstract class FiringPattern_SO : ScriptableObject
{
    public abstract IEnumerator Fire(GameObject projectile, Transform spawnTransform, WeaponStats weaponStats, MonoBehaviour shooter);
}