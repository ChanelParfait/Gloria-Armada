using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AeroSurface {
    public float area;
    public Vector3 position;
    public Vector3 orientation;
}

[Serializable]
public class Surfaces{
    public AeroSurface wing, tail, rudder, horizontalStabilizer, aileron;
}

public struct Controls{
    public float pitch, roll, yaw, throttle;
}



public class Plane : MonoBehaviour
{

    //public KeyCode Up = KeyCode.W, Down = KeyCode.S, Left = KeyCode.A, Right = KeyCode.D, YawRight = KeyCode.E, YawLeft = KeyCode.Q;
    //private KeyCode Fire = KeyCode.Space;
    public KeyCode throttleUp = KeyCode.Tab, throttleDown = KeyCode.LeftShift;

    [SerializeField] Vector3 controlDeflection = Vector3.zero;
    
    public float throttle = 0.7f;

    public float spawnSpeed = 50f;

    // Aircraft parameters
    public float liftPower = 2f;
    public float weight = 200;    // Weight of the aircraft (kg)
    public Surfaces surfaces;
    public float thrust = 1800;   // Maximum thrust (N)

    public bool thrustVectoring = false;
    Vector3 controlInputs;
    Vector3 blendedInputs;
    Vector3 apInputs;
    Perspective pers;
    LevelManager lm;
    public int health = 6;
    public bool isAlive = true;

    // Aircraft state
    [Space(10)]
    [Header("Aircraft State")]
    private float AoAYaw;
    public float AoA {get; private set;}

    public float scaleVelocity = 1.0f;
    public Vector3 internalVelocity;   // Velocity of the aircraft (not passed to RB) (m/s)
    public Vector3 localVelocity;      // Velocity of the aircraft from local (m/s)
    
    private Vector3 lastVelocity;
    private Vector3 lastRBAngularVelocity;

    public Vector3 acceleration; // Acceleration of the aircraft (m/s^2)
    public Vector3 angularAcceleration { get; private set; } // Angular acceleration of the aircraft (rad/s^2)
    public Vector3 localAngularVelocity { get; private set; } // Angular velocity of the aircraft from local (rad/s)


    [Flags]
    public enum ControlChannels
    {
        None = 0,
        Horizontal = 1 << 0,  // 1
        Vertical = 1 << 1,    // 2
        Throttle = 1 << 2,   // 4
    }

    [SerializeField] ControlChannels enabledControls;

    public float cd = 25f; //0.2f
    [SerializeField] AnimationCurve cl = new AnimationCurve();

    [SerializeField] AnimationCurve pitchCurve = new AnimationCurve();

    // Unity
    [SerializeField] Rigidbody rb;
    [SerializeField] Autopilot ap;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem fire;

    [SerializeField] ParticleSystem boost;
    [SerializeField] ParticleSystem[] wingtipVortices;

    public bool isOutOfBounds = false;

