using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;


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

    public PID(float _Kp, float _Ki, float _Kd){
        Kp = _Kp;
        Ki = _Ki;
        Kd = _Kd;
        integral = 0;
        lastError = 0;
    }
}


public class Autopilot : MonoBehaviour
{
    [SerializeField] Vector3 autopilotDeflection = new Vector3(0, 0, 0);
    Vector3 controlInputs = new Vector3(0, 0, 0);
    
    public enum AutopilotState {Off = 0, targetPoint = 1, targetFlat = 2, targetVelocity = 3, targetStabilize = 4, targetForward = 5};
    [Header("AP State")]
    public AutopilotState autopilotState = AutopilotState.Off;
    AutopilotState lastAutopilotState = AutopilotState.Off;

    Rigidbody rb = null;
    Plane p = null;
    
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

    [Space(10)]
    [Header("PID Controllers")]
    [SerializeField] PID pitchPID = new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID yawPID =   new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID rollPID =  new PID(1.0f, 0.01f, 0.2f);


    [Space(10)]
    [Header("Targets")]
    [SerializeField] GameObject[] targetObjects;

    Dictionary<string, GameObject> targetDict = new Dictionary<string, GameObject>();

    [Space(10)] 
    [Header("Target")]
    [SerializeField] GameObject targetObject;
    public Vector3 targetLocation = new Vector3(0, 0, 0);
    public Vector3 aimVector = new Vector3(0, 0, 0);
    public float rangeToTarget = float.MaxValue;

    public float shotSpeed = 100f;





    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        p = GetComponent<Plane>(); 

        //based on the tag
        if (tag == "Player"){
            targetObjects = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else{
            targetObjects = GameObject.FindGameObjectsWithTag("Player");
        }
        foreach(GameObject target in targetObjects){
            targetDict.Add(target.name, target);
        }
    }

    public bool HasTarget(){
        if (targetObject != null){
            if (targetObject.GetComponent<Plane>() != null){
                if (!targetObject.GetComponent<Plane>().isAlive){
                    targetDict.Remove(targetObject.name);
                    if (targetDict.Count > 0){
                        targetObject = targetDict.FirstOrDefault().Value;
                    }
                    else{

                        targetObject = null;
                    }   

                }
                aimVector = tPosition(targetObject.transform.position);
                rangeToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                return true;
            }
        }
        aimVector = new Vector3(0, 0, 0);
        rangeToTarget = float.MaxValue;
        return false;
    }



    // Update is called once per frame
    void Update()
    {
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
        
    }

