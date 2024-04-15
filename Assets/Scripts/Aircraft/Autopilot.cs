using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using TMPro.EditorUtilities;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Unity.VisualScripting;


[Serializable]
class PID{
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
    
    public enum AutopilotState {Off = 0, pointAt = 1, vectorAt = 3, targetFlat = 2};
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

    PID[] mainPIDs;

    [SerializeField] PID aPitchPID = new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID aYawPID =   new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID aRollPID =  new PID(1.0f, 0.01f, 0.2f);

    PID[] autoPIDs;

    [SerializeField] PID sPitchPID = new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID sYawPID =   new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID sRollPID =  new PID(1.0f, 0.01f, 0.2f);

    PID[] stabilityPIDs;
    enum VectorType{
        position,
        direction
    }


    [Space(10)]
    [Header("Targets")]
    [SerializeField] GameObject[] targetObjects;

    Dictionary<string, GameObject> targetDict = new Dictionary<string, GameObject>();

    [Space(10)] 
    [Header("Target")]
    [SerializeField] GameObject targetObject;
    public Vector3 targetLocation = new Vector3(0, 0, 0);
    public Vector2 aimVector = new Vector3(0, 0);
    public float rangeToTarget = float.MaxValue;

    public bool hasTarget = false;

    public float shotSpeed = 300f;
    public float targetAngularSize;

    enum Lead {pure, lead, lag, leadShot, lagShot, intercept};



    // Start is called before the first frame update
    void Start()
    {
        mainPIDs = new PID[] {pitchPID, yawPID, rollPID};
        autoPIDs = new PID[] {aPitchPID, aYawPID, aRollPID};
        stabilityPIDs = new PID[] {sPitchPID, sYawPID, sRollPID};
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
        if (targetObject??false && p.isAlive){
            if (targetObject.GetComponent<Plane>() != null){
                if (!targetObject.GetComponent<Plane>().isAlive){
                    targetDict.Remove(targetObject.name);
                    if (targetDict.Count > 0){
                        targetObject = targetDict.FirstOrDefault().Value;
                    }
                    else{
                        targetObject = null;
                        autopilotState = AutopilotState.targetFlat;                      
                    }   
                }
                rangeToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                targetAngularSize = Mathf.Atan(2 / rangeToTarget) * Mathf.Rad2Deg;
                return hasTarget = true;
            }
        }
        aimVector = new Vector3(0, 0, 0);
        rangeToTarget = float.MaxValue;
        return hasTarget = false;
    }

    public Vector3 GetAPInput(Vector3 _controlInputs){
        controlInputs = _controlInputs;
        Vector3 apInputs = AutopilotControl(autopilotState);
        Vector3 blendedInputs = Utilities.ClampVec3(controlInputs + apInputs, -1, 1);
        return blendedInputs;
    }

