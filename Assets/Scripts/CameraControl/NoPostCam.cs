using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPostCam : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        cam = GetComponent<Camera>();

        if (!mainCam)
        {
            Debug.LogError("No parent camera found");
        }
        cam.fieldOfView = mainCam.fieldOfView;
        
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView = mainCam.fieldOfView;
    }
}
