using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;




public class Plane : MonoBehaviour
{

    //KEYS
    public KeyCode Up = KeyCode.W, Down = KeyCode.S, Left = KeyCode.A, Right = KeyCode.D;
    public KeyCode throttleUp = KeyCode.LeftShift, throttleDown = KeyCode.LeftControl;

    // Aircraft parameters
    [SerializeField] float wingArea = 30.04f;  // Area of the wing (m^2)
    [SerializeField] float weight = 100;    // Weight of the aircraft (kg)

    //[SerializeField] float thrustWeightRatio = 0.9f;   // Maximum thrust (N)
    //[SerializeField] float controlAuthority = 1.0f;    // Elevator Force tuning value
    //[SerializeField] float stability = 1.0f;           // Stability tuning value
    [SerializeField] float thrust = 1800;   // Maximum thrust (N)

    Vector2 controlInputs;

    // Aircraft state
    public float throttle = 0.7f;

    public float AoA;
    public Vector3 internalVelocity;   // Velocity of the aircraft (not passed to RB) (m/s)
    public Vector3 localVelocity;      // Velocity of the aircraft from local (m/s)
    public Vector3 acceleration;  // Acceleration of the aircraft (m/s^2)

    public bool autoTargetPoint = false;
    bool autoTargetFlat = false;

    public enum AutopilotState {Off = 0, targetPoint = 1, targetFlat = 2, targetVelocity = 3, targetStabilize = 4, targetForward = 5};

    public AutopilotState autopilotState = AutopilotState.Off;
    AutopilotState lastAutopilotState = AutopilotState.Off;

    public float AoATrim = 24.0f;

    [SerializeField] AnimationCurve pitchCurve = new AnimationCurve();


    // PID parameters for pitch and yaw
    [SerializeField] float Kp_pitch = 1.0f, Ki_pitch = 0.1f, Kd_pitch = 0.5f;
    [SerializeField] float Kp_yaw = 1.0f, Ki_yaw = 0.1f, Kd_yaw = 0.5f;
    [SerializeField] float Kp_vel = 1.0f, Ki_vel = 0.1f, Kd_vel = 0.5f;

    // PID state for pitch and yaw
    private float integral_pitch, lastError_pitch;
    private float integral_yaw, lastError_yaw;
    private float integral_vel, lastError_vel;

    [SerializeField] GameObject targetObject;

    struct PID
    {
        public float Kp, Ki, Kd;
        public float integral, lastError;
    }

    float[] PIDTerms = new float[15];
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
        targetObject = GameObject.Find("TargetPoint");

        PIDTerms = new float[] { Kp_pitch, Ki_pitch, Kd_pitch, Kp_yaw, Ki_yaw, Kd_yaw, Kp_vel, Ki_vel, Kd_vel, integral_pitch, lastError_pitch, integral_yaw, lastError_yaw, integral_vel, lastError_vel };

