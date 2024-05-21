using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlane", menuName = "Plane/PlaneStats", order = 1)]
public class PlaneStats : ScriptableObject
{
    public Surfaces surfaces;

    [Space (20)]
    public float weight = 200.0f;
    public float thrust = 1800f;
    public bool thrustVectoring = false;
    public float scaleVelocity = 1.0f;
    public float cd = 0.2f;
    public float liftPower = 1.0f;
    public float health = 6.0f;
}
