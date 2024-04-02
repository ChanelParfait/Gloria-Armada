using System.Collections;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    // Player Shooting
    [SerializeField] GameObject projectile; 

    // Player Movement
    [SerializeField] float aerodynamicEfficiency = 1f;
    [SerializeField] float aerodynamicDrag = 1f;
    [SerializeField] float wingspan = 10;
    [SerializeField] float negativeGLimit = -20;

    // Player Health / Life
    public bool isDead = false; 


    public void ResetPosition(float duration) {
        StartCoroutine(ResetPosition_async(duration));
    }
    IEnumerator ResetPosition_async(float duration) {
        Vector3 target = Vector3.zero;
        float elapsedTime = 0;
        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float completion = elapsedTime / duration;
            virtualCursor = Vector2.Lerp(virtualCursor, new Vector2(0, 0), completion);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, completion);
            yield return null;
        }
    }

    public float turnMultiplier = 0.2f;
    public float thrustMultiplier = 0.1f;

    [SerializeField] Transform viewport;
    Rigidbody rb;
    Vector3 lastViewportPosition;

    void Start() {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        lastViewportPosition = viewport.position;
    }

        // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            Shoot();
        }
    }


    Vector3 bakedVelocity = new Vector3(1, 0, 0) * 800;
    Vector2 virtualCursor = Vector2.right * 5; //could implement better
    void FixedUpdate() {
        virtualCursor.x += Input.GetAxis("Mouse X");
        virtualCursor.y += Input.GetAxis("Mouse Y");

        //again, could implement better with perhaps a 3D cursor but this is fine for now
        Vector3 targetDirection = viewport.localRotation * virtualCursor;
        Debug.DrawRay(transform.localPosition, targetDirection);

        //We should probably do something about this

        Vector3 velocity = bakedVelocity + rb.velocity;

        //split the inputs
        float axisInput = Vector3.Dot(targetDirection.normalized, bakedVelocity.normalized);  //-1=reverseThrust, 1=thrusts, 0=turning
        Vector3 sideAxis = Vector3.Cross(-bakedVelocity.normalized, -viewport.forward);
        float axialInput = Vector3.Dot(targetDirection.normalized, sideAxis); //-1=left, 1,right, 0=neutral

        //visual rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnMultiplier);

        //for calculating aerodynamic forces`
        float slipAngle = Quaternion.Angle(rb.rotation, rotation);
        float referenceArea = Mathf.Sin(slipAngle) * wingspan;

        float drag = velocity.magnitude * referenceArea * aerodynamicDrag; //simplified drag equation
        float normal = Mathf.Pow(velocity.magnitude, 2) * referenceArea * aerodynamicEfficiency; //simplified lift equation
        Vector3 normalAccel = sideAxis * axialInput * normal;

        //Could enforce g force limits - e.g. 10G up (KO), 2G down (pop!)
        #region
        if(normalAccel.sqrMagnitude < negativeGLimit) {
            Quaternion upsideDown = Quaternion.LookRotation(transform.forward, -transform.up);
            //rb.rotation = Quaternion.Slerp(rb.rotation, upsideDown, turn);
        }
        else if (normalAccel.sqrMagnitude > 0){
            Quaternion upside = Quaternion.LookRotation(transform.forward, transform.up);
            //rb.rotation = Quaternion.Slerp(rb.rotation, upside, turn);
        }
        #endregion

        //apply rotation here in case we clamp based on normalAccel or slipAngle
        rb.MoveRotation(rotation);

        rb.AddForce(normalAccel);
    }

    void Shoot(){
        // get the position 4 units in front of the enemy 
        Vector3 spawnPosition = gameObject.transform.position + gameObject.transform.forward * 8;
        Instantiate(projectile, spawnPosition, gameObject.transform.rotation); 
    }

    private void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "EnemyProjectile"){
            // Take Damage? / Die
            isDead = true; 
        }
    }
}








