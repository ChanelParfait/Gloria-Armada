using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpaceBoundary : MonoBehaviour
{
    
    Camera mainCamera;
    [SerializeField] BoxCollider boxCollider;

    Perspective pers;

    LevelManager levelManager;

    float height;
    float width;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(0, 0, 250);
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        UpdateBounds();
        boxCollider.size = new Vector3(width*0.9f, height*0.9f, width);
        //make a box that is width wide, height tall, and 1 unit deep centered at the camera
    }   

    // Update is called once per frame
    void Update()
    {
        UpdateBounds();
        boxCollider.size = new Vector3(width*0.9f, height*0.9f, width);
    }

    void UpdateBounds(){
        //With a perspective cam at -250 units on the z axis, find the bounds of the camera view using the camera's FOV
        height = 2.0f * Mathf.Tan(0.5f * mainCamera.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        width = height * mainCamera.aspect;  
    }

    void OnTriggerExit(Collider collision){
        if(collision.gameObject.tag == "Player"){
            collision.gameObject.GetComponent<Plane>().isOutOfBounds = true;
        }
    }
    void OnTriggerEnter(Collider collision){
        if(collision.gameObject.tag == "Player"){
            collision.gameObject.GetComponent<Plane>().isOutOfBounds = false;
        }
    }
}
