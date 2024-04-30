using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBase
{
    public GameObject enemy;
    [SerializeField] GameObject aimPoint;
    [SerializeField] GameObject azimuthObj;
    [SerializeField] GameObject elevationObj;
    EnemyWeaponManager weaponManager; 
    [SerializeField] private float fireInterval = 3;
    private float timer = 0;

    FieldOfView fov;

    
    [SerializeField] float rotationSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        fov = GetComponentInChildren<FieldOfView>(); //FOV is a child of "Gun Turret Body" - reflecting the actual aiming direction
        weaponManager = gameObject.GetComponentInChildren<EnemyWeaponManager>();

    }
    // Update is called once per frame
    void Update()
    {
        
        if (enemy != null){
            AimpointToTarget();
        }
        else {
            AimpointReset();
            SearchForTarget();
        }
        LookAtTarget();
        timer += Time.deltaTime; 
        if(timer >= fireInterval){
            timer = 0; 
            Fire();
        }
        
    }

    void AimpointToTarget(){
        //get projectile velocity from weapon manager
        float projectileSpeed = weaponManager.ActiveWeapon.GetProjectileStats().speed;
        aimPoint.transform.position = Utilities.FirstOrderIntercept(azimuthObj.transform.position, Vector3.zero, projectileSpeed, enemy.transform.position, enemy.GetComponent<Rigidbody>().velocity);
    }

    void AimpointReset(){
        aimPoint.transform.position = transform.position + new Vector3(-50, 50, 0);
    }


    void SearchForTarget(){
        // Smoothly rotate the turret back and forth in the X axis to scan for targets by moving aimPoint in an arc
        if (fov.visibleTargets.Count > 0)
        {
            // Check if the gameObject still exists
            if (fov.visibleTargets[0] != null)
            {
                enemy = fov.visibleTargets[0].gameObject;
            }
        }
    }

    public override void Fire(){
        weaponManager.FireActiveWeapon();
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
            EaseInOut(t)
        );
        // For elevation, find the direction relative to the azimuth's current forward
        Vector3 elevationDirection = aimPoint.transform.position - elevationObj.transform.position;

        // Compute the elevation rotation based on the azimuth's current forward
        Quaternion elevationRotation = Quaternion.LookRotation(elevationDirection, azimuthObj.transform.forward);
        // Constrain elevation to pitch only (no yaw or roll changes)
        Vector3 constrainedElevation = elevationRotation.eulerAngles;
        constrainedElevation.y = 0;  // Remove yaw changes
        constrainedElevation.z = 0;  // Remove roll changes

        Quaternion finalElevationRotation = Quaternion.Euler(constrainedElevation);

        elevationObj.transform.localRotation =Quaternion.Slerp(
            elevationObj.transform.localRotation,
            finalElevationRotation,
            EaseInOut(t)
        );
    }

    static float Flip(float x)
    {
        return 1 - x;
    }

    public static float EaseIn(float t)
    {
        return t * t;
    }

    public static float EaseOut(float t)
    {
        return Flip(Mathf.Pow(Flip(t),2));
    }         

     public static float EaseInOut(float t)
    {
        return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
    }

}


