using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoundary : MonoBehaviour
{

    Camera mainCamera;
    [SerializeField] BoxCollider boxCollider;

    LevelManager levelManager;

    float height;
    float width;

    float colliderWidth = 100.0f;

    float maxSpeedX = 75.0f;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        maxSpeedX = levelManager.maxHorizontalSpeed;
        boxCollider.size = new Vector3(colliderWidth, 1000, 1);
        
    }

    private void UpdateBounds(){
        //With a perspective cam at -250 units on the z axis, find the bounds of the camera view using the camera's FOV
        height = 2.0f * Mathf.Tan(0.5f * mainCamera.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        width = height * mainCamera.aspect;  
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBounds();
        //Set the position to be the rightMost edge of the bounds + the width of the collider
        transform.position = new Vector3(mainCamera.transform.position.x + (width + colliderWidth)/2 - 40f, mainCamera.transform.position.y, mainCamera.transform.position.z + 250);        
    }

    void OnTriggerStay(Collider collision){
        if(collision.gameObject.tag == "Player"){
            Plane plane = collision.gameObject.GetComponent<Plane>();
            Rigidbody prb = collision.gameObject.GetComponent<Rigidbody>();
            // Add a force pushing the player towards 0,0 from this object's center in x
            float innerEdgeX = transform.position.x - colliderWidth/2;
            Vector3 relPosCollider = mainCamera.transform.position - transform.position;
            float relPos =  innerEdgeX - (plane.transform.position.x + 20);
            float positionSign = Mathf.Sign(relPos);
            // float damping = plane.getRBVelocity().y - rb.velocity.y;
            // damping *= damping*Mathf.Sign(damping);
            if (prb.velocity.x > maxSpeedX - 10)
            {
                prb.AddForce(new Vector3(positionSign * (float)Mathf.Pow(relPos, 2) + prb.velocity.x, 0, 0));
            }
        

            
        }
    }
    // void OnTriggerExit(Collider collision){
    //     if(collision.gameObject.tag == "Player"){
            
    //     }
    // }
}
