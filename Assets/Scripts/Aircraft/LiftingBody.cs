using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LiftingBody : MonoBehaviour
{
    public Vector3 relativePosition; // Position relative to the aircraft's center
    public float area; // Surface area
    public float liftCoefficient; // Lift coefficient (placeholder value for now)

    float airDensity = 1.225f; // Standard air density at sea level
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + relativePosition, new Vector3(1, 1, 1));
    }

    public Vector3 CalculateLift(Vector3 localVelocity, float airDensity)
    {
        // Lift Equation: Lift = Cl * 0.5 * rho * V^2 * S
        float liftForceMagnitude = liftCoefficient * 0.5f * airDensity * localVelocity.magnitude * localVelocity.magnitude * area;
        Vector3 liftDirection = Vector3.Cross(localVelocity.normalized, Vector3.right).normalized; // Assuming right vector is perpendicular to airflow
        return liftForceMagnitude * liftDirection;
    }

    public Vector3 CalculateDrag(Vector3 localVelocity, float airDensity)
    {
        // Drag Equation: Drag = Cd * 0.5 * rho * V^2 * S
        float dragCoefficient = 0.02f; // Placeholder value for drag coefficient
        float dragForceMagnitude = dragCoefficient * 0.5f * airDensity * localVelocity.magnitude * localVelocity.magnitude * area;
        Vector3 dragDirection = -localVelocity.normalized; // Opposite direction of velocity
        return dragForceMagnitude * dragDirection;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
