using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlane", menuName = "Plane/PlaneStats", order = 1)]
public class PlaneStats : ScriptableObject
{
    public Surfaces surfaces;
    
    public float weight;
    public float thrust;
    public float thrustVectoring;
    public float scaleVelocity;
    public float cd = 0.2f;
}
