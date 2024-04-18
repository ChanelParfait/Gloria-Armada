using System;
using UnityEngine;

[Serializable]
class AeroSurface {
    public float area;
    public Vector3 position;
    public Vector3 orientation;
}

[Serializable]
class Surfaces{
    public AeroSurface wing, tail, rudder, horizontalStabilizer, aileron;
}

public class Plane : MonoBehaviour
{

    //public KeyCode Up = KeyCode.W, Down = KeyCode.S, Left = KeyCode.A, Right = KeyCode.D, YawRight = KeyCode.E, YawLeft = KeyCode.Q;
    private KeyCode Fire = KeyCode.Space;
    public KeyCode throttleUp = KeyCode.Tab, throttleDown = KeyCode.LeftShift;

    [SerializeField] Vector3 controlDeflection = Vector3.zero;
    
    public float throttle = 0.7f;

    // Aircraft parameters
    [SerializeField] float liftPower = 2f;
    [SerializeField] float weight = 200;    // Weight of the aircraft (kg)
    [SerializeField] Surfaces surfaces;
    [SerializeField] float thrust = 1800;   // Maximum thrust (N)
    [SerializeField] float fireRate = 10f;
    float lastShotTime = 0f;    
    Vector3 controlInputs;

    public int health = 6;
    public bool isAlive = true;

    // Aircraft state
    [Space(10)]
    [Header("Aircraft State")]
    private float AoAYaw;
    public float AoA {get; private set;}
    public Vector3 internalVelocity;   // Velocity of the aircraft (not passed to RB) (m/s)
    public Vector3 localVelocity;      // Velocity of the aircraft from local (m/s)
    private Vector3 localAngularVelocity; // Angular velocity of the aircraft (rad/s)
    
    private Vector3 lastVelocity;
    private Vector3 lastRBAngularVelocity;

    public Vector3 acceleration; // Acceleration of the aircraft (m/s^2)
    public Vector3 angularAcceleration { get; private set; } // Angular acceleration of the aircraft (rad/s^2)

    [SerializeField] float cd = 25f; //0.2f
    [SerializeField] AnimationCurve cl = new AnimationCurve();

    // Unity
    [SerializeField] Rigidbody rb;
    [SerializeField] Autopilot ap;
    [SerializeField] Bullet gun;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem[] wingtipVortices;

    private void Awake()
    {
        rb.drag = float.Epsilon;
        rb.angularDrag = 0.2f ;
    }
 
    void Start()
    {
        // Set the center of mass
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;
        rb.velocity = transform.forward * 20;
        //targetObject = GameObject.Find("TargetPoint");   
        ap = GetComponent<Autopilot>();    
        foreach (Transform child in transform){
            if (child.name == "Smoke"){
                smoke = child.GetComponent<ParticleSystem>();
            }
        }
    }

    public Vector3 getRBVelocity(){
        return rb.velocity;
    }

    public void ApplyDamage(float _damage){
        if (health <= 0){
            return;
        } 
        else if (health - (int)_damage <= 5){
            health = 0;
            smoke.Play();
        }
        if (health - (int)_damage <= 0){
            health = 0;
            isAlive = false;
            smoke.Play();
            surfaces.wing.position += new Vector3(UnityEngine.Random.Range(-5f, 5f), surfaces.wing.position.y, UnityEngine.Random.Range(-1f, 1f));
            surfaces.tail.area *= UnityEngine.Random.Range(0f, 0.9f);
            ap.autopilotState = Autopilot.AutopilotState.targetFlat;
        }
        health -= (int)_damage;
    }
    
    void Shoot(){
        if (Time.time - lastShotTime < 1/fireRate){
            return;
        }
        Bullet bullet = Instantiate(gun, transform.position + transform.forward, transform.rotation);
        bullet.GetComponent<Bullet>().Fire(this);
        lastShotTime = Time.time;
    }


    #region physics
    void CalculateState(float dt){
        var InverseRotation = Quaternion.Inverse(transform.rotation);
        //internalVelocity = rb.velocity + new Vector3(60, 0, 0);
        internalVelocity = rb.velocity;
        localVelocity = InverseRotation * internalVelocity;
        localAngularVelocity = InverseRotation * rb.angularVelocity;
        
        acceleration = (internalVelocity - lastVelocity) / dt;
        angularAcceleration = (rb.angularVelocity - lastRBAngularVelocity) / dt;
        lastVelocity = internalVelocity;
        lastRBAngularVelocity = rb.angularVelocity;
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

    #endregion physics
    
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
    
    // Update is called once per frame
    void Update()
    {
        //Get key inputs -> these can be overridden by autopilot
        //Currently control inputs for all controls, we will simplify this later
        if (tag =="Player" && isAlive){
            controlInputs.y = Input.GetAxis("P1_Horizontal");
            controlInputs.x = Input.GetAxis("P1_Vertical");

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
            if (Input.GetKey(Fire) ){
            Shoot();
            }
        }


        if (ap.autopilotState != Autopilot.AutopilotState.Off && ap.HasTarget() ){
            if (ap.aimVector.magnitude < 0.08 && ap.rangeToTarget < 200){
                Shoot();
            }
        }
        else if (Input.GetKey(KeyCode.F)){
            ap.autopilotState = Autopilot.AutopilotState.Off;
        }

        controlInputs = ap.GetAPInput(controlInputs);
        //Draw debug line for velocity
        // Debug.DrawRay(transform.position, internalVelocity, Color.magenta);
        // Debug.DrawRay(transform.position, acceleration, Color.cyan);


        // if AoA > 10, add wingtip vortices
        if (AoA > 10){
            foreach (ParticleSystem vortex in wingtipVortices){
                vortex.Play();
            }
        }
        else{
            foreach (ParticleSystem vortex in wingtipVortices){
                vortex.Stop();
            }
        }
        
    }
}


