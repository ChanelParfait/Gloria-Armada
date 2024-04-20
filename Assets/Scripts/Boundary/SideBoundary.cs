using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class SideBoundary : MonoBehaviour
{

    [SerializeField] Vector3 orientation = Vector3.right;

    Camera mainCamera;
    [SerializeField] BoxCollider boxCollider;

    Perspective pers;

    LevelManager levelManager;

    float height;
    float width;

    float colliderWidth = 200.0f;

    float maxSpeedX = 75.0f;

    float zOffset = 250.0f; 

    void OnEnable(){
        LevelManager.OnPerspectiveChange += UpdateOrientation;
    }

    void OnDisable(){
        LevelManager.OnPerspectiveChange -= UpdateOrientation;
    }

    void UpdateOrientation(int newPerspective){
        pers = (Perspective)newPerspective;
        if(newPerspective == (int)Perspective.Side_On){
            zOffset = 250f;
        }
        else if(newPerspective == 2){
            zOffset = 0;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        maxSpeedX = levelManager.maxHorizontalSpeed;
        boxCollider.size = new Vector3(colliderWidth, 1000, colliderWidth);
        pers = levelManager.currentPerspective;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateBounds();
        float innerOffset;

        Vector3 positions = new Vector3(width, 0, width);
        if (pers == Perspective.Top_Down){
            positions = new Vector3(height, 0, width);
        }

        if (Mathf.Abs(orientation.x) > 0){
            innerOffset = 0.2f * width;
            boxCollider.size = new Vector3(innerOffset, 1000, colliderWidth);
        }
        else if (Mathf.Abs(orientation.z) > 0){
            innerOffset = 0.2f * width;
            boxCollider.size = new Vector3(colliderWidth, 1000, innerOffset);
        }
        
        transform.position = new Vector3(mainCamera.transform.position.x + (positions.x/2 - boxCollider.size.x) * orientation.x,
                                        mainCamera.transform.position.y,
                                        mainCamera.transform.position.z + zOffset + (positions.z/2 - boxCollider.size.z) * orientation.z);        
    }


    private void UpdateBounds(){
        //With a perspective cam at -250 units on the z axis, find the bounds of the camera view using the camera's FOV
        height = 2.0f * Mathf.Tan(0.5f * mainCamera.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        width = height * mainCamera.aspect;  
    }

    void OnTriggerStay(Collider collision){
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy"){
            Plane plane = collision.gameObject.GetComponent<Plane>();
            Rigidbody prb = collision.gameObject.GetComponent<Rigidbody>();
            // Add a force that pushes the player away from the outer edge (orientation)
            Vector3 outerEdge = transform.position + Utilities.MultiplyComponents(boxCollider.size/2 + 5*orientation, orientation);
            // inverse exponent of proximity to outer edge
            float relPos = Vector3.Dot(outerEdge - plane.transform.position, orientation);
            Debug.DrawLine(plane.transform.position, outerEdge, Color.red);
            Vector3 force = 1/Mathf.Pow(relPos, 2) * 10 * -orientation;
            Debug.DrawRay(plane.transform.position, force, Color.green);
            Debug.Log("Force Applied: " + force);
            force = Utilities.ClampVec3(force, -prb.velocity, prb.velocity);
            prb.AddForce(force, ForceMode.VelocityChange);
        }
    }
    // void OnTriggerExit(Collider collision){
    //     if(collision.gameObject.tag == "Player"){
            
    //     }
    // }
}
