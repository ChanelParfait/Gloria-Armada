using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[System.Serializable]
class vec3PID {
    public PID x;
    public PID y;
    public PID z;

    public vec3PID(float p, float i, float d){
        x = new PID(p, i, d);
        y = new PID(p, i, d);
        z = new PID(p, i, d);
    }

    public Vector3 Solve(Vector3 target, Vector3 current){
        return new Vector3(x.Solve(target.x, current.x), y.Solve(target.y, current.y), z.Solve(target.z, current.z));
    }

    public Vector3 Solve(Vector3 error){
        return new Vector3(x.Solve(error.x), y.Solve(error.y), z.Solve(error.z));
    
    }
}

public class EnemyPlane : EnemyBase
{
    [SerializeField] private float fireInterval = 1;
    [SerializeField] private int speed = 8;
    public float referenceSpeed = 0;
    public Vector3 moveDir;
    public Vector3 orientation;

    [SerializeField] vec3PID pid = new vec3PID(1f, 0.01f, 22f);

    protected GameObject targetObj;
    protected Camera cam;
    [SerializeField] protected Vector3 targetOffset;
    protected float randomOffsetComponent;
    protected Vector3 targetPos;

    protected Perspective currentPerspective;

    protected float timer = 0;
    protected float radarTimer = 0;
    protected float randFireTime;
    public GameObject deathExplosion;


    [SerializeField] private PowerupManager powerupManager;
    
    protected CameraUtils camUtils;

    [SerializeField] protected GameObject deathObj;

    void OnEnable(){
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    void OnDisable(){
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    // Start is called before the first frame update
    protected override void Start()
    {   
        base.Start();
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        powerupManager = GameObject.FindObjectOfType<PowerupManager>();
        rb = GetComponent<Rigidbody>();
        GameObject lmObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (lmObj != null){
            LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            currentPerspective = lm.currentPerspective;
            if(targetObj == null){
                targetObj = GameObject.FindGameObjectWithTag("LevelManager");
            }
        }
        
        
        cam = Camera.main;
        camUtils = FindObjectOfType<CameraUtils>();
        randomOffsetComponent = Random.Range(-0.4f, 0.4f);
        randFireTime = Random.Range(1f, 2.0f);
        StartCoroutine(Initialize());
        timer = fireInterval - 1; 
        
    }

    virtual protected IEnumerator Initialize(){
        yield return new WaitForSeconds(0.1f);
        if (orientation == Vector3.zero && moveDir == Vector3.zero){
            yield break;
        }
        rb.AddForce(referenceSpeed * Utilities.MultiplyComponents(orientation, moveDir), ForceMode.VelocityChange);
    }

    void UpdatePerspective(int _pers){
        currentPerspective = (Perspective)_pers;
        if (rb){
            rb.MoveRotation(Quaternion.Euler(0,-90,0));
        }
    }

    protected virtual Vector3 GetTargetOffset(){
        switch (currentPerspective){
            case Perspective.Top_Down:
                return new Vector3(camUtils.height/2 - 30.0f, 0, camUtils.width/2 * randomOffsetComponent);
            case Perspective.Side_On:
                return new Vector3(camUtils.width/2 - 30.0f, camUtils.height/2 * randomOffsetComponent,0);
            case Perspective.Null:
                return Vector3.zero;
        }

        return targetObj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= fireInterval * randFireTime){
            randFireTime = Random.Range(0.5f, 2.0f);
            timer = 0; 
            Fire();
        }
    }

    protected virtual void FixedUpdate(){
        targetOffset = GetTargetOffset();
        Vector3 targetObjPos = targetObj.transform.position;
        targetPos = targetObjPos + targetOffset;
        if (rb.angularVelocity.magnitude > 0.1f){
            rb.useGravity = true;
        }
        else {
            MoveEnemy();
            radarTimer += Time.deltaTime;
            if (radarTimer > 0.5f){
                AvoidGround();
                radarTimer = 0;
            }
        }
    }

    public override void TakeDamage(float damage){
        base.TakeDamage(damage);
    }

    protected override void Die(){
        // Destroy Self and emit death explosion
        GameObject powerCrate = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        if (powerupManager != null){
            powerupManager.SpawnPowerUp(transform.position, rb.velocity);
        }
        if (deathObj != null)
        {
            GameObject deadObj = Instantiate(deathObj, transform.position, transform.rotation);
            foreach (Rigidbody rb in deadObj.GetComponentsInChildren<Rigidbody>())
            {
                //Add force to the rigid body
                rb.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                // Translate the angular velocity of the parent by the localPosition of the child to get the correct velocity
                Vector3 angularVelocity = GetComponent<Rigidbody>().angularVelocity;
                Vector3 pointOffset = rb.transform.localPosition;
                Vector3 linearVelocityAtPoint = Vector3.Cross(angularVelocity, pointOffset);
                rb.AddForce(linearVelocityAtPoint, ForceMode.VelocityChange);
            }
        }
        base.Die();
    }

    protected virtual void MoveEnemy(){
        Vector3 error = targetPos - transform.position;
        //Scale the error by the screen width
        Vector3 moveDir = pid.Solve(targetPos, transform.position);
        rb.AddForce(moveDir.normalized * speed * 20.0f);
    }

    void AvoidGround(){
        //Raycast down to check for ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f)){
            //If the distance to the ground is less than 10 units, add a force upwards
            if (hit.distance < 10.0f){
                rb.AddForce(Vector3.up * (10 - hit.distance) * 20.0f);
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            //Get the normal of the collision
            Vector3 normal = col.contacts[0].normal;
            //Get dot product of the normal and the velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            float dot = Vector3.Dot(rb.velocity.normalized, normal) * rb.velocity.magnitude;
            
            //Debug.Log(dot);

            dot = Mathf.Clamp01(dot);
            
            //Reduce health by a minimum of 1health, max of MaxLife based on dot
            int damage = (int)Mathf.Lerp(1,maxHealth, dot);

            TakeDamage(damage);
        }
    }

}
