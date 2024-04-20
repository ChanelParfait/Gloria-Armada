using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBoundary : MonoBehaviour
{

    Camera mainCamera;
    [SerializeField] BoxCollider boxCollider;

    LevelManager levelManager;

    float height;
    float width;

    float colliderHeight = 200.0f;

    float maxHeight = 200.0f;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        maxHeight = levelManager.maxHeight;
        boxCollider.size = new Vector3(1000, colliderHeight, 1);
        
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
        float innerOffset = 0.1f * width;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + (height + colliderHeight)/2 - innerOffset, mainCamera.transform.position.z + 250);        
    }

    void OnTriggerStay(Collider collision){
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy"){
            Plane plane = collision.gameObject.GetComponent<Plane>();
            Rigidbody prb = collision.gameObject.GetComponent<Rigidbody>();
            // Add a force pushing the player towards 0,0 from this object's center in x
            float innerEdgeY = transform.position.y - colliderHeight/2;
            float relPos =  innerEdgeY - (plane.transform.position.y + 5);
            float positionSign = Mathf.Sign(relPos);
            if (prb.transform.position.y > maxHeight){
                prb.AddForce(new Vector3(0, positionSign*(float)Mathf.Pow(relPos, 2) - prb.velocity.y, 0));   
            }           
        }
    }
    // void OnTriggerExit(Collider collision){
    //     if(collision.gameObject.tag == "Player"){
            
    //     }
    // }
}
