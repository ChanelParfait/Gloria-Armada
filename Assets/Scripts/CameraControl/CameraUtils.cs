using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour
{

    public Vector3 forwardEdge;
    public float width;
    public float height;

    public float forwardEdgeDistance;

    public Vector3 center;

    CameraDirector camDirector;
    LevelManager lm;


    // Start is called before the first frame update
    void Start()
    {
        camDirector = GameObject.FindObjectOfType<CameraDirector>();
        lm = GameObject.FindObjectOfType<LevelManager>();
        UpdateCameraDimensions();
        
    }

    public Vector2 UpdateCameraDimensions(){

        Camera cam = Camera.main;
        height = 2.0f * Mathf.Tan(0.5f * cam.fieldOfView * Mathf.Deg2Rad) * 250.0f;
        width = height * cam.aspect;  

        return new Vector2(width, height);
    }

    // Update is called once per frame
    void Update()
    {

        UpdateCameraDimensions();
        center = Camera.main.transform.position + Camera.main.transform.forward * transform.position.z;
        switch (lm.currentPerspective){
            case Perspective.Top_Down:
                forwardEdge = new Vector3(center.x + height/2, center.y, center.z);
                forwardEdgeDistance = height/2;
                break;
            case Perspective.Side_On:
                forwardEdge = new Vector3(center.x + width/2, center.y, center.z);
                forwardEdgeDistance = width/2;
                break;
            case Perspective.Null:
                forwardEdge = center;
                forwardEdgeDistance = 0;
                break;
        }

        
    }
}
