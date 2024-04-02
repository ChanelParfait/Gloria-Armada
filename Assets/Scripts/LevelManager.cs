using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Perspective {Null = 0, Side_On = 1, Top_Down = 2};

public class LevelManager : MonoBehaviour
{
    // Keep track of level perspective and update all other objects 
    [SerializeField] Perspective initPerspective; 
    private Perspective currentPerspective; 
    [SerializeField] private Animator anim; 

    [SerializeField] private Enemy_Spawner enemySpawner;
    [SerializeField] private JetControl jetControl;

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        currentPerspective = initPerspective; 
        enemySpawner.UpdatePerspective(currentPerspective);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            UpdatePerspective(col.GetComponent<TransitionPoint>().GetPerspective());
        }
    }

    private void UpdatePerspective(Perspective pers){
        currentPerspective = pers; 
        anim.SetInteger("Perspective", (int)currentPerspective);

        enemySpawner.UpdatePerspective(currentPerspective); 
        jetControl.ResetPosition(5f);
    }

}
