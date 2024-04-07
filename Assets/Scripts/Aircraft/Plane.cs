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

    //Default surfaces
    LiftingBody wingL;
    LiftingBody wingR;
    LiftingBody tail;
    LiftingBody horizontalStab;

    // Aircraft parameters
    [SerializeField] float wingArea = 30.04f;  // Area of the wing (m^2)
    [SerializeField] float weight = 19670;    // Weight of the aircraft (kg)
    [SerializeField] float thrust = 232000;   // Maximum thrust (N)
    // [SerializeField] float wingSpan = 22;      // Wing span (meters)
    // [SerializeField] float aspectRatio = 7.5f; // Aspect ratio
    [SerializeField] Vector3 centerOfLift;  // Center of lift (relative to the object's origin)
    [SerializeField] Vector3 centerOfMass;  // Center of mass (relative to the object's origin)

    // Aircraft state
    public float throttle = 0.7f;

    public Vector3 localVelocity;      // Velocity of the aircraft (m/s)
    public Vector3 acceleration;  // Acceleration of the aircraft (m/s^2)

    public float p = 1.225f; // air density

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
        rb.centerOfMass = centerOfMass;
        rb.mass = weight;
        rb.velocity = transform.forward * 35;

        // Add lifting bodies to the aircraft
        wingL = LiftingBody.AddLiftingBody(gameObject, "Wing Left", new Vector3(-2.5f, -0.0f, -0.05f), new Vector3(0.0f, 0, 0), wingArea, 1.0f);
        wingR = LiftingBody.AddLiftingBody(gameObject, "Wing Right", new Vector3(2.5f, -0.0f, -0.05f), new Vector3(0.0f, 0, 0), wingArea, 1.0f);
        //tail = LiftingBody.AddLiftingBody(gameObject, "Tail", new Vector3(0, 0.20f, -4f), new Vector3(0.0f, 00.0f, 90.0f), wingArea / 2.0f, 1.0f);
        horizontalStab = LiftingBody.AddLiftingBody(gameObject, "Horizontal Stabilizer", new Vector3(0, 0, -4.5f), new Vector3(0.0f, 0, 0), wingArea/6, 1.0f);
        
    }
    
    void FixedUpdate(){
        // Calculate the local velocity
        localVelocity = transform.InverseTransformDirection(rb.velocity);

        // Calculate the lift and drag forces
        Vector3 liftForce = Vector3.zero;
        Vector3 dragForce = Vector3.zero;

        // Calculate the thrust force
        Vector3 thrustForce = throttle * thrust * transform.forward;

        // Calculate the weight force
        Vector3 weightForce = Vector3.down * weight;
        

        //rb.AddForceAtPosition(weightForce, transform.position + centerOfMass);
        foreach (LiftingBody lb in GetComponents<LiftingBody>())
        {
            Vector3 lift = new Vector3();
            Vector3 drag = new Vector3();
            
            rb.AddForceAtPosition(lb.CalculateForces(localVelocity), transform.TransformPoint(lb.relativePosition));
            //rb.AddForceAtPosition(lb.CalculateDrag(localVelocity), transform.TransformPoint(lb.relativePosition));
        }

        // Apply the acceleration to the aircraft
        //rb.AddForce(acceleration);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + centerOfMass, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + centerOfLift, 0.1f);
        Gizmos.color = Color.green;

        if (wingL != null)
        {
            Gizmos.DrawCube(transform.TransformPoint(wingL.relativePosition), new Vector3(3.0f , 1 , 0.1f));
            Gizmos.DrawCube(transform.TransformPoint(wingR.relativePosition), new Vector3(3.0f , 1 , 0.1f));
            Gizmos.color = Color.yellow;
            //Gizmos.DrawCube(transform.TransformPoint(tail.relativePosition), new Vector3(3.0f, 1, .1f));
            //Gizmos.DrawCube(transform.TransformPoint(horizontalStab.relativePosition), new Vector3(3.0f, 1, 0.1f));
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Get key inputs
        if (Input.GetKey(Up))
        {
            horizontalStab.relativeRotation = Quaternion.Euler(new Vector3(0, 30, 0));
        }
        else if (Input.GetKey(Down))
        {
             horizontalStab.relativeRotation = Quaternion.Euler(new Vector3(0, -30, 0));
        }
        else
        {
            horizontalStab.relativeRotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
        Debug.DrawRay(transform.position, transform.TransformDirection(localVelocity), Color.magenta);
        foreach (LiftingBody lb in GetComponents<LiftingBody>())
        {
            lb.DrawDebugLines();
        }
        //Draw debug line for lift Direction
        //Debug.DrawRay(transform.position, rb.GetPointVelocity(transform.position), Color.green);
        
    }
}
