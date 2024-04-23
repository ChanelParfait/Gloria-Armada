using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private Perspective perspective; 

    public Perspective GetPerspective(){
        return perspective;
    }

}
