using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Perspective {Null = 0, Side_On = 1, Top_Down = 2};

public class LevelManager : MonoBehaviour
{
    // Keep track of level perspective and update all other objects 
    [SerializeField] Perspective initPerspective; 
    static Perspective currentPerspective; 
    [SerializeField] private Animator anim; 

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        currentPerspective = initPerspective; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            currentPerspective = col.GetComponent<TransitionPoint>().GetPerspective(); 
            anim.SetInteger("Perspective", (int)currentPerspective);
        }
    }

}
