using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

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

    vec3PID pid = new vec3PID(1f, 0.01f, 6f);

    GameObject targetObj;
    Camera cam;
    [SerializeField] Vector3 targetOffset;
    float randomOffsetComponent;
    Vector3 targetPos;

    Rigidbody rb;
    private float timer = 0;
    public GameObject deathExplosion;

    [SerializeField] private GameObject deathObj;

    // Start is called before the first frame update
    protected override void Start()
    {   
        base.Start();
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        if(targetObj == null){
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }
        randomOffsetComponent = Random.Range(-0.4f, 0.4f);
        StartCoroutine(Initialize());
        
    }

    IEnumerator Initialize(){
        yield return new WaitForSeconds(0.1f);
        rb.velocity = referenceSpeed * Utilities.MultiplyComponents(orientation, moveDir);
    }

    Vector2 GetCameraDimensions(){
        //With a perspective cam at -250 units on the z axis, find the bounds of the camera view using the camera's FOV
        float height = 2.0f * Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        float width = height * cam.aspect;  

        return new Vector2(width, height);
    }

    // Update is called once per frame
    void Update()
    {

        if(targetObj == null){
            targetObj = GameObject.FindGameObjectWithTag("LevelManager");
        }
        targetOffset = new Vector3(GetCameraDimensions().x/2 - 10.0f, GetCameraDimensions().y/2 * randomOffsetComponent, 0);
        targetPos = targetObj.transform.position + targetOffset;
        MoveEnemy();


        timer += Time.deltaTime; 
        if(timer >= Random.Range(fireInterval, 3f)){
            timer = 0; 
            Fire();
        }
    }

    private void OnTriggerEnter(Collider col){
        // if hit by a player projectile
        if(col.gameObject.tag == "PlayerProjectile"){
            // Take Damage
            TakeDamage(col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            //Debug.Log("Enemy Health:" + currentHealth);
            //Debug.Log("Enemy Damage Taken:" + col.gameObject.GetComponent<Projectile>().projectileStats.damage);
            // Destroy Projectile
            Destroy(col.gameObject);
        }
    }

    public override void TakeDamage(float damage){
        base.TakeDamage(damage);
    }

    protected override void Die(){
        // Destroy Self and emit death explosion
        Instantiate(deathExplosion, transform.position, Quaternion.identity);
        if (deathObj != null)
        {
            GameObject deadObj = Instantiate(deathObj, transform.position, transform.rotation);
            foreach (Rigidbody rb in deadObj.GetComponentsInChildren<Rigidbody>())
            {
                //Add force to the rigid body
                rb.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                // Using the offset of the child from the parent, apply the appropriate velocity from the angular velocity
                rb.AddTorque(GetComponent<Rigidbody>().angularVelocity, ForceMode.VelocityChange);
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

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")){
            //Get the normal of the collision
            Vector3 normal = col.contacts[0].normal;
            //Get dot product of the normal and the velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            float dot = Vector3.Dot(rb.velocity.normalized, normal);
            
            //Debug.Log(dot);

            dot = Mathf.Clamp01(dot * 5);
            
            //Reduce health by a minimum of 1health, max of MaxLife based on dot
            int damage = (int)Mathf.Lerp(1,maxHealth, dot);

            TakeDamage(damage);
        }
    }

}
