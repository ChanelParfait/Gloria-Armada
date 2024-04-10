using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
struct AeroSurface {
    public float area;
    public Vector3 position;
    public Vector3 orientation;
}

[Serializable]
struct Surfaces{
    public AeroSurface wing, tail, rudder, horizontalStabilizer, aileron;

}

[Serializable]
struct PID{
    public float Kp, Ki, Kd;
    public float integral, lastError;
    public float getInt(){
        return integral;
    }
    public float getLastError(){
        return lastError;
    }
    public void setInt(float i){
        integral = i;
    }
    public void setLastError(float e){
        lastError = e;
    }
}

public class Plane : MonoBehaviour
{

    //KEYS
    public KeyCode Up = KeyCode.W, Down = KeyCode.S, Left = KeyCode.A, Right = KeyCode.D, YawRight = KeyCode.E, YawLeft = KeyCode.Q;
    public KeyCode throttleUp = KeyCode.Tab, throttleDown = KeyCode.LeftShift;

    [SerializeField] Vector3 controlDeflection = Vector3.zero;
    [SerializeField] Vector3 autopilotDeflection = Vector3.zero;
    
    public float throttle = 0.7f;

    // Aircraft parameters
    [SerializeField] float liftPower = 2f;
    [SerializeField] float weight = 200;    // Weight of the aircraft (kg)
    [SerializeField] Surfaces surfaces;
    [SerializeField] float thrust = 1800;   // Maximum thrust (N)

    Vector3 controlInputs;

    // Aircraft state
    [Space(10)]
    [Header("Aircraft State")]
    public float AoA;
    public float AoAYaw;
    public Vector3 internalVelocity;   // Velocity of the aircraft (not passed to RB) (m/s)
    public Vector3 localVelocity;      // Velocity of the aircraft from local (m/s)
    private Vector3 localAngularVelocity; // Angular velocity of the aircraft (rad/s)
    public Vector3 acceleration;  // Acceleration of the aircraft (m/s^2)


    [SerializeField] bool autoArm = true;

    [SerializeField] Dictionary<string, bool> APSystems = new() {
        {"AutoPilot", false},
        {"AutoThrottle", false},
        {"AutoTrim", false},
        {"AutoYaw", false},
        {"AutoRoll", false},
        {"AutoPitch", false},
        {"AutoStabilize", false},
        {"AutoVelocity", false},
        {"AutoForward", false}
    };

    public enum AutopilotState {Off = 0, targetPoint = 1, targetFlat = 2, targetVelocity = 3, targetStabilize = 4, targetForward = 5};

    public AutopilotState autopilotState = AutopilotState.Off;
    AutopilotState lastAutopilotState = AutopilotState.Off;

    public float AoATrim = 24.0f;

    [SerializeField] float cd = 0.2f;
    [SerializeField] AnimationCurve cl = new AnimationCurve();

    [SerializeField] PID pitchPID, yawPID, rollPID, velPID;
    // PID parameters for pitch and yaw
    float Kp_pitch = 0.05f, Ki_pitch = 0.02f, Kd_pitch = 0.02f;
    float Kp_yaw = 1.0f, Ki_yaw = 0.1f, Kd_yaw = 0.5f;
    float Kp_vel = 1.0f, Ki_vel = 0.1f, Kd_vel = 0.5f;

    // PID state for pitch and yaw
    private float integral_pitch, lastError_pitch;
    private float integral_yaw, lastError_yaw;
    private float integral_vel, lastError_vel;

    [SerializeField] GameObject targetObject;
    Vector3 targetLocation = Vector3.zero;
    Vector3 targetAngle = Vector3.up;


    float[] PIDTerms = new float[15];
    // Unity
    [SerializeField] Rigidbody rb;
    

    private void Awake()
    {
        rb.drag = float.Epsilon;
        rb.angularDrag = 0.2f ;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Set the center of mass
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;
        rb.velocity = transform.forward * 20;
        //targetObject = GameObject.Find("TargetPoint");

        PIDTerms = new float[] { Kp_pitch, Ki_pitch, Kd_pitch, Kp_yaw, Ki_yaw, Kd_yaw, Kp_vel, Ki_vel, Kd_vel, integral_pitch, lastError_pitch, integral_yaw, lastError_yaw, integral_vel, lastError_vel };        
    }

    void CalculateState(float dt){
        var InverseRotation = Quaternion.Inverse(transform.rotation);
        internalVelocity = rb.velocity;
        localVelocity = InverseRotation * internalVelocity;
        localAngularVelocity = InverseRotation * rb.angularVelocity;
    }