    void OnEnable(){
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    void OnDisable(){
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    void UpdatePerspective(int p){
        pers = (Perspective)p;
    }

    private void Awake()
    {
        rb.drag = float.Epsilon;
        rb.angularDrag = 0.21f ;
    }

    public void SetThrottle(float _throttle){
        throttle = _throttle;
        if (boost != null){
            if (throttle == 1){
                boost.Play();
            }
            else {
                boost.Stop();
            }
        }
    }

    public void SetControlsEnabled(ControlChannels _enabled){
        enabledControls = _enabled;
    }
 
    void Start()
    {
        // Set the center of mass
        rb = GetComponent<Rigidbody>();
        rb.mass = weight;
        rb.velocity = transform.forward * spawnSpeed;
        //targetObject = GameObject.Find("TargetPoint");   
        ap = GetComponent<Autopilot>();    
        foreach (Transform child in transform){
            if (child.name == "Smoke"){
                smoke = child.GetComponent<ParticleSystem>();
            }
        }
        if (GameObject.Find("LevelManager") != null){
            lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            pers = lm.currentPerspective;
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
            smoke.Play();   
        }
        if (health - (int)_damage <= 0){
            health = 0;
            isAlive = false;
            fire.Play();
            rb.constraints = RigidbodyConstraints.None;
            thrust = 0;
            surfaces.wing.position += new Vector3(UnityEngine.Random.Range(-5f, 5f), surfaces.wing.position.y, UnityEngine.Random.Range(-1f, 1f));
            surfaces.tail.area *= UnityEngine.Random.Range(0f, 0.9f);
            surfaces.horizontalStabilizer.position += new Vector3(UnityEngine.Random.Range(-3f, 3f), surfaces.horizontalStabilizer.position.y, UnityEngine.Random.Range(-3f, 3f));
            ap.autopilotState = Autopilot.AutopilotState.targetFlat;
            
        }
        health -= (int)_damage;
    }
    
    void Shoot(){
        // if (Time.time - lastShotTime < 1/fireRate){
        //     return;
        // }
        // Bullet bullet = Instantiate(gun, transform.position + transform.forward, transform.rotation);
        // bullet.GetComponent<Bullet>().Fire(this);
        // lastShotTime = Time.time;
    }


    #region physics
    void CalculateState(float dt){
        var InverseRotation = Quaternion.Inverse(transform.rotation);
        //internalVelocity = rb.velocity + new Vector3(60, 0, 0);
        internalVelocity = rb.velocity * scaleVelocity;
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
        if (!thrustVectoring){
            rb.AddRelativeForce(throttle * thrust * Vector3.forward);
        }
        else{
            // Angle the thrust vector by the control inputs
            Vector3 thrustVector = transform.forward;
            thrustVector = Quaternion.AngleAxis(-controlDeflection.x, transform.right) * thrustVector;
            thrustVector = Quaternion.AngleAxis(controlDeflection.y, transform.up) * thrustVector;
            Vector3 thrustForce = throttle * thrust * thrustVector;
            rb.AddRelativeForce(throttle * thrust * Vector3.forward);
            // Add torque assuming engine is behind center of mass
            rb.AddRelativeTorque(Vector3.Cross(new Vector3(0, 0, -4.5f), thrustForce));
         }
            
    }

    void UpdateDrag(){
        //Note: cd is not currently scaled by plane orientation
        float drag = cd * localVelocity.sqrMagnitude;
        rb.AddRelativeForce(-localVelocity.normalized * drag);
    }

    void UpdateAngularDrag(){
        //Resist rotation around Z axis
        rb.AddTorque(-transform.InverseTransformDirection(rb.angularVelocity).z* 1500f * transform.forward);
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
        //rb.AddRelativeForce(ailForceR + ailForceL);
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

    void ApplyOutOfBoundsForce(){
        //Get a reference to the boundary gameObject
        GameObject playArea = GameObject.Find("PlayArea");
        if (playArea == null){
            return;
        }
        //Get the box collider of the boundary
        BoxCollider boxCollider = playArea.GetComponent<BoxCollider>();
        // Get the proximity of the player to the edge of the boxCollider
        float strength = 1;
        Vector3 proxVec = boxCollider.ClosestPoint(transform.position) - transform.position;
        Vector3 force = 2/(Mathf.Pow(20*(1/strength)-proxVec.magnitude, 2)+ 0.1f) * proxVec.normalized + proxVec;
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    #endregion physics
    
    void FixedUpdate(){
        float dt = Time.fixedDeltaTime;


        apInputs = ap.GetAPInput(controlInputs);
        blendedInputs = Utilities.ClampVec3(controlInputs + apInputs, -1, 1);

        if (pers == Perspective.Top_Down){
            ap.setZTarget(controlInputs.y);
            //controlDeflection = new Vector3(-pitchCurve.Evaluate(-apInputs.x) * 40f - 0.0f, apInputs.y * 20f, apInputs.z * 20f);
            controlDeflection = new Vector3(-pitchCurve.Evaluate(-apInputs.x) * 40f - 0.0f, apInputs.y * 20f, apInputs.z * 20f);
        }
        else {
            controlDeflection = new Vector3(-pitchCurve.Evaluate(-blendedInputs.x) * 40f - 0.0f, blendedInputs.y * 20f, blendedInputs.z * 20f);
        }

        if (isOutOfBounds){
            ApplyOutOfBoundsForce();
        }
        
        CalculateState(dt);
        CalculateAoA();

        UpdateThrust();
        UpdateDrag();
        UpdateLift();
        UpdateAngularDrag();

    }

    bool IsChannelEnabled(ControlChannels channel){
        return (enabledControls & channel) == channel;
    }

        public void EnableChannel(ControlChannels channel)
    {
        enabledControls |= channel;
    }

    public void DisableChannel(ControlChannels channel)
    {
        enabledControls &= ~channel;
    }

    public void EnableAllChannels()
    {
        enabledControls = ControlChannels.Horizontal | ControlChannels.Vertical | ControlChannels.Throttle;
    }

    public void DisableAllChannels()
    {
        enabledControls = ControlChannels.None;
    }
    
    // Update is called once per frame
    void Update()
    {
        //Get key inputs -> these can be overridden by autopilot
        //Currently control inputs for all controls, we will simplify this later
        if (tag =="Player" && isAlive){
            controlInputs.y = IsChannelEnabled(ControlChannels.Horizontal) ?  Input.GetAxis("P1_Horizontal") : 0;
            controlInputs.x = IsChannelEnabled(ControlChannels.Vertical) ?  Input.GetAxis("P1_Vertical") : 0;
            
            if (PlayerPrefs.HasKey("InvertPitch")){
                if (PlayerPrefs.GetInt("InvertPitch") == 1){
                    controlInputs.x *= -1;
                }
            }

            if (IsChannelEnabled(ControlChannels.Throttle)){
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
                    throttle = 0.2f;
                }
            }

        }

        if (boost != null){
            if (throttle == 1.0f && !boost.isPlaying){
                boost.Play();
            }
            else if (throttle != 1.0f && boost.isPlaying){
                boost.Stop();
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


        // if AoA > 10, add wingtip vortices
        if (AoA > 10 && wingtipVortices.Length > 0){
            foreach (ParticleSystem vortex in wingtipVortices){
                if (vortex != null){
                    vortex.Play();
                }        
            }
        }
        else{
            foreach (ParticleSystem vortex in wingtipVortices){
                if (vortex != null){
                    vortex.Stop();
                }
            }
        }
        
    }
}