        //PIDTerms = {Kp_pitch, Ki_pitch, Kd_pitch, Kp_yaw, Ki_yaw, Kd_yaw, Kp_vel, Ki_vel, Kd_vel, integral_pitch, lastError_pitch, integral_yaw, lastError_yaw, integral_vel, lastError_vel};
        
    }
    
    void FixedUpdate(){
        // Calculate the local velocity
        internalVelocity = Mathf.Min(rb.velocity.magnitude * 10, 450) * rb.velocity.normalized;
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

            //Elevator force becomes less effective at abs(AoA) > 20
            Vector3 elevatorForce = controlInputs.x * transform.up * localVelocity.sqrMagnitude * 0.1f * Mathf.Cos(Mathf.Abs(AoA) * Mathf.Deg2Rad);
            rb.AddForceAtPosition(elevatorForce, transform.TransformPoint(new Vector3(0, 0, -4)));
    
            // Add a force at the tail of the plane that corresponds to the deflection of controlInputs.x
            Vector3 restoringForce = -2*Mathf.Sin(0.5f*AoA * Mathf.Deg2Rad) * transform.up * localVelocity.sqrMagnitude * 0.05f;
            rb.AddForceAtPosition(restoringForce, transform.TransformPoint(new Vector3(0, 0, -1)));

            // Lift force at the center of the plane that corresponds to AoA and velocity
            Vector3 liftForce = localVelocity.sqrMagnitude * wingArea * 0.1f * -Mathf.Sin(AoA * Mathf.Deg2Rad) * transform.up;
            rb.AddForceAtPosition(liftForce, transform.TransformPoint(new Vector3(0, 0, -1.0f)));
    
            throttle = controlInputs.y; 
            rb.AddForceAtPosition(throttle * thrust * transform.forward, transform.TransformPoint(new Vector3(0, 0, -4)));

            //float drag = 0.5f * 1.225f * localVelocity.sqrMagnitude * 0.01f * wingArea;
            //rb.AddForce(drag * -localVelocity.normalized, ForceMode.Force);
    }

    float AutoTargetPoint(){
        Transform targetPoint = targetObject.transform;
        Vector3 targetDirection = (targetPoint.position - rb.transform.position).normalized;
        // Get the angle between the target point and the aircraft's forward direction in the X/Y plane
        float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.forward);

        //Draw debug line for target direction
        Debug.DrawRay(transform.position, targetDirection * angle, Color.green);

        // Minimize the value of angle using pitch PID
        float pitchError = angle;
        // PID for pitch
        float proportional_pitch = pitchError * Kp_pitch;
        integral_pitch += pitchError * Time.deltaTime;
        float integralTerm_pitch = integral_pitch * Ki_pitch;
        float derivative_pitch = (pitchError - lastError_pitch) / Time.deltaTime;
        float derivativeTerm_pitch = derivative_pitch * Kd_pitch;
        
        // Adjust pitch control input
        float autoXInput = proportional_pitch + integralTerm_pitch + derivativeTerm_pitch;
        Mathf.Clamp(autoXInput, -1f, 1f);

        // Update last error
        lastError_pitch = pitchError;

        // LERP between targetting and stabilizing based on angular velocity
        // float t = (Mathf.Clamp(rb.angularVelocity.magnitude, 1.5f, 5f) - 1.5f)/5f;
        // Debug.Log("t: " + t);   
        // float autoXInputStabilize = -AutoTargetStabilize();
        // autoXInput = Mathf.Lerp(autoXInput, autoXInputStabilize, t);


        return -autoXInput;
    }

    float AutoTargetStabilize(){
        //Attempt to restrain AoA to +-24 degrees
        float autoXInput = controlInputs.x;
        if (AoA > 24){
            float pitchError = AoA - 24;
            // PID for pitch
            float proportional_pitch = pitchError * Kp_pitch;
            integral_pitch += pitchError * Time.deltaTime;
            float integralTerm_pitch = integral_pitch * Ki_pitch;
            float derivative_pitch = (pitchError - lastError_pitch) / Time.deltaTime;
            float derivativeTerm_pitch = derivative_pitch * Kd_pitch;
            
            // Adjust pitch control input
            autoXInput = proportional_pitch + integralTerm_pitch + derivativeTerm_pitch;
            Mathf.Clamp(autoXInput, -1f, 1f);

            // Update last error
            lastError_pitch = pitchError;
        }
        // If angular velocity is high, stabilize the aircraft
        if (rb.angularVelocity.magnitude > 1f){
            float pitchError = rb.angularVelocity.z;
            // PID for pitch
            float proportional_pitch = pitchError * Kp_pitch;
            integral_pitch += pitchError * Time.deltaTime;
            float integralTerm_pitch = integral_pitch * Ki_pitch;
            float derivative_pitch = (pitchError - lastError_pitch) / Time.deltaTime;
            float derivativeTerm_pitch = derivative_pitch * Kd_pitch;

            // Adjust pitch control input
            autoXInput = proportional_pitch + integralTerm_pitch + derivativeTerm_pitch;   
            Mathf.Clamp(autoXInput, -1f, 1f);

            lastError_pitch = pitchError;
        }

        return autoXInput;
    }

    // Target a 2D plane
    float AutoTargetPlane(){

        return -1;
    }

    Vector2 AutopilotControl(AutopilotState state){
        
        float autoXInput = controlInputs.x;

        if (state == AutopilotState.targetPoint){
            autoXInput = AutoTargetPoint();
            
        }
        else if (state == AutopilotState.targetStabilize){
            autoXInput = AutoTargetStabilize();
        }
        else if (state == AutopilotState.targetFlat){
            autoXInput = AutoTargetPlane();
        }

        return new Vector2(autoXInput, 0.0f);

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
        if (Input.GetKey(throttleUp))
        {
            controlInputs.y = 1f;
        }
        else if (Input.GetKey(throttleDown))
        {
            controlInputs.y = 0;
        }
        else
        {
            controlInputs.y = 0.7f;
        }

        if (autopilotState != AutopilotState.Off)
        {
            controlInputs.x = AutopilotControl(autopilotState).x;
        }

        // If autopilot is toggled between modes reset the PID terms
        if (autopilotState != lastAutopilotState){
            for (int i = 0 ; i < PIDTerms.Length; i++){
               PIDTerms[i] = 0.0f;
            }
            lastAutopilotState = autopilotState;
        }

        //Draw debug line for velocity
        Debug.DrawRay(transform.position, transform.TransformDirection(localVelocity), Color.magenta);
        
    }
}
