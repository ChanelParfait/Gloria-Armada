using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
public class BossEnemy : MonoBehaviour
{
    public static event UnityAction OnBossDeath;
    private void OnEnable(){
 
    }
 
    private void OnDisable(){
        // invoke game over event when enemy is killed or despawned
        OnBossDeath?.Invoke();
    }
}