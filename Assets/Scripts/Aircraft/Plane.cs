using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    //KEYS
    public KeyCode Up = KeyCode.W;
    public KeyCode Down = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;

    public KeyCode throttleUp = KeyCode.LeftShift;

    

    // Aircraft parameters
    [SerializeField] float wingArea = 30.04f;  // Area of the wing (m^2)
    [SerializeField] float weight = 100;    // Weight of the aircraft (kg)
    [SerializeField] float thrust = 1800;   // Maximum thrust (N)
    // [SerializeField] float wingSpan = 22;      // Wing span (meters)
    // [SerializeField] float aspectRatio = 7.5f; // Aspect ratio

    Vector2 controlInputs;

    // Aircraft state
    public float throttle = 0.7f;

    public float AoA;
    public Vector3 internalVelocity;   // Velocity of the aircraft (not passed to RB) (m/s)
    public Vector3 localVelocity;      // Velocity of the aircraft from local (m/s)
    public Vector3 acceleration;  // Acceleration of the aircraft (m/s^2)


    // Unity
    [SerializeField] Rigidbody rb;

    private void Awake()
    {
        rb.drag = float.Epsilon;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Set the center of mass
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;
        rb.velocity = transform.forward * 20;
        
    }
    
    void FixedUpdate(){
        // Calculate the local velocity
        internalVelocity = Mathf.Min(rb.velocity.magnitude * 10, 250) * rb.velocity.normalized;
        localVelocity = transform.InverseTransformDirection(internalVelocity);

  
        Vector3 dragForce = Vector3.zero;

        // Calculate the thrust force
        Vector3 thrustForce = throttle * thrust * transform.forward;
        if (localVelocity.z >= 0)
        {
            AoA = Mathf.Rad2Deg * Mathf.Atan2(localVelocity.y, Mathf.Max(localVelocity.z, float.Epsilon));
        }
        else
        {
            AoA = 180 + Mathf.Rad2Deg * Mathf.Atan2(-localVelocity.y, Mathf.Max(-localVelocity.z, float.Epsilon));
        }
        //AoA = Mathf.Rad2Deg * Mathf.Atan2(localVelocity.y, Mathf.Max(localVelocity.z, 0.01f));

/*        //Forward Velocity Limiter
        float forwardVelocity = Vector3.Dot(rb.velocity, Vector3.right);
        float straightening = 0.0f;
        if (rb.velocity.z < 1.0f) {
            // Adjust control inputs to align with velocity
            Vector3 forwardVelocityDirection = rb.velocity.normalized;
            float angleToVelocity = Vector3.SignedAngle(transform.forward, Vector3.right, Vector3.up);
            straightening = Mathf.Sign(angleToVelocity) * Mathf.Clamp(Mathf.Abs(angleToVelocity) / 180, 0, 1);
        }

        //LERP between control input and straighten as velocity approaches 0 (so t is 1.0 at 0, 0.0 at 0.5f)
        float t = Mathf.Clamp01(rb.velocity.z / 0.5f);
        controlInputs.x = Mathf.Lerp(straightening, controlInputs.x, t);*/

            //Elevator force becomes less effective at abs(AoA) > 20
            Vector3 elevatorForce = controlInputs.x * transform.up * localVelocity.sqrMagnitude * 0.1f * Mathf.Cos(Mathf.Abs(AoA) * Mathf.Deg2Rad);
            acceleration = elevatorForce / rb.mass;
            rb.AddForceAtPosition(elevatorForce, transform.TransformPoint(new Vector3(0, 0, -4)));
    
            // Add a force at the tail of the plane that corresponds to the deflection of controlInputs.x
            Vector3 restoringForce = -2*Mathf.Sin(0.5f*AoA * Mathf.Deg2Rad) * transform.up * localVelocity.sqrMagnitude * 0.01f;
            rb.AddForceAtPosition(restoringForce, transform.TransformPoint(new Vector3(0, 0, -1)));

            // Lift force at the center of the plane that corresponds to AoA and velocity
            Vector3 liftForce = localVelocity.sqrMagnitude * wingArea * 0.1f * -Mathf.Sin(AoA * Mathf.Deg2Rad) * transform.up;
            rb.AddForceAtPosition(liftForce, transform.TransformPoint(new Vector3(0, 0, -1.0f)));
    
            throttle = controlInputs.y; 
            rb.AddForceAtPosition(throttle * thrust * transform.forward, transform.TransformPoint(new Vector3(0, 0, -4)));

            //float drag = 0.5f * 1.225f * localVelocity.sqrMagnitude * 0.01f * wingArea;
            //rb.AddForce(drag * -localVelocity.normalized, ForceMode.Force);
    }


    // Update is called once per frame
    void Update()
    {
        //Get key inputs
        if (Input.GetKey(Up))
        {
            controlInputs.x = 1;
        }
        else if (Input.GetKey(Down))
        {
             controlInputs.x = -1;
        }
        else
        {
            controlInputs.x = 0;
        }
        if (Input.GetKey(Left))
        {
            controlInputs.y = 0.0f;
        }
        else if (Input.GetKey(Right))
        {
            controlInputs.y = 1;
        }
        else
        {
            controlInputs.y = 0.7f;
        }

        //Draw Debug sphere for CoM/CoL
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position + centerOfMass, 0.1f);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(transform.position + centerOfLift, 0.1f);

        //Draw debug line for velocity
        Debug.DrawRay(transform.position, transform.TransformDirection(localVelocity), Color.magenta);
        foreach (LiftingBody lb in GetComponents<LiftingBody>())
        {
            lb.DrawDebugLines();
        }
        //Draw debug line for lift Direction
        //Debug.DrawRay(transform.position, rb.GetPointVelocity(transform.position), Color.green);
        
    }
}
