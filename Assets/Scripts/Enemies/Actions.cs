using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Actions
{
    public static UnityAction<Enemy> OnEnemyDeath;

    public static UnityAction<float> OnAmmoChange;

}