    public Vector3 GetAPInput(Vector3 _controlInputs){
        controlInputs = _controlInputs;
        return AutopilotControl(autopilotState);
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

    Vector3 tPosition(Vector3 targetPoint){
        // Get the plane component of the target object
        Plane e = targetObject.GetComponent<Plane>();
        Vector3 ip = Utilities.FirstOrderIntercept(rb.transform.position, rb.velocity, rb.velocity.magnitude + shotSpeed, targetPoint, e.getRBVelocity());
        // Vector3 leadIP = Utilities.FirstOrderIntercept(rb.transform.position, rb.velocity, shotSpeed, targetPoint, enemyPlane.getRBVelocity());
        // Vector3 lagIP = targetPoint + (targetPoint - leadIP);
        e.getRBVelocity();
        Vector3 ip2 = Utilities.SecondOrderIntercept(rb.transform.position, rb.velocity, rb.velocity.magnitude + shotSpeed, targetPoint, e.getRBVelocity(), e.acceleration, 0);

        //Draw a debug line to the intercept point
        Debug.DrawLine(transform.position, ip, Color.red);
        Debug.DrawLine(ip, ip2, Color.yellow);
        //Debug.DrawLine(lagIP, ip, Color.green);
        //Vector3 targetPoint = targetObject.transform.position;
        Vector3 targetDirection = (ip2 - rb.transform.position).normalized;
        // Get the angle between the target point and the aircraft's forward in the vertical plane
        // Max pull is when 90 degrees or more offset from target
        float anglePitch = Vector3.SignedAngle(transform.forward, targetDirection, transform.right) / 180f;
        // Get the roll angle to the target point
        float angleRoll = -Vector3.SignedAngle(transform.up, targetDirection, transform.forward) /180f;
        // Get the angle between the target point and the aircraft's forward in the horizontal plane
        float angleYaw = Vector3.SignedAngle(transform.forward, targetDirection, transform.up) / 180f * 2f;
        
        // Multiply signed angle by the magnitude of the error projected in that direction
        anglePitch *= Vector3.Cross(targetDirection, rb.transform.right).magnitude;
        angleRoll *= Vector3.Cross(targetDirection, rb.transform.forward).magnitude;
        angleYaw *= Vector3.Cross(targetDirection, rb.transform.up).magnitude;

        //Draw debug line for target direction
        Debug.DrawRay(transform.position, targetDirection * anglePitch, Color.yellow);
        Debug.DrawRay(transform.position, targetDirection * angleYaw, Color.green);

        Debug.DrawRay(transform.position, transform.right * angleYaw, Color.red);
        Debug.DrawRay(transform.position, transform.up * anglePitch, Color.blue);

        float autoXInput = Mathf.Clamp(PIDSolve(anglePitch, ref pitchPID), -1, 1);
        float autoYInput = Mathf.Clamp(PIDSolve(angleRoll, ref rollPID), -1.0f, 1.0f);
        float autoZInput = Mathf.Clamp(PIDSolve(angleYaw, ref yawPID), -1.0f, 1.0f);

        return new Vector3(autoXInput,autoYInput, autoZInput);
    }

    Vector3 tVelocityVec(Vector3 targetVelocity){
        Vector3 curVelocity = p.internalVelocity;
        Vector3 velocityError = targetVelocity - curVelocity.normalized;

        //Get signed angles for angle up, roll, right
        float anglePitch       = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.right) / 180f;
        float angleLiftVUp    = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward)       / 180f;
        float angleAlignLiftV = -Vector3.SignedAngle(rb.transform.up, velocityError, rb.transform.forward)    / 180f;
        float angleYaw         = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.up)    / 180f;

        // Multiply signed angle by the magnitude of the error projected in that direction
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
        // float autoXInput = controlInputs.x;
        // if (AoA > 24){
        //     float pitchError = AoA - 24;
        //     // PID for pitch
        //     float proportional_pitch = pitchError * Kp_pitch;
        //     integral_pitch += pitchError * Time.deltaTime;
        //     float integralTerm_pitch = integral_pitch * Ki_pitch;
        //     float derivative_pitch = (pitchError - lastError_pitch) / Time.deltaTime;
        //     float derivativeTerm_pitch = derivative_pitch * Kd_pitch;
            
        //     // Adjust pitch control input
        //     autoXInput = proportional_pitch + integralTerm_pitch + derivativeTerm_pitch;
        //     Mathf.Clamp(autoXInput, -1f, 1f);

        //     // Update last error
        //     lastError_pitch = pitchError;
        // }
        // // If angular velocity is high, stabilize the aircraft
        // if (rb.angularVelocity.magnitude > 1f){
        //     float pitchError = rb.angularVelocity.z;
        //     // PID for pitch
        //     float proportional_pitch = pitchError * Kp_pitch;
        //     integral_pitch += pitchError * Time.deltaTime;
        //     float integralTerm_pitch = integral_pitch * Ki_pitch;
        //     float derivative_pitch = (pitchError - lastError_pitch) / Time.deltaTime;
        //     float derivativeTerm_pitch = derivative_pitch * Kd_pitch;

        //     // Adjust pitch control input
        //     autoXInput = proportional_pitch + integralTerm_pitch + derivativeTerm_pitch;   
        //     Mathf.Clamp(autoXInput, -1f, 1f);

        //     lastError_pitch = pitchError;
        // }

        return -1;
    }

    // Target a 2D plane
    Vector3 AutoTargetPlane(char plane = 'Y'){
        return new Vector3(0, 0, 0);
    }

    Vector3 AutopilotControl(AutopilotState state){
        
        autopilotDeflection = controlInputs;
        if (state == AutopilotState.Off){
            return controlInputs;
        }
        else

        if (state == AutopilotState.targetPoint){
            autopilotDeflection = tPosition(targetObject.transform.position);
        }
        else if (state == AutopilotState.targetStabilize){
            autopilotDeflection.x = AutoTargetStabilize();
        }
        else if (state == AutopilotState.targetFlat){
            autopilotDeflection = AutoTargetPlane('Y');
        }
        else if (state == AutopilotState.targetVelocity){
            if (HasTarget()){
                autopilotDeflection = tVelocityVec((targetObject.transform.position - rb.transform.position).normalized);
            }  
            autopilotDeflection = tVelocityVec(new Vector3(0, 0, 1));
        }
        else if (state == AutopilotState.targetForward){
            autopilotDeflection = AutoTargetPlane('Z');
        }
        else{
            autopilotDeflection = new Vector3(0, 0, 0);
        }

        //PID errors
        //Debug.Log("PID Errors: " + pitchPID.lastError + ", " + rollPID.lastError + ", " + yawPID.lastError);

        return autopilotDeflection;
    }

}
