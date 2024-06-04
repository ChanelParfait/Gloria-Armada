using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public float slowAmount = 0.2f; // 20% slow
    public float duration = 5f; // Freeze duration

    public GameObject projectileObj;

    void Start()
    {
        projectileObj = gameObject;
    }

    public void ApplyFreeze(EnemyBase enemy)
    {
        if (enemy != null)
        {
            enemy.SetFrozen(freezeDuration: duration, slowAmount: slowAmount);
        }
    }
}