    Vector3 AutopilotControl(AutopilotState state){
        
        autopilotDeflection = controlInputs;
        if (state == AutopilotState.Off){
            return controlInputs;
        }

        if (HasTarget()){
            targetLocation = CalcLead(targetObject.GetComponent<Plane>(), Lead.leadShot);
        }
        
        if (state == AutopilotState.pointAt){
            autopilotDeflection = PointAt(targetLocation, mainPIDs);
        }
        else if (state == AutopilotState.vectorAt){
            if (HasTarget()){
                autopilotDeflection = VectorAt(targetObject.transform.position, mainPIDs, VectorType.position);
            }  
            autopilotDeflection = VectorAt(targetLocation, mainPIDs, VectorType.direction);
        }
        else if (state == AutopilotState.targetFlat){
            autopilotDeflection = AutoTargetPlane('Y');
        }
        else{
            autopilotDeflection = new Vector3(0, 0, 0);
        }

        if (autoArm){
            AvoidGround();
        }
        return autopilotDeflection;
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

    Vector3 CalcLead(Plane e, Lead lead){
        Vector3 firstOrder = Utilities.FirstOrderIntercept(rb.transform.position, rb.velocity, shotSpeed, e.transform.position, e.getRBVelocity());
        Vector3 secondOrder = Utilities.SecondOrderIntercept(rb.transform.position, rb.velocity, shotSpeed, e.transform.position, e.getRBVelocity(), e.acceleration, 0);

        Vector3 firstOrderOffset = firstOrder - e.transform.position;
        Vector3 secondOrderOffset = secondOrder - e.transform.position;

        Vector3 pos = e.transform.position;
        if (lead == Lead.pure){
            return pos;
        }
        if (lead == Lead.lead){
            return firstOrder;
        }
        else if (lead == Lead.lag){
            return e.transform.position - firstOrderOffset;
        }
        else if (lead == Lead.leadShot){
            return  secondOrder;
        }
        else if (lead == Lead.lagShot){
            return e.transform.position - secondOrderOffset;
        }
        else if (lead == Lead.intercept){
            return Utilities.SecondOrderIntercept(rb.transform.position, rb.velocity, rb.velocity.magnitude, e.transform.position, e.getRBVelocity(), e.acceleration, 0);
        }
        
        return new Vector3(0, 0, 0);
    }
    Vector3 PointAt(Vector3 targetPoint, PID[] pids = null){
        Vector3 targetDirection = (targetPoint - rb.transform.position).normalized;
        float anglePitch = Vector3.SignedAngle(transform.forward, targetDirection, transform.right) / 180f;
        float angleRoll = -Vector3.SignedAngle(transform.up, targetDirection, transform.forward)    / 180f;
        float angleYaw = Vector3.SignedAngle(transform.forward, targetDirection, transform.up)      / 180f;
        
        // Multiply signed angle by the magnitude of the error projected in that direction
        anglePitch *= Vector3.Cross(targetDirection, rb.transform.right).magnitude;
        angleRoll *= Vector3.Cross(targetDirection, rb.transform.forward).magnitude;
        angleYaw *= Vector3.Cross(targetDirection, rb.transform.up).magnitude;

        float autoXInput = Mathf.Clamp(PIDSolve(anglePitch, ref pids[0]), -1, 1);
        float autoYInput = Mathf.Clamp(PIDSolve(angleRoll, ref pids[1]), -1.0f, 1.0f);
        float autoZInput = Mathf.Clamp(PIDSolve(angleYaw, ref pids[2]), -1.0f, 1.0f);

        return new Vector3(autoXInput,autoYInput, autoZInput);
    }

    Vector3 VectorAt(Vector3 targetPoint, PID[] pids = null, VectorType type = VectorType.direction){
        
        Vector3 desiredVelocity = targetPoint.normalized;
        if (type == VectorType.position){
            desiredVelocity = (targetPoint - rb.transform.position).normalized;
        };

        Vector3 curVelocity = p.internalVelocity;
        Vector3 velocityError = desiredVelocity - curVelocity.normalized;

        //Get signed angles for angle up, roll, right
        float anglePitch       = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.right) / 180f;
        float angleLiftVUp    = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward)       / 180f;
        float angleAlignLiftV = -Vector3.SignedAngle(rb.transform.up, velocityError, rb.transform.forward)    / 180f;
        float angleYaw         = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.up)    / 180f;

        aimVector = new Vector2(anglePitch, angleYaw);

        // Multiply signed angle by the magnitude of the error projected in that direction
        anglePitch *= Vector3.Cross(velocityError, rb.transform.right).magnitude;
        angleAlignLiftV *= Vector3.Cross(velocityError, rb.transform.forward).magnitude;
        angleYaw *= Vector3.Cross(velocityError, rb.transform.up).magnitude;
    
        float angleRoll = Mathf.Lerp(angleLiftVUp, angleAlignLiftV, Mathf.Clamp01(velocityError.magnitude));

        float x = Mathf.Clamp(PIDSolve(anglePitch,  ref pids[0]), -1, 1);
        float y = Mathf.Clamp(PIDSolve(angleRoll,   ref pids[1]), -1, 1);
        float z = Mathf.Clamp(PIDSolve(angleYaw,    ref pids[2]), -1, 1);
        return new Vector3(x, y, z);
    }

    #region Assists
    float Upright(){
        // Get the angle between the aircraft's up and the world up
        float angle = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward) / 180f;
        return PIDSolve(angle, ref aRollPID);
    }

    void AvoidGround(){
        // Cast a ray in the velocity direction
        if (Physics.Raycast(transform.position, rb.velocity, out RaycastHit hit, 1000))
        {
            // If the ray hits something with an rb
            Debug.DrawRay(transform.position, rb.velocity * 1000, Color.red);
            Vector3 normal = hit.normal;
            // Get the angle between the velocity and the normal

            //Get the time til we hit the ground
            float timeToHit = hit.distance / rb.velocity.magnitude;

            //rollUpright 
            float roll = Upright();
            float pitch = -0.7f;
            float t = Mathf.Clamp01((timeToHit - 6) * -1 / 5);
            Debug.Log(t + ": pitch: " + pitch + ", roll: " + roll + ", timeToHit: " + timeToHit + ", distance: " + hit.distance);
            //LERP between auto inputs and ground avoidance based on timeToHit < 3seconds -> 0 seconds
            autopilotDeflection = new Vector3(Mathf.Lerp(controlInputs.x, pitch, t), Mathf.Lerp(controlInputs.y, roll, t), controlInputs.z);
        }
    }

    float AutoTargetStabilize(){
        //Attempt to smooth AoA and restrict to < 24 degrees
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
    #endregion Assists

    // Target a 2D plane
    Vector3 AutoTargetPlane(char plane = 'Y'){
        // If we are not already on the plane then go to it
        float x = rb.transform.position.x;
        float y = rb.transform.position.y;
        float z = rb.transform.position.z;
        float signX = Mathf.Sign(x);
        float signY = Mathf.Sign(y);
        float smoothing = 200.0f;

        Vector3 targetVector = new Vector3(Mathf.Min(signX * -x * x, smoothing), y, smoothing).normalized;
        Vector3 vecToPlane =  VectorAt(targetVector, mainPIDs, VectorType.direction);
        return vecToPlane;
    }

    void OnDrawGizmos(){
        Color[] colors = {Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black};
        if (HasTarget() && tag == "Player"){
            foreach (Lead lead in Enum.GetValues(typeof(Lead))){
                //assign a color to each lead type
                Gizmos.color = colors[(int)lead];
                Gizmos.DrawSphere(CalcLead(targetObject.GetComponent<Plane>(), lead), 1.0f);
            }
        }
    }

}
