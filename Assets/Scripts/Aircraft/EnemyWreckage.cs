using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyWreckage : MonoBehaviour
{
    Collider self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponentInChildren<Collider>();
        StartCoroutine(timeOut());

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Despawner"))
        {
            StartCoroutine(Despawn());
        }
    }

    IEnumerator timeOut()
    {
        yield return new WaitForSeconds(20);
        Destroy(gameObject);
    }
    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        ParticleManager[] pms = GetComponentsInChildren<ParticleManager>();
        foreach (ParticleManager pm in pms)
        {
            pm.transform.SetParent(null);
            pm.Detach();
        }
        Destroy(gameObject);
    }
}