    void CalculateAoA(){
        if (localVelocity.sqrMagnitude < 0.1f){
            AoA = 0;
            AoAYaw = 0;
            return;
        }
        AoA = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
        AoAYaw = Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg;

    }

    void UpdateThrust(){
        rb.AddRelativeForce(throttle * thrust * Vector3.forward);
    }

    void UpdateDrag(){
        //Note: cd is not currently scaled by plane orientation
        float drag = 0.5f * cd * localVelocity.sqrMagnitude;
        rb.AddRelativeForce(-localVelocity.normalized * drag);
    }

    void UpdateAngularDrag(){
        //Resist rotation around Z axis
        rb.AddTorque(-transform.InverseTransformDirection(rb.angularVelocity).z* 1000f * transform.forward);
    }

    void UpdateLift(){
        Vector3 liftForce = CalculateLift(AoA,
                                         surfaces.wing.orientation,
                                         liftPower * surfaces.wing.area);
        Vector3 yawForce = CalculateLift(AoAYaw,
                                         surfaces.tail.orientation,
                                         liftPower * surfaces.tail.area);


        //Control surfaces
        Vector3 hosForce = CalculateLift(AoA + controlDeflection.x,     
                                        surfaces.horizontalStabilizer.orientation, 
                                        liftPower * surfaces.horizontalStabilizer.area);
        Vector3 rudForce = CalculateLift(AoAYaw + controlDeflection.z,  
                                        surfaces.rudder.orientation, 
                                        liftPower * surfaces.rudder.area);
        Vector3 ailForceR = CalculateLift(AoA - controlDeflection.y,
                                        surfaces.aileron.orientation, 
                                        liftPower * surfaces.aileron.area);
        Vector3 ailForceL = CalculateLift(AoA + controlDeflection.y,
                                        surfaces.aileron.orientation, 
                                        liftPower * surfaces.aileron.area);

        rb.AddRelativeForce(liftForce);
        rb.AddRelativeTorque(Vector3.Cross(surfaces.wing.position, liftForce));
        rb.AddRelativeForce(yawForce);
        rb.AddRelativeTorque(Vector3.Cross(surfaces.tail.position, yawForce));

        //Control surfaces as torque only - to make it easier to control
        rb.AddRelativeTorque(Vector3.Cross(surfaces.horizontalStabilizer.position, hosForce)); 
        rb.AddRelativeTorque(Vector3.Cross(surfaces.rudder.position, rudForce)); 

        //Left and right ailerons
        rb.AddRelativeForce(ailForceR + ailForceL);
        rb.AddRelativeTorque(Vector3.Cross(surfaces.aileron.position, ailForceR));
        rb.AddRelativeTorque(Vector3.Cross(-surfaces.aileron.position, ailForceL));
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightVector, float lp){
        var liftVelocity = Vector3.ProjectOnPlane(localVelocity, rightVector);
        var v2 = liftVelocity.sqrMagnitude;

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        var liftCoefficient = cl.Evaluate(angleOfAttack);
        float liftForce = v2 * liftCoefficient * lp;

        //lift is perpendicular to velocity
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightVector);
        var lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        var dragForce = liftCoefficient * liftCoefficient;
        var dragDirection = -liftVelocity.normalized;
        var inducedDrag = dragDirection * v2 * dragForce * cd;
        // Again cd not scaled by plane orientation
        //var inducedDrag = dragDirection * v2 * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, LocalVelocity.z));
        return lift + inducedDrag;
    }
    
    void FixedUpdate(){
        float dt = Time.fixedDeltaTime;

        controlDeflection = new Vector3(controlInputs.x * 30f - 0.0f, controlInputs.y * 20f, controlInputs.z * 20f);
        CalculateState(dt);
        CalculateAoA();

        UpdateThrust();
        UpdateDrag();
        UpdateLift();
        UpdateAngularDrag();

    }

    float PIDSolve(float error, ref PID pid){
        float proportional = error * pid.Kp;
        pid.integral += error * Time.deltaTime;
        float integralTerm = pid.integral * pid.Ki;
        float derivative = (error - pid.lastError) / Time.deltaTime;
        float derivativeTerm = derivative * pid.Kd;
        pid.lastError = error;

        return proportional + integralTerm + derivativeTerm;
    }

    Vector3 AutoTargetPoint(Vector3 targetPoint){
        //Vector3 targetPoint = targetObject.transform.position;
        Vector3 targetDirection = (targetPoint - rb.transform.position).normalized;
        // Get the angle between the target point and the aircraft's forward in the vertical plane
        // Max pull is when 90 degrees or more offset from target
        float angleUp = Vector3.SignedAngle(transform.forward, targetDirection, transform.right) / 180f;
        // Get the roll angle to the target point
        float angleRoll = -Vector3.SignedAngle(transform.up, targetDirection, transform.forward) /180f;
        // Get the angle between the target point and the aircraft's forward in the horizontal plane
        float angleRight = Vector3.SignedAngle(transform.forward, targetDirection, transform.up) / 180f * 2f;
        

        //Draw debug line for target direction
        Debug.DrawRay(transform.position, targetDirection * angleUp, Color.yellow);
        Debug.DrawRay(transform.position, targetDirection * angleRight, Color.green);

        Debug.DrawRay(transform.position, transform.right * angleRight, Color.red);
        Debug.DrawRay(transform.position, transform.up * angleUp, Color.blue);

        float autoXInput = Mathf.Clamp(PIDSolve(angleUp, ref pitchPID), -1, 1);
        float autoYInput = Mathf.Clamp(PIDSolve(angleRoll, ref rollPID), -1.0f, 1.0f);
        float autoZInput = Mathf.Clamp(PIDSolve(angleRight, ref yawPID), -1.0f, 1.0f);

        

        return new Vector3(autoXInput,autoYInput, autoZInput);
    }

    Vector3 AutoTargetVelocityVector(Vector3 targetVelocity){
        Vector3 curVelocity = internalVelocity;
        Vector3 velocityError = targetVelocity - curVelocity.normalized;

        //Get signed angles for angle up, roll, right
        float anglePitch       = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.right) / 180f;
        float angleLiftVUp    = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward)       / 180f;
        float angleAlignLiftV = -Vector3.SignedAngle(rb.transform.up, velocityError, rb.transform.forward)    / 180f;
        float angleYaw         = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.up)    / 180f;



        // Multiple signed angle by the magnitude of the error projected in that direction
        anglePitch *= Vector3.Cross(velocityError, rb.transform.right).magnitude;
        angleAlignLiftV *= Vector3.Cross(velocityError, rb.transform.forward).magnitude;
        angleYaw *= Vector3.Cross(velocityError, rb.transform.up).magnitude;

        
        //blend between a roll angle of "Up" when velocity is close to target and a roll angle that points to target when not
        float angleRoll = Mathf.Lerp(angleLiftVUp, angleAlignLiftV, Mathf.Clamp01(velocityError.magnitude));

        //Debug.DrawRay(transform.position, velocityError * 100.0f, Color.yellow);
        //Debug.DrawRay(transform.position, targetVelocity * 100.0f, Color.red);

        float x = PIDSolve(anglePitch, ref pitchPID);
        float y = PIDSolve(angleRoll, ref rollPID);
        float z = PIDSolve(angleYaw, ref yawPID);
        return new Vector3(x, y, z);
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
    Vector3 AutoTargetPlane(char plane = 'Y'){
        //E.g. target the YZ plane with no deviation in X
        Char.ToLower(plane); //Ensure plane is lowercase (e.g. 'Y' -> 'y')

        //Draw a 45 degree angle from current position to the target plane and find an intercept point
        //This will be the target angle
        Vector3 interceptRay;
        Vector3 pos = transform.position;
        Vector3 invPosSigns = new Vector3(-Mathf.Sign(pos.x), -Mathf.Sign(pos.y), -Mathf.Sign(pos.z)); 
        Vector3 targetPosition;
        switch (plane){
            case 'y':
                interceptRay = new Vector3(1, 0, 1);
                //plot the interception point from our pos + interceptRay to reach X = 0
                targetPosition = pos + interceptRay * (pos.x / interceptRay.x);
                break;
            case 'x':
                interceptRay = new Vector3(0, 1, 1);
                targetPosition = pos + interceptRay * (pos.y / interceptRay.y);
                break;
            case 'z':
                interceptRay = new Vector3(1, 1, 0);
                targetPosition = pos + interceptRay * (pos.z / interceptRay.z);
                break;
            default:
                interceptRay = new Vector3(1, 0, 1);
                targetPosition = pos + interceptRay * (pos.x / interceptRay.x);
                break;
        }
        Debug.Log("Target Position: " + targetPosition);
        return AutoTargetPoint(targetPosition);
        // float offsetPos;
        // // Get the roll angle to the target point
        // float angleRoll = Vector3.SignedAngle(transform.up, targetAngle, transform.forward) /180f;
        // switch (plane){
        //     case 'y':
        //         offsetPos = transform.position.y;
        //         float x, y, z;
        //         x = PIDSolve(offsetPos, ref yawPID);
        //         y = PIDSolve(angleRoll , ref rollPID);
        //         z = PIDSolve(targetPosition.z, ref pitchPID);
        //         return new Vector3(x, y, z);
        //     case 'x':
        //         offsetPos = transform.position.x;
        //         x = PIDSolve(offsetPos, ref yawPID);
        //         y = PIDSolve(angleRoll , ref rollPID);
        //         z = PIDSolve(targetLocation.z, ref pitchPID);
        //         return new Vector3(x, y, z);
        //     case 'z':
        //         offsetPos = transform.position.z;
        //         x = PIDSolve(offsetPos, ref yawPID);
        //         y = PIDSolve(angleRoll , ref rollPID);
        //         z = PIDSolve(targetLocation.z, ref pitchPID);
        //         return new Vector3(x, y, z);
        //     default:
        //         offsetPos = transform.position.y;
        //         x = PIDSolve(offsetPos, ref yawPID);
        //         y = PIDSolve(angleRoll , ref rollPID);
        //         z = PIDSolve(targetPosition.z, ref pitchPID);
        //         return new Vector3(x, y, z);
        //}
    }

    Vector3 AutopilotControl(AutopilotState state){
        
        autopilotDeflection.x = controlInputs.x;
        autopilotDeflection.y = controlInputs.y;
        autopilotDeflection.z = controlInputs.z;

        if (state == AutopilotState.targetPoint){
            autopilotDeflection = AutoTargetPoint(targetObject.transform.position);
        }
        else if (state == AutopilotState.targetStabilize){
            autopilotDeflection.x = AutoTargetStabilize();
        }
        else if (state == AutopilotState.targetFlat){
            autopilotDeflection = AutoTargetPlane('Y');
        }
        else if (state == AutopilotState.targetVelocity){
            autopilotDeflection = AutoTargetVelocityVector((targetObject.transform.position - rb.transform.position).normalized);
            //autopilotDeflection = AutoTargetVelocityVector(new Vector3(0, 0, 1));
        }
        else if (state == AutopilotState.targetForward){
            autopilotDeflection = AutoTargetPlane('Z');
        }
        else{
            autopilotDeflection = controlInputs;
        }

        //PID errors
        Debug.Log("PID Errors: " + pitchPID.lastError + ", " + yawPID.lastError + ", " + rollPID.lastError);

        return autopilotDeflection;
    }

    


    // Update is called once per frame
    void Update()
    {
        //Get key inputs -> these can be overridden by autopilot
        //Currently control inputs for all controls, we will simplify this later
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
        if (Input.GetKey(Right))
        {
            controlInputs.y = 1;
        }
        else if (Input.GetKey(Left))
        {
            controlInputs.y = -1;
        }
        else
        {
            controlInputs.y = 0.0f;
        }
        if (Input.GetKey(YawRight))
        {
            controlInputs.z = 1;
        }
        else if (Input.GetKey(YawLeft))
        {
            controlInputs.z = -1;
        }
        else
        {
            controlInputs.z = 0;
        }
        if (Input.GetKey(throttleUp))
        {
            throttle = 1f;
        }
        else if (Input.GetKey(throttleDown))
        {
            throttle = 0;
        }
        else
        {
            throttle = 0.7f;
        }

        if (autopilotState != AutopilotState.Off){
            if (autopilotState != lastAutopilotState){
                lastAutopilotState = autopilotState;
                switch (autopilotState){
                case AutopilotState.targetPoint:
                    targetLocation = targetObject.transform.position;
                    break;
                case AutopilotState.targetFlat:
                    targetLocation = transform.position;
                    break;
                case AutopilotState.targetStabilize:
                    break;
                case AutopilotState.targetVelocity:
                    break;
                case AutopilotState.targetForward:
                    break;
                default:
                    break;
                }
            }
            controlInputs = AutopilotControl(autopilotState);
        }

        //Draw debug line for velocity
        Debug.DrawRay(transform.position, transform.TransformDirection(localVelocity), Color.magenta);
        
    }
}
