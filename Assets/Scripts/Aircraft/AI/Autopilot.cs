using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
class PID{
    public float Kp, Ki, Kd;
    public float integral, lastError;
    public float GetInt(){
        return integral;
    }
    public float GetLastError(){
        return lastError;
    }
    public void SetInt(float i){
        integral = i;
    }
    public void SetLastError(float e){
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
    
    public enum AutopilotState {Off, pointAt,  targetFlat, vectorAt, targetStraight, targetFormation};
    [Header("AP State")]
    public AutopilotState autopilotState = AutopilotState.Off;
    [SerializeField] AutopilotState lastAutopilotState = AutopilotState.Off;
    [SerializeField] private Rigidbody rb;
    Plane p;
    
    [SerializeField] bool autoArm = true;

    public bool InFormation {get; private set;}

    [Space(10)]
    [Header("PID Controllers")]
    [SerializeField] PID pitchPID = new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID yawPID =   new PID(1.0f, 0.01f, 0.2f);
    [SerializeField] PID rollPID =  new PID(1.0f, 0.01f, 0.2f);

    PID throttlePID = new PID(0.1f, 0.001f, 0.1f);

    PID[] mainPIDs;

    PID aPitchPID = new PID(1.0f, 0.01f, 0.2f);
    PID aYawPID =   new PID(1.0f, 0.01f, 0.2f);
    PID aRollPID =  new PID(1.0f, 0.01f, 0.2f);

    

    PID[] autoPIDs;

    PID sPitchPID = new PID(1.0f, 0.01f, 0.2f);
    PID sYawPID =   new PID(1.0f, 0.01f, 0.2f);
    PID sRollPID =  new PID(1.0f, 0.01f, 0.2f);

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
    public bool onAxes = false;
    public float shotSpeed = 300f;

    public float zTarget = 0;
    public float targetAngularSize;

    [SerializeField] Perspective pers = Perspective.Null;

    enum Lead {pure, lead, lag, leadShot, lagShot, intercept};

    private void OnEnable() {
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    private void OnDisable() {
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    void UpdatePerspective(int _pers)
    {
        pers = (Perspective)_pers;
        rb.constraints = RigidbodyConstraints.None;
        lastAutopilotState = autopilotState;
        autopilotState = AutopilotState.targetFlat;
        onAxes = false;
    }

    public void setTargetObject(GameObject target){
        targetObject = target;
    }

    public void setAPState(AutopilotState apState){
        autopilotState = apState;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("LevelManager") != null){
            LevelManager lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            pers = lm.currentPerspective;
        }
        if (rb == null){
            rb = GetComponent<Rigidbody>();
        }
        mainPIDs = new PID[] {pitchPID, rollPID, yawPID};
        autoPIDs = new PID[] {aPitchPID, aRollPID, aYawPID};
        stabilityPIDs = new PID[] {sPitchPID, sRollPID, sYawPID};
        p = GetComponent<Plane>(); 

        //based on the tag
        if (CompareTag("Player"))
        {
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
        //Guard against null object, or object not a plane
        if (targetObject == null){
            if (targetDict.Count > 0){
                //targetObject = targetDict.FirstOrDefault().Value;
                return false;
            }
            else{
                if (autopilotState != AutopilotState.Off || autopilotState != AutopilotState.targetStraight || autopilotState != AutopilotState.targetFormation){
                    lastAutopilotState = autopilotState;
                    autopilotState = AutopilotState.targetFlat;
                    return false;
                }
                else{
                    return false;
                }
            }
        }
        else if (targetObject.GetComponent<Plane>() == null){
            targetLocation = targetObject.transform.position;
            return hasTarget = false;
        }

        //If target is dead, remove it from the list and try to find a new target
        else if (!targetObject.GetComponent<Plane>().isAlive){
            targetDict.Remove(targetObject.name);
            if (targetDict.Count > 0){
                targetObject = targetDict.FirstOrDefault().Value;
            }
            else{
                targetObject = null;
                autopilotState = autopilotState == AutopilotState.Off ? AutopilotState.Off : AutopilotState.targetFlat; 
                return false;                     
            }   
        }
        else
        {
            rangeToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
            targetAngularSize = Mathf.Atan(2 / rangeToTarget) * Mathf.Rad2Deg;
            return hasTarget = true;
        }
        return false;

    }

    public Vector4 GetAPInput(Vector4 _controlInputs){
        controlInputs = _controlInputs;
        Vector3 apInputs = AutopilotControl(autopilotState);
        if (CompareTag("Player"))
        {
            //
            if (pers == Perspective.Top_Down && onAxes){
                // controlInputs.y *= RestrictRoll();    
                // controlInputs.x += AutoTurn()/2;
                // controlInputs.y += Upright();
                // Mathf.Clamp(controlInputs.y, -1, 1);  

                //apInputs = Utilities.ClampVec3(apInputs + controlInputs, -1, 1);
            }
               
        }
        return apInputs;
    }

    Vector3 AutopilotControl(AutopilotState state){
        
        autopilotDeflection = Vector3.zero;
        if (state == AutopilotState.Off){
            return controlInputs;
        }

        if (HasTarget()){
            targetLocation = CalcLead(targetObject.GetComponent<Plane>(), Lead.intercept);
            aimVector = CalcAimVector(targetObject.GetComponent<Plane>());
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
            autopilotDeflection = AutoTargetPlane();
        }
        else if (state == AutopilotState.targetStraight){
            autopilotDeflection = AutoStraight();
        }
        else if (state == AutopilotState.targetFormation){
            autopilotDeflection = FormationWith(targetObject, mainPIDs);
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

    Vector2 CalcAimVector(Plane e){
        Vector3 targetPoint = CalcLead(e, Lead.leadShot);
        Vector3 targetDirection = (targetPoint - rb.transform.position).normalized;

        float anglePitch = Vector3.SignedAngle(transform.forward, targetDirection, transform.right) / 180f;
        float angleYaw = Vector3.SignedAngle(transform.forward, targetDirection, transform.up)      / 180f;

        return new Vector2(anglePitch, angleYaw);
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
        
        Debug.Log("Lead type not found on AP.CalcLead(), object: " + e.name + ", lead: " + lead + ", returning 0,0,0");
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

        Debug.DrawRay(transform.position, targetDirection * 20, Color.yellow);

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

        Debug.DrawRay(transform.position, velocityError, Color.yellow);

        float x = Mathf.Clamp(PIDSolve(anglePitch,  ref pids[0]), -1, 1);
        float y = Mathf.Clamp(PIDSolve(angleRoll,   ref pids[1]), -1, 1);
        float z = Mathf.Clamp(PIDSolve(angleYaw,    ref pids[2]), -1, 1);
        return new Vector3(x, y, z);
    }

    Vector3 FormationWith(GameObject targetObject, PID[] pids = null){
        Vector3 targetOffset = new Vector3(5, -10, 0);
        if (targetObject == null){
            targetObject = GameObject.Find("LevelManager").gameObject;   
        }
       Vector3 targetPosition = targetObject.transform.position + targetOffset;
        
        Vector3 targetVelocity = targetObject.GetComponent<Rigidbody>().velocity;

        float throttle;    
        Vector3 positionalError = targetPosition - rb.transform.position;
        Debug.DrawRay(rb.transform.position, positionalError, Color.magenta);
        float dot = Vector3.Dot(positionalError.normalized, targetVelocity.normalized);
        Vector3 projectedPosition = targetPosition + Vector3.Project(-positionalError, targetVelocity);
        Debug.DrawLine(targetPosition, projectedPosition, Color.green);
        float distanceBehindTarget = (projectedPosition -  rb.transform.position).magnitude * Mathf.Sign(dot);

        //Debug.Log("Behind by: " + distanceBehindTarget + "m");
        
        throttle = Mathf.Clamp01(PIDSolve(distanceBehindTarget, ref throttlePID));
        if (distanceBehindTarget < 10){
            Vector3 tempTarget = targetPosition + 10 * targetVelocity.normalized;
            if (distanceBehindTarget < 0){
                tempTarget = projectedPosition + 10 * targetVelocity.normalized;
            }
            Debug.DrawLine(rb.transform.position, tempTarget, Color.red);
            targetPosition = tempTarget;
        }

        if (Mathf.Abs(distanceBehindTarget) < 5){
            InFormation = true;
        }
    
        Vector3 targetDirection = (targetPosition - rb.transform.position).normalized;
        Vector3 desiredVelocity = targetDirection;
        Vector3 curVelocity = p.internalVelocity;
        Vector3 velocityError = desiredVelocity - curVelocity.normalized;
        

        //Get signed angles for angle up, roll, right
        float anglePitch       = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.right) / 180f;
        float angleLiftVUp    = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward)       / 180f;
        float angleAlignLiftV = -Vector3.SignedAngle(rb.transform.up, velocityError, rb.transform.forward)    / 180f;
        float angleYaw         = Vector3.SignedAngle(rb.transform.forward, velocityError, rb.transform.up)    / 180f;

        // Multiply signed angle by the magnitude of the error projected in that direction
        anglePitch *= Vector3.Cross(velocityError, rb.transform.right).magnitude;
        angleAlignLiftV *= Vector3.Cross(velocityError, rb.transform.forward).magnitude;
        angleYaw *= Vector3.Cross(velocityError, rb.transform.up).magnitude;
    
        float angleRoll = Mathf.Lerp(angleLiftVUp, angleAlignLiftV, Mathf.Clamp01(velocityError.magnitude));

        Debug.DrawRay(transform.position, velocityError, Color.yellow);

        float x = Mathf.Clamp(PIDSolve(anglePitch,  ref pids[0]), -1, 1);
        float y = Mathf.Clamp(PIDSolve(angleRoll,   ref pids[1]), -1, 1);
        float z = Mathf.Clamp(PIDSolve(angleYaw,    ref pids[2]), -1, 1);
        p.SetThrottle(throttle);
        return new Vector4(x, y, z);
    }

    #region Assists
    float Upright(){
        // Get the angle between the aircraft's up and the world up
        float angle = -Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward) / 180f;
        return PIDSolve(angle, ref aRollPID);
    }

    float RestrictRoll(){
        // only allow the player to roll up to 90 degrees left/right
        float angle = Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward);
        if (Mathf.Abs(angle) > 85){
            //Debug.Log("Angle: " + angle);
            return 0;
        }
        return 1;
    }

    float AutoTurn(){
        float angle = Vector3.SignedAngle(rb.transform.up, Vector3.up, rb.transform.forward) / 180f;
        return -Mathf.Abs(angle);
    }

    void AvoidGround(){
        // Cast a ray in the velocity direction
        if (Physics.Raycast(transform.position, rb.velocity, out RaycastHit hit, 1000))
        {
            // If the ray hits something with an rb
            //Debug.DrawRay(transform.position, rb.velocity * 1000, Color.red);
            Vector3 normal = hit.normal;
            // Get the angle between the velocity and the normal

            //Get the time til we hit the ground
            float timeToHit = hit.distance / rb.velocity.magnitude;

            //rollUpright 
            float roll = Upright();
            float pitch = -0.7f;
            float t = Mathf.Clamp01((timeToHit - 6) * -1 / 5);
            //Debug.Log(t + ": pitch: " + pitch + ", roll: " + roll + ", timeToHit: " + timeToHit + ", distance: " + hit.distance);
            //LERP between auto inputs and ground avoidance based on timeToHit < 3seconds -> 0 seconds
            autopilotDeflection = new Vector3(Mathf.Lerp(controlInputs.x, pitch, t), Mathf.Lerp(controlInputs.y, roll, t), controlInputs.z);
        }
    }
    #endregion Assists

    public void setZTarget(float controlInputLR){
        zTarget = -controlInputLR;
    }

    Vector3 AutoStraight(){
        // Go set a vector that is straight ahead
        return VectorAt(rb.transform.forward * 1000, mainPIDs, VectorType.direction);
    }

    // Target a 2D plane based on LevelManager perspective
    Vector3 AutoTargetPlane(bool positional = false, float holdAltitude = float.MaxValue){
        // If we are not already on the plane then go to it
        float x = rb.transform.position.x;
        float y = rb.transform.position.y;
        if (holdAltitude != float.MaxValue){
            y = holdAltitude - y;
        }
        float z = rb.transform.position.z;
        float signX = Mathf.Sign(x);
        float signY = Mathf.Sign(y);
        float signZ = Mathf.Sign(z);
        float smoothing = 200.0f;

        // Set zAngle between 0 and 90 degrees
        float zAngle = 60;
        // Multiply by Tan(45 dgrees) * smoothing to get the distance to the plane
        zTarget = Mathf.Tan(zAngle * Mathf.Deg2Rad * zTarget) * smoothing;

        float ty = Mathf.Min(signY * -y * y, smoothing);
        float tz = Mathf.Min(signZ * -z * z, smoothing); // By default -> got to the center of the screen
        if (pers == Perspective.Side_On){ ty = onAxes ? 0 : ty;}
        if (pers == Perspective.Top_Down){tz = onAxes ? zTarget : tz;} // In top-down, go to point defined by player input
        
        Vector3 targetVector = new Vector3(smoothing, ty, tz); //As a direction
        if (positional){
            targetVector = new Vector3(60, 0, 0); //As a position
        }

        if (pers == Perspective.Top_Down && !onAxes && Mathf.Abs(y) < 2 && (rb.velocity.normalized - Vector3.right).magnitude < 1f){
            transform.position.Set(x, 0, z);
            onAxes = true;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            autopilotState = lastAutopilotState;
        }
        // if rb.velocity is CLOSE to right, and z is CLOSE to 0, and we are not on the axes
        else if (pers == Perspective.Side_On && !onAxes && Mathf.Abs(z) < 2 && (rb.velocity.normalized - Vector3.right).magnitude < 0.1f){
            //force the plane to the xz plane
            transform.position.Set(x, y, 0);   
            transform.rotation = Quaternion.Euler(0, 90, 0);
            onAxes = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY  | RigidbodyConstraints.FreezePositionZ;
            autopilotState = lastAutopilotState;
            
        }
        else if (pers == Perspective.Null){
            onAxes = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        Vector3 vecToPlane =  PointAt(targetVector + transform.position, mainPIDs);
        if (tz == 0){
            vecToPlane.z += Upright();
        }
        if (onAxes){
            Utilities.MultiplyComponents(vecToPlane, new Vector3(1, 1, 0.3f));
        }
        return vecToPlane;
    }


    // void OnDrawGizmos(){
    //     Color[] colors = {Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black};
    //     if (HasTarget() && tag != "Player"){
    //         foreach (Lead lead in Enum.GetValues(typeof(Lead))){
    //             //assign a color to each lead type
    //             Gizmos.color = colors[(int)lead];
    //             Gizmos.DrawSphere(CalcLead(targetObject.GetComponent<Plane>(), lead), 1.0f);
    //         }
    //     }
    // }

}
