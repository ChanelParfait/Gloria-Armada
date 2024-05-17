using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Split : MonoBehaviour
{
    //How many projectiles the projectile will split into
    public int splitCount = 4;

    //The total arc of the split in degrees
    public float spreadAngle = 45;

    //The projectile to be split   
    public GameObject projectileObj;
    Projectile projectile;

    //Time before the projectile splits
    public float splitTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        projectileObj = gameObject;
        projectile = projectileObj.GetComponent<Projectile>();
        StartCoroutine(SplitProjectile());
    }

    IEnumerator SplitProjectile()
    {
        yield return new WaitForSeconds(splitTime);

        //Rotate the parent object such that z is zero
        transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

        for (int i = 0; i < splitCount; i++)
        {
            GameObject splitProjectileObj = Instantiate(projectileObj, transform.position, transform.rotation);
            
            Vector3 parentRotation = transform.rotation.eulerAngles;
            splitProjectileObj.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(parentRotation.x + (spreadAngle * (i - splitCount/2)), parentRotation.y, 0));
            splitProjectileObj.GetComponent<Projectile>().Launch(projectile.GetStats(), GetComponent<Rigidbody>().velocity);
            //remove the split component from the split projectile
            Destroy(splitProjectileObj.GetComponent<Split>());
        }
        //Destroy the parent projectile (this)
        Destroy(gameObject);
    }
}
