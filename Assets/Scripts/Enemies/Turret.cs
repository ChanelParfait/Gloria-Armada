using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBase
{
    public GameObject enemy;
    [SerializeField] GameObject aimPoint;
    [SerializeField] GameObject azimuthObj;
    [SerializeField] GameObject elevationObj;

    TurretMount turretMount;
    float offsetAngle = 0;

    [SerializeField] bool leadShot = true;
    [SerializeField] float overrideProjectileSpeed = 25.0f;
    [SerializeField] float elevationConstraintAngle = 45.0f;
    FieldOfView fov;
    [SerializeField] float rotationSpeed = 60.0f;

    public GameObject deathExplosion;

    // Start is called before the first frame update
    protected override void Start()
    {
        //If has parent && parent has turret mount script, get the turret mount script
        if (transform.parent != null){
            if (transform.parent.GetComponent<TurretMount>() != null){
                turretMount = transform.parent.GetComponent<TurretMount>();
            }
        }
        base.Start();
        fov = GetComponentInChildren<FieldOfView>(); //FOV is a child of "Gun Turret Body" - reflecting the actual aiming direction
        weaponManager = gameObject.GetComponentInChildren<EnemyWeaponManager>();
        TryGetComponent<Rigidbody>(out rb);
        if (rb == null)
        {
            rb = transform.root.GetComponent<Rigidbody>();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        if (enemy != null){
            AimpointToTarget();
            if (rb != null){
                weaponManager.FireActiveWeapon(rb.velocity);
            }
            else {
                weaponManager.FireActiveWeapon();
            }
            
        }
        else {
            AimpointReset();
            SearchForTarget();
        }
        LookAtTarget();  
    }

    void AimpointToTarget(){
        if (!leadShot){
            aimPoint.transform.position = enemy.transform.position;
            return;
        }
        float projectileSpeed;
        if (overrideProjectileSpeed > 0){
            projectileSpeed = overrideProjectileSpeed;
        }
        else {projectileSpeed = weaponManager.ActiveWeapon.GetProjectileStats().speed;}


        Vector3 relativePos = enemy.transform.position - transform.position;
        //Estimate time to target
        float timeToTarget = relativePos.magnitude / (2*projectileSpeed);
        //Predict position
        Vector3 targetPosition = enemy.transform.position + enemy.GetComponent<Rigidbody>().velocity * timeToTarget;
        //Set aimpoint to target position
        aimPoint.transform.position = targetPosition;
    }

    Vector3 AngleOffsetToPosition(){
        float dist = (aimPoint.transform.position - transform.position).magnitude;
        // Transform an offsetAngle into a position offset at the distance of the aimpoint (rotating around z)
        float x = dist * Mathf.Cos(offsetAngle * Mathf.Deg2Rad);
        float y = dist * Mathf.Sin(offsetAngle * Mathf.Deg2Rad);
        return new Vector3(x, y, 0);
    }

    void AimpointReset(){
        aimPoint.transform.position = transform.position + new Vector3(-50, 5, 0);
    }

    protected override void Die(){
        //Alert turretMount that this turret has died
        if (turretMount != null){
            turretMount.TurretDied();
        }
        // Destroy Self and emit death explosion
        Instantiate(deathExplosion, transform.position, Quaternion.identity);

        base.Die();
    }


    void SearchForTarget(){
        if (fov.visibleTargets.Count > 0)
        {
            // Check if the gameObject still exists
            if (fov.visibleTargets[0] != null)
            {
                enemy = fov.visibleTargets[0].gameObject;
            }
        }
    }


    void LookAtTarget(){
            
        // Determine the direction for azimuth and elevation
        Vector3 azimuthDirection = aimPoint.transform.position - azimuthObj.transform.position;

        // Constrain the direction to a single axis for azimuth
        azimuthDirection.y = 0;  // Keep azimuth flat on the horizontal plane
        
        // Compute the azimuth rotation
        Quaternion azimuthRotation = Quaternion.LookRotation(azimuthDirection,Vector3.up);

        float t = 10*rotationSpeed * Time.deltaTime;

        azimuthObj.transform.rotation = Quaternion.Slerp(
            azimuthObj.transform.rotation,
            azimuthRotation,
            Utilities.EaseInOut(t)
        );

        // For elevation, find the direction relative to the azimuth's current forward
        Vector3 elevationDirection = aimPoint.transform.position - elevationObj.transform.position;

        // Compute the elevation rotation based on the azimuth's current forward
        Quaternion elevationRotation = Quaternion.LookRotation(elevationDirection, azimuthObj.transform.forward);
        // Constrain elevation to pitch only (no yaw or roll changes)
        Vector3 constrainedElevation = elevationRotation.eulerAngles;
        constrainedElevation.y = 0;  // Remove yaw changes
        constrainedElevation.z = 0;  // Remove roll changes
        constrainedElevation.x = Mathf.Clamp(constrainedElevation.x,  270, 270 + elevationConstraintAngle);  // Constrain pitch changes


        Quaternion finalElevationRotation = Quaternion.Euler(constrainedElevation);

        elevationObj.transform.localRotation =Quaternion.Slerp(
            elevationObj.transform.localRotation,
            finalElevationRotation,
            Utilities.EaseInOut(t)
        );

    }

    IEnumerator arcSpray(){
        while (true){
            azimuthObj.transform.rotation = Quaternion.Euler(0, Mathf.Sin(Time.time) * 45, 0);
            yield return null;
        }
    }

}


