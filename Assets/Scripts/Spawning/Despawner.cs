using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider col){
        if(col.tag == "Enemy" || col.tag == "PlayerProjectile" || col.tag == "EnemyProjectile"){
            Debug.Log("Despawn: " + col.name);
            Destroy(col.gameObject);
        }
        
    }
    
}
