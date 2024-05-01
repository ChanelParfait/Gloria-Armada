using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private Camera mainCam;

    GameObject wreck;

    [SerializeField] bool deathSequence = false; 

    private void OnEnable() {
        PlayerPlane.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDisable() {
        PlayerPlane.OnPlayerDeath -= OnPlayerDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player == null){
           player = GameObject.FindGameObjectWithTag("Player");
        }
        if (mainCam == null){
            mainCam = Camera.main;
        }
        
    }

    private void OnPlayerDeath(){
    //     wreck = GameObject.FindGameObjectWithTag("PlayerWreckage");
    //     //Pick a random fragment from the children of the child of the
    //     Debug.Log("Player Wreckage: " + wreck + " Main Camera: " + mainCam);
    //     if (wreck && mainCam){
    //         deathTime = Time.time;
    //         StartCoroutine(zoomIn(15, 0.5f));
    //         StartCoroutine(lookAtWreck());
    //         deathSequence = true;
    //     }
    }

    // IEnumerator zoomIn(float newFOV, float duration = 0.5f){
    //     float startFOV = mainCam.fieldOfView;
    //     float time = 0;
    //     while (time < duration){
    //         float t = time/duration;
    //         mainCam.fieldOfView = Mathf.LerpUnclamped(startFOV, newFOV, Utilities.EaseInOutBack(t));
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     mainCam.fieldOfView = newFOV;
    // }

    // IEnumerator lookAtWreck(float duration = 2f){
    //     Vector3 target = wreck.transform.position;
    //     Vector3 direction = target - transform.position;
    //     Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

    //     float time = 0;
    //     while (time < duration){
    //         float t = time/duration;
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Utilities.EaseInOutBack(t));
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     StartCoroutine(lookAtWreck());
    //     transform.rotation = targetRotation;
    // }

    // Update is called once per frame
    void Update()
    {
    }
}
