using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LiftingBody : MonoBehaviour
{
    public Vector3 relativePosition; // Position relative to the aircraft's center
    public Quaternion relativeRotation; // Rotation relative to the aircraft's center (by default flat level wing facing forwards)
    public float area; // Surface area
    public float liftCoefficient; // Lift coefficient (placeholder value for now)
    public string StrName;
    public float airDensity = 1.225f;   

    // Placeholder values for the lift and drag coefficients at zero angle of attack
    private const float Cl0 = 0.2f;
    private const float ClAlpha = 0.1f; // Slope of the Cl vs. AoA curve before stall
    private const float AlphaStall = 15.0f; // Angle of attack at stall in degrees
    private const float ClMax = 1.2f; // Maximum lift coefficient at stall
    private const float ClPostStallSlope = -0.08f; // Slope of the Cl after stall

    [SerializeField] private float angleOfAttack;
    

    [SerializeField] private Vector3 lift;
    [SerializeField] private Vector3 drag;
    [SerializeField] private Vector3 adjustedVelocity;

    // Start is called before the first frame update
    void Start()
    {
    }
    public static LiftingBody AddLiftingBody(GameObject parent, string name, Vector3 relativePosition, Vector3 relativeRotation, float area, float liftCoefficient)
    {
        LiftingBody lb = parent.AddComponent<LiftingBody>();
        lb.relativePosition = relativePosition;
        lb.relativeRotation = Quaternion.Euler(relativeRotation);
        lb.area = area;
        lb.liftCoefficient = liftCoefficient;
        lb.StrName = name;
        return lb;
    }
    public Vector3 CalculateForces(Vector3 localVelocity)
    {
        //Adjust the local velocity to firstly include the aircraft's rotational velocity for this relative position
        adjustedVelocity = Quaternion.Inverse(relativeRotation) * localVelocity;
        float direction = Mathf.Sign(adjustedVelocity.z);

        // Assuming a simple linear lift model with a sharp drop off past stall (not using actual airfoil data)
        angleOfAttack = Mathf.Rad2Deg * Mathf.Atan2(adjustedVelocity.y, Mathf.Max(adjustedVelocity.z, 0.01f));
        angleOfAttack *= -1;
        float liftCoefficient = Cl0;
        float AoA = Mathf.Abs(angleOfAttack);
        if (AoA < AlphaStall)
        {
            liftCoefficient += ClAlpha * Mathf.Abs(angleOfAttack);
        }
        else if (AoA > AlphaStall && AoA < 30.0f)
        {
            liftCoefficient = ClMax + ClPostStallSlope * (AoA - AlphaStall);
        }
/*        else if (AoA > 20.0f && AoA < 30.0f)
        {
            float Cl20 = ClMax + ClPostStallSlope * (20.0f - AlphaStall);
            liftCoefficient = Cl20 + 0.155f * (AoA);
        }*/
        else
        {
            float CL30 = ClMax + ClPostStallSlope * (30.0f - AlphaStall);
            //Asymptote from CL30 down to 0.0 at AoA = 180f
            liftCoefficient = CL30 - (CL30 * (AoA - 30.0f) / 150.0f);
        }

        liftCoefficient = Mathf.Max(liftCoefficient, 0.0f); // Prevent negative lift coefficients (shouldn't happen in practice)

        
        if (adjustedVelocity.z < 0)
        {
            liftCoefficient *= -1; // Reverse the lift direction
        }

        float liftForceMagnitude = liftCoefficient * 0.5f * airDensity * adjustedVelocity.magnitude * adjustedVelocity.magnitude * area;
        Vector3 liftDirection = transform.TransformDirection(Vector3.Cross(adjustedVelocity, transform.right).normalized);

        lift = liftForceMagnitude * liftDirection;

        float dragCoefficient = (liftCoefficient * liftCoefficient) / (Mathf.PI * 6.0f); // Simplified induced drag calculation with fixed aspect ratio

        // Adjust drag for reverse flow, assuming significantly increased drag
        if (adjustedVelocity.z < 0)
        {
            dragCoefficient *= 1; // Increase drag in reverse flow
        }

        // Calculate the drag force
        float dragForceMagnitude = dragCoefficient * 0.5f * airDensity * adjustedVelocity.magnitude * adjustedVelocity.magnitude * area;
        Vector3 dragDirection = transform.TransformDirection(-adjustedVelocity.normalized);

        drag = dragForceMagnitude * dragDirection;
        return lift + drag;

    }

    public void DrawDebugLines()
    {
        Debug.DrawLine( transform.TransformPoint(relativePosition), transform.TransformPoint(relativePosition) + (lift * 0.001f), Color.green);
        Debug.DrawLine( transform.TransformPoint(relativePosition),transform.TransformPoint(relativePosition) + (drag * 0.001f), Color.red);
        //Draw a line representing the chord line of the lifting body
        Debug.DrawLine(transform.TransformPoint(relativePosition), transform.TransformPoint(relativePosition) + transform.forward * 5f, Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
