using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Perspective {Null = 0, Side_On = 1, Top_Down = 2};

public class LevelManager : MonoBehaviour
{
    // Keep track of level perspective and update all other objects 
    [SerializeField] Perspective initPerspective; 
    public Perspective currentPerspective { get; private set;} 
    [SerializeField] private Animator anim; 

    [SerializeField] private Enemy_Spawner enemySpawner;
    [SerializeField] private JetControl jetControl;

    [SerializeField] private GameObject playerPlane;

    [SerializeField] private GameObject gameOverPnl; 
    [SerializeField] private GameObject youWinPnl; 
    [SerializeField] private Text CurrentHealthTxt; 
    private int playerHealth; 

    Rigidbody rb;
    public float maxDistance = 5.0f; // Distance at which camera starts moving
    public float smoothTime = 0.1f; // Smoother transition time
    public float minHorizontalSpeed = 20.0f; // Minimum horizontal speed
    public float maxHorizontalSpeed = 75.0f; // Maximum horizontal speed
    public float minSpeedXOffset = 43f;
    public Vector3 velocity = Vector3.zero;

    public static event Action<int> OnPerspectiveChange;

    bool isGameOver = false;

    void Awake(){
        rb = GetComponent<Rigidbody>();
        currentPerspective = initPerspective;
    }

    // Player, Enemy Spawner, and Camera will all need to update when perspective changes 
    // Start is called before the first frame update
    void Start()
    {
        UpdatePerspective(initPerspective);
        enemySpawner.UpdatePerspective(currentPerspective);
        playerHealth = playerPlane.GetComponent<Plane>().health;

        //This is the minimum velocity to keep the player moving
        //rb.velocity = Vector3.right * 20;
    }

    void FixedUpdate(){
        // Calculate the current distance from the target to the camera's position
        
        //Modify X of target position based on rb velocity between minSpeed and maxSpeed
        float range = maxHorizontalSpeed - minHorizontalSpeed;
        minSpeedXOffset = -((playerPlane.GetComponent<Plane>().getRBVelocity().x - minHorizontalSpeed)/range - 1)* 43f;

        Vector3 targetPosition = playerPlane.transform.position + new Vector3(minSpeedXOffset, 0, 0);   
        Vector3 cameraPosition = transform.position;
        Vector3 offset = targetPosition - cameraPosition;

        // Calculate the desired speed based on the horizontal offset
        float desiredSpeed = offset.x / Time.fixedDeltaTime;
        
        // Clamp the speed to ensure it's between min and max speeds
        float clampedSpeed = Mathf.Clamp(desiredSpeed, minHorizontalSpeed, maxHorizontalSpeed);

        // Calculate how much to move per fixed frame based on the clamped speed
        float horizontalMove = clampedSpeed * Time.fixedDeltaTime;

        // Calculate the new position to move towards using smooth damping
        Vector3 targetCameraPos = new Vector3(cameraPosition.x + horizontalMove, cameraPosition.y, cameraPosition.z);
        // Update the camera position
        transform.position = targetCameraPos;

    //     // If the player is not in the center of the rigidbody, accel the rigidbody towards the player
    //     // Proportional to how far away the player is from the center
    //     Vector3 playerPos = playerPlane.transform.position;
    //     Vector3 rbPos = transform.position;
    //     Vector3 diff = playerPos - rbPos;
    //     float dist = diff.magnitude;
    //     if (rb.velocity.x < minSpeed)
    //     {
    //         rb.velocity = Vector3.right * minSpeed;
    //     }
    //     else if (rb.velocity.x >= minSpeed)
    //     {
    //         rb.AddForce(diff.x * Vector3.right * 5);
    //     }

    }

    // Update is called once per frame
    void Update()
    {
        // if(jetControl.isDead && !isGameOver){
        //     GameOver();
        //     isGameOver = true;
        // }




        if(isGameOver){
            if(Input.GetKeyDown(KeyCode.Return)){
                Restart();
            }
        }

        // if(playerHealth != jetControl.health){
        //     playerHealth = jetControl.health;
        //     UpdateHealthTxt(playerHealth.ToString());
        // }
    }

    private void OnTriggerEnter(Collider col){
        if(col.tag == "TransitionPoint"){
            UpdatePerspective(col.GetComponent<TransitionPoint>().GetPerspective());
        }
        if(col.tag == "WinPoint"){
            YouWin();
        }
    }

    private void UpdatePerspective(Perspective pers){
        currentPerspective = pers; 
        anim.SetInteger("Perspective", (int)currentPerspective);

        enemySpawner.UpdatePerspective(currentPerspective); 
        //jetControl.ResetPosition(5f);
        //Invoke action to update others without storing references to all objects
        OnPerspectiveChange?.Invoke((int)currentPerspective);
    }

    private void UpdateHealthTxt(string health){
        CurrentHealthTxt.text = health;
    }

    private void GameOver(){
        //Debug.Log("Game Over");
        gameOverPnl.SetActive(true);
        Time.timeScale = 0; 
    }

    private void YouWin(){
        youWinPnl.SetActive(true);
        Time.timeScale = 0; 
    }

    private void Restart(){
        SceneManager.LoadScene(1);
        Time.timeScale = 1; 
    }

}
