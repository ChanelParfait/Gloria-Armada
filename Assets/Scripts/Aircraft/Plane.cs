using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{

    //KEYS
    public KeyCode elevatorUp = KeyCode.W;
    public KeyCode elevatorDown = KeyCode.S;

    public KeyCode throttleUp = KeyCode.LeftShift;



    // Aircraft parameters
    [SerializeField] float wingArea = 78.04f;  // Area of the wing (m^2)
    [SerializeField] float weight = 19670;    // Weight of the aircraft (kg)
    [SerializeField] float thrust = 232000;   // Maximum thrust (N)
    [SerializeField] float wingSpan = 22;      // Wing span (meters)
    [SerializeField] float aspectRatio = 7.5f; // Aspect ratio
    [SerializeField] Vector3 centerOfLift;  // Center of lift (relative to the object's origin)
    [SerializeField] Vector3 centerOfMass;  // Center of mass (relative to the object's origin)

    // Aircraft state
    public float throttle = 0.7f;

    public Vector3 localVelocity;      // Velocity of the aircraft (m/s)
    public Vector3 acceleration;  // Acceleration of the aircraft (m/s^2)
    public Vector3 angularVelocity; // Angular velocity of the aircraft (rad/s)
    public Vector3 angularAcceleration; // Angular acceleration of the aircraft (rad/s^2)
    public float angleOfAttack;   // Angle of attack (degrees)
    public float liftCoefficient; // Lift coefficient
    public float dragCoefficient; // Drag coefficient
    public float thrustCoefficient; // Thrust coefficient

    public float p; // air density

    // Unity
    [SerializeField] Rigidbody rb;

    private void Awake()
    {
        rb.drag = float.Epsilon;
    }
    // Start is called before the first frame update
    void Start()
    {
        p = 1.225f; // standard air density at sea level
        //Center of lift slightly aft of the center of mass
        centerOfLift = new Vector3(0.0f, 0.0f, -0.0f);
        centerOfMass = new Vector3(0, 0, 0);
        aspectRatio = (wingSpan * wingSpan) / wingArea; 
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
        rb.mass = weight;
        rb.velocity = transform.forward * 50;
        
    }
    
    void FixedUpdate(){
        //Convert worldspace rb velocity to local velocity 
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

      
        // Lift Coefficient (Cl)
        float Cl0 = 0.2f; // Lift coefficient at zero angle of attack (example value)
        float Cla = 2 * Mathf.PI / 180; // Lift curve slope (around 0.11 per degree)
        float liftCoefficient = Cl0 + Cla * angleOfAttack;
        float liftForceMagnitude = 0.5f * wingArea * p * localVelocity.magnitude * localVelocity.magnitude * liftCoefficient;
        Vector3 liftDirection = Vector3.Cross(rb.velocity.normalized, transform.right).normalized;
        //draw Debug Line for lift Direction
        Debug.DrawRay(transform.position + centerOfLift , liftDirection, Color.blue);
        // Apply the Lift Force
        Vector3 liftForce = liftForceMagnitude * liftDirection;
         rb.AddForceAtPosition(liftForce, transform.position + centerOfLift);

        // Drag Coefficient (Cd)
        float Cd0 = 0.02f; // Zero-lift drag coefficient (example value)
        float e = 0.8f; // Oswald efficiency factor (example value)
        float inducedDrag = liftCoefficient * liftCoefficient / (Mathf.PI * e * aspectRatio);
        float dragCoefficient = Cd0 + inducedDrag;

        // Calculate the lift and drag coefficients
        // liftCoefficient = 0.5f * wingArea * p * localVelocity.magnitude * localVelocity.magnitude;
        // dragCoefficient = 0.5f * wingArea * p * localVelocity.magnitude * localVelocity.magnitude;

        // Calculate the thrust coefficient
        thrustCoefficient = throttle*thrust / weight;

  
/*        var inducedLift = angleOfAttack * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI;
        var inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);*/

        // Calculate the lift and drag forces
        Vector3 dragDirection = -localVelocity.normalized;
        //Vector3 liftDirection = Vector3.Cross(dragDirection, transform.right);
        //Vector3 lift = inducedLift * liftCoefficient * liftDirection;
        Vector3 drag = inducedDrag * dragCoefficient * dragDirection;

        // Calculate the thrust force
        Vector3 thrustForce = thrustCoefficient * transform.forward;

        // Update the rigidbody
        //rb.AddForceAtPosition(drag, transform.position);
       

    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + centerOfMass, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + centerOfLift, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        //Get key inputs
        if (Input.GetKey(elevatorUp))
        {
            rb.AddForceAtPosition(transform.up, transform.position + new Vector3(0,0,-0.5f));
        }
        if (Input.GetKey(elevatorDown))
        {
             rb.AddForceAtPosition(-transform.up, transform.position + new Vector3(0,0,-0.5f));
        }
        if (Input.GetKey(throttleUp))
        {
            throttle = 1.0f;
        }

        //Draw Debug sphere for CoM/CoL
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position + centerOfMass, 0.1f);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(transform.position + centerOfLift, 0.1f);

        //Draw debug line for velocity
        Debug.DrawRay(transform.position, localVelocity, Color.red);
        //Draw debug line for lift Direction
        //Debug.DrawRay(transform.position, rb.GetPointVelocity(transform.position), Color.green);
        
    }
}